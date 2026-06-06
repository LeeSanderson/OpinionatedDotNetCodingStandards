## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5382 ("Use Secure Cookies In ASP.NET Core") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when IResponseCookies.Append is called without setting Secure = true in the CookieOptions; requires Microsoft.AspNetCore.Http which is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter accepts a `Microsoft.AspNetCore.Http` package reference and whether that package is available on nuget.org with a stable version compatible with the test harness's target framework.
- Write a minimal test that adds `Microsoft.AspNetCore.Http` via `packageReferences` and calls `IResponseCookies.Append(string, string)` without a `CookieOptions` argument (or with `CookieOptions { Secure = false }`), then verify whether CA5382 appears in the SARIF output.
- If the direct `IResponseCookies` approach does not fire, check the analyzer source (dotnet/roslyn-analyzers `UseSecureCookiesAnalyzer`) to confirm exactly which method signatures and types trigger the diagnostic, and adjust the violation pattern accordingly.
- If adding `Microsoft.AspNetCore.Http` as a standalone package reference is blocked (e.g. version conflicts with the SDK's implicit ASP.NET Core references), check whether switching the test project's SDK to `Microsoft.NET.Sdk.Web` in `CreateProjectBuilder` would allow the types to resolve without additional package references.
- If none of the above produce a firing diagnostic, confirm permanently untestable: document the specific confirmed reason (e.g. "Microsoft.AspNetCore.Http cannot be added as an isolated package reference in the simple console-SDK build harness; switching to Microsoft.NET.Sdk.Web is out of scope for the single-project harness") and update the `Untestable` reason string in `UntestableRules.cs` accordingly.

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
