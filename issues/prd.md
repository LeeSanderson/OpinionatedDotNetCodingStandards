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
| Meziantou.Analyzer | 3.0.320 | 3.0.322 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.403 | 11.0.100 |
| StyleCop.Analyzers | 1.3.151-beta | 1.3.177-beta |

`xunit.v3` was also bumped from 4.0.2 to 4.0.3. CI's repo-wide `--fail-on-updates` gate requires
this. It is a test-only dependency, so it gets no issue and no changelog entry.

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| CA1517 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA1876 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA1877 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA2026 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA2027 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA2028 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |
| CA2267 | Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig | Added |

The script also reported these stale IDs: `MA0165`, `MA0212`, `CA1047`, `CA2218`, `CA2224` and
`S4792`. All of them were already stale before this bump and are already handled. They are either
at `severity = none` or have a class-level entry in `UntestableRules.cs`. No retirement work is
needed.

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
  are not part of the published package, so they get no issue here and no changelog entry.
- Changing rule severities for existing rules — that is a separate, deliberate change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.

This is a major version bump of `Microsoft.CodeAnalysis.NetAnalyzers` (10.x → 11.x). The repo's
own build stayed clean with no new diagnostics. The test harness still builds throwaway
`net10.0` projects. Any new rule that depends on a .NET 11-only API needs that checked first.
