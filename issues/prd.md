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
| Meziantou.Analyzer | 3.0.121 | 3.0.123 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.301 | 10.0.302 |
| SonarAnalyzer.CSharp | 10.28.0.143324 | 10.29.0.143774 |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0211 | Analyzer.Meziantou.Analyzer.editorconfig | Added |

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
- The SonarAnalyzer.CSharp bump surfaced a real behavior change on this repo's own dogfooded
  code: `S6444` (regular expressions should be executed with a timeout) now flags three
  pre-existing `Regex`/`new Regex(...)` constructions in
  `src/Opinionated.DotNet.CodingStandards.Tooling/` (`RuleReferenceGenerator.cs`,
  `EditorConfigMergeGenerator.cs`, `AnalyzerResolver.cs`) that the prior analyzer version did not
  flag. These were fixed in place by adding an explicit `TimeSpan` timeout argument rather than
  suppressing the rule, per AGENTS.md's "don't fight the analyzers" guidance.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build --filter "FullyQualifiedName~MyNewTest"`.
- Only run the full suite if shared helpers or package content changed.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`).
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Microsoft.CodeAnalysis.BannedApiAnalyzers and StyleCop.Analyzers were checked and are already
  at their latest available version (stable and pre-release respectively) — no action needed.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.

`S4792`, `MA0165`, `CA1047`, `CA2218`, and `CA2224` were reported as stale by the editorconfig
update script, but none of that staleness is new to this bump — `S4792` was already documented
as removed in the `v0.0.5` changelog entry, and the others were already at a no-op severity
(`none`) or otherwise unchanged by this run's diff. No new `### Removed` changelog entry is
needed for this release.
