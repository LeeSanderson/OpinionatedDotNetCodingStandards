using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers;

[Collection(nameof(PackageCollection))]
public class MeziantouAnalyzers3Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("MA0183", "The format string should use placeholders",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0183.md")]
    public async Task FormatStringShouldUsePlaceholders()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M(int value) => string.Format("text without placeholder", value);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0183").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0184", "Do not use interpolated string without parameters",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0184.md")]
    public async Task DoNotUseInterpolatedStringWithoutParameters()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M() => $"hello world";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0184").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0185", "Simplify string.Create when all parameters are culture invariant",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0185.md")]
    public async Task SimplifyStringCreateWhenAllParametersAreCultureInvariant()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M(System.Guid id) => string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{id}");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0185").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0186", "Equals method should use [NotNullWhen(true)] on the parameter",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0186.md")]
    public async Task EqualsMethodShouldUseNotNullWhenTrue()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C : System.IEquatable<C>
            {
                public bool Equals(C? other) => other != null;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0186").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0187", "Use constructor injection instead of [Inject] attribute",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0187.md")]
    public async Task UseConstructorInjectionInsteadOfInjectAttribute()
    {
        using var project = await CreateProjectBuilderAsync(
            properties: [("AssemblyVersion", "9.0.0.0")]);
        await project.AddFileAsync("Program.cs", """
            namespace Microsoft.AspNetCore.Components
            {
                public interface IComponent { }
                public sealed class InjectAttribute : System.Attribute { }
            }
            namespace test
            {
                public class MyComponent : Microsoft.AspNetCore.Components.IComponent
                {
                    [Microsoft.AspNetCore.Components.Inject]
                    public System.IServiceProvider? Services { get; set; }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0187").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0188", "Use System.TimeProvider instead of a custom time abstraction",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0188.md")]
    public async Task UseSystemTimeProviderInsteadOfCustomAbstraction()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public interface IDateTimeProvider
            {
                System.DateTime UtcNow { get; }
                System.DateTime Now { get; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0188").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0189", "Use InlineArray instead of fixed-size buffers",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0189.md")]
    public async Task UseInlineArrayInsteadOfFixedSizeBuffers()
    {
        using var project = await CreateProjectBuilderAsync(
            properties: [("AllowUnsafeBlocks", "true")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public unsafe struct Buffer
            {
                public fixed int Data[4];
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0189").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0190", "Use partial property instead of partial method for GeneratedRegex",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0190.md")]
    public async Task UsePartialPropertyInsteadOfPartialMethodForGeneratedRegex()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public partial class C
            {
                [System.Text.RegularExpressions.GeneratedRegex(@"\d+")]
                private static partial System.Text.RegularExpressions.Regex Digits();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0190").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0191", "Do not use the null-forgiving operator",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0191.md")]
    public async Task DoNotUseTheNullForgivingOperator()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M() { string x = null!; return x; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0191").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0192", "Use HasFlag instead of bitwise checks",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0192.md")]
    public async Task UseHasFlagInsteadOfBitwiseChecks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            [System.Flags]
            public enum Status { None = 0, Active = 1, Inactive = 2 }
            public class C
            {
                public bool M(Status s) => (s & Status.Active) != 0;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0192").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0193", "Use an overload with a MidpointRounding argument",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0193.md")]
    public async Task UseOverloadWithMidpointRoundingArgument()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public double M(double x) => System.Math.Round(x);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0193").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0194", "Merge is expressions on the same value",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0194.md")]
    public async Task MergeIsExpressionsOnSameValue()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(object x) => x is not int && x is not string;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0194").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0195", "Do not use static fields before they are initialized",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0195.md")]
    public async Task DoNotUseStaticFieldsBeforeTheyAreInitialized()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Config
            {
                public static readonly int Count = Size + 1;
                public static readonly int Size = 10;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0195").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0196", "Do not use inheritdoc on non-inheriting members",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0196.md")]
    public async Task DoNotUseInheritdocOnNonInheritingMembers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Base { }
            public class C : Base
            {
                /// <inheritdoc/>
                public void M() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0196").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0197", "Add dedicated documentation on types",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0197.md")]
    public async Task AddDedicatedDocumentationOnTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public interface IFoo { }
            /// <inheritdoc/>
            public class C : IFoo { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0197").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0198", "Specify cref for ambiguous inheritdoc on types",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0198.md")]
    public async Task SpecifyCrefForAmbiguousInheritdocOnTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public interface IA { }
            public interface IB { }
            /// <inheritdoc/>
            public class C : IA, IB { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0198").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0199", "Do not use inheritdoc on types without inheritance source",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0199.md")]
    public async Task DoNotUseInheritdocOnTypesWithoutInheritanceSource()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            /// <inheritdoc/>
            public class C { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0199").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0200", "Do not use empty property patterns with non-nullable value types",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0200.md")]
    public async Task DoNotUseEmptyPropertyPatternsWithNonNullableValueTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public bool M(int x) => x is { };
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0201", "Do not use zero-valued enum flags in flag checks",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0201.md")]
    public async Task DoNotUseZeroValuedEnumFlagsInFlagChecks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            [System.Flags]
            public enum Status { None = 0, Active = 1 }
            public class C
            {
                public bool M(Status s) => s.HasFlag(Status.None);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0201").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0202", "Conditional compilation branches have identical code",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0202.md")]
    public async Task ConditionalCompilationBranchesHaveIdenticalCode()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public int M()
                {
            #if DEBUG
                    return 42;
            #else
                    return 42;
            #endif
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0202").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("MA0203", "Do not use return tag for void method",
        HelpLink = "https://www.meziantou.net/analyzer/rules/203")]
    public async Task ProhibitReturnTagOnVoidMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Program
            {
                /// <summary>Does something.</summary>
                /// <returns>Nothing — this is wrong on a void method.</returns>
                public static void DoSomething() { }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();
        buildOutput.HasError("MA0203").ShouldBeTrue();
    }
}
