## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Apply the one-time GitHub settings needed for the Actions workflows to function during the
**parallel phase**, before the Azure pipelines are removed. See the "Manual GitHub settings
changes owned by the maintainer" implementation decision in the parent PRD. **HITL** — these
are manual GitHub UI / `gh api` actions, not code changes.

1. Create a repository secret **`NUGET_API_KEY`** (Settings → Secrets and variables →
   Actions) holding the existing NuGet.org API key — the GitHub equivalent of the Azure
   variable-group `SharedVariables.NuGetApiKey`, consumed by `release.yml`.
2. Update `main` branch protection to **add** the `build` required status-check context
   **alongside** the existing Azure `OpinionatedDotNetCodingStandards` context, preserving
   `strict: true`. During the parallel phase PRs must pass both; do not remove the Azure
   context yet (that happens at cutover in issue
   `issues/006-decommission-azure-pipelines.md`). Removing it prematurely, or adding `build`
   before the job has reported once, can block PRs.

## Acceptance criteria

- [ ] A `NUGET_API_KEY` repository secret exists with the correct NuGet.org key value (verified by a successful publish path, not by reading the value).
- [ ] `main` branch protection lists both `build` and `OpinionatedDotNetCodingStandards` as required status-check contexts, with `strict: true` retained.
- [ ] A test PR confirms both required checks must pass before merge is allowed.
- [ ] No secret value is written into the repository or any workflow file.

## Blocked by

- Blocked by `issues/002-add-ci-workflow.md`
- Blocked by `issues/003-add-release-workflow.md`

## User stories addressed

- User story 11
- User story 25
