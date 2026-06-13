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
}
