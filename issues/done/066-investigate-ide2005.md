## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE2005 ("Blank line not allowed after conditional expression token") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitBlankLineAfterConditionalExpressionToken` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2005; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE2005", "Blank line not allowed after conditional expression token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2005",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2005; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task ProhibitBlankLineAfterConditionalExpressionToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                static bool Cond => true;
                public static int Main() => Cond ?

                    1 : 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2005").ShouldBeTrue();
    }
```

## Investigation plan

1. Attempt to suppress IDE0055 in the generated project by adding a `dotnet_diagnostic.IDE0055.severity = none` entry to the editorconfig via `additionalFiles` in `CreateProjectBuilder`, then re-run the test to see whether IDE2005 surfaces on its own or disappears entirely.
2. Check the Roslyn source (or NuGet package metadata) for the `EnforceOnBuild` property of IDE2005 — formatter-style rules often set `EnforceOnBuild = false` or `EnforceOnBuildRecommendedDefault = false`, which would prevent the diagnostic from appearing in MSBuild output regardless of editorconfig settings.
3. Add `#pragma warning disable IDE0055` directly inside `Program.cs` in the test file and rebuild; if the build output then contains IDE2005 the suppression approach is viable and the test can be promoted.
4. Test against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily changing the package reference in `CreateProjectBuilder` to confirm whether IDE2005 ever emitted its own diagnostic ID in build output or has always tunnelled through IDE0055.
5. Inspect the raw MSBuild/SARIF output from `project.BuildAndGetOutput()` for any diagnostic ID that co-fires with the blank-line violation (e.g. IDE0055 with a message referencing IDE2005) and determine whether `HasError` can be adjusted to match the surfaced ID or message text instead.
6. Confirm the `dotnet_style_allow_multiple_blank_lines_experimentalfeature` / `csharp_style_allow_blank_lines_between_consecutive_braces_experimentalfeature` editorconfig keys are present and correctly set in the generated `.editorconfig`; a missing key means the formatter never flags the pattern and no diagnostic of any ID is emitted.
7. If all approaches above confirm the rule cannot emit IDE2005 directly in build output, update the `Untestable` reason in the `[RuleDoc]` attribute with the specific Roslyn source location or GitHub issue link that documents the formatter-tunnel behaviour, and close the investigation.

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
