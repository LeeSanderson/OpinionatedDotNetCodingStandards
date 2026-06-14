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
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S1313</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                private static string ServerAddress { get; } = "192.168.1.100";

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1313").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2068", "Credentials should not be hard-coded",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2068/")]
    public async Task WarnOnHardcodedCredentials()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Config
            {
                private static string password = "secret123";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

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
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.AspNetCore.Http", Version: "2.3.10")],
            additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2092</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Http;

            namespace test;

            public static class Program
            {
                public static CookieOptions CreateUnsafe() => new CookieOptions();

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2092").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2115", "A secure password should be used when connecting to a database",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2115/")]
    public async Task WarnOnEmptyDatabasePassword()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences:
            [
                (Name: "Microsoft.EntityFrameworkCore", Version: "8.0.0"),
                (Name: "Microsoft.EntityFrameworkCore.SqlServer", Version: "8.0.0")
            ]);
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2115").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2245", "Using pseudorandom number generators (PRNGs) is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2245/")]
    public async Task WarnOnPseudorandomNumberGeneratorUsage()
    {
        // S2245 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2245</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2245").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2257", "Using non-standard cryptographic algorithms is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2257/")]
    public async Task WarnOnNonstandardCryptographicAlgorithm()
    {
        // S2257 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S2257</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2257").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2612", "File permissions should not be set to world-accessible values",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2612/")]
    public async Task WarnOnWorldAccessibleFilePermissions()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2612").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2755", "XML parsers should not be vulnerable to XXE attacks",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2755/")]
    public async Task WarnOnXxeVulnerableXmlParser()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2755").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3011", "Reflection should not be used to increase accessibility of classes, methods, or fields",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3011/")]
    public async Task ProhibitReflectionAccessibilityBypass()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Reflection;
            namespace test;
            public class C
            {
                public MethodInfo? GetPrivateMethod(Type t) =>
                    t.GetMethod("secret", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

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
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.AspNetCore.Http", Version: "2.3.10")],
            additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S3330</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Http;

            namespace test;

            public static class Program
            {
                public static CookieOptions CreateUnsafe() => new CookieOptions { HttpOnly = false };

                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3330").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3884", "CoSetProxyBlanket and CoInitializeSecurity should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3884/")]
    public async Task WarnOnCoSetProxyBlanketAndCoInitializeSecurity()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3884").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4000", "Pointers to unmanaged memory should not be visible",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4000/")]
    public async Task ProhibitPublicUnmanagedPointerMembers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyResource
            {
                public System.IntPtr myPointer;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4000").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4036", "Searching OS commands in PATH is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4036/")]
    public async Task WarnOnUnsafeOsCommandPathSearch()
    {
        // S4036 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S4036</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics;
            namespace test;
            public static class Launcher
            {
                public static void Run()
                {
                    // Bare command name with no path prefix — relies on PATH resolution
                    Process.Start("cmd");
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4036").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4211", "Members should not have conflicting transparency annotations",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4211/")]
    public async Task WarnOnConflictingTransparencyAnnotations()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Security;

            namespace test;

            [SecurityCritical]
            public class SecureService
            {
                private readonly int _value = 1;

                [SecuritySafeCritical]
                public int Execute() => _value;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4211").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4212", "Serialization constructors should be secured",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4212/")]
    public async Task WarnOnUnsecuredSerializationConstructor()
    {
        using var project = await CreateProjectBuilderAsync(
            properties: [(Name: "NoWarn", Value: "SYSLIB0003;SYSLIB0051")],
            packageReferences: [(Name: "System.Security.Permissions", Version: "10.0.9")]);
        await project.AddFileAsync("Program.cs", """
            // Requires packageReferences: [("System.Security.Permissions", "10.0.9")]
            // and properties: [("NoWarn", "SYSLIB0003;SYSLIB0051")]
            //
            // REFUTATION: The claim that CAS types are absent from net10.0 is wrong.
            // System.Security.AllowPartiallyTrustedCallersAttribute lives in System.Runtime (no extra package needed).
            // System.Security.Permissions.CodeAccessSecurityAttribute and its subtypes (FileIOPermissionAttribute, etc.)
            // are in the System.Security.Permissions NuGet package (v10.0.9), which targets net10.0.
            // All three predicates in FindPossibleViolations can be satisfied:
            //   1) isAssemblyIsPartiallyTrusted — [assembly: AllowPartiallyTrustedCallers] resolves at net10.0.
            //   2) IsSerializationConstructor — SerializationInfo + StreamingContext exist in net10.0.
            //   3) isConstructorMissingAttributes — public Foo() has [FileIOPermissionAttribute] (a CAS subtype);
            //      the serialization constructor lacks it, so the diagnostic fires.
            // The types are marked [Obsolete(SYSLIB0003)] but that is a warning, not a compile error;
            // NoWarn=SYSLIB0003 keeps the build green and lets the SARIF diagnostic surface.
            // The rule is only silent in this harness because the package editorconfig sets
            // dotnet_diagnostic.S4212.severity = none (deprecated rule). Promoting it to warning unblocks it.

            using System;
            using System.Runtime.Serialization;
            using System.Security;
            using System.Security.Permissions;

            [assembly: AllowPartiallyTrustedCallersAttribute()]
            namespace MyLibrary
            {
                [Serializable]
                public class Foo : ISerializable
                {
                    private int n;

                    [FileIOPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
                    public Foo()
                    {
                        n = -1;
                    }

                    // Noncompliant: serialization constructor is missing the [FileIOPermission] CAS attribute
                    // that the regular constructor carries. S4212 fires here.
                    protected Foo(SerializationInfo info, StreamingContext context)
                    {
                        n = (int)info.GetValue("n", typeof(int));
                    }

                    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
                    {
                        info.AddValue("n", n);
                    }
                }

                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4212").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4426", "Cryptographic keys should be robust",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4426/")]
    public async Task WarnOnWeakCryptographicKeySize()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Security.Cryptography;
            namespace test;
            public static class WeakKeyExample
            {
                public static RSACryptoServiceProvider Create() => new RSACryptoServiceProvider(1024);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4426").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4433", "LDAP connections should be authenticated",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4433/")]
    public async Task WarnOnUnauthenticatedLdapConnection()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.DirectoryServices", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using System.DirectoryServices;
            public static class LdapConfig
            {
                public static void Configure()
                {
                    var entry = new DirectoryEntry();
                    entry.AuthenticationType = AuthenticationTypes.None;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4433").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4507", "Delivering code in production with debug features activated is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4507/")]
    public async Task WarnOnDebugFeaturesInProduction()
    {
        // S4507 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        // The analyzer checks for calls to UseDeveloperExceptionPage() that are not
        // guarded by an IsDevelopment() check in the same method scope, and where the
        // class is not named StartupDevelopment.
        // Stub types work here because MemberDescriptor resolves by FQN via SemanticModel.
        using var project = await CreateProjectBuilderAsync(additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S4507</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            namespace Microsoft.AspNetCore.Builder
            {
                public interface IApplicationBuilder { }

                public static class DeveloperExceptionPageExtensions
                {
                    public static IApplicationBuilder UseDeveloperExceptionPage(
                        this IApplicationBuilder app) => app;
                }
            }

            namespace test
            {
                using Microsoft.AspNetCore.Builder;

                public class Startup
                {
                    // Noncompliant: UseDeveloperExceptionPage() called unconditionally,
                    // no IsDevelopment() guard in this method scope, class is not StartupDevelopment.
                    public void Configure(IApplicationBuilder app)
                    {
                        app.UseDeveloperExceptionPage();
                    }
                }

                public static class Program { public static int Main() => 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4507").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4502", "Disabling CSRF protections is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4502/")]
    public async Task WarnOnDisabledCsrfProtection()
    {
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
                  <Key>S4502</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Mvc;

            namespace test;

            [IgnoreAntiforgeryToken]
            public class SomeController : Controller { }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4502").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4792", "Configuring loggers is security-sensitive",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4792/")]
    public async Task WarnOnLoggerConfiguration()
    {
        // S4792 is a Sonar security hotspot — it only fires when the rule is listed
        // in a SonarLint.xml passed as an AdditionalFile (simulating Sonar Scanner mode).
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "log4net", Version: "2.0.17")],
            properties:
            [
                (Name: "NuGetAudit", Value: "false"),
                (Name: "NoWarn", Value: "NU1902;NU1903")
            ],
            additionalFiles: ["SonarLint.xml"]);
        await project.AddFileAsync("SonarLint.xml", """
            <?xml version="1.0" encoding="UTF-8"?>
            <AnalysisInput>
              <Rules>
                <Rule>
                  <Key>S4792</Key>
                </Rule>
              </Rules>
            </AnalysisInput>
            """);
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Setup
            {
                public static void ConfigureLogging() => log4net.Config.XmlConfigurator.Configure();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4792").ShouldBeTrue();
    }
}
