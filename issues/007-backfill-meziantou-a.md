## Parent PRD

`issues/prd.md`

## What to build

Backfill the first half of the Meziantou.Analyzer rules. No Meziantou rule is tested today, so all 56
are new (see PRD "Further Notes"). This slice covers the lower-numbered half — approximately
`MA0015` through `MA0082` (~28 rules; balance the split with `issues/008`). For each rule, write a
positive test in the existing harness style (build a sample that violates the rule, assert the
diagnostic) and add its canonical `[RuleDoc]`. For any rule the harness cannot trigger, add a
reasoned `UntestableRules` entry instead. Remove each covered rule from the `KnownUncovered`
allowlist.

## Acceptance criteria

- [ ] Every Meziantou rule in the `MA0015`–`MA0082` range has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] The covered rules are removed from the allowlist.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
