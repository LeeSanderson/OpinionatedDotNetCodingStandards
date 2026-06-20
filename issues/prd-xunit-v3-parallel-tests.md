# PRD: Parallelise Integration Tests via xUnit v3 Migration

## Problem Statement

The full integration test suite takes approximately 40 minutes to run. This makes the inner
development loop slow: confirming a new test passes, or verifying that a package change has
not broken existing rules, requires an unreasonably long wait.

The root cause is that all 795 test methods live in a single xUnit collection
(`PackageCollection`). xUnit guarantees that tests within a named collection run serially.
The collection was introduced to ensure `dotnet pack` runs only once — an `ICollectionFixture`
is a convenient way to share state — but it has the side effect of serialising the entire
test suite, eliminating all parallelism.

## Solution

Upgrade the test project from xUnit 2.9.3 to xUnit v3 and use xUnit v3's
`IAssemblyFixture<T>` to run `dotnet pack` exactly once per test assembly. With the
pack guaranteed at the assembly level, the `PackageCollection` and all `[Collection]`
attributes can be removed. xUnit v3 then runs test classes in parallel by default,
reducing wall-clock time from ~40 minutes to under 10 minutes.

## User Stories

1. As a developer, I want the full integration test suite to complete in under 10 minutes,
   so that I can get fast feedback without long waits.
2. As a developer, I want to run `dotnet test` without any extra flags and get parallel
   execution automatically, so that I don't need to remember special invocations.
3. As a developer, I want `dotnet pack` to still run exactly once per test run,
   so that I don't pay the cost of multiple redundant pack operations.
4. As a developer, I want the packed NuGet package to be cleaned up reliably after the
   test run completes, so that temporary directories do not accumulate on my machine.
5. As a developer, I want cleanup to happen even when tests fail, so that the machine
   stays tidy under all exit conditions.
6. As a developer, I want to continue using `--filter` to run a single test class in
   isolation, so that I can verify a new test quickly before committing.
7. As a developer, I want test output (stdout/stderr from build processes) to still be
   routed to the xUnit test output helper, so that failing tests remain diagnosable.
8. As a developer, I want each test class to still receive its own isolated temporary
   project directory, so that tests cannot interfere with one another.
9. As a CI system, I want the test suite to complete faster on GitHub Actions, so that
   pull request feedback is available sooner.
10. As a developer, I want the migration to preserve all existing test behaviour and
    assertions, so that no rule coverage is lost.
11. As a developer, I want the parallelism degree to be tunable via `xunit.runner.json`
    if contention becomes a problem, so that I have an escape hatch without code changes.
12. As a developer, I want the lightweight unit tests (rule-doc coverage, reference
    generator, etc.) that currently run outside the collection to continue working
    unchanged after the migration.

## Implementation Decisions

- **Upgrade xUnit.** Replace the `xunit` 2.9.3 package reference with `xunit.v3`
  (the xUnit v3 meta-package). Update `xunit.runner.visualstudio` to a version that
  supports xUnit v3 (3.x already in use; verify the minimum required version).

- **Register `PackageFixture` as an assembly fixture.** Add an
  `[assembly: AssemblyFixture(typeof(PackageFixture))]` attribute to the test project.
  xUnit v3 will instantiate `PackageFixture` once for the entire assembly, call
  `InitializeAsync` before any test runs, and call `DisposeAsync` after all tests
  complete — regardless of test outcomes.

- **Update `PackageFixture` to implement xUnit v3's `IAsyncLifetime`.** The interface
  moves to the `Xunit` namespace in v3; the implementation (run `dotnet pack` once,
  dispose the temporary directory) is unchanged.

- **Delete `PackageCollection.cs`.** The collection definition and its
  `ICollectionFixture<PackageFixture>` wiring are no longer needed.

- **Remove `[Collection(nameof(PackageCollection))]` from all 32 test classes.** Without
  a shared collection, xUnit v3 runs each test class as an independent parallel unit.

- **Update `CodingStandardsTestBase`.** Remove `PackageFixture` from the constructor.
  Test classes that need `PackageFixture` inject it directly (xUnit v3 supports assembly
  fixture injection into test class constructors). `ITestOutputHelper` injection is
  unchanged.

- **Fix xUnit v3 namespace changes.** `Xunit.Abstractions.ITestOutputHelper` becomes
  `Xunit.ITestOutputHelper`; update any `using` directives or implicit usings accordingly.

- **No parallelism cap initially.** Leave xUnit's default (parallel at the class level,
  degree = logical processor count). An `xunit.runner.json` file can be added later if
  saturation becomes a problem.

## Testing Decisions

This work is entirely structural — no analyzer rules, editorconfigs, or package content
change. The existing 795 integration tests collectively constitute the test for this
migration: if they all pass after the migration, the behaviour is preserved.

- **Verify in isolation first.** After the migration, run a single test class using
  `--filter` to confirm the new fixture wiring works before running the full suite.
- **Run the full suite last.** Run `dotnet test` without filters to confirm all 795 tests
  pass and that the parallel execution produces no flakiness (e.g. from shared state).
- **No new test classes required.** The migration does not introduce logic that needs its
  own coverage.

## Out of Scope

- Parallelism tuning (`xunit.runner.json`, `MaxParallelThreads`) — leave at xUnit's
  default for now; tune only if contention is observed.
- Per-test `dotnet restore` optimisation (e.g. `--no-restore`) — each test build
  already hits the local NuGet cache and does not regress.
- Any other xUnit v3 features (data-driven tests, `[Theory]` improvements, etc.).
- Upgrading `Shouldly` or `CliWrap` — verify compatibility but do not update versions
  as part of this PRD.
- CI timing benchmarks — wall-clock improvement on GitHub Actions is expected but not
  formally measured as part of this work.

## Further Notes

- The current `xunit.runner.visualstudio` is already at 3.1.5, which supports xUnit v3.
  Confirm the minimum version requirement in the xUnit v3 documentation before finalising
  the package reference.
- xUnit v3 removes the `Xunit.Abstractions` assembly entirely. Any `using Xunit.Abstractions`
  imports in test helpers or the base class will need updating.
- The 5 test classes that currently run *without* the `[Collection]` attribute
  (`RuleDocAttributeShould`, `RuleDocCoverageShould`, `RuleReferenceGeneratorShould`,
  `EditorConfigMergeGeneratorShould`, and one other) already run in parallel today.
  They require no changes beyond any xUnit v3 namespace fixes.
- Branch for this work: `feat/prd-xunit-v3-parallel-tests`.
