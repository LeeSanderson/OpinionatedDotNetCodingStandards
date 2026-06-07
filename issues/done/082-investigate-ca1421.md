## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1421 ("This method uses runtime marshalling even when the DisableRuntimeMarshallingAttribute is applied") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Requires [assembly: DisableRuntimeMarshalling] which has assembly-wide impact on all string and reference-type P/Invoke marshalling; cannot be applied in isolation in a single-project test without breaking unrelated code"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Read the CA1421 analyzer source (or its documentation at https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1421) to confirm exactly which code patterns trigger the diagnostic — specifically whether the attribute must be at assembly scope or whether a method-level or type-level scope is sufficient.

2. Check whether a separate isolated project file (created via `CreateProjectBuilder` with its own set of source files) could host only the P/Invoke declaration and the `[assembly: DisableRuntimeMarshalling]` attribute, with no string or reference-type marshalling in any other P/Invoke in that file, so the assembly-wide impact does not interfere with the rest of the harness.

3. Attempt to write a minimal self-contained violation: a single `.cs` source file containing only an `extern` P/Invoke declaration that uses a non-blittable parameter type (e.g. `string`) together with `[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]`, passed to `CreateProjectBuilder` as the sole source, and verify whether the diagnostic fires.

4. If step 3 succeeds, check whether the test harness's `CreateProjectBuilder` method supports passing `additionalFiles` or an equivalent mechanism to keep the marshalling-attribute source isolated from the shared project source, and confirm the test passes without breaking any other test in the suite.

5. If no isolation path exists, confirm permanence: cross-reference the .NET runtime specification for `DisableRuntimeMarshallingAttribute` (https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.disableruntimemarshallingattribute) to verify that assembly-scope is the only supported target and that no per-method or per-type override exists, then update the untestable reason in `UntestableRules.cs` with the confirmed citation.

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
