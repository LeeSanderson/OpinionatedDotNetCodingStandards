## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2312 ("Ensure NetDataContractSerializer.Binder is set before deserializing") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant of CA2311; additionally requires taint analysis across statements; same NetDataContractSerializer unavailability as CA2310"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. **Confirm type unavailability.** Verify that `NetDataContractSerializer` is absent from .NET 5+ by checking the Microsoft docs and/or the dotnet/runtime repository. Confirm that the type lived in `System.Runtime.Serialization` on .NET Framework and was not ported to .NET Core. Cross-reference with the CA2310 entry in `UntestableRules.cs`, which already documents the same unavailability.

2. **Search for a compatible replacement type.** Check whether any .NET 5+ BCL type or NuGet package re-exposes `NetDataContractSerializer` (e.g. a compatibility shim or a `System.Runtime.Serialization.NetDataContractSerializer` NuGet package). If such a package exists on nuget.org with a stable version, check whether `CreateProjectBuilder`'s `packageReferences` parameter could be used to add it and whether the analyzer still fires against the shim type.

3. **Inspect the Roslyn analyzer source for CA2312.** Locate the CA2312 implementation in the `dotnet/roslyn-analyzers` repository (search for `CA2312` or `NetDataContractSerializerBinderCheck`). Determine exactly which type symbols the analyzer pattern-matches against. If it matches only the original `System.Runtime.Serialization.NetDataContractSerializer` fully-qualified name, a shim with a different assembly identity will not trigger it.

4. **Assess the taint-analysis requirement.** CA2312 is documented as a data-flow/taint-analysis variant of CA2311, requiring the analyzer to track the `Binder` property assignment across multiple statements. Confirm whether the test harness (Roslyn compiler invocation with analyzers) supports inter-statement data-flow diagnostics, or whether this class of rule requires a full IOperation graph that the single-file harness does not provide.

5. **Document the confirmed reason.** If steps 1–4 confirm that (a) `NetDataContractSerializer` is not available in .NET 5+ and (b) no NuGet shim that the analyzer recognises exists, update the `Untestable` reason in `UntestableRules.cs` to the specific confirmed wording (e.g. "NetDataContractSerializer was removed in .NET Core 1.0 and the type is absent from the .NET 5+ BCL; no compatible NuGet shim exists that the CA2312 analyzer pattern-matches against; permanently untestable in the build harness").

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
