## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5375 ("Do Not Use Account Shared Access Signature") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on Azure Storage SDK CloudStorageAccount.GetSharedAccessSignature calls; the Azure Storage SDK is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding the Azure Storage SDK package (`WindowsAzure.Storage` or `Azure.Storage.Blobs`) as a NuGet reference, and whether the test harness infrastructure supports resolving external packages at build time.
- Search nuget.org for a stable version of the required Azure Storage package (`WindowsAzure.Storage` or its successor `Azure.Storage.Blobs`) and confirm it is publicly available and compatible with the target .NET version used by the test harness.
- Write a minimal test that adds the Azure Storage SDK via `packageReferences`, then calls `CloudStorageAccount.GetSharedAccessSignature` in the violation source code, and observe whether CA5375 appears in the SARIF/diagnostic output.
- If `WindowsAzure.Storage` (the classic SDK) is unavailable or deprecated, check whether the analyzer also fires on `Azure.Storage.Blobs` or `Azure.Storage.Common` equivalents, and if so update the test accordingly.
- If the package cannot be resolved in the harness (network policy, package feed, or harness limitation), document this as the confirmed permanent untestable reason and update the `Untestable` string in `UntestableRules.cs` with the specific confirmed reason (e.g. "Azure Storage SDK cannot be added as a packageReference in the isolated test harness; NuGet restore is unavailable in the build environment").

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
