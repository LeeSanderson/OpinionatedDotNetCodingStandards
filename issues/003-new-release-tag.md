## Parent PRD

`issues/prd-release-tagging-scripts.md`

## What to build

Create `scripts/New-ReleaseTag.ps1`. The script validates the repo is in a releasable state, calculates the next semantic version, and pushes a `v*` tag to trigger the Azure Pipelines release pipeline. Tags must be in `vMAJOR.MINOR.PATCH` format (the `v` prefix is mandatory — the release pipeline extracts the version via `$(Build.SourceBranch) -replace "refs/tags/v", ""`).

**Parameters:** Three mutually exclusive switches `-Major`, `-Minor`, `-Patch`. Exactly one must be supplied; the script errors if none or more than one is provided. An additional `-Force` switch skips the changelog check.

**Step order (per PRD):**
1. Assert current branch is `main`; exit with error if not.
2. `git pull` to fast-forward to the latest remote state.
3. `dotnet ./scripts/CheckNugetDependenciesMatchProps.cs`; exit on non-zero.
4. `dotnet ./scripts/GenerateRuleReference.cs --check`; exit on non-zero.
5. Find the highest existing `v*` tag sorted by semver (numeric, not lexicographic); default to `v0.0.0` if no tags exist.
6. Increment the appropriate version component; reset lower components to zero (e.g. `-Minor` on `v1.2.3` → `v1.3.0`).
7. Unless `-Force`, scan `CHANGELOG.md` for a heading matching `## [<version>]` (case-insensitive, tolerates date suffix); exit with error if not found.
8. `dotnet test`; exit on non-zero.
9. `git tag <new-version>`.
10. `git push origin <new-version>`.

No confirmation prompts before any step. All error messages must be clear and actionable.

## Acceptance criteria

- [ ] `scripts/New-ReleaseTag.ps1` exists alongside the existing scripts in `scripts/`
- [ ] `-Major`, `-Minor`, `-Patch` are mutually exclusive; passing none or more than one produces a clear error
- [ ] Script refuses to run on any branch other than `main`
- [ ] Script pulls latest before doing anything else
- [ ] Dependency sync check (`CheckNugetDependenciesMatchProps.cs`) runs and blocks on failure
- [ ] Rule reference check (`GenerateRuleReference.cs --check`) runs and blocks on failure
- [ ] Changelog check finds `## [<version>]` headings case-insensitively and tolerates date suffixes (e.g. `## [1.2.0] - 2026-06-22`)
- [ ] `-Force` skips the changelog check
- [ ] Full test suite (`dotnet test`) runs and blocks on failure
- [ ] Tag is only created and pushed after all validation passes
- [ ] First release works with no existing tags (defaults implied previous version to `v0.0.0`)
- [ ] Semver sorting is numeric (e.g. `v1.10.0` sorts above `v1.9.0`)
- [ ] Pushed tag is in `vMAJOR.MINOR.PATCH` format

## Blocked by

None — can start immediately. Completing `issues/001-delete-xunit-runner-json.md` first is recommended so the `dotnet test` step in this script runs at full parallelism.

## User stories addressed

- User story 1
- User story 2
- User story 3
- User story 4
- User story 5
- User story 6
- User story 7
- User story 8
- User story 9
- User story 10
- User story 11
- User story 12
- User story 13
- User story 14
