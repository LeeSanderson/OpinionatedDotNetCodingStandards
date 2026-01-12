using System.Text;
using CliWrap;

namespace Opinionated.DotNet.CodingStandards.Tests.Helpers;

/// <summary>
/// Build the Coding Standards NuGet package once for all tests
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class PackageFixture : IAsyncLifetime
{
    private readonly TemporaryDirectory _packageDirectory = TemporaryDirectory.Create();

    public string PackageDirectory => this._packageDirectory.FullPath;

    public async Task InitializeAsync()
    {
        var projectPath =
            Path.Combine(
                PathHelpers.GetRootDirectory(),
                "src",
                "Opinionated.DotNet.CodingStandards",
                "Opinionated.DotNet.CodingStandards.csproj");

        string[] args = ["pack", projectPath, "-p:NuspecProperties=version=999.9.9", "--output", this._packageDirectory.FullPath];
        var output = new StringBuilder();
        var result = await Cli.Wrap("dotnet")
            .WithArguments(args)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(output))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Error while creating the NuGet package:\n" + output);
        }
    }

    public Task DisposeAsync()
    {
        this._packageDirectory.Dispose();
        return Task.CompletedTask;
    }
}