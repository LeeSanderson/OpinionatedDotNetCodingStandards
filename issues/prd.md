## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. Two of the five owned analyzer packages had newer releases on NuGet.org:
Meziantou.Analyzer and StyleCop.Analyzers. The regenerated editorconfigs must be verified
against the new versions and `docs/rule-reference.md` must be regenerated so it reflects the
shipped rule metadata (StyleCop moved its per-rule documentation to GitHub Pages, so every
StyleCop help link in the generated editorconfig changed).

## Solution

Update the analyzer package versions in `Directory.Packages.props` and `.nuspec`, regenerate
all analyzer editorconfigs, fix the editorconfig tooling so it can still resolve StyleCop's
analyzer DLL under the new package layout, and regenerate the rule reference.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.294 | 3.0.302 |
| StyleCop.Analyzers | 1.2.0-beta.556 | 1.3.1-beta |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| (none) | — | Neither package added a new rule ID; `scripts/UpdateAnalyzerEditorConfigs.cs` reported `Added: (none)` for every editorconfig |

## User Stories

1. As a maintainer, I want the analyzer packages updated so that upstream fixes are enforced on
   consuming projects.
2. As a maintainer, I want each new rule covered by a test so that the package's rule coverage
   is verified. (No new rules in this bump, so there are no per-rule issues.)
3. As a maintainer, I want the test suite to remain green and the rule reference to be current
   after the update so that the package remains releasable.

## Implementation Decisions

- `Directory.Packages.props` and `.nuspec` versions are updated in lockstep (the
  `CheckNugetDependenciesMatchProps.cs` script enforces this).
- `scripts/UpdateAnalyzerEditorConfigs.cs` is re-run after each package bump to pick up
  added/stale rule IDs in the editorconfig files.
- **StyleCop package layout change.** Up to 1.2.0-beta.556, `StyleCop.Analyzers` was a
  metapackage whose analyzer DLL lived in the transitive `StyleCop.Analyzers.Unstable` package.
  From 1.3.1-beta the DLL ships directly in `StyleCop.Analyzers` and the `.Unstable` package no
  longer exists. `AnalyzerResolver.PackageAllowMap` in the Tooling project keyed StyleCop on
  `stylecop.analyzers.unstable`, so the editorconfig script silently skipped StyleCop after the
  bump. The map is now keyed on `stylecop.analyzers`. The editorconfig file keeps its existing
  `Analyzer.StyleCop.Analyzers.Unstable.editorconfig` name because it is part of the shipped
  package payload and is imported by name.
- The regenerated StyleCop editorconfig changes only comments: every help link moved from
  `github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/<id>.md` to
  `dotnetanalyzers.github.io/StyleCopAnalyzers/<id>.html`, and the SA1413 title gained "and
  lists". No severity changed.
- `xunit.v3` (test-only) was bumped from 4.0.1 to 4.0.2 on the same branch because CI's
  repo-wide `dotnet-outdated --fail-on-updates` gate would otherwise fail the PR. It is not a
  dependency of the published package and gets no changelog entry.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`. This bump added no rule IDs, so
  no `[RuleDoc]` changes are needed; `RuleDocCoverageShould` must still pass.
- StyleCop 1.3.1-beta changes the behaviour of many existing rules (records, primary
  constructors, extension blocks, SA1600, SA1316, SA1312, SA1205, SA1304, SA1122, SA1130,
  SA1121/SA1125, SA1649 and others). This package enforces only `SA1649` from StyleCop, so the
  existing `StyleCopAnalyzersShould` test is the one to watch. The full suite runs in CI on the PR
  and again in `New-ReleaseTag.ps1` before tagging.
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Package content changed in this PRD (versions, editorconfig), so the full suite must be green
  before release.

## Out of Scope

- Rule coverage for non-analyzer dependencies (e.g., `xunit`, `CliWrap`). Any that were outdated
  are bumped on this branch — CI's repo-wide `--fail-on-updates` gate leaves no choice — but they
  are not part of the published package, so they get no issue here and no changelog entry.
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Renaming `Analyzer.StyleCop.Analyzers.Unstable.editorconfig` to drop the now-historical
  `.Unstable` suffix. It would touch the shipped payload, the MSBuild imports and the generated
  docs heading for no consumer benefit.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity. For this bump the
output was `Added: (none)` for every editorconfig.
