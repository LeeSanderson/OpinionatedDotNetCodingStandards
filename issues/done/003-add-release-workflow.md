## Parent PRD

`issues/prd-migrate-to-github-actions.md`

## What to build

Add `.github/workflows/release.yml` — the tag-triggered publish workflow that replaces the
Azure `release.yml`. See the "`release.yml` trigger and steps" and "Publish authentication:
stored API key now" implementation decisions in the parent PRD. Independent of `ci.yml`; can
be authored in parallel with issue 002.

The workflow triggers on `push` of tags matching `v*`, runs on `windows-latest` with
`permissions: contents: read`. It derives the package version from the tag name
(`GITHUB_REF_NAME` with the leading `v` stripped). Steps: `actions/setup-dotnet` (honours
`global.json`) → `actions/cache@v4` → restore → build (`Release`) → test → `dotnet pack` of
the package project with `-p:NuspecProperties=version=<version>` → upload the `.nupkg` via
`actions/upload-artifact` → `dotnet nuget push ... --api-key ${{ secrets.NUGET_API_KEY }}
--skip-duplicate`. Leave a `# TODO(OIDC)` marker at the publish step for the future move to
trusted publishing.

Authoring this file is AFK. The live publish is validated on the next real release; during
the parallel phase both the Azure and Actions release workflows fire on the same tag and
`--skip-duplicate` makes the second push a safe no-op (see parent PRD "Further Notes"). The
`NUGET_API_KEY` secret itself is created in issue
`issues/005-github-settings-secret-and-required-check.md`.

## Acceptance criteria

- [ ] `.github/workflows/release.yml` exists, triggered by `push` of `v*` tags, on `windows-latest`, with `permissions: contents: read`.
- [ ] The package version is taken from the tag name with the leading `v` stripped.
- [ ] Steps run in order: restore (cached), build (`Release`), test, pack with `-p:NuspecProperties=version=<version>`, upload `.nupkg` artifact, `dotnet nuget push --api-key ${{ secrets.NUGET_API_KEY }} --skip-duplicate`.
- [ ] A `# TODO(OIDC)` marker is present at the publish step.
- [ ] The `v` prefix contract matches what `scripts/New-ReleaseTag.ps1` produces (`vMAJOR.MINOR.PATCH`).
- [ ] No secret value is committed to the repo; the key is read only from `secrets.NUGET_API_KEY`.

## Blocked by

- Blocked by `issues/001-add-global-json-sdk-pin.md`

## User stories addressed

- User story 10
- User story 11
- User story 12
- User story 13
- User story 14
- User story 26
