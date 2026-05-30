# Remove the unused coverage dependency

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Casing & cleanup*, *Testing Decisions → Notes on scope of testing*, and User story 11.

## What to build

The test project references `coverlet.collector`, but this package emits no
production assembly (it is a config/development-dependency package), so code-coverage
measurement is not meaningful and the dependency is dead. Remove the
`coverlet.collector` `PackageReference` from
`tests/Opinionated.DotNet.CodingStandards.Tests/Opinionated.DotNet.CodingStandards.Tests.csproj`
and its corresponding entry in `Directory.Packages.props` (if present), so the
project's dependency list no longer misrepresents what is measured.

End-to-end behavior: the test project restores, builds, and runs its tests with no
coverage collector referenced.

## Acceptance criteria

- [ ] `coverlet.collector` is no longer referenced by the test project
- [ ] Any corresponding version entry in `Directory.Packages.props` is removed, and the dependency-check script still passes
- [ ] `dotnet test` of the solution succeeds with the same set of tests passing as before

## Blocked by

None - can start immediately.

## User stories addressed

- User story 11
