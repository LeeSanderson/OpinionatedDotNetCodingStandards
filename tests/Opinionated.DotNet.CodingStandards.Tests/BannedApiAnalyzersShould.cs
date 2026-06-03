using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class BannedApiAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("RS0031", "The list of banned symbols contains a duplicate",
        HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md")]
    public async Task ReportDuplicateBannedSymbolEntry()
    {
        using var project = await CreateProjectBuilder(additionalFiles: ["BannedSymbols.txt"]);
        // P:System.DateTime.Now is already in the package's BannedSymbols.NonUtcDates.txt — adding it
        // again in a second file triggers RS0031.
        await project.AddFile("BannedSymbols.txt", "P:System.DateTime.Now;Duplicate entry");
        await project.AddFile("sample.cs", "class Program { static int Main() => 0; }");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0031").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("RS0030", "Do not use banned APIs",
        HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md")]
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
        using var project = await CreateProjectBuilder(properties: [(Name: "BanNonUtcDateApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task NotBanInvariantCultureStringComparisonWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanInvariantCultureStringComparisonApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = "a test".IndexOf("test", StringComparison.InvariantCulture);""");
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
    public async Task NotBanEnumTryParseWithoutIgnoreCaseWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanEnumTryParseWithoutIgnoreCaseApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = Enum.TryParse<StringComparison>("StringComparison.Ordinal", out _);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
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
    public async Task NotBanRoundWithoutMidpointRoundingWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanRoundWithoutMidpointRoundingApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = Math.Round(0.4);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
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
    public async Task NotBanUseOfCultureInfoConstructorWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanUseOfCultureInfoConstructorApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = new System.Globalization.CultureInfo("en-UK");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
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
    public async Task NotBanUseOfTupleInFavourOfValueTupleWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanUseOfTupleInFavourOfValueTupleApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = new Tuple<int, int>(1, 2);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
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
    public async Task NotBanUseOfNewtonsoftJsonWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(
            properties: [(Name: "BanUseOfNewtonsoftJsonApis", Value: "false")],
            packageReferences: [(Name: "Newtonsoft.Json", Version: "13.0.4")]);
        await project.AddFile("sample.cs", """_ = Newtonsoft.Json.JsonConvert.SerializeObject("test");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanUseOfNewtonSoftJson()
    {
        using var project = await CreateProjectBuilder(packageReferences: [(Name: "Newtonsoft.Json", Version: "13.0.4")]);

        // Replace with System.Text.Json.JsonSerializer.Serialize("test");
        await project.AddFile("sample.cs", """_ = Newtonsoft.Json.JsonConvert.SerializeObject("test");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }
}