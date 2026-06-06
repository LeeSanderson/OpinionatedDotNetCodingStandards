## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5367 ("Do Not Serialize Types With Pointer Fields") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitSerializingTypesWithPointerFields` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA5367 does not fire in NetAnalyzers 10.0.x build analysis for [Serializable] types with unsafe pointer fields; exhaustive probing with AllowUnsafeBlocks=true and a [Serializable] class containing int* fields confirmed the diagnostic is absent from SARIF output"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5367", "Do Not Serialize Types With Pointer Fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5367",
        Untestable = "CA5367 does not fire in NetAnalyzers 10.0.x build analysis for [Serializable] types with unsafe pointer fields; exhaustive probing with AllowUnsafeBlocks=true and a [Serializable] class containing int* fields confirmed the diagnostic is absent from SARIF output")]
    public async Task ProhibitSerializingTypesWithPointerFields()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "AllowUnsafeBlocks", Value: "true")]);
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            [Serializable]
            public unsafe class UnsafeData
            {
                public int* Pointer;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5367").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the `EnforceOnBuild` metadata for CA5367 in the NetAnalyzers source (search `dotnet/roslyn-analyzers` on GitHub for `CA5367` in `Metadata.json` or `RulesMissingDocumentation.md`) to confirm whether the rule is marked as build-enforceable; if `EnforceOnBuild` is `false` or absent, that alone explains why no diagnostic appears in SARIF output.

2. Try alternative violation patterns that may be more likely to trigger the analyzer: add a nested pointer field via a `fixed`-size buffer (`public fixed int Buffer[4];`), add multiple pointer fields of different types (`void*`, `char*`), and try a `struct` instead of a `class` — all still annotated with `[Serializable]` and built with `AllowUnsafeBlocks=true`.

3. Inspect the analyzer implementation in the NetAnalyzers repository (`SerializationRulesDiagnosticAnalyzer.cs` or similar) to find which syntax or symbol kinds it checks; confirm whether it operates on `FieldDeclarationSyntax`, `IFieldSymbol`, or pointer type symbols specifically, and whether there is a target-framework guard (e.g. `netstandard2.0` only) that suppresses the diagnostic on modern TFMs.

4. Test against an older NetAnalyzers version (e.g. 8.0.x) by temporarily pinning the `Microsoft.NetFramework.Analyzers` or `Microsoft.CodeAnalysis.NetAnalyzers` package version in the generated project to see whether the diagnostic was ever emitted during build analysis; this determines whether the rule regressed or was never build-enforced.

5. Verify that the SARIF output file is being read correctly and that CA5367 is not silently downgraded to a warning or info severity that `HasError` misses; re-run the test with `HasWarning("CA5367")` and `HasDiagnostic("CA5367")` variants (or inspect raw SARIF) to rule out a severity mismatch.

6. Check whether the rule requires the type to implement `ISerializable` or be used with a formatter (e.g. `BinaryFormatter`) rather than merely being annotated with `[Serializable]`; if so, add a usage site that calls `BinaryFormatter.Serialize` with an instance of the unsafe type and rebuild.

7. Search the NetAnalyzers GitHub issues and changelog for CA5367 to find any deliberate decision to disable build enforcement (e.g. a linked issue or PR comment); if found, record the URL as the confirmed root cause in the `Untestable` reason.

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
