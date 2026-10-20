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
| Meziantou.Analyzer | 3.0.302 | 3.0.315 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.401 | 10.0.402 |
| StyleCop.Analyzers | 1.3.1-beta | 1.3.133-beta |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0243 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| SA1143 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |
| SA1315 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |
| SA1317 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |
| SA1653 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |
| SA1654 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |
| SX1412 | Analyzer.StyleCop.Analyzers.Unstable.editorconfig | Added |

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
- MA0243 fired on this repo's own test code (a literal tab inside a raw string fixture in
  `SonarAnalyzerRulesBestPractices2Should.cs`, the S2479 test). That was fixed in code — the tab is
  now emitted through a `$$"""` interpolation hole — rather than by relaxing the rule.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed. (Package content *did*
  change in this PRD — the editorconfigs — so the full suite runs before release.)

## Out of Scope

- Rule coverage for non-analyzer dependencies (e.g., `xunit`, `CliWrap`). None were outdated in
  this run.
- Changing rule severities for existing rules — that is a separate, deliberate change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity. Two are worth a
deliberate look by a human after release:

- **SX1412** (store files as UTF-8 without BOM) is opt-in upstream (`Enabled: False`) but is
  now `warning` here; consumers whose `.cs` files carry a UTF-8 BOM will see build errors.
- **SA1317** (identifiers use only Latin letters) affects codebases with non-English identifiers.

Stale IDs reported by the script: MA0165 and MA0212 (already `severity = none`, no tests), and
CA1047, CA2218, CA2224, S4792 (already stale before this bump, already recorded in
`UntestableRules.cs`). No stale rule has a passing positive test that would now break.
