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

    [Fact]
    [RuleDoc("S2068", "Credentials should not be hard-coded",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2068/")]
    public async Task WarnOnHardcodedCredentials()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Config
            {
                private static string password = "secret123";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2068").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2092", "Creating cookies without the \"secure\" flag is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2092/")]
    public async Task WarnOnCookieWithoutSecureFlag()
    {
        // S2092 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        // CookieOptions lives in Microsoft.AspNetCore.Http (standalone 2.x package).
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "Microsoft.AspNetCore.Http", Version: "2.3.10")],
            additionalFiles: ["SonarLint.xml"]);
        await project.AddFile("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2092</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFile("Program.cs", """
            using Microsoft.AspNetCore.Http;

            namespace test;

            public static class Program
            {
                public static CookieOptions CreateUnsafe() => new CookieOptions();

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2092").ShouldBeTrue();
    }
}
