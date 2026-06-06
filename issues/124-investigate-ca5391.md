## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5391 ("Use antiforgery tokens in ASP.NET Core MVC controllers") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when ASP.NET Core MVC controller action methods lack antiforgery token validation attributes; requires Microsoft.AspNetCore.Mvc which is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.AspNetCore.Mvc` (or the umbrella `Microsoft.AspNetCore.App` framework reference) to the test project, and whether doing so is practical given the harness's single-project constraint.
- Check nuget.org for a stable, pinnable version of `Microsoft.AspNetCore.Mvc` (or the ASP.NET Core shared framework) that can be added as a `<PackageReference>` without pulling in a large transitive closure that would destabilise the rest of the harness.
- Write a minimal test that adds the package reference via `CreateProjectBuilder`, declares a class that inherits from `Microsoft.AspNetCore.Mvc.Controller`, adds a POST action method without `[ValidateAntiForgeryToken]` or `[AutoValidateAntiforgeryToken]`, and checks whether CA5391 appears in the SARIF output.
- If the package reference approach fails (e.g. ASP.NET Core types are only available via the shared framework / `Microsoft.AspNetCore.App` FrameworkReference, not a standalone NuGet package), check whether a stub/shim type — a hand-written class named `Microsoft.AspNetCore.Mvc.ControllerBase` in the test source — is enough to satisfy the analyzer's symbol lookup and make CA5391 fire without the real assembly.
- If neither approach works, confirm the root cause precisely (shared-framework-only availability, FrameworkReference not supported by the harness, or analyzer requires real assembly identity) and update the `Untestable` reason in `UntestableRules.cs` with that specific confirmed reason.

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
