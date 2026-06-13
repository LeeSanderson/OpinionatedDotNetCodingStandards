# 282 — Promote S4502 (Disabling CSRF protections is security-sensitive)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S4502 — "Disabling CSRF protections is security-sensitive"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S4502` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S4502", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S4502`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S4502`; rule-reference lists it).
- [ ] Any `S4502` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
