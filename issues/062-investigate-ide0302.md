## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0302 ("Simplify collection initialization") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `SimplifyEmptyCollectionWithArrayEmpty` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "In .NET 10 Roslyn, empty collection factory methods (Array.Empty, Enumerable.Empty, ImmutableArray<T>.Empty) fire as IDE0301 (collection initialization) not IDE0302; the empty-specific rule is subsumed by IDE0301 in the build analyzer"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0302", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302",
        Untestable = "In .NET 10 Roslyn, empty collection factory methods (Array.Empty, Enumerable.Empty, ImmutableArray<T>.Empty) fire as IDE0301 (collection initialization) not IDE0302; the empty-specific rule is subsumed by IDE0301 in the build analyzer")]
    public async Task SimplifyEmptyCollectionWithArrayEmpty()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static int[] GetEmpty() => Array.Empty<int>();
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0302").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm which diagnostic actually fires by running the existing violation pattern (`Array.Empty<int>()`) and inspecting the build output for any IDE03xx notes — determine whether IDE0301 fires instead of IDE0302 and note the exact diagnostic ID and message emitted.
2. Consult the Roslyn source for the `UseCollectionInitializerDiagnosticAnalyzer` (or equivalent) to understand the boundary between IDE0301 and IDE0302 — specifically look for any condition that routes empty-factory-method patterns to IDE0301 rather than IDE0302 in .NET 10.
3. Attempt to find a code pattern that targets IDE0302 specifically and is not subsumed by IDE0301 — for example, a `new List<int>()` assigned where a collection expression could be used, or an `Enumerable.Empty<int>()` returned as `IEnumerable<int>` in a context where IDE0301 would not apply.
4. Check the Roslyn/dotnet-roslyn GitHub changelog, issues, and IDE0302 rule metadata (including `EnforceOnBuild` value) to determine whether IDE0302 was intentionally merged into IDE0301 for build-time enforcement or whether it remains a distinct diagnostic in all scenarios.
5. Try suppressing IDE0301 via `#pragma warning disable IDE0301` around the violation and observe whether IDE0302 then surfaces in the build output, confirming or refuting the subsumption theory.
6. Test the violation pattern against an older NetAnalyzers version (e.g., 8.x or 9.x) by temporarily downgrading the package reference in the test project builder to determine whether the subsumption was introduced in a specific release.
7. If no triggerable pattern is found, document the confirmed root cause with a link to the relevant Roslyn source file or GitHub issue and update the `Untestable` reason accordingly.

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
