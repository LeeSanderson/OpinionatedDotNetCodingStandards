## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0079 ("Remove unnecessary suppression") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RemoveUnnecessarySuppression` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn build analysis, IDE0079 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: an unnecessary SuppressMessage triggers IDE0055 while an equivalent necessary suppression does not. The rule uses the formatter as its build-mode enforcement mechanism."

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0079", "Remove unnecessary suppression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0079",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0079 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: an unnecessary SuppressMessage triggers IDE0055 while an equivalent necessary suppression does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task RemoveUnnecessarySuppression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity")]
                public static void M() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0079").ShouldBeTrue();
    }
```

## Investigation plan

1. Suppress IDE0055 in the generated project by adding an additionalFiles editorconfig entry (e.g. `dotnet_diagnostic.IDE0055.severity = none`) and re-run the build to determine whether IDE0079 then surfaces as its own diagnostic ID or disappears entirely.
2. Add `#pragma warning disable IDE0055` around the unnecessary suppression attribute in the violation file and check whether IDE0079 is emitted on its own once the formatter diagnostic is silenced.
3. Inspect the Roslyn source for `RemoveUnnecessarySuppressionDiagnosticAnalyzer` (or equivalent) to confirm the value of `EnforceOnBuild` — if it is `Never` or `WhenExplicitlyEnabled` that explains why build-mode never raises IDE0079 directly.
4. Check the Roslyn changelog and IDE0079 GitHub history (dotnet/roslyn issues/PRs) for any deliberate decision to route the build-mode diagnostic through the formatter (IDE0055) rather than emitting IDE0079, and record the source URL.
5. Try an alternative violation pattern that uses `#pragma warning disable` / `#pragma warning restore` for a warning that does not exist in the project (e.g. `#pragma warning disable CA9999`) to see whether a pragma-based unnecessary suppression emits IDE0079 directly rather than IDE0055.
6. Test against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package reference in `CreateProjectBuilder` to confirm whether the IDE0055 indirection is a regression introduced in 10.x or has always been the behaviour.
7. If all build-mode paths confirm IDE0079 is permanently routed through IDE0055 in build mode, update the `Untestable` reason with the confirmed root cause including the Roslyn source file path or GitHub issue link, then close the investigation.

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
