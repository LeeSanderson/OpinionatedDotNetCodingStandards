## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0304 ("Simplify collection initialization") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `SimplifyEmptyImmutableArrayCollection` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn, ImmutableArray<T>.Empty fires as IDE0301 (collection initialization) not IDE0304; the ImmutableArray-specific empty collection rule is subsumed by IDE0301 in the build analyzer"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0304", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304",
        Untestable = "In .NET 10 Roslyn, ImmutableArray<T>.Empty fires as IDE0301 (collection initialization) not IDE0304; the ImmutableArray-specific empty collection rule is subsumed by IDE0301 in the build analyzer")]
    public async Task SimplifyEmptyImmutableArrayCollection()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Immutable;
            namespace test;
            public static class Program
            {
                public static ImmutableArray<int> GetEmpty() => ImmutableArray<int>.Empty;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0304").ShouldBeTrue();
    }
```

## Investigation plan

1. Understand which rule fires instead: run the current test pattern without the Skip and capture the full build output to confirm IDE0301 fires (not IDE0304) for `ImmutableArray<int>.Empty`, and record the exact diagnostic message.
2. Search the Roslyn source for the IDE0304 analyzer implementation (look for `UseCollectionInitializerDiagnosticAnalyzer` or `IDE0304` constant) to determine the exact conditions under which IDE0304 is emitted versus IDE0301, and whether the ImmutableArray path was intentionally merged into IDE0301 in a specific Roslyn version.
3. Try alternative violation patterns that reference `ImmutableArray` creation without using `.Empty` — for example `new ImmutableArray<int>()` or `ImmutableArray.Create<int>()` — to find a code shape that triggers IDE0304 rather than IDE0301.
4. Check the Roslyn/dotnet-roslyn GitHub changelog, issues, and pull requests for any intentional decision to subsume IDE0304 into IDE0301 for ImmutableArray, to find a canonical source (issue URL or PR link) confirming the subsumption is permanent.
5. Check the EnforceOnBuild metadata for IDE0304 in the NetAnalyzers/Roslyn source (look for `EnforceOnBuild` or `customTags` on the IDE0304 descriptor) to determine whether the rule is expected to fire during `dotnet build` at all, or whether it is IDE-only.
6. If no triggerable pattern is found, update the `Untestable` reason in the `[RuleDoc]` attribute with the confirmed root cause, citing the specific Roslyn source location or GitHub issue URL found in steps 2 and 4.

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
