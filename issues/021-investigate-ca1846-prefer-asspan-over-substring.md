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

## Acceptance criteria

- [ ] Root cause confirmed: whether IDE0057 is the sole reason CA1846 is absent, and whether
      the two-argument `Substring(i, n).AsSpan()` form avoids IDE0057
- [ ] One of:
  - [ ] An alternative pattern found (two-argument Substring, global IDE0057 suppression, or
        attribute suppression) that causes `error:CA1846` to appear in SARIF → test updated,
        `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1846 without being preempted by IDE0057 → `Untestable`
        reason updated with specific confirmed mechanism
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately. Findings from
`issues/020-investigate-ca1845-span-based-string-concat.md` (IDE0057 suppression approach)
directly inform steps 3 and 4 above.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
