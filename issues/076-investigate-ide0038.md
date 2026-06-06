## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0038 ("Use pattern matching to avoid is check followed by a cast") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "IDE-only; not emitted by build analysis"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the "IDE-only" claim by checking the Roslyn source for IDE0038: locate the diagnostic descriptor in the `dotnet/roslyn` repository and verify whether its `customTags` includes `WellKnownDiagnosticTags.NotConfigurable` or the `"Telemetry"` tag, which are markers that suppress emission during command-line/MSBuild builds.

2. Check the `.editorconfig` severity setting: verify that setting `dotnet_diagnostic.IDE0038.severity = warning` (or `error`) in the test project's `.editorconfig` does not cause the diagnostic to appear in SARIF output when the harness runs `dotnet build` — compare against IDE rules that are known to emit (e.g. IDE0005) to establish a baseline for what "IDE-only" means in practice.

3. Attempt a minimal violation pattern in the test harness: write a test method using `CreateProjectBuilder` with a source file containing `if (obj is Foo) { var x = (Foo)obj; }` and confirm whether the build produces a IDE0038 diagnostic or silently passes; record the exact SARIF/build output either way.

4. Check whether `EnforceCodeStyleInBuild` changes behaviour: the MSBuild property `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` is required for many IDE rules to emit during builds; verify whether `CreateProjectBuilder` already sets this property, and if not, test whether adding it via the `properties` parameter causes IDE0038 to fire.

5. Search the Roslyn GitHub issues and the Microsoft docs changelog for IDE0038 to find any explicit statement that it is or is not emitted under `EnforceCodeStyleInBuild`; cross-reference with similar pattern-matching rules (IDE0020, IDE0078) that share the same analyser infrastructure to determine whether the IDE-only classification is permanent or a documentation gap.

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
