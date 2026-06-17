## Parent PRD

`issues/prd.md`

## What to build

Promote S6664 ("The code block contains too many logging calls") from `UntestableRules.cs` to a verified passing test in `SonarAnalyzerRulesNewShould`.

The rule requires a supported logging library to be referenced (`Microsoft.Extensions.Logging.Abstractions`). The Warning-category threshold is 1, so two `logger.LogWarning(...)` calls in the same code block is the minimal trigger. `LegacyIsRegisteredActionEnabled` returns `true` during `dotnet build` (the `ShouldExecuteRegisteredAction` delegate is null), so no special Sonar build context is needed. The editorconfig already sets `dotnet_diagnostic.S6664.severity = warning`.

The prior-art test to follow is `SonarAnalyzerRulesNewShould.LoggingInCatchShouldPassCaughtException` (same file, same package reference, same ILogger usage structure).

See PRD §S6664 for the full rationale and §Testing Decisions for the prior-art shape to follow.

## Acceptance criteria

- [ ] A new test method exists in `tests/.../SonarAnalyzerRules/SonarAnalyzerRulesNewShould.cs` decorated with `[RuleDoc("S6664", "The code block contains too many logging calls", HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6664/")]`
- [ ] The test calls `CreateProjectBuilderAsync` with `packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]`
- [ ] The sample file uses `Microsoft.Extensions.Logging.ILogger` and contains exactly two `logger.LogWarning(...)` calls in the same code block
- [ ] `buildOutput.HasError("S6664").ShouldBeTrue()` passes
- [ ] The `[RuleDoc("S6664", ...)]` entry is removed from `tests/.../UntestableRules.cs`
- [ ] The full test suite passes (no regressions)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 4 (S6664 verified by the test suite)
- User story 5 (UntestableRules.cs only contains structurally untestable rules)
- User story 6 (newly promoted rule carries a `[RuleDoc]` attribute)
