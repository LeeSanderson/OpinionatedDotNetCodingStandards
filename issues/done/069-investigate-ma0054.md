## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse MA0054 ("Embed the caught exception as innerException") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `EmbedCaughtExceptionAsInnerException` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "MA0054 does not fire in the build harness for any catch-and-rethrow pattern; the analyzer's data-flow conditions are not met by single-project builds"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("MA0054", "Embed the caught exception as innerException",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0054.md",
        Untestable = "MA0054 does not fire in the build harness for any catch-and-rethrow pattern; the analyzer's data-flow conditions are not met by single-project builds")]
    public async Task EmbedCaughtExceptionAsInnerException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System;
            namespace test;
            public class C
            {
                public void M()
                {
                    try { }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed");
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0054").ShouldBeTrue();
    }
```

## Investigation plan

1. Read the Meziantou.Analyzer source for MA0054 at `https://github.com/meziantou/Meziantou.Analyzer/blob/main/src/Meziantou.Analyzer/Rules/MA0054Analyzer.cs` to understand exactly which syntax patterns the analyzer matches (e.g. does it require the caught variable to be in scope at the `throw new …` site, does it inspect constructor arguments, does it use data-flow or purely syntactic matching).

2. Try a same-method, syntactically direct pattern where the caught variable `ex` is provably unused at the `throw` site — confirm the caught variable is in scope but not passed to the new exception constructor — to rule out any data-flow / interprocedural requirement and check whether a purely syntactic trigger already exists.

3. Try additional rethrow patterns that the analyzer documentation describes as violations: e.g. rethrowing inside a nested `catch` block, rethrowing after a `when` filter, and rethrowing a wrapped exception type whose constructor accepts an `innerException` parameter but is not passed one — to find a pattern the analyzer actually matches.

4. Check whether MA0054 is emitted as a warning or an error in the generated project by inspecting the severity configured in the `.editorconfig` produced by `CreateProjectBuilder`; if it is set to `none` or `suggestion`, the `HasError` assertion will never pass — try `HasDiagnostic` or `HasWarning` instead and observe whether the diagnostic appears at a lower severity.

5. Confirm that the `Meziantou.Analyzer` NuGet package version referenced in the test harness actually ships MA0054 (the rule was introduced in a specific release); cross-reference the package version in the project's `.csproj` or `global.json` against the Meziantou.Analyzer changelog to verify the rule is present.

6. Check the Meziantou.Analyzer GitHub issues and changelog for any known limitation of MA0054 in build-time (`EnforceOnBuild`) mode versus IDE mode — some Meziantou rules are intentionally disabled during build (`EnforceOnBuild = false`) and only surface as IDE suggestions; if MA0054 is one of those, document the GitHub issue or source location as the confirmed root cause.

7. If all patterns fail to produce a diagnostic, reproduce the failure locally with verbose MSBuild output (`/bl` or `-v diag`) to confirm that the analyzer assembly is loaded and MA0054's category/severity is not overridden by a ruleset or `.editorconfig` entry that silences it globally; update the `Untestable` reason with the specific source location or GitHub issue link confirming the root cause.

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
