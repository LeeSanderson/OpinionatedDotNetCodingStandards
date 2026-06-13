using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBugsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1206", "'Equals(Object)' and 'GetHashCode()' should be overridden in pairs",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1206/")]
    public async Task DetectEqualsWithoutGetHashCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class MyValue
            {
                private readonly int _x;
                public MyValue(int x) { _x = x; }

                public override bool Equals(object? obj)
                {
                    if (obj is MyValue other)
                        return _x == other._x;
                    return false;
                }
                // GetHashCode intentionally NOT overridden — violates S1206
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1206").ShouldBeTrue();
    }
}
