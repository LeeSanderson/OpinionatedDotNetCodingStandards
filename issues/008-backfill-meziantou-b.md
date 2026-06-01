## Parent PRD

`issues/prd.md`

## What to build

Backfill the second half of the Meziantou.Analyzer rules — approximately `MA0085` through `MA0178`
(~28 rules; the remainder after `issues/007`). For each, write a positive test in the existing
harness style and add its canonical `[RuleDoc]`; for any rule the harness cannot trigger, add a
reasoned `UntestableRules` entry. Remove each covered rule from the `KnownUncovered` allowlist. After
this slice, the entire Meziantou group is fully covered.

## Acceptance criteria

- [ ] Every Meziantou rule in the `MA0085`–`MA0178` range has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No Meziantou rule remains on the allowlist.
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
