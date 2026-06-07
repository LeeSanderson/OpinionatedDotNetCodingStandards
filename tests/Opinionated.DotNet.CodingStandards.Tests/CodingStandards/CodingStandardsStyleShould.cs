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

    [Fact]
    [RuleDoc("IDE0030", "Use coalesce expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0030")]
    public async Task UseCoalesceExpressionForNullableValueType()
    {
        // IDE0030's analyzer (AbstractUseCoalesceExpressionForNullableTernaryConditionalCheckDiagnosticAnalyzer)
        // fires only on the exact shape "!x.HasValue ? y : x.Value" for a Nullable<T>, NOT on a "x != null" comparison.
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int GetValue(int? x) => !x.HasValue ? 0 : x.Value;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0031", "Use null propagation",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0031")]
    public async Task UseNullPropagation()
    {
        using var project = await CreateProjectBuilder();
        // Non-null branch must be a property/field access on a reference-typed nullable
        // receiver: the analyzer explicitly bails when the accessed member is a method
        // symbol (`x == null ? x : x.M` cannot become `x?.M`), so a method-call branch
        // never fires IDE0031. A property access (node.Value) is convertible to node?.Value
        // and triggers "Null check can be simplified".
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public sealed class Node
                {
                    public string Value { get; set; } = "";
                }
                public static string? GetValue(Node? node) => node != null ? node.Value : null;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0031").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("IDE0049", "Use language keywords instead of framework type names",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0049",
        Untestable = "EnforceOnBuild.Never: IDE0049's analyzer (PreferFrameworkTypeDiagnosticAnalyzerBase, src/Features/Core/Portable/PreferFrameworkType/PreferFrameworkTypeDiagnosticAnalyzerBase.cs) builds its descriptor with EnforceOnBuildValues.PreferBuiltInOrFrameworkType, which EnforceOnBuildValues.cs defines as EnforceOnBuild.Never (public const EnforceOnBuild PreferBuiltInOrFrameworkType = /*IDE0049*/ EnforceOnBuild.Never;). EnforceOnBuild.cs documents Never as 'an IDE-only diagnostic that cannot be enforced on build', and AbstractBuiltInCodeStyleDiagnosticAnalyzer_Core.CreateDescriptorWithId stamps the descriptor with the EnforceOnBuild_Never custom tag (DiagnosticCustomTags.Create). Consequently the command-line/build (csc) host never reports IDE0049 regardless of editorconfig severity. Empirically confirmed: building a project referencing the packed package with the System.String violation present, IDE0055 silenced, and IDE0049 escalated via both dotnet_diagnostic.IDE0049.severity=error and dotnet_style_predefined_type_for_locals_parameters_members/_member_access=true:error, yields an EMPTY SARIF (build succeeds) while sibling rules (IDE0161, IDE0007, CA2211) fire in the same build. The prior 'formatter-backed/emits IDE0055' explanation was incorrect; IDE0049 emits nothing at build. Corroborated by dotnet/roslyn issues #50173 and #77120.")]
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

    [Fact]
    [RuleDoc("IDE0070", "Use 'System.HashCode'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0070")]
    public async Task UseSystemHashCodeInsteadOfManualHashCodeAccumulator()
    {
        using var project = await CreateProjectBuilder();

        // IDE0070 fires only on the VS-generated multi-statement accumulator GetHashCode
        // (HashCodeAnalyzer requires an IBlockOperation body with statements.Length >= 3),
        // NOT on an expression-bodied `=> X ^ Y`. It is severity=suggestion so it surfaces as a SARIF note.
        await project.AddFile(
            "Program.cs",
            """
            namespace test;

            public static class Program
            {
                public static int Main() => 0;
            }

            public class Point
            {
                public int X { get; set; }

                public int Y { get; set; }

                public override bool Equals(object? obj) => obj is Point other && X == other.X && Y == other.Y;

                public override int GetHashCode()
                {
                    var hashCode = -1671106492;
                    hashCode = (hashCode * -1521134295) + X.GetHashCode();
                    hashCode = (hashCode * -1521134295) + Y.GetHashCode();
                    return hashCode;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0070").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0074", "Use compound assignment",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0074")]
    public async Task UseNullCoalescingCompoundAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static string? _cache;
                public static string GetOrInit(string value)
                {
                    // `x ?? (x = y)` is IDE0074's actual trigger (-> `x ??= y`);
                    // the analyzer registers on SyntaxKind.CoalesceExpression and
                    // requires the coalesce's right to be a parenthesized simple
                    // assignment whose left equals the coalesce's left.
                    return _cache ?? (_cache = value);
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
        Untestable = "IDE0079 is an IDE-only analyzer that structurally cannot fire during `dotnet build`. Its descriptor is created with EnforceOnBuildValues.RemoveUnnecessaryPragmaSuppression = EnforceOnBuild.Never (commented \"IDE-only analyzer\" in src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs), and its analyzer (AbstractRemoveUnnecessaryInlineSuppressionsDiagnosticAnalyzer : IPragmaSuppressionsAnalyzer, src/Analyzers/Core/Analyzers/RemoveUnnecessarySuppressions/AbstractRemoveUnnecessaryPragmaSuppressionsDiagnosticAnalyzer.cs) overrides InitializeWorker with an EMPTY body (\"We do not register any normal analyzer actions as we need 'CompilationWithAnalyzers' context... Instead, the analyzer defines a special 'AnalyzeAsync' method that should be invoked by the host\"). That special AnalyzeAsync also bails immediately unless Compilation.Options.ReportSuppressedDiagnostics is true. The command-line/MSBuild analyzer driver never calls AnalyzeAsync, so no diagnostic is ever produced at build. Empirically confirmed in a harness-replica project: an unnecessary [SuppressMessage] produced only an unrelated IDE0055 formatting error, and with `dotnet_diagnostic.IDE0055.severity = none` the build emitted zero diagnostics (IDE0079 absent at note/warning/error). Source: dotnet/roslyn EnforceOnBuildValues.cs and AbstractRemoveUnnecessaryPragmaSuppressionsDiagnosticAnalyzer.cs (main).")]
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

    [Fact]
    [RuleDoc("IDE0080", "Remove unnecessary suppression operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0080")]
    public async Task RemoveUnnecessaryNullForgivingOperator()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                // `o!` (null-forgiving operator) on the left of an `is` expression has no
                // effect and is confusing -> IDE0080. Analyzer fires on SyntaxKind.IsExpression
                // whose left operand is a SuppressNullableWarningExpression.
                public static bool Check(object? o) => o! is string;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0080").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0110", "Remove unnecessary discard",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0110")]
    public async Task RemoveUnnecessaryDiscardPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                // 'int _' is an unnecessary discard designation; the discard '_' adds
                // nothing over the plain type pattern 'int', so IDE0110 flags it.
                public static bool IsInt(object o) => o is int _;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0110").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0260", "Use pattern matching",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260")]
    public async Task UsePatternMatchingOverAsWithMemberAccess()
    {
        using var project = await CreateProjectBuilder();
        // IDE0260 fires on `(expr as T)?.Member == constant` (as-cast + null-conditional
        // member access compared to a non-null constant), convertible to `expr is T { Member: constant }`.
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool HasEmptyLength(object obj) => (obj as string)?.Length == 0;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("IDE0260").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0280", "Use 'nameof'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0280")]
    public async Task UseNameofInsteadOfStringLiteral()
    {
        // IDE0280 fires on a string-literal parameter name inside a nullable-analysis attribute
        // (here NotNullIfNotNull) when a parameter of that name is in scope - the fix is nameof(input).
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public static class Program
            {
                [return: NotNullIfNotNull("input")]
                public static string? Echo(string? input) => input;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0280").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE0304", "Simplify collection initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304")]
    public async Task SimplifyBuilderCollectionInitialization()
    {
        // IDE0304 ("Use collection expression for builder") fires on the ImmutableArray builder
        // pattern: CreateBuilder<T>() ... Add(...) ... a local assigned from ToImmutable(). The
        // 'using' import keeps the factory receiver a simple name (the analyzer skips qualified
        // receivers), and the explicit-typed result local gives the analyzer a collection-expression
        // target (a 'var' result does not fire). The explicit type trips IDE0007 'use var', so that
        // companion rule is suppressed via NoWarn to isolate IDE0304.
        using var project = await CreateProjectBuilder(properties: [
            (Name: "NoWarn", Value: "IDE0007;IDE0008"),
        ]);
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Immutable;

            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var builder = ImmutableArray.CreateBuilder<int>();
                    builder.Add(1);
                    builder.Add(2);
                    ImmutableArray<int> result = builder.ToImmutable();
                    return result.Length;
                }
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
