# 128 — Promote S2701 (Literal boolean values should not be used in assertions)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S2701 — "Literal boolean values should not be used in assertions"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S2701` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S2701", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S2701`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S2701`; rule-reference lists it).
- [ ] Any `S2701` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
