## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Add a branch-check step to both the `work-on-next-issue` and `work-on-this` skills, inserted immediately before the commit step in each skill's workflow. The logic is identical in both skills but the branch derivation differs:

**`work-on-next-issue`** — derive the expected branch from the issue being worked on:
1. Read the issue file's `Parent PRD` field. If present, derive branch as `feat/<prd-slug>` (strip `issues/` prefix and `.md` suffix from the PRD filename).
2. If no parent PRD, derive branch as `feat/NNN-issue-slug` (strip `issues/` prefix and `.md` suffix from the issue filename).

**`work-on-this`** — derive the expected branch from the task description:
1. Slugify the first five words of the task description (lowercase, spaces to hyphens, strip punctuation), prepend `feat/`.

**Three-way branch logic (same for both skills):**
- Currently on `main` → create and checkout the expected branch, then proceed.
- Already on the expected branch → proceed.
- On a different feature branch → warn the user, default to refusing; ask them to switch manually.

See the **Implementation Decisions** section of the parent PRD for full details.

## Acceptance criteria

- [ ] `.claude/skills/work-on-next-issue/SKILL.md` contains the branch-check step in the correct position (before the commit step).
- [ ] `.claude/skills/work-on-this/SKILL.md` contains the branch-check step in the correct position (before the commit step).
- [ ] Both skills derive the expected branch name as specified (PRD-linked vs. standalone for `work-on-next-issue`; first-five-words slug for `work-on-this`).
- [ ] Both skills document all three cases: `main` → create, correct branch → proceed, unexpected branch → warn and refuse by default.
- [ ] No other parts of either skill are changed beyond inserting the branch-check step.
- [ ] `dotnet build` continues to pass.

## Blocked by

- Blocked by `issues/003-agents-md-branch-discipline.md`

## User stories addressed

- User story 9 (agents prevented from committing to `main`)
- User story 10 (PRD-linked issues share a feature branch)
- User story 11 (standalone issues get their own branch)
- User story 12 (agents warned and refuse on unexpected branch)
