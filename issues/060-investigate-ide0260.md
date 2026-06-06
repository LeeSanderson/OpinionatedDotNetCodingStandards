## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0260 ("Use pattern matching") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UsePatternMatchingInsteadOfAsNotNull` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn build analysis, IDE0260 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: 'obj as T != null' triggers IDE0055 while the equivalent 'obj is T' does not. The rule uses the formatter as its build-mode enforcement mechanism."

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0260", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0260 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: 'obj as T != null' triggers IDE0055 while the equivalent 'obj is T' does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task UsePatternMatchingInsteadOfAsNotNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsString(object obj) => (obj as string) != null;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0260").ShouldBeTrue();
    }
```

## Investigation plan

1. Suppress IDE0055 in the generated project by adding an editorconfig additionalFiles entry (e.g. `dotnet_diagnostic.IDE0055.severity = none`) and re-run the probe to see whether IDE0260 then surfaces as its own diagnostic ID in the build output.

2. Add `#pragma warning disable IDE0055` around the violation site in the test source and rebuild; determine whether the pragma causes IDE0260 to appear separately or whether it is silently dropped.

3. Inspect the Roslyn / dotnet/roslyn source for the IDE0260 analyzer (search `UsePatternMatchingDiagnosticAnalyzer` or `IDE0260`) and confirm whether `EnforceOnBuild` is set to `WhenExplicitlyConfigured`, `Never`, or another value that would prevent build-mode emission under the current editorconfig.

4. Set `dotnet_diagnostic.IDE0260.severity = warning` explicitly in the editorconfig (rather than relying on the style-rule cascade) and re-run the build probe to determine whether an explicit severity override bypasses the formatter-routing behaviour.

5. Try alternative violation patterns that are covered by IDE0260 — for example a null-check guard (`if ((obj as string) != null)`) or a conditional-access form — to determine whether any pattern routes the diagnostic through IDE0260 rather than IDE0055.

6. Run the same probe against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package in the generated project, to establish whether the formatter-routing behaviour is a regression introduced in NetAnalyzers 10.x.

7. Check the dotnet/roslyn changelog, GitHub issues, and any `EnforceOnBuild` metadata for IDE0260 to find an authoritative upstream explanation; if a tracking issue exists, record its URL as the sourced reason in the Untestable field.

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
