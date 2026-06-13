# 310 — Promote S6566 (Use DateTimeOffset instead of DateTime)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S6566 — "Use DateTimeOffset instead of DateTime"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S6566` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S6566", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S6566`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S6566`; rule-reference lists it).
- [ ] Any `S6566` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
