using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBestPracticesShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1006", "Method overrides should not change parameter defaults",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1006/")]
    public async Task WarnOnOverrideChangingParameterDefault()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Base
            {
                public virtual void DoWork(int value = 0) { }
            }
            public class Derived : Base
            {
                public override void DoWork(int value = 42) { }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1006").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S106", "Standard outputs should not be used directly to log anything",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-106/")]
    public async Task ProhibitDirectStandardOutputLogging()
    {
        using var project = await CreateProjectBuilder(properties: [("OutputType", "Library")]);
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public void Log(string message)
                {
                    Console.WriteLine(message);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S106").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1066", "Mergeable \"if\" statements should be combined",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1066/")]
    public async Task WarnOnMergeableIfStatements()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class C
            {
                public static void Method(int a, int b)
                {
                    if (a > 0)
                    {
                        if (b > 0)
                        {
                            System.Console.WriteLine(a + b);
                        }
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1066").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1075", "URIs should not be hardcoded",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1075/")]
    public async Task ProhibitHardcodedUris()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    string url = "https://www.example.com/api/v1/data";
                    return url.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1075").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S108", "Nested blocks of code should not be left empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-108/")]
    public async Task ProhibitEmptyNestedBlock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method(int[] items)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S108").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S109", "Magic numbers should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-109/")]
    public async Task WarnOnMagicNumbers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Calculator
            {
                public static int Compute(int a) => a * 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S109").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1110", "Redundant pairs of parentheses should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1110/")]
    public async Task ProhibitRedundantParentheses()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Calculator
            {
                public static int Add(int a, int b) => ((a + b));
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1110").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1116", "Empty statements should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1116/")]
    public async Task ProhibitEmptyStatements()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class C
            {
                public static void Method()
                {
                    ; // empty statement — S1116
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1116").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1121", "Assignments should not be made from within sub-expressions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1121/")]
    public async Task ProhibitAssignmentInSubExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int GetValue() => 0;

                public static void Use(int v) { _ = v; }

                public static int Main()
                {
                    int x = 0;
                    Use(x = GetValue());
                    return x;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1121").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1125", "Boolean literals should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1125/")]
    public async Task ProhibitRedundantBooleanLiterals()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Guard
            {
                public static bool IsPositive(int value) => value > 0 == true;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1125").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1128", "Unnecessary \"using\" should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1128/")]
    public async Task WarnOnUnnecessaryUsingDirective()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Text;

            namespace test;

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1128").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1133", "Deprecated code should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1133/")]
    public async Task WarnOnObsoleteDeclaration()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class MyService
            {
                [System.Obsolete("Use NewMethod instead")]
                public static void OldMethod() { }

                public static void NewMethod() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1133").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1134", "Track uses of \"FIXME\" tags",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1134/")]
    public async Task WarnOnFixmeTag()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Calculator
            {
                // FIXME: this method is broken and needs to be reimplemented
                public static int Add(int a, int b) => a + b;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1134").ShouldBeTrue();
    }
}
