using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class HappyPathShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task ProduceZeroDiagnosticsForFullyCompliantCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", "return;\r\n");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.AllResults().ShouldBeEmpty();
    }
}
