# PRD: Renovate Configuration and Branch Protection

## Problem Statement

The project has no automated dependency update mechanism. Analyzer package versions are checked
manually via `dotnet outdated` in a scheduled Azure Pipelines run (`outdated.yml`), and bumps
require manual edits to both `Directory.Packages.props` and the `.nuspec` file (which must stay
in sync). There is also no branch protection on `main`, meaning commits can be pushed directly
without CI passing — and the AI agent skills (`work-on-next-issue`, `work-on-this`,
`update-nuget-packages`) do not enforce working on a feature branch before committing, which
causes problems when branch protection is added.

## Solution

1. Add a `renovate.json` that automates analyzer package bumps: Renovate opens a weekly grouped
   PR updating both `Directory.Packages.props` and the `.nuspec` atomically, the existing
   `analyzer-bump.yml` pipeline regenerates editorconfigs and runs the full test suite, and
   GitHub automerges when all checks pass.
2. Enable branch protection on `main` (required status checks, no required reviewers) so that
   the automerge gate is CI-driven.
3. Delete the now-redundant `outdated.yml` Azure Pipelines scheduled run.
4. Update `AGENTS.md` and the committing/file-modifying skills to enforce feature-branch
   discipline: skills never commit to `main`, and branches are named after the PRD or issue
   they belong to.

## User Stories

1. As a maintainer, I want analyzer packages bumped automatically each week, so that I don't
   have to manually check `dotnet outdated` or remember to update both version files.
2. As a maintainer, I want both `Directory.Packages.props` and the `.nuspec` updated atomically
   in the same Renovate PR, so that the `CheckNugetDependenciesMatchProps.cs` CI check never
   fails on a Renovate PR.
3. As a maintainer, I want all five analyzer packages grouped into a single Renovate PR, so that
   the `dotnet outdated --fail-on-updates` CI check doesn't fail when one package updates while
   another is still pending.
4. As a maintainer, I want Renovate PRs to automerge once all CI checks are green, so that
   routine analyzer bumps land in `main` without manual intervention.
5. As a maintainer, I want the release cycle to remain tag-driven, so that automerging into
   `main` never publishes to NuGet.org without my explicit intent to tag a release.
6. As a maintainer, I want branch protection on `main` with required status checks, so that
   no commit — including Renovate's automerge — can land without the full CI pipeline passing.
7. As a maintainer, I want no required PR reviewer approvals, so that Renovate automerge
   works unattended while I retain full control via the release tag gate.
8. As a maintainer, I want the redundant `outdated.yml` scheduled pipeline removed, so that
   there is a single mechanism (Renovate) for tracking outdated packages.
9. As an AI agent, I want to be prevented from committing to `main`, so that branch protection
   never rejects my pushes.
10. As an AI agent working on issues that share a parent PRD, I want to commit to a shared
    feature branch derived from the PRD filename, so that related issues accumulate on one branch
    and land as a single PR.
11. As an AI agent working on a standalone issue (no parent PRD), I want to commit to a feature
    branch derived from the issue filename, so that each independent issue has its own branch.
12. As an AI agent, I want to be warned and default to refusing when invoked on an unexpected
    feature branch, so that I never accidentally mix unrelated issues onto the same branch.
13. As an AI agent running `update-nuget-packages`, I want a feature branch created automatically
    before any files are modified (only when updates exist), so that version bumps never land as
    uncommitted changes on `main`.

## Implementation Decisions

- **`renovate.json` at repo root** with:
  - `enabledManagers: ["nuget", "regex"]` — prevents Renovate touching Azure Pipelines YAML
    or any other file type.
  - Built-in `nuget` manager handles `Directory.Packages.props` automatically.
  - A `regexManagers` entry targets the `.nuspec`, matching
    `<dependency id="..." version="..."/>` with `datasourceTemplate: "nuget"` and
    `versioningTemplate: "nuget"`.
  - A `packageRules` entry with `groupName: "analyzer packages"` groups all five packages
    into a single PR.
  - `schedule: ["before 9am on Monday"]` — weekly cadence, replaces `outdated.yml`.
  - `automerge: true` and `platformAutomerge: true` — uses GitHub's native automerge,
    triggered only after all required status checks pass.

- **Delete `.azure-pipelines/outdated.yml`** — Renovate supersedes it entirely.

- **Branch protection on `main` (GitHub settings — manual step)**:
  - Require status checks to pass before merging; add the CI pipeline check as required
    (exact check name identified from the Checks tab of an existing PR).
  - Require branches to be up to date before merging.
  - No required reviewer approvals.

- **`AGENTS.md` — `Git & boundaries` section** strengthened with:
  - Explicit prohibition on committing directly to `main`.
  - Branch naming convention: `feat/<prd-slug>` for PRD-linked work (all issues sharing a
    PRD use the same branch); `feat/NNN-issue-slug` for standalone issues.

- **`work-on-next-issue` skill** — new branch-check step inserted before the commit step:
  1. Read the issue's `Parent PRD` field; derive expected branch from the PRD filename
     (strip `issues/` prefix and `.md` suffix, prepend `feat/`). If no parent PRD, derive
     from the issue filename instead.
  2. If currently on `main` → create/checkout the expected branch and proceed.
  3. If already on the expected branch → proceed.
  4. If on a different feature branch → ask the user what to do; default is to refuse
     (stop and ask them to switch branches manually).

- **`work-on-this` skill** — same branch-check step before the commit step:
  - Derive branch name by slugifying the first five words of the task description,
    prepend `feat/`.
  - Same three-way logic: `main` → create, correct branch → proceed, unexpected → ask/refuse.

- **`update-nuget-packages` skill** — after confirming that updates exist (step 2), before
  modifying any files:
  - Create and switch to `feat/bump-analyzers-YYYY-MM-DD` (today's date).
  - If the branch already exists (e.g. a prior run on the same day), switch to it and
    continue; do not create a duplicate.

## Testing Decisions

This PRD is entirely configuration, documentation, and skill (prompt) changes — no production
code or test-harness code is modified. There are no automated tests to write.

Manual verification steps for each deliverable:
- `renovate.json`: dry-run via `npx renovate --dry-run` or inspect the first Renovate PR to
  confirm both `Directory.Packages.props` and the `.nuspec` are updated together.
- Branch protection: attempt a direct push to `main` and confirm it is rejected.
- Skill branch logic: invoke `work-on-next-issue` while on `main` and confirm it creates the
  correct feature branch; invoke it while on an unexpected branch and confirm it asks before
  proceeding.

## Out of Scope

- Bumping non-analyzer dependencies (`xunit`, `Shouldly`, `CliWrap`, `Microsoft.NET.Test.Sdk`).
- Automerging non-Renovate PRs (human-authored PRs still require manual merge).
- Adding GitHub Actions workflows (CI remains on Azure Pipelines).
- Changing any rule severities or editorconfig contents.
- Creating or modifying the `prd-to-issues` or `write-a-prd` skills (they write files but do
  not commit, so branch discipline is the committer's responsibility).

## Further Notes

- The `analyzer-bump.yml` pipeline already handles the regeneration workflow: it fires on any
  PR touching `Directory.Packages.props` or the `.nuspec`, regenerates editorconfigs, commits
  back to the PR branch, then runs the full build and test suite. Renovate PRs will trigger
  this pipeline automatically — no changes to `analyzer-bump.yml` are needed.
- Branch protection requires knowing the exact Azure Pipelines check name as it appears in
  GitHub. Identify it from the Checks tab of any merged PR before configuring required checks.
- `StyleCop.Analyzers` uses pre-release versions (`1.2.0-beta.xxx`). Renovate's `nuget`
  versioning tracks the highest version including pre-release when the current pinned version
  is itself a pre-release, so no special `allowedVersions` config is needed.
- The `platformAutomerge: true` setting relies on GitHub's native auto-merge feature being
  available on the repository (it is enabled by default for public repos on GitHub.com).
