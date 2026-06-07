## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE2002 ("Consecutive braces must not have blank line between them") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitBlankLineBetweenConsecutiveBraces` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2002; the blank-line-between-braces pattern also triggers CS0161 (not all code paths return a value) in methods with empty bodies"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE2002", "Consecutive braces must not have blank line between them",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2002",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2002; the blank-line-between-braces pattern also triggers CS0161 (not all code paths return a value) in methods with empty bodies")]
    public async Task ProhibitBlankLineBetweenConsecutiveBraces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void M()
                {
                    if (true)
                    {
                        return;

                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2002").ShouldBeTrue();
    }
```

## Investigation plan

1. Attempt to suppress IDE0055 in the generated project by adding a `dotnet_diagnostic.IDE0055.severity = none` entry to the editorconfig used by `CreateProjectBuilder`, then re-run the test to see whether IDE2002 surfaces on its own in the build SARIF output.

2. Add `#pragma warning disable IDE0055` directly inside the violation file around the blank-line-between-braces pattern and rebuild, checking whether IDE2002 now appears as a distinct diagnostic in the output.

3. Inspect the Roslyn/dotnet-roslyn source for IDE2002 (search `FormattingDiagnosticAnalyzer` or the `IDE2002` descriptor) and confirm the value of `EnforceOnBuild` on the rule — if it is `Never` or `WhenExplicitlyEnabled`, the diagnostic is intentionally suppressed in command-line builds regardless of editorconfig severity.

4. Try an alternative violation pattern that avoids the CS0161 compiler error: place the blank line between consecutive closing braces in a `void` method or a top-level `if` block that requires no return value, eliminating the compiler error as a confounding factor, then rebuild and inspect SARIF.

5. Test against an older NetAnalyzers version (e.g. 8.x) by temporarily downgrading the `Microsoft.CodeAnalysis.NetAnalyzers` package reference in the test harness project, to determine whether IDE2002 was ever emitted under its own diagnostic ID in build output or has always been subsumed by IDE0055.

6. Search the Roslyn GitHub repository issues and changelog for any explicit statement that IDE2002 (and related IDE20xx formatter rules) emit IDE0055 by design in MSBuild/CLI builds, and record the permalink to that source as the authoritative citation for the Untestable reason.

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
