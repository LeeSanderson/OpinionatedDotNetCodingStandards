## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and the new rule IDs (MA0209, MA0210)
      appear in it
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

All per-rule test issues for this PRD (issues 007 through 008).

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
