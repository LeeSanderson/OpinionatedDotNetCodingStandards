## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5360 ("Do Not Call Dangerous Methods In Deserialization") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitDangerousMethodsInDeserialization` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the standard ISerializable constructor + Process.Start pattern documented in the rule's official examples; likely requires inter-procedural data-flow analysis not available in the single-project build harness"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5360", "Do Not Call Dangerous Methods In Deserialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5360",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the standard ISerializable constructor + Process.Start pattern documented in the rule's official examples; likely requires inter-procedural data-flow analysis not available in the single-project build harness")]
    public async Task ProhibitDangerousMethodsInDeserialization()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Diagnostics;
            using System.Runtime.Serialization;
            namespace test;
            [Serializable]
            public class MyClass : ISerializable
            {
                protected MyClass(SerializationInfo info, StreamingContext context)
                {
                    Process.Start("cmd.exe");
                }
                public void GetObjectData(SerializationInfo info, StreamingContext context) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5360").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where both the deserialization context and the dangerous call appear in a single constructor body with no helper methods, confirming whether the analyzer requires any cross-method flow or fires on a pure single-method pattern.
2. Try alternative dangerous method calls documented in the rule's official examples (e.g. `File.Delete`, `Directory.Delete`, `Registry` writes) instead of `Process.Start` to rule out the possibility that `Process.Start` is specifically excluded from the rule's sink list.
3. Check the `EnforceOnBuild` metadata for CA5360 in the NetAnalyzers source (look in `src/NetAnalyzers/Core/AnalyzerReleases.Shipped.md` and the rule's `.cs` source file) to confirm whether the rule is enabled for build-time enforcement at all and which diagnostic severity it ships with.
4. Confirm whether the rule requires inter-procedural taint analysis by reading the CA5360 analyzer implementation in the `roslyn-analyzers` GitHub repository, specifically checking whether the rule's visitor only flags calls that appear syntactically inside `IDeserializationCallback.OnDeserialization` or `ISerializable` constructors without any data-flow tracking.
5. Test whether removing the `GetObjectData` method or adding additional `[Serializable]`-related attributes changes diagnostic output, to narrow down whether the rule has a structural prerequisite beyond the constructor signature.
6. Run the existing test without `Skip` against a pinned older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily modifying the test project's package reference, to determine whether the rule ever fired in practice or has always been inactive at build time.
7. Search the `dotnet/roslyn-analyzers` GitHub issue tracker for CA5360 reports of the rule not firing, and record any confirmed issue link or source-level comment that explains whether the rule is intentionally limited to IDE-only or requires a runtime context unavailable in the build harness.

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
