## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5377 ("Use Container Level Access Policy") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when Azure Blob Storage container SAS tokens use an ad-hoc policy without a stored access policy; the Azure Storage SDK is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Check whether `CreateProjectBuilder`'s `packageReferences` parameter accepts a `WindowsAzure.Storage` or `Azure.Storage.Blobs` package reference, and whether either package is available on nuget.org with a stable version compatible with the test harness's target framework.
2. Review the CA5377 analyzer source (in `dotnet/roslyn-analyzers`) to identify the exact type and method call pattern that triggers the diagnostic — specifically which SDK types (`CloudBlobContainer`, `BlobContainerClient`, or similar) and which overloads of `GetSharedAccessSignature` / `GenerateSasUri` are matched.
3. Write a minimal test using `CreateProjectBuilder` with the identified package reference and the violation pattern. Run the test to see whether the diagnostic fires; if it does not fire, inspect SARIF output for any suppression or package-resolution issue.
4. If the package cannot be resolved in the harness (network dependency, incompatible TFM, or package not available), document the exact failure mode and confirm the rule as permanently untestable, updating the `Untestable` reason in `UntestableRules.cs` with that specific detail.
5. Cross-check CA5375 and CA5376 (same "Azure Storage SDK not included" reason, already confirmed untestable) to ensure CA5377's confirmed status is consistent and any shared conclusion is noted in the updated reason string.

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
