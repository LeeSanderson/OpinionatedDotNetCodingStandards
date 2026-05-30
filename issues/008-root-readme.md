# Root README (contributor-focused)

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Documentation* and User story 13.

## What to build

A root `README.md` written for contributors that explains what the package is and how
to work on it, so a contributor can be productive without reverse-engineering the
repo. Cover:

- What the package is (a NuGet config/development-dependency package bundling Roslyn
  analyzers plus curated editorconfig/MSBuild props/targets).
- How to build and test the solution locally.
- The repository layout (`src/`, `packages/pkgsrc/...`, `tests/`, `scripts/`,
  `.azure-pipelines/`).
- The release process (tag-driven versioning; pushing a `v*` tag publishes — see the
  release stage).

No CONTRIBUTING file is added in this PRD (out of scope).

End-to-end behavior: a new contributor can read the root README and build, test, and
understand how a release is cut.

## Acceptance criteria

- [ ] A root `README.md` exists describing what the package is
- [ ] It documents how to build and test the solution
- [ ] It documents the repository layout
- [ ] It documents the tag-driven release process
- [ ] It is distinct from the packaged (consumer-facing) README

## Blocked by

None - can start immediately.

## User stories addressed

- User story 13
