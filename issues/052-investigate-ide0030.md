## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0030 ("Use coalesce expression") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseCoalesceExpressionForNullableValueType` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn the build-time analyzer does not fire IDE0030 for nullable value type coalesce patterns; IDE0055 fires instead and IDE0030 is absent from SARIF output even when IDE0055 is suppressed. IDE0031 has the same symptom"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0030", "Use coalesce expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0030",
        Untestable = "In .NET 10 Roslyn the build-time analyzer does not fire IDE0030 for nullable value type coalesce patterns; IDE0055 fires instead and IDE0030 is absent from SARIF output even when IDE0055 is suppressed. IDE0031 has the same symptom")]
    public async Task UseCoalesceExpressionForNullableValueType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int GetValue(int? x) => x != null ? x.Value : 0;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0030").ShouldBeTrue();
    }
```

## Investigation plan

1. Understand which rule fires and why: run the existing test code without the Skip, observe the exact SARIF/build output, and confirm that IDE0055 is the diagnostic that appears in place of IDE0030. Note the exact diagnostic message and the code location to establish the baseline substitution behaviour.

2. Find a pattern that triggers IDE0030 but not the subsuming IDE0055: consult the Roslyn source (`src/Analyzers/CSharp/Analyzers/UseCoalesceExpression/`) to understand the precise pattern that IDE0030 targets for nullable value types versus what IDE0055 covers. Try alternative violation patterns — for example using a local variable with explicit `.HasValue` check rather than a `!= null` comparison — to see if any pattern escapes IDE0055's reach.

3. Try suppressing IDE0055 in the generated project via the editorconfig: add `dotnet_diagnostic.IDE0055.severity = none` (or `suggestion`) to the test project's editorconfig via `additionalFiles` in `CreateProjectBuilder`, then rebuild and check whether IDE0030 surfaces in the SARIF output without IDE0055 noise.

4. Try a `#pragma warning disable IDE0055` guard around the violation in the test source file, then rebuild and inspect whether IDE0030 is reported independently of IDE0055.

5. Check the `EnforceOnBuild` metadata for IDE0030 in the Roslyn/NetAnalyzers source (look for `IDEDiagnosticIds.UseCoalesceExpressionForNullableId` and the associated descriptor's `customTags`). If `EnforceOnBuild` is absent or set to `Never`, IDE0030 is architecturally excluded from command-line builds and cannot appear in SARIF regardless of editorconfig settings — this would confirm permanent untestability.

6. Test on an older NetAnalyzers version (e.g. 8.x) by temporarily pinning the `Microsoft.CodeAnalysis.NetAnalyzers` package reference in the test harness to `8.*` and re-running the test to determine whether IDE0030 did fire in earlier SDK generations and was intentionally superseded by IDE0055 in .NET 10.

7. Check the Roslyn and dotnet/roslyn-analyzers changelogs and GitHub issues for any record of IDE0030 being intentionally subsumed by or merged into IDE0055 for nullable value type patterns. If a tracking issue or PR is found, record its URL as the sourced reason in the Untestable attribute.

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
