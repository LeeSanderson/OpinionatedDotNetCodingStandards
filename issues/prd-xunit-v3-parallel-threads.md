# PRD: xUnit v3 Migration, Parallelisation, and Analyzer Package Update

## Problem Statement

Two related problems need to be solved together to make the test suite releasable after the
analyzer bump:

1. **Slow test suite.** The full integration test suite takes approximately 40 minutes to run.
   All 795 test methods live in a single xUnit collection (`PackageCollection`), which
   serialises the entire suite and eliminates all parallelism.

2. **New analyzer rules without test coverage.** `Meziantou.Analyzer` has been updated from
   3.0.105 to 3.0.107, introducing two new diagnostic rules (MA0204, MA0205) that are not yet
   covered by the test suite. The analyzer bump must land on the same branch as the xUnit
   migration because both touch `Directory.Packages.props`.

## Solution

1. **xUnit v3 migration.** Upgrade the test project from xUnit 2.9.3 to xUnit v3 and use
   `IAssemblyFixture<T>` to run `dotnet pack` exactly once per test assembly. Remove
   `PackageCollection` and all `[Collection]` attributes so xUnit v3 runs test classes in
   parallel by default, reducing wall-clock time from ~40 minutes to under 10 minutes.

2. **Analyzer bump.** Update `Meziantou.Analyzer` from 3.0.105 to 3.0.107 in
   `Directory.Packages.props` and `.nuspec`, regenerate the affected editorconfig, and add
   test coverage for the two newly-discovered rules.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | 3.0.105 | 3.0.107 |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| MA0204 | Analyzer.Meziantou.Analyzer.editorconfig | Added |
| MA0205 | Analyzer.Meziantou.Analyzer.editorconfig | Added |

## User Stories

### xUnit v3 Migration

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

### Analyzer Package Update

13. As a maintainer, I want the analyzer packages updated so that new rules are enforced on
    consuming projects.
14. As a maintainer, I want each new rule covered by a test so that the package's rule coverage
    is verified.
15. As a maintainer, I want the test suite to remain green after the update so that the package
    remains releasable.

## Implementation Decisions

### xUnit v3 Migration

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

### Analyzer Package Update

- **`Directory.Packages.props` and `.nuspec` versions are updated in lockstep.** The
  `CheckNugetDependenciesMatchProps.cs` script enforces this; update both together.
- **Re-run `scripts/UpdateAnalyzerEditorConfigs.cs`** after the package bump to pick up
  added/stale rule IDs in the editorconfig files.
- **Each new rule gets its own issue** with guidance on how to write the test and which
  confounders to exhaust before marking a rule untestable.

## Testing Decisions

### xUnit v3 Migration

This work is entirely structural — no analyzer rules, editorconfigs, or package content
change. The existing integration tests collectively constitute the test for this migration:
if they all pass after the migration, the behaviour is preserved.

- **Verify in isolation first.** After the migration, run a single test class using
  `--filter` to confirm the new fixture wiring works before running the full suite.
- **Run the full suite last.** Run `dotnet test` without filters to confirm all tests
  pass and that the parallel execution produces no flakiness (e.g. from shared state).
- **No new test classes required.** The migration does not introduce logic that needs its
  own coverage.

### Analyzer Package Update

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and
  each per-rule issue).
- Run new tests in isolation: `dotnet test --no-build --filter "FullyQualifiedName~MyNewTest"`.
- Only run the full suite if shared helpers or package content changed.

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
- Bumping non-analyzer dependencies beyond xUnit v3 (e.g., `CliWrap`).
- Changing rule severities for existing rules — that is a separate, deliberate change.

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
- The editorconfig update script adds new rules at their default/suggested severity. Review
  the `Added:` output to identify rules that might warrant a different severity.
- Branch for this work: `feat/prd-xunit-v3-parallel-tests`.
