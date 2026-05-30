# Nuspec version token with a `0.0.0-dev` fallback

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Versioning & release* and User stories 2, 3, and 24.

## What to build

Replace the hardcoded `<version>0.1.0</version>` literal in the package nuspec with a
replacement token (e.g. `$version$`) so the version can be injected at pack time.
Set a default pre-release version of `0.0.0-dev` in the shared build props so that
untagged and local builds — which pack on every build because
`GeneratePackageOnBuild` is enabled — produce a valid, obviously non-releasable
package version rather than failing for lack of a version.

Because the package uses a custom nuspec, the injected version must flow through
nuspec properties (`NuspecProperties`) rather than the plain MSBuild version
property; confirm the exact wiring during implementation. This change also makes the
test harness's existing override meaningful: `PackageFixture` already passes
`-p:NuspecProperties=version=999.9.9`, which currently has no effect because the
nuspec version is a literal.

End-to-end behavior: a local `dotnet pack` (or build) produces a `0.0.0-dev`
package, and the test harness's packed package is versioned `999.9.9`.

## Acceptance criteria

- [ ] The nuspec `<version>` is a replacement token, not a hardcoded literal
- [ ] A default `0.0.0-dev` version is defined in the shared build props and flows into the packed package when no override is supplied
- [ ] A local `dotnet pack` produces a package whose version is `0.0.0-dev`
- [ ] A test asserts that the package packed by `PackageFixture` carries the overridden version (`999.9.9`), proving the harness override now takes effect
- [ ] `dotnet build` and `dotnet test` of the solution succeed

## Blocked by

- Blocked by `issues/001-standardize-dotnet-casing.md` (modifies the renamed nuspec and shared build props)

## User stories addressed

- User story 2
- User story 3
- User story 24
