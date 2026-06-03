using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;


[Collection(nameof(PackageCollection))]
public class CodingStandardsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
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
    [RuleDoc("IDE0080", "Remove unnecessary suppression operator",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0080")]
    public async Task RequireRemovalOfUnnecessarySuppressionOperator()
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
                    var value = "test";
                    return value!.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0080").ShouldBeTrue();
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
    [RuleDoc("IDE0110", "Remove unnecessary discard",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0110")]
    public async Task RequireRemovalOfUnnecessaryDiscard()
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
                    _ = GetValue();
                    return 0;
                }
                private static int GetValue() => 1;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0110").ShouldBeTrue();
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
    [RuleDoc("IDE0280", "Use 'nameof'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0280")]
    public async Task RequireNameofForStrings()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void CheckArg(string value)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new System.ArgumentException("value");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0280").ShouldBeTrue();
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
    [RuleDoc("IDE1006", "Naming Styles",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1006")]
    public async Task RequireCorrectNamingStyle()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private int Field = 1;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1006").ShouldBeTrue();
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
    [RuleDoc("IDE2002", "Consecutive braces must not have blank line between them",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2002")]
    public async Task RequireNoBlankLineBetweenBraces()
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

                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2002").ShouldBeTrue();
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

    [Fact]
    [RuleDoc("IDE2005", "Blank line not allowed after conditional expression token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2005")]
    public async Task RequireNoBlankLineAfterConditionalToken()
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
                    var result = true ?
            
                        1 : 0;
                    return result;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("IDE2006", "Blank line not allowed after arrow expression clause token",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2006")]
    public async Task RequireNoBlankLineAfterArrowExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Get() =>
            
                    1;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2006").ShouldBeTrue();
    }
}