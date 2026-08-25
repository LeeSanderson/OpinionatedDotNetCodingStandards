## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. The updated packages expose new diagnostic rules that are not yet covered by
the test suite.

## Solution

Update the analyzer package versions in `Directory.Packages.props` and `.nuspec`, regenerate all
analyzer editorconfigs, and add test coverage for each newly-discovered rule.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.140 | 3.0.177 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.302 | 10.0.400 |
| SonarAnalyzer.CSharp | 10.31.0.145097 | 10.33.0.1635 |

`Microsoft.CodeAnalysis.BannedApiAnalyzers` (5.6.0) and `StyleCop.Analyzers`
(1.2.0-beta.556) were already current.

## Newly Discovered Rules

All four new rules come from the Meziantou.Analyzer bump; the NetAnalyzers and SonarAnalyzer
bumps added no new rule IDs.

| Rule ID | Editorconfig | Status | Shipped severity |
|---------|-------------|--------|------------------|
| MA0213 | Analyzer.Meziantou.Analyzer.editorconfig | Added | warning |
| MA0214 | Analyzer.Meziantou.Analyzer.editorconfig | Added | suggestion |
| MA0215 | Analyzer.Meziantou.Analyzer.editorconfig | Added | suggestion |
| MA0217 | Analyzer.Meziantou.Analyzer.editorconfig | Added | suggestion |

No rule IDs were removed by this bump. The `Stale:` entries the update script reports
(`MA0165`, `CA1047`, `CA2218`, `CA2224`, `S4792`) were verified to be stale *before* this bump
as well — they are pre-existing and unchanged.

## User Stories

1. As a maintainer, I want the analyzer packages updated so that new rules are enforced on
   consuming projects.
2. As a maintainer, I want each new rule covered by a test so that the package's rule coverage
   is verified.
3. As a maintainer, I want the test suite to remain green after the update so that the package
   remains releasable.

## Implementation Decisions

- `Directory.Packages.props` and `.nuspec` versions are updated in lockstep (the
  `CheckNugetDependenciesMatchProps.cs` script enforces this).
- `scripts/UpdateAnalyzerEditorConfigs.cs` is re-run after each package bump to pick up
  added/stale rule IDs in the editorconfig files.
- Each new rule gets its own issue with guidance on how to write the test and which confounders
  to exhaust before marking a rule untestable.

### Severity decisions for the new rules

The update script adds new rules at `warning`. Three of the four were deliberately curated down
to `suggestion` instead, and the reasoning is recorded in the header of
`Analyzer.Meziantou.Analyzer.editorconfig` (per-rule comments are regenerated, so notes that must
survive the script belong in the header):

- **MA0214 / MA0215 → `suggestion`.** These two rules are exact inverses and fire on the *same
  expression*. At `warning` they form an unsatisfiable pair: returning a task trips MA0214, and
  awaiting it trips MA0215. This was confirmed empirically against the repo's own
  `AnalyzerResolver.ResolveAsync` overload — both diagnostics landed on the same line depending
  only on which form the code took. Shipping both at `warning` would hand consumers a build error
  with no way to fix it.
- **MA0217 → `suggestion`.** It fires on every non-capturing lambda, including idiomatic LINQ —
  32 sites in this repo alone. Promoting it to `warning` would break the build of essentially any
  consumer that uses LINQ. It is also disabled-by-default upstream.
- **MA0213 → `warning`** (the script's default). It fires nowhere in this repo and the diagnostic
  is unambiguous.

This matches the established curation taste of the Meziantou editorconfig, which sets 121 rules to
`none`, 57 to `warning`, and 37 to `suggestion`, and follows the precedent of relaxing `S1309`
from `warning` to `suggestion`.

### Incidental fixes required to get here

Two pre-existing problems blocked the bump and were fixed as part of it:

- **`global.json` SDK pin.** It requested SDK `10.0.101` with `rollForward: latestPatch`, which
  only rolls forward inside the `10.0.1xx` feature band. On a machine with only `10.0.3xx` SDKs,
  every `dotnet` command in the repo failed. Changed to `rollForward: latestFeature`.
- **`AnalyzerResolver.SelectBestRoslynVersion` picked an unloadable analyzer DLL.** Meziantou
  3.0.177 ships `roslyn4.8` through `roslyn5.9` folders. The resolver picked the highest
  unconditionally, but the tooling references Microsoft.CodeAnalysis.CSharp **5.6.0** and cannot
  load a Roslyn 5.9 build — `GetAnalyzers()` threw, the exception was swallowed, and *zero*
  descriptors came back for the package. That surfaced as "all 212 MA rules are stale" and would
  have silently mis-generated the editorconfig. Selection is now capped at the Roslyn version the
  tooling actually loads, and the update script now refuses to rewrite a file when a package
  yields zero descriptors instead of treating it as "every rule was removed".

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- **Assert on the shipped severity.** `RuleDocCoverageShould` treats `warning`, `error` *and*
  `suggestion` as active, so all four rules need coverage — but the three at `suggestion` emit a
  note, not an error. Use `buildOutput.HasNote("MA02xx")` for MA0214, MA0215 and MA0217, and
  `buildOutput.HasError("MA0213")` for MA0213.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build --filter "FullyQualifiedName~MyNewTest"`.
- Only run the full suite if shared helpers or package content changed. This PRD *does* change
  package content (editorconfigs) and a shared helper (`AnalyzerResolver`), so the full suite must
  run before the release.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`).
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Clearing the pre-existing stale rule entries (`MA0165`, `CA1047`, `CA2218`, `CA2224`, `S4792`).

## Further Notes

The editorconfig update script adds new rules at `warning`. Review the `Added:` output on every
bump to identify rules that warrant a different severity — MA0214/MA0215/MA0217 above are a
worked example of why that review matters.
