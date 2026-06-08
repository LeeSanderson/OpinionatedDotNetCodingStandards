using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodingStandards;

[Collection(nameof(PackageCollection))]
public class CodingStandardsModernSyntaxShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task AllowWarningsAsErrorsToBeDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [
            (Name: "TreatWarningsAsErrors", Value: "false"),
            (Name: "MSBuildTreatWarningsAsErrors", Value: "false"),
        ]);
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        // Convert to file-scoped namespace should now be a warning instead of an error
        buildOutput.HasWarning("IDE0161").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0071", "Simplify interpolation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0071")]
    public async Task RequireSimplifiedInterpolation()
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
                    var value = 123;
                    return $"{value.ToString()}";
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0071").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0076", "Invalid global 'SuppressMessageAttribute'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0076")]
    public async Task RejectInvalidGlobalSuppressMessageAttribute()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            [assembly: SuppressMessage("Usage", "CA1063", Scope = "member", Target = "~M:test.Program.NonExistentMethod")]
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0076").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0078", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0078")]
    public async Task RequireOrPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Check(object obj)
                {
                    if (obj is int || obj is long)
                    {
                        return 1;
                    }
                    return 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0078").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0082", "'typeof' can be converted to 'nameof'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0082")]
    public async Task RequireNameofOverTypeof()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get() => typeof(Program).Name;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0082").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0083", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0083")]
    public async Task RequireNotPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Check(object obj)
                {
                    if (!(obj is string))
                    {
                        return 0;
                    }
                    return 1;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0083").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0100", "Remove redundant equality",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0100")]
    public async Task RequireRemovalOfRedundantEquality()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsPositive(int x)
                {
                    return (x > 0) == true;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0100").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0130", "Namespace does not match folder structure",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0130")]
    public async Task RequireNamespaceToMatchFolderStructure()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Services/Service.cs",
            """
            namespace test;
            public static class Service { }
            """);
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0130").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0161", "Convert to file-scoped namespace",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0161")]
    public async Task RequireFileScopedNamespaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0161").ShouldBeTrue(); // Convert to file-scoped namespace
    }

    [Fact]
    [RuleDoc("IDE0170", "Property pattern can be simplified",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0170")]
    public async Task RequireSimplifiedPropertyPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Person
            {
                public string Name { get; set; } = "";
            }
            public static class Program
            {
                public static bool Check(Person p)
                {
                    return p is { Name: { Length: > 0 } };
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0170").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0180", "Use tuple to swap values",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0180")]
    public async Task RequireTupleSwap()
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
                    var a = 1;
                    var b = 2;
                    var temp = a;
                    a = b;
                    b = temp;
                    return a + b;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0180").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0200", "Remove unnecessary lambda expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0200")]
    public async Task RequireMethodGroupConversion()
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
                    var items = new System.Collections.Generic.List<int>() { 1, 2, 3 };
                    items.ForEach(x => System.Console.WriteLine(x));
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0230", "Use UTF-8 string literal",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0230")]
    public async Task RequireUtf8StringLiteral()
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
                    System.ReadOnlySpan<byte> bytes = new byte[] { 104, 101, 108, 108, 111 };
                    return bytes.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0230").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0240", "Remove redundant nullable directive",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0240")]
    public async Task RequireRemovalOfRedundantNullableDirective()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #nullable enable
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0240").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0241", "Remove unnecessary nullable directive",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0241")]
    public async Task RejectUnnecessaryNullableRestore()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs",
            """
            namespace test;
            #nullable disable
            public static class Program
            {
                public static int Main() => 0;
            }
            #nullable restore
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0241").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0250", "Make struct 'readonly'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0250")]
    public async Task RequireReadonlyStruct()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public struct MyPoint
            {
                public int X { get; }
                public int Y { get; }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0250").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0270", "Use coalesce expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0270")]
    public async Task RejectNullCheckThrowWhenCoalesceThrowApplies()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get(string? input)
                {
                    string? s = input?.Trim();
                    if (s == null)
                    {
                        throw new System.InvalidOperationException("null");
                    }

                    return s;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0270").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0300", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0300")]
    public async Task RequireCollectionExpressionForArray()
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
                    int[] arr = new int[] { 1, 2, 3 };
                    return arr.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0300").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0301", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0301")]
    public async Task RequireCollectionExpressionForList()
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
                    System.Collections.Immutable.ImmutableArray<int> arr = System.Collections.Immutable.ImmutableArray<int>.Empty;
                    return arr.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0301").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0302", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302")]
    public async Task RequireCollectionExpressionForStackAlloc()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Sum()
                {
                    // IDE0302 fires on a stackalloc array-creation expression with an
                    // initializer assigned to a Span<int>; net10.0 supports InlineArrayTypes
                    // so the analyzer registers and the collection expression is suggested.
                    System.Span<int> numbers = stackalloc int[] { 1, 2, 3 };
                    var total = 0;
                    foreach (var n in numbers)
                        total += n;
                    return total;
                }
                public static int Main() => Sum();
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0302").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0303", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0303")]
    public async Task RequireCollectionExpressionForImmutableArrayCreate()
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
                    System.Collections.Immutable.ImmutableArray<int> arr = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
                    return arr.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0303").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0305", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0305")]
    public async Task RequireCollectionExpressionForFluentCreation()
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
                    System.Collections.Generic.List<int> list = new[] { 1, 2, 3 }.ToList();
                    return list.Count;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0305").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0330", "Use 'System.Threading.Lock'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0330")]
    public async Task RequireSystemThreadingLock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static readonly object _lock = new object();
                public static int Main()
                {
                    lock (_lock)
                    {
                        return 0;
                    }
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0330").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE1005", "Delegate invocation can be simplified.",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1005")]
    public async Task RequireSimplifiedDelegateInvocation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void Invoke(System.Action? action)
                {
                    if (action != null)
                        action();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE2000", "Avoid multiple blank lines",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2000")]
    public async Task RejectMultipleBlankLines()
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
                    var x = 0;


                    return x;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE2000").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE2001", "Embedded statements must be on their own line",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2001")]
    public async Task RequireEmbeddedStatementsOnOwnLine()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() { if (true) { return 1; } return 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2001").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE2003", "Blank line required between block and subsequent statement",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2003")]
    public async Task RequireBlankLineAfterBlock()
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
                    if (true)
                    {
                        var x = 1;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2003").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE2004", "Blank line not allowed after constructor initializer colon",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2004")]
    public async Task RequireNoBlankLineAfterConstructorColon()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Base
            {
                public Base(int x) { }
            }
            public class Derived : Base
            {
                public Derived() :

                    base(1) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2004").ShouldBeTrue();
    }
}
