## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse MA0130 ("GetType() should not be used on System.Type instances") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ShouldNotCallGetTypeOnTypeInstance` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Diagnostic ID is registered in the Meziantou.Analyzer 2.0.286 editorconfig but the analyzer implementation is absent in this version; the rule never fires"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("MA0130", "GetType() should not be used on System.Type instances",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0130.md",
        Untestable = "Diagnostic ID is registered in the Meziantou.Analyzer 2.0.286 editorconfig but the analyzer implementation is absent in this version; the rule never fires")]
    public async Task ShouldNotCallGetTypeOnTypeInstance()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System;
            namespace test;
            public class C
            {
                public Type GetActualType(Type t) => t.GetType();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0130").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the Meziantou.Analyzer NuGet package version currently referenced in the test project and compare it against the changelog at https://github.com/meziantou/Meziantou.Analyzer/releases to find the first release that ships an implementation for MA0130.
2. Search the Meziantou.Analyzer GitHub repository source tree for any class or file that references `MA0130` as a diagnostic ID (e.g. `DiagnosticId = "MA0130"` or `MA0130` in an analyzer registration) to confirm whether an implementation exists in the current or any later release.
3. Upgrade the Meziantou.Analyzer package reference in the test project (or pass a higher version via `CreateProjectBuilder`) to the latest available release, rebuild, and re-run the test to see whether the diagnostic fires with the existing violation pattern.
4. If upgrading the package does not make the diagnostic fire, try alternative violation patterns that more directly express calling `GetType()` on a `System.Type` variable — for example calling `typeof(int).GetType()` or assigning `Type t = typeof(string); var r = t.GetType();` in the same method — and rebuild after each change.
5. Inspect the editorconfig entry shipped with the package (the `.editorconfig` embedded in the NuGet package's `build/` folder) to verify that MA0130 is set to `error` or `warning` in the severity map and that no `EnforceOnBuild` metadata disables it.
6. If a working pattern is found, remove `[Fact(Skip = "untestable")]` and the `Untestable =` property from the `[RuleDoc]` attribute, confirm the test passes in CI (including the RuleReferenceGenerator coverage test), and commit.
7. If no version of the package ships the implementation, update the `Untestable` reason with a direct link to the GitHub issue or PR that tracks the missing implementation (or confirms the rule was registered speculatively), so the reason is permanently sourced.

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
