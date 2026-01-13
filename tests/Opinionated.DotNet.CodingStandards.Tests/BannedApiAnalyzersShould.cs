using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class BannedApiAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task BanNonUtcDates()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanNonUtcDatesWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: new Dictionary<string, string> { { "BanNonUtcDateApis", "false" } });
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanInvariantCultureStringComparision()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", """_ = "a test".IndexOf("test", StringComparison.InvariantCulture);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanEnumTryParseWithoutIgnoreCase()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", """_ = Enum.TryParse<StringComparison>("StringComparison.Ordinal", out _);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanRoundWithoutMidpointRoundingArgument()
    {
        using var project = await CreateProjectBuilder();

        // Replace with Math.Round(0.4, MidpointRounding.ToZero)
        await project.AddFile("sample.cs", "_ = Math.Round(0.4);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanUseOfCultureInfoConstructor()
    {
        using var project = await CreateProjectBuilder();

        // Replace with CultureInfo.GetCultureInfo("en-UK")
        await project.AddFile("sample.cs", """_ = new System.Globalization.CultureInfo("en-UK");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanUseOfTupleInFavourOfValueTuple()
    {
        using var project = await CreateProjectBuilder();

        // Replace with _ = new ValueTuple<int, int>(1, 2);
        await project.AddFile("sample.cs", "_ = new Tuple<int, int>(1, 2);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task BanUseOfNewtonSoftJson()
    {
        using var project = await CreateProjectBuilder(packageReferences: new Dictionary<string, string> { { "Newtonsoft.Json", "13.0.4" } });

        // Replace with System.Text.Json.JsonSerializer.Serialize("test");
        await project.AddFile("sample.cs", """_ = Newtonsoft.Json.JsonConvert.SerializeObject("test");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }
}