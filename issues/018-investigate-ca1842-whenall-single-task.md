## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1842 ("Do not use 'WhenAll' with a single task") to determine why it produces
no diagnostic in build SARIF, then either fix the test so it passes or update the `Untestable`
note with a confirmed, well-sourced reason.

**Current state:** `ProhibitWhenAllWithSingleTask` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
Every pattern tried (`Task.WhenAll(Task.FromResult(42))`, generic `Task<T>`, non-generic
`Task`, `await` form, `return` form) produced an empty SARIF or unrelated diagnostics —
`CA1842` never appeared.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1802, CA1842, CA1843, CA1853, and CA1870 were all observed to produce no diagnostic in
build SARIF. The prime suspect is `EnforceOnBuild = Never` or `CustomTags.NotConfigurable`
in the rule descriptor.

## Investigation plan

1. **Check `EnforceOnBuild` metadata in NetAnalyzers source.**
   Browse the `dotnet/roslyn-analyzers` GitHub repo. Find
   `DoNotUseWhenAllWithSingleTaskAnalyzer.cs` (or similar). Check:
   - Whether the rule descriptor includes `CustomTags.NotConfigurable` or `WellKnownDiagnosticTags.Telemetry`
   - Whether `DiagnosticAnalyzer` is decorated with `[DiagnosticAnalyzer(LanguageNames.CSharp)]`
     and whether it registers a symbol/syntax action that fires during build

2. **Check the NetAnalyzers 10.0.x release notes / changelog for CA1842.**
   The rule may have been intentionally disabled for build-mode enforcement in a recent release.
   Search https://github.com/dotnet/roslyn-analyzers/releases for "CA1842".

3. **Try explicit `dotnet build` with SARIF capture outside the test harness.**
   Create a minimal `csproj` with `<Nullable>enable</Nullable>` and `<ErrorLog>` pointing to a
   `.sarif` file, add the violation directly, and run `dotnet build` manually. Inspect the raw
   SARIF to confirm zero CA1842 entries (vs. a harness-specific issue).

4. **Try `Task.WhenAll` with `IEnumerable<Task>` wrapping a single task.**
   The rule documentation says it covers both the `params Task[]` and `IEnumerable<Task>`
   overloads. Try:
   ```csharp
   IEnumerable<Task> tasks = new[] { Task.CompletedTask };
   await Task.WhenAll(tasks);
   ```
   Also try `new List<Task> { Task.CompletedTask }` to rule out array-specific suppression.

5. **Try `ValueTask.WhenAll` (introduced in .NET 9).**
   If the analyzer was extended to cover `ValueTask.WhenAll`, that overload might fire where
   `Task.WhenAll` does not.

6. **Test on NetAnalyzers 8.x.**
   Add `<PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.*" />` to a
   scratch project and check whether CA1842 fires with its own ID.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1842", "Do not use 'WhenAll' with a single task",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1842",
    Untestable = "CA1842 produces no diagnostic in build SARIF for any Task.WhenAll(singleTask) pattern (generic Task<T>, non-generic Task, await or return form); the SARIF is empty even with clean code and dotnet_diagnostic.CA1842.severity = warning configured")]
public async Task ProhibitWhenAllWithSingleTask()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        namespace test;

        public static class Program
        {
            public static async Task M()
            {
                await Task.WhenAll(Task.FromResult(42));
            }

            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1842").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause identified: confirmed whether CA1842 is suppressed in build mode (`EnforceOnBuild`)
      or is absent from SARIF for another reason
- [ ] One of:
  - [ ] A violation pattern found that triggers `error:CA1842` in SARIF → test updated to use
        that pattern, `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1842 in build SARIF → `Untestable` reason updated
        with the confirmed root cause (e.g. linked NetAnalyzers GitHub issue URL or source
        location showing `EnforceOnBuild = Never`)
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
