## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2355 ("Unsafe DataSet or DataTable type found in deserializable object graph") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitDataTableInDeserializableTypeHierarchy` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow analysis rule: fires when DataSet/DataTable appears in a potentially-deserialized type hierarchy; same inter-procedural type-graph constraint as CA2354"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2355", "Unsafe DataSet or DataTable type found in deserializable object graph",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2355",
        Untestable = "Data-flow analysis rule: fires when DataSet/DataTable appears in a potentially-deserialized type hierarchy; same inter-procedural type-graph constraint as CA2354")]
    public async Task ProhibitDataTableInDeserializableTypeHierarchy()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            using System.IO;
            using System.Xml.Serialization;
            namespace test;
            public class Container { public DataTable? Table { get; set; } }
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var ser = new XmlSerializer(typeof(Container));
                    return ser.Deserialize(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2355").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern: place the `XmlSerializer` construction and the `Deserialize` call in a single method body with no helper indirection, and use `BinaryFormatter` as the deserializer instead of `XmlSerializer`, since the rule documentation explicitly targets `BinaryFormatter`-style deserialization paths that accept arbitrary object graphs.
2. Confirm whether the rule requires cross-boundary taint analysis: read the NetAnalyzers source for `UnsafeDataSetDataTableInSerializableObjectGraphAnalyzer` (or the CA2355-specific analyzer class) and check whether the diagnostic is only emitted when the type is reachable from a `Deserialize`/`ReadObject` call site rather than from a type-graph walk alone.
3. Check the `EnforceOnBuild` metadata in the NetAnalyzers source (the `DiagnosticDescriptor` for CA2355) to confirm whether the rule is marked `EnforceOnBuild = false` or its default severity is `None`, which would prevent it from appearing in build output even when the violation pattern is correct.
4. Test on NetAnalyzers 8.x by temporarily downgrading the `Microsoft.CodeAnalysis.NetAnalyzers` package reference in the generated project to `8.0.*` and re-running the same violation pattern, to determine whether the rule regressed or was never enforced on build in any version.
5. Check for target-framework guards in the analyzer source: search for `#if` blocks or `compilationOptions.Options` checks that restrict CA2355 to specific TFMs (e.g., `net5.0` or earlier), which could explain why the diagnostic is absent on the current TFM used by the test harness.
6. Try alternative violation patterns explicitly listed in the rule documentation — for example, a type hierarchy where a base class or interface holds a `DataSet` field and the derived type is passed to `XmlSerializer`, `DataContractSerializer`, or `JavaScriptSerializer` — to determine whether the existing test pattern is simply not one the analyzer recognises.
7. If no pattern produces a diagnostic, search the dotnet/roslyn-analyzers GitHub issue tracker for CA2355 reports of "no diagnostic" or "EnforceOnBuild" and capture the canonical issue URL to use as the sourced reason in the `Untestable` attribute.

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
