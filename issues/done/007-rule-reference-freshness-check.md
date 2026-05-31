# CI freshness check for the rule reference

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Documentation* (the generate-and-compare CI step) and User story 12.

## What to build

A new CI step — modeled on the existing dependency-sync check in
`.azure-pipelines/ci.yml` — that regenerates the rule reference from the analyzer
editorconfigs and fails the build if the committed reference is out of date. This
guarantees the documentation cannot silently drift from the editorconfigs.

Reuse the generator logic from `issues/006-rule-reference-generator.md`: generate the
reference into a temporary location (or to stdout) and compare it against the
committed reference, failing with a non-zero exit code on any difference.

End-to-end behavior: editing an analyzer editorconfig severity without regenerating
the committed reference causes CI to fail with a clear message; regenerating and
committing makes CI pass again.

## Acceptance criteria

- [ ] A CI step runs the generate-and-compare check against the committed reference
- [ ] The step exits non-zero (failing the build) when the committed reference differs from freshly generated output
- [ ] The step passes when the committed reference is up to date
- [ ] The step is wired into `.azure-pipelines/ci.yml` alongside the existing dependency-sync check

## Blocked by

- Blocked by `issues/006-rule-reference-generator.md` (reuses the generator logic)

## User stories addressed

- User story 12
