## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5368 ("Set ViewStateUserKey For Classes Derived From Page") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when a class derived from System.Web.UI.Page does not set ViewStateUserKey in Page_Init; System.Web.UI is not available in .NET Core/5+"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that `System.Web.UI.Page` is absent from .NET 5+ by checking the .NET API compatibility database (https://apisof.net) and verifying that no NuGet package re-exposes the type in a .NET 5+ compatible form.
- Search NuGet.org for any community or Microsoft package (e.g. `Microsoft.AspNetCore.SystemWebAdapters`) that provides a `System.Web.UI.Page` stub or shim compatible with `net8.0`; if found, test whether adding it via `CreateProjectBuilder`'s `packageReferences` parameter allows CA5368 to fire.
- Review the Roslyn analyzer source for CA5368 (https://github.com/dotnet/roslyn-analyzers) to confirm the rule performs a hard type-symbol lookup against `System.Web.UI.Page` and would silently produce no diagnostics if the type is unresolvable, rather than falling back to a string-based match.
- Attempt a minimal `CreateProjectBuilder` test using `<TargetFramework>net48</TargetFramework>` (or `net472`) to determine whether the test harness supports targeting .NET Framework; if it does, verify that a class deriving from `System.Web.UI.Page` without setting `ViewStateUserKey` triggers CA5368.
- If no .NET Framework target or compatible shim is available, update the untestable reason in `UntestableRules.cs` to the confirmed specific wording, e.g. "System.Web.UI.Page was removed from .NET Core 1.0 and no compatible shim exists for net8.0; cannot reference System.Web.UI in the test harness".

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
