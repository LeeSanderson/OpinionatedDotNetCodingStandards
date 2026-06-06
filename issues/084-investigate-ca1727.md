## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1727 ("Use PascalCase for named placeholders") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on non-PascalCase named placeholders in structured logging message templates (ILogger extension methods); Microsoft.Extensions.Logging is not available in the simple single-project build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.Extensions.Logging.Abstractions` as a NuGet package reference, and verify the package is available on nuget.org with a stable version compatible with the test harness's target framework.
- Write a minimal test using `CreateProjectBuilder` with `Microsoft.Extensions.Logging.Abstractions` added as a `packageReference`, containing an `ILogger.LogInformation` call with a non-PascalCase named placeholder (e.g. `{myValue}` instead of `{MyValue}`), and check whether CA1727 fires in the SARIF output.
- If the above test does not fire CA1727, inspect the analyzer source (dotnet/roslyn-analyzers CA1727 implementation) to determine whether the rule requires the full `Microsoft.Extensions.Logging` package (runtime types) rather than just the abstractions package, and try adding `Microsoft.Extensions.Logging` instead.
- If adding the logging package makes CA1727 fire, remove the class-level `[RuleDoc]` entry from `UntestableRules.cs`, add a passing `[Fact]` test method with `[RuleDoc]` to the appropriate test file, and confirm the test passes in CI.
- If no combination of package references makes CA1727 fire (e.g. the analyzer requires a specific assembly identity or the package version conflicts with the harness), document the exact failure mode and update the `Untestable` reason in `UntestableRules.cs` with the confirmed specific reason.

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
