## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading the NetAnalyzers source.

## What to build

Deep-analyse CA1870 ("Use a cached 'SearchValues' instance") to determine why it produces no
diagnostic in build SARIF, then either fix the test so it passes or update the `Untestable`
note with a confirmed, well-sourced reason.

**Current state:** `UseCachedSearchValuesInstance` in
`CodeAnalysisRulesPerformanceModernShould.cs` is marked `[Fact(Skip = "untestable")]`.
Every pattern tried (`span.IndexOfAny(SearchValues.Create("aeiou"))` inline, with and without
a `foreach` wrapper, using `ContainsAny`, with fully-qualified types) produced an empty SARIF
for CA1870. Unrelated diagnostics (IDE0055, IDE0007) fired but CA1870 never appeared.

**Known background (from `issues/016-investigate-formatter-backed-ide-rules.md`):**
CA1870 is in the same "absent from SARIF" failure group as CA1842, CA1843, CA1802, CA1853.
None of these rules produce their own diagnostic ID in build SARIF despite being configured
as `dotnet_diagnostic.CA1870.severity = warning` with `TreatWarningsAsErrors=true`.

**Key context:** CA1870 detects when `SearchValues.Create(...)` is called inline (not stored
as a `static readonly` field) inside a hot path. The rule's fix is to cache the result as a
`static readonly SearchValues<char>` field. This is inherently a data-flow rule because it
requires the analyzer to recognize that the same `SearchValues.Create(...)` call happens
every time the code path executes.

## Investigation plan

1. **Check whether CA1870 requires dataflow/control-flow analysis.**
   Browse `dotnet/roslyn-analyzers` for `UseSearchValuesAnalyzer.cs` (or similar). If the
   rule tracks whether `SearchValues.Create` is called in a non-static context (method body,
   loop, etc.), it may be classified as a flow-based analyzer that only runs in IDE mode.

2. **Try the most direct, unambiguous pattern.**
   Avoid any loop or conditional — put the inline `SearchValues.Create` directly in a method
   body with an immediate return:
   ```csharp
   public static int FindVowel(string s)
   {
       return s.AsSpan().IndexOfAny(SearchValues.Create("aeiou"));
   }
   ```
   If this doesn't fire, the rule is not a flow analysis issue — it simply doesn't run.

3. **Try `ContainsAny` and `LastIndexOfAny`.**
   The rule covers `IndexOfAny`, `ContainsAny`, `LastIndexOfAny`, and `IndexOfAnyExcept`.
   Test each:
   ```csharp
   s.AsSpan().ContainsAny(SearchValues.Create("aeiou"));
   s.AsSpan().LastIndexOfAny(SearchValues.Create("aeiou"));
   s.AsSpan().IndexOfAnyExcept(SearchValues.Create("bcdfg"));
   ```
   If one overload fires and the others don't, the analyzer may have incomplete coverage.

4. **Try storing `SearchValues.Create` in a local variable (not `static readonly`).**
   If the inline pattern is too common to detect, try the "stored in a non-static local"
   variant — which is clearly wrong but not the static-field pattern the rule wants:
   ```csharp
   public static int FindVowel(string s)
   {
       var sv = SearchValues.Create("aeiou");  // local, not static
       return s.AsSpan().IndexOfAny(sv);
   }
   ```
   This is still a violation (not cached) and should fire CA1870 if the rule runs at all.

5. **Try passing the `SearchValues` result as a `ReadOnlySpan<char>` argument instead.**
   `SearchValues.Create` can accept either a `string` or `ReadOnlySpan<char>` of search chars.
   Try the span overload:
   ```csharp
   s.AsSpan().IndexOfAny(SearchValues.Create("aeiou".AsSpan()));
   ```

6. **Check the `EnforceOnBuild` metadata and NetAnalyzers changelog.**
   CA1870 was added in NetAnalyzers 7.0 (for .NET 8). Search the changelog for any notes
   about enabling/disabling it in build mode. Confirm the `.editorconfig` entry
   `dotnet_diagnostic.CA1870.severity = warning` is present in the package editorconfig
   (it should be in `Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig`).

7. **Test on NetAnalyzers 8.x.**
   Confirm whether CA1870 fired with its own ID in an earlier analyzer version.

## Current test code

```csharp
[Fact(Skip = "untestable")]
[RuleDoc("CA1870", "Use a cached 'SearchValues' instance",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1870",
    Untestable = "CA1870 produces no diagnostic in build SARIF for any SearchValues.Create called inline in IndexOfAny/ContainsAny, including patterns inside a foreach loop; the SARIF is empty even with clean code and dotnet_diagnostic.CA1870.severity = warning configured")]
public async Task UseCachedSearchValuesInstance()
{
    using var project = await CreateProjectBuilder();
    await project.AddFile(
        "Program.cs",
        """
        using System.Buffers;

        namespace test;

        public static class Program
        {
            public static bool ContainsVowel(string[] strs)
            {
                foreach (var s in strs)
                {
                    if (s.AsSpan().IndexOfAny(SearchValues.Create("aeiou")) >= 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            public static int Main() => 0;
        }
        """);
    var buildOutput = await project.BuildAndGetOutput();

    buildOutput.HasError("CA1870").ShouldBeTrue();
}
```

## Acceptance criteria

- [ ] Root cause identified: confirmed whether CA1870 requires dataflow analysis disabled in
      build mode, has `EnforceOnBuild = Never`, or another cause
- [ ] One of:
  - [ ] A violation pattern found (direct non-loop call, local variable storage, or
        `ContainsAny` overload) that triggers `error:CA1870` in SARIF → test updated,
        `Skip` removed, test passes in CI; OR
  - [ ] Confirmed no pattern triggers CA1870 in build SARIF → `Untestable` reason updated
        with confirmed root cause (e.g. "CA1870 is a dataflow rule that only runs in IDE mode;
        build-mode analysis does not execute interprocedural data-flow passes")
- [ ] No regressions in other `CodeAnalysisRulesPerformanceModernShould` tests
- [ ] If the test is promoted, `RuleReferenceGenerator` coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
