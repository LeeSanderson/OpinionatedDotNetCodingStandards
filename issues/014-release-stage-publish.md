# Release stage: publish to NuGet.org and create a GitHub Release on a `v*` tag

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Versioning & release* and *Setup prerequisites*, and User stories 1, 4, 5, and 6.

## What to build

> **Final task.** This is intentionally the last issue: publishing must not happen
> until every other issue is resolved and the package is fully ready for public
> release. Do not start until all prior issues (`001`–`013`) are complete.

A new Azure DevOps release stage, triggered by pushing a `v*` git tag, that makes
releasing a single deliberate action tied to a commit:

- Read the version from the `v*` tag and inject it at pack time via the nuspec
  version token / `NuspecProperties` mechanism established in
  `issues/003-nuspec-version-token-and-fallback.md` (User story 1).
- Run the existing build and test steps first, and publish **only after** they pass,
  so a broken package can never reach the public feed (User stories 4, 5).
- On success, push the package to NuGet.org using the maintainer-provided API key.
- Create a matching GitHub Release populated from `CHANGELOG.md` (from
  `issues/004-changelog-and-package-metadata.md`) so consumers have a human-readable
  record of what changed (User story 6).

This is a **HITL** slice: it depends on maintainer-provided setup prerequisites (a
NuGet.org API key as an Azure secret/service connection, a GitHub service connection
for creating Releases, and the Azure Pipelines GitHub app wired up for PR checks) and
warrants a pipeline-design review before merge.

End-to-end behavior: pushing a `v1.2.3` tag builds and tests the solution, packs a
`1.2.3` package, publishes it to NuGet.org, and creates a GitHub Release `v1.2.3`
with notes drawn from the changelog.

## Acceptance criteria

- [ ] A release stage triggers on `v*` tags
- [ ] The package version is derived from the tag and injected at pack time
- [ ] Build and tests run before publishing; publishing is skipped if either fails
- [ ] On success the package is pushed to NuGet.org using the configured API key/service connection
- [ ] A GitHub Release is created for the tag with notes populated from `CHANGELOG.md`
- [ ] The existing per-build CI behaviour (build/test/pack and the outdated gate) is preserved

## Blocked by

- Blocked by `issues/003-nuspec-version-token-and-fallback.md` (version injection mechanism)
- Blocked by `issues/004-changelog-and-package-metadata.md` (changelog for the GitHub Release notes)
- Blocked by all other issues (`001`, `002`, `005`–`013`): publishing must wait until the package is fully releasable, documented, and tested

## User stories addressed

- User story 1
- User story 4
- User story 5
- User story 6

