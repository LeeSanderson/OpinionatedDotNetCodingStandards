## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1826 ("Do not use Enumerable methods on indexable collections") to determine
why it produces `IDE0055` instead of `CA1826` in build SARIF, then either fix the test so it
passes or update the `Untestable` note with a confirmed, well-sourced reason.

**Current state:** `ProhibitEnumerableMethodsOnIndexableCollections` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`. Every
pattern tried (`list.First()`, `arr.Last()`, `list.ElementAt(n)`) produced `IDE0055` ("Fix
formatting") at the class declaration line instead of `error:CA1826`.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1826, CA1852, CA1061, and CA1511 were all observed to route through IDE0055 in build SARIF.
This is unexpected for CA-prefix quality rules. The formatter-backed IDE0055 fires at
`(N, 1)` — the first line of the first type declaration — never at the offending expression.

## Investigation plan

1. **Check `EnforceOnBuild` metadata in NetAnalyzers source.**
   Browse the `dotnet/roslyn-analyzers` GitHub repo for `CA1826`. Look at:
   - `DoNotUseEnumerableMethodsOnIndexableCollectionsAnalyzer.cs` (or similar name)
   - Whether the rule descriptor has `isEnabledByDefault: false` or a custom `EnforceOnBuild` tag
   - Whether there is a code-fix provider that hooks into the formatter pipeline

2. **Try suppressing IDE0055 at the violation site.**
   Add `#pragma warning disable IDE0055` around the `arr.Last()` call and rebuild.
   If CA1826 then appears in SARIF, the rule is testable with an explicit suppression — update
   the test to include the pragma and remove the `Skip`.

3. **Try setting `dotnet_diagnostic.IDE0055.severity = none` in the generated project.**
   Pass an additional `.editorconfig` entry via `additionalFiles` in `CreateProjectBuilder` that
   disables IDE0055. Rebuild and check whether CA1826 appears with its own ID.

4. **Try the `Contains` overload as an alternative trigger.**
   The rule is described as covering `First()`, `Last()`, `ElementAt()`, `Count()`, and `Any()`.
   Test `list.Count()` (note: `list.Count` without parens is not a violation; `list.Count()` is)
   and `list.Any()` — both on `List<int>` and `int[]`. The formatter may not intercept
   extension methods that the rule considers lower-priority triggers.

5. **Test with an older version of NetAnalyzers.**
   Add `<PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.*" />` to a
   scratch project outside the harness and check whether CA1826 fires with its own ID on .NET 8.
   If it does, this is a regression in NetAnalyzers 9+ or 10+ routing.

6. **Check whether SA1649 is masking the diagnostic.**
   The harness injects a single `Program.cs` which triggers SA1649 (file-name/type-name mismatch).
   Add `dotnet_diagnostic.SA1649.severity = none` and check whether CA1826 appears.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1826", "Do not use Enumerable methods on indexable collections",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1826",
    Untestable = "CA1826 is formatter-backed: list.First(), list.Last(), arr.First(), arr.Last(), and list.ElementAt(n) all produce IDE0055 (Fix formatting) instead of CA1826 in NetAnalyzers 10.0.x SARIF output; the rule cannot be triggered with its own diagnostic ID in build analysis")]
public async Task ProhibitEnumerableMethodsOnIndexableCollections()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        using System.Linq;
        namespace test;
        public static class Program
        {
            public static int GetLast()
            {
                int[] arr = { 1, 2, 3 };
                return arr.Last();
            }
            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1826").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause identified: confirmed whether CA1826 uses the formatter pipeline in `EnforceOnBuild`
      mode (same as IDE-style rules) or has `EnforceOnBuild = Never`
- [ ] One of:
  - [ ] A violation pattern found that triggers `error:CA1826` in SARIF → test updated to use
        that pattern, `Skip` removed, test passes in CI; OR
  - [ ] A workaround found (e.g. suppressing IDE0055 exposes CA1826) → test updated to include
        the suppression pragma, `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1826 in build SARIF → `Untestable` reason updated
        with the confirmed root cause (e.g. linked NetAnalyzers GitHub issue URL)
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason

## Resolution (2026-06-06) — PROMOTED TO TESTED

**Root cause: CA1826 is NOT formatter-backed.** It emits its own diagnostic ID correctly in
build SARIF. The original test failed for two compounding reasons:

1. **Wrong receiver type.** The probe used an `int[]` array (`arr.Last()`). CA1826 only fires on
   **interface-typed** receivers (`IReadOnlyList<T>` / `IList<T>`). Arrays *and* concrete
   `List<T>` are excluded — this matches the canonical Microsoft docs example, which uses an
   `IReadOnlyList<int>` parameter.
2. **Redundant `using System.Linq;`.** Under `ImplicitUsings=enable`, `System.Linq` is a default
   global using, so the explicit directive triggered `IDE0005`, which surfaced as `IDE0055`
   ("Fix formatting") at `(2,1)`. That incidental `IDE0055` — the *only* diagnostic the array
   probe produced — was misread as "CA1826 routing through the formatter."

**Probe matrix** (scratch project referencing the packed package, `dotnet build` + SARIF):

| Receiver | Call | CA1826 in SARIF? |
|----------|------|------------------|
| `int[]` (original, + redundant using) | `arr.Last()` | ❌ (only IDE0005/IDE0055/IDE0300) |
| `int[]` | `arr.Last()` | ❌ |
| `List<int>` (concrete) | `list.First()` | ❌ |
| `IReadOnlyList<int>` (param) | `list.First()`/`.Last()` | ✅ `error:CA1826` |

The cleanest pattern (file-scoped namespace + blank lines + implicit usings +
`IReadOnlyList<int>` param) produces **only** `error:CA1826` (1 SARIF result, zero noise).

**Change:** `ProhibitEnumerableMethodsOnIndexableCollections` un-skipped, `Untestable` reason
removed from its `[RuleDoc]`, violation pattern switched to `IReadOnlyList<int>.Last()`. Test
passes; `RuleDocCoverageShould` passes; `docs/rule-reference.md` regenerates with no diff.

**Cross-issue note:** issue 016 lists CA1852 / CA1061 / CA1511 as "CA-prefix rules that route
through IDE0055." This investigation shows that hypothesis is unreliable for at least CA1826 —
the IDE0055 there was incidental noise, not the CA rule. Issues 022 (CA1852), 025 (CA1061), and
028 (CA1511) should be re-probed with correct (non-array, non-redundant-using) violation patterns
before being accepted as untestable.
