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

## Resolution (2026-06-06)

**CA1852 is now a tested rule.** The "formatter-backed / routes through IDE0055" hypothesis
was wrong — it was an instance of the confounder flagged when closing issue 017 (CA1826):
the original probe's `internal class InternalService { }` puts the opening brace on the same
line, violating `csharp_new_line_before_open_brace`, so IDE0055 ("Fix formatting") fired and
was the *only* diagnostic — misread as "CA1852 routing through the formatter." With a
well-formatted class, IDE0055 disappears entirely and is **not** how CA1852 behaves.

### Real root cause: the package's own `InternalsVisibleTo` injection

CA1852 is a normal NetAnalyzers quality rule (`SealInternalTypes.cs`,
`RuleLevel.IdeHidden_BulkConfigurable` → default severity Hidden, **not** `EnforceOnBuild = Never`).
It reports its own ID at command-line `dotnet build` once severity is raised — which the package
editorconfig does (`dotnet_diagnostic.CA1852.severity = warning`).

The reason it never fired in the harness: **CA1852 suppresses itself for the entire compilation
when the assembly carries `[assembly: InternalsVisibleTo(...)]`** (a friend assembly could subclass
the internal type). And `build/Opinionated.DotNet.CodingStandards.targets` (lines 81–91)
auto-injects `[assembly: InternalsVisibleTo("<ProjectName>.Tests")]` into every project whose name
does **not** end in `Tests`. The harness project is `test.csproj` → `InternalsVisibleTo` is always
added → CA1852 is suppressed.

Verified by scratch-project probe matrix (`dotnet build` + SARIF, package v999.9.9):

| Probe | Result |
|-------|--------|
| `internal class InternalService { }` (brace same line), used | only `error:IDE0055` (formatting) — the original confounder |
| well-formatted unsealed internal class, unused | only `note:MA0182` (unused), no CA1852 |
| well-formatted unsealed internal class, used (Exe) | **0 diagnostics** — suppressed by injected `InternalsVisibleTo` |
| same, `OutputType=Library` | 0 diagnostics — not Exe-vs-Library |
| same + `.editorconfig` `dotnet_code_quality.CA1852.ignore_internalsvisibleto = true` | **single `error:CA1852`** ✓ |

### Fix applied

Un-skipped `SealInternalTypes` in `CodeAnalysisRulesPerformanceModernShould.cs`; removed the
`Untestable` reason; the test now adds a discovered `.editorconfig` with the documented
`dotnet_code_quality.CA1852.ignore_internalsvisibleto = true` option (available since .NET 8) so
the rule fires despite the package-injected friend-assembly attribute, then asserts
`HasError("CA1852")`. Test passes in the harness.

### Note for sibling issues 025 (CA1061) and 028 (CA1511)

The prior session (issue 017) grouped CA1061/CA1511/CA1852 with CA1826 as "CA-prefix rules that
route through IDE0055." That grouping is now confirmed unreliable for CA1852: its real cause was
`InternalsVisibleTo`, unrelated to the formatter. CA1061 and CA1511 have *different* suspected
causes (CS0109 compiler preemption / VB-only semantics for CA1061; a separate confounder for
CA1511) and must each be re-probed independently with well-formatted code before being accepted
as untestable.

### Sources

- Analyzer source: `dotnet/roslyn-analyzers` `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Runtime/SealInternalTypes.cs`
- Option docs: https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852 (`dotnet_code_quality.CAxxxx.ignore_internalsvisibleto`)
- Related issues: dotnet/roslyn-analyzers #7511 (CA1852 fires at CLI build), #6438 (`InternalsVisibleTo` disables CA1852), #6141/PR #6278 (top-level-statements `Program` skip)

## Acceptance criteria

- [x] Root cause identified: CA1852 is a normal quality rule (not formatter-backed, not
      `EnforceOnBuild = Never`); it is suppressed because the package injects
      `[assembly: InternalsVisibleTo("<project>.Tests")]` and CA1852 ignores types exposed to a
      friend assembly by default
- [x] A violation pattern found (opt back in via `dotnet_code_quality.CA1852.ignore_internalsvisibleto = true`)
      that triggers `error:CA1852` in SARIF → test updated, `Skip` removed, test passes in CI
- [x] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [x] `RuleReferenceGenerator` coverage test continues to pass (CA1852 still has exactly one
      method-level `[RuleDoc]`, now with `Untestable == null`); `docs/rule-reference.md` regenerates
      with no diff

## Blocked by

None — can start immediately. Findings from
`issues/017-investigate-ca1826-enumerable-on-indexable.md` (same IDE0055 formatter-backed
failure mode) directly inform steps 3 and 5 above.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
