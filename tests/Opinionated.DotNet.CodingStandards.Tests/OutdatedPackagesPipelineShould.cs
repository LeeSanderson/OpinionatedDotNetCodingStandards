using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class OutdatedPackagesPipelineShould
{
    private static readonly string PipelinePath =
        Path.Combine(PathHelpers.GetRootDirectory(), ".azure-pipelines", "outdated.yml");

    [Fact]
    public void Exist()
    {
        File.Exists(PipelinePath).ShouldBeTrue(
            $".azure-pipelines/outdated.yml not found at {PipelinePath}");
    }

    [Fact]
    public void HaveASchedulesTrigger()
    {
        var content = File.ReadAllText(PipelinePath);
        content.ShouldContain("schedules:");
    }

    [Fact]
    public void RunDotnetOutdated()
    {
        var content = File.ReadAllText(PipelinePath);
        content.ShouldContain("dotnet outdated");
    }

    [Fact]
    public void RestorePackagesBeforeCheck()
    {
        var content = File.ReadAllText(PipelinePath);
        content.ShouldContain("restore");
    }
}
