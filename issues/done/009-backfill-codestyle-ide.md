## Parent PRD

`issues/prd.md`

## What to build

Backfill the remaining build-enforced CSharp.CodeStyle (`IDE####`) rules — the ~44 active rules in
`Analyzer.Microsoft.CodeAnalysis.CSharp.CodeStyle.editorconfig` that are not already tagged by
`issues/005`. (The editor-tier `IDE####` rules from `Opinionated.editorconfig` are handled separately
in `issues/004` and are out of scope here.) For each remaining rule, write a positive test in the
existing harness style and add its canonical `[RuleDoc]`; for any rule the harness cannot trigger,
add a reasoned `UntestableRules` entry. Remove each covered rule from the `KnownUncovered` allowlist.

## Acceptance criteria

- [ ] Every active CSharp.CodeStyle rule has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No CSharp.CodeStyle rule remains on the allowlist.
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
