## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. The updated packages expose new diagnostic rules that are not yet covered by
the test suite.

## Solution

Update the analyzer package versions in `Directory.Packages.props` and `.nuspec`,
regenerate all analyzer editorconfigs, and add test coverage for each newly-discovered rule.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.341 | 3.0.342 |
| SonarAnalyzer.CSharp | 10.36.0.15337 | 10.37.0.15768 |
| StyleCop.Analyzers | 1.3.180 | 1.4.47-beta |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| SA1144 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added (warning) |
| SA1145 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added (warning) |
| SA1415 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added (none — disabled by default upstream; fires 23 times on this repo's own code, matching the existing convention of `none` for disabled-by-default StyleCop rules) |
| SA1655 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added (warning) |

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
  added/stale rule IDs in the editorconfig files. All `Stale:` IDs it reported in this run
  (MA0165, MA0212, CA1047, CA2218, CA2224, S4792) were already stale before this bump and are
  already handled, so nothing needs retiring.
- SA1415 is set to `none` (inactive), so it needs no `[RuleDoc]` and has no issue.
- Each new active rule gets its own issue with guidance on how to write the test and which
  confounders to exhaust before marking a rule untestable.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Package content changed in this bump (editorconfigs, nuspec, props), so the full suite must
  run before release.

## Out of Scope

- Rule coverage for non-analyzer dependencies (e.g., `xunit`, `CliWrap`). None were outdated in
  this run.
- Changing rule severities for existing rules — that is a separate, deliberate change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.
