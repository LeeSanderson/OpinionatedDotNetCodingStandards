# Changelog and package metadata (release notes link + search tags)

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Documentation* and User stories 18 and 19.

## What to build

Introduce a `CHANGELOG.md` in the [Keep a Changelog](https://keepachangelog.com)
format at the repository root, and enrich the package nuspec metadata so consumers
on NuGet.org can find the package and see what changed:

- Add a `releaseNotes` field to the nuspec that links to the changelog (User story 19).
- Add NuGet search `tags` to the nuspec covering discovery terms such as analyzers,
  code style, and editorconfig (User story 18).

The changelog file is also the source the release stage will later use to populate
the GitHub Release (see `issues/014-release-stage-publish.md`).

End-to-end behavior: packing the package produces metadata whose release-notes field
links to the changelog and whose tags include the discovery terms.

## Acceptance criteria

- [ ] `CHANGELOG.md` exists at the repository root in Keep a Changelog format with an initial entry
- [ ] The nuspec `releaseNotes` metadata links to the changelog
- [ ] The nuspec `tags` metadata includes analyzer/code-style/editorconfig discovery terms
- [ ] The packed `.nupkg` metadata contains the release-notes link and the tags
- [ ] `dotnet build` and `dotnet test` of the solution succeed

## Blocked by

- Blocked by `issues/001-standardize-dotnet-casing.md` (modifies the renamed nuspec)

## User stories addressed

- User story 18
- User story 19
