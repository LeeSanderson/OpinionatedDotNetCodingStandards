## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA3075 ("Insecure DTD processing in XML") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitInsecureDtdProcessingInXml` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlReaderSettings { DtdProcessing = DtdProcessing.Parse } + XmlReader.Create patterns; the XmlTextReader approach requires System.Xml.XmlTextReader which is not in .NET 10"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA3075", "Insecure DTD processing in XML",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3075",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlReaderSettings { DtdProcessing = DtdProcessing.Parse } + XmlReader.Create patterns; the XmlTextReader approach requires System.Xml.XmlTextReader which is not in .NET 10")]
    public async Task ProhibitInsecureDtdProcessingInXml()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Xml;
            namespace test;
            public static class Program
            {
                public static void LoadXml(string path)
                {
                    var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
                    using var reader = XmlReader.Create(path, settings);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA3075").ShouldBeTrue();
    }
```

## Investigation plan

1. Try alternative violation patterns that CA3075 is documented to cover: use `XmlDocument.Load` with a path string, use `XmlDocument` with `XmlResolver` set to a non-null value, and use `XmlTextReader` constructed directly (even if deprecated) — compile each pattern in the test harness and check whether CA3075 fires on any of them.

2. Check the NetAnalyzers GitHub source for the CA3075 analyzer implementation (file `InsecureDtdProcessingAnalyzer.cs` or similar) to identify which specific API patterns are checked and whether any target-framework guards (e.g., `#if NETCOREAPP` or `IsSymbol` checks for types absent in .NET 10) cause the analysis to short-circuit silently.

3. Check the `EnforceOnBuild` metadata for CA3075 in the NetAnalyzers package (look in `Microsoft.CodeAnalysis.NetAnalyzers.props` or the SARIF/ruleset metadata shipped with the NuGet package) to confirm the rule is set to `WhenExplicitlyConfigured` or `Never` — either value would explain why no diagnostic appears even with a matching pattern.

4. Inspect the raw MSBuild/compiler output from `BuildAndGetOutput` for the current test pattern (without the `HasError` assertion) to confirm whether CA3075 is absent from the SARIF output entirely, present as a warning rather than an error, or suppressed by another rule — this distinguishes "rule never fires" from "rule fires but at warning severity".

5. Try running the same violation code against an older NetAnalyzers package (8.0.x) by temporarily passing a `packageReferences` override to `CreateProjectBuilder` (if supported), or by manually editing the generated project file, to determine whether the rule regressed between analyzer versions or was never build-enforced.

6. Search the NetAnalyzers GitHub issue tracker and changelog for CA3075 + .NET 10 / .NET Core to find any recorded decision to disable or downgrade the rule for non-Framework targets, and record the issue URL as the sourced reason if found.

7. If no pattern fires and no official issue explains the gap, document the confirmed root cause (e.g., "CA3075 has `EnforceOnBuild = Never` in NetAnalyzers 10.0.x" or "all triggering APIs are absent from .NET 10 BCL") with the specific source file path or GitHub permalink, then update the `Untestable` string in the `[RuleDoc]` attribute accordingly.

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
