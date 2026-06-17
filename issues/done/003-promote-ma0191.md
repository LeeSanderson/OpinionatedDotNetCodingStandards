## Parent PRD

`issues/prd.md`

## What to build

Promote MA0191 ("Do not use the null-forgiving operator") from `UntestableRules.cs` to a verified passing test in `MeziantouAnalyzers3Should`.

The rule specifically targets `null!` or `default!` on the right-hand side of an assignment. Previous wrong-probe tests used general nullable-suppression patterns (`value!`, `_value!.Length`) which the rule does not target. The editorconfig already sets `dotnet_diagnostic.MA0191.severity = warning`; no additional opt-in is required.

See PRD §MA0191 for the full rationale and §Testing Decisions for the prior-art shape to follow.

## Acceptance criteria

- [ ] A new test method exists in `tests/.../MeziantouAnalyzers/MeziantouAnalyzers3Should.cs` decorated with `[RuleDoc("MA0191", "Do not use the null-forgiving operator", HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0191.md")]`
- [ ] The test calls `CreateProjectBuilderAsync` with no special properties
- [ ] The sample file contains `string x = null!;` or `string field = default!;` (an assignment where the RHS is `null!` or `default!`)
- [ ] `buildOutput.HasError("MA0191").ShouldBeTrue()` passes
- [ ] The `[RuleDoc("MA0191", ...)]` entry is removed from `tests/.../UntestableRules.cs`
- [ ] The full test suite passes (no regressions)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 3 (MA0191 verified by the test suite)
- User story 5 (UntestableRules.cs only contains structurally untestable rules)
- User story 6 (newly promoted rule carries a `[RuleDoc]` attribute)
