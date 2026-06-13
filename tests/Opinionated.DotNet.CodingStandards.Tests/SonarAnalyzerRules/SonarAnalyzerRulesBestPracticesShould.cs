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
}
