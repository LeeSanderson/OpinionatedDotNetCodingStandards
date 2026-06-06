## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse MA0023 ("Add RegexOptions.ExplicitCapture") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RequireExplicitCaptureInRegex` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "SYSLIB1045 fires for all runtime Regex construction and appears to suppress MA0023 in Meziantou.Analyzer 2.0.286"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("MA0023", "Add RegexOptions.ExplicitCapture",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0023.md",
        Untestable = "SYSLIB1045 fires for all runtime Regex construction and appears to suppress MA0023 in Meziantou.Analyzer 2.0.286")]
    public async Task RequireExplicitCaptureInRegex()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Text.RegularExpressions;
            namespace test;
            public class C
            {
                public bool M(string input) => new Regex("(foo)bar").IsMatch(input);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0023").ShouldBeTrue();
    }
```

## Investigation plan

1. Add `#pragma warning disable SYSLIB1045` around the `new Regex(...)` call in the test's violation pattern and rebuild, then check whether MA0023 now appears in the build output — this directly tests the hypothesis that SYSLIB1045 suppresses MA0023.
2. If step 1 succeeds, update the test to use the pragma-suppressed pattern, remove the `Skip`, and confirm the test passes in CI; if MA0023 still does not appear, continue to step 3.
3. Target .NET 8 in the generated project (change the TFM passed to `CreateProjectBuilder`) to test on a framework where `Regex` constructor overloads are not yet deprecated, so SYSLIB1045 should not fire, and check whether MA0023 fires cleanly.
4. Check the Meziantou.Analyzer source for `MA0023` (file `MA0023*` or `RegexOptionsExplicitCapture*`) to determine whether the rule has any logic that short-circuits or defers when another diagnostic (e.g. SYSLIB1045) is already reported on the same node.
5. Check the Meziantou.Analyzer GitHub issue tracker and changelog for any notes on intentional interplay between MA0023 and SYSLIB1045, or any version in which this suppression behaviour was introduced or fixed.
6. Try a violation pattern that avoids runtime `Regex` construction entirely while still triggering MA0023 — for example a compiled or static `Regex` pattern if the rule supports it — and check whether that pattern is free of SYSLIB1045 and fires MA0023.
7. If no working pattern is found, update the `Untestable` reason with the specific root cause (source file location in meziantou/Meziantou.Analyzer or a GitHub issue link) confirming that SYSLIB1045 permanently suppresses MA0023 for all applicable patterns on the supported TFMs.

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
