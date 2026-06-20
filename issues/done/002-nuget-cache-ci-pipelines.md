## Parent PRD

`issues/prd-improve-ci-pipeline-speed.md`

## What to build

Add a `Cache@2` task to all three Azure Pipelines files (`ci.yml`, `analyzer-bump.yml`,
`release.yml`), inserted immediately before the `DotNetCoreCLI restore` task in each. The
task persists the NuGet global packages folder (`$(UserProfile)\.nuget\packages`) between
runs, keyed on `Directory.Packages.props` (single source of truth for all package versions)
and `**/*.csproj`. Include a `restoreKeys` partial-match fallback so that a version-bump PR
that changes one package still gets a near-hit from the prior cache rather than a full cold
restore.

Only the Build stage of `analyzer-bump.yml` needs the cache task — the Regen stage does not
run `dotnet restore` or tests.

## Acceptance criteria

- [ ] `Cache@2` task added before `dotnet restore` in `ci.yml`
- [ ] `Cache@2` task added before `dotnet restore` in the Build stage of `analyzer-bump.yml`
- [ ] `Cache@2` task added before `dotnet restore` in `release.yml`
- [ ] Cache key includes both `Directory.Packages.props` and `**/*.csproj` fingerprints
- [ ] `restoreKeys` partial-match fallback is present in all three cache tasks
- [ ] Restore path is `$(UserProfile)\.nuget\packages`
- [ ] Pipeline builds and tests pass after the change
- [ ] Azure DevOps shows "Cache hit" on the second CI run after the PR merges

## Blocked by

None — can start immediately.

## User stories addressed

- User story 1 (CI test stage completes in under 15 minutes)
- User story 2 (release pipeline test stage completes faster)
- User story 4 (NuGet global packages cache persisted between CI runs)
- User story 5 (cache key tied to `Directory.Packages.props`, auto-invalidated on version change)
- User story 7 (`analyzer-bump.yml` Build stage benefits from the cache)
- User story 8 (release pipeline benefits from the cache)
