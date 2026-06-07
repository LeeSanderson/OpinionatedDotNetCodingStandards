## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0031 ("Use null propagation") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseNullPropagation` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0031; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0031", "Use null propagation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0031",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0031; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseNullPropagation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string? GetUpper(string? s) => s != null ? s.ToUpper() : null;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0031").ShouldBeTrue();
    }
```

## Investigation plan

1. Try suppressing IDE0055 in the generated project by adding `dotnet_diagnostic.IDE0055.severity = none` to the editorconfig additionalFiles entry in `CreateProjectBuilder`, then re-run the test to see whether IDE0031 surfaces on its own once the formatter diagnostic is silenced.

2. Search the Roslyn source (or the `Microsoft.CodeAnalysis.CSharp.CodeStyle` NuGet package) for the `UseNullPropagationDiagnosticAnalyzer` and check its `EnforceOnBuild` metadata — confirm whether the analyzer opts in to build enforcement at all or delegates entirely to the formatter pipeline.

3. Add `#pragma warning disable IDE0055` inside the test violation file to suppress the formatter diagnostic at the source level, then check whether the build SARIF now contains an IDE0031 entry instead.

4. Test the same violation pattern against an older NetAnalyzers version (e.g. 8.x) by temporarily pinning the package reference in the test project, to determine whether the IDE0055-subsumption behaviour was introduced in a specific SDK/analyzer version or has always been the case.

5. Check the Roslyn GitHub repository (dotnet/roslyn) for issues or PRs referencing IDE0031 and `EnforceOnBuild`, and note whether there is a documented decision to route this rule through the formatter rather than the analyzer diagnostic pipeline.

6. Attempt an alternative violation pattern — for example a more complex ternary with a method call on the non-null branch — to rule out the possibility that the current code snippet is being rewritten by the formatter before the analyzer runs, and verify whether any pattern consistently produces IDE0031 in the build output.

7. If none of the above unblocks IDE0031, update the `Untestable` reason with the confirmed source: the specific `EnforceOnBuild` enum value (or its absence) found in the Roslyn source, plus a link to the relevant Roslyn issue or source file, so the reason is permanently well-sourced.

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
