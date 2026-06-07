## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2356 ("Unsafe DataSet or DataTable type in web deserializable object graph") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on ASP.NET Web API / WCF action methods whose return type or parameters include a DataSet or DataTable in an unsafe deserialization context; requires System.Web or ASP.NET Core web framework not available in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Read the official CA2356 documentation and the roslyn-analyzers source (DataSetDataTableInWebSerializableObjectGraphAnalyzer) to confirm exactly which trigger conditions the rule checks for — specifically whether it requires ASP.NET Web API / WCF controller base types or merely a specific attribute pattern on any class.
2. Check whether `System.Data.DataSet` and `System.Data.DataTable` are available in .NET 8+ (they are in System.Data, part of the base BCL); if available, determine whether CA2356 fires on a plain class method annotated with a web-framework attribute stub (e.g. a hand-written `[WebMethod]` attribute defined locally in the test project) or whether it requires the actual `System.Web.Services.WebMethodAttribute` type reference.
3. Check if adding `Microsoft.AspNetCore.Mvc` (or `Microsoft.AspNetCore.App` framework reference) as a `packageReferences` entry in `CreateProjectBuilder` is feasible and would supply the controller base types / `[ApiController]` / `[HttpGet]` attributes that CA2356 checks; verify whether the package is available as a stable NuGet reference outside the ASP.NET Core shared framework.
4. Check the roslyn-analyzers GitHub issue tracker and release notes for any reports of CA2356 firing outside of a full web-framework context (e.g. on any method whose parameter or return type graph contains DataSet/DataTable regardless of controller ancestry); if so, write a minimal inline test to confirm.
5. If no viable workaround is found, update the `Untestable` reason in `UntestableRules.cs` to be more precise — distinguishing whether the blocker is (a) the absence of ASP.NET Core MVC / Web API types that CA2356 inspects as trigger symbols, or (b) a taint/data-flow requirement that cannot be satisfied in a single-file harness — and cite the source (analyzer file name or documentation URL) that confirms the constraint.

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
