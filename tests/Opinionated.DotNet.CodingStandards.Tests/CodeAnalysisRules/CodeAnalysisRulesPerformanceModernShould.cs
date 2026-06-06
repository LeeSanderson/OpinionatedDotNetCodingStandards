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
                public static bool Find()
                {
                    var text = "hello";
                    return text.Contains("e");
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
            namespace test;
            public static class Program
            {
                public static string Format()
                {
                    return string.Join(", ", new[] { "a", "b", "c" });
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

    [Fact]
    [RuleDoc("CA1826", "Do not use Enumerable methods on indexable collections",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1826")]
    public async Task ProhibitEnumerableMethodsOnIndexableCollections()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static int GetLast(System.Collections.Generic.IReadOnlyList<int> list)
                {
                    return list.Last();
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1826").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1842", "Do not use 'WhenAll' with a single task",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1842")]
    public async Task ProhibitWhenAllWithSingleTask()
    {
        // LangVersion 12 is required to trigger CA1842. The analyzer
        // (DoNotUseWhenAllOrWaitAllWithSingleArgument) only fires when the bare single-task call
        // binds to the params Task[]/Task<T>[] overload AND produces an *implicit* single-element
        // array (IsSingleTaskArgument checks for an implicit IArrayCreationOperation). .NET 9 added
        // `params ReadOnlySpan<Task>` overloads, and C# 13's "params collections" overload
        // resolution prefers the span overload for a bare single-task argument — that call produces
        // no implicit Task[] creation, so the analyzer never matches it. Pinning C# 12 disables
        // params-span expansion, so the call binds to params Task[] and the rule fires.
        using var project = await CreateProjectBuilder(properties: [("LangVersion", "12")]);
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static async Task M()
                {
                    await Task.WhenAll(Task.FromResult(42));
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1842").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1843", "Do not use 'WaitAll' with a single task",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1843")]
    public async Task ProhibitWaitAllWithSingleTask()
    {
        // LangVersion 12 is required to trigger CA1843, the blocking WaitAll counterpart of CA1842.
        // The shared analyzer (DoNotUseWhenAllOrWaitAllWithSingleArgument) only fires when the bare
        // single-task call binds to the params Task[] overload AND produces an *implicit*
        // single-element array (IsSingleTaskArgument checks for an implicit IArrayCreationOperation).
        // .NET 9 added a `params ReadOnlySpan<Task>` overload, and C# 13's "params collections"
        // overload resolution prefers the span overload for a bare single-task argument — that call
        // produces no implicit Task[] creation, so the analyzer never matches it. Pinning C# 12
        // disables params-span expansion, so the call binds to params Task[] and the rule fires.
        using var project = await CreateProjectBuilder(properties: [("LangVersion", "12")]);
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static void M()
                {
                    var t = Task.CompletedTask;
                    Task.WaitAll(t);
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1843").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1845", "Use span-based 'string.Concat'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1845")]
    public async Task UseSpanBasedStringConcat()
    {
        // CA1845 (UseSpanBasedStringConcat) registers only on OperationKind.Binary — the string
        // `+` concatenation operator — and never on string.Concat(...) invocations. It reports the
        // top-most concat when every operand is a string (or char literal), at least one operand is
        // a Substring(int)/Substring(int, int) call, and a span-based string.Concat overload of the
        // matching arity (2-4 spans) exists. The earlier "untestable" note blamed IDE0057
        // subsumption, but that was a misdiagnosis: the old probe used a method call
        // (a.AsSpan().SequenceEqual(b.Substring(1))) that the analyzer never inspects. A real `+`
        // concat with a Substring operand fires error:CA1845 — IDE0057 still fires as a note at the
        // Substring site but does not suppress CA1845.
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static string Shorten(string text)
                {
                    return text.Substring(1) + "!";
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1845").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1846", "Prefer 'AsSpan' over 'Substring'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1846")]
    public async Task PreferAsSpanOverSubstring()
    {
        // CA1846 (PreferAsSpanOverSubstring) registers on OperationKind.Argument, not on the
        // Substring(...).AsSpan() chain the earlier "untestable" probe asserted on (the analyzer
        // never sees that chain as an argument substitution, so it could never fire). It fires when
        // a Substring(int)/Substring(int, int) result is passed directly as an argument to a method
        // that exposes another overload of the same arity/return type taking ReadOnlySpan<char> in
        // that position. int.TryParse(string, out int) has an int.TryParse(ReadOnlySpan<char>, out int)
        // overload, so the text.Substring(7) argument fires error:CA1846. IDE0057 still co-fires as a
        // harmless note at the Substring site but does not suppress CA1846.
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static bool TryParseSuffix(string text, out int value)
                {
                    return int.TryParse(text.Substring(7), out value);
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1846").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1852", "Seal internal types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852")]
    public async Task SealInternalTypes()
    {
        using var project = await CreateProjectBuilder();

        // The package auto-injects [assembly: InternalsVisibleTo("<project>.Tests")] into every
        // non-test project (see build/Opinionated.DotNet.CodingStandards.targets). CA1852 suppresses
        // itself whenever the assembly exposes its internals to a friend assembly, because the friend
        // could subclass the type. Opt back in via the documented
        // dotnet_code_quality.CA1852.ignore_internalsvisibleto option so the rule fires here.
        await project.AddFile(
            ".editorconfig",
            """
            [*.cs]
            dotnet_code_quality.CA1852.ignore_internalsvisibleto = true
            """);
        await project.AddFile(
            "InternalService.cs",
            """
            namespace test;

            internal class InternalService
            {
                public int GetValue() => 42;
            }
            """);
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var service = new InternalService();
                    return service.GetValue();
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1852").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1853", "Unnecessary call to 'Dictionary.ContainsKey(key)'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1853")]
    public async Task ProhibitUnnecessaryContainsKeyCall()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                // CA1853 fires when a Dictionary.Remove(key) call is guarded by a
                // redundant ContainsKey(key) check: Remove already no-ops (returns
                // false) when the key is absent, so the guard is pointless. The
                // analyzer is DoNotGuardDictionaryRemoveByContainsKey.
                //
                // The original "ContainsKey + TryGetValue" probe never fired because
                // that is not a CA1853 pattern at all: ContainsKey + indexer is
                // CA1854, ContainsKey + Add is CA1864, ContainsKey + Remove is CA1853.
                public static void RemoveIfPresent(Dictionary<string, int> dict, string key)
                {
                    if (dict.ContainsKey(key))
                        dict.Remove(key);
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1853").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1867", "Use char overload",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1867")]
    public async Task UseCharOverloadForStartsWithOrdinalIgnoreCase()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static bool StartsWithO(string s) => s.StartsWith("o", StringComparison.OrdinalIgnoreCase);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1867").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA1870", "Use a cached 'SearchValues' instance",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1870",
        Untestable = "CA1870 produces no diagnostic in build SARIF for any SearchValues.Create called inline in IndexOfAny/ContainsAny, including patterns inside a foreach loop; the SARIF is empty even with clean code and dotnet_diagnostic.CA1870.severity = warning configured")]
    public async Task UseCachedSearchValuesInstance()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Buffers;

            namespace test;

            public static class Program
            {
                public static bool ContainsVowel(string[] strs)
                {
                    foreach (var s in strs)
                    {
                        if (s.AsSpan().IndexOfAny(SearchValues.Create("aeiou")) >= 0)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1870").ShouldBeTrue();
    }
}
