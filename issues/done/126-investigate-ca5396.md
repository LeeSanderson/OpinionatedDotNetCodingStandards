## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5396 ("Set HttpOnly to true for HttpCookie") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when System.Web.HttpCookie.HttpOnly is set to false or not set; System.Web is not available in .NET Core/5+"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that `System.Web.HttpCookie` is absent from the .NET 5+ BCL by checking the official .NET API browser (https://learn.microsoft.com/dotnet/api/system.web.httpcookie) and verifying the target framework moniker list does not include `net5.0` or later.
- Check whether any NuGet package (e.g. a community shim or the `Microsoft.AspNetCore.SystemWebAdapters` compatibility package) re-exports `System.Web.HttpCookie` in a form that is resolvable in a `net9.0` project and that the Roslyn analyzer actually inspects the re-exported type.
- Read the CA5396 analyzer source in the `dotnet/roslyn-analyzers` repository (search `CA5396` under `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security`) to confirm whether the rule matches by fully-qualified type name `System.Web.HttpCookie` only, or by duck-typing on a property named `HttpOnly`; if duck-typed, a hand-written stand-in class might trigger it.
- If `CreateProjectBuilder` supports a `packageReferences` parameter, attempt a minimal test project that references `Microsoft.AspNetCore.SystemWebAdapters` (stable version available on nuget.org) and writes `new HttpCookie("x") { HttpOnly = false }`, then run the build harness to see whether CA5396 appears in SARIF output.
- If no path forward is found in the above steps, update the `Untestable` reason in `UntestableRules.cs` to the specific confirmed wording, e.g. "System.Web.HttpCookie was removed in .NET Core 1.0; no compatible replacement type triggers CA5396 in .NET 5+ projects; permanently untestable".

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
