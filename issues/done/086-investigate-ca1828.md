## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1828 ("Do not use CountAsync() or LongCountAsync() when AnyAsync() can be used") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires only for Entity Framework Core CountAsync()/LongCountAsync() extension methods on IQueryable<T>; the full EF Core package is not practical to add to a standalone single-project harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.EntityFrameworkCore` (or a lighter EF Core stub) and whether the package is available on nuget.org with a stable version compatible with the harness's target framework.

2. Attempt to write a minimal test that adds `Microsoft.EntityFrameworkCore` via `packageReferences`, defines a class with an `IQueryable<T>` source, and calls `CountAsync()` where `AnyAsync()` should be used instead — confirm whether CA1828 fires.

3. If the full `Microsoft.EntityFrameworkCore` package causes restore or compilation issues in the single-project harness, investigate whether a trimmed stub assembly that declares `IQueryable<T>` extension methods `CountAsync` and `AnyAsync` with the exact signatures EF Core uses can fool the analyzer into firing CA1828 without the real package.

4. Check the Roslyn analyzer source for CA1828 (dotnet/roslyn-analyzers repository) to confirm exactly which type and method signatures trigger the diagnostic — verify whether the rule checks for the presence of the real EF Core assembly or only inspects method names and parameter types.

5. If none of the above paths produce a firing diagnostic, document the specific confirmed reason (e.g. "analyzer checks for the concrete EF Core assembly identity, not just method signatures; adding the full EF Core package to the single-project harness causes restore conflicts with the test SDK") and update the untestable reason in `UntestableRules.cs` accordingly.

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
