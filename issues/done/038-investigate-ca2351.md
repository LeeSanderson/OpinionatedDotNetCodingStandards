## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2351 ("Do not use DataSet.ReadXml() with untrusted data") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitDataSetReadXmlWithUntrustedData` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow/taint analysis rule: fires only when untrusted input reaches DataSet.ReadXml(); same taint-analysis constraint as CA2350"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2351", "Do not use DataSet.ReadXml() with untrusted data",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2351",
        Untestable = "Data-flow/taint analysis rule: fires only when untrusted input reaches DataSet.ReadXml(); same taint-analysis constraint as CA2350")]
    public async Task ProhibitDataSetReadXmlWithUntrustedData()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            namespace test;
            public static class Program
            {
                public static void ReadData(string xml)
                {
                    var ds = new DataSet();
                    ds.ReadXml(xml);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2351").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where the untrusted string literal or parameter flows into `DataSet.ReadXml()` in a single statement with no intermediate variables, to determine whether collapsing the call chain changes diagnostic output.
2. Try passing the value from `Console.ReadLine()` (a recognised taint source in NetAnalyzers) directly to `ds.ReadXml()` in the same method body, since some taint-tracking analyzers only fire when a known source API is present rather than when an arbitrary parameter is used.
3. Check the NetAnalyzers GitHub source (under `src/NetAnalyzers/Core/Microsoft.NetFramework.Analyzers/`) for the CA2351 analyzer implementation and inspect the `EnforceOnBuild` metadata property — if it is `false` or absent the rule will never fire during a build-time analysis pass regardless of the code pattern.
4. Confirm whether CA2351 requires cross-boundary interprocedural taint analysis by reading the analyzer's data-flow visitor: if the sink check only triggers after tracking taint across method call boundaries, any single-method pattern will be insufficient and the rule is structurally untestable with the current test harness.
5. Test whether the rule fires at all under NetAnalyzers 8.x (e.g. `Microsoft.CodeAnalysis.NetAnalyzers` version `8.0.*`) by temporarily pinning the package version in the generated test project, since some taint rules were downgraded or had their `EnforceOnBuild` flag removed between major analyzer versions.
6. Check for target-framework guards in the analyzer source — some DataSet-related rules are conditionally compiled or skipped for .NET 5+ targets because the runtime already emits its own warning; if CA2351 is guarded this way it will never produce a diagnostic in the `net10.0` TFM used by the test harness.
7. If none of the above patterns produce a diagnostic, record the specific method name, file path, and `EnforceOnBuild` value found in the NetAnalyzers source (or the relevant GitHub issue/PR) as the confirmed root cause, and update the `Untestable` string accordingly.

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
