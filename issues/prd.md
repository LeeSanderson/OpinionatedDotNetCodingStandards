## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. The updated packages expose new diagnostic rules that are not yet covered by
the test suite.

## Solution

Update the five analyzer package versions in `Directory.Packages.props` and `.nuspec`,
regenerate all analyzer editorconfigs, and add test coverage for each newly-discovered rule.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.259 | 3.0.266 |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0240 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0241 | Analyzer.Meziantou.Analyzer.editorconfig | Added |

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
- Both new rules (`MA0240`, `MA0241`) are emitted by the same analyzer,
  `DoNotUseBannedSyntaxAnalyzer`, and are driven by a `BannedSyntaxes.txt` additional file. They
  are the syntax-level counterpart of `BannedSymbols.txt` / `RS0030`, so the existing
  `BannedApiAnalyzersShould.ReportDuplicateBannedSymbolEntry` test is the pattern to follow:
  pass `additionalFiles: ["BannedSyntaxes.txt"]` and write the file with `AddFileAsync`.
- `MA0165`, `S4792`, `CA1047`, `CA2218` and `CA2224` are still reported as `Stale` by the
  editorconfig script, but every one of those is pre-existing and already documented (`MA0165`
  and `S4792` in `CHANGELOG.md`; the three `CA` rules in `UntestableRules.cs`). This bump removes
  no rules.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Both new rules belong in
  `tests/Opinionated.DotNet.CodingStandards.Tests/MeziantouAnalyzers/MeziantouAnalyzersCoreShould.cs`
  (487 lines, and already the home of the most recent `MA02xx` additions including `MA0239`).
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed.

## Out of Scope

- Rule coverage for non-analyzer dependencies (e.g., `xunit`, `CliWrap`). Any that were outdated
  are bumped on this branch — CI's repo-wide `--fail-on-updates` gate leaves no choice — but they
  are not part of the published package, so they get no issue here and no changelog entry. (For
  this run, nothing else was outdated.)
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Shipping a `BannedSyntaxes.txt` file in `pkgsrc/` to ban syntax by default. `MA0240` costs
  nothing until a consumer adds such a file, and choosing which language constructs this package
  bans for every consumer is a deliberate design decision, not part of a version bump.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity. Both `MA0240` and
`MA0241` are enabled by default upstream at `warning`, and the regenerated editorconfig sets both
to `warning`, which matches upstream — no deviation to justify.
