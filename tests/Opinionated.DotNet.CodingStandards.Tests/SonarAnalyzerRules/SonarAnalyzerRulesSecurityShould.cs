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

    [Fact]
    [RuleDoc("S2612", "File permissions should not be set to world-accessible values",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2612/")]
    public async Task WarnOnWorldAccessibleFilePermissions()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            using System.Security.AccessControl;
            using System.Security.Principal;

            public static class FilePermissionExample
            {
                public static void GrantWorldAccess()
                {
                    var fileSecurity = new FileSecurity();
                    var rule = new FileSystemAccessRule(
                        new NTAccount("Everyone"),
                        FileSystemRights.FullControl,
                        AccessControlType.Allow);
                    fileSecurity.AddAccessRule(rule);
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2612").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2755", "XML parsers should not be vulnerable to XXE attacks",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2755/")]
    public async Task WarnOnXxeVulnerableXmlParser()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Xml;
            namespace test;
            public static class Vulnerable
            {
                public static void Parse()
                {
                    var parser = new XmlDocument();
                    parser.XmlResolver = new XmlUrlResolver(); // Noncompliant: enables external entity resolution
                    parser.LoadXml("<root/>");
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2755").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3011", "Reflection should not be used to increase accessibility of classes, methods, or fields",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3011/")]
    public async Task ProhibitReflectionAccessibilityBypass()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Reflection;
            namespace test;
            public class C
            {
                public MethodInfo? GetPrivateMethod(Type t) =>
                    t.GetMethod("secret", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3011").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3330", "Creating cookies without the \"HttpOnly\" flag is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3330/")]
    public async Task WarnOnCookieWithoutHttpOnlyFlag()
    {
        // S3330 is a Sonar security hotspot — it only fires when the rule is listed
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
                  <Key>S3330</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFile("Program.cs", """
            using Microsoft.AspNetCore.Http;

            namespace test;

            public static class Program
            {
                public static CookieOptions CreateUnsafe() => new CookieOptions { HttpOnly = false };

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3330").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3884", "CoSetProxyBlanket and CoInitializeSecurity should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3884/")]
    public async Task WarnOnCoSetProxyBlanketAndCoInitializeSecurity()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Runtime.InteropServices;

            namespace test;

            public static class Program
            {
                [DllImport("ole32.dll")]
                [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
                public static extern int CoSetProxyBlanket(
                    nint pProxy,
                    uint dwAuthnSvc,
                    uint dwAuthzSvc,
                    nint pServerPrincName,
                    uint dwAuthnLevel,
                    uint dwImpLevel,
                    nint pAuthInfo,
                    uint dwCapabilities);

                [DllImport("ole32.dll")]
                [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
                public static extern int CoInitializeSecurity(
                    nint pSecDesc,
                    int cAuthSvc,
                    nint asAuthSvc,
                    nint pReserved1,
                    uint dwAuthnLevel,
                    uint dwImpLevel,
                    nint pAuthList,
                    uint dwCapabilities,
                    nint pReserved3);

                public static int Main()
                {
                    var r1 = CoInitializeSecurity(nint.Zero, -1, nint.Zero, nint.Zero, 0, 0, nint.Zero, 0, nint.Zero);
                    var r2 = CoSetProxyBlanket(nint.Zero, 0, 0, nint.Zero, 0, 0, nint.Zero, 0);
                    return r1 + r2;
                }
            }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3884").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4000", "Pointers to unmanaged memory should not be visible",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4000/")]
    public async Task ProhibitPublicUnmanagedPointerMembers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyResource
            {
                public System.IntPtr myPointer;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S4000").ShouldBeTrue();
    }
}
