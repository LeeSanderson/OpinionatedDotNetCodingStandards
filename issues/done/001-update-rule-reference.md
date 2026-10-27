## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that the analyzer packages have been bumped. The
reference is generated from the editorconfig files and the test assembly's `[RuleDoc]`
attributes. This bump added no new rules, so this issue confirms the reference and rule
coverage are still complete after the version change.

## Acceptance criteria

- [ ] `RuleDocCoverageShould` passes outright (no missing or duplicate `[RuleDoc]` entries)
- [ ] `docs/rule-reference.md` has been regenerated (any diff from the bump is committed; if
      the regeneration produces no diff, note that in the commit or issue result)
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify `docs/rule-reference.md` is current, then commit.

## Blocked by

None — there are no per-rule test issues for this PRD (the bump added no new rules).

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
