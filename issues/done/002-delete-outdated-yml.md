## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Delete `.azure-pipelines/outdated.yml`. This scheduled Azure Pipelines run (Mondays 08:00 UTC, runs `dotnet outdated`) is superseded by Renovate and should be removed to leave a single mechanism for tracking outdated packages.

No other files need to change — `ci.yml`, `analyzer-bump.yml`, and `release.yml` are unaffected.

## Acceptance criteria

- [ ] `.azure-pipelines/outdated.yml` no longer exists in the repository.
- [ ] The remaining pipeline files (`ci.yml`, `analyzer-bump.yml`, `release.yml`) are untouched.
- [ ] `dotnet build` continues to pass.

## Blocked by

None — can start immediately.

## User stories addressed

- User story 8 (single mechanism for tracking outdated packages)
