## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2321 ("Do not deserialize with JavaScriptSerializer using a SimpleTypeResolver") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "JavaScriptSerializer is in System.Web.Script.Serialization (.NET Framework only); the type is not available in .NET Core/5+ and cannot be referenced in the test harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Web.Script.Serialization.JavaScriptSerializer` is absent from all .NET 5+ TFMs by checking the official .NET API browser (https://learn.microsoft.com/dotnet/api/system.web.script.serialization.javascriptserializer) and the dotnet/runtime repo for any compat shim or polyfill package.
2. Check whether any NuGet package (e.g. a community shim or a `Microsoft.AspNet.*` compatibility package) re-exposes `JavaScriptSerializer` and `SimpleTypeResolver` on .NET 5+; if found, verify the CA2321 analyzer source recognises the shim's fully-qualified type names (inspect the rule implementation at https://github.com/dotnet/roslyn-analyzers/blob/main/src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/).
3. Check the CA2321 analyzer source to confirm exactly which type symbols it checks (`JavaScriptSerializer` and `SimpleTypeResolver` by fully-qualified name). If the rule hardcodes `System.Web.Script.Serialization` namespace names, a shim package would need to expose exactly those names to fire the diagnostic.
4. Attempt a minimal `CreateProjectBuilder` test that adds any identified shim package via `packageReferences`, compiles a snippet that calls `new JavaScriptSerializer(new SimpleTypeResolver()).Deserialize<object>(input)`, and checks whether CA2321 appears in SARIF output; record the result.
5. If no viable shim or workaround is found in steps 1–4, confirm permanent untestability and update the `Untestable` reason in `UntestableRules.cs` to the specific confirmed reason (e.g. "type removed in .NET Core 1.0; System.Web.Script.Serialization is not available in any .NET 5+ TFM or compatible NuGet package; cannot be referenced in the test harness").

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
