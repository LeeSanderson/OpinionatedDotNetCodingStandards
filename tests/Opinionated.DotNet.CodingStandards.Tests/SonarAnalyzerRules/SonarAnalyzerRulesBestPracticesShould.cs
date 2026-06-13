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
}
