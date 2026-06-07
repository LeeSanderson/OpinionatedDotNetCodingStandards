## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1824 ("Mark assemblies with NeutralResourcesLanguageAttribute") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires only when an assembly contains embedded .resx resources but lacks NeutralResourcesLanguageAttribute; the test harness does not support adding EmbeddedResource items to the csproj"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Read the CA1824 analyzer source (in `dotnet/roslyn-analyzers`) to confirm whether the rule triggers on the presence of any `ResourceManager` instantiation in compiled IL, or strictly requires a physical `.resx` file listed as `<EmbeddedResource>` in the `.csproj`; if it triggers on `ResourceManager` usage alone, a hand-written violation pattern may be possible without any `.resx` file.
2. Check whether `CreateProjectBuilder` (in the test harness) exposes a way to inject arbitrary MSBuild item groups (e.g. `additionalFiles`, `properties`, or a raw XML fragment) that would allow adding an `<EmbeddedResource>` item pointing to a minimal `.resx` file placed in the temp project directory alongside the `.cs` source.
3. Attempt a minimal reproduction: create a temp `.resx` file (valid XML, single string entry) in the same directory as the generated `.cs` source and add it as `<EmbeddedResource>` via whatever hook `CreateProjectBuilder` provides; build and check whether CA1824 fires in the SARIF output when `NeutralResourcesLanguageAttribute` is absent.
4. If step 3 succeeds, write a `[Fact]` test method with a `[RuleDoc]` attribute that uses this pattern and verify it passes in CI; remove the class-level entry from `UntestableRules.cs`.
5. If neither the `ResourceManager`-only pattern (step 1) nor the injected `.resx` approach (step 3) fires CA1824, confirm permanent untestability and update the `Untestable` reason in `UntestableRules.cs` to the specific confirmed reason (e.g. "CA1824 requires a physical EmbeddedResource .resx item in the compiled assembly; the test harness generates a single .cs file with no mechanism to inject additional MSBuild item groups").

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
