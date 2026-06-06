## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1843 ("Do not use 'WaitAll' with a single task") to determine why it produces
no diagnostic in build SARIF, then either fix the test so it passes or update the `Untestable`
note with a confirmed, well-sourced reason.

**Current state:** `ProhibitWaitAllWithSingleTask` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
Every pattern tried (`Task.WaitAll(Task.CompletedTask)`, `Task.WaitAll(Task.FromResult(0))`,
generic and non-generic forms) produced an empty SARIF — `CA1843` never appeared.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1843 is in the same "absent from SARIF" failure group as CA1842, CA1802, CA1853, CA1870.
They all produce zero CA-prefixed diagnostics in build analysis despite being configured as
`dotnet_diagnostic.CA1843.severity = warning` with `TreatWarningsAsErrors=true`.

**Note:** CA1843 is the blocking (`WaitAll`) counterpart to CA1842 (`WhenAll`). Because
`Task.WaitAll` is a synchronous, blocking call it is more dangerous than `WhenAll`. The fact
that this rule doesn't fire at all — while its sister CA1842 also doesn't fire — strongly
suggests a shared root cause: both rules may be excluded from `EnforceOnBuild`.

## Investigation plan

1. **Check `EnforceOnBuild` metadata in NetAnalyzers source.**
   In the `dotnet/roslyn-analyzers` repo, find the analyzer for CA1843. It may share a file
   with CA1842 (e.g. `DoNotUseWhenOrWaitAllWithSingleTaskAnalyzer.cs`). Confirm:
   - Whether the `DiagnosticDescriptor` for CA1843 has `isEnabledByDefault: false`
   - Whether it carries `CustomTags.NotConfigurable` or is excluded from `EnforceOnBuild`

2. **Check for a .NET 10-specific suppression.**
   In .NET 9+, `Task.WaitAll(Task)` may be accompanied by a new overload
   `Task.WaitAll(params ReadOnlySpan<Task>)` that changes the semantics. Confirm whether
   the rule targets the specific `Task.WaitAll(Task[])` overload and whether the compiler
   selects a different overload in .NET 10.

3. **Try `Task.WaitAll` with an explicit `Task[]` cast.**
   Force the compiler to select the array overload:
   ```csharp
   Task.WaitAll(new Task[] { Task.CompletedTask });
   ```
   This rules out an overload-resolution issue where the single-arg call resolves to a
   different overload than the analyzer expects.

4. **Try `Task.WaitAll` with a `CancellationToken` (overload added in .NET 6).**
   ```csharp
   Task.WaitAll(new[] { Task.CompletedTask }, CancellationToken.None);
   ```
   If the rule only targets the original overload(s), a newer overload may have been missed.

5. **Test on NetAnalyzers 8.x.**
   Confirm whether CA1843 fired in an earlier version of the analyzer package.

6. **Confirm CA1842 shares the same root cause.**
   Both rules likely have the same fix: if one is testable via a workaround, the other
   probably is too. Coordinate findings with `issues/018-investigate-ca1842-whenall-single-task.md`.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1843", "Do not use 'WaitAll' with a single task",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1843",
    Untestable = "CA1843 produces no diagnostic in build SARIF for any Task.WaitAll(singleTask) pattern (generic Task<T>, non-generic Task, expression-body or statement form); the SARIF is empty even with clean code and dotnet_diagnostic.CA1843.severity = warning configured")]
public async Task ProhibitWaitAllWithSingleTask()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        namespace test;

        public static class Program
        {
            public static void M()
            {
                var t = Task.CompletedTask;
                Task.WaitAll(t);
            }

            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1843").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause identified: confirmed whether CA1843 is suppressed in build mode (`EnforceOnBuild`)
      or absent for another reason (e.g. overload-resolution, .NET 10 API change)
- [ ] One of:
  - [ ] A violation pattern found that triggers `error:CA1843` in SARIF → test updated,
        `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1843 in build SARIF → `Untestable` reason updated
        with the confirmed root cause (linked source or GitHub issue)
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Cross-issue note (added 2026-06-06 while closing issue 018)

Root cause and fix are **confirmed and identical to CA1842 (issue 018)** — apply the same
solution here. The investigation-plan hypotheses in steps 2 and 4 were correct.

The analyzer (`DoNotUseWhenAllOrWaitAllWithSingleArgument`) only reports CA1843 when the bare
single-task call binds to the `params Task[]` overload **and** the compiler synthesises an
*implicit* single-element `Task[]` (`IsSingleTaskArgument` checks for an implicit
`IArrayCreationOperation`). .NET 9 added `Task.WaitAll(params ReadOnlySpan<Task>)`, and under
C# 13's params-collections overload resolution a bare `Task.WaitAll(t)` binds to the **span**
overload, which produces no implicit array — so the analyzer never matches. CA1843 is therefore
absent on the default modern toolchain, not because of `EnforceOnBuild = Never`.

**Fix:** build the violation project with `<LangVersion>12</LangVersion>` (pass
`CreateProjectBuilder(properties: [("LangVersion", "12")])`). During issue 018's probe,
`Task.WaitAll(t)` (with `var t = Task.CompletedTask;`) emitted `error:CA1843` under LangVersion 12.
Un-skip `ProhibitWaitAllWithSingleTask`, remove the `Untestable` reason from its `[RuleDoc]`, and
assert `HasError("CA1843")`. Keep the real-world caveat in mind: CA1843 will not catch a bare
single-task `WaitAll` in default C# 13+ user code (span-overload gap upstream).

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
