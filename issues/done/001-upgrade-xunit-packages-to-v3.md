## Parent PRD

`issues/prd-xunit-v3-parallel-tests.md`

## What to build

Replace the `xunit` 2.9.3 package reference with `xunit.v3` in `Directory.Packages.props`
and the test project `.csproj`. Fix any compilation errors caused by xUnit v3's namespace
reorganisation (the `Xunit.Abstractions` assembly is removed in v3; types move to the `Xunit`
namespace). The project must compile cleanly after this slice; no behavioural change is
expected — tests still run serially via the existing `PackageCollection` at this point.

Refer to the "Upgrade xUnit" and "Fix xUnit v3 namespace changes" bullets in the PRD's
Implementation Decisions section.

## Acceptance criteria

- [ ] `Directory.Packages.props` no longer references `xunit` 2.x; a new `xunit.v3` entry
      is present with the latest stable version.
- [ ] The test `.csproj` references `xunit.v3` (not the old `xunit` meta-package).
- [ ] All `using Xunit.Abstractions` directives in the test project (base class, helpers,
      test classes) are updated to `using Xunit`.
- [ ] `dotnet build Opinionated.DotNet.CodingStandards.slnx` succeeds with zero errors and
      zero new warnings.
- [ ] A single test class passes when run via `--filter` (confirming the runner is still
      functional after the package swap).

## Blocked by

None — can start immediately.

## User stories addressed

- User story 10 (preserve all existing test behaviour across the migration)
