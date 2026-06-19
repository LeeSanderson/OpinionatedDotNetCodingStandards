## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Add a branch-creation step to the `update-nuget-packages` skill. After step 2 (confirming that updates exist) and before step 3 (modifying any files), the skill must:

1. Derive the branch name as `feat/bump-analyzers-YYYY-MM-DD` (using today's date).
2. If the branch does not exist → create it and switch to it.
3. If the branch already exists (e.g. a prior run on the same day) → switch to it and continue; do not create a duplicate.
4. Proceed with the rest of the workflow (updating files, running scripts, writing PRD and issues) on this branch.

If no updates are found at step 2, skip the branch-creation step entirely — no branch should be created for a no-op run.

See the **Implementation Decisions** section of the parent PRD for full details.

## Acceptance criteria

- [ ] `.claude/skills/update-nuget-packages/SKILL.md` contains the branch-creation step inserted between the "confirm updates exist" step and the "modify files" step.
- [ ] The branch name pattern `feat/bump-analyzers-YYYY-MM-DD` is specified.
- [ ] The skill handles the "branch already exists" case without creating a duplicate.
- [ ] The skill skips branch creation when no updates are found.
- [ ] No other parts of the skill are changed beyond inserting the branch-creation step.
- [ ] `dotnet build` continues to pass.

## Blocked by

- Blocked by `issues/003-agents-md-branch-discipline.md`

## User stories addressed

- User story 13 (version bumps never land as uncommitted changes on `main`)
