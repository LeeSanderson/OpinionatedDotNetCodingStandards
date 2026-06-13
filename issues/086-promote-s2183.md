# 086 — Promote S2183 (Integral numbers should not be shifted by zero or more than their number of bits-1)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S2183 — "Integral numbers should not be shifted by zero or more than their number of bits-1"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S2183` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S2183", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S2183`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S2183`; rule-reference lists it).
- [ ] Any `S2183` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
