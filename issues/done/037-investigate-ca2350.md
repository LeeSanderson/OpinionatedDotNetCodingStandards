## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2350 ("Do not use DataTable.ReadXml() with untrusted data") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitDataTableReadXmlWithUntrustedData` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow/taint analysis rule: fires only when untrusted input (method parameter, user-controlled source) reaches DataTable.ReadXml(); the build harness cannot trigger inter-procedural taint analysis without a full program analysis configuration"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2350", "Do not use DataTable.ReadXml() with untrusted data",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2350",
        Untestable = "Data-flow/taint analysis rule: fires only when untrusted input (method parameter, user-controlled source) reaches DataTable.ReadXml(); the build harness cannot trigger inter-procedural taint analysis without a full program analysis configuration")]
    public async Task ProhibitDataTableReadXmlWithUntrustedData()
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
                    var dt = new DataTable();
                    dt.ReadXml(xml);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2350").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where the untrusted input originates and is consumed in a single method body — for example, read a value directly from `Console.ReadLine()` or `Environment.GetEnvironmentVariable()` and pass it straight to `dt.ReadXml()` in the same statement, to confirm whether eliminating inter-procedural taint tracking makes the diagnostic fire.
2. Try the simplest possible single-statement sink call with no explicit source tracking — call `dt.ReadXml(Console.ReadLine()!)` inline with no intermediate variable — to determine whether the rule has any intra-procedural mode or requires a recognised taint source at all.
3. Check the NetAnalyzers GitHub source (src/NetAnalyzers/Core/Microsoft.NetFramework.Analyzers/DoNotUseDataTableReadXml.cs or similar) for the `EnforceOnBuild` metadata property and the `AnalysisKind` used; confirm whether the rule is registered as a dataflow/taint rule or a simple syntax-pattern rule, and whether it carries `EnforceOnBuild = false`.
4. Inspect the analyzer's SARIF / diagnostic descriptor to confirm whether `CA2350` is emitted with `DiagnosticSeverity.Warning` and `isEnabledByDefault: true`; a rule with `isEnabledByDefault: false` or severity `Hidden` will never appear in build output regardless of the code pattern used.
5. Run the existing test without `Skip` against the current build and capture the full MSBuild output; check whether any diagnostic resembling CA2350 appears at all (even as a note or suggestion rather than an error), which would indicate a severity-mapping problem rather than a taint-analysis gap.
6. Test on an older NetAnalyzers version (e.g. 8.x) by temporarily pinning the analyzer package in the generated project, to determine whether the rule ever fired in a build context or has always required full-program taint analysis — establishing whether this is a regression or a by-design limitation from the start.
7. Search the dotnet/roslyn-analyzers issue tracker for CA2350 and "EnforceOnBuild" or "build" to find any official statement that the rule is intentionally excluded from build-time enforcement, and record the issue URL as the authoritative source for the Untestable reason if found.

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
