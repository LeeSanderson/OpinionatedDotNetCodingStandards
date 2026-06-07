## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5365 ("Do Not Disable HTTP Header Checking") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when HttpRuntimeSection.EnableHeaderChecking is set to false; requires System.Web.Configuration which is not available in .NET Core/5+"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that `System.Web.Configuration.HttpRuntimeSection` is absent from .NET 5+ by checking the dotnet/runtime API catalogue or attempting to reference it in the test harness; document the exact .NET version in which the type was removed.
- Check whether a compatible replacement type exists in .NET Core/5+ (e.g. in `Microsoft.AspNetCore.Server.Kestrel` or a `System.Web` compatibility shim) that the CA5365 analyzer recognises as the same trigger point; if so, write a minimal test using `CreateProjectBuilder` with a `packageReferences` entry for that package.
- Read the CA5365 analyzer source (search `dotnet/roslyn-analyzers` on GitHub for `DoNotDisableHttpHeaderChecking` or `CA5365`) to confirm which exact type and property the rule pattern-matches against; verify there is no alternative API surface that could serve as a stand-in trigger.
- Check NuGet for a `System.Web` compatibility package (e.g. `Microsoft.AspNet.WebApi.Core` or a community shim) that exposes `HttpRuntimeSection` and is resolvable in a .NET 5+ project; if found, attempt to add it via `CreateProjectBuilder`'s `packageReferences` parameter and run the rule.
- If no path forward is found in the steps above, update the untestable reason in `UntestableRules.cs` to include the specific confirmed evidence (e.g. "HttpRuntimeSection removed in .NET Core 1.0; no compatible replacement type exists that the CA5365 analyzer recognises; System.Web.Configuration cannot be referenced in .NET 5+ projects").

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
