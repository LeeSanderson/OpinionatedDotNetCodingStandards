## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Enable branch protection on `main` in the GitHub repository settings. This is a manual (HITL) step performed in the GitHub UI or via `gh` CLI:

- **Require status checks to pass before merging** — add the Azure Pipelines CI check as a required check. The exact check name must be read from the Checks tab of a merged Renovate PR (or any existing merged PR) before configuring this setting.
- **Require branches to be up to date before merging** — ensure the branch is not behind `main` at merge time.
- **No required reviewer approvals** — Renovate automerge must work unattended.

This issue cannot be started until at least one Renovate PR has been opened, so the CI check name is known. Identify the check name, then configure protection.

See the **Implementation Decisions** section of the parent PRD for the full specification.

## Acceptance criteria

- [ ] Branch protection is active on `main` in GitHub.
- [ ] The Azure Pipelines CI check is listed as a required status check.
- [ ] "Require branches to be up to date before merging" is enabled.
- [ ] No required reviewer approvals are configured.
- [ ] A direct push to `main` is rejected (verify by attempting one).
- [ ] Renovate automerge still works: a green Renovate PR merges automatically without manual approval.

## Blocked by

- Blocked by `issues/001-add-renovate-json.md` (need a live Renovate PR to identify the exact CI check name)

## User stories addressed

- User story 6 (branch protection with required status checks)
- User story 7 (no required reviewer approvals so automerge works unattended)
