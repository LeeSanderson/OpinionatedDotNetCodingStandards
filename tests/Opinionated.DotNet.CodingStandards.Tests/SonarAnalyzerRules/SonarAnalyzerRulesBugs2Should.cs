// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBugs2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S2737", "catch clauses should do more than rethrow",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2737/")]
    public async Task WarnOnCatchClauseThatOnlyRethrows()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Example
            {
                public static void Run()
                {
                    try
                    {
                        System.Console.WriteLine("work");
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2737").ShouldBeTrue();
    }
}
