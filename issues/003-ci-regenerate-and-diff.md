## Parent PRD

`issues/prd.md`

## What to build

Rewire CI to make the doc-generation script the single freshness guard. See PRD "Removed / changed"
and "Mechanism". Replace the existing "Check rule reference is up to date" step with: run
`dotnet scripts/GenerateRuleReference.cs`, then `git diff --exit-code docs/`. Because generation now
reflects over the built test assembly, this step must run **after** the build (and after restore),
not before it as today. Delete `scripts/CheckRuleReferenceFreshness.cs`.

## Acceptance criteria

- [ ] `scripts/CheckRuleReferenceFreshness.cs` is deleted.
- [ ] The CI pipeline regenerates the doc after build and fails on a dirty working tree (`git diff --exit-code`).
- [ ] The freshness step no longer runs before build.
- [ ] Locally, regenerating against a clean checkout produces no diff (committed doc is current).

## Blocked by

- Blocked by `issues/001-ruledoc-attribute-and-generator-tracer-bullet.md`

## User stories addressed

- User story 11
- User story 21
