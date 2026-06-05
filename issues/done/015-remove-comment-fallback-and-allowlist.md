## Parent PRD

`issues/prd.md`

## What to build

Final cleanup once coverage is 100%. See PRD "Migration" / "Transition completeness decision" and
user stories 17, 18, 25. With the `KnownUncovered` allowlist now empty, remove it and the coverage
test's allowlist handling, and remove the editorconfig-comment **fallback** from
`RuleReferenceGenerator` so descriptions/help come exclusively from `[RuleDoc]` attributes. After
this slice every rule's documentation is attribute-sourced and the unreliable comment parsing is
fully retired.

## Acceptance criteria

- [ ] The `KnownUncovered` allowlist is deleted and the coverage test no longer references it.
- [ ] The editorconfig-comment fallback is removed from the generator; descriptions/help are sourced only from `[RuleDoc]`.
- [ ] Regenerating `docs/rule-reference.md` produces no diff (all 393 rules already attribute-sourced).
- [ ] The coverage test fails if any active rule lacks a `[RuleDoc]` (no allowlist escape hatch remains).
- [ ] `dotnet test` and the CI regenerate-and-diff step are green.

## Blocked by

- Blocked by `issues/005-tag-existing-canonical-tests.md`
- Blocked by `issues/006-backfill-bannedapi-and-stylecop.md`
- Blocked by `issues/007-backfill-meziantou-a.md`
- Blocked by `issues/008-backfill-meziantou-b.md`
- Blocked by `issues/009-backfill-codestyle-ide.md`
- Blocked by `issues/010-backfill-netanalyzers-design.md`
- Blocked by `issues/011-backfill-netanalyzers-globalization-interop-maintainability-naming.md`
- Blocked by `issues/012-backfill-netanalyzers-performance.md`
- Blocked by `issues/013-backfill-netanalyzers-reliability-usage.md`
- Blocked by `issues/014-backfill-netanalyzers-security.md`

## User stories addressed

- User story 17
- User story 18
- User story 25
