# Standardize on `DotNet` casing and fix the dependency-check script reference

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Casing & cleanup* and User stories 9 and 10.

## What to build

Make the repository internally case-consistent by standardizing on the `DotNet`
casing everywhere (matching the already-correct published package id
`Opinionated.DotNet.CodingStandards` and the test namespaces). Today the `src/` and
`packages/` folders, the `.slnx` references, the shared build props/targets imports,
and the nuspec path embedded in the dependency-check script use the `Dotnet`
(lower-case `o`) variant, while the test project and package id use `DotNet`. This
only works because CI is Windows-only; on a case-sensitive filesystem the
`PackageFixture` project path and the script's nuspec path would not resolve.

In the same slice, reconcile the CI step that invokes
`scripts/CheckNuGetDependenciesMatchProps.cs` with the real filename
(`scripts/CheckNugetDependenciesMatchProps.cs`) so the dependency-check step runs
regardless of filesystem case-sensitivity.

End-to-end behavior: after the change, a fresh clone builds, tests pass, the
dependency-check step runs, and no `Opinionated.Dotnet` (lower-case `o`) path,
import, or reference remains in the repository.

## Acceptance criteria

- [x] `src/` and `packages/` directories and their `.csproj`/`.nuspec` files use the `Opinionated.DotNet.CodingStandards` casing
- [x] `Opinionated.Dotnet.CodingStandards.slnx`, `Directory.Build.props`, and `Directory.Build.targets` import paths are updated to the renamed paths
- [x] The nuspec path inside `scripts/CheckNugetDependenciesMatchProps.cs` resolves to the renamed `packages/` path
- [x] The CI step in `.azure-pipelines/ci.yml` references the dependency-check script by its real filename
- [x] A repository-wide search for the `Opinionated.Dotnet` (lower-case `o`) casing returns no matches outside of git history
- [~] `dotnet build` and `dotnet test` of the solution succeed, and the dependency-check script step passes — build passes, dependency check passes, but 20 tests have pre-existing failures (introduced by package version update b7644bf before this issue; verified by running against unmodified code)

## Blocked by

None - can start immediately. (This is the riskiest churn; later work builds on the renamed paths, so it is sequenced first.)

## User stories addressed

- User story 9
- User story 10
