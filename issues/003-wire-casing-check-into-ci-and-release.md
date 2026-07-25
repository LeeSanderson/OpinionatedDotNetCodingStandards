## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Wire `scripts/CheckImportPathCasing.cs` (added in `issues/001`, passing since `issues/002`) into
this repository's automated gates (see the PRD's Implementation Decisions section):

- Add a step to the existing `windows-latest` job in `.github/workflows/ci.yml` that runs the
  script. It's cheap, so it should run early in the job, before the slower restore/build/test/pack
  steps.
- Add the same script invocation to `scripts/New-ReleaseTag.ps1`'s existing pre-flight checks,
  alongside the existing `CheckNugetDependenciesMatchProps.cs` and
  `GenerateRuleReference.cs --check` calls, in the same fail-fast style.

## Acceptance criteria

- [ ] `.github/workflows/ci.yml`'s `windows-latest` job runs
      `dotnet ./scripts/CheckImportPathCasing.cs` as a step, and the job passes.
- [ ] `scripts/New-ReleaseTag.ps1` runs the same script as part of its pre-flight checks, failing
      the release with a clear message if the script exits non-zero — matching how the existing
      `CheckNugetDependenciesMatchProps.cs`/`GenerateRuleReference.cs --check` calls behave.

## Blocked by

- Blocked by `issues/002-fix-case-sensitive-path-casing.md`

## User stories addressed

- User story 5
