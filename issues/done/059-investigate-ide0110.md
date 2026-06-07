## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0110 ("Remove unnecessary discard") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RemoveUnnecessaryDiscardPattern` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0110; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0110", "Remove unnecessary discard",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0110",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0110; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task RemoveUnnecessaryDiscardPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsAny(object? o) => o is _;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0110").ShouldBeTrue();
    }
```

## Investigation plan

1. Attempt to suppress IDE0055 in the generated project by adding an editorconfig additionalFiles entry (`dotnet_diagnostic.IDE0055.severity = none`) and re-run the build, to determine whether IDE0110 then appears in the SARIF output independently.
2. Check the Roslyn source (or the `Microsoft.CodeAnalysis.CSharp.CodeStyle` NuGet package metadata) for the `EnforceOnBuild` attribute on the IDE0110 diagnostic descriptor to confirm whether the rule is intentionally excluded from command-line build enforcement.
3. Try adding `#pragma warning disable IDE0055` around the violation pattern in the test file to see if suppressing the formatter diagnostic allows IDE0110 to surface on its own.
4. Test with an older NetAnalyzers / Roslyn CodeStyle version (e.g. .NET 8 SDK) by temporarily changing the target framework in `CreateProjectBuilder`, to determine whether the formatter-backed behaviour was introduced in a specific SDK release.
5. Try alternative violation patterns for IDE0110 — for example, a standalone expression-statement discard (`_ = Foo();` where the return value is not needed, or `_ = default;`) — to determine whether the `is _` pattern is specifically routed through the formatter while other patterns emit IDE0110 directly.
6. Search the Roslyn GitHub repository for the IDE0110 diagnostic registration and any linked issues or PRs that discuss `EnforceOnBuild` being set to `Never` or `WhenExplicitlyEnabled`, and record the source location or issue URL as the confirmed root cause if found.

## Acceptance criteria

- [ ] Root cause identified and documented
- [ ] One of:
  - [ ] A working violation pattern found → test updated, Skip removed, test passes in CI; OR
  - [ ] Confirmed permanently untestable → Untestable reason updated with the specific root cause (source location or GitHub issue link)
- [ ] No regressions in other tests in the same test file
- [ ] If the test is promoted, RuleReferenceGenerator coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
