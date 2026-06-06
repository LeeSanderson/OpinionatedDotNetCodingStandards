## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1853 ("Unnecessary call to 'Dictionary.ContainsKey(key)'") to determine why
it produces no diagnostic in build SARIF, then either fix the test so it passes or update the
`Untestable` note with a confirmed, well-sourced reason.

**Current state:** `ProhibitUnnecessaryContainsKeyCall` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
The pattern `dict.ContainsKey(key)` followed by `dict.TryGetValue(key, out var value)`
produced an empty SARIF. The related pattern `ContainsKey + dict[key]` triggers `CA1854`
("Prefer the IDictionary.TryGetValue(TKey, out TValue) method") instead of CA1853.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1853 is in the same "absent from SARIF" failure group as CA1842, CA1843, CA1870. All
were confirmed to produce zero CA-prefixed diagnostics despite `dotnet_diagnostic.CA1853.severity
= warning` and `TreatWarningsAsErrors=true`.

**Key distinction between CA1853 and CA1854:**
- CA1854: `if (dict.ContainsKey(k)) { _ = dict[k]; }` → use `TryGetValue`
- CA1853: `if (dict.ContainsKey(k)) { dict.TryGetValue(k, out _); }` → `ContainsKey` is redundant
  because `TryGetValue` already checks for the key and returns `false` if absent.
CA1854 fires correctly. CA1853 does not fire at all.

## Investigation plan

1. **Verify the exact CA1853 trigger pattern from the documentation.**
   The rule fires when `ContainsKey` is followed by `TryGetValue` for the same key — the
   `ContainsKey` check is redundant because `TryGetValue` already returns `false` if the key
   is absent. Try the negative-guard form:
   ```csharp
   if (!dict.ContainsKey("key"))
   {
       dict.TryGetValue("key", out _);
   }
   ```
   and the positive-guard form:
   ```csharp
   if (dict.ContainsKey("key"))
   {
       dict.TryGetValue("key", out var v);
       _ = v;
   }
   ```

2. **Check the NetAnalyzers source for CA1853.**
   Browse `dotnet/roslyn-analyzers` for `DoNotGuardDictionaryRemoveByContainsKey.cs` or
   `UseContainsKeyCorrectly.cs` (the CA1853 analyzer). Check:
   - Whether it registers via a dataflow/control-flow analysis (which is typically disabled
     in build mode — only IDE mode supports interprocedural analysis)
   - Whether `EnforceOnBuild` is set to `Never` because it requires two-statement flow analysis
   - What `DiagnosticDescriptor` flags are set

3. **Try a simpler single-expression form.**
   If CA1853 requires control-flow analysis of two separate statements, it may be classified
   as a "flow-based" analyzer that doesn't run in build mode. Try the most direct form without
   branching:
   ```csharp
   dict.ContainsKey("key");  // standalone call — does any diagnostic fire?
   dict.TryGetValue("key", out _);
   ```

4. **Try `IDictionary<string, int>` instead of `Dictionary<string, int>`.**
   The rule may target the concrete `Dictionary<TKey, TValue>` type or the interface. Both
   overloads exist; confirm which one the analyzer targets:
   ```csharp
   IDictionary<string, int> dict = new Dictionary<string, int>();
   if (dict.ContainsKey("k")) { dict.TryGetValue("k", out _); }
   ```

5. **Try `ConcurrentDictionary.ContainsKey` followed by `TryGetValue`.**
   `ConcurrentDictionary<TKey, TValue>` also has `ContainsKey` and `TryGetValue`. The rule
   may cover this type as well, and the concurrent dictionary may trigger a different code path.

6. **Test on NetAnalyzers 8.x.**
   Confirm whether CA1853 fired in an earlier version.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1853", "Unnecessary call to 'Dictionary.ContainsKey(key)'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1853",
    Untestable = "CA1853 produces no diagnostic in build SARIF for ContainsKey followed by TryGetValue; the SARIF is empty with clean code. Note: ContainsKey followed by direct indexer access fires CA1854 (a related but distinct rule), which is tested separately")]
public async Task ProhibitUnnecessaryContainsKeyCall()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        namespace test;

        public static class Program
        {
            public static int? GetValue(Dictionary<string, int> dict, string key)
            {
                if (dict.ContainsKey(key))
                {
                    dict.TryGetValue(key, out var value);
                    return value;
                }

                return null;
            }

            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1853").ShouldBeTrue();
}
```

## Resolution (closed 2026-06-06)

**Root cause: the test probed the wrong pattern.** CA1853 was never "absent from SARIF for a
real violation" — the original probe simply did not contain a CA1853 violation. CA1853's
analyzer is `DoNotGuardDictionaryRemoveByContainsKey` (the filename this issue itself cites in
step 2): it fires when a `Dictionary<TKey,TValue>.Remove(key)` call is guarded by a redundant
`ContainsKey(key)` check, because `Remove` already returns `false` (no throw) when the key is
absent. The three sibling guard rules are distinct:

| Pattern | Rule |
|---------|------|
| `if (dict.ContainsKey(k)) _ = dict[k];`        | CA1854 (prefer TryGetValue) |
| `if (!dict.ContainsKey(k)) dict.Add(k, v);`    | CA1864 (prefer TryAdd)      |
| `if (dict.ContainsKey(k)) dict.Remove(k);`     | **CA1853** (Remove is unguarded) |

The old probe used `ContainsKey` followed by `TryGetValue` — a combination that matches *no*
CA rule, so the SARIF was correctly empty. The "produces no diagnostic" note was a
misdiagnosis, same class of error as issues 017/018/019/020/021/022. CA1853 is **not**
`EnforceOnBuild = Never`, does not require interprocedural/dataflow analysis, and is not
formatter-backed.

**Fix:** replaced the body with the canonical `ContainsKey`-guarding-`Remove` violation
(relying on ImplicitUsings for `Dictionary`, so no redundant `using` → no IDE0005/IDE0055
noise), un-skipped the `[Fact]`, and removed the `Untestable` note from `[RuleDoc]` (now
`Untestable == null`, satisfying the method-level RuleDoc convention).

**Verified this iteration:**
- Targeted `ProhibitUnnecessaryContainsKeyCall`: passed (1/1) — `HasError("CA1853")` true
  (severity=warning + TreatWarningsAsErrors surfaces it as `error:CA1853`).
- `dotnet build`: 0 warnings, 0 errors.
- Full suite: 317 passed, 48 skipped, 0 failed (CA1853 moved from skipped to passing).
- `docs/rule-reference.md` needs no regeneration: the generator does not emit the `Untestable`
  field and CA1853's description/HelpLink are unchanged (RuleReferenceGeneratorShould green).

## Acceptance criteria

- [x] Root cause identified: confirmed CA1853 is *not* `EnforceOnBuild = Never` and does *not*
      require disabled-in-build control-flow analysis — the original probe used a non-CA1853
      pattern (`ContainsKey`+`TryGetValue`). The real trigger is `ContainsKey` guarding `Remove`.
- [x] A violation pattern found that triggers `error:CA1853` in SARIF → test updated, `Skip`
      removed, test passes.
- [x] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests (full suite green).
- [x] `RuleReferenceGenerator` coverage test continues to pass.

## Original acceptance criteria

- [ ] Root cause identified: confirmed whether CA1853 requires control-flow analysis that is
      disabled in build mode, or has `EnforceOnBuild = Never`, or another cause
- [ ] One of:
  - [ ] A violation pattern found that triggers `error:CA1853` in SARIF → test updated,
        `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern triggers CA1853 in build SARIF → `Untestable` reason updated
        with confirmed root cause (e.g. "CA1853 requires two-statement control-flow analysis
        which is disabled in build mode; only IDE mode runs interprocedural analysis")
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
