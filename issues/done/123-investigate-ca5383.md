## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5383 ("Ensure Use Secure Cookies In ASP.NET Core") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant of CA5382 that tracks CookieOptions through variables; same ASP.NET Core dependency and taint-analysis constraint as CA5382"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Check whether `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.AspNetCore.App` or a minimal `Microsoft.AspNetCore.Http` NuGet package, and whether doing so is feasible without pulling in the full ASP.NET Core framework.
2. Check nuget.org for a stable, lightweight package that exposes `IResponseCookies`, `CookieOptions`, and `IResponseCookiesExtensions` — the exact types CA5383 requires — without requiring the full ASP.NET Core metapackage.
3. Write a minimal two-statement test that assigns a `CookieOptions` to a local variable and then passes it to `IResponseCookies.Append` without setting `Secure = true`, to confirm whether the taint-analysis path actually fires when the type is resolvable.
4. If the package is available and the test compiles, run it against the harness to see whether the SARIF output contains a CA5383 diagnostic; if it does, the rule is testable and a `[Fact]` can be written.
5. If the package cannot be added (network constraint, metapackage-only distribution, or no stable standalone package), confirm the rule is permanently untestable and update the `Untestable` reason in `UntestableRules.cs` to record the specific package-availability finding (e.g. "Microsoft.AspNetCore.Http is part of the shared framework and is not available as a standalone NuGet package; cannot be added via CreateProjectBuilder packageReferences").

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
