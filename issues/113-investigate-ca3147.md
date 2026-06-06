## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA3147 ("Mark Verb Handlers With Validate Antiforgery Token") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Requires ASP.NET MVC (System.Web.Mvc) controller action methods decorated with HTTP verb attributes; System.Web.Mvc is not available in .NET Core and ASP.NET Core MVC is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Web.Mvc` is absent from .NET 5+ by checking the .NET BCL reference source and NuGet — verify there is no `System.Web.Mvc` package targeting `net8.0` or later that would allow referencing `System.Web.Mvc.Controller` and the HTTP verb attributes (`[HttpGet]`, `[HttpPost]`, etc.) that CA3147 looks for.
2. Check the CA3147 analyzer source (roslyn-analyzers repo, `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/MarkVerbHandlersWithValidateAntiForgeryToken.cs`) to determine the exact set of types and attributes the rule inspects — specifically whether it checks only `System.Web.Mvc` types or also `Microsoft.AspNetCore.Mvc` types.
3. If the analyzer source references `Microsoft.AspNetCore.Mvc` types as well, check whether adding `Microsoft.AspNetCore.Mvc` (or `Microsoft.AspNetCore.App` framework reference) as a `packageReferences` entry in `CreateProjectBuilder` is feasible, then write a minimal test: a controller action decorated with `[HttpPost]` but missing `[ValidateAntiForgeryToken]`.
4. If the analyzer source only references `System.Web.Mvc` types, confirm there is no .NET Core-compatible shim or compatibility package (e.g. `Microsoft.AspNet.Mvc` on NuGet) that exposes the exact type names the analyzer checks against, and document the finding.
5. Record the confirmed outcome: either a working violation pattern (proceed to write the `[Fact]` test and remove the class-level `UntestableRules` entry) or a permanent untestability confirmation with the specific type unavailability cited.

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
