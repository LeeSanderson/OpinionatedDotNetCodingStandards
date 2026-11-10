## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `RuleDocCoverageShould` passes in full (every active rule covered exactly once)
- [ ] `docs/rule-reference.md` has been regenerated and the new rule IDs appear in it
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

- `issues/001-test-ca1517.md`
- `issues/002-test-ca1876.md`
- `issues/003-test-ca1877.md`
- `issues/004-test-ca2026.md`
- `issues/005-test-ca2027.md`
- `issues/006-test-ca2028.md`
- `issues/007-test-ca2267.md`

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)