## Parent PRD

`issues/prd.md`

## What to build

Backfill the NetAnalyzers **Performance** rules (`CA18xx`, ~59 active) that are not already tagged by
`issues/005`. This is the largest single category; keep the slice focused on `CA18xx` only. For each
remaining rule, write a positive test in the existing harness style and add its canonical
`[RuleDoc]`; for any rule the harness cannot trigger, add a reasoned `UntestableRules` entry. Remove
each covered rule from the `KnownUncovered` allowlist.

If this proves too large for a single pass, split it into two issues (`CA1800`–`CA1849` and
`CA1850`–`CA1899`) at implementation time.

## Acceptance criteria

- [ ] Every active `CA18xx` rule has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No `CA18xx` rule remains on the allowlist.
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
