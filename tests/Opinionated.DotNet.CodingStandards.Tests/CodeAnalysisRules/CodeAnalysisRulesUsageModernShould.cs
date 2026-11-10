using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

public class CodeAnalysisRulesUsageModernShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA2266", "File-based program entry point should start with '#!'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2266")]
    public async Task RequireShebangInFileBasedProgramEntryPoint()
    {
        // The analyzer only runs on the file whose path equals the EntryPointFilePath MSBuild property
        // (made compiler-visible by the NetAnalyzers targets), and only reports when that file uses an
        // '#:include' or '#:ref' directive without a leading '#!' line. The compiler rejects '#:'
        // directives in a normal project unless the FileBasedProgram feature flag is on.
        using var project = await CreateProjectBuilderAsync(properties:
        [
            ("Features", "FileBasedProgram"),
            ("EntryPointFilePath", "$([System.IO.Path]::Combine($(MSBuildProjectDirectory), 'Program.cs'))"),
        ]);
        await project.AddFileAsync(
            "Program.cs",
            """
            #:include Helpers.cs
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2266").ShouldBeTrue();
    }

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
