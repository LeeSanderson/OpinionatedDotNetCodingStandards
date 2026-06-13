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
}
