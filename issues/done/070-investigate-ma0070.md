## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse MA0070 ("Obsolete attributes should include explanations") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ObsoleteAttributesShouldIncludeExplanations` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA1041 covers the same null/empty ObsoleteAttribute message condition and fires instead of MA0070 in this harness"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("MA0070", "Obsolete attributes should include explanations",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0070.md",
        Untestable = "CA1041 covers the same null/empty ObsoleteAttribute message condition and fires instead of MA0070 in this harness")]
    public async Task ObsoleteAttributesShouldIncludeExplanations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                [System.Obsolete]
                public void OldMethod() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0070").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm which rule fires for the current pattern by inspecting the raw build output (notes/warnings) for both CA1041 and MA0070 — establish whether CA1041 is present, MA0070 is absent, or both/neither appear.

2. Check the MA0070 analyzer source and the CA1041 analyzer source to understand the exact conditions each rule fires on — determine whether MA0070 has a suppression guard that defers to CA1041, or whether both rules are designed to fire independently and the harness severity configuration is the issue.

3. Attempt to suppress CA1041 via a `#pragma warning disable CA1041` directive around the violation in the generated `Program.cs` and re-run the test to see whether MA0070 surfaces once CA1041 is silenced.

4. Attempt to suppress CA1041 at the project level by adding `<NoWarn>CA1041</NoWarn>` (or the equivalent editorconfig severity override `dotnet_diagnostic.CA1041.severity = none`) to the generated project configuration in `CreateProjectBuilder`, then check whether MA0070 fires.

5. Review the Meziantou.Analyzer changelog and the MA0070 GitHub issue tracker to determine whether MA0070 is intentionally designed to not fire when CA1041 is active, or whether this is a known bug/overlap that was later resolved in a newer package version.

6. Try an alternative violation pattern that may satisfy MA0070 without triggering CA1041 — for example, `[System.Obsolete("")]` (empty string rather than omitted message), or `[System.Obsolete(null)]` explicitly, to see whether the two rules diverge on any specific form of the attribute.

7. If none of the above produces a passing MA0070 diagnostic, update the `Untestable` reason with the confirmed root cause, citing the relevant analyzer source file or GitHub issue URL, and document the exact conditions under which MA0070 would theoretically fire if CA1041 were absent.

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
