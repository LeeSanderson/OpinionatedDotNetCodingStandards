## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5363 ("Do Not Disable Request Validation") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ASP.NET [ValidateInput(false)] attribute on MVC action methods; System.Web.Mvc is not available in .NET Core/5+"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Web.Mvc` (the assembly containing `ValidateInputAttribute`) is absent from .NET Core/5+ by checking the dotnet/runtime and dotnet/aspnetcore repos and the official .NET API browser (https://learn.microsoft.com/dotnet/api/?view=net-9.0) for the type `System.Web.Mvc.ValidateInputAttribute`.
2. Check whether any replacement type in ASP.NET Core triggers the same CA5363 diagnostic — search the NetAnalyzers source for the list of types/attributes the rule recognises (https://github.com/dotnet/roslyn-analyzers) to see if `Microsoft.AspNetCore.*` types are included alongside `System.Web.Mvc`.
3. Attempt a minimal test using `CreateProjectBuilder` with a `packageReferences` entry for `Microsoft.AspNet.Mvc` (the legacy NuGet shim) to see if the System.Web.Mvc types can be brought in under .NET Core; verify whether the package compiles and whether CA5363 fires.
4. If no replacement type fires CA5363, verify on nuget.org and the Microsoft docs that `System.Web.Mvc` has no .NET 5+ compatible package, and confirm the rule is limited to classic ASP.NET MVC (targeting `net48` or lower only).
5. Update the untestable reason in `UntestableRules.cs` with the specific confirmed finding — either "type `System.Web.Mvc.ValidateInputAttribute` removed in .NET Core 1.0; no ASP.NET Core replacement recognised by CA5363; permanently untestable on .NET 5+" or, if a workaround is found, create a `[Fact]` test and remove the class-level entry.

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
