using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesSecurityShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA2352", "Unsafe DataSet or DataTable in serializable type can be vulnerable to remote code execution attacks",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2352")]
    public async Task ProhibitDataSetInSerializableType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Data;
            namespace test;
            [Serializable]
            public class MyData
            {
                public DataSet Dataset { get; set; } = new DataSet();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2352").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2353", "Unsafe DataSet or DataTable in serializable type",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2353")]
    public async Task ProhibitDataSetInDataContractType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            using System.Runtime.Serialization;
            namespace test;
            [DataContract]
            public class MyData
            {
                [DataMember]
                public DataSet Dataset { get; set; } = new DataSet();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2353").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5350", "Do Not Use Weak Cryptographic Algorithms",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5350")]
    public async Task ProhibitWeakCryptographicAlgorithms()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var tdes = TripleDES.Create();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5350").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5351", "Do Not Use Broken Cryptographic Algorithms",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5351")]
    public async Task ProhibitBrokenCryptographicAlgorithms()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var md5 = MD5.Create();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5351").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5358", "Review cipher mode usage with cryptography experts",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5358")]
    public async Task RequireSecureCipherMode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var aes = Aes.Create();
                    aes.Mode = CipherMode.ECB;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5358").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5359", "Do Not Disable Certificate Validation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5359")]
    public async Task RequireCertificateValidation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Net;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    #pragma warning disable SYSLIB0014
                    ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) => true;
                    #pragma warning restore SYSLIB0014
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5359").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5361", "Do Not Disable SChannel Use of Strong Crypto",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5361")]
    public async Task RequireSchannelStrongCrypto()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    AppContext.SetSwitch("Switch.System.Net.DontEnableSchUseStrongCrypto", true);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5361").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5364", "Do Not Use Deprecated Security Protocols",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5364")]
    public async Task ProhibitDeprecatedSecurityProtocols()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Net;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    #pragma warning disable SYSLIB0014
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    #pragma warning restore SYSLIB0014
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5364").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5366", "Use XmlReader for 'DataSet.ReadXml()'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5366")]
    public async Task RequireXmlReaderForDataSetReadXml()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var ds = new DataSet();
                    ds.ReadXml("file.xml");
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5366").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5369", "Use XmlReader for 'XmlSerializer.Deserialize()'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5369")]
    public async Task RequireXmlReaderForXmlSerializerDeserialize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Xml.Serialization;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var serializer = new XmlSerializer(typeof(string));
                    serializer.Deserialize(Stream.Null);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5369").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5371", "Use XmlReader for 'XmlSchema.Read()'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5371")]
    public async Task RequireXmlReaderForXmlSchemaRead()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Xml.Schema;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    XmlSchema.Read(new StringReader("<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"/>"), null);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5371").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5372", "Use XmlReader for XPathDocument constructor",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5372")]
    public async Task RequireXmlReaderForXPathDocument()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Xml.XPath;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    _ = new XPathDocument("http://example.com/data.xml");
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5372").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5378", "Do not disable ServicePointManagerSecurityProtocols",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5378")]
    public async Task RequireServiceModelSecurityProtocols()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    AppContext.SetSwitch("Switch.System.ServiceModel.DisableUsingServicePointManagerSecurityProtocols", true);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5378").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5379", "Ensure Key Derivation Function algorithm is sufficiently strong",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5379")]
    public async Task RequireStrongKeyDerivationAlgorithm()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var salt = new byte[16];
                    using var kdf = new Rfc2898DeriveBytes("password", salt, 100000, HashAlgorithmName.SHA1);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5379").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5380", "Do Not Add Certificates To Root Store",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5380")]
    public async Task ProhibitAddingCertificatesToRootStore()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography.X509Certificates;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);
                    var cert = X509CertificateLoader.LoadCertificate(new byte[] { 0x30 });
                    store.Add(cert);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5380").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5386", "Avoid hardcoding SecurityProtocolType value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5386")]
    public async Task ProhibitHardcodedSecurityProtocolType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Net;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    #pragma warning disable SYSLIB0014
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    #pragma warning restore SYSLIB0014
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5386").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5390", "Do not hard-code encryption key",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5390")]
    public async Task ProhibitHardcodedEncryptionKey()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var aes = Aes.Create();
                    var key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                    aes.CreateEncryptor(key, null);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5390").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5392", "Use DefaultDllImportSearchPaths attribute for P/Invokes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5392")]
    public async Task RequireDefaultDllImportSearchPathsOnPInvoke()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public static class Program
            {
                [DllImport("kernel32.dll")]
                private static extern bool CloseHandle(nint hObject);

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5392").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5393", "Do not use unsafe DllImportSearchPath value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5393")]
    public async Task ProhibitUnsafeDllImportSearchPath()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public static class Program
            {
                [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
                [DllImport("kernel32.dll")]
                private static extern bool CloseHandle(nint hObject);

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5393").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5397", "Do not use deprecated SslProtocols values",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5397")]
    public async Task ProhibitDeprecatedSslProtocols()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Net.Security;
            using System.Security.Authentication;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var ssl = new SslStream(Stream.Null);
                    #pragma warning disable SYSLIB0039
                    ssl.AuthenticateAsClient("server", null, SslProtocols.Tls, false);
                    #pragma warning restore SYSLIB0039
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5397").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5398", "Avoid hardcoded SslProtocols values",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5398")]
    public async Task ProhibitHardcodedSslProtocolsValues()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Net.Security;
            using System.Security.Authentication;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var ssl = new SslStream(Stream.Null);
                    ssl.AuthenticateAsClient("server", null, SslProtocols.Tls12, false);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5398").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5401", "Do not use CreateEncryptor with non-default IV",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5401")]
    public async Task ProhibitCreateEncryptorWithNonDefaultIv()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    using var aes = Aes.Create();
                    aes.CreateEncryptor(aes.Key, new byte[16]);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5401").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA5403", "Do not hard-code certificate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5403")]
    public async Task ProhibitHardcodedCertificate()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography.X509Certificates;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var certData = new byte[] { 0x30, 0x82, 0x01, 0x00, 0x00 };
                    #pragma warning disable SYSLIB0057
                    var cert = new X509Certificate2(certData);
                    #pragma warning restore SYSLIB0057
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5403").ShouldBeTrue();
    }
}
