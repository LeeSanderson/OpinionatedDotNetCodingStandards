using System.Text.Json;
using System.Xml.Linq;
using CliWrap;

namespace Opinionated.DotNet.CodingStandards.Tests.Helpers;

internal sealed class ProjectBuilder : IDisposable
{
    private const string BuildOutputFileName = "BuildOutput.sarif";

    private readonly TemporaryDirectory _directory;
    private readonly ITestOutputHelper _testOutputHelper;

    public string RootFolder => _directory.FullPath;

    public ProjectBuilder(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _directory = TemporaryDirectory.Create();
        _directory.CreateTextFile(
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

    }

    public async ValueTask AddFileAsync(string relativePath, string content)
    {
        var path = _directory.GetPath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content);
    }

    public ValueTask AddCsprojFileAsync(
        (string Name, string Value)[]? properties = null,
        (string Name, string Version)[]? packageReferences = null,
        string[]? additionalFiles = null)
    {
        var propertyElement = BuildPropertyElement(properties);
        var referencesElement = BuildReferencesElement(packageReferences);
        var additionalFilesElement = BuildAdditionalFilesElement(additionalFiles);

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
                         {additionalFilesElement}
                       </Project>
                       """;

        return AddFileAsync(_directory.GetPath("test.csproj"), content);
    }

    private static XElement BuildReferencesElement((string Name, string Version)[]? packageReferences)
    {
        var referencesElement = new XElement("ItemGroup");
        if (packageReferences != null)
        {
            foreach (var reference in packageReferences)
            {
                var packageReference = new XElement("PackageReference");
                packageReference.SetAttributeValue("Include", reference.Name);
                packageReference.SetAttributeValue("Version", reference.Version);

                referencesElement.Add(packageReference);
            }
        }

        return referencesElement;
    }

    private static XElement BuildAdditionalFilesElement(string[]? additionalFiles)
    {
        var element = new XElement("ItemGroup");
        if (additionalFiles != null)
        {
            foreach (var file in additionalFiles)
            {
                var item = new XElement("AdditionalFiles");
                item.SetAttributeValue("Include", file);
                element.Add(item);
            }
        }

        return element;
    }

    private static XElement BuildPropertyElement((string Name, string Value)[]? properties)
    {
        var propertyElement = new XElement("PropertyGroup");
        if (properties != null)
        {
            foreach (var prop in properties)
            {
                propertyElement.Add(new XElement(prop.Name, prop.Value));
            }
        }

        return propertyElement;
    }

    public void Dispose()
    {
        _directory.Dispose();
    }

    public Task<BuildOutputFile> BuildAndGetOutputAsync(string[]? buildArguments = null)
    {
        return ExecuteDotnetCommandAndGetOutputAsync("build", buildArguments);
    }

    private async Task<BuildOutputFile> ExecuteDotnetCommandAndGetOutputAsync(string command, string[]? buildArguments = null)
    {
        var result = await Cli.Wrap("dotnet")
            .WithWorkingDirectory(_directory.FullPath)
            .WithArguments([command, .. (buildArguments ?? [])])
            .WithEnvironmentVariables(env => env.Set("CI", null).Set("GITHUB_ACTIONS", null))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(_testOutputHelper.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(_testOutputHelper.WriteLine))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        _testOutputHelper.WriteLine("Process exit code: " + result.ExitCode);

        return await ReadBuildOutputFileAsync();
    }

    private async Task<BuildOutputFile> ReadBuildOutputFileAsync()
    {
        var bytes = await File.ReadAllBytesAsync(_directory.GetPath(BuildOutputFileName));
        var buildOutputFile = JsonSerializer.Deserialize<BuildOutputFile>(bytes) ?? throw new InvalidOperationException("The sarif file is invalid");

        _testOutputHelper.WriteLine("Sarif result:\n" + string.Join("\n", buildOutputFile.AllResults().Select(r => r.ToString())));
        return buildOutputFile;
    }
}
