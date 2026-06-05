using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesPerformanceModernShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA1841", "Prefer Dictionary.Contains methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1841")]
    public async Task RequireDictionaryContainsMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool HasKey() 
                {
                    var dict = new Dictionary<string, int>();
                    return dict.Keys.Contains("test");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1841").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1844", "Provide memory-based overrides of async methods when subclassing 'Stream'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1844")]
    public async Task RequireMemoryBasedStreamAsyncOverrides()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Threading;
            using System.Threading.Tasks;
            namespace test;
            public class MyStream : Stream
            {
                public override bool CanRead => true;
                public override bool CanSeek => false;
                public override bool CanWrite => true;
                public override long Length => 0;
                public override long Position { get => 0; set { } }
                public override void Flush() { }
                public override int Read(byte[] buffer, int offset, int count) => 0;
                public override long Seek(long offset, SeekOrigin origin) => 0;
                public override void SetLength(long value) { }
                public override void Write(byte[] buffer, int offset, int count) { }
                public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
                    => Task.FromResult(0);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1844").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1846", "Prefer 'AsSpan' over 'Substring'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1846")]
    public async Task RequireAsSpanOverSubstring()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get() 
                {
                    var text = "hello world";
                    return text.Substring(0, 5);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1846").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1847", "Use char literal for a single character lookup",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1847")]
    public async Task RequireCharLiteralForLookup()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Find() 
                {
                    var text = "hello";
                    return text.IndexOf("e");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1847").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1849", "Call async methods when in an async method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1849")]
    public async Task RequireAsyncMethodsInAsyncContext()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task<int> Main()
                {
                    var semaphore = new SemaphoreSlim(1);
                    semaphore.Wait();
                    return await Task.FromResult(0);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1849").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1850", "Prefer static 'HashData' method over 'ComputeHash'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1850")]
    public async Task RequireStaticHashDataMethod()
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
                    byte[] data = new byte[10];
                    using var sha = SHA256.Create();
                    byte[] hash = sha.ComputeHash(data);
                    return hash.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1850").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1851", "Possible multiple enumerations of 'IEnumerable' collection",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1851")]
    public async Task ProhibitMultipleEnumerationsOfIEnumerable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int CountItems(IEnumerable<int> items)
                {
                    if (!items.Any())
                        return 0;
                    return items.Count();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1851").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1852", "Seal internal types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852")]
    public async Task RequireSealInternalTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            internal class MyClass
            {
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1852").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1854", "Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1854")]
    public async Task RequireTryGetValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Get() 
                {
                    var dict = new Dictionary<string, int>();
                    if (dict.ContainsKey("test"))
                        return dict["test"];
                    return 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1854").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1855", "Prefer 'Clear' over 'Fill'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1855")]
    public async Task PreferClearOverFillDefault()
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
                    System.Span<byte> span = bytes;
                    span.Fill(default);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1855").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1856", "Incorrect usage of ConstantExpected attribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1856")]
    public async Task ProhibitIncorrectConstantExpectedUsage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public static class Program
            {
                public static int Method([ConstantExpected(Min = 10, Max = 5)] int value) => value;
                public static int Main() => Method(7);
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1856").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1857", "A constant is expected for the parameter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1857")]
    public async Task RequireConstantForConstantExpectedParameter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public static class Helper
            {
                public static int Double([ConstantExpected] int x) => x * 2;
            }
            public static class Program
            {
                static int _multiplier = 3;
                public static int Main() => Helper.Double(_multiplier);
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1857").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1858", "Use 'StartsWith' instead of 'IndexOf'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1858")]
    public async Task RequireStartsWithInsteadOfIndexOfEqualsZero()
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
                    string s = "hello world";
                    bool startsWithHello = s.IndexOf("hello") == 0;
                    return startsWithHello ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1858").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1861", "Avoid constant arrays as arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1861")]
    public async Task RequireAvoidConstantArrays()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool Contains(int value) 
                {
                    return new[] { 1, 2, 3 }.Contains(value);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1861").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1862", "Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1862")]
    public async Task RequireStringComparisonForCaseInsensitive()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool Compare() 
                {
                    return "hello".ToLower() == "HELLO".ToLower();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1862").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1864", "Prefer the 'IDictionary.TryAdd(TKey, TValue)' method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1864")]
    public async Task RequireTryAddInsteadOfContainsKeyAndAdd()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var dict = new Dictionary<string, int>();
                    if (!dict.ContainsKey("key"))
                        dict.Add("key", 1);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1864").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1865", "Use char overload",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1865")]
    public async Task RequireCharOverloadForStartsWith()
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
                    string s = "hello world";
                    bool starts = s.StartsWith("h", StringComparison.Ordinal);
                    return starts ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1865").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1868", "Unnecessary call to 'Contains(item)'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1868")]
    public async Task ProhibitUnnecessaryContainsBeforeSetAdd()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var set = new HashSet<string>();
                    if (!set.Contains("item"))
                        set.Add("item");
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1868").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1869", "Cache and reuse 'JsonSerializerOptions' instances",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1869")]
    public async Task RequireCachedJsonSerializerOptions()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.Json;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string json = JsonSerializer.Serialize(42, new JsonSerializerOptions { WriteIndented = true });
                    return json.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1869").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1871", "Do not pass a nullable struct to 'ArgumentNullException.ThrowIfNull'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1871")]
    public async Task ProhibitNullableStructThrowIfNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main(int? value = null)
                {
                    System.ArgumentNullException.ThrowIfNull(value);
                    return value!.Value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1871").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1872", "Prefer 'Convert.ToHexString' and 'Convert.ToHexStringLower' over call chains based on 'BitConverter.ToString'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1872")]
    public async Task RequireConvertToHexStringOverBitConverter()
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
                    byte[] bytes = new byte[] { 1, 2, 3 };
                    string hex = System.BitConverter.ToString(bytes).Replace("-", "");
                    return hex.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1872").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1874", "Use 'Regex.IsMatch'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1874")]
    public async Task RequireRegexIsMatchInsteadOfMatchesCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.RegularExpressions;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool hasDigits = Regex.Match("abc123", @"\d+").Success;
                    return hasDigits ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1874").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1875", "Use 'Regex.Count'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1875")]
    public async Task RequireRegexCountInsteadOfMatchesCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.RegularExpressions;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int count = Regex.Matches("abc123def456", @"\d+").Count;
                    return count;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1875").ShouldBeTrue();
    }
}
