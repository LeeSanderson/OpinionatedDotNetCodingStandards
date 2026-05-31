using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class PackagedReadmeShould
{
    private static readonly string ReadmePath = Path.Combine(
        PathHelpers.GetRootDirectory(),
        "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "README.md");

    private string Content => File.ReadAllText(ReadmePath);

    [Fact]
    public void ExplainHowToInstall()
    {
        Content.ShouldContain("dotnet add package");
    }

    [Fact]
    public void DocumentAllSevenBanToggles()
    {
        Content.ShouldContain("BanNonUtcDateApis");
        Content.ShouldContain("BanInvariantCultureStringComparisonApis");
        Content.ShouldContain("BanEnumTryParseWithoutIgnoreCaseApis");
        Content.ShouldContain("BanRoundWithoutMidpointRoundingApis");
        Content.ShouldContain("BanUseOfCultureInfoConstructorApis");
        Content.ShouldContain("BanUseOfTupleInFavourOfValueTupleApis");
        Content.ShouldContain("BanUseOfNewtonsoftJsonApis");
    }

    [Fact]
    public void ShowHowToOptOutOfABanToggle()
    {
        Content.ShouldContain("false");
    }

    [Fact]
    public void LinkToTheRuleReference()
    {
        Content.ShouldContain("rule-reference");
    }

    [Fact]
    public void IncludeStyleAndNamingProse()
    {
        Content.ShouldContain("var");
        Content.ShouldContain("PascalCase");
    }
}
