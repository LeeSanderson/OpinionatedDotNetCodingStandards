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

---

## Resolution (closed 2026-06-06)

**Root cause: the "IDE0057 subsumption / untestable" note was a misdiagnosis** — the same
false-untestable confounder pattern already corrected for CA1826 (017), CA1852 (022), and
CA1842/CA1843 (018/019). CA1845 fires correctly in build SARIF; the old probe simply used a
violation pattern the analyzer never inspects.

The analyzer is `UseSpanBasedStringConcat` (NetAnalyzers,
`src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Runtime/UseSpanBasedStringConcat.cs`,
verified against the current source). It registers **one** action, on
`OperationKind.Binary` — the string **`+` concatenation operator**. It does **not** register on
`IInvocationOperation` / existing `string.Concat(...)` calls, and it has **no** reference to
IDE0057 or any Substring-simplification self-suppression guard (the two analyzers are fully
independent). It reports the top-most concat when:

1. every operand is `string` (or a `char` literal),
2. at least one operand is a `Substring(int)` / `Substring(int, int)` call, and
3. a fixed-arity span-based `string.Concat(ReadOnlySpan<char>, …)` overload of the matching
   arity exists (BCL provides 2-, 3-, and 4-span overloads; a 5+-operand chain finds no overload
   and the rule stays silent).

The old test asserted on `a.AsSpan().SequenceEqual(b.Substring(1))` — a `MemoryExtensions`
method invocation, not a `+` binary op — so CA1845 was never even evaluated. The IDE0057 `note`
that co-fired on the `Substring` call was incidental and was misread as "the rule routes through
/ is subsumed by IDE0057."

**Fix / pattern found (acceptance criterion 1):** a real `+` concat with a `Substring` operand,
e.g. `return text.Substring(1) + "!";`, fires `error:CA1845` (CA1845 is configured
`severity = warning` and the project sets `TreatWarningsAsErrors`). Verified by scratch-project
probe (`dotnet build` + SARIF) **and** by the promoted harness test. IDE0057 still emits a `note`
at the `Substring` site in the same SARIF — confirming it does **not** suppress CA1845, so no
`#pragma`/editorconfig IDE0057 suppression is needed.

**Note (`RuleLevel`, not `EnforceOnBuild`):** NetAnalyzers CA rules do not use the
`EnforceOnBuildValues` enum (that is a dotnet/roslyn IDE concept). CA1845 uses
`RuleLevel.IdeSuggestion` → default `(Info, enabledByDefault: true)`. Raising it to `warning` in
the package editorconfig is sufficient for build enforcement; the rule was never gated off
build.

**Changes:**
- Un-skipped `UseSpanBasedStringConcat`; removed the `Untestable` reason from its `[RuleDoc]`
  (now `Untestable == null`). Test asserts `HasError("CA1845")` on the `+` concat pattern and
  carries an explanatory comment.
- Cross-issue note added to issue 021 (CA1846): the shared "IDE0057 subsumption" theory is also
  wrong; CA1846 fires on a `Substring` result passed as a method argument that has a
  `ReadOnlySpan<char>` overload — confirmed `error:CA1846` for
  `int.TryParse(text.Substring(7), out value)` during this investigation.

**Verification:** `dotnet build` 0 warnings / 0 errors; `UseSpanBasedStringConcat` +
`RuleDocCoverageShould` + `RuleReferenceGeneratorShould` green (8 passed); full suite
315 passed / 50 skipped / 0 failed (CA1845 moved from skipped to passing).
`docs/rule-reference.md` regenerates with no diff (description/help link unchanged; the generator
does not emit the `Untestable` field).
