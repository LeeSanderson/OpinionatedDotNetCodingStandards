## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0070 ("Use 'System.HashCode'") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseSystemHashCodeInsteadOfXorGetHashCode` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn build analysis, IDE0070 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: the XOR GetHashCode pattern triggers IDE0055 across every file in the compilation, and replacing it with HashCode.Combine removes IDE0055 entirely. The rule uses the formatter as its build-mode enforcement mechanism."

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0070", "Use 'System.HashCode'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0070",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0070 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: the XOR GetHashCode pattern triggers IDE0055 across every file in the compilation, and replacing it with HashCode.Combine removes IDE0055 entirely. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task UseSystemHashCodeInsteadOfXorGetHashCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Point
            {
                public int X { get; set; }
                public int Y { get; set; }
                public override bool Equals(object? obj) => obj is Point p && X == p.X && Y == p.Y;
                public override int GetHashCode() => X ^ Y;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0070").ShouldBeTrue();
    }
```

## Investigation plan

1. Suppress IDE0055 in the generated project by adding a `dotnet_diagnostic.IDE0055.severity = none` entry via the test harness editorconfig mechanism, then re-run the build and inspect whether IDE0070 now appears in the build output.
2. Try adding `#pragma warning disable IDE0055` around the `GetHashCode` method body (or at file scope) in the violation file, rebuild, and check whether IDE0070 surfaces as its own diagnostic ID.
3. Check the Roslyn/roslyn-analyzers source (or the NetAnalyzers NuGet package metadata) for the `EnforceOnBuild` property of IDE0070 — confirm whether the rule is tagged `WhenExplicitlyConfigured`, `Never`, or another value that prevents it from emitting its own ID in build mode.
4. Look up the analyzer changelog and any linked Roslyn GitHub issues for IDE0070 to confirm whether the formatter-backed emission via IDE0055 is intentional and permanent design, or a known regression with a planned fix.
5. Test on an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package reference in the test harness to see whether IDE0070 emitted its own diagnostic ID in earlier toolchain versions, which would confirm whether this is a regression or longstanding design.
6. Try a multi-property XOR pattern (three or more properties XOR-combined) and a struct-based `GetHashCode` to determine whether any alternative violation shape causes IDE0070 to fire independently of IDE0055.
7. If none of the above unblocks the diagnostic ID, update the `Untestable` reason on the `[RuleDoc]` attribute with the confirmed root cause, including the specific Roslyn source location or GitHub issue URL, so the reason is traceable and precise.

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
