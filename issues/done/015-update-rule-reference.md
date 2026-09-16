## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and all fourteen new rule IDs (`MA0226`
      through `MA0239`) appear in it
- [ ] Each of them is listed at `warning` severity, matching the editorconfig
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

All per-rule test issues for this PRD:

- `issues/001-test-ma0226.md`
- `issues/002-test-ma0227.md`
- `issues/003-test-ma0228.md`
- `issues/004-test-ma0229.md`
- `issues/005-test-ma0230.md`
- `issues/006-test-ma0231.md`
- `issues/007-test-ma0232.md`
- `issues/008-test-ma0233.md`
- `issues/009-test-ma0234.md`
- `issues/010-test-ma0235.md`
- `issues/011-test-ma0236.md`
- `issues/012-test-ma0237.md`
- `issues/013-test-ma0238.md`
- `issues/014-test-ma0239.md`

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
