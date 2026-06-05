using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesPerformanceShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA1805", "Do not initialize unnecessarily",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1805")]
    public async Task RequireDoNotInitializeUnnecessarily()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static int _value = 0;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1805").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1806", "Do not ignore method results",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1806")]
    public async Task RequireNotIgnoringMethodResults()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void Method() 
                {
                    "test".Clone();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1806").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1810", "Initialize reference type static fields inline",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1810")]
    public async Task RequireInlineStaticFieldInitialization()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Config
            {
                public static readonly string Value;
                static Config() { Value = "hello"; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1810").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1813", "Avoid unsealed attributes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1813")]
    public async Task RequireSealedAttributes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class MyAttribute : System.Attribute { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1813").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1814", "Prefer jagged arrays over multidimensional",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1814")]
    public async Task PreferJaggedArraysOverMultidimensional()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Grid
            {
                public int[,] Matrix = new int[3, 3];
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1814").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1815", "Override equals and operator equals on value types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1815")]
    public async Task RequireEqualsOverrideOnValueTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public struct Point { public int X; public int Y; }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1815").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1816", "Dispose methods should call SuppressFinalize",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1816")]
    public async Task RequireDisposeCallsSuppressFinalize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IDisposable
            {
                public void Dispose() 
                {
                    // Missing GC.SuppressFinalize(this)
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1816").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1820", "Test for empty strings using string length",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1820")]
    public async Task RequireStringLengthForEmptyStringTests()
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
                    string s = "hello";
                    if (s.Equals(string.Empty)) { }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1820").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1821", "Remove empty Finalizers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1821")]
    public async Task ProhibitEmptyFinalizers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass { ~MyClass() { } }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1821").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1823", "Avoid unused private fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1823")]
    public async Task ProhibitUnusedPrivateFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private string _unused = "value";
                public string GetValue() => "other";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1823").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1825", "Avoid zero-length array allocations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1825")]
    public async Task RequireAvoidZeroLengthArrays()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int[] GetArray() => new int[0];
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1825").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1826", "Do not use Enumerable methods on indexable collections",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1826")]
    public async Task RequireIndexerOverLinq()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int Get() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.First();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1826").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1827", "Do not use Count() or LongCount() when Any() can be used",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1827")]
    public async Task RequireAnyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool HasItems() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.Count() > 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1827").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1829", "Use Length/Count property instead of Count() when available",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1829")]
    public async Task RequireLengthPropertyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int GetCount() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.Count();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1829").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1830", "Prefer strongly-typed Append and Insert method overloads on StringBuilder",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1830")]
    public async Task RequireStronglyTypedStringBuilderOverloads()
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
                    var sb = new System.Text.StringBuilder();
                    sb.Append(true.ToString());
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1830").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1831", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1831")]
    public async Task RequireAsSpanInsteadOfStringRangeIndexer()
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
                    string text = "hello world";
                    System.ReadOnlySpan<char> slice = text[0..5];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1831").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1832", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1832")]
    public async Task RequireAsSpanInsteadOfArrayRangeIndexerForReadOnlySpan()
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
                    byte[] bytes = new byte[10];
                    System.ReadOnlySpan<byte> slice = bytes[2..];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1832").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1833", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1833")]
    public async Task RequireAsMemoryInsteadOfArrayRangeIndexerForReadOnlyMemory()
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
                    byte[] bytes = new byte[10];
                    System.Memory<byte> slice = bytes[2..];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1833").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1834", "Consider using 'StringBuilder.Append(char)' when applicable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1834")]
    public async Task RequireStringBuilderAppendChar()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text;
            namespace test;
            public static class Program
            {
                public static string Build() 
                {
                    var sb = new StringBuilder();
                    sb.Append("a");
                    return sb.ToString();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1834").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1835", "Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1835")]
    public async Task RequireMemoryBasedStreamReadAsyncOverloads()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task<int> Main()
                {
                    using var stream = new MemoryStream();
                    var buffer = new byte[1024];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1835").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1836", "Prefer IsEmpty over Count",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1836")]
    public async Task RequireIsEmptyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Concurrent;
            namespace test;
            public static class Program
            {
                public static bool HasItems() 
                {
                    var bag = new ConcurrentBag<int>();
                    return bag.Count > 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1836").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1837", "Use 'Environment.ProcessId'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1837")]
    public async Task RequireEnvironmentProcessId()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int pid = Process.GetCurrentProcess().Id;
                    return pid == 0 ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1837").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1838", "Avoid 'StringBuilder' parameters for P/Invokes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1838")]
    public async Task ProhibitStringBuilderPInvokeParameters()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            using System.Text;
            namespace test;
            internal static class NativeMethods
            {
                [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
                internal static extern int GetTempPath(int nBufferLength, StringBuilder lpBuffer);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1838").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1839", "Use 'Environment.ProcessPath'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1839")]
    public async Task RequireEnvironmentProcessPath()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string path = Process.GetCurrentProcess().MainModule!.FileName;
                    return path == null ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1839").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1840", "Use 'Environment.CurrentManagedThreadId'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1840")]
    public async Task RequireEnvironmentCurrentManagedThreadId()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    return threadId == 0 ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1840").ShouldBeTrue();
    }
}
