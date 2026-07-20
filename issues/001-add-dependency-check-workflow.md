## Parent PRD

`issues/prd-replace-renovate-with-scheduled-check.md`

## What to build

A new scheduled GitHub Actions workflow, `.github/workflows/dependency-check.yml`, that replaces
Renovate's one useful function — noticing that a package is outdated — with a deterministic
mechanism built on tools already proven in this repo (see the PRD's Implementation Decisions
section, "New scheduled workflow").

The workflow runs on a weekly `schedule` (targeting "before 9am UTC on Monday", the same cadence
the old `renovate.json` used) plus `workflow_dispatch` for manual/verification runs. It declares
only `contents: read` and `issues: write` permissions. It checks out the repo, sets up the .NET
SDK from `global.json`, restores, installs the `dotnet-outdated-tool` global tool (the same
install pattern already used in `ci.yml`), then runs `dotnet outdated` across
`Opinionated.DotNet.CodingStandards.slnx` using structured (JSON) output — **without**
`--fail-on-updates`, since this job should succeed and report, not fail.

This issue covers the "packages are outdated" path only: when the run finds one or more outdated
packages, the workflow finds an open issue with one exact, fixed title (e.g. "Outdated NuGet
packages detected") and either creates it (if none is open) or updates its body in place (if one
already exists) — never opening a second one. The issue body lists every outdated package with
its current and latest version, and separates the five analyzer packages this repo already owns
a full bump pipeline for (`Meziantou.Analyzer`, `Microsoft.CodeAnalysis.BannedApiAnalyzers`,
`Microsoft.CodeAnalysis.NetAnalyzers`, `SonarAnalyzer.CSharp`, `StyleCop.Analyzers`) from
everything else.

The "nothing is outdated / close the issue" path is deliberately out of scope for this issue —
see `issues/002-close-tracking-issue-when-resolved.md`.

## Acceptance criteria

- [ ] `.github/workflows/dependency-check.yml` exists with `schedule` and `workflow_dispatch`
      triggers, and `permissions: contents: read, issues: write` only
- [ ] The job installs `dotnet-outdated-tool` and runs `dotnet outdated` (JSON output, no
      `--fail-on-updates`) across `Opinionated.DotNet.CodingStandards.slnx`
- [ ] Triggering the workflow manually via `workflow_dispatch` while at least one package
      (analyzer or not) is genuinely outdated creates a tracking issue with an accurate,
      correctly-categorized list of outdated packages (name, current version, latest version)
- [ ] Re-triggering the workflow while the same package is still outdated updates the existing
      tracking issue in place rather than creating a duplicate
- [ ] The job succeeds (exit 0) regardless of whether outdated packages were found — this
      workflow reports, it never fails the build

## Blocked by

None - can start immediately

## User stories addressed

- User story 1
- User story 2
- User story 3
- User story 4
- User story 5
- User story 6
- User story 7
- User story 8
- User story 10
- User story 11
- User story 12
- User story 21
