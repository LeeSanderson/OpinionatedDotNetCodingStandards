// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesComplexityShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S3776", "Cognitive Complexity of methods should not be too high",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3776/")]
    public async Task ProhibitExcessiveCognitiveComplexity()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                // Cognitive complexity deliberately above 15:
                // multiple nested ifs, else-ifs, loops, and breaks
                public static int Method(int a, int b, int c, int d)
                {
                    int result = 0;
                    if (a > 0)              // +1
                    {
                        if (b > 0)          // +2 (nesting)
                        {
                            if (c > 0)      // +3 (nesting)
                            {
                                result += a + b + c;
                            }
                            else            // +1
                            {
                                result -= c;
                            }
                        }
                        else if (b < 0)     // +1
                        {
                            for (int i = 0; i < a; i++)  // +3 (nesting)
                            {
                                if (c > i)  // +4 (nesting)
                                {
                                    result += i;
                                }
                                if (d > i)  // +4 (nesting)
                                {
                                    result -= i;
                                }
                            }
                        }
                    }
                    else                    // +1
                    {
                        while (d > 0)       // +2 (nesting)
                        {
                            result += d--;
                            if (result > 100) // +3 (nesting)
                            {
                                break;
                            }
                        }
                    }
                    return result;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3776").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1541", "Methods and properties should not be too complex",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1541/")]
    public async Task ProhibitExcessiveCyclomaticComplexity()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                // Cyclomatic complexity deliberately above 10:
                // each if/else-if adds +1 to the branch count
                public static string Classify(int n)
                {
                    if (n < 0) { return "negative"; }
                    if (n == 0) { return "zero"; }
                    if (n < 10) { return "tiny"; }
                    if (n < 100) { return "small"; }
                    if (n < 1_000) { return "medium"; }
                    if (n < 10_000) { return "large"; }
                    if (n < 100_000) { return "huge"; }
                    if (n < 1_000_000) { return "massive"; }
                    if (n < 10_000_000) { return "enormous"; }
                    if (n < 100_000_000) { return "gigantic"; }
                    return "astronomical";
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1541").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S134", "Control flow statements \"if\", \"switch\", \"for\", \"foreach\", \"while\", \"do\" and \"try\" should not be nested too deeply",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-134/")]
    public async Task ProhibitExcessiveNestingDepth()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method(bool a, int n)
                {
                    if (a)                               // level 1
                    {
                        while (n > 0)                    // level 2
                        {
                            for (int i = 0; i < n; i++) // level 3
                            {
                                if (i > 0)               // level 4 — exceeds default threshold of 3
                                {
                                    n--;
                                }
                            }
                        }
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S134").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S138", "Functions should not have too many lines of code",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-138/")]
    public async Task ProhibitExcessiveMethodLength()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                // Method body deliberately exceeds 80 lines to trigger S138
                public static int LongMethod()
                {
                    int result = 0;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    result++;
                    return result;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S138").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S107", "Methods should not have too many parameters",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-107/")]
    public async Task ProhibitTooManyMethodParameters()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                // Exceeds the default threshold of 7 parameters
                public static int Add(int a, int b, int c, int d, int e, int f, int g, int h)
                    => a + b + c + d + e + f + g + h;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S107").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1067", "Expressions should not be too complex",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1067/")]
    public async Task ProhibitExcessiveExpressionComplexity()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class C
            {
                public static bool Evaluate(bool a, bool b, bool c, bool d, bool e) =>
                    a && b && c && d && e;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1067").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1151", "“switch case” clauses should not have too many lines of code",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1151/")]
    public async Task ProhibitExcessiveSwitchCaseLength()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int Process(int x)
                {
                    int result = 0;
                    switch (x)
                    {
                        case 1:
                            result += 1;
                            result += 2;
                            result += 3;
                            result += 4;
                            result += 5;
                            result += 6;
                            result += 7;
                            result += 8;
                            result += 9;
                            result += 10;
                            break;
                        default:
                            break;
                    }
                    return result;
                }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1151").ShouldBeTrue();
    }
}
