## Parent PRD

`issues/prd.md`

## What to build

Tag the rules that already have tests, with no new behavioral tests written. See PRD user stories 19
and 23 and "The `[RuleDoc]` attribute". For each of the ~75 already-tested rules (every distinct rule
id asserted positively in the existing suite except `CA1000`, which `issues/001` already tagged, and
excluding the editor-tier rules handled in `issues/004`), add a canonical `[RuleDoc]` to the single
positive test that demonstrates it. Where a rule has several positive tests (e.g. `RS0030`'s
banned-API families), tag exactly one. Leave negative and toggle tests untagged. Remove every
newly-tagged rule from the `KnownUncovered` allowlist.

Each `[RuleDoc]` description should match the rule's current doc text so the regenerated
`docs/rule-reference.md` stays byte-identical (the value simply migrates from the comment fallback to
the attribute).

## Acceptance criteria

- [ ] Every already-tested rule (positive) carries exactly one canonical `[RuleDoc]`.
- [ ] Negative/toggle tests remain untagged.
- [ ] The allowlist shrinks by the count of newly-tagged rules.
- [ ] Regenerating `docs/rule-reference.md` produces no diff (descriptions migrated, content unchanged).
- [ ] The coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`

## User stories addressed

- User story 2
- User story 5
- User story 19
- User story 23
