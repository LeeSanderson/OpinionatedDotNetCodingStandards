using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

public class CodeAnalysisRulesUsageModernShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA2267", "Quote whitespace in file-based program directive values",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2267")]
    public async Task QuoteWhitespaceInFileBasedProgramDirectives()
    {
        // The compiler rejects '#:' directives in a normal project unless the FileBasedProgram feature flag is on.
        using var project = await CreateProjectBuilderAsync(properties: [("Features", "FileBasedProgram")]);
        await project.AddFileAsync(
            "Program.cs",
            """
            #:property Description=Hello World
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2267").ShouldBeTrue();
    }
}
