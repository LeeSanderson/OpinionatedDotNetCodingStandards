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
| Meziantou.Analyzer | 3.0.266 | 3.0.294 |
| SonarAnalyzer.CSharp | 10.34.0.3385 | 10.35.0.4138 |

`Microsoft.CodeAnalysis.BannedApiAnalyzers` (5.6.0), `Microsoft.CodeAnalysis.NetAnalyzers`
(10.0.401) and `StyleCop.Analyzers` (1.2.0-beta.556) are already at their newest applicable
versions and are unchanged.

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0242 | Analyzer.Meziantou.Analyzer.editorconfig | Added |

`SonarAnalyzer.CSharp` 10.35.0.4138 added no new rule IDs — the editorconfig update script
reported `Added: (none)` for it, so the bump needs no new coverage.

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

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed.

## Out of Scope

- Rule coverage for non-analyzer dependencies (e.g., `xunit`, `CliWrap`). Any that were outdated
  are bumped on this branch — CI's repo-wide `--fail-on-updates` gate leaves no choice — but they
  are not part of the published package, so they get no issue here and no changelog entry. On this
  run there were none: `dotnet-outdated` reported only the two analyzer packages above.
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Removing the rule IDs the update script reported as `Stale` (`MA0165`, `MA0212`, `CA1047`,
  `CA2218`, `CA2224`, `S4792`). The script left them in place; pruning them is its own change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity. `MA0242` was added at
`warning` (its analyzer default is `suggestion`), consistent with how the generator treats the
rest of the Meziantou set.
