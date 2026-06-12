# 005 — Promote S134 (control-flow nesting depth)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S134 — control-flow statements should not be nested too deeply** from `none`
to an enforced severity, with the opinionated threshold chosen in `002`, covered by a
`[RuleDoc]` trigger test, and fix any resulting violations in the repository's own source.

No existing bundled analyzer enforces nesting depth (PRD → Problem Statement; story 2).
End-to-end slice: editorconfig severity flip + threshold → a test asserting `S134` on
over-nested control flow → green dogfooding build.

See PRD → Testing Decisions and PRD → Further Notes (*Dogfooding can break the repository's
own build*).

## Acceptance criteria

- [ ] `S134` is set to an enforced severity (`warning`) in the Sonar editorconfig, with the
      maximum-nesting-depth threshold from `002` configured via its analyzer option.
- [ ] A method-level `[RuleDoc("S134", …)]` test builds a project whose control flow nests
      beyond the threshold and asserts the build output reports `S134`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S134`; rule-reference lists it).
- [ ] Any `S134` violations in the repository's own `src/` and `tests/` are resolved so the
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
