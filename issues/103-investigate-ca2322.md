## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2322 ("Ensure JavaScriptSerializer is not initialized with SimpleTypeResolver before deserializing") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant of CA2321; additionally requires taint analysis tracking of the resolver; same JavaScriptSerializer unavailability as CA2321"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Web.Script.Serialization.JavaScriptSerializer` and `System.Web.Script.Serialization.SimpleTypeResolver` are absent from the .NET 5+ BCL by checking the Microsoft reference source and/or the dotnet/runtime GitHub repository. The types live in `System.Web.Extensions.dll` (.NET Framework only) — verify no .NET 5+ TFM ships an equivalent in any in-box assembly.

2. Check whether any NuGet package re-exposes `JavaScriptSerializer` for .NET 5+ (search nuget.org for `System.Web.Extensions` or `Microsoft.AspNet.WebPages` compatible with `net8.0`). If a stable package exists, test whether `CreateProjectBuilder`'s `packageReferences` parameter can add it and whether the analyzer fires on a minimal violation snippet: create a `JavaScriptSerializer` with a `SimpleTypeResolver` in one statement and call `Deserialize` in a second statement (the taint-tracking pattern the rule requires).

3. Verify the taint-tracking requirement independently of the type-availability blocker: read the CA2322 analyzer source in the `dotnet/roslyn-analyzers` repository (file `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerJavaScriptSerializerWithSimpleTypeResolver.cs`) to confirm whether the rule is a straightforward syntactic check or a true inter-statement data-flow analysis. If inter-statement, confirm whether the test harness's single-file compilation model supports data-flow diagnostics (other data-flow rules such as CA2100 are tested in the harness, so check those tests for precedent).

4. If both blockers appear to be real (type unavailable and no compatible NuGet package), attempt one fallback: write a test that uses `#if NETFRAMEWORK` gating or targets `net48` in `CreateProjectBuilder`. Check whether `CreateProjectBuilder` supports a `targetFramework` override parameter and whether the CI agent has a .NET Framework 4.8 SDK installed. If net48 targeting is possible, write the minimal violation and run the test locally.

5. If all paths above are exhausted without a working violation pattern, update the `Untestable` reason in `UntestableRules.cs` to be more precise, for example: "JavaScriptSerializer and SimpleTypeResolver are in System.Web.Extensions (.NET Framework only); the types do not exist in any .NET 5+ in-box assembly or compatible NuGet package, making it impossible to compile a violation in the net8.0 test harness; additionally CA2322 requires inter-statement taint-flow analysis, which would require a multi-statement test fixture even if the types were available."

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
