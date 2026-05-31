using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RootReadmeShould
{
    private static readonly string ReadmePath =
        Path.Combine(PathHelpers.GetRootDirectory(), "README.md");

    private string Content => File.ReadAllText(ReadmePath);

    [Fact]
    public void Exist()
    {
        File.Exists(ReadmePath).ShouldBeTrue(
            $"README.md not found at {ReadmePath}");
    }

    [Fact]
    public void DescribeWhatThePackageIs()
    {
        Content.ShouldContain("NuGet");
        Content.ShouldContain("analyzer");
    }

    [Fact]
    public void DocumentHowToBuildAndTest()
    {
        Content.ShouldContain("dotnet build");
        Content.ShouldContain("dotnet test");
    }

    [Fact]
    public void DocumentTheRepositoryLayout()
    {
        Content.ShouldContain("src/");
        Content.ShouldContain("tests/");
        Content.ShouldContain("scripts/");
        Content.ShouldContain(".azure-pipelines/");
    }

    [Fact]
    public void DocumentTheReleaseProcess()
    {
        Content.ShouldContain("v*");
    }
}
