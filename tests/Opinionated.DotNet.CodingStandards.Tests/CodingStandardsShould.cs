using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class CodingStandardsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<PackageFixture>
{
    [Fact]
    public async Task NotFailWhenThisUsedDespiteNameCanBeSimplifiedBeingEnabledAsError()
    {
        using var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Program
            {
                private readonly int _i = 1;
                public int Get() => this._i;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0003").ShouldBeFalse();
    }

    [Fact]
    public async Task BanNonUtcDates()
    {
        using var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile();
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanInvariantCultureStringComparision()
    {
        using var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile();
        await project.AddFile("sample.cs", "_ = \"a test\".IndexOf(\"test\", StringComparison.InvariantCulture);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanEnumTryParseWithoutIgnoreCase()
    {
        using var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile();
        await project.AddFile("sample.cs", "_ = Enum.TryParse<StringComparison>(\"StringComparison.Ordinal\", out _);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }
}