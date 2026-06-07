## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA3077 ("Insecure Processing in API Design, XmlDocument and XmlTextReader") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitInsecureXmlDocumentResolver` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlDocument with XmlResolver + LoadXml patterns; appears to require specific API design patterns (method accepting XmlDocument parameter) not replicable in a simple harness"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA3077", "Insecure Processing in API Design, XmlDocument and XmlTextReader",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3077",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlDocument with XmlResolver + LoadXml patterns; appears to require specific API design patterns (method accepting XmlDocument parameter) not replicable in a simple harness")]
    public async Task ProhibitInsecureXmlDocumentResolver()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Xml;
            namespace test;
            public static class Program
            {
                public static void LoadXml(string xml)
                {
                    var doc = new XmlDocument();
                    doc.XmlResolver = new XmlUrlResolver();
                    doc.LoadXml(xml);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA3077").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the NetAnalyzers source for the CA3077 analyzer (look for `XmlDocumentAnalyzer` or `InsecureXmlAnalyzer` in the `roslyn-analyzers` GitHub repo) and verify whether the rule requires the tainted data to flow across a method boundary — i.e. a method that accepts an `XmlDocument` or `XmlTextReader` parameter — as its trigger condition, rather than inline construction within a single method body.

2. Try alternative same-method violation patterns that more closely match the documented trigger: write a test method whose signature accepts an `XmlDocument` parameter with an insecure `XmlResolver` already set, or a method that accepts a raw `string` and passes it to `XmlDocument.Load` without sanitising the resolver, to see if cross-boundary taint is the key requirement.

3. Confirm whether the rule is data-flow / taint-tracking by examining the `EnforceOnBuild` metadata in the analyzer's `.editorconfig` or SARIF descriptor: if `EnforceOnBuild = Never` or the severity is `suggestion`, the rule may not fire during `dotnet build` at all regardless of the code pattern.

4. Check the NetAnalyzers GitHub issues and changelog for CA3077 to determine whether the rule was intentionally scoped to "API design" patterns only (i.e. public methods accepting `XmlDocument`/`XmlTextReader` as parameters) and whether inline usage was deliberately excluded from the diagnostic.

5. Try a pattern that exactly matches the Microsoft documentation example — a public method with an `XmlDocument` parameter whose `XmlResolver` is set to `new XmlUrlResolver()` inside the method — and run the test harness against it to confirm or deny that the rule fires for this specific shape.

6. Test with NetAnalyzers 8.x (pin the `Microsoft.CodeAnalysis.NetAnalyzers` package version in `CreateProjectBuilder`) to determine whether the rule fired in an earlier version and was subsequently changed or removed, which would confirm a version regression rather than a pattern mismatch.

7. If none of the above patterns produce a diagnostic, search the analyzer source for any target-framework guard (e.g. `#if NET` or a `compilationStartContext.RegisterOperationAction` conditional) that could suppress the rule on .NET 9+ or .NET 10 targets used by the test harness.

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
