using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class MeziantouAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("MA0015", "Specify the parameter name in ArgumentException",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0015.md")]
    public async Task SpecifyParameterNameInArgumentException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M(string value) => throw new System.ArgumentException("message");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0015").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0017", "Abstract types should not have public or internal constructors",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0017.md")]
    public async Task AbstractTypesShouldNotHavePublicConstructors()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public abstract class AbstractFoo
            {
                public AbstractFoo() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0017").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0019", "Use EventArgs.Empty",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0019.md")]
    public async Task UseEventArgsEmpty()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public event System.EventHandler? MyEvent;
                public void M() => MyEvent?.Invoke(this, new System.EventArgs());
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0022", "Return Task.FromResult instead of returning null",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0022.md")]
    public async Task ReturnTaskFromResultInsteadOfNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public System.Threading.Tasks.Task<int> GetCount() => null!;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0022").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0027", "Prefer rethrowing an exception implicitly",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0027.md")]
    public async Task PreferRethrowingExceptionImplicitly()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    try { }
                    catch (System.Exception ex) { throw ex; }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0027").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0029", "Combine LINQ methods",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0029.md")]
    public async Task CombineLinqMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.Where(x => x > 0).Where(x => x < 10).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0029").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0030", "Remove useless OrderBy call",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0030.md")]
    public async Task RemoveUselessOrderByCall()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.OrderBy(x => x).OrderBy(x => x).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0035", "Do not use dangerous threading methods",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0035.md")]
    public async Task DoNotUseDangerousThreadingMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    System.Threading.Thread.CurrentThread.Suspend();
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0035").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0037", "Remove empty statement",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0037.md")]
    public async Task RemoveEmptyStatement()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M() { ; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0037").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0040", "Forward the CancellationToken parameter to methods that take one",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0040.md")]
    public async Task ForwardCancellationTokenParameter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public async System.Threading.Tasks.Task M(System.Threading.CancellationToken ct)
                {
                    await System.Threading.Tasks.Task.Delay(100);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0040").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0042", "Do not use blocking calls in an async method",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0042.md")]
    public async Task DoNotUseBlockingCallsInAsyncMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public async System.Threading.Tasks.Task M()
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0042").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0044", "Remove useless ToString call",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0044.md")]
    public async Task RemoveUselessToStringCall()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public string M(string value) => $"{value.ToString()}";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0044").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0052", "Replace constant Enum.ToString with nameof",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0052.md")]
    public async Task ReplaceConstantEnumToStringWithNameof()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static string S = System.StringComparison.Ordinal.ToString();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0055", "Do not use finalizer",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0055.md")]
    public async Task DoNotUseFinalizer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                ~C() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0055").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0056", "Do not call overridable members in constructor",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0056.md")]
    public async Task DoNotCallOverridableMembersInConstructor()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Base
            {
                public Base() { M(); }
                public virtual void M() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0056").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0060", "The value returned by Stream.Read/Stream.ReadAsync is not used",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0060.md")]
    public async Task ObserveStreamReadReturnValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    var s = new System.IO.MemoryStream(new byte[10]);
                    var buf = new byte[10];
                    s.Read(buf, 0, 10);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0060").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0063", "Use Where before OrderBy",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0063.md")]
    public async Task UseWhereBeforeOrderBy()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.OrderBy(x => x).Where(x => x > 0).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0063").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0067", "Use Guid.Empty",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0067.md")]
    public async Task UseGuidEmpty()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static System.Guid EmptyGuid = new System.Guid();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0067").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0068", "Invalid parameter name for nullable attribute",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0068.md")]
    public async Task DetectInvalidParameterNameForNullableAttribute()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public class C
            {
                [return: NotNullIfNotNull("wrongParamName")]
                public string? M(string? value) => value;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0068").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0072", "Do not throw from a finally block",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0072.md")]
    public async Task DoNotThrowFromFinallyBlock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    try { }
                    finally { throw new System.Exception(); }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0072").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0073", "Avoid comparison with bool constant",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0073.md")]
    public async Task AvoidComparisonWithBoolConstant()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(bool condition) => condition == true;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0073").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0079", "Forward the CancellationToken using .WithCancellation()",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0079.md")]
    public async Task ForwardCancellationTokenWithWithCancellation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public async System.Threading.Tasks.Task M(
                    System.Collections.Generic.IAsyncEnumerable<int> source,
                    System.Threading.CancellationToken ct)
                {
                    await foreach (var item in source)
                    {
                        _ = item;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0079").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0082", "NaN should not be used in comparisons",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0082.md")]
    public async Task DoNotUseNaNInComparisons()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(double value) => value == double.NaN;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0082").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0085", "Anonymous delegates should not be used to unsubscribe from Events",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0085.md")]
    public async Task DoNotUnsubscribeFromEventWithAnonymousDelegate()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public event System.EventHandler? MyEvent;
                public void M() => MyEvent -= (sender, e) => { };
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0085").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0086", "Do not throw from a finalizer",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0086.md")]
    public async Task DoNotThrowFromFinalizer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                ~C() { throw new System.Exception(); }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0086").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0087", "Parameters with [DefaultParameterValue] attributes should also be marked [Optional]",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0087.md")]
    public async Task DefaultParameterValueShouldAlsoBeMarkedOptional()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Runtime.InteropServices;
            namespace test;
            public class C
            {
                public void M([DefaultParameterValue(42)] int x) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0087").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0088", "Use [DefaultParameterValue] instead of [DefaultValue]",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0088.md")]
    public async Task UseDefaultParameterValueInsteadOfDefaultValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.ComponentModel;
            namespace test;
            public class C
            {
                public void M([DefaultValue(42)] int x = 42) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0088").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0090", "Remove empty else/finally block",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0090.md")]
    public async Task RemoveEmptyElseFinallyBlock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    try { System.Console.WriteLine(); }
                    finally { }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0090").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0093", "EventArgs should not be null",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0093.md")]
    public async Task EventArgsShouldNotBeNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public event System.EventHandler? MyEvent;
                public void M() => MyEvent?.Invoke(this, null!);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0093").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0099", "Use Explicit enum value instead of 0",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0099.md")]
    public async Task UseExplicitEnumValueInsteadOfZero()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public enum Status { None, Active }
            public class C
            {
                public Status M() => 0;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0099").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0100", "Await task before disposing of resources",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0100.md")]
    public async Task AwaitTaskBeforeDisposingResources()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Res : System.IDisposable { public void Dispose() { } }
            public class C
            {
                public System.Threading.Tasks.Task M()
                {
                    using var scope = new Res();
                    return System.Threading.Tasks.Task.Delay(1);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0100").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0103", "Use SequenceEqual instead of equality operator",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0103.md")]
    public async Task UseSequenceEqualInsteadOfEqualityOperator()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public bool M() => "hello".AsSpan() == "hello".AsSpan();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0103").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0108", "Remove redundant argument value",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0108.md")]
    public async Task RemoveRedundantArgumentValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M(string value)
                {
                    System.ArgumentNullException.ThrowIfNull(value, "value");
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0108").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0113", "Use DateTime.UnixEpoch",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0113.md")]
    public async Task UseDateTimeUnixEpoch()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static readonly System.DateTime Epoch =
                    new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0113").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0114", "Use DateTimeOffset.UnixEpoch",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0114.md")]
    public async Task UseDateTimeOffsetUnixEpoch()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static readonly System.DateTimeOffset Epoch =
                    new System.DateTimeOffset(1970, 1, 1, 0, 0, 0, System.TimeSpan.Zero);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0114").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0128", "Use 'is' operator instead of SequenceEqual",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0128.md")]
    public async Task UseIsOperatorInsteadOfSequenceEqual()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(System.ReadOnlySpan<char> span) => span.SequenceEqual("hello");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0128").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0129", "Await task in using statement",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0129.md")]
    public async Task AwaitTaskInUsingStatement()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public async System.Threading.Tasks.Task M()
                {
                    using var t = System.Threading.Tasks.Task.Delay(100);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0129").ShouldBeTrue();
    }


    [Fact]
    [RuleDoc("MA0134", "Observe result of async calls",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0134.md")]
    public async Task ObserveResultOfAsyncCalls()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M()
                {
                    SomeAsync();
                }
                private System.Threading.Tasks.Task<int> SomeAsync() =>
                    System.Threading.Tasks.Task.FromResult(42);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0134").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0140", "Both if and else branch have identical code",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0140.md")]
    public async Task BothBranchesHaveIdenticalCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public int M(bool cond)
                {
                    if (cond) return 1;
                    else return 1;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0140").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0143", "Primary constructor parameters should be readonly",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0143.md")]
    public async Task PrimaryConstructorParametersShouldBeReadonly()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C(int x)
            {
                public void M() { x = 42; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0143").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0144", "Use System.OperatingSystem to check the current OS",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0144.md")]
    public async Task UseSystemOperatingSystemToCheckOs()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Runtime.InteropServices;
            namespace test;
            public class C
            {
                public bool M() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0144").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0145", "Signature for [UnsafeAccessorAttribute] method is not valid",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0145.md")]
    public async Task UnsafeAccessorSignatureMustBeValid()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public struct MyStruct { private int _x; }
            public class C
            {
                [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_x")]
                public static extern ref int GetField(MyStruct a);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0145").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0146", "Name must be set explicitly on local functions",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0146.md")]
    public async Task UnsafeAccessorLocalFunctionNameMustBeSetExplicitly()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Target { private int _x; }
            public static class Accessor
            {
                public static void M(Target t)
                {
                    [System.Runtime.CompilerServices.UnsafeAccessor(System.Runtime.CompilerServices.UnsafeAccessorKind.Field)]
                    static extern ref int LocalAccessor(Target tgt);
                    LocalAccessor(t) = 42;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0146").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0151", "DebuggerDisplay must contain valid members",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0151.md")]
    public async Task DebuggerDisplayMustContainValidMembers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            [System.Diagnostics.DebuggerDisplay("{NonExistentMember}")]
            public class C { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0151").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0152", "Use Unwrap instead of using await twice",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0152.md")]
    public async Task UseUnwrapInsteadOfDoubleAwait()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public async System.Threading.Tasks.Task<int> M()
                {
                    System.Threading.Tasks.Task<System.Threading.Tasks.Task<int>> nested =
                        System.Threading.Tasks.Task.FromResult(System.Threading.Tasks.Task.FromResult(42));
                    return await await nested;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0152").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0158", "Use System.Threading.Lock",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0158.md")]
    public async Task UseSystemThreadingLock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                private readonly object _lock = new();
                public void M()
                {
                    lock (_lock) { System.Console.WriteLine(); }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0158").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0159", "Use 'Order' instead of 'OrderBy'",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0159.md")]
    public async Task UseOrderInsteadOfOrderBy()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public System.Collections.Generic.IEnumerable<int> M(int[] items) =>
                    items.OrderBy(x => x);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0159").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0160", "Use ContainsKey instead of TryGetValue",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0160.md")]
    public async Task UseContainsKeyInsteadOfTryGetValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(System.Collections.Generic.Dictionary<string, int> dict, string key)
                {
                    return dict.TryGetValue(key, out _);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0160").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0166", "Forward the TimeProvider to methods that take one",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0166.md")]
    public async Task ForwardTimeProviderToMethodsThatTakeOne()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void M(System.TimeProvider timeProvider)
                {
                    System.Threading.Tasks.Task.Delay(default(System.TimeSpan));
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0166").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0173", "Use LazyInitializer.EnsureInitialize",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0173.md")]
    public async Task UseLazyInitializerEnsureInitialize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                private object? _instance;
                public object GetInstance()
                {
                    System.Threading.Interlocked.CompareExchange(ref _instance, new object(), null);
                    return _instance!;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0173").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0176", "Optimize guid creation",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0176.md")]
    public async Task OptimizeGuidCreation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public System.Guid M() => System.Guid.Parse("d3c4d2f1-5f6a-4b7c-8e9f-0a1b2c3d4e5f");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0176").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0178", "Use TimeSpan.Zero instead of TimeSpan.FromXXX(0)",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0178.md")]
    public async Task UseTimeSpanZeroInsteadOfFromXxx()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static readonly System.TimeSpan Zero = System.TimeSpan.FromSeconds(0);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0178").ShouldBeTrue();
    }
}
