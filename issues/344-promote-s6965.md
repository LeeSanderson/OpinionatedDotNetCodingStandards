# 344 — Promote S6965 (REST API actions should be annotated with an HTTP verb attribute)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S6965 — "REST API actions should be annotated with an HTTP verb attribute"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S6965` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S6965", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S6965`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S6965`; rule-reference lists it).
- [ ] Any `S6965` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
