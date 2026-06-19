## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Strengthen the `Git & boundaries` section of `AGENTS.md` with explicit branch naming rules and a prohibition on committing directly to `main`. The updated section must state:

- **No commits to `main`** — agents must never commit or push directly to `main`.
- **Branch naming for PRD-linked work** — all issues that share a parent PRD commit to `feat/<prd-slug>` (strip `issues/` prefix and `.md` suffix from the PRD filename, prepend `feat/`). Example: issues whose parent PRD is `issues/prd-renovate-branch-protection.md` all land on `feat/prd-renovate-branch-protection`.
- **Branch naming for standalone issues** — issues with no parent PRD commit to `feat/NNN-issue-slug` (strip `issues/` prefix and `.md` suffix from the issue filename, prepend `feat/`).
- **Unexpected branch warning** — when an agent is invoked on a branch that does not match the expected name derived from the issue, it must warn the user and default to refusing; the agent should not mix unrelated issues onto the same branch.

See the **Implementation Decisions** section of the parent PRD for the full prose.

## Acceptance criteria

- [ ] `AGENTS.md` `Git & boundaries` section explicitly prohibits committing to `main`.
- [ ] Branch naming convention for PRD-linked issues (`feat/<prd-slug>`) is documented with an example.
- [ ] Branch naming convention for standalone issues (`feat/NNN-issue-slug>`) is documented.
- [ ] The unexpected-branch warning and default-refuse behaviour is documented.
- [ ] The rest of `AGENTS.md` is unchanged.
- [ ] `dotnet build` continues to pass.

## Blocked by

None — can start immediately.

## User stories addressed

- User story 9 (agents prevented from committing to `main`)
- User story 10 (PRD-linked issues share a feature branch)
- User story 11 (standalone issues get their own branch)
- User story 12 (agents warned and refuse on unexpected branch)
