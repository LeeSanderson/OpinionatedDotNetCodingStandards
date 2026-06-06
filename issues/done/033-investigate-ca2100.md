## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2100 ("Review SQL queries for security vulnerabilities") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ReviewSqlQueriesForSecurityVulnerabilities` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Requires data-flow taint analysis to track untrusted input from parameter to SQL string; build-based harness cannot trigger inter-procedural data-flow rules that require full program analysis"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2100", "Review SQL queries for security vulnerabilities",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2100",
        Untestable = "Requires data-flow taint analysis to track untrusted input from parameter to SQL string; build-based harness cannot trigger inter-procedural data-flow rules that require full program analysis")]
    public async Task ReviewSqlQueriesForSecurityVulnerabilities()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data.Common;
            namespace test;
            public static class Program
            {
                public static void Execute(DbCommand cmd, string tableName)
                {
                    cmd.CommandText = "SELECT * FROM " + tableName;
                    cmd.ExecuteNonQuery();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2100").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where the tainted string and the SQL assignment appear in a single method body with no cross-method call, to determine whether CA2100 requires inter-procedural taint or fires on intra-procedural string concatenation with a parameter.

2. Try a minimal pattern where the SQL string is constructed directly from a method parameter and assigned to `CommandText` in the same statement (e.g., `cmd.CommandText = "SELECT * FROM " + userInput; cmd.ExecuteNonQuery();`) inside a single method, with no helper calls, to confirm whether the harness can trigger the diagnostic at all.

3. Check the NetAnalyzers source (specifically `src/NetAnalyzers/Core/Microsoft.NetFramework.Analyzers/DoNotUseInsecureDtdProcessing.cs` or the equivalent CA2100 analyzer file) to confirm whether `EnforceOnBuild` is set to `true` and whether the rule is gated on any target-framework guard that would suppress it for modern .NET targets.

4. Confirm whether CA2100 produces a diagnostic in the build output (SARIF or MSBuild errors) by running the test harness with `--no-build` disabled and inspecting raw build output for any CA2100 entry, to rule out the possibility that the diagnostic is emitted but at a severity level the `HasError` check does not match (e.g., warning vs. error).

5. Check the NetAnalyzers changelog or GitHub issues for CA2100 to determine whether the rule was intentionally scoped to legacy `System.Data` ADO.NET patterns or was restricted to specific `IDbCommand` implementations, which could explain why the `DbCommand` base-class pattern does not fire.

6. If no intra-procedural pattern fires, verify the root cause by checking the NetAnalyzers source for CA2100's `OperationBlockStartAction` or `DataFlowAnalysis` usage to confirm the rule is implemented as a full inter-procedural taint-tracking analysis that cannot produce diagnostics without cross-boundary program analysis, and document the exact source file and line range as evidence.

7. If the rule is confirmed untestable, update the `Untestable` reason in the `[RuleDoc]` attribute with a precise source reference (GitHub file path and the specific method or field that gates the analysis) so the reason is verifiable and not just a general description.

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
