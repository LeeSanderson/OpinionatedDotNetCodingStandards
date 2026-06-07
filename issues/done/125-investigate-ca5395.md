## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5395 ("Miss HttpVerb attribute for action methods") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when ASP.NET MVC controller action methods lack an HTTP verb attribute; requires System.Web.Mvc or Microsoft.AspNetCore.Mvc which are not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter accepts additional NuGet packages, and whether adding `Microsoft.AspNetCore.Mvc` (or the relevant metapackage) is feasible without pulling in a large dependency graph that breaks the harness.
- Search nuget.org for a stable version of `Microsoft.AspNetCore.Mvc` (or `Microsoft.AspNetCore.App` framework reference) that is compatible with the target TFM used in the test harness, and confirm the package is publicly available without authentication.
- Write a minimal probe test that adds `Microsoft.AspNetCore.Mvc` via `packageReferences` in `CreateProjectBuilder`, defines a class that inherits from `Controller`, and declares a public method without any HTTP verb attribute (`[HttpGet]`, `[HttpPost]`, etc.) — verify whether CA5395 fires in SARIF output.
- If the AspNetCore route does not fire CA5395, check whether the analyzer targets the legacy `System.Web.Mvc.Controller` base class instead, and whether that type is accessible via any .NET-compatible shim or compatibility package on nuget.org.
- If neither package makes CA5395 fire, confirm the rule is permanently untestable and update the `Untestable` reason in `UntestableRules.cs` with the specific confirmed reason (e.g. "Microsoft.AspNetCore.Mvc cannot be added as a packageReference in the simple single-project build harness; System.Web.Mvc is .NET Framework-only and unavailable in .NET 5+").

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
