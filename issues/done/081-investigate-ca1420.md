## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1420 ("Property, type, or attribute requires runtime marshalling") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Requires [assembly: DisableRuntimeMarshalling] which has assembly-wide impact on all string and reference-type P/Invoke marshalling; cannot be applied in isolation in a single-project test without breaking unrelated code"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if a separate isolated project file could be used just for the marshalling test, without affecting the rest of the harness. Specifically, verify whether `CreateProjectBuilder` (or its overloads) supports creating a standalone project that applies `[assembly: DisableRuntimeMarshalling]` without contaminating the shared test project context.
- Read the CA1420 analyzer source (in dotnet/roslyn-analyzers on GitHub) to confirm exactly which code patterns trigger it: determine whether the rule fires when a P/Invoke method uses `string` or a reference type as a parameter/return type while `DisableRuntimeMarshalling` is in effect, and whether any subset of that pattern (e.g. only value-type structs with marshalling attributes) can fire the rule without the assembly-level attribute.
- Attempt a minimal reproduction: create a `CreateProjectBuilder` call that adds `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` and an `[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]` attribute inside the source snippet (not via an assembly-level project attribute), then write a P/Invoke that returns a `string`, and observe whether the diagnostic fires.
- If the attribute cannot be placed inline in source and must be a true assembly-level attribute, check whether the test harness's project builder exposes a `properties` or `globalAttributes` hook that injects the attribute into the compilation without sharing it across other test compilations.
- If no isolation path is found, document the permanently untestable status with a precise reason: confirm that `DisableRuntimeMarshalling` is an assembly-level-only attribute (not applicable per-method or per-file), cite the official .NET 7 runtime marshalling documentation or the roslyn-analyzers source, and update the untestable reason in `UntestableRules.cs` accordingly.

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
