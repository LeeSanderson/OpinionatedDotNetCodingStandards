## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2305 ("Do not use insecure deserializer LosFormatter") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "LosFormatter is in System.Web (ASP.NET classic); the type is not available in .NET Core/5+ and cannot be referenced in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Web.UI.LosFormatter` is absent from .NET 5+ by checking the dotnet/runtime repository and the official .NET API browser (https://learn.microsoft.com/dotnet/api/system.web.ui.losformatter) for the supported framework versions listed; verify that no .NET 5/6/7/8/9 target is shown.

2. Check whether any NuGet package re-exposes `LosFormatter` or a compatible shim type on .NET 5+ (search nuget.org for "LosFormatter" and "System.Web shim"); if a package exists confirm whether the analyzer diagnostic fires against the shim type or only against the original `System.Web.UI.LosFormatter` fully-qualified name.

3. Review the CA2305 analyzer source in the dotnet/roslyn-analyzers repository (src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerLosFormatter.cs) to determine whether the rule matches by fully-qualified type name, by interface, or by some other pattern — this determines whether a shim or duck-typed replacement could trigger it.

4. Attempt to add `Microsoft.AspNet.WebFormsDependencyInjection` or any other package that transitively brings in `System.Web` types to `CreateProjectBuilder`'s `packageReferences` parameter in the test harness; verify whether the package resolves on a `net9.0` target and whether the build compiles.

5. If no package path forward exists, update the `Untestable` reason in `UntestableRules.cs` to include the confirmed specific evidence, for example: "LosFormatter exists only in .NET Framework's System.Web.UI assembly (confirmed absent from .NET 5+ API browser and dotnet/runtime); no NuGet shim exposes the exact type matched by the CA2305 analyzer; permanently untestable on .NET 5+ targets".

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
