## Parent PRD

`issues/prd-replace-renovate-with-scheduled-check.md`

## What to build

Extend `.github/workflows/dependency-check.yml` (added in
`issues/001-add-dependency-check-workflow.md`) with the recovery path: when a run finds **no**
outdated packages, and the persistent tracking issue is currently open, close it with a short
explanatory comment. See the PRD's Implementation Decisions section ("Issue management") and
user story 9.

This reuses the same issue-lookup-by-fixed-title logic already built in issue 001; this issue
only adds the branch where the outdated-package list from `dotnet outdated` is empty.

## Acceptance criteria

- [ ] When `dotnet outdated` reports zero outdated packages and the tracking issue is open, the
      workflow closes it with a short comment explaining why
- [ ] When nothing is outdated and the tracking issue is already closed (or has never been
      created), the workflow is a no-op — it does not error and does not reopen or recreate
      anything
- [ ] Manually verified end-to-end: after bumping the package(s) that caused a prior run to open
      the tracking issue, re-triggering the workflow via `workflow_dispatch` closes that issue
      (per the PRD's Testing Decisions, "Scheduled workflow, resolved")

## Blocked by

- Blocked by `issues/001-add-dependency-check-workflow.md`

## User stories addressed

- User story 9
- User story 23
