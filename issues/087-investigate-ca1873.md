## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1873 ("Avoid potentially expensive logging") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on string concatenation/interpolation in ILogger.LogXxx calls; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.Extensions.Logging.Abstractions`; review the method signature in the test harness to confirm the parameter exists and accepts NuGet package identifiers.
- Search nuget.org for `Microsoft.Extensions.Logging.Abstractions` and confirm a stable version is available that is compatible with the target framework used by the test harness.
- Write a minimal test that adds `Microsoft.Extensions.Logging.Abstractions` as a `packageReference` in `CreateProjectBuilder`, then supplies a code snippet that calls `ILogger.LogInformation` with a string interpolation argument (e.g. `logger.LogInformation($"Value is {value}")`), and verify whether CA1873 fires.
- If the package reference approach works, create a `[Fact]` test method with a `[RuleDoc]` attribute, remove the class-level entry from `UntestableRules.cs`, and confirm the test passes in CI.
- If the package reference approach fails (e.g. the harness cannot resolve transitive dependencies or the rule still does not fire), document the specific failure mode and update the untestable reason in `UntestableRules.cs` with a precise, sourced explanation (e.g. "Microsoft.Extensions.Logging.Abstractions cannot be resolved in the single-project build harness because …").

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
