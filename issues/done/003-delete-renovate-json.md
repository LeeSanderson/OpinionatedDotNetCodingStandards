## Parent PRD

`issues/prd-replace-renovate-with-scheduled-check.md`

## What to build

Delete `renovate.json` from the repository root. No replacement configuration file is added —
see the PRD's Problem Statement and Implementation Decisions ("`renovate.json` deleted
outright"). Renovate has been non-functional for roughly five scheduled runs (no PRs, no
Dependency Dashboard issue) and its one useful function is being replaced by the scheduled
workflow in `issues/001-add-dependency-check-workflow.md`, so this deletion carries no
functional dependency on that workflow landing first.

## Acceptance criteria

- [ ] `renovate.json` no longer exists in the repository
- [ ] No remaining file references Renovate's configuration functionally (the historical branch-
      name examples in `AGENTS.md` and `.claude/skills/work-on-next-issue/SKILL.md`, e.g.
      `feat/prd-renovate-branch-protection`, are unrelated naming-convention examples and should
      be left untouched)

## Blocked by

None - can start immediately

## User stories addressed

- User story 13
- User story 15
