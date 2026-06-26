## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Update `README.md` to reflect the GitHub Actions migration. See the "README updates in the
same PR" implementation decision in the parent PRD.

1. Replace the Azure DevOps build-status badge (the `dev.azure.com/sixsideddice` image) with
   a GitHub Actions badge for the `ci.yml` workflow
   (`https://github.com/LeeSanderson/OpinionatedDotNetCodingStandards/actions/workflows/ci.yml/badge.svg`,
   linking to the workflow's Actions page).
2. Correct the "Repository layout" section: replace the `.azure-pipelines/` block (which
   currently lists a non-existent `outdated.yml` and omits `release.yml`) with a
   `.github/workflows/` block listing the real workflow files (`ci.yml`, `release.yml`).

Out of scope for this issue: the unrelated pre-existing reference to the non-existent
`CheckRuleReferenceFreshness.cs` script (see parent PRD "Out of Scope").

## Acceptance criteria

- [ ] The README build-status badge points at the GitHub Actions `ci.yml` workflow, not Azure DevOps.
- [ ] The "Repository layout" section lists `.github/workflows/` with `ci.yml` and `release.yml` and no longer references `.azure-pipelines/` or `outdated.yml`.
- [ ] The README still builds/renders correctly (no broken markdown or links).

## Blocked by

- Blocked by `issues/002-add-ci-workflow.md`

## User stories addressed

- User story 22
- User story 23
