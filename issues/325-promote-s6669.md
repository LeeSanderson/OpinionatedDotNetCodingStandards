# 325 — Promote S6669 (Logger field or property name should comply with a naming convention)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S6669 — "Logger field or property name should comply with a naming convention"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S6669` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S6669", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S6669`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S6669`; rule-reference lists it).
- [ ] Any `S6669` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
