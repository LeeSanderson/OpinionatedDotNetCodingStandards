## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2315 ("Do not use insecure deserializer ObjectStateFormatter") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "ObjectStateFormatter is in System.Web.UI (ASP.NET classic WebForms); the type is not available in .NET Core/5+ and cannot be referenced in the test harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that `System.Web.UI.ObjectStateFormatter` is absent from all .NET 5+ / .NET Core target frameworks by checking the dotnet/runtime repository and the official .NET API browser (https://learn.microsoft.com/dotnet/api/system.web.ui.objectstateformatter) for supported frameworks.
- Check whether any NuGet package (e.g. a System.Web compatibility shim or a community re-implementation) re-exposes `ObjectStateFormatter` in a form that .NET 5+ projects can reference, and whether such a package would satisfy the analyzer's type-identity check.
- Review the Roslyn analyzer source for CA2315 (https://github.com/dotnet/roslyn-analyzers/blob/main/src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerObjectStateFormatter.cs) to determine whether the rule matches by fully-qualified type name or by interface/pattern, which would affect whether a compatible shim could trigger it.
- Attempt to add a `packageReferences` entry for any identified shim package via `CreateProjectBuilder` in the test harness and write a minimal violation pattern (instantiating `ObjectStateFormatter` and calling `Deserialize`); verify whether the CA2315 diagnostic fires or whether the type-identity mismatch prevents it.
- If no compatible package exists and the type-identity check confirms the analyzer requires the real `System.Web.UI.ObjectStateFormatter`, document as permanently untestable and update the `Untestable` reason in `UntestableRules.cs` with the specific confirmed detail (e.g. "System.Web.UI.ObjectStateFormatter was never ported to .NET Core/5+; no compatible shim satisfies the analyzer's type-identity check; permanently untestable").

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
