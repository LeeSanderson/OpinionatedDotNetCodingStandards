using System.Text.Json;
using System.Xml.Linq;
using CliWrap;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.Helpers;

internal sealed class ProjectBuilder : IDisposable
{
    private const string BuildOutputFileName = "BuildOutput.sarif";

    private readonly TemporaryDirectory _directory;
    private readonly ITestOutputHelper _testOutputHelper;

    public string RootFolder => this._directory.FullPath;

    public ProjectBuilder(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;

        this._directory = TemporaryDirectory.Create();
        this._directory.CreateTextFile(
            "NuGet.config",
            $"""
                        <configuration>
                          <config>
                            <add key="globalPackagesFolder" value="{fixture.PackageDirectory}/packages" />
                          </config>
                          <packageSources>
                            <clear />
                            <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
                            <add key="TestSource" value="{fixture.PackageDirectory}" />
                          </packageSources>
                          <packageSourceMapping>
                            <packageSource key="nuget.org">
                                <package pattern="*" />
                            </packageSource>
                            <packageSource key="TestSource">
                                <package pattern="Opinionated.DotNet.CodingStandards" />
                            </packageSource>
                          </packageSourceMapping>
                        </configuration>
                        """);

        // File.Copy(Path.Combine(PathHelpers.GetRootDirectory(), "global.json"), this._directory.GetPath("global.json"));
    }

    public async ValueTask AddFile(string relativePath, string content) =>
        await File.WriteAllTextAsync(this._directory.GetPath(relativePath), content);

    public ValueTask AddCsprojFile(Dictionary<string, string>? properties = null, Dictionary<string, string>? packageReferences = null)
    {
        var propertyElement = BuildPropertyElement(properties);
        var referencesElement = BuildReferencesElement(packageReferences);

        var content = $"""
                       <Project Sdk="Microsoft.NET.Sdk">
                         <PropertyGroup>
                           <OutputType>exe</OutputType>
                           <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
                           <ImplicitUsings>enable</ImplicitUsings>
                           <Nullable>enable</Nullable>
                           <ErrorLog>{BuildOutputFileName},version=2.1</ErrorLog>
                         </PropertyGroup>
                         {propertyElement}

                         <ItemGroup>
                           <PackageReference Include="Opinionated.DotNet.CodingStandards" Version="*" />
                         </ItemGroup>
                         {referencesElement}
                       </Project>
                       """;

        return AddFile(this._directory.GetPath("test.csproj"), content);
    }

    private static XElement BuildReferencesElement(Dictionary<string, string>? packageReferences)
    {
        var referencesElement = new XElement("ItemGroup");
        if (packageReferences != null)
        {
            foreach (var reference in packageReferences)
            {
                var packageReference = new XElement("PackageReference");
                packageReference.SetAttributeValue("Include", reference.Key);
                packageReference.SetAttributeValue("Version", reference.Value);

                referencesElement.Add(packageReference);
            }
        }

        return referencesElement;
    }

    private static XElement BuildPropertyElement(Dictionary<string, string>? properties)
    {
        var propertyElement = new XElement("PropertyGroup");
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                propertyElement.Add(new XElement(prop.Key, prop.Value));
            }
        }

        return propertyElement;
    }

    public void Dispose()
    {
        _directory.Dispose();
    }

    public Task<BuildOutputFile> BuildAndGetOutput(string[]? buildArguments = null)
    {
        return ExecuteDotnetCommandAndGetOutput("build", buildArguments);
    }

    private async Task<BuildOutputFile> ExecuteDotnetCommandAndGetOutput(string command, string[]? buildArguments = null)
    {
        var result = await Cli.Wrap("dotnet")
            .WithWorkingDirectory(this._directory.FullPath)
            .WithArguments([command, .. (buildArguments ?? [])])
            .WithEnvironmentVariables(env => env.Set("CI", null).Set("GITHUB_ACTIONS", null))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(this._testOutputHelper.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(this._testOutputHelper.WriteLine))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        this._testOutputHelper.WriteLine("Process exit code: " + result.ExitCode);

        return await this.ReadBuildOutputFile();
    }

    private async Task<BuildOutputFile> ReadBuildOutputFile()
    {
        var bytes = await File.ReadAllBytesAsync(this._directory.GetPath(BuildOutputFileName));
        var buildOutputFile = JsonSerializer.Deserialize<BuildOutputFile>(bytes) ?? throw new InvalidOperationException("The sarif file is invalid");

        // this.AppendAdditionalResult(buildOutputFile);

        this._testOutputHelper.WriteLine("Sarif result:\n" + string.Join("\n", buildOutputFile.AllResults().Select(r => r.ToString())));
        return buildOutputFile;
    }
}