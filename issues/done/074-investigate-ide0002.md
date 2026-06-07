## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0002 ("Simplify member access") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "IDE-only; not emitted by build analysis"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the "IDE-only" claim by checking the Roslyn source for IDE0002: locate the `SimplifyMemberAccessDiagnosticAnalyzer` (or equivalent) in the dotnet/roslyn repository and verify whether it is registered under `GeneratedCodeAnalysisFlags` or restricted to the IDE layer via `IConfigurationFixProvider` / `IDEDiagnosticIds`. If the analyzer is only registered in the language-service pipeline (not the command-line build pipeline), this confirms IDE-only status.

2. Check whether setting `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` in the test project (via `CreateProjectBuilder` properties) causes IDE0002 to be emitted during `dotnet build`. This MSBuild property is the documented mechanism for running IDE code-style rules in CI builds. Write a minimal violation — e.g. `int x = System.Int32.MaxValue;` where `System.Int32` can be simplified to `int` — and observe whether a diagnostic appears in the SARIF output.

3. If `EnforceCodeStyleInBuild` does surface the diagnostic, verify that the `.editorconfig` severity setting for `dotnet_diagnostic.IDE0002.severity` is respected in the build output, and write a `[Fact]` test that exercises the violation pattern, then confirm it passes in CI.

4. If `EnforceCodeStyleInBuild` does not surface IDE0002 even with a clear simplifiable member-access expression, cross-reference the official Microsoft documentation at `https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0002` to confirm the rule is explicitly excluded from build analysis (look for the "Applies to" or "Build enforcement" note in the docs).

5. Record a definitive conclusion: if no build-time emission path exists, update the `Untestable` reason in `UntestableRules.cs` to cite the specific mechanism (e.g. "IDE-only; not emitted during `dotnet build` even with EnforceCodeStyleInBuild=true because the SimplifyMemberAccess analyzer is registered only in the IDE language-service layer").

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
