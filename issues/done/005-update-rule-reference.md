## Parent PRD

`issues/prd-xunit-v3-parallel-threads.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and MA0204 and MA0205 appear in it
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify MA0204 and MA0205 appear in `docs/rule-reference.md`, then commit.

## Blocked by

All per-rule test issues for this PRD (issues 003 through 004).

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
