## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2311 ("Do not deserialize without first setting NetDataContractSerializer.Binder") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "NetDataContractSerializer was removed from .NET Core; the type does not exist in .NET 5+ BCL (same unavailability as CA2310)"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm `NetDataContractSerializer` is absent from the .NET 5+ BCL by checking the official dotnet/runtime repository or the .NET API browser (https://learn.microsoft.com/en-us/dotnet/api/) — verify there is no `System.Runtime.Serialization.NetDataContractSerializer` entry for any .NET 5+ target.
2. Check whether a NuGet package re-ships `NetDataContractSerializer` for .NET Core (e.g. search nuget.org for "NetDataContractSerializer"); if one exists, verify whether it carries a stable version and whether the CA2311 analyzer recognises calls through that package's type rather than the BCL type.
3. Review the CA2311 analyzer source in the dotnet/roslyn-analyzers repository (src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerNetDataContractSerializerMethods.cs) to confirm the rule keys off `System.Runtime.Serialization.NetDataContractSerializer` by fully-qualified name — if it does, a re-shipped NuGet type with a different identity would not trigger the rule.
4. Check whether `CreateProjectBuilder`'s `packageReferences` parameter could add any compatible package; if the type identity does not match what the analyzer expects, document that adding a package cannot help.
5. If no compatible type or workaround is found after steps 1–4, confirm the rule as permanently untestable and update the `Untestable` reason in `UntestableRules.cs` to include the specific .NET Core removal version and the dotnet/runtime issue or announcement as a source (e.g. "removed in .NET Core 1.0; no NuGet re-ship recognised by the analyzer; see https://github.com/dotnet/runtime/issues/…").

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
