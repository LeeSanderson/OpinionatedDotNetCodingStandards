## Parent PRD

`issues/prd.md`

## What to build

Promote MA0187 ("Use constructor injection instead of [Inject] attribute") from `UntestableRules.cs` to a verified passing test in `MeziantouAnalyzers3Should`.

The analyzer resolves `InjectAttribute` and `IComponent` via pure metadata-name lookup (`GetBestTypeByMetadataName`) — no assembly-identity check — so both types can be defined as stubs directly in the sample file. The test project's `AssemblyVersion` must be set to `9.0.0.0` to satisfy the analyzer's version guard (`ContainingAssembly.Identity.Version >= 9.0.0.0`), since the stub attribute lives in the test project.

See PRD §MA0187 for the full rationale and §Testing Decisions for the prior-art shape to follow.

## Acceptance criteria

- [ ] A new test method exists in `tests/.../MeziantouAnalyzers/MeziantouAnalyzers3Should.cs` decorated with `[RuleDoc("MA0187", "Use constructor injection instead of [Inject] attribute", HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0187.md")]`
- [ ] The test calls `CreateProjectBuilderAsync` with `properties: [(Name: "AssemblyVersion", Value: "9.0.0.0")]`
- [ ] The sample file defines a stub `IComponent` interface and a stub `InjectAttribute : Attribute` class, both in the `Microsoft.AspNetCore.Components` namespace
- [ ] The sample file contains a class implementing `IComponent` with a property decorated with `[Microsoft.AspNetCore.Components.Inject]`
- [ ] `buildOutput.HasError("MA0187").ShouldBeTrue()` passes
- [ ] The `[RuleDoc("MA0187", ...)]` entry is removed from `tests/.../UntestableRules.cs`
- [ ] The full test suite passes (no regressions)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 1 (MA0187 verified by the test suite)
- User story 5 (UntestableRules.cs only contains structurally untestable rules)
- User story 6 (newly promoted rule carries a `[RuleDoc]` attribute)
- User story 7 (stub-attribute technique demonstrated for future Blazor-framework rules)
