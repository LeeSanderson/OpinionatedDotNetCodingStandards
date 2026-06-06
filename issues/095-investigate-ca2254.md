## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2254 ("Template should be a static expression") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ILogger calls where the message template argument is a variable or non-constant expression rather than a string literal; Microsoft.Extensions.Logging is not available in the simple single-project build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.Extensions.Logging.Abstractions` as a NuGet package reference to the test project.
- Confirm that `Microsoft.Extensions.Logging.Abstractions` has a stable version available on nuget.org that is compatible with the target framework used by the build harness.
- Write a minimal test that adds `Microsoft.Extensions.Logging.Abstractions` via `packageReferences`, then calls an `ILogger` extension method (e.g. `LogInformation`) with a non-constant string variable as the message template argument, and verify whether CA2254 appears in the SARIF output.
- If the diagnostic fires, remove the class-level `[RuleDoc]` from `UntestableRules.cs`, add a `[Fact]` test method with a `[RuleDoc]` attribute in the appropriate test file, and confirm the test passes in CI.
- If the diagnostic does not fire even with the package reference, inspect the CA2254 analyzer source (dotnet/roslyn-analyzers) to determine whether the rule requires a specific `ILogger` overload, a minimum package version, or additional MSBuild properties; update the `Untestable` reason in `UntestableRules.cs` with the confirmed specific cause.

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
