using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers;

[Collection(nameof(PackageCollection))]
public class MeziantouAnalyzersExtendedShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
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

    [Fact]
    [RuleDoc("MA0054", "Embed the caught exception as innerException",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0054.md")]
    public async Task EmbedCaughtExceptionAsInnerException()
    {
        using var project = await CreateProjectBuilder();
        // Throwing a NEW System.Exception from inside a catch block without passing the
        // caught exception as innerException fires MA0054: System.Exception declares an
        // (string, Exception) overload, so the analyzer's HasOverloadWithAdditionalParameterOfType guard is satisfied.
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    try
                    {
                        return 0;
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        throw new System.Exception("Failed");
                    }
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0054").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0070", "Obsolete attributes should include explanations",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0070.md")]
    public async Task ObsoleteAttributesShouldIncludeExplanations()
    {
        using var project = await CreateProjectBuilder();
        // [System.Obsolete] with no constructor argument => ConstructorArguments.Length == 0,
        // which is exactly what ObsoleteAttributesShouldIncludeExplanationsAnalyzer reports.
        // MA0070 ships with default severity Info, so it surfaces in SARIF as level "note" (not "error").
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                [System.Obsolete]
                public void OldMethod() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("MA0070").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0130", "GetType() should not be used on System.Type instances",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0130.md")]
    public async Task ShouldNotCallGetTypeOnTypeInstance()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public System.Type Force(System.Type t) => ((object)t).GetType();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0130").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0179", "Use Attribute.IsDefined instead of GetCustomAttribute(s)",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0179.md")]
    public async Task UseAttributeIsDefinedInsteadOfGetCustomAttribute()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Reflection;
            namespace test;
            public class C
            {
                public bool M() => typeof(System.Console).GetCustomAttribute<System.ObsoleteAttribute>() != null;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0179").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0180", "ILogger type parameter should match containing type",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0180.md")]
    public async Task LoggerTypeParameterShouldMatchContainingType()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFile("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class MyService
            {
                public MyService(ILogger<OtherService> logger) { }
            }
            public class OtherService { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0180").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0181", "Do not use cast",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0181.md")]
    public async Task DoNotUseCast()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public int M(object obj) => (int)obj;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0181").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0182", "Avoid unused internal types",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0182.md")]
    public async Task AvoidUnusedInternalTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            internal class Unused { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("MA0182").ShouldBeTrue();
    }
}
