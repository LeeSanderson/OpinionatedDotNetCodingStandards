# 205 — Promote S3466 (Optional parameters should be passed to base calls)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S3466 — "Optional parameters should be passed to base calls"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S3466` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S3466", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S3466`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S3466`; rule-reference lists it).
- [ ] Any `S3466` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
