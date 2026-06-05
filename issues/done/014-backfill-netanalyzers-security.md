## Parent PRD

`issues/prd.md`

## What to build

Backfill the NetAnalyzers **Security** rules — deserialization/data-flow (`CA23xx`), XML/injection
(`CA30xx`), and cryptography (`CA5xxx`), ~73 active in total — that are not already tagged by
`issues/005`. Many of these are data-flow / taint rules that the simple build-based harness cannot
trigger without security analysis configuration; expect a high proportion of reasoned
`UntestableRules` entries. For each rule, either write a positive test in the existing harness style
or add an `UntestableRules` entry with a written reason. Remove each covered rule from the
`KnownUncovered` allowlist.

This is the category most likely to need sub-splitting (e.g. `CA23xx`+`CA30xx` vs `CA5xxx`); split at
implementation time if needed.

## Acceptance criteria

- [ ] Every active `CA23xx`, `CA30xx`, and `CA5xxx` rule has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No rule in these ranges remains on the allowlist.
- [ ] Untestable rules each carry a specific, written reason (not a generic placeholder).
- [ ] New tests follow the prior-art harness shape and pass; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
