## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1846 ("Prefer 'AsSpan' over 'Substring'") to find a violation pattern that
reaches `error:CA1846` in build SARIF without being subsumed by IDE0057, then either fix the
test so it passes or update the `Untestable` note with a confirmed, well-sourced reason.

**Current state:** `PreferAsSpanOverSubstring` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
The canonical pattern (`s.Substring(1).AsSpan()`) fires IDE0057 ("Substring can be simplified")
as a suggestion-level `note` in SARIF. CA1846 never appears even with `#pragma warning disable
IDE0057` — suggestion-level diagnostics are not suppressed by `#pragma` in Roslyn's build
pipeline.

**Shared root cause with CA1845 (see
`issues/020-investigate-ca1845-span-based-string-concat.md`):**
Both CA1845 and CA1846 target `Substring()` calls, and IDE0057 fires first. The key difference
is CA1846's specific trigger: `s.Substring(n)` followed by `.AsSpan()` — the range indexer
(`s[n..]`) would already return a `string`, so calling `.AsSpan()` on a range-indexed string
does NOT trigger CA1846. CA1846 is specifically about avoiding the `Substring` allocation when
the result is immediately converted to span.

## Cross-issue note (confirmed during issue 020, 2026-06-06) — START HERE

The "IDE0057 subsumption / untestable" theory is **wrong** for both CA1845 and CA1846 (verified
by reading the analyzer source and by scratch-project + harness probes while closing issue 020).
This is the same false-untestable confounder already corrected for CA1826/CA1852/CA1842/CA1843.

- The analyzer is `PreferAsSpanOverSubstring` (NetAnalyzers,
  `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Runtime/PreferAsSpanOverSubstring.cs`).
  It registers on `OperationKind.Argument`. It fires when the value of an **argument** (after
  walking down implicit conversions) is a `Substring(int)` / `Substring(int, int)` invocation
  whose parent is a method invocation, **and** that method has another accessible overload with
  the same return type and arity where the `Substring` argument position instead takes
  `ReadOnlySpan<char>`. It has **no** IDE0057 reference and **no** self-suppression guard.
- CA1846 is therefore **not** about `s.Substring(n).AsSpan()` chaining (what the current test
  asserts on — the analyzer never sees that as an argument substitution). It is about passing a
  `Substring` result directly into a method that has a `ReadOnlySpan<char>` overload.
- **Confirmed working pattern** (`error:CA1846` in build SARIF, IDE0057 co-fires as a harmless
  `note` and does not suppress it):

  ```csharp
  namespace test;

  public static class Program
  {
      public static bool TryParseSuffix(string text, out int value)
      {
          return int.TryParse(text.Substring(7), out value);
      }

      public static int Main() => 0;
  }
  ```

  `int.TryParse(string, out int)` has a `int.TryParse(ReadOnlySpan<char>, out int)` overload of
  the same arity/return type, so the `text.Substring(7)` argument triggers CA1846.

To close: un-skip `PreferAsSpanOverSubstring`, remove the `Untestable` reason from its
`[RuleDoc]`, replace the body with the pattern above (drop the `using System;` /
`#pragma warning disable IDE0057` — both are unnecessary and the redundant `using` would itself
fire IDE0005→IDE0055 noise), assert `HasError("CA1846")`, and verify the full suite +
`RuleDocCoverageShould` + `RuleReferenceGeneratorShould`. `RuleLevel.IdeSuggestion` (not
`EnforceOnBuild`) governs default severity; the package's `severity = warning` is sufficient.

The investigation-plan steps below (IDE0057 suppression, `[SuppressMessage]`, two-argument
`Substring`) are now moot — keep for historical context only.

## Investigation plan

1. **Determine whether there is any `AsSpan` context that doesn't involve `Substring`.**
   CA1846 is documented as: "prefer `AsSpan` over `Substring` when the result is immediately
   converted to a span." The trigger requires `Substring`. All `Substring` calls also trigger
   IDE0057. Therefore, the question is whether there is a `Substring`-free path to CA1846.
   Check the rule's documentation and source for patterns beyond `Substring(i).AsSpan()`.

2. **Try `String.Substring(startIndex, length).AsSpan()` with two arguments.**
   `s.Substring(0, 5).AsSpan()` has two arguments. Confirm whether this form also fires IDE0057.
   If IDE0057 only fires on single-argument `Substring(n)` (the `[n..]` range case), the
   two-argument form may avoid IDE0057 while still triggering CA1846:
   ```csharp
   // Does this form fire CA1846 without IDE0057?
   ReadOnlySpan<char> slice = s.Substring(2, 3).AsSpan();
   ```

3. **Disable IDE0057 globally for the test project.**
   Pass an additional editorconfig entry via `additionalFiles`:
   `dotnet_diagnostic.IDE0057.severity = none`
   Then rebuild with `s.Substring(1).AsSpan()`. If CA1846 appears once IDE0057 is silenced,
   the rule is testable with a project-level suppression added to the test.

4. **Try `[SuppressMessage("Style", "IDE0057")]` on the containing method.**
   Attribute-level suppression may be honoured differently from `#pragma` in the build pipeline.

5. **Check the NetAnalyzers 10.0.x source for CA1846's trigger conditions.**
   Browse `dotnet/roslyn-analyzers` for `PreferAsSpanOverSubstringAnalyzer.cs` (or similar).
   Confirm:
   - Exact syntax patterns the rule registers (MemberAccessExpression? InvocationExpression?)
   - Whether it has a guard that prevents firing when IDE0057 is already triggered
   - Whether the `EnforceOnBuild` tag is set

6. **Test on NetAnalyzers 8.x outside the harness.**
   If CA1846 fired without IDE0057 interference in an older version, this is a regression
   caused by IDE0057 being configured at suggestion level.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1846", "Prefer 'AsSpan' over 'Substring'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1846",
    Untestable = "CA1846 does not appear in build SARIF for s.Substring(n).AsSpan() patterns; IDE0057 (suggestion: Substring can be simplified) fires at the same site and CA1846 is absent — #pragma warning disable IDE0057 does not suppress suggestion-level IDE diagnostics in Roslyn's build pipeline, so CA1846 never appears independently")]
public async Task PreferAsSpanOverSubstring()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        using System;
        namespace test;
        public static class Program
        {
            public static ReadOnlySpan<char> GetSlice(string s)
            {
        #pragma warning disable IDE0057
                return s.Substring(1).AsSpan();
        #pragma warning restore IDE0057
            }
            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1846").ShouldBeTrue();
}
```

## Resolution (closed 2026-06-06)

Promoted CA1846 to a tested rule using the pattern confirmed in the cross-issue note above —
the "IDE0057 subsumption / untestable" theory was a misdiagnosis (the same false-untestable
confounder corrected for CA1826/CA1842/CA1843/CA1845/CA1852).

**Root cause of the original failure:** the old probe asserted on `s.Substring(1).AsSpan()`.
The analyzer `PreferAsSpanOverSubstring` registers on `OperationKind.Argument` and only fires
when a `Substring(int)`/`Substring(int, int)` result is passed *as an argument* to a method that
exposes another overload of the same arity/return type taking `ReadOnlySpan<char>` in that
position. A `Substring(...).AsSpan()` chain is never an argument substitution, so the analyzer
could never see it — CA1846 was absent for that reason, not because IDE0057 suppressed it.

**Working pattern (verified — `error:CA1846` present in build SARIF this iteration):**

```csharp
namespace test;

public static class Program
{
    public static bool TryParseSuffix(string text, out int value)
    {
        return int.TryParse(text.Substring(7), out value);
    }

    public static int Main() => 0;
}
```

`int.TryParse(string, out int)` has an `int.TryParse(ReadOnlySpan<char>, out int)` overload of
the same arity/return type, so the `text.Substring(7)` argument fires CA1846. IDE0057 co-fires as
a harmless `note` at the Substring site and does **not** suppress CA1846 — no pragma/editorconfig
suppression is needed (dropped the redundant `using System;` and `#pragma warning disable IDE0057`
from the old probe, both of which only added noise).

**Verification this iteration:**
- Targeted `PreferAsSpanOverSubstring`: passed (1/1) — confirmed `HasError("CA1846")`.
- `dotnet build`: 0 warnings, 0 errors.
- Full suite: 316 passed, 49 skipped, 0 failed (CA1846 moved from skipped → passing).
- `RuleReferenceGeneratorShould` + `RuleDocCoverageShould`: green; `docs/rule-reference.md`
  needs no regeneration (generator does not emit the `Untestable` field; description/HelpLink
  unchanged).

## Acceptance criteria

- [x] Root cause confirmed: CA1846 registers on `OperationKind.Argument`; the original
      `Substring(...).AsSpan()` probe was never an argument substitution the analyzer inspects.
      IDE0057 is NOT the cause — it co-fires as a note and does not suppress CA1846.
- [x] An alternative pattern found (`int.TryParse(text.Substring(7), out value)`) that causes
      `error:CA1846` to appear in SARIF → test updated, `Skip` removed, test passes.
- [x] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [x] `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately. Findings from
`issues/020-investigate-ca1845-span-based-string-concat.md` (IDE0057 suppression approach)
directly inform steps 3 and 4 above.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
