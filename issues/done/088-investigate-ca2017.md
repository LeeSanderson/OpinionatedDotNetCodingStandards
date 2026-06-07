## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2017 ("Parameter count mismatch") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ILogger message templates where argument count does not match the number of named placeholders; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Check if `CreateProjectBuilder`'s `packageReferences` parameter accepts additional NuGet packages, and whether `Microsoft.Extensions.Logging.Abstractions` can be injected without breaking the harness's single-project assumptions.
2. Find the latest stable version of `Microsoft.Extensions.Logging.Abstractions` on nuget.org and verify it is compatible with the target framework used in the test harness (check the `.csproj` or `CreateProjectBuilder` defaults for `<TargetFramework>`).
3. Write a minimal test snippet that calls `ILogger.LogInformation` (or an equivalent `LoggerExtensions` overload) with a mismatched argument count — e.g. `logger.LogInformation("Hello {Name} {Age}", "Alice")` — and confirm via a local build that CA2017 fires when the package reference is present.
4. If the package reference approach works, create a new `[Fact]` test method in the appropriate `CodeAnalysisRules*Should.cs` file (mirroring the CA1727 pattern if one exists), annotate it with `[RuleDoc]`, remove the class-level entry from `UntestableRules.cs`, and run the full test suite to confirm no regressions.
5. If adding the package reference does not make CA2017 fire (e.g. the analyzer requires the full `Microsoft.Extensions.Logging` host infrastructure, not just the abstractions), document the precise reason — citing the specific assembly or type that is missing — and update the untestable reason in `UntestableRules.cs` accordingly.

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
