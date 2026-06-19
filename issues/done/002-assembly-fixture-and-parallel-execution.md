## Parent PRD

`issues/prd-xunit-v3-parallel-tests.md`

## What to build

Rewire the test fixture infrastructure so `dotnet pack` runs exactly once per assembly and
all 32 integration test classes execute in parallel. This involves four coordinated changes
that must land together (each is broken in isolation):

1. Update `PackageFixture` to implement xUnit v3's `IAsyncLifetime` (namespace is now `Xunit`;
   the implementation — run `dotnet pack` once, dispose the temp directory — is unchanged).
2. Add `[assembly: AssemblyFixture(typeof(PackageFixture))]` to the test project so xUnit v3
   instantiates the fixture once for the whole assembly.
3. Delete `PackageCollection.cs` — the `ICollectionFixture` wiring is superseded by the
   assembly fixture.
4. Remove `[Collection(nameof(PackageCollection))]` from all 32 test classes that currently
   carry it, and update `CodingStandardsTestBase` to remove `PackageFixture` from its
   constructor (test classes receive it via direct assembly-fixture injection instead).

Refer to the "Register PackageFixture as an assembly fixture", "Delete PackageCollection.cs",
"Remove [Collection]", and "Update CodingStandardsTestBase" bullets in the PRD's
Implementation Decisions section.

## Acceptance criteria

- [ ] `PackageFixture` implements `Xunit.IAsyncLifetime` (v3 namespace) and compiles cleanly.
- [ ] An `[assembly: AssemblyFixture(typeof(PackageFixture))]` attribute is present in the
      test project (e.g. in `AssemblyFixtures.cs` or an existing assembly-attribute file).
- [ ] `PackageCollection.cs` is deleted.
- [ ] No test class carries a `[Collection]` attribute.
- [ ] `CodingStandardsTestBase` no longer takes `PackageFixture` in its constructor; test
      classes that need it inject it directly.
- [ ] A single test class passes when run in isolation via `--filter`.
- [ ] The full suite passes via `dotnet test` with no failures and no flakiness from shared
      state (run at least once end-to-end to confirm).
- [ ] `dotnet pack` is invoked exactly once during a full test run (observable from test
      output — the fixture log line appears only once).
- [ ] The 4 test classes that never had a `[Collection]` attribute
      (`RuleDocAttributeShould`, `RuleDocCoverageShould`, `RuleReferenceGeneratorShould`,
      `EditorConfigMergeGeneratorShould`) continue to pass without modification beyond any
      namespace fixes already applied in `issues/001-upgrade-xunit-packages-to-v3.md`.

## Blocked by

- `issues/001-upgrade-xunit-packages-to-v3.md`

## User stories addressed

- User story 1 (full suite completes in under 10 minutes)
- User story 2 (parallel execution without extra flags)
- User story 3 (`dotnet pack` runs exactly once per test run)
- User story 4 (packed NuGet package cleaned up reliably after run)
- User story 5 (cleanup happens even when tests fail)
- User story 6 (`--filter` still works for a single class in isolation)
- User story 7 (test output still routed to `ITestOutputHelper`)
- User story 8 (each test class gets its own isolated temp project directory)
- User story 9 (faster CI feedback on GitHub Actions)
- User story 11 (parallelism degree tunable via `xunit.runner.json` if needed)
- User story 12 (lightweight unit tests continue working unchanged)
