using CliWrap;
using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleReferenceFreshnessShould
{
    private static readonly string RootDirectory = PathHelpers.GetRootDirectory();
    private static readonly string ScriptPath =
        Path.Combine(RootDirectory, "scripts", "CheckRuleReferenceFreshness.cs");

    [Fact]
    public void ScriptExists()
    {
        File.Exists(ScriptPath).ShouldBeTrue(
            $"scripts/CheckRuleReferenceFreshness.cs not found at {ScriptPath}");
    }

    [Fact]
    public async Task ExitsZeroWhenDocumentIsUpToDate()
    {
        File.Exists(ScriptPath).ShouldBeTrue();

        var result = await Cli.Wrap("dotnet")
            .WithArguments([ScriptPath])
            .WithWorkingDirectory(RootDirectory)
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        result.ExitCode.ShouldBe(0, "freshness check should pass when docs/rule-reference.md is current");
    }
}
