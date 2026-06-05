using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesReliabilityShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA2002", "Do not lock on objects with weak identity",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2002")]
    public async Task ProhibitLockOnWeakIdentityObjects()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static readonly string _lock = "lock";
                public static void Method()
                {
                    lock (_lock) { }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2002").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2009", "Do not call ToImmutableCollection on an ImmutableCollection value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2009")]
    public async Task ProhibitRedundantToImmutableCollection()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Immutable;
            namespace test;
            public static class Program
            {
                public static void Method(ImmutableList<int> list)
                {
                    var result = list.ToImmutableList();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2009").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2011", "Avoid infinite recursion",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2011")]
    public async Task ProhibitInfiniteRecursionInPropertySetter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private int _value;
                public int Value
                {
                    get => _value;
                    set => Value = value;
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2011").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2012", "Use ValueTasks correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2012")]
    public async Task ProhibitAwaitingSameValueTaskTwice()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task Method()
                {
                    var vt = GetValueAsync();
                    var a = await vt;
                    var b = await vt;
                }
                static ValueTask<int> GetValueAsync() => ValueTask.FromResult(42);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2012").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2013", "Do not use ReferenceEquals with value types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2013")]
    public async Task ProhibitReferenceEqualsWithValueTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool AreEqual(int a, int b)
                {
                    return System.Object.ReferenceEquals(a, b);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2013").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2014", "Do not use stackalloc in loops",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2014")]
    public async Task ProhibitStackallocInLoops()
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
                    for (int i = 0; i < 10; i++)
                    {
                        System.Span<int> buffer = stackalloc int[100];
                        buffer[0] = i;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2014").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2015", "Do not define finalizers for types derived from MemoryManager<T>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2015")]
    public async Task ProhibitFinalizersOnMemoryManagerSubclasses()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Buffers;
            namespace test;
            public class MyMemoryManager : MemoryManager<byte>
            {
                ~MyMemoryManager() { }
                public override Memory<byte> Memory => Memory<byte>.Empty;
                public override Span<byte> GetSpan() => Span<byte>.Empty;
                public override MemoryHandle Pin(int elementIndex = 0) => default;
                public override void Unpin() { }
                protected override void Dispose(bool disposing) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2015").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2016", "Forward the 'CancellationToken' parameter to methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2016")]
    public async Task RequireForwardCancellationToken()
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
                public static async Task MethodAsync(CancellationToken cancellationToken) 
                {
                    await Task.Delay(100);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2016").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2018", "'Buffer.BlockCopy' expects the number of bytes to be copied for the 'count' argument",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2018")]
    public async Task RequireByteCountForBufferBlockCopy()
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
                    int[] src = { 1, 2, 3 };
                    int[] dst = new int[3];
                    System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2019", "Improper 'ThreadStatic' field initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2019")]
    public async Task ProhibitThreadStaticFieldWithInitializer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class MyClass
            {
                [System.ThreadStatic]
                private static int _counter = 1;
                public static int GetCounter() => _counter;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2021", "Do not call Enumerable.Cast<T> or Enumerable.OfType<T> with incompatible types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021")]
    public async Task ProhibitEnumerableCastWithIncompatibleTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    double[] doubles = { 1.0, 2.0 };
                    var ints = doubles.Cast<int>();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2021").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2022", "Avoid inexact read with 'Stream.Read'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2022")]
    public async Task ProhibitInexactStreamRead()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            namespace test;
            public static class Program
            {
                public static void Process(Stream stream)
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2022").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2024", "Do not use 'StreamReader.EndOfStream' in async methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2024")]
    public async Task ProhibitStreamReaderEndOfStreamInAsyncMethods()
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
                public static async Task ReadAsync(StreamReader reader)
                {
                    while (!reader.EndOfStream)
                    {
                        await reader.ReadLineAsync();
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2024").ShouldBeTrue();
    }
}
