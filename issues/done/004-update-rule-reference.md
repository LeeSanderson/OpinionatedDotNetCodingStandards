## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and `MA0218`, `MA0219` and `MA0220` appear in it
- [ ] `MA0219` is listed at its deliberate `suggestion` severity, not `warning`
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

All per-rule test issues for this PRD:

- `issues/001-test-ma0218.md`
- `issues/002-test-ma0219.md`
- `issues/003-test-ma0220.md`

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
