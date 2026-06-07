## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0080 ("Remove unnecessary suppression operator") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RemoveUnnecessaryNullForgivingOperator` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0080; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0080", "Remove unnecessary suppression operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0080",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0080; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task RemoveUnnecessaryNullForgivingOperator()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string s = "hello";
                    _ = s!.Length;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0080").ShouldBeTrue();
    }
```

## Investigation plan

1. Suppress IDE0055 in the generated project by injecting an additional editorconfig entry (`dotnet_diagnostic.IDE0055.severity = none`) via the test harness `CreateProjectBuilder` options, then rebuild and check whether `IDE0080` now appears in the SARIF output independently.
2. Check the Roslyn source for `IDE0080` (class `UnnecessarySuppressionOperatorDiagnosticAnalyzer` in `roslyn/src/Analyzers`) to confirm whether `EnforceOnBuild` is set to `WhenExplicitlyConfigured`, `Never`, or another value — a value of `Never` means the diagnostic is intentionally suppressed in command-line builds regardless of editorconfig severity.
3. Try adding `#pragma warning disable IDE0055` around the violation in the test file so the formatter diagnostic cannot fire, and observe whether `IDE0080` is then emitted on its own.
4. Test the same violation pattern against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily downgrading the package reference in the test harness, to determine whether the formatter-routing behaviour was introduced in a specific release.
5. Try alternative violation patterns for IDE0080 — for example a nullable reference type context (`string? s = null; _ = s!.Length;` with `<Nullable>enable</Nullable>`) — in case the non-nullable pattern is not recognised as a valid suppression-operator violation, causing no diagnostic at all rather than a re-routed one.
6. Search the Roslyn GitHub repository issues and changelog for `IDE0080` and `EnforceOnBuild` together to find any official acknowledgement that the rule is intentionally formatter-backed and cannot surface its own ID in a CLI build, then record the issue URL as the sourced reason if confirmed.
7. If none of the above yields a working `IDE0080` diagnostic, verify that the SARIF output contains `IDE0055` (not `IDE0080`) when the violation is present — confirming the formatter-routing hypothesis — and update the `Untestable` reason with the specific Roslyn source location or GitHub issue link that explains the routing.

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
