using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class ReleasePipelineShould
{
    private static readonly string PipelinePath =
        Path.Combine(PathHelpers.GetRootDirectory(), ".azure-pipelines", "release.yml");

    private static readonly string CiPath =
        Path.Combine(PathHelpers.GetRootDirectory(), ".azure-pipelines", "ci.yml");

    private string Content => File.ReadAllText(PipelinePath);

    [Fact]
    public void Exist()
    {
        File.Exists(PipelinePath).ShouldBeTrue(
            $".azure-pipelines/release.yml not found at {PipelinePath}");
    }

    [Fact]
    public void TriggerOnVersionTags()
    {
        Content.ShouldContain("v*");
        Content.ShouldContain("tags");
    }

    [Fact]
    public void UseSharedVariablesGroup()
    {
        Content.ShouldContain("SharedVariables");
    }

    [Fact]
    public void ReferenceNuGetApiKey()
    {
        Content.ShouldContain("NuGetApiKey");
    }

    [Fact]
    public void PackWithVersionFromTag()
    {
        Content.ShouldContain("NuspecProperties");
        Content.ShouldContain("PackageVersion");
    }

    [Fact]
    public void PushToNuGetOrg()
    {
        Content.ShouldContain("nuget.org");
        Content.ShouldContain("push");
    }

    [Fact]
    public void RunTestsBeforePublishing()
    {
        var packIndex = Content.IndexOf("dotnet pack", StringComparison.OrdinalIgnoreCase);
        var testIndex = Content.IndexOf("dotnet test", StringComparison.OrdinalIgnoreCase);
        (testIndex < packIndex).ShouldBeTrue("test step must appear before pack step in the pipeline");
    }

    [Fact]
    public void NotModifyCiPipeline()
    {
        var ciContent = File.ReadAllText(CiPath);
        ciContent.ShouldNotContain("NuGetApiKey");
    }
}
