## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5370 ("Use XmlReader for XmlValidatingReader constructor") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "XmlValidatingReader is a .NET Framework 1.x type not available in .NET Core/5+; the type was removed from the cross-platform BCL"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the type is absent in .NET Core/5+: check the .NET API browser (https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlvalidatingreader) and verify that `System.Xml.XmlValidatingReader` carries a "This API is not CLS-compliant" or "not supported on this platform" annotation, or is simply absent from the `System.Xml` assembly shipping with .NET 5+.

2. Check the Roslyn analyzer source for CA5370 (https://github.com/dotnet/roslyn-analyzers) to identify exactly which type or method symbol the rule triggers on — confirm it is `System.Xml.XmlValidatingReader` and no alternative overload or subtype exists that would also fire the diagnostic.

3. Attempt to reference `System.Xml.XmlValidatingReader` in a minimal test project targeting `net9.0` (the harness target) via `CreateProjectBuilder` with no extra packages; record whether the compiler emits CS0246 (type not found) or whether the diagnostic fires, to determine if a test can even be compiled.

4. Check whether any NuGet compatibility shim (e.g. `Microsoft.Windows.Compatibility`) re-exposes `System.Xml.XmlValidatingReader` on .NET Core/5+ and, if so, whether adding it via the `packageReferences` parameter of `CreateProjectBuilder` allows a violation pattern to compile and the rule to fire.

5. If no shim exists or the type remains absent even with compatibility packages, document the confirmed reason — "XmlValidatingReader was deprecated in .NET Framework 2.0 and removed entirely from .NET Core / .NET 5+; no cross-platform shim restores the type, so CA5370 can never fire in a net9.0 test project" — and update the `UntestableRules.cs` entry accordingly.

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
