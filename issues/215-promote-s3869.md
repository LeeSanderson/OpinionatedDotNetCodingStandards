# 215 — Promote S3869 (SafeHandle.DangerousGetHandle should not be called)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S3869 — "SafeHandle.DangerousGetHandle should not be called"** from `none` to an enforced severity (`warning`),
covered by a `[RuleDoc]` trigger test, fixing any resulting violations in the repository's own source.

See PRD → Testing Decisions (test shape, organisation, prior art).

## Acceptance criteria

- [ ] `S3869` is set to `warning` in the Sonar editorconfig.
- [ ] A method-level `[RuleDoc("S3869", …)]` test in the Sonar test subfolder builds a project
      that triggers the rule and asserts the build output reports `S3869`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S3869`; rule-reference lists it).
- [ ] Any `S3869` violations in the repository's own `src/` and `tests/` are resolved.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 3
- User story 6
- User story 14
