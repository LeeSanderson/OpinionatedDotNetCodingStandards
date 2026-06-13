// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesSecurityShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1313", "Using hardcoded IP addresses is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1313/")]
    public async Task WarnOnHardcodedIpAddress()
    {
        // S1313 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilder(additionalFiles: ["SonarLint.xml"]);
        await project.AddFile("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S1313</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                private static string ServerAddress { get; } = "192.168.1.100";

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1313").ShouldBeTrue();
    }
}
