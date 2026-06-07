## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2354 ("Unsafe DataSet or DataTable in deserialized object graph can be vulnerable to remote code execution attacks") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitDataSetInDeserializedObjectGraph` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow analysis rule: fires when a DataSet/DataTable type appears in the deserialization graph of a call to a generic deserialization API; requires inter-procedural type-graph analysis not triggerable from a single-project build harness"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2354", "Unsafe DataSet or DataTable in deserialized object graph can be vulnerable to remote code execution attacks",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2354",
        Untestable = "Data-flow analysis rule: fires when a DataSet/DataTable type appears in the deserialization graph of a call to a generic deserialization API; requires inter-procedural type-graph analysis not triggerable from a single-project build harness")]
    public async Task ProhibitDataSetInDeserializedObjectGraph()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            using System.IO;
            using System.Runtime.Serialization;
            namespace test;
            [DataContract]
            public class Container { [DataMember] public DataSet? Data { get; set; } }
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var ser = new DataContractSerializer(typeof(Container));
                    return ser.ReadObject(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2354").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where the deserialization call and the DataSet/DataTable type reference appear in a single statement or immediately adjacent statements, eliminating any interprocedural hop, to check whether the rule fires at all in a trivial single-method scenario.
2. Try additional deserialization API entry points known to trigger CA2354 (e.g. `XmlSerializer`, `BinaryFormatter`, `JsonSerializer` with a type containing `DataSet`) in a same-method direct pattern to determine whether the failure is specific to `DataContractSerializer` or applies to all deserialization APIs.
3. Confirm that the rule requires cross-boundary taint analysis by checking the NetAnalyzers source for CA2354 (file `DataSetDataTableInSerializableTypeAnalyzer.cs` or similar under `src/NetAnalyzers`) to understand the exact trigger condition — specifically whether it only fires on types reachable from an external assembly boundary rather than a same-project type.
4. Check the `EnforceOnBuild` metadata for CA2354 in the NetAnalyzers source (look for `Rule` definitions or `.editorconfig` defaults in the NetAnalyzers repo) to confirm whether the rule is enabled by default during build or only in IDE analysis, since a rule that is IDE-only will never appear in `BuildAndGetOutput`.
5. If CA2354 has `EnforceOnBuild = never` or is IDE-only, check the NetAnalyzers changelog and GitHub issues (search `microsoft/roslyn-analyzers` for `CA2354`) for the official rationale and link the relevant issue or PR in the updated Untestable reason.
6. If the rule does appear to be build-enforceable, test against an older NetAnalyzers version (e.g. 8.x) to determine whether the diagnostic was removed or regressed in 10.0.x, cross-referencing the feedback note about CA security rules not firing in 10.0.102.
7. Based on findings, either remove `Skip` and update the test with a working violation pattern, or update the `Untestable` string with the confirmed root cause (e.g. `EnforceOnBuild = never`, specific GitHub issue URL, or confirmed interprocedural-only trigger).

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
