# PRD: Migrate CI/CD from Azure Pipelines to GitHub Actions

## Problem Statement

The repository is hosted on GitHub but its CI/CD runs on Azure DevOps Pipelines via the
Azure Pipelines GitHub App. Three pipelines live under `.azure-pipelines/`:
`ci.yml` (build/test/pack on PRs to `main`), `release.yml` (build/test/pack/publish to
NuGet.org on `v*` tag push), and `analyzer-bump.yml` (regenerate editorconfigs and the
rule reference on dependency-bump PRs, commit the result back to the PR branch, then
build/test). Splitting the source host (GitHub) from the CI host (Azure DevOps) means two
systems to configure, a separate secret store, a separate GitHub App with repo access, and
status checks that originate outside GitHub. The maintainer wants CI/CD to live next to the
code on GitHub Actions, which — for a public repository using standard GitHub-hosted
runners — is free with unlimited minutes.

## Solution

Port all three Azure pipelines to GitHub Actions under `.github/workflows/`, consolidating
to **two** workflow files, and add a `global.json` to pin the SDK. The migration is
performed **parallel-then-cut**: the new workflows are added while the Azure pipelines
remain in place, and `.azure-pipelines/` is deleted only after the Actions workflows have
each had a green run.

1. **`.github/workflows/ci.yml`** — merges the old `ci.yml` and `analyzer-bump.yml` into a
   single PR workflow with one `build` job. It **checks, never fixes**: it builds, tests,
   and packs, and fails the PR when dependencies are outdated, when the `.nuspec` and
   `Directory.Packages.props` disagree, or when the analyzer editorconfigs / rule-reference
   documentation are stale relative to the referenced analyzer packages. Regenerating those
   generated files is the author's responsibility (done locally, e.g. via the
   `update-nuget-packages` skill), not CI's.
2. **`.github/workflows/release.yml`** — ports the tag-triggered release pipeline:
   build, test, pack with the tag version, and publish to NuGet.org using a stored API key.
3. **`global.json`** — pins the .NET SDK so CI and local builds resolve the same toolchain.

Premise validated during planning: GitHub Actions usage is free with no minute cap for
**public** repositories on **standard** GitHub-hosted runners (Linux/Windows/macOS). Only
*larger* runners are billed, and none are used here. (GitHub reduced hosted-runner prices
on 2026-01-01 but kept public-repo standard-runner usage free.)

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.114 | 3.0.115 |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0209 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0210 | Analyzer.Meziantou.Analyzer.editorconfig | Added |

## User Stories

1. As a maintainer, I want CI/CD defined in the same repository as the code, so that I
   manage one system instead of two.
2. As a maintainer, I want pull requests to `main` to build, test, and pack on GitHub
   Actions, so that I get the same gate I had on Azure Pipelines.
3. As a maintainer, I want the PR workflow to run the analyzer "outdated" check
   (`dotnet outdated --fail-on-updates`), so that a PR cannot merge while dependencies are
   stale.
4. As a maintainer, I want the PR workflow to verify the `.nuspec` dependency versions
   match `Directory.Packages.props`, so that I cannot merge an inconsistent package
   manifest.
5. As a maintainer, I want the PR workflow to regenerate the rule reference and fail on a
   diff, so that `docs/rule-reference.md` can never drift from the editorconfigs.
6. As a maintainer, I want CI to fail when the analyzer editorconfigs are stale relative to
   the referenced analyzer packages, so that a dependency bump that introduces new rules
   cannot merge until the editorconfigs are regenerated.
7. As a maintainer, I want CI to fail when `docs/rule-reference.md` is stale, so that the
   published rule documentation always matches the editorconfigs.
8. As a maintainer, I want CI to *check* these generated files rather than auto-fixing and
   committing them, so that CI never writes to my branches and every change is authored
   deliberately.
9. As a maintainer, I want bump PRs to show a single set of checks rather than two
   overlapping workflows, so that the PR status is unambiguous.
10. As a maintainer, I want pushing a `v*` tag to build, test, pack with the tag's version,
    and publish to NuGet.org, so that releasing stays a single `git push` of a tag.
11. As a maintainer, I want the release workflow to read the NuGet API key from GitHub's
    encrypted secret store, so that the credential never lives in the repository.
12. As a maintainer, I want the published package version to come from the tag name, so
    that the tag is the single source of truth for the release version.
13. As a maintainer, I want the release push to use `--skip-duplicate`, so that re-running a
    release (or an overlapping Azure release during cutover) is a safe no-op.
14. As a maintainer, I want the `.nupkg` uploaded as a build artifact on release, so that I
    can retrieve exactly what was published.
15. As a maintainer, I want the PR workflow to cancel superseded in-progress runs on the
    same branch, so that I do not waste runner time on stale commits (parity with Azure
    `autoCancel`).
16. As a maintainer, I want draft PRs to skip CI, so that work-in-progress does not consume
    runners (parity with Azure `drafts: false`).
17. As a maintainer, I want CI and local builds to resolve the same SDK via `global.json`,
    so that "works on my machine" matches "works in CI".
18. As a maintainer, I want the NuGet restore cached across runs, so that the main-solution
    restore is not re-downloaded every build (parity with Azure `Cache@2`).
19. As a maintainer, I want the workflows to run on `windows-latest`, so that the only thing
    changing during migration is the CI host, not the runner OS.
20. As a maintainer, I want the PR workflow to declare least-privilege `contents: read`
    permissions, so that CI cannot write to the repository at all.
21. As a contributor opening a PR from a fork, I want CI to build and test my change with no
    special permissions, so that external contributions are validated safely (CI is
    read-only and never pushes).
22. As a consumer of the README, I want an accurate build-status badge that reflects GitHub
    Actions, so that the badge links to the real CI.
23. As a reader of the README, I want the repository-layout section to list the actual
    workflow files, so that the documentation matches the tree.
24. As a maintainer, I want the Azure pipelines retained until the Actions workflows are
    proven green, so that the migration never leaves `main` without a working gate.
25. As a maintainer, I want a clear list of the one-time manual GitHub settings changes
    (secret creation, branch-protection update, app removal), so that I can complete the
    cutover without guesswork.
26. As a maintainer, I want a documented path to move the release to OIDC trusted
    publishing later, so that the stored API key can eventually be eliminated.

## Implementation Decisions

- **Two workflow files replace three pipelines.** `ci.yml` (PR) and `release.yml` (tag).
  The Azure `ci.yml` and `analyzer-bump.yml` collapse into a single PR `build` job: with the
  commit-back dropped, `analyzer-bump.yml`'s only remaining responsibility is the freshness
  checks, which run on every PR — so no separate workflow or bump-path gating is needed.

- **`ci.yml` trigger and structure.**
  - Trigger: `pull_request` targeting `main`. Draft PRs are excluded with a job-level
    `if: github.event.pull_request.draft == false`. A `concurrency` group keyed on the head
    ref with `cancel-in-progress: true` reproduces Azure `autoCancel`.
  - **Single `build` job**, `permissions: contents: read` (read-only — CI never writes to
    the repository). Steps: checkout, set up the SDK, restore with caching, run the outdated
    check (`dotnet outdated --fail-on-updates`), run the nuspec/props dependency-sync check,
    build in `Release`, then the freshness checks (below), run the tests, and pack.
  - **Freshness checks (check, never fix).** The job regenerates the analyzer editorconfigs
    (`dotnet ./scripts/UpdateAnalyzerEditorConfigs.cs`) and then the rule reference
    (`dotnet ./scripts/GenerateRuleReference.cs`) into the working tree and fails on a
    `git diff --exit-code` over the generated editorconfig directory and `docs/`.
    Editorconfigs are regenerated *before* the rule reference so the documentation check
    reflects the actually-referenced analyzers, not stale config. No commit, no push — a
    stale tree fails the PR and the author regenerates locally.

- **Required status check is the `build` job.** Branch protection requires the context named
  `build` (the workflow's only PR job).

- **CI is read-only; no commit-back.** The PR workflow never writes to the repository — it
  checks freshness and fails on drift rather than regenerating and pushing. This removes the
  need for `contents: write`, head-branch checkout, `GITHUB_TOKEN` push, and any fork-PR
  special casing; `pull_request_target` is not used.

- **`release.yml` trigger and steps.** Trigger: `push` of tags matching `v*`. The package
  version is derived from the tag name (`GITHUB_REF_NAME` with the leading `v` stripped).
  Steps: set up SDK, cache, restore, build (`Release`), test, `dotnet pack` of the package
  project with `-p:NuspecProperties=version=<version>`, upload the `.nupkg` as an artifact,
  then `dotnet nuget push ... --api-key ${{ secrets.NUGET_API_KEY }} --skip-duplicate`.
  `permissions: contents: read`.

- **Publish authentication: stored API key now.** A GitHub **repository secret** named
  `NUGET_API_KEY` replaces the Azure variable-group `SharedVariables.NuGetApiKey`. OIDC
  trusted publishing is deferred (see Out of Scope / Further Notes); `release.yml` carries a
  `# TODO(OIDC)` marker at the publish step.

- **Runner OS: `windows-latest`** for all jobs, matching the current Azure pipelines so the
  migration changes one variable (the CI host) at a time.

- **SDK pinning via `global.json`.** A new `global.json` pins the SDK to the installed
  `10.0.101` with `rollForward: latestPatch` (accept patch updates, no feature-band drift).
  `actions/setup-dotnet` honours it; the `includePreviewVersions` flag from Azure is dropped
  because .NET 10 is GA.

- **NuGet caching via `actions/cache@v4`.** Faithful port of Azure `Cache@2`: caches the
  user NuGet packages folder keyed on `Directory.Packages.props` + `**/*.csproj`. Note this
  speeds up only the main-solution restore; the integration tests point each throwaway
  project at a per-fixture temporary `globalPackagesFolder`, so the test phase is unaffected
  (unchanged from today).

- **Test results: exit code only.** No test-reporter action and no result artifacts;
  `dotnet test` failing the job is the gate. Failures remain visible in the job log.

- **README updates in the same PR.** Replace the Azure DevOps build-status badge with a
  GitHub Actions badge for `ci.yml`, and correct the "Repository layout" section to list
  `.github/workflows/` and the real workflow files (the current block references a
  non-existent `outdated.yml` and omits `release.yml`/`analyzer-bump.yml`).

- **Azure decommission is a follow-up step, not part of the initial add.** `.azure-pipelines/`
  is deleted only after the new workflows have each gone green (parallel-then-cut).

- **Manual GitHub settings changes owned by the maintainer (documented, not automated):**
  1. Create the `NUGET_API_KEY` repository secret.
  2. Update `main` branch protection: add the `build` required context (keeping the existing
     Azure `OpinionatedDotNetCodingStandards` context during the parallel phase), then remove
     the Azure context at cutover. `strict: true` is preserved throughout.
  3. After cutover, remove the Azure Pipelines GitHub App's repository access.

## Testing Decisions

This change is CI/CD configuration, not library code, so it is **not** covered by the xUnit
integration suite — mirroring the precedent set by the release-tagging-scripts PRD, where
developer tooling was verified by running it rather than by unit tests. The new workflows
*invoke* the existing test suite, so the suite continues to be exercised on every PR; what
needs validating here is that the workflows are wired correctly. Validation is by observed
workflow runs during the parallel phase, when Azure remains the safety net:

- **`ci.yml` (normal PR)** — open the migration PR itself; confirm the `build` job goes
  green and that its build/test/pack/freshness/outdated/dependency-sync steps match the
  Azure run's outcome.
- **`ci.yml` (bump PR, generated files fresh)** — open a PR that bumps an analyzer and
  regenerates the editorconfigs/rule reference locally; confirm the `build` job's freshness
  checks pass.
- **`ci.yml` (bump PR, generated files stale)** — bump an analyzer *without* regenerating;
  confirm the `build` job fails on the editorconfig/`docs/` diff with an actionable message.
- **`ci.yml` (fork PR)** — confirm the `build` job builds and tests with read-only
  permissions and never attempts to push.
- **`release.yml`** — validated on the next real release: a `v*` tag triggers both the
  Azure and Actions release workflows during the parallel phase; `--skip-duplicate` makes
  the second push a no-op, so the Actions release can prove itself with Azure as backstop.
  Confirm the artifact contains the correctly versioned `.nupkg`.

A good test here asserts on observable workflow behaviour — the package published, the files
committed back, the gate passing/failing — never on internal step wiring.

## Out of Scope

- **Auto-regeneration / commit-back of generated files.** CI *checks* that the analyzer
  editorconfigs and rule reference are fresh and fails on drift; it does not regenerate or
  commit them. Regeneration is the author's job (e.g. via the `update-nuget-packages` skill)
  and is committed in the PR. This intentionally drops the old Azure `analyzer-bump.yml`
  commit-back behaviour.
- **OIDC trusted publishing for NuGet.org.** Deferred to a future PRD; the API-key secret
  is used initially. A `# TODO(OIDC)` marker is left at the publish step.
- **Switching the runner OS to Linux.** A potential follow-up optimisation (faster, larger
  pool); kept on `windows-latest` here to isolate the migration to a single variable.
- **A larger/self-hosted runner.** Not needed; standard runners are free for this public
  repo.
- **Committing `packages.lock.json` / `RestorePackagesWithLockFile`.** Would be required to
  use `setup-dotnet`'s built-in cache; out of scope — `actions/cache@v4` is used instead.
- **A test-reporter action or PR test summary.** Exit-code gating only.
- **GitHub Environments / publish approval gate.** The deliberate `v*` tag push (via
  `New-ReleaseTag.ps1`, which already runs the full suite and changelog check) remains the
  human gate.
- **Fixing the README reference to the non-existent `CheckRuleReferenceFreshness.cs`
  script.** A pre-existing documentation bug unrelated to the CI host; not addressed here.
- **Automated branch-protection or secret changes.** These are GitHub settings the
  maintainer applies manually; this PRD documents the exact changes only.

## Further Notes

- **Parallel-release safety.** While both Azure and Actions release pipelines exist, a `v*`
  tag triggers both. Because `dotnet nuget push` uses `--skip-duplicate`, whichever publishes
  first wins and the other is a no-op — making the parallel phase a safe way to prove the
  Actions release before deleting the Azure one.
- **Branch protection currently requires** the context `OpinionatedDotNetCodingStandards`
  from the Azure Pipelines GitHub App (`app_id 9426`) with `strict: true`. The new required
  context will be `build`. Do not remove the Azure context until Azure is decommissioned, or
  PRs will block on a check that no longer reports.
- **The `v` tag prefix is mandatory** — `release.yml` strips a leading `v` to derive the
  version, exactly as the Azure pipeline did and as `New-ReleaseTag.ps1` produces.
- **`GeneratePackageOnBuild` is enabled**, so the solution packs on every build; the explicit
  `pack` step in `ci.yml` is retained for parity with the Azure pipeline.
- Branch for this work: `feat/prd-migrate-to-github-actions`.
