## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump (MA0212, S8949,
S8969, S8970) have been written (or declared untestable). The reference is generated from the
editorconfig files and the test assembly's `[RuleDoc]` attributes, so it must be regenerated
**after** all per-rule issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and MA0212, S8949, S8969, S8970 appear in it
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

- Blocked by `issues/006-test-ma0212.md`
- Blocked by `issues/007-test-s8949.md`
- Blocked by `issues/008-test-s8969.md`
- Blocked by `issues/009-test-s8970.md`

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
