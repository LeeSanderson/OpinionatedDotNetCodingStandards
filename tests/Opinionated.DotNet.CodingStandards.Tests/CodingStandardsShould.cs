using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class CodingStandardsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<PackageFixture>
{
    [Fact]
    public async Task BanNonUtcDates()
    {
        using var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile();
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();
        Assert.True(buildOutput.HasError("RS0030"));
    }
}