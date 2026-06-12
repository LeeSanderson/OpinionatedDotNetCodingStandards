# 003 — Promote S3776 (cognitive complexity)

## Parent PRD

`issues/prd.md`

## What to build

Promote Sonar **S3776 — "Cognitive Complexity of methods should not be too high"** from `none`
to an enforced severity, with the opinionated threshold chosen in `002`, covered by a
`[RuleDoc]` trigger test, and fix any resulting violations in the repository's own source.

S3776 is the **flagship gap**: no existing bundled analyzer measures cognitive complexity (PRD
→ Problem Statement). End-to-end slice: editorconfig severity flip + threshold → a test that
builds a project whose method exceeds the threshold and asserts the build reports `S3776` →
green dogfooding build.

See PRD → Testing Decisions (test shape, organisation, prior art) and PRD → Further Notes
(*Dogfooding can break the repository's own build*).

## Acceptance criteria

- [ ] `S3776` is set to an enforced severity (`warning`) in the Sonar editorconfig, with the
      cognitive-complexity threshold from `002` configured via its analyzer option.
- [ ] A method-level `[RuleDoc("S3776", …)]` test in the new Sonar test subfolder builds a
      project whose method exceeds the threshold and asserts the build output reports `S3776`.
- [ ] The coverage gate passes (exactly one `[RuleDoc]` for `S3776`; rule-reference lists it).
- [ ] Any `S3776` violations in the repository's own `src/` and `tests/` are resolved so the
      dogfooding build stays green — or, if a violation cannot reasonably be resolved, the
      threshold/decision is revisited and the outcome noted on this issue.
- [ ] No regression: diagnostics from the other four analyzers are unchanged.

## Blocked by

- Blocked by `issues/002-sonar-gap-analysis-and-disposition.md`

## User stories addressed

- User story 1
- User story 6
- User story 14
- User story 18
- User story 24
- User story 25
