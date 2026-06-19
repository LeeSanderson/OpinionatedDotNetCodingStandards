using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class StyleCopAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("SA1649", "File name should match first type name",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md")]
    public async Task RequireFileNameToMatchFirstTypeName()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("sample.cs", "class Program { static int Main() => 0; }");
        // File is named WrongName.cs but the first type is RightName — SA1649 fires.
        await project.AddFileAsync("WrongName.cs", "public class RightName { }");
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1649").ShouldBeTrue();
    }
}
