## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2023 ("Invalid braces in message template") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ILogger calls with syntactically invalid brace patterns in message templates; Microsoft.Extensions.Logging is not available in the simple single-project build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.Extensions.Logging.Abstractions`; if so, write a minimal test that adds the package and calls `ILogger.LogInformation` with an invalid brace pattern (e.g. `"{unclosed"` or `"{{}"`) to see if CA2023 fires.
- Search nuget.org for `Microsoft.Extensions.Logging.Abstractions` to confirm a stable version is available that is compatible with the target framework used by the build harness (net9.0 or net10.0).
- If the package reference approach works, write the minimal violation snippet and verify the diagnostic is emitted; confirm the test passes in CI before removing the class-level entry from `UntestableRules.cs`.
- If adding the package reference does not make the rule fire, read the CA2023 analyzer source (or the Roslyn analyzer documentation) to determine whether the rule requires the full `Microsoft.Extensions.Logging` implementation assembly rather than just the abstractions, and document the exact reason.
- Based on findings, either create a passing `[Fact]` test with `[RuleDoc]` and remove the class-level `UntestableRules.cs` entry, or update the untestable reason in `UntestableRules.cs` with a specific confirmed explanation (e.g. "CA2023 requires Microsoft.Extensions.Logging.Abstractions as a packageReference; package is available but CreateProjectBuilder does not support adding NuGet references to the transient project").

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
