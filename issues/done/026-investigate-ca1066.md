## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA1066 ("Implement IEquatable when overriding Object.Equals") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RequireIEquatableWhenOverridingObjectEquals` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA1066 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern where a class overrides Object.Equals(object) without implementing IEquatable<T>; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1066.severity = warning configured"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA1066", "Implement IEquatable when overriding Object.Equals",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1066",
        Untestable = "CA1066 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern where a class overrides Object.Equals(object) without implementing IEquatable<T>; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1066.severity = warning configured")]
    public async Task RequireIEquatableWhenOverridingObjectEquals()
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
                public override int GetHashCode() => System.HashCode.Combine(X, Y);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1066").ShouldBeTrue();
    }
```

## Investigation plan

1. Try alternative violation patterns to confirm the diagnostic is truly absent: test a struct overriding `Equals`, a sealed class, and a class that only overrides `Equals` without overriding `GetHashCode`, to rule out pattern-specific suppression.

2. Check the `EnforceOnBuild` metadata for CA1066 in the NetAnalyzers source (search `dotnet/roslyn-analyzers` for `CA1066` in `Descriptors.cs` or the relevant rule file) to confirm whether the rule is marked `EnforceOnBuild = true`; if it is `false` or missing, the diagnostic will never appear in MSBuild output regardless of severity configuration.

3. Test the same violation pattern against an older NetAnalyzers version (e.g., 8.0.x) by temporarily downpinning the `Microsoft.CodeAnalysis.NetAnalyzers` package reference in the generated test project, to determine if the rule regressed or was intentionally disabled in 10.0.x.

4. Check for target-framework guards in the CA1066 analyzer source: search for `#if`, `TargetFramework`, or version checks in the analyzer implementation that might suppress the diagnostic for .NET 9/10 targets.

5. Verify the SARIF output directly by running `dotnet build` with `/p:ErrorLog=output.sarif;version=2` on a minimal hand-crafted project containing the violation, and inspect whether CA1066 appears with any severity (including `note` or `hidden`) before concluding it is fully absent.

6. Search the `dotnet/roslyn-analyzers` GitHub issues and changelog for CA1066 to identify any intentional deprecation, known regression, or plans to remove the rule in favour of a compiler-enforced equivalent.

7. If all patterns confirm the rule never fires in build analysis, update the `Untestable` reason with the specific root cause found (EnforceOnBuild value, source file location, or GitHub issue link) and leave the `[Fact(Skip = "untestable")]` annotation in place.

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
