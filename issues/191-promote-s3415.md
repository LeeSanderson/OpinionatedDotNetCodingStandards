# 191 — Promote S3415 (Assertion arguments should be passed in the correct order)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S3415 — "Assertion arguments should be passed in the correct order"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S3415` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S3415", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S3415`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S3415`; rule-reference lists it).
- [ ] Any `S3415` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
