# 004 — Promote S1541 (cyclomatic complexity)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S1541 — methods/properties should not be too complex (cyclomatic
complexity)** from `none` to an enforced severity, with the opinionated threshold chosen in
`002`, covered by a `[RuleDoc]` trigger test, and fix any resulting violations in the
repository's own source.

No existing bundled analyzer measures cyclomatic complexity (PRD → Problem Statement;
story 2). End-to-end slice: editorconfig severity flip + threshold → a test asserting `S1541`
on an over-complex method → green dogfooding build.

See PRD → Testing Decisions and PRD → Further Notes (*Dogfooding can break the repository's
own build*). The exact rule id/threshold are confirmed against the extracted descriptor set in
`002`.

## Acceptance criteria

- [ ] `S1541` is set to an enforced severity (`warning`) in the Sonar editorconfig, with the
      cyclomatic-complexity threshold from `002` configured via its analyzer option.
- [ ] A method-level `[RuleDoc("S1541", …)]` test builds a project whose method exceeds the
      threshold and asserts the build output reports `S1541`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S1541`; rule-reference lists it).
- [ ] Any `S1541` violations in the repository's own `src/` and `tests/` are resolved so the
      dogfooding build stays green — or the threshold/decision is revisited and noted here.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 2
- User story 6
- User story 14
- User story 18
- User story 24
- User story 25
