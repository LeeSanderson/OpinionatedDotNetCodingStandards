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
| Meziantou.Analyzer | 3.0.177 | 3.0.200 |

The other four owned analyzer packages were already at their latest versions and are unchanged:
`Microsoft.CodeAnalysis.BannedApiAnalyzers` 5.6.0, `Microsoft.CodeAnalysis.NetAnalyzers` 10.0.400,
`SonarAnalyzer.CSharp` 10.33.0.1635, `StyleCop.Analyzers` 1.2.0-beta.556.

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0218 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0219 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `suggestion` (deliberately downgraded) |
| MA0220 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |

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
- **`MA0219` is deliberately configured as `suggestion`, not the `warning` the generator writes
  by default.** It fires on every `<c>`/`<code>` element in an XML doc comment that has no
  `language` attribute, so at `warning` it would break the build of essentially any consumer that
  documents its API. This was confirmed empirically against this repo: adding
  `/// <summary>Probe <c>{ "value": 1 }</c></summary>` to a source file failed the build with
  `error MA0219`. The attribute is only a convention — it is not part of the XML documentation
  comment specification, tools may ignore it, and upstream ships the rule at `hidden` severity for
  exactly that reason. This mirrors the existing `MA0214`/`MA0215`/`MA0217` decisions, and the
  rationale is recorded in the header of `Analyzer.Meziantou.Analyzer.editorconfig` so it survives
  future runs of the generator (which regenerates the per-rule comments but preserves severities).
- `MA0218` stays at `warning`: it fires only on an explicitly empty `language=""` attribute, which
  is an unambiguous typo rather than a missing optional convention.
- `MA0220` stays at `warning` (also its upstream default): it reports an invalid regular expression
  in an `.editorconfig` option value, which silently changes a rule's behaviour if left unfixed.
- `MA0165` is reported as `Stale` by the generator, but this is **pre-existing and out of scope**.
  Re-running the generator against the previous 3.0.177 pin reports it as stale too, so it was not
  dropped by this bump; it is also configured as `severity = none`, so it affects nothing.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Rules enforced as `suggestion` assert with `buildOutput.HasNote(...)`, not `HasError(...)` —
  see the existing `MA0215`/`MA0217` tests for the pattern. This applies to `MA0219`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed. Package content **did**
  change in this PRD (the editorconfig), so the full suite must be run before the release.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`). `dotnet outdated` reports none.
- Changing rule severities for existing rules — that is a separate, deliberate change. The
  `MA0219` decision above concerns a *newly added* rule's initial severity, not a change to an
  already-shipped one.
- Removing the stale `MA0165` entry.

## Further Notes

The editorconfig update script adds new rules at `warning` regardless of the analyzer's own
default severity, so every `Added:` rule needs a deliberate severity review. In this bump that
review changed one of the three (`MA0219`).
