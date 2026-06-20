## Parent PRD

`issues/prd-improve-ci-pipeline-speed.md`

## What to build

Add an `xunit.runner.json` configuration file to the test project that pins
`maxParallelThreads` to `16`. This overrides xUnit v3's default of using the host's logical
processor count (2 on the Azure DevOps `windows-latest` agent), giving the CI agent 8× more
concurrent test threads. Because each test spawns I/O-bound child `dotnet build` processes
rather than doing CPU work in the test runner itself, oversubscribing threads directly
increases throughput on CI without degrading local runs.

The file goes in the test project directory alongside its `.csproj`, which is where xUnit
discovers it automatically. No pipeline changes are needed — this affects every environment
that runs the test project.

## Acceptance criteria

- [ ] `xunit.runner.json` exists in the test project directory with `maxParallelThreads` set
  to `16`
- [ ] The project builds without errors (`dotnet build`)
- [ ] A focused test passes locally (`dotnet test --filter` on any single test method)
- [ ] xUnit logs the resolved `MaxParallelThreads` value as `16` in CI test output after the
  PR merges

## Blocked by

None — can start immediately.

## User stories addressed

- User story 1 (CI test stage completes in under 15 minutes)
- User story 2 (release pipeline test stage completes faster)
- User story 3 (xUnit thread count explicitly configured, not silently CPU-count-dependent)
- User story 6 (same configuration applies locally and on CI)
