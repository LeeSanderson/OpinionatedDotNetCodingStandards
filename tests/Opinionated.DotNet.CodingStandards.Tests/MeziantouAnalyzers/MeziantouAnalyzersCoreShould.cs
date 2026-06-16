using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers;

[Collection(nameof(PackageCollection))]
public class MeziantouAnalyzersCoreShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("MA0015", "Specify the parameter name in ArgumentException",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0015.md")]
    public async Task SpecifyParameterNameInArgumentException()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void M(string value) => throw new System.ArgumentException("message");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0015").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0017", "Abstract types should not have public or internal constructors",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0017.md")]
    public async Task AbstractTypesShouldNotHavePublicConstructors()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public abstract class AbstractFoo
            {
                public AbstractFoo() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0017").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0019", "Use EventArgs.Empty",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0019.md")]
    public async Task UseEventArgsEmpty()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public event System.EventHandler? MyEvent;
                public void M() => MyEvent?.Invoke(this, new System.EventArgs());
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0022", "Return Task.FromResult instead of returning null",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0022.md")]
    public async Task ReturnTaskFromResultInsteadOfNull()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public System.Threading.Tasks.Task<int> GetCount() => null!;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0022").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0027", "Prefer rethrowing an exception implicitly",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0027.md")]
    public async Task PreferRethrowingExceptionImplicitly()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0027").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0029", "Combine LINQ methods",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0029.md")]
    public async Task CombineLinqMethods()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.Where(x => x > 0).Where(x => x < 10).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0029").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0030", "Remove useless OrderBy call",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0030.md")]
    public async Task RemoveUselessOrderByCall()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.OrderBy(x => x).OrderBy(x => x).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0035", "Do not use dangerous threading methods",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0035.md")]
    public async Task DoNotUseDangerousThreadingMethods()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0035").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0037", "Remove empty statement",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0037.md")]
    public async Task RemoveEmptyStatement()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void M() { ; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0037").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0040", "Forward the CancellationToken parameter to methods that take one",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0040.md")]
    public async Task ForwardCancellationTokenParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0040").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0042", "Do not use blocking calls in an async method",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0042.md")]
    public async Task DoNotUseBlockingCallsInAsyncMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0042").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0044", "Remove useless ToString call",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0044.md")]
    public async Task RemoveUselessToStringCall()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M(string value) => $"{value.ToString()}";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0044").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0052", "Replace constant Enum.ToString with nameof",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0052.md")]
    public async Task ReplaceConstantEnumToStringWithNameof()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static string S = System.StringComparison.Ordinal.ToString();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0055", "Do not use finalizer",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0055.md")]
    public async Task DoNotUseFinalizer()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                ~C() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0055").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0056", "Do not call overridable members in constructor",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0056.md")]
    public async Task DoNotCallOverridableMembersInConstructor()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Base
            {
                public Base() { M(); }
                public virtual void M() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0056").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0060", "The value returned by Stream.Read/Stream.ReadAsync is not used",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0060.md")]
    public async Task ObserveStreamReadReturnValue()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0060").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0063", "Use Where before OrderBy",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0063.md")]
    public async Task UseWhereBeforeOrderBy()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public int[] M(int[] items) =>
                    items.OrderBy(x => x).Where(x => x > 0).ToArray();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0063").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0067", "Use Guid.Empty",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0067.md")]
    public async Task UseGuidEmpty()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static System.Guid EmptyGuid = new System.Guid();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0067").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0068", "Invalid parameter name for nullable attribute",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0068.md")]
    public async Task DetectInvalidParameterNameForNullableAttribute()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public class C
            {
                [return: NotNullIfNotNull("wrongParamName")]
                public string? M(string? value) => value;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0068").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0072", "Do not throw from a finally block",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0072.md")]
    public async Task DoNotThrowFromFinallyBlock()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0072").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0073", "Avoid comparison with bool constant",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0073.md")]
    public async Task AvoidComparisonWithBoolConstant()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(bool condition) => condition == true;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0073").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0079", "Forward the CancellationToken using .WithCancellation()",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0079.md")]
    public async Task ForwardCancellationTokenWithWithCancellation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0079").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0023", "Add RegexOptions.ExplicitCapture",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0023.md")]
    public async Task RequireExplicitCaptureInRegex()
    {
        using var project = await CreateProjectBuilderAsync();
        // MA0023 fires only when a RegexOptions argument is physically present and lacks
        // ExplicitCapture/ECMAScript while the pattern has an unnamed capturing group "([a-z]+)".
        // (No RegexOptions argument => the analyzer skips the report entirely, which is why the
        // earlier "new Regex(\"(foo)bar\")" probe never fired.) Configured severity is suggestion
        // => surfaces as SARIF "note".
        await project.AddFileAsync("Program.cs", """
            using System.Text.RegularExpressions;
            namespace test;
            public class C
            {
                public bool M(string input) => new Regex("([a-z]+)", RegexOptions.None).IsMatch(input);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("MA0023").ShouldBeTrue();
    }
}
