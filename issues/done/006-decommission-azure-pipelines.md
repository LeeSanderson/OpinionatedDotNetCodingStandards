## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

The **cutover** step of the parallel-then-cut migration: retire Azure DevOps once the GitHub
Actions workflows have each had a green run. See the "Azure decommission is a follow-up step"
implementation decision and the "Further Notes" branch-protection caveat in the parent PRD.
**HITL** — requires human verification of the parallel runs plus manual GitHub settings
changes.

Only after `ci.yml` has gone green on a PR and `release.yml` has been proven on a real `v*`
tag (Azure acting as backstop, `--skip-duplicate` making the overlapping push a no-op):

1. Remove the Azure `OpinionatedDotNetCodingStandards` required status-check context from
   `main` branch protection, leaving only `build` (keep `strict: true`).
2. Delete the `.azure-pipelines/` directory (`ci.yml`, `release.yml`, `analyzer-bump.yml`).
3. Remove the Azure Pipelines GitHub App's access to the repository.

## Acceptance criteria

- [x] `ci.yml` has had at least one green run on a PR and `release.yml` has been validated on a real release before any teardown.
- [x] `main` branch protection requires only the `build` context (Azure context removed), `strict: true` retained.
- [x] The `.azure-pipelines/` directory is deleted from the repo.
- [x] The Azure Pipelines GitHub App no longer has access to the repository.
- [x] After teardown, a fresh PR is gated solely by the Actions `build` check and a release publishes solely via the Actions `release.yml`.

## Blocked by

- Blocked by `issues/002-add-ci-workflow.md`
- Blocked by `issues/003-add-release-workflow.md`
- Blocked by `issues/004-update-readme-badge-and-layout.md`
- Blocked by `issues/005-github-settings-secret-and-required-check.md`

## User stories addressed

- User story 24
- User story 25
