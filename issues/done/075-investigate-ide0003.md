## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0003 ("Remove this or Me qualification") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "IDE-only; not emitted by build analysis"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the "IDE-only" claim by checking the Roslyn source for IDE0003: look at the `IDEDiagnosticIds` list and the `DiagnosticAnalyzer` registration to verify whether the analyzer is gated behind `IDEDiagnosticResultKind.IfGeneratedAndSupportedByGeneratedCodeAnalysis` or similar IDE-only activation, and whether `dotnet build` / `dotnet msbuild` ever invokes it.
2. Search for any `EnforceCodeStyleInBuild` MSBuild property support for IDE0003: check if setting `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` in the test project (via `CreateProjectBuilder` properties) causes IDE0003 to emit a diagnostic during a build-time analysis pass, as Microsoft added build-time IDE style enforcement in .NET 6+.
3. Write a minimal probe test that enables `EnforceCodeStyleInBuild` and provides a C# snippet using `this.` qualification on a field access (e.g. `this.myField = 0;` inside an instance method) and check whether the SARIF output contains an IDE0003 diagnostic — if yes, a working violation pattern exists.
4. Check the `.editorconfig` option that governs IDE0003 (`dotnet_style_qualification_for_field`, `dotnet_style_qualification_for_property`, etc.) and verify whether those options are already set in the project's `.editorconfig`; confirm the severity is `warning` or `error` (not `none` or `silent`), because a `silent` severity suppresses build-time emission even when `EnforceCodeStyleInBuild` is on.
5. If steps 2–4 show that IDE0003 still does not emit during build even with `EnforceCodeStyleInBuild=true`, locate the Roslyn GitHub issue or documentation that explicitly classifies IDE0003 as a "refactoring-only" or "hidden-severity" rule excluded from build analysis, and record that URL as the permanent untestable confirmation source.

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
