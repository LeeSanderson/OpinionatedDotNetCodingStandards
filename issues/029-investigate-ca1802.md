## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA1802 ("Use literals where appropriate") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseLiteralsForStaticReadonlyConstantFields` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA1802 does not fire in NetAnalyzers 10.0.x build analysis for public static readonly fields initialized with compile-time constants (string, int, bool) in either static or instance classes; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1802.severity = warning configured"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA1802", "Use literals where appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1802",
        Untestable = "CA1802 does not fire in NetAnalyzers 10.0.x build analysis for public static readonly fields initialized with compile-time constants (string, int, bool) in either static or instance classes; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1802.severity = warning configured")]
    public async Task UseLiteralsForStaticReadonlyConstantFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Config
            {
                public static readonly string DefaultName = "DefaultName";
                public static readonly int MaxRetries = 3;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1802").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the `EnforceOnBuild` metadata for CA1802 in the NetAnalyzers 10.0.x source (the `Microsoft.CodeAnalysis.NetAnalyzers` NuGet package or the `dotnet/roslyn-analyzers` GitHub repository) to confirm whether the rule is marked `EnforceOnBuild = false`, which would prevent it from firing during `dotnet build` regardless of editorconfig severity settings.

2. Try alternative violation patterns that may be more likely to trigger the diagnostic: use an internal class instead of a public class, use a non-static class with `static readonly` fields, or use primitive types other than `string` (e.g., `double`, `char`). Verify whether the absence from SARIF is pattern-specific or rule-wide.

3. Test with NetAnalyzers 8.0.x by temporarily pinning the `Microsoft.NET.CodeAnalysis.NetAnalyzers` package version in the generated test project (if `CreateProjectBuilder` supports a `packageReferences` parameter or equivalent) to confirm whether the rule fired in an older version and was subsequently disabled for build enforcement.

4. Check the `dotnet/roslyn-analyzers` GitHub repository (issues and pull requests) for any discussion of CA1802 being intentionally excluded from build enforcement, or any known regression that caused diagnostics to stop appearing in SARIF output.

5. Inspect the generated `.editorconfig` and `.sarif` output from an actual test run with `dotnet_diagnostic.CA1802.severity = error` set explicitly, and confirm whether the rule ID appears at all in the SARIF results array (even as a suppressed or informational entry), to distinguish between "rule fires but is suppressed" and "rule never runs during build".

6. Search the NetAnalyzers analyzer source for target-framework guards or `<LangVersion>` conditions inside the CA1802 implementation (e.g., attributes or `#if` blocks) that might cause the diagnostic to be skipped for the target framework version used by the test harness.

7. If steps 1–6 confirm that CA1802 is permanently excluded from build enforcement, update the `Untestable` reason with the specific source location or GitHub issue link that proves it, so the documented reason is authoritative and reproducible.

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
