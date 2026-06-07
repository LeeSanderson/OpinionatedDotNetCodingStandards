## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0049 ("Use language keywords instead of framework type names") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseLanguageKeywordsInsteadOfFrameworkTypeNames` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0049; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0049", "Use language keywords instead of framework type names",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0049",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0049; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseLanguageKeywordsInsteadOfFrameworkTypeNames()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static System.String GetName() => "hello";
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0049").ShouldBeTrue();
    }
```

## Investigation plan

1. Try suppressing IDE0055 in the generated project by adding `dotnet_diagnostic.IDE0055.severity = none` to the editorconfig additionalFiles entry in `CreateProjectBuilder`, then re-run the test to see if IDE0049 surfaces on its own in the build SARIF output.

2. Check the Roslyn source (or the `Microsoft.CodeAnalysis.CSharp.CodeStyle` NuGet package metadata) for the `EnforceOnBuild` property of IDE0049 — confirm whether it is set to `Never`, `WhenExplicitlyEnabled`, or `Recommended`, as a value of `Never` would confirm the rule is permanently excluded from build enforcement regardless of editorconfig severity.

3. Try adding `#pragma warning disable IDE0055` immediately around the violating code in the test file and rebuild, to determine whether suppressing the formatter diagnostic allows the IDE0049 diagnostic ID to appear in the SARIF output instead.

4. Test the same violation pattern against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package reference in `CreateProjectBuilder`, to determine whether the formatter-backed behaviour is a regression introduced in a specific release or has always been present.

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
