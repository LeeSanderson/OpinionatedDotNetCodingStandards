## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2300 ("Do not use insecure deserializer BinaryFormatter") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitBinaryFormatterUsage` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; with TreatWarningsAsErrors=true the SYSLIB0011 diagnostic becomes an error and the build fails before CA2300 fires as a distinct diagnostic; the rule cannot be tested without suppressing SYSLIB0011 across the test harness"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2300", "Do not use insecure deserializer BinaryFormatter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2300",
        Untestable = "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; with TreatWarningsAsErrors=true the SYSLIB0011 diagnostic becomes an error and the build fails before CA2300 fires as a distinct diagnostic; the rule cannot be tested without suppressing SYSLIB0011 across the test harness")]
    public async Task ProhibitBinaryFormatterUsage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0011
            using System.IO;
            using System.Runtime.Serialization.Formatters.Binary;
            namespace test;
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0011
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2300").ShouldBeTrue();
    }
```

## Investigation plan

1. Run the existing skipped test as-is (remove `Skip`) and capture the full build output to confirm whether `#pragma warning disable SYSLIB0011` already suppresses the obsolescence error and whether CA2300 appears at all in the output.

2. If CA2300 does not appear even with SYSLIB0011 suppressed, check the NetAnalyzers source (dotnet/roslyn-analyzers on GitHub) for the `DoNotUseInsecureDeserializerBinaryFormatterAnalyzer` implementation — specifically look at the `EnforceOnBuild` metadata property and any target-framework guards (`#if NET` blocks or `targetFramework` checks) that might disable the rule on .NET 9+.

3. Try moving the `#pragma warning disable SYSLIB0011` to a file-level suppression (place it at the top of the file before all `using` directives without a matching `restore`) and rebuild, to rule out the possibility that the pragma scope is too narrow and the obsolescence diagnostic is still emitted for the type reference in the `using` directive.

4. Try testing on .NET 8 (change the target framework in `CreateProjectBuilder` if the builder supports a `targetFramework` parameter) where `BinaryFormatter` may not yet carry the hard-obsolescence attribute, to determine whether the CA2300 rule fires correctly on an older TFM and the issue is purely a .NET 9+ regression.

5. Check whether `NoWarn` or `<WarningsNotAsErrors>` properties can be injected into the generated test project via `CreateProjectBuilder` (e.g. a `globalProperties` or similar API) to suppress SYSLIB0011 at the MSBuild level rather than via pragma, and verify whether that allows CA2300 to surface as a distinct diagnostic.

6. Search the NetAnalyzers GitHub issues and changelog for CA2300 + SYSLIB0011 interaction reports to find any upstream acknowledgement that CA2300 is intentionally suppressed or removed on .NET 9+, and capture the issue/PR URL as the sourced reason if confirmed.

7. If a working suppression strategy is found in any of the above steps, update the test (remove `Skip`, adjust the pragma or property approach) and run the full test file to confirm CA2300 fires and no regressions are introduced in neighbouring tests.

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
