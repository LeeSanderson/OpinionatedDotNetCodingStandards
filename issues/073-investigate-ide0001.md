## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0001 ("Simplify name") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "IDE-only; not emitted by build analysis"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

The failure mode is "IDE-only; not emitted by build analysis". IDE style rules such as IDE0001 are implemented in the Roslyn IDE layer and historically were not surfaced during `dotnet build`. The steps below verify whether that has changed in recent SDK/analyzer versions and whether any configuration can force emission.

1. **Check the Roslyn source for IDE0001's DiagnosticDescriptor** — read the `SimplifyNameDiagnosticAnalyzer` source in the Roslyn repository to confirm whether the rule's descriptor carries `customTags: WellKnownDiagnosticTags.Unnecessary` and/or is marked `isEnabledByDefault: false` outside the IDE host. This determines whether the rule can ever appear in SARIF output from `dotnet build`.

2. **Check the .editorconfig `EnforceCodeStyleInBuild` switch** — verify whether setting `dotnet_analyzer_diagnostic.category-Style.severity = warning` and/or `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` in the test harness's csproj causes IDE0001 to appear in build output. Write a minimal single-file reproduction that uses a fully-qualified name (e.g. `System.Collections.Generic.List<int>`) where the `using` directive already makes the short form available, and run `dotnet build` with `EnforceCodeStyleInBuild` enabled.

3. **Check the Microsoft documentation and SDK release notes** — read the IDE0001 docs page (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0001) and the .NET SDK changelog for versions 6–9 to confirm whether IDE0001 is listed under rules that are emitted by `dotnet build` with `EnforceCodeStyleInBuild=true`, or whether it is explicitly excluded from command-line analysis.

4. **Try adding `EnforceCodeStyleInBuild` to `CreateProjectBuilder`** — check whether the test harness's `CreateProjectBuilder` helper accepts a `properties` parameter that can pass `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>`, and if so, write a trial test that sets this property alongside the IDE0001 violation pattern from step 2 and observe whether the SARIF output contains the diagnostic.

5. **Document the finding** — if steps 2–4 confirm the rule fires, write a `[Fact]` test with the violation pattern and remove the class-level entry from `UntestableRules.cs`. If all evidence confirms the rule is suppressed by the Roslyn build host regardless of configuration, update the `Untestable` reason string in `UntestableRules.cs` with the specific confirmed reason (e.g. "IDE-only; `EnforceCodeStyleInBuild=true` does not surface IDE0001 in MSBuild SARIF output because the simplification analyzers are excluded from the command-line analyzer set").

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
