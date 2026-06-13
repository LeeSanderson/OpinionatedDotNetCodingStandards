# 135 — Promote S2857 (SQL keywords should be delimited by whitespace)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S2857 — "SQL keywords should be delimited by whitespace"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S2857` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S2857", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S2857`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S2857`; rule-reference lists it).
- [ ] Any `S2857` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
