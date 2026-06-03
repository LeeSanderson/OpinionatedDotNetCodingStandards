using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class StyleCopAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("SA1649", "File name should match first type name",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md")]
    public async Task RequireFileNameToMatchFirstTypeName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", "class Program { static int Main() => 0; }");
        // File is named WrongName.cs but the first type is RightName — SA1649 fires.
        await project.AddFile("WrongName.cs", "public class RightName { }");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("SA1649").ShouldBeTrue();
    }
}
