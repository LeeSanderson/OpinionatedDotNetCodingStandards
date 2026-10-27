## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. This bump exposes no new diagnostic rules, but the package versions and the
generated rule reference must stay current and the test suite must remain green.

## Solution

Update the analyzer package versions in `Directory.Packages.props` and `.nuspec`, regenerate all
analyzer editorconfigs, and confirm rule coverage and the rule reference are still complete.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.315 | 3.0.317 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.402 | 10.0.403 |
| StyleCop.Analyzers | 1.3.133-beta | 1.3.151-beta |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| (none) | — | `UpdateAnalyzerEditorConfigs.cs` reported `Added: (none)` for every editorconfig |

The script's `Stale:` output (MA0165, MA0212, CA1047, CA2218, CA2224, S4792) lists only rules
already retired in earlier releases — each is already at `severity = none` with no live test.

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
  to exhaust before marking a rule untestable. This bump has none, so the only issue is the
  rule-reference regeneration gate.

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
  are not part of the published package, so they get no issue here and no changelog entry.
- Changing rule severities for existing rules — that is a separate, deliberate change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.
