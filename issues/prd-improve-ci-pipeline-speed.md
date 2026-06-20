# PRD: Improve CI Pipeline Test Speed

## Problem Statement

The integration test suite runs in approximately 6 minutes locally (8-core machine) but takes
approximately 30 minutes on the Azure DevOps CI agent (`windows-latest`, 2 vCPUs). This makes
CI feedback slow: a developer merging a PR or cutting a release waits five times longer than
their local run suggests, and the full release pipeline is bottlenecked by the test stage.

Two root causes combine to produce the gap:

1. **Parallelism mismatch.** xUnit v3 defaults `MaxParallelThreads` to the logical processor
   count of the host. Locally that is 8; on the Azure DevOps hosted agent it is 2. Because each
   test spawns child `dotnet build` processes (I/O-bound, not CPU-bound in the test runner
   itself), the test runner thread count directly gates throughput. The 4× reduction in threads
   accounts for most of the 5× wall-clock difference.

2. **Cold NuGet global packages cache.** Each CI run starts on a fresh agent VM. The `dotnet
   restore` for the solution populates the global packages cache, but if the cache is not
   persisted between runs, subsequent runs re-download the same packages unnecessarily, adding
   latency to both the solution restore step and any package restores inside the throwaway test
   projects.

## Solution

Two targeted, additive configuration changes — no changes to test logic or package content:

1. **Add `xunit.runner.json`** to the test project, pinning `maxParallelThreads` to `16`.
   This oversubscribes the CI agent's 2 vCPUs intentionally: the test runner threads are almost
   entirely idle (waiting for child `dotnet build` processes to complete), so more threads
   directly increases I/O concurrency. The value 16 matches observed effective throughput on
   local 8-core machines and leaves headroom for CI agents with more cores.

2. **Add a NuGet `Cache@2` task** to all three Azure Pipelines files (`ci.yml`,
   `analyzer-bump.yml`, `release.yml`), placed before the `dotnet restore` step. The cache is
   keyed on `Directory.Packages.props` (all package versions live here) and the `**/*.csproj`
   glob (project references), restoring to the NuGet global packages folder
   (`$(UserProfile)\.nuget\packages`). On a cache hit the restore step becomes a no-op.

Expected outcome: CI test run drops from ~30 minutes to ~8–12 minutes.

## User Stories

1. As a developer, I want the CI test stage to complete in under 15 minutes, so that I receive
   PR feedback quickly without context-switching away.
2. As a developer, I want the release pipeline test stage to complete faster, so that version
   tags I push are published to NuGet.org sooner.
3. As a developer, I want xUnit's thread count to be explicitly configured, so that CI
   performance does not silently degrade if Azure DevOps changes the vCPU count of hosted
   agents.
4. As a developer, I want the NuGet global packages cache to be persisted between CI runs, so
   that package restores are near-instant on cache hits.
5. As a developer, I want the cache key to be tied to `Directory.Packages.props`, so that the
   cache is automatically invalidated whenever a package version changes.
6. As a developer, I want the same `xunit.runner.json` to apply locally, so that local and CI
   runs use the same configuration and there are no surprises.
7. As a developer, I want the `analyzer-bump.yml` Build stage to also benefit from the NuGet
   cache, so that analyzer-bump PRs are not slower than regular CI runs.
8. As a developer, I want the release pipeline to also benefit from the NuGet cache, so that
   the release cycle is not unnecessarily prolonged.

## Implementation Decisions

- **`xunit.runner.json` location:** placed in the test project directory alongside the
  `.csproj`, which is the standard location xUnit discovers automatically.
- **`maxParallelThreads: 16`:** chosen to oversubscribe CI's 2 vCPUs because the bottleneck
  is I/O (child process throughput), not CPU in the test runner. The value matches effective
  local throughput on an 8-core machine.
- **Cache key design:** primary fingerprint is `Directory.Packages.props` (the single source
  of truth for all package versions in this repo) combined with `**/*.csproj` to catch any
  project-level package additions. A change to either file invalidates the cache.
- **Cache restore path:** `$(UserProfile)\.nuget\packages` — the NuGet global packages folder
  on Windows hosted agents.
- **All three pipelines updated:** `ci.yml` (PR + main push), `analyzer-bump.yml` (Build
  stage only — Regen stage does not run tests), and `release.yml`. The cache task is inserted
  before the `DotNetCoreCLI restore` task in each.
- **No changes to test code, package content, or existing pipeline logic** — purely additive
  configuration.

## Testing Decisions

These two changes are infrastructure-only. No new xUnit tests are required. Correctness is
verified by observing CI run duration after the PR merges:

- The `xunit.runner.json` change is self-evidencing: xUnit logs the resolved
  `MaxParallelThreads` value at the start of a test run; confirm it shows `16` in CI output.
- The cache task is self-evidencing: Azure DevOps marks a `Cache@2` step as "Cache hit" or
  "Cache miss" in the pipeline UI; confirm a hit on the second run after the PR merges.
- End-to-end signal: CI test stage wall-clock time drops from ~30 min toward ~8–12 min.

## Out of Scope

- **Larger Azure DevOps hosted agents** (e.g. 4-vCPU or 8-vCPU tiers) — adding more real CPU
  would further reduce time but incurs cost and requires a paid plan change; not part of this
  PRD.
- **Fanning tests out across multiple parallel CI jobs** (matrix strategy) — would give linear
  scaling but requires significant pipeline restructuring; revisit if this PRD does not achieve
  the target.
- **`analyzer-bump.yml` Regen stage** — does not run tests; no cache or parallelism change
  needed there.
- **Changes to test logic, `PackageFixture`, or `ProjectBuilder`** — out of scope; the
  performance fix is configuration-only.
- **Linux or macOS agents** — the repo targets `windows-latest`; no cross-platform changes
  needed.

## Further Notes

- The xUnit v3 migration (previous PRD: `prd-xunit-v3-parallel-tests.md`) already removed the
  serial `PackageCollection` constraint and enabled class-level parallelism. This PRD builds
  on that work by tuning the thread count for the CI environment.
- If `maxParallelThreads: 16` causes flakiness (e.g. disk contention from too many concurrent
  `dotnet build` processes), reduce to `8` as a first step before investigating further.
- The `Cache@2` task uses a `restoreKeys` fallback (partial key match) so that a
  `Directory.Packages.props` change that adds one package still gets a near-hit from the
  previous cache, minimising cold-restore latency on version-bump PRs.
