## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and MA0213, MA0214, MA0215 and MA0217 appear in it
- [ ] `git diff --exit-code -- packages/Opinionated.DotNet.CodingStandards/pkgsrc/config/analyzers/ docs/`
      is clean after re-running both generator scripts (this is exactly what CI asserts)
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

Note the four new rules are listed with their shipped severities — MA0213 as `warning`, and
MA0214/MA0215/MA0217 as `suggestion`.

## Blocked by

All per-rule test issues for this PRD (issues 001 through 004).

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
