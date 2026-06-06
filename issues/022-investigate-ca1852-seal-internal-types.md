## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1852 ("Seal internal types") to determine why it produces `IDE0055` instead of
`CA1852` in build SARIF, then either fix the test so it passes or update the `Untestable`
note with a confirmed, well-sourced reason.

**Current state:** `SealInternalTypes` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
Every pattern tried (`internal class InternalService { }`, with or without members, in a
separate file) produced `IDE0055` ("Fix formatting") at the class declaration line instead
of `error:CA1852`.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1826 and CA1852 are confirmed to route through IDE0055 in the same way as the IDE-style
rules (IDE0260, IDE0070, IDE0079). The formatter-backed diagnostic fires at `(N, 1)` — the
first line of the type declaration — never at the unsealed class declaration itself. CA1852
was grouped with CA1061, CA1511, and CA1826 as "CA-prefix rules that also route through IDE0055."

## Investigation plan

1. **Confirm the current test setup targets an actually-unsealed internal class.**
   The current test uses a two-file project (`InternalService.cs` + `Program.cs`). Ensure
   `InternalService` has no subclasses anywhere in the compilation — if a subclass exists
   (e.g. a nested class or a derived class in Program.cs), the rule correctly does NOT fire.

2. **Try adding a public method to the internal class.**
   Some configurations of CA1852 require at least one accessible member for the rule to
   suggest sealing is safe. Test:
   ```csharp
   internal class InternalService
   {
       public void DoWork() { }
   }
   ```
   vs. the empty class version already tried.

3. **Suppress IDE0055 to see if CA1852 appears independently.**
   Add `dotnet_diagnostic.IDE0055.severity = none` via `additionalFiles` in
   `CreateProjectBuilder` and rebuild. If CA1852 appears once IDE0055 is suppressed, the rule
   is testable with a project-level IDE0055 suppression in the test.

4. **Try with `#pragma warning disable IDE0055` around the class declaration.**
   Unlike suggestion-level diagnostics, IDE0055 is typically a `warning` severity. Pragma
   suppression of warnings in build mode is normally honoured. Test:
   ```csharp
   #pragma warning disable IDE0055
   internal class InternalService { }
   #pragma warning restore IDE0055
   ```

5. **Check the NetAnalyzers source for CA1852's analyzer implementation.**
   Browse `dotnet/roslyn-analyzers` for `TypeCanBeSealedAnalyzer.cs` (or similar). Look for:
   - Whether it registers via `SymbolKind.NamedType` or a formatting operation
   - Whether it has a code-fix that hooks into the Roslyn formatter, which would cause it to
     appear as IDE0055 in build mode (same mechanism as CA1511/CA1826)
   - Whether `EnforceOnBuild` is set to `Never` or `WhenExplicitlyConfigured`

6. **Test on NetAnalyzers 8.x.**
   Confirm whether CA1852 fired with its own ID in an older version.

7. **Try a multi-level inheritance scenario.**
   CA1852 may have different triggering logic when the class has both a base class and
   is not subclassed. Try an internal class that extends a public abstract class:
   ```csharp
   public abstract class Base { }
   internal class Derived : Base { }
   ```

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1852", "Seal internal types",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852",
    Untestable = "CA1852 is formatter-backed: unsealed internal class patterns produce IDE0055 (Fix formatting) in SARIF instead of CA1852 in NetAnalyzers 10.0.x; CA1852 cannot be triggered with its own diagnostic ID in build analysis regardless of whether the class has members, is in a separate file, or has other variations")]
public async Task SealInternalTypes()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile("InternalService.cs",
        """
        namespace test;
        internal class InternalService { }
        """);
    await project.AddFile(
        "Program.cs",
        """
        namespace test;
        public static class Program { public static int Main() => 0; }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1852").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause identified: confirmed whether CA1852 uses the formatter pipeline in `EnforceOnBuild`
      mode (same as IDE-style rules) or has `EnforceOnBuild = Never`
- [ ] One of:
  - [ ] A violation pattern found (or workaround like suppressing IDE0055) that triggers
        `error:CA1852` in SARIF → test updated, `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1852 in build SARIF → `Untestable` reason updated
        with the confirmed root cause (e.g. linked NetAnalyzers GitHub issue URL)
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately. Findings from
`issues/017-investigate-ca1826-enumerable-on-indexable.md` (same IDE0055 formatter-backed
failure mode) directly inform steps 3 and 5 above.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
