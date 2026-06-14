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

    [Fact]
    [RuleDoc("S5753", "Disabling ASP.NET Request Validation feature is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-5753/")]
    public async Task WarnOnDisabledRequestValidation()
    {
        // S5753 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S5753</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Stubs.cs", """
            // ReSharper disable All
            #pragma warning disable
            namespace System.Web.Mvc
            {
                [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
                public sealed class ValidateInputAttribute : Attribute
                {
                    public ValidateInputAttribute(bool enableValidation) { EnableValidation = enableValidation; }
                    public bool EnableValidation { get; }
                }
            }
            #pragma warning restore
            """);
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class HomeController
            {
                [System.Web.Mvc.ValidateInput(false)]
                public string Index() => string.Empty;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S5753").ShouldBeTrue();
    }
}
