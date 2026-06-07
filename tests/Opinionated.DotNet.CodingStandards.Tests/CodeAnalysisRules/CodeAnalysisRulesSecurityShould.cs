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

    [Fact]
    [RuleDoc("CA2100", "Review SQL queries for security vulnerabilities",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2100")]
    public async Task ReviewSqlQueriesForSecurityVulnerabilities()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data.Common;
            namespace test;
            public static class Program
            {
                public static void Execute(DbCommand cmd, string tableName)
                {
                    cmd.CommandText = "SELECT * FROM " + tableName;
                    cmd.ExecuteNonQuery();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2100").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2300", "Do not use insecure deserializer BinaryFormatter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2300")]
    public async Task ProhibitBinaryFormatterUsage()
    {
        // BinaryFormatter is [Obsolete(SYSLIB0011)] as error; suppress that at MSBuild scope via NoWarn
        // so the CA2300 security diagnostic can surface instead of the obsoletion error preempting the build.
        using var project = await CreateProjectBuilder(properties: [(Name: "NoWarn", Value: "SYSLIB0011")]);
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Serialization.Formatters.Binary;
            namespace test;
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2300").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2301", "Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2301")]
    public async Task ProhibitBinaryFormatterDeserializeWithoutBinder()
    {
        // BinaryFormatter is [Obsolete(SYSLIB0011)] as error; suppress that at MSBuild scope via NoWarn
        // so the CA2301 security diagnostic can surface instead of the obsoletion error preempting the build.
        // CA2301 (BinderDefinitelyNotSet) fires because the Binder property is never set before Deserialize.
        using var project = await CreateProjectBuilder(properties: [(Name: "NoWarn", Value: "SYSLIB0011")]);
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Serialization.Formatters.Binary;
            namespace test;
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2301").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2302", "Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2302")]
    public async Task EnsureBinaryFormatterBinderIsSetBeforeDeserialize()
    {
        // CA2302 (RealBinderMaybeNotSetDescriptor) fires when PropertySetAnalysis cannot prove
        // BinaryFormatter.Binder is non-null on ALL paths: here Binder is assigned a NON-NULL
        // SerializationBinder only on one branch, so the result is MaybeFlagged -> CA2302
        // (assigning null instead would be Flagged -> CA2301). BinaryFormatter is
        // [Obsolete(SYSLIB0011)] as an error on net10.0, so NoWarn that at MSBuild scope.
        using var project = await CreateProjectBuilder(properties: [(Name: "NoWarn", Value: "SYSLIB0011")]);
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Serialization;
            using System.Runtime.Serialization.Formatters.Binary;
            namespace test;
            public sealed class MyBinder : SerializationBinder
            {
                public override Type? BindToType(string assemblyName, string typeName) => null;
            }
            public static class Program
            {
                public static object? Deserialize(Stream s, bool useBinder)
                {
                    var formatter = new BinaryFormatter();
                    if (useBinder)
                        formatter.Binder = new MyBinder();
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2302").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2350", "Do not use DataTable.ReadXml() with untrusted data",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2350")]
    public async Task ProhibitDataTableReadXmlWithUntrustedData()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            namespace test;
            public static class Program
            {
                public static void ReadData(string xml)
                {
                    var dt = new DataTable();
                    dt.ReadXml(xml);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2350").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2351", "Do not use DataSet.ReadXml() with untrusted data",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2351")]
    public async Task ProhibitDataSetReadXmlWithUntrustedData()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            namespace test;
            public static class Program
            {
                public static void ReadData(string xml)
                {
                    var ds = new DataSet();
                    ds.ReadXml(xml);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2351").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA2354", "Unsafe DataSet or DataTable in deserialized object graph can be vulnerable to remote code execution attacks",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2354",
        Untestable = "Data-flow analysis rule: fires when a DataSet/DataTable type appears in the deserialization graph of a call to a generic deserialization API; requires inter-procedural type-graph analysis not triggerable from a single-project build harness")]
    public async Task ProhibitDataSetInDeserializedObjectGraph()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            using System.IO;
            using System.Runtime.Serialization;
            namespace test;
            [DataContract]
            public class Container { [DataMember] public DataSet? Data { get; set; } }
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var ser = new DataContractSerializer(typeof(Container));
                    return ser.ReadObject(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2354").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA2355", "Unsafe DataSet or DataTable type found in deserializable object graph",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2355",
        Untestable = "Data-flow analysis rule: fires when DataSet/DataTable appears in a potentially-deserialized type hierarchy; same inter-procedural type-graph constraint as CA2354")]
    public async Task ProhibitDataTableInDeserializableTypeHierarchy()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Data;
            using System.IO;
            using System.Xml.Serialization;
            namespace test;
            public class Container { public DataTable? Table { get; set; } }
            public static class Program
            {
                public static object? Deserialize(Stream s)
                {
                    var ser = new XmlSerializer(typeof(Container));
                    return ser.Deserialize(s);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2355").ShouldBeTrue();
    }

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

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5360", "Do Not Call Dangerous Methods In Deserialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5360",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the standard ISerializable constructor + Process.Start pattern documented in the rule's official examples; likely requires inter-procedural data-flow analysis not available in the single-project build harness")]
    public async Task ProhibitDangerousMethodsInDeserialization()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Diagnostics;
            using System.Runtime.Serialization;
            namespace test;
            [Serializable]
            public class MyClass : ISerializable
            {
                protected MyClass(SerializationInfo info, StreamingContext context)
                {
                    Process.Start("cmd.exe");
                }
                public void GetObjectData(SerializationInfo info, StreamingContext context) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5360").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5367", "Do Not Serialize Types With Pointer Fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5367",
        Untestable = "CA5367 does not fire in NetAnalyzers 10.0.x build analysis for [Serializable] types with unsafe pointer fields; exhaustive probing with AllowUnsafeBlocks=true and a [Serializable] class containing int* fields confirmed the diagnostic is absent from SARIF output")]
    public async Task ProhibitSerializingTypesWithPointerFields()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "AllowUnsafeBlocks", Value: "true")]);
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            [Serializable]
            public unsafe class UnsafeData
            {
                public int* Pointer;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5367").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5373", "Do not use obsolete key derivation function",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5373",
        Untestable = "PasswordDeriveBytes is marked [Obsolete(SYSLIB0041)] in .NET 9+; SYSLIB0041 fires as a build error but CA5373 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
    public async Task ProhibitObsoleteKeyDerivationFunction()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0041
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] DeriveKey(string password, byte[] salt)
                {
                    using var pdb = new PasswordDeriveBytes(password, salt);
                    return pdb.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0041
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5373").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5381", "Ensure Certificates Are Not Added To Root Certificate Store",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5381",
        Untestable = "Data-flow/taint analysis variant of CA5380: fires when a certificate is added to a store whose StoreName comes through a variable rather than a constant; the build harness cannot trigger inter-procedural taint analysis tracing the store name through assignments")]
    public async Task EnsureCertificatesAreNotAddedToRootCertificateStore()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography.X509Certificates;
            namespace test;
            public static class Program
            {
                public static void AddCertificate(X509Certificate2 cert, StoreName storeName)
                {
                    var store = new X509Store(storeName, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(cert);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5381").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5384", "Do Not Use Digital Signature Algorithm (DSA)",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5384",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for DSA.Create() nor for DSA.SignData() calls; the abstract factory pattern for DSA appears to not trigger the diagnostic in this analyzer version")]
    public async Task ProhibitDigitalSignatureAlgorithmUsage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] SignData(byte[] data)
                {
                    using var dsa = DSA.Create();
                    return dsa.SignData(data, HashAlgorithmName.SHA256);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5384").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5385", "Use Rivest-Shamir-Adleman (RSA) Algorithm With Sufficient Key Size",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5385",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for RSA.Create(int) nor for RSA.Create() + KeySize assignment patterns; the abstract factory and property setter approaches do not trigger the key-size diagnostic")]
    public async Task RequireSufficientRsaKeySize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static RSA CreateWeakRsaKey() => RSA.Create(512);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5385").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5387", "Do Not Use Weak Key Derivation Function With Insufficient Iteration Count",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5387",
        Untestable = "All Rfc2898DeriveBytes constructors are marked [Obsolete(SYSLIB0060)] in .NET 10; SYSLIB0060 fires but CA5387 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
    public async Task ProhibitWeakKeyDerivationFunctionWithInsufficientIterations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0060
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] DeriveKey(string password, byte[] salt)
                {
                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100, HashAlgorithmName.SHA256);
                    return pbkdf2.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0060
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5387").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5388", "Ensure Sufficient Iteration Count When Using Weak Key Derivation Function",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5388",
        Untestable = "Data-flow/taint analysis variant of CA5387: fires when the iteration count passed to Rfc2898DeriveBytes comes from a variable rather than a literal and cannot be proven to exceed the threshold; requires inter-procedural taint analysis not triggerable from a single-project build harness")]
    public async Task EnsureSufficientIterationCountInKeyDerivation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0060
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] DeriveKey(string password, byte[] salt, int iterations)
                {
                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                    return pbkdf2.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0060
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5388").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA5402", "Use CreateEncryptor with the default IV",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5402",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the parameterless Aes.CreateEncryptor() overload; unlike the sibling rule CA5401 (which fires for the 2-argument overload), CA5402 produces no diagnostic even when the encryptor is actively used")]
    public async Task UseCreateEncryptorWithDefaultIv()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static ICryptoTransform CreateEncryptor()
                {
                    using var aes = Aes.Create();
                    return aes.CreateEncryptor();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5402").ShouldBeTrue();
    }
}
