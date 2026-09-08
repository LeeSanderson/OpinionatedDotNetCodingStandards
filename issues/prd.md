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
| Meziantou.Analyzer | 3.0.200 | 3.0.228 |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0221 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0222 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0223 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0224 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0225 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0165 | Analyzer.Meziantou.Analyzer.editorconfig | Stale (dropped upstream; already `severity = none`) |

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
- All five new rules are disabled by default upstream (`Enabled: False`); the regenerated
  editorconfig turns each of them on at `warning`, matching how the package treats the rest of
  the Meziantou rule set.
- MA0165 (`Make interpolated string`) was dropped from Meziantou.Analyzer in this version. It was
  already configured `severity = none`, so it is left in the editorconfig exactly as the update
  script left it — consistent with the other long-standing stale entries (CA1047, CA2218, CA2224,
  S4792). No consumer-visible behaviour changes.
- Each new rule gets its own issue with guidance on how to write the test and which confounders
  to exhaust before marking a rule untestable.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- All five new rules go in `tests/Opinionated.DotNet.CodingStandards.Tests/MeziantouAnalyzers/MeziantouAnalyzers3Should.cs`
  (705 lines — five more tests keeps it comfortably under the 1000-line limit).
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`) — `dotnet outdated` reports none
  pending.
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Removing the stale MA0165 entry from the editorconfig.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.

All five new rules cluster into two themes: nullability annotations on `TryGetValue`-shaped
methods (MA0221), and System.Text.Json options hygiene (MA0222–MA0225, covering both the
source-generation attribute and the runtime options object). The Json rules need no extra
`PackageReference` — `System.Text.Json` and `JsonSerializerContext` are in the `net10.0` BCL, and
`RespectNullableAnnotations` / `RespectRequiredConstructorParameters` have existed since .NET 9.
