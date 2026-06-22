# PRD: Release Tagging Scripts

## Problem Statement

Creating a release currently requires the developer to manually determine the next semantic
version, verify the repo is in a releasable state, construct the correct `v*` tag format,
and push it to trigger the Azure Pipelines release pipeline. There is no guardrail against
tagging on the wrong branch, with stale docs, with a missing changelog entry, or while
tests are failing. There is also no fast recovery path when a pipeline run fails and the
tag needs to be retracted.

## Solution

Two PowerShell scripts:

1. **`scripts/New-ReleaseTag.ps1`** — validates the repo is releasable, calculates the
   next semantic version, and pushes a `v*` tag to trigger the release pipeline.
2. **`scripts/Remove-LastReleaseTag.ps1`** — deletes the highest semver tag both locally
   and from the remote, providing a recovery path when a pipeline run fails.

As a prerequisite, `tests/Opinionated.DotNet.CodingStandards.Tests/xunit.runner.json`
is deleted permanently. That file limited `maxParallelThreads` to 4 for CI stability under
the old xUnit v2 collection model. With xUnit v3's class-level parallelism, the cap is
unnecessary and causes local test runs to take longer than they should; without it, the
suite completes in approximately 6 minutes locally.

## User Stories

1. As a maintainer, I want to increment the patch version and push a release tag with a
   single command, so that I do not need to remember the current version or the tag format.
2. As a maintainer, I want to increment the minor version and push a release tag with a
   single command, resetting the patch component to zero automatically.
3. As a maintainer, I want to increment the major version and push a release tag with a
   single command, resetting both minor and patch components to zero automatically.
4. As a maintainer, I want the script to refuse to run on any branch other than `main`,
   so that I cannot accidentally trigger a release from a feature branch.
5. As a maintainer, I want the script to pull the latest changes before doing anything
   else, so that the tag always points at the most recent commit on `main`.
6. As a maintainer, I want the script to verify that NuGet dependency versions in the
   `.nuspec` match `Directory.Packages.props`, so that I cannot release a package with
   inconsistent dependency declarations.
7. As a maintainer, I want the script to verify that the rule reference doc is up to date,
   so that I cannot release a package whose documentation is stale.
8. As a maintainer, I want the script to verify that a changelog section for the new
   version exists before running tests, so that I get a fast failure rather than waiting
   6 minutes for tests before discovering the entry is missing.
9. As a maintainer, I want the script to run the full test suite before tagging, so that
   I cannot push a release tag for a build that would fail.
10. As a maintainer, I want the script to error if no changelog entry exists for the new
    version, so that I am reminded to document the release before tagging.
11. As a maintainer, I want to pass `-Force` to skip the changelog check, so that I can
    tag a release in exceptional circumstances without being blocked.
12. As a maintainer, I want the script to default to `v0.0.0` as the implied previous
    version when no tags exist, so that the first release can be created with
    `-Major`, `-Minor`, or `-Patch` without any special-casing.
13. As a maintainer, I want to see a clear, actionable error message when any validation
    step fails, so that I know exactly what needs to be fixed before retrying.
14. As a maintainer, I want the tag to be created locally and pushed to the remote only
    after all validation passes, so that a broken remote tag is never created.
15. As a maintainer, I want to retract the highest semver tag (locally and remotely) with
    a single command, so that I can quickly recover from a failed pipeline run without
    looking up the tag name manually.
16. As a maintainer, I want the delete script to tell me which tag it deleted, so that I
    can confirm the right tag was removed.
17. As a maintainer, I want the test suite to run faster locally now that the parallelism
    cap in `xunit.runner.json` has been removed, so that the release script's test step
    completes in approximately 6 minutes rather than longer.

## Implementation Decisions

- **Parameters for `New-ReleaseTag.ps1`.** Three mutually exclusive PowerShell switches:
  `-Major`, `-Minor`, `-Patch`. An additional `-Force` switch skips the changelog check.
  Exactly one of `-Major`, `-Minor`, `-Patch` must be supplied; the script errors if none
  or more than one is provided.

- **Step order for `New-ReleaseTag.ps1`.**
  1. Assert the current branch is `main`; exit with error if not.
  2. `git pull` to fast-forward to the latest remote state.
  3. Run `dotnet ./scripts/CheckNugetDependenciesMatchProps.cs`; exit on non-zero.
  4. Run `dotnet ./scripts/GenerateRuleReference.cs --check`; exit on non-zero.
  5. Determine the highest existing `v*` tag sorted by semver; default to `v0.0.0` if
     the repo has no tags.
  6. Increment the appropriate version component per the supplied switch; lower components
     are reset to zero (e.g. `-Minor` on `v1.2.3` → `v1.3.0`).
  7. Unless `-Force`, scan `CHANGELOG.md` for a section heading that matches the new
     version (e.g. `## [1.2.0]`); exit with error if not found.
  8. Run `dotnet test`; exit on non-zero.
  9. `git tag <new-version>`.
  10. `git push origin <new-version>`.

- **Changelog detection.** The script scans `CHANGELOG.md` for a line matching
  `## [<version>]` (with or without a leading `v`). The check is case-insensitive and
  tolerates the date suffix that Keep a Changelog appends (e.g. `## [1.2.0] - 2026-06-22`).

- **Step order for `Remove-LastReleaseTag.ps1`.** No parameters.
  1. Find all local tags matching `v*`, sort by semver descending, take the highest; exit
     with error if no tags exist.
  2. `git tag -d <tag>` to delete locally.
  3. `git push origin --delete <tag>` to delete from the remote.
  4. Print a confirmation message naming the deleted tag.

- **Semver sorting.** Both scripts sort tags by splitting on `.`, casting each component
  to `[int]`, and comparing numerically — not lexicographically. This avoids `v1.9.0`
  sorting after `v1.10.0`.

- **No confirmation prompts.** Neither script prompts the user before performing remote
  operations. The delete script is the intended recovery path for the create script, so
  the pair is the safety mechanism.

- **Script location.** Both scripts live in `scripts/` alongside the existing dotnet
  file-based scripts.

- **Delete `xunit.runner.json`.** Remove
  `tests/Opinionated.DotNet.CodingStandards.Tests/xunit.runner.json` from the repo. No
  replacement is needed; xUnit v3 defaults to class-level parallelism with degree equal
  to the logical processor count, which is correct for both local and CI runs.

## Testing Decisions

The scripts are developer tooling, not library code, so they are not covered by the
xUnit integration test suite. Correctness is verified by running the scripts themselves:

- **`New-ReleaseTag.ps1`** — run once on `main` after the PR is merged; observe that all
  validation steps pass and that the tag appears on the remote, triggering the Azure
  Pipelines release pipeline.
- **`Remove-LastReleaseTag.ps1`** — verify manually by running it after a test tag is
  pushed and confirming the tag is gone locally (`git tag`) and remotely
  (`git ls-remote --tags origin`).

The deletion of `xunit.runner.json` is validated by the existing test suite: run
`dotnet test` after removing the file and confirm all tests pass and that wall-clock
time is approximately 6 minutes.

## Out of Scope

- Automatically updating `CHANGELOG.md` — maintainers update the changelog in a PR before
  tagging; branch protection prevents the release script from committing directly to `main`.
- GitHub Releases / release notes generation — the pipeline publishes to NuGet only.
- Automated rollback of the NuGet push if the pipeline fails mid-way — `dotnet nuget push`
  uses `--skip-duplicate`; a failed push does not need tag retraction.
- Signing or notarising the tag (`git tag -s`) — not currently required.
- Parallelism tuning after removing `xunit.runner.json` — if contention is observed on CI,
  a new `xunit.runner.json` can be added at that time.

## Further Notes

- The release pipeline (`release.yml`) extracts the version from the tag via
  `$(Build.SourceBranch) -replace "refs/tags/v", ""`. The `v` prefix is mandatory; the
  script must always produce tags in `vMAJOR.MINOR.PATCH` format.
- Azure Pipelines variable group `SharedVariables` holds the `NuGetApiKey` secret; the
  release script does not need API key access — it only pushes the git tag.
- Branch for this work: `feat/prd-release-tagging-scripts`.
