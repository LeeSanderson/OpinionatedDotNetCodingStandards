using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers;

public class MeziantouAnalyzersEventSourceShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("MA0226", "EventSource class should be sealed",
        HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0226.md")]
    public async Task RequireEventSourceClassToBeSealed()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics.Tracing;
            namespace test;
            public class MyEventSource : EventSource
            {
                [Event(1)]
                public void Started(string message) => WriteEvent(1, message);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("MA0226").ShouldBeTrue();
    }
}
