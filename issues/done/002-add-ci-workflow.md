## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Add `.github/workflows/ci.yml` — a single, read-only PR workflow that replaces the Azure
`ci.yml` **and** `analyzer-bump.yml`. It **checks, never fixes**. See the "`ci.yml` trigger
and structure" and "CI is read-only; no commit-back" implementation decisions in the parent
PRD.

One `build` job runs on `pull_request` → `main`, on `windows-latest`, with
`permissions: contents: read`. It skips draft PRs (`if: github.event.pull_request.draft == false`)
and uses a `concurrency` group on the head ref with `cancel-in-progress: true` (parity with
Azure `autoCancel`). Steps: checkout → `actions/setup-dotnet` (honours `global.json`) →
`actions/cache@v4` for NuGet (key ported from Azure `Cache@2`: `Directory.Packages.props` +
`**/*.csproj`) → restore → `dotnet outdated --fail-on-updates` → nuspec/props dependency-sync
check (`scripts/CheckNugetDependenciesMatchProps.cs`) → build (`Release`) → **freshness
checks** → test (exit-code gating only) → pack.

The freshness checks regenerate the analyzer editorconfigs
(`scripts/UpdateAnalyzerEditorConfigs.cs`) and then the rule reference
(`scripts/GenerateRuleReference.cs`) into the working tree and fail on `git diff --exit-code`
over the generated editorconfig directory and `docs/`. Editorconfigs are regenerated before
the rule reference so the docs check reflects the actually-referenced analyzers. No commit,
no push — a stale tree fails the PR and the author regenerates locally.

The job is named `build` because that becomes the required status-check context (see issue
`issues/005-github-settings-secret-and-required-check.md`).

## Acceptance criteria

- [ ] `.github/workflows/ci.yml` exists with a single `build` job on `windows-latest`, triggered by `pull_request` to `main`.
- [ ] The job declares `permissions: contents: read` and performs no `git push` / commit-back.
- [ ] Draft PRs are skipped; superseded in-progress runs on the same head ref are cancelled.
- [ ] The job runs, in order: restore (cached), outdated check, dependency-sync check, build (`Release`), editorconfig + `docs/` freshness checks, test, pack.
- [ ] Opening the migration PR shows the `build` job green, with parity to the Azure CI outcome.
- [ ] A PR that leaves the editorconfigs or `docs/rule-reference.md` stale fails the `build` job on the freshness diff with an actionable message.
- [ ] A fork PR builds and tests with read-only permissions and never attempts to push.

## Blocked by

- Blocked by `issues/001-add-global-json-sdk-pin.md`

## User stories addressed

- User story 2
- User story 3
- User story 4
- User story 5
- User story 6
- User story 7
- User story 8
- User story 9
- User story 15
- User story 16
- User story 18
- User story 19
- User story 20
- User story 21
