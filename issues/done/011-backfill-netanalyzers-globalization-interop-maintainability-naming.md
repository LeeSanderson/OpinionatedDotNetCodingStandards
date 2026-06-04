## Parent PRD

`issues/prd.md`

## What to build

Backfill the NetAnalyzers **Globalization** (`CA13xx`, ~6), **Interop/Portability** (`CA14xx`, ~8),
**Maintainability & throw-helpers** (`CA15xx`, ~6), and **Naming** (`CA17xx`, ~8) rules — ~28 active
across these ranges — that are not already tagged by `issues/005`. For each remaining rule, write a
positive test in the existing harness style and add its canonical `[RuleDoc]`; for any rule the
harness cannot trigger (e.g. platform/portability rules such as `CA1416`), add a reasoned
`UntestableRules` entry. Remove each covered rule from the `KnownUncovered` allowlist.

## Acceptance criteria

- [ ] Every active `CA13xx`, `CA14xx`, `CA15xx`, and `CA17xx` rule has a canonical `[RuleDoc]` (positive test or reasoned exemption).
- [ ] No rule in these ranges remains on the allowlist.
- [ ] New tests follow the prior-art harness shape and pass.
- [ ] Regenerated doc reflects attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
