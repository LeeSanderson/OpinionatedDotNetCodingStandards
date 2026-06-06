## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1845 ("Use span-based 'string.Concat'") to find a violation pattern that reaches
`error:CA1845` in build SARIF without being subsumed by IDE0057, then either fix the test so
it passes or update the `Untestable` note with a confirmed, well-sourced reason.

**Current state:** `UseSpanBasedStringConcat` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
The canonical violation (`string.Concat(s.Substring(0, 5), "!")`) fires IDE0057
("Substring can be simplified to range indexer") as a `note` (suggestion-level) in SARIF.
`CA1845` never appears. Adding `#pragma warning disable IDE0057` around the call did not
suppress IDE0057 because suggestion-level diagnostics are not suppressed by `#pragma` in
Roslyn's build pipeline.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1845 and CA1846 both have Substring calls at the violation site, and IDE0057 fires there
first. IDE0057 is configured at `suggestion` severity (`dotnet_diagnostic.IDE0057.severity
= suggestion`), which appears in SARIF as `level: "note"`. Pragma cannot suppress it.

## Investigation plan

1. **Determine all overloads that CA1845 covers.**
   The rule fires on:
   - `string.Concat(s.Substring(i), t)` → use `string.Concat(s.AsSpan(i), t)`
   - `MemoryExtensions.SequenceEqual(a.Substring(i), b)` → use `a.AsSpan(i).SequenceEqual(b)`
   - Possibly `string.Concat` with 3+ string arguments where any is a Substring call

   The Substring-based patterns all trigger IDE0057. Try patterns where CA1845 fires on a
   method that is NOT a `Substring` call — e.g.:
   ```csharp
   // Does this trigger CA1845? (no Substring, but passing a char[] to Concat)
   string.Concat(new char[] { 'a', 'b' });
   ```

2. **Try `string.Concat(s[i..])` vs `string.Concat(s.AsSpan(i))`.**
   The range indexer `s[i..]` returns a new `string`, not a `ReadOnlySpan<char>`. If CA1845
   fires on `string.Concat(s[i..], t)` (because a `ReadOnlySpan<char>` overload of Concat
   exists), this would be a pattern that bypasses IDE0057 entirely.

3. **Try `MemoryExtensions.SequenceEqual` with a non-Substring span arg.**
   ```csharp
   // violation: Compare and pass string directly (not via Substring)
   a.AsSpan().SequenceEqual(b);           // (control — no violation)
   MemoryExtensions.SequenceEqual(a, b);  // (static method form — does this fire CA1845?)
   ```

4. **Disable IDE0057 globally for the test project.**
   Pass an additional editorconfig entry via `additionalFiles` in `CreateProjectBuilder`:
   `dotnet_diagnostic.IDE0057.severity = none`
   Then rebuild with the Substring pattern. If CA1845 appears once IDE0057 is silenced,
   the rule is testable with a project-level suppression added to the test.

5. **Try suppressing IDE0057 with `[SuppressMessage]` instead of `#pragma`.**
   `[SuppressMessage("Style", "IDE0057")]` is an attribute-level suppression and may be
   honoured differently from `#pragma` in the build pipeline.

6. **Check the NetAnalyzers source for CA1845's `EnforceOnBuild` value.**
   If CA1845 has `EnforceOnBuild = WhenExplicitlyConfigured` or similar, it may only fire
   when the rule is explicitly set to `warning` or `error` in editorconfig — which it is in
   our package. Confirm the `severity = warning` in the package editorconfig is correct.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1845", "Use span-based 'string.Concat'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1845",
    Untestable = "CA1845 does not appear in build SARIF for any Substring pattern passed to string.Concat or MemoryExtensions.SequenceEqual; IDE0057 (Substring can be simplified) fires as a note/suggestion at the same site and CA1845 is absent even when IDE0057 is suppressed via #pragma — the suggestion-level severity is not suppressed by #pragma warning disable in Roslyn's IDE diagnostic pipeline")]
public async Task UseSpanBasedStringConcat()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        using System;
        namespace test;
        public static class Program
        {
            public static bool AreEqual(string a, string b)
            {
        #pragma warning disable IDE0057
                return a.AsSpan().SequenceEqual(b.Substring(1));
        #pragma warning restore IDE0057
            }
            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1845").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause confirmed: IDE0057 preempts CA1845 because both target the same Substring call,
      and suggestion-level pragma suppression does not work in Roslyn's build pipeline
- [ ] One of:
  - [ ] An alternative pattern found (non-Substring trigger, global IDE0057 suppression, or
        `[SuppressMessage]`) that causes `error:CA1845` to appear in SARIF → test updated,
        `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern can trigger CA1845 without being preempted → `Untestable` reason
        updated with the specific confirmed mechanism (e.g. "CA1845 requires a Substring call
        which always co-fires IDE0057 as a note; suggestion-level notes cannot be suppressed
        via pragma in Roslyn 4.x build mode")
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
