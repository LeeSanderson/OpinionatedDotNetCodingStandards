## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. This bump did not expose any new diagnostic rule IDs, but one of the packages
required a source fix in the repo's own tooling code, and rule titles changed for several
existing SonarAnalyzer rules.

## Solution

Update the five analyzer package versions in `Directory.Packages.props` and `.nuspec`,
regenerate all analyzer editorconfigs, and regenerate the rule reference doc so it reflects the
current state of the packages.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.115 | 3.0.121 |
| Microsoft.CodeAnalysis.BannedApiAnalyzers | 4.14.0 | 5.6.0 |
| SonarAnalyzer.CSharp | 10.27.0.140913 | 10.28.0.143324 |

`Microsoft.CodeAnalysis.NetAnalyzers` (10.0.301) and `StyleCop.Analyzers` (1.2.0-beta.556) were
already at their latest available version and were not changed.

## Newly Discovered Rules

None. Regenerating the editorconfigs for all five packages reported `Added: (none)` for every
package — this bump changed rule titles/descriptions and fixed rule logic but did not introduce
any new default-enabled diagnostic IDs.

## User Stories

1. As a maintainer, I want the analyzer packages updated so that the package tracks upstream
   fixes and improvements.
2. As a maintainer, I want the rule reference doc regenerated so that any changed rule
   titles/descriptions are reflected accurately.
3. As a maintainer, I want the test suite to remain green after the update so that the package
   remains releasable.

## Implementation Decisions

- `Directory.Packages.props` and `.nuspec` versions are updated in lockstep (the
  `CheckNugetDependenciesMatchProps.cs` script enforces this).
- `scripts/UpdateAnalyzerEditorConfigs.cs` was re-run after each package bump; it reported no
  added rule IDs, only cosmetic title changes to existing SonarAnalyzer.CSharp rule comments and a
  few pre-existing stale entries (`MA0165`, `S4792`, `CA1047`, `CA2218`, `CA2224`) that are
  unrelated to this bump and were left untouched by the generator (it preserves stale blocks
  verbatim).
- The `SonarAnalyzer.CSharp` bump (10.27.0.140913 → 10.28.0.143324) changed the detection logic
  for rule `S4036` ("OS commands should not rely on PATH resolution"), which then fired on
  pre-existing code in `src/Opinionated.DotNet.CodingStandards.Tooling/AnalyzerResolver.cs`
  (`ProcessStartInfo.FileName = "dotnet"`). This was fixed at the source — using
  `Environment.ProcessPath` to resolve the absolute path to the currently-running `dotnet` host —
  rather than suppressing the rule, since the underlying security concern (PATH-based command
  resolution) is real and the fix is straightforward.
- No changes needed for the `Microsoft.CodeAnalysis.BannedApiAnalyzers` major version bump
  (4.14.0 → 5.6.0); the full solution build succeeds with 0 warnings/errors.

## Testing Decisions

- Since no new rule IDs were introduced, no new `[RuleDoc]` tests are required.
- The full solution build (`dotnet build Opinionated.DotNet.CodingStandards.slnx`) passed with 0
  warnings/errors after the version bump and the `AnalyzerResolver.cs` fix — this is a strong
  regression signal on its own since this repo dogfoods its own analyzers with warnings-as-errors.
- Run the full test suite (`dotnet test`) before merging, since package content
  (`pkgsrc/config/analyzers/*.editorconfig`) changed.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`).
- Changing rule severities for existing rules — that is a separate, deliberate change.
- Investigating the pre-existing stale rule IDs (`MA0165`, `S4792`, `CA1047`, `CA2218`,
  `CA2224`) — they were already stale before this bump and are unrelated to it.

## Further Notes

The `SonarAnalyzer.CSharp` editorconfig diff is almost entirely rule-title/description text
changes (Sonar renamed many "security-sensitive" hotspot titles to imperative rule statements,
e.g. "Using hardcoded IP addresses is security-sensitive" → "IP addresses should not be
hardcoded"). No severities changed as part of this.
