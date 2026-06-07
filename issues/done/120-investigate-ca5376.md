## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5376 ("Use SharedAccessProtocol HttpsOnly") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when Azure Storage SDK SharedAccessPolicy uses HTTP instead of HTTPS-only; the Azure Storage SDK is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter accepts additional NuGet packages, and if so whether adding the Azure Storage SDK package (`Azure.Storage.Blobs` or the legacy `WindowsAzure.Storage`) is practical in the test harness.
- Search nuget.org for the Azure Storage SDK packages (`Azure.Storage.Blobs`, `Azure.Storage.Common`, `WindowsAzure.Storage`) to confirm a stable version is available and determine which package actually exposes the `SharedAccessProtocol` enum and `SharedAccessPolicy` type that CA5376 analyses.
- Read the CA5376 analyzer source (in the `dotnet/roslyn-analyzers` repository) to identify the exact type names and method signatures that trigger the diagnostic, so a minimal violation pattern can be written.
- Write a minimal test using `CreateProjectBuilder` with the identified package reference, containing code that constructs a `SharedAccessPolicy` without setting `SharedAccessProtocol.HttpsOnly`, and verify whether the diagnostic fires.
- If the package reference approach does not work (e.g. network dependency, package resolution failure, or the rule still does not fire), confirm permanently untestable and update the `Untestable` reason in `UntestableRules.cs` with the specific confirmed reason (e.g. "Azure Storage SDK cannot be added as a packageReference in the build harness due to X").

## Acceptance criteria

- [ ] Root cause confirmed: the rule is either permanently untestable (documented reason) or a workaround exists
- [ ] One of:
  - [ ] A working violation pattern found → new `[Fact]` test method created with `[RuleDoc]`, class-level entry removed from UntestableRules.cs, test passes in CI; OR
  - [ ] Permanently untestable confirmed → Untestable reason in UntestableRules.cs updated with the specific confirmed reason (e.g. "type removed in .NET Core 1.0; cannot reference System.Web in .NET 5+ projects")
- [ ] No regressions in other tests
- [ ] RuleReferenceGenerator coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
