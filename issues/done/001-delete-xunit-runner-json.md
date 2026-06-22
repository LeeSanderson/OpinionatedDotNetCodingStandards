## Parent PRD

`issues/prd-release-tagging-scripts.md`

## What to build

Delete `tests/Opinionated.DotNet.CodingStandards.Tests/xunit.runner.json` from the repo permanently. The file capped `maxParallelThreads` to 4, which was needed for CI stability under the old xUnit v2 collection model. With xUnit v3's class-level parallelism, the cap is unnecessary and slows local test runs. No replacement file is needed.

## Acceptance criteria

- [ ] `tests/Opinionated.DotNet.CodingStandards.Tests/xunit.runner.json` is deleted and not replaced
- [ ] `dotnet test` passes with all tests green after the deletion
- [ ] Wall-clock time for the full local test run is approximately 6 minutes (down from longer with the 4-thread cap)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 17
