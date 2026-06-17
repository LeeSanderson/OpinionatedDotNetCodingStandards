## Parent PRD

`issues/prd.md`

## What to build

Promote S6798 ("[JSInvokable] attribute should only be used on public methods") from `UntestableRules.cs` to a verified passing test in `SonarAnalyzerRulesNewShould`.

The SonarAnalyzer resolves `JSInvokableAttribute` via `KnownType.Microsoft_JSInterop_JSInvokable` (string-based lookup, no assembly-identity check), so the attribute can be defined as a stub directly in the sample file. No special project properties are needed.

See PRD §S6798 for the full rationale and §Testing Decisions for the prior-art shape to follow.

## Acceptance criteria

- [ ] A new test method exists in `tests/.../SonarAnalyzerRules/SonarAnalyzerRulesNewShould.cs` decorated with `[RuleDoc("S6798", "[JSInvokable] attribute should only be used on public methods", HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6798/")]`
- [ ] The test calls `CreateProjectBuilderAsync` with no special properties
- [ ] The sample file defines a stub `JSInvokableAttribute : Attribute` class in the `Microsoft.JSInterop` namespace
- [ ] The sample file contains a non-public (`private` or `internal`) method decorated with `[Microsoft.JSInterop.JSInvokable]`
- [ ] `buildOutput.HasError("S6798").ShouldBeTrue()` passes
- [ ] The `[RuleDoc("S6798", ...)]` entry is removed from `tests/.../UntestableRules.cs`
- [ ] The full test suite passes (no regressions)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2 (S6798 verified by the test suite)
- User story 5 (UntestableRules.cs only contains structurally untestable rules)
- User story 6 (newly promoted rule carries a `[RuleDoc]` attribute)
- User story 7 (stub-attribute technique demonstrated for future JSInterop rules)
