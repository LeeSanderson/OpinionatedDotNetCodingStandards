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
| Meziantou.Analyzer | 3.0.228 | 3.0.258 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.400 | 10.0.401 |
| SonarAnalyzer.CSharp | 10.33.0.1635 | 10.34.0.3385 |

The other two owned analyzer packages were already at their latest versions and are unchanged:
`Microsoft.CodeAnalysis.BannedApiAnalyzers` 5.6.0, `StyleCop.Analyzers` 1.2.0-beta.556.

Only the Meziantou bump introduced new rule IDs. The NetAnalyzers and SonarAnalyzer bumps added
none (`Added: (none)` for both editorconfigs).

## Newly Discovered Rules

All fourteen come from `Meziantou.Analyzer` and land in
`Analyzer.Meziantou.Analyzer.editorconfig`. Twelve of them (everything except `MA0227` and
`MA0239`) are a single new family covering `System.Diagnostics.Tracing.EventSource` correctness.

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0226 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0227 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0228 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0229 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0230 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0231 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0232 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0233 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0234 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0235 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0236 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0237 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0238 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` |
| MA0239 | Analyzer.Meziantou.Analyzer.editorconfig | Added — enforced as `warning` (upstream default is `suggestion`) |

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
- **Severity review: all fourteen new rules stay at the `warning` the generator writes.** The
  twelve EventSource rules (`MA0226`, `MA0228`–`MA0238`) each report a concrete defect in an
  `EventSource` subclass — a duplicate event id, a payload that does not match the method's
  parameters, an unsupported parameter type. These are silent-at-runtime bugs (ETW simply drops or
  mis-renders the event), they only fire on code that actually derives from `EventSource`, and each
  has an unambiguous fix, so none of them can flood an ordinary consumer's build the way `MA0217`
  or `MA0219` would. `MA0227` is likewise narrow — it fires only on the two shapes where
  `Enumerable.Contains` genuinely degrades a set's O(1) lookup to O(n).
- **`MA0239` stays at `warning` even though upstream ships it at `suggestion`.** It fires only on
  `GetType()` called on an expression whose static type is already `sealed`, where `typeof(T)` is
  exactly equivalent and cheaper. That is a narrow, mechanically fixable shape, not a stylistic
  preference that a consumer could reasonably disagree with — the same reasoning that kept
  `MA0218` and `MA0221` at `warning` despite upstream suggestion defaults. This is deliberately
  *not* the `MA0214`/`MA0215`/`MA0217`/`MA0219` case: those were downgraded because they fire on
  ubiquitous idiomatic code (LINQ, XML comments) or form an unsatisfiable pair.
- **SonarAnalyzer 10.34.0 flips five rules to disabled-by-default upstream** — `S1264`, `S2692`,
  `S3249`, `S3885` and `S6670` all changed from `Enabled: True` to `Enabled: False` in the
  regenerated editorconfig comments. This package sets each of them to `severity = warning`
  explicitly, so they remain enforced and **nothing changes for consumers**. Only the regenerated
  metadata comments moved; no `dotnet_diagnostic.*.severity` line changed.
- The generator reports `MA0165`, `CA1047`, `CA2218`, `CA2224` and `S4792` as `Stale`, but this is
  **pre-existing and out of scope**. All five were already stale before this bump (`MA0165` was
  documented as removed in `v0.0.13`, `S4792` earlier still), the generator leaves stale entries in
  place, and the diff confirms no severity line was removed by this run.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- **The twelve EventSource rules go in a new split file**,
  `tests/Opinionated.DotNet.CodingStandards.Tests/MeziantouAnalyzers/MeziantouAnalyzersEventSourceShould.cs`,
  following AGENTS.md's `<OriginalClass><Group>Should` convention. The three existing Meziantou
  files are 858 / 696 / 441 lines, and twelve more tests would push `MeziantouAnalyzers3Should.cs`
  past the 1000-line limit. `MA0227` and `MA0239` are not EventSource rules and go in
  `MeziantouAnalyzersCoreShould.cs` (441 lines — ample room).
- All fourteen are configured at `warning`, which the SARIF ErrorLog reports as `error`, so every
  test asserts with `buildOutput.HasError("MA02xx").ShouldBeTrue()` — not `HasNote`.
- **Each EventSource test must trigger exactly one new rule.** The family overlaps heavily: an
  `EventSource` subclass that is not `sealed` trips `MA0226`, so every test other than the
  `MA0226` one must declare its subclass `sealed`; a `WriteEvent` call whose payload does not match
  the method's parameters trips `MA0235`, so the `MA0234` test must keep the payload correct and
  vary only the event id. Assert on the target rule and keep the rest of the class valid.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue). None of these rules is expected to be untestable —
  `System.Diagnostics.Tracing.EventSource` is in the BCL, so no `PackageReference` is needed.
- Run new tests in isolation: `dotnet test --no-build -- --filter-method "*MyNewTest*"` (the test
  project runs on Microsoft.Testing.Platform, so filters go after a bare `--` and are glob-based;
  the old `--filter "FullyQualifiedName~..."` VSTest form fails on the .NET 10 SDK).
- Only run the full suite if shared helpers or package content changed. Package content **did**
  change in this PRD (the editorconfigs), so the full suite must be run before the release.

## Out of Scope

- Bumping non-analyzer dependencies. `dotnet outdated` also reports `xunit.v3` 4.0.0 → 4.0.1;
  that is a separate lightweight bump, deliberately not folded into this PRD.
- Changing rule severities for existing rules — that is a separate, deliberate change. The
  `MA0239` decision above concerns a *newly added* rule's initial severity, not a change to an
  already-shipped one.
- Removing the stale `MA0165` / `CA1047` / `CA2218` / `CA2224` / `S4792` entries.

## Further Notes

The editorconfig update script adds new rules at `warning` regardless of the analyzer's own
default severity, so every `Added:` rule needs a deliberate severity review. In this bump that
review confirmed all fourteen at `warning` (see Implementation Decisions for the `MA0239`
reasoning, the one rule whose upstream default differs).
