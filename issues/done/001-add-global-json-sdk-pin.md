## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Add a `global.json` at the repository root that pins the .NET SDK so CI and local builds
resolve the same toolchain. See the "SDK pinning via `global.json`" implementation decision
in the parent PRD.

Pin the currently-installed SDK `10.0.101` with `rollForward: latestPatch` (accept patch
updates within the pinned feature band, no feature-band drift). This lands first because both
new workflows set up the SDK via `actions/setup-dotnet`, which honours `global.json`.

## Acceptance criteria

- [ ] A `global.json` exists at the repo root pinning SDK `10.0.101` with `rollForward: latestPatch`.
- [ ] `dotnet --version` in the repo resolves to a `10.0.x` SDK (no error about a missing SDK).
- [ ] `dotnet build Opinionated.DotNet.CodingStandards.slnx` and `dotnet test` still succeed locally with the pin in place.
- [ ] No existing behaviour changes (the pin matches the SDK already in use).

## Blocked by

None - can start immediately.

## User stories addressed

- User story 17
