## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2253 ("Named placeholders should not be numeric values") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ILogger structured logging message templates that use numeric placeholder names ({0}) instead of named placeholders ({name}); Microsoft.Extensions.Logging is not available in the simple single-project build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter accepts `Microsoft.Extensions.Logging.Abstractions` and whether the package is available on nuget.org with a stable version compatible with the test harness's target framework.
- Write a minimal test that adds `Microsoft.Extensions.Logging.Abstractions` via `packageReferences`, then compiles a snippet that calls `ILogger.LogInformation("{0}", value)` using a numeric placeholder, and verify whether CA2253 fires.
- If the minimal test does not fire, inspect the CA2253 analyzer source (dotnet/roslyn-analyzers on GitHub) to confirm exactly which method signatures or attributes it requires, and whether the abstraction package alone is sufficient or whether `Microsoft.Extensions.Logging` (the full package) is also needed.
- If the full `Microsoft.Extensions.Logging` package is required, check whether it can be added to `CreateProjectBuilder` without pulling in transitive dependencies that break the harness, and repeat the minimal firing test.
- If no combination of package references produces a diagnostic, document as permanently untestable with a specific sourced reason (e.g. "CA2253 requires runtime ILogger extension methods that are only resolved when Microsoft.Extensions.Logging is present; adding the package to the single-project harness introduces transitive conflicts preventing compilation").

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
