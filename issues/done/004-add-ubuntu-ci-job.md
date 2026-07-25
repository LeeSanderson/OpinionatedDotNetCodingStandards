## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Add a new `ubuntu-latest` job to `.github/workflows/ci.yml` mirroring the existing `windows-latest`
job's full sequence (`dotnet restore`/`build`/`test`/`pack`), so this repository's own CI runs on a
real case-sensitive filesystem going forward — closing the blind spot that let the original bug
ship in all seven prior releases undetected (see the PRD's Implementation Decisions and Further
Notes sections).

## Acceptance criteria

- [ ] A new `ubuntu-latest` job exists in `ci.yml`, running the same
      `restore`/`build`/`test`/`pack` sequence as the existing `windows-latest` job.
- [ ] The new job passes — confirming, via a real Linux `dotnet pack` + build, that the fix from
      `issues/002` actually resolves the original consumer-facing bug end-to-end.

## Blocked by

- Blocked by `issues/002-fix-case-sensitive-path-casing.md`

## User stories addressed

- User story 6
- User story 13
