## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5374 ("Do Not Use XslTransform") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "XslTransform is a .NET Framework 1.x type not available in .NET Core/5+; the type was removed from the cross-platform BCL"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the type is absent: attempt to reference `System.Xml.Xsl.XslTransform` in a .NET 8/9 target project and verify that the compiler reports CS0234 or CS0246, confirming the type does not exist in any in-box assembly.
2. Check the analyzer source: read the CA5374 implementation in the `dotnet/roslyn-analyzers` GitHub repository (SecurityRules/DoNotUseXslTransform) to identify exactly which type symbol(s) the rule checks for — confirm it only matches `System.Xml.Xsl.XslTransform` and not any successor type.
3. Check for a compatible replacement: determine whether `System.Xml.Xsl.XslCompiledTransform` (the recommended .NET Core replacement) triggers CA5374 by reviewing the analyzer source; if it does not, document why the replacement cannot serve as a test vehicle.
4. Check NuGet for a compatibility shim: search nuget.org for packages that re-expose `System.Xml.Xsl.XslTransform` on .NET 5+ (e.g. `System.Web` compat packages or third-party shims); if a stable package exists, test whether adding it via `CreateProjectBuilder`'s `packageReferences` parameter makes the rule fire.
5. If no path forward exists: update the `Untestable` reason in `UntestableRules.cs` to the specific confirmed reason (e.g. "XslTransform was removed in .NET Core 1.0; no in-box or compat-shim type exists in .NET 5+ that triggers CA5374, confirmed by analyzer source review").

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
