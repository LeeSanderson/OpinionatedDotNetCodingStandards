using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodingStandards;

[Collection(nameof(PackageCollection))]
public class CodingStandardsStyleShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task IgnoreNameCanBeSimplifiedAsOnlyTreatedAsErrorsInIDE()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private readonly int _i = 1;
                public int Get() => this._i;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0003").ShouldBeFalse();
    }

    [Fact]
    [RuleDoc("CA1866", "Use char overload",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1866")]
    public async Task AllowIndividualRuleSuppressionViaNoWarn()
    {
        // NoWarn targets RS0030 only — CA1866 (a different rule) must still fire,
        // proving the override is scoped and the ruleset remains layerable
        using var project = await CreateProjectBuilder(properties: [(Name: "NoWarn", Value: "RS0030")]);
        await project.AddFile("Program.cs", "_ = System.DateTime.Now;\r\n_ = \"hello\".IndexOf(\"h\");\r\n");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
        buildOutput.HasError("CA1866").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0004", "Remove Unnecessary Cast",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0004")]
    public async Task RequireRemovalOfUnnecessaryCast()
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
                    var i = (int)1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0004").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0005", "Using directive is unnecessary.",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0005")]
    public async Task RequireRemovalOfUnnecessaryUsings()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0007", "Use implicit type",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0007")]
    public async Task RequireVarInsteadOfExplicitType()
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
                    int i = 1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0007").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0010", "Add missing cases",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0010")]
    public async Task RequireAllSwitchCasesToBeCovered()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            enum Color { Red, Green, Blue }
            public static class Program
            {
                public static int Main()
                {
                    var color = Color.Red;
                    switch (color)
                    {
                        case Color.Red:
                            return 1;
                    }

                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0010").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0011", "Add braces",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0011")]
    public async Task RequireBracesForControlFlow()
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
                        return 1;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0011").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0017", "Simplify object initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0017")]
    public async Task RequireObjectInitializerSyntax()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Config { public int Value { get; set; } }
            public static class Program
            {
                public static int Main()
                {
                    var config = new Config();
                    config.Value = 42;
                    return config.Value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0017").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0018", "Inline variable declaration",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0018")]
    public async Task RequireInlineVariableDeclaration()
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
                    int result;
                    if (int.TryParse("42", out result))
                        return result;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0019", "Use pattern matching to avoid 'as' followed by a 'null' check",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0019")]
    public async Task RequirePatternMatchingForAsNullCheck()
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
                    object? obj = "test";
                    var s = obj as string;
                    if (s != null)
                        return s.Length;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0020", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0020")]
    public async Task RequirePatternMatchingForIsCheck()
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
                    object obj = "test";
                    if (obj is string)
                    {
                        var s = (string)obj;
                        return s.Length;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0020").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0028", "Use collection initializers or expressions",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0028")]
    public async Task RequireCollectionInitializerSyntax()
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
                    var list = new System.Collections.Generic.List<int>();
                    list.Add(1);
                    list.Add(2);
                    return list.Count;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0028").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0029", "Use coalesce expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0029")]
    public async Task RequireCoalesceExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get(string? input)
                {
                    return input != null ? input : "default";
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0029").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0031", "Use null propagation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0031")]
    public async Task RequireNullPropagation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Get(string? input)
                {
                    return input != null ? input.Length : 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0031").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0033", "Use explicitly provided tuple name",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0033")]
    public async Task RequireExplicitlyProvidedTupleName()
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
                    var t = (a: 1, b: 2);
                    return t.Item1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0033").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0034", "Simplify 'default' expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0034")]
    public async Task RequireSimplifiedDefaultExpression()
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
                    int x = default(int);
                    return x;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0034").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0036", "Order modifiers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0036")]
    public async Task RequireCorrectModifierOrder()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                static public int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0036").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0039", "Use local function instead of lambda",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0039")]
    public async Task RequireLocalFunctionOverLambda()
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
                    System.Func<int> getValue = () => 42;
                    return getValue();
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0039").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0040", "Add accessibility modifiers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0040")]
    public async Task RequireAccessibilityModifiers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Program
            {
                void Method() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0040").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0043", "Format string contains invalid placeholder",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0043")]
    public async Task RejectInvalidFormatString()
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
                    var s = string.Format("{0} {1}", "hello");
                    return s.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0043").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0044", "Add readonly modifier",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0044")]
    public async Task RequireReadonlyModifierOnFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Program
            {
                private int _value = 1;
                public int GetValue() => _value;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0044").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0045", "Convert to conditional expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0045")]
    public async Task RequireConditionalExpressionForAssignment()
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
                    var x = 1;
                    string result;
                    if (x > 0)
                    {
                        result = "positive";
                    }
                    else
                    {
                        result = "non-positive";
                    }
                    return result.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0045").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0046", "Convert to conditional expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0046")]
    public async Task RequireConditionalExpressionForReturn()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get(int x)
                {
                    if (x > 0)
                    {
                        return "positive";
                    }
                    else
                    {
                        return "non-positive";
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0046").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0049", "Use language keywords instead of framework type names",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0049")]
    public async Task RequireLanguageKeywordsOverFrameworkTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static Int32 Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0049").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0051", "Remove unused private members",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0051")]
    public async Task RequireUnusedPrivateMembersToBeRemoved()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static void UnusedMethod()
                {
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0051").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0052", "Remove unread private members",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0052")]
    public async Task RequireUnreadPrivateMembersToBeRemoved()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static int _writeOnly;
                private static void Write()
                {
                    _writeOnly = 42;
                }
                public static int Main()
                {
                    Write();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0054", "Use compound assignment",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0054")]
    public async Task RequireCompoundAssignment()
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
                    var i = 1;
                    i = i + 1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0054").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0055", "Fix formatting",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0055")]
    public async Task RequireProperFormatting()
    {
        using var project = await CreateProjectBuilder();
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

        buildOutput.HasError("IDE0055").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0056", "Use index operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0056")]
    public async Task RequireIndexOperator()
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
                    var arr = new[] { 1, 2, 3 };
                    return arr[arr.Length - 1];
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0056").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0057", "Use range operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0057")]
    public async Task RequireRangeOperator()
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
                    var s = "hello";
                    return s.Substring(2).Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0057").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0059", "Unnecessary assignment of a value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0059")]
    public async Task RequireRemovalOfUnnecessaryValueAssignment()
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
                    var i = 1;
                    i = 2;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0059").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0060", "Remove unused parameter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0060")]
    public async Task RequireUnusedParametersToBeRemoved()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static int Add(int x, int unused)
                {
                    return x;
                }
                public static int Main()
                {
                    return Add(1, 2);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0060").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0062", "Make local function 'static'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0062")]
    public async Task RequireStaticLocalFunctions()
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
                    return LocalFunc();
                    int LocalFunc() => 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0062").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0065", "Misplaced using directive",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0065")]
    public async Task RequireUsingDirectivesOutsideNamespace()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                using System;
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0065").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0066", "Convert switch statement to expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0066")]
    public async Task RequireSwitchExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get(int x)
                {
                    switch (x)
                    {
                        case 1:
                            return "one";
                        default:
                            return "other";
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0066").ShouldBeTrue();
    }
}
