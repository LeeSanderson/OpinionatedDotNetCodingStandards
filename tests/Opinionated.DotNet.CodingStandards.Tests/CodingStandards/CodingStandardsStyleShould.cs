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

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0030", "Use coalesce expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0030",
        Untestable = "In .NET 10 Roslyn the build-time analyzer does not fire IDE0030 for nullable value type coalesce patterns; IDE0055 fires instead and IDE0030 is absent from SARIF output even when IDE0055 is suppressed. IDE0031 has the same symptom")]
    public async Task UseCoalesceExpressionForNullableValueType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int GetValue(int? x) => x != null ? x.Value : 0;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0030").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0031", "Use null propagation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0031",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0031; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseNullPropagation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string? GetUpper(string? s) => s != null ? s.ToUpper() : null;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0031").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0049", "Use language keywords instead of framework type names",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0049",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0049; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseLanguageKeywordsInsteadOfFrameworkTypeNames()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static System.String GetName() => "hello";
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0049").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0070", "Use 'System.HashCode'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0070",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0070 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: the XOR GetHashCode pattern triggers IDE0055 across every file in the compilation, and replacing it with HashCode.Combine removes IDE0055 entirely. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task UseSystemHashCodeInsteadOfXorGetHashCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Point
            {
                public int X { get; set; }
                public int Y { get; set; }
                public override bool Equals(object? obj) => obj is Point p && X == p.X && Y == p.Y;
                public override int GetHashCode() => X ^ Y;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0070").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0074", "Use compound assignment",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0074",
        Untestable = "In .NET 10 Roslyn, x = x ?? y (null-coalescing compound assignment) fires as IDE0054 (general compound assignment) not IDE0074; the two rules share the same diagnostic trigger in this analyzer version")]
    public async Task UseNullCoalescingCompoundAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string GetOrDefault(string? value)
                {
                    value = value ?? "default";
                    return value;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0074").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0079", "Remove unnecessary suppression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0079",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0079 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: an unnecessary SuppressMessage triggers IDE0055 while an equivalent necessary suppression does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task RemoveUnnecessarySuppression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity")]
                public static void M() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0079").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0080", "Remove unnecessary suppression operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0080",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0080; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task RemoveUnnecessaryNullForgivingOperator()
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
                    _ = s!.Length;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0080").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0110", "Remove unnecessary discard",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0110",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0110; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task RemoveUnnecessaryDiscardPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsAny(object? o) => o is _;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0110").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0260", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260",
        Untestable = "In .NET 10 Roslyn build analysis, IDE0260 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: 'obj as T != null' triggers IDE0055 while the equivalent 'obj is T' does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
    public async Task UsePatternMatchingInsteadOfAsNotNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsString(object obj) => (obj as string) != null;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0260").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0280", "Use 'nameof'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0280",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0280; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseNameofInsteadOfStringLiteral()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void ValidateValue(string value)
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0280").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0302", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302",
        Untestable = "In .NET 10 Roslyn, empty collection factory methods (Array.Empty, Enumerable.Empty, ImmutableArray<T>.Empty) fire as IDE0301 (collection initialization) not IDE0302; the empty-specific rule is subsumed by IDE0301 in the build analyzer")]
    public async Task SimplifyEmptyCollectionWithArrayEmpty()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static int[] GetEmpty() => Array.Empty<int>();
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0302").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0304", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304",
        Untestable = "In .NET 10 Roslyn, ImmutableArray<T>.Empty fires as IDE0301 (collection initialization) not IDE0304; the ImmutableArray-specific empty collection rule is subsumed by IDE0301 in the build analyzer")]
    public async Task SimplifyEmptyImmutableArrayCollection()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Immutable;
            namespace test;
            public static class Program
            {
                public static ImmutableArray<int> GetEmpty() => ImmutableArray<int>.Empty;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0304").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE1006", "Naming Styles",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1006",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE1006; also, CS0708 (member cannot be declared static in a non-static class) preempts the instance field violation pattern in a static class")]
    public async Task RequireUnderscorePrefixForPrivateFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyService
            {
                private string myField = "value";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1006").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE2002", "Consecutive braces must not have blank line between them",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2002",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2002; the blank-line-between-braces pattern also triggers CS0161 (not all code paths return a value) in methods with empty bodies")]
    public async Task ProhibitBlankLineBetweenConsecutiveBraces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void M()
                {
                    if (true)
                    {
                        return;

                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2002").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE2005", "Blank line not allowed after conditional expression token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2005",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2005; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task ProhibitBlankLineAfterConditionalExpressionToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                static bool Cond => true;
                public static int Main() => Cond ?

                    1 : 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2005").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE2006", "Blank line not allowed after arrow expression clause token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2006",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE2006; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task ProhibitBlankLineAfterArrowExpressionClauseToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() =>

                    0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2006").ShouldBeTrue();
    }
}
