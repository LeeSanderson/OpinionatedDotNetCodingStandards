## Parent PRD

`issues/prd.md`

## What to build

Backfill the NetAnalyzers **Design** rules (`CA10xx`, ~31 active) that are not already tagged by
`issues/005`. For each remaining rule, write a positive test in the existing harness style
(`CodeAnalysisRulesShould` is the prior art) and add its canonical `[RuleDoc]`; for any rule the
harness cannot trigger (e.g. assembly-level rules such as `CA1016`), add a reasoned `UntestableRules`
entry. Remove each covered rule from the `KnownUncovered` allowlist.

## Acceptance criteria

- [ ] Every active `CA10xx` rule has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No `CA10xx` rule remains on the allowlist.
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
