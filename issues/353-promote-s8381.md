# 353 — Promote S8381 (scoped should be escaped when used as an identifier or type name in parenthesized lambda parameter lists)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S8381 — "scoped should be escaped when used as an identifier or type name in parenthesized lambda parameter lists"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S8381` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S8381", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S8381`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S8381`; rule-reference lists it).
- [ ] Any `S8381` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
