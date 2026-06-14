// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesSecurity2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S5693", "Allowing requests with excessive content length is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-5693/")]
    public async Task WarnOnDisabledRequestSizeLimit()
    {
        // S5693 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.AspNetCore.Mvc", Version: "2.3.10")],
            properties:
            [
                (Name: "NuGetAudit", Value: "false"),
                (Name: "NoWarn", Value: "NU1903;NU1902;CA1515;CA1822")
            ],
            additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S5693</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Mvc;
            namespace test;
            public class UploadController : Controller
            {
                [HttpPost]
                [DisableRequestSizeLimit]
                public IActionResult Upload() => Ok();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S5693").ShouldBeTrue();
    }
}
