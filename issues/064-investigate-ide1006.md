## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE1006 ("Naming Styles") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RequireUnderscorePrefixForPrivateFields` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE1006; also, CS0708 (member cannot be declared static in a non-static class) preempts the instance field violation pattern in a static class"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE1006", "Naming Styles",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1006",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE1006; also, CS0708 (member cannot be declared static in a non-static class) preempts the instance field violation pattern in a static class")]
    public async Task RequireUnderscorePrefixForPrivateFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyService
            {
                private string myField = "value";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1006").ShouldBeTrue();
    }
```

## Investigation plan

1. Attempt to suppress IDE0055 in the generated project by injecting an `[*.cs]` editorconfig entry that sets `dotnet_diagnostic.IDE0055.severity = none`, then re-run the build and check whether IDE1006 appears in the SARIF output without IDE0055 masking it.
2. Check the Roslyn/NetAnalyzers source for IDE1006's `EnforceOnBuild` metadata value (look for `NamingStyleDiagnosticAnalyzer` or the equivalent descriptor in the `Microsoft.CodeAnalysis.CSharp.CodeStyle` assembly) to confirm whether the rule is intentionally excluded from build enforcement.
3. Try `#pragma warning disable IDE0055` directly inside the violation file to see whether suppressing the formatter diagnostic at the source level allows IDE1006 to surface on its own in build output.
4. Test against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package in `CreateProjectBuilder`, to determine whether the formatter-backed emission of IDE0055 instead of IDE1006 is a regression introduced in 10.x.
5. Confirm whether any C# field pattern avoids the CS0708 compiler error when working inside a non-static class — specifically, verify that the existing test code already uses a non-static class (`MyService`) and that CS0708 is therefore not the active blocker, leaving IDE0055 subsumption as the sole issue.
6. Search the Roslyn GitHub issues and changelog for any discussion of IDE1006 being intentionally routed through IDE0055 in build mode, and record the canonical issue URL if found, to use as the sourced reason in the Untestable field.
7. If all suppression attempts fail, update the `Untestable` reason with the confirmed root cause (EnforceOnBuild value or Roslyn issue link) and remove any secondary CS0708 rationale that the investigation shows is not applicable.

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
