## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE2006 ("Blank line not allowed after arrow expression clause token") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitBlankLineAfterArrowExpressionClauseToken` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2006; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE2006", "Blank line not allowed after arrow expression clause token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2006",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2006; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task ProhibitBlankLineAfterArrowExpressionClauseToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() =>

                    0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2006").ShouldBeTrue();
    }
```

## Investigation plan

1. Attempt to suppress IDE0055 in the generated project by adding `dotnet_diagnostic.IDE0055.severity = none` as an additional editorconfig entry in `CreateProjectBuilder`, then rebuild and check whether IDE2006 surfaces directly in the SARIF output instead.
2. Check the Roslyn source for IDE2006 (search `FormattingDiagnosticIds` and the `BlankLineAfterArrowExpressionClause` rule class) to confirm whether the rule sets `EnforceOnBuild = true` or delegates entirely through the formatter pipeline, and record the source file URL or commit reference.
3. Try suppressing IDE0055 inline with `#pragma warning disable IDE0055` around the violation in the test file and verify whether this causes IDE2006 to be emitted independently or whether the diagnostic simply disappears.
4. Test on an older NetAnalyzers version (e.g., pin `Microsoft.CodeAnalysis.NetAnalyzers` to 8.x in `CreateProjectBuilder`) to determine whether IDE2006 was ever emitted as its own diagnostic ID prior to the formatter unification change.
5. Search the Roslyn GitHub repository issues and changelog for any record of IDE2006 being intentionally merged into IDE0055 enforcement, and capture a direct link to confirm the design decision.
6. If IDE2006 still does not appear after the above steps, confirm the root cause by checking whether `csharp_style_allow_blank_line_after_arrow_expression_clause` severity set to `error` in the editorconfig causes the build to report IDE2006 or only IDE0055, using `dotnet build --no-incremental` with `-p:EnforceCodeStyleInBuild=true`.
7. Based on findings, either remove `[Fact(Skip = "untestable")]` and update the test with the working pattern, or update the `Untestable` string with a precise root-cause citation (Roslyn source path or GitHub issue URL).

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
