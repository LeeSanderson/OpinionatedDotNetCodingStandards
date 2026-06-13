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

    [Fact]
    [RuleDoc("S2115", "A secure password should be used when connecting to a database",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2115/")]
    public async Task WarnOnEmptyDatabasePassword()
    {
        using var project = await CreateProjectBuilder(
            packageReferences:
            [
                (Name: "Microsoft.EntityFrameworkCore", Version: "8.0.0"),
                (Name: "Microsoft.EntityFrameworkCore.SqlServer", Version: "8.0.0")
            ]);
        await project.AddFile("Program.cs", """
            using Microsoft.EntityFrameworkCore;

            namespace test;

            public sealed class BadContext : DbContext
            {
                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    optionsBuilder.UseSqlServer("Server=myServer;Database=myDb;User Id=sa;Password=");
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2115").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2245", "Using pseudorandom number generators (PRNGs) is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2245/")]
    public async Task WarnOnPseudorandomNumberGeneratorUsage()
    {
        // S2245 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilder(additionalFiles: ["SonarLint.xml"]);
        await project.AddFile("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2245</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Randomizer
            {
                public static int GetValue()
                {
                    var rng = new System.Random();
                    return rng.Next();
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2245").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2257", "Using non-standard cryptographic algorithms is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2257/")]
    public async Task WarnOnNonstandardCryptographicAlgorithm()
    {
        // S2257 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilder(additionalFiles: ["SonarLint.xml"]);
        await project.AddFile("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2257</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFile("Program.cs", """
            using System.Security.Cryptography;

            namespace test;

            public class CustomHashAlgorithm : HashAlgorithm
            {
                public override void Initialize() => HashValue = new byte[20];
                protected override void HashCore(byte[] array, int ibStart, int cbSize) { }
                protected override byte[] HashFinal() => HashValue!;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2257").ShouldBeTrue();
    }
}
