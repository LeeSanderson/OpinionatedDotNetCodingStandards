# Scheduled outdated-packages pipeline

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Outdated-packages checking* and User stories 7 and 8.

## What to build

Add a separate Azure DevOps pipeline that runs the `dotnet outdated` check on a
recurring schedule and reports staleness, decoupled from the build/release path. This
lets the maintainer learn about outdated dependencies during quiet periods without
waiting for the next commit.

The existing per-build outdated gate in `.azure-pipelines/ci.yml` must remain
unchanged (User story 7) — this slice only adds the new scheduled pipeline, it does
not relocate or weaken the existing gate.

End-to-end behavior: the new scheduled pipeline can be triggered (manually or on its
schedule) and runs the outdated check, surfacing any out-of-date packages.

## Acceptance criteria

- [ ] A new pipeline definition (e.g. `.azure-pipelines/outdated.yml`) exists with a recurring `schedules` trigger
- [ ] The pipeline restores and runs the `dotnet outdated` check against the solution
- [ ] The existing per-build outdated gate in `ci.yml` is unchanged
- [ ] The pipeline YAML is valid and can be added as a pipeline in Azure DevOps

## Blocked by

None - can start immediately.

## User stories addressed

- User story 7
- User story 8
