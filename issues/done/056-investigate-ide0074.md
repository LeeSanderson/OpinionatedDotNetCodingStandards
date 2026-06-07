## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0074 ("Use compound assignment") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseNullCoalescingCompoundAssignment` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn, x = x ?? y (null-coalescing compound assignment) fires as IDE0054 (general compound assignment) not IDE0074; the two rules share the same diagnostic trigger in this analyzer version"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0074", "Use compound assignment",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0074",
        Untestable = "In .NET 10 Roslyn, x = x ?? y (null-coalescing compound assignment) fires as IDE0054 (general compound assignment) not IDE0074; the two rules share the same diagnostic trigger in this analyzer version")]
    public async Task UseNullCoalescingCompoundAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string GetOrDefault(string? value)
                {
                    value = value ?? "default";
                    return value;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0074").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm which diagnostic actually fires for the current violation pattern by inspecting the raw build output (SARIF or compiler output) for the `value = value ?? "default"` pattern — determine whether IDE0054, IDE0074, both, or neither appears.
2. Search the Roslyn source (dotnet/roslyn on GitHub) for the analyzer class responsible for IDE0074 to understand whether IDE0074 and IDE0054 share a single `DiagnosticDescriptor` or use separate ones, and whether IDE0074 has an `EnforceOnBuild` severity that would cause it to surface as a build error.
3. Check the Roslyn changelog or IDE0074 rule history to determine whether IDE0074 was intentionally merged into or subsumed by IDE0054 in a specific Roslyn/SDK version, and note the version and any linked GitHub issue.
4. Try alternative null-coalescing compound assignment patterns (e.g. on a field rather than a local variable, on a property, or with a reference type field) to find any pattern that produces IDE0074 rather than IDE0054 under .NET 10 Roslyn.
5. Test whether explicitly disabling IDE0054 in the generated project's editorconfig (e.g. `dotnet_diagnostic.IDE0054.severity = none`) causes the same violation pattern to surface as IDE0074 instead, confirming or refuting the "shared trigger" hypothesis.
6. If steps 4–5 find no working pattern, verify whether IDE0074 is effectively dead code in .NET 10 Roslyn by checking the NetAnalyzers 10.x source for any remaining reference to the IDE0074 descriptor, and confirm whether it is intentionally retired or still present but unreachable.
7. Based on findings: either update the test with a confirmed working pattern and remove `Skip`, or update the `Untestable` reason with the specific Roslyn source location or GitHub issue URL that confirms permanent subsumption by IDE0054.

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
