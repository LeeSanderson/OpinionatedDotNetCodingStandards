## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse RS0035 ("External access to internal symbols outside the restricted namespace(s) is prohibited") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Requires cross-assembly setup with RestrictedInternalsVisibleToAttribute; not triggerable from a single-project build"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Read the Roslyn source for RS0035 (in `Microsoft.CodeAnalysis.BannedApiAnalyzers`) to confirm that the diagnostic requires the consuming assembly to reference a second assembly that has `[assembly: RestrictedInternalsVisibleTo(...)]` applied to it; verify there is no single-assembly code pattern that triggers the rule.
- Check whether `CreateProjectBuilder` supports multi-project or referenced-assembly scenarios (e.g. via `additionalReferences` or an `AdditionalProjects` parameter), or whether a second in-memory compilation could be passed as a metadata reference to make the cross-assembly precondition reachable.
- Search the Roslyn / BannedApiAnalyzers test suite on GitHub for any existing single-project test fixtures for RS0035; if found, determine whether the same pattern is portable to the harness's `CreateProjectBuilder` API.
- Attempt a minimal two-file workaround: define an internal type in a helper source file that the test project compiles, annotate its containing assembly with `RestrictedInternalsVisibleToAttribute`, and check whether the analyzer fires when the type is accessed from outside the restricted namespace within the same compilation.
- If no single-compilation path exists, document the confirmed reason: RS0035 is implemented as a cross-assembly taint check that reads `RestrictedInternalsVisibleToAttribute` from a referenced assembly's metadata; it cannot fire when both the defining and consuming code live in the same compilation unit, making it permanently untestable in the single-project harness.

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
