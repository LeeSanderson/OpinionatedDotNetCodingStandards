using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class TransitiveConsumptionShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task EnforceRulesInProjectReferencingPackageIndirectly()
    {
        // Library project directly references the coding-standards package.
        // App project only has a ProjectReference to the library — no direct package ref.
        // The buildTransitive assets must flow from the package → lib → app.
        using var project = new ProjectBuilder(Fixture, TestOutputHelper);

        await project.AddFile("lib/lib.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="Opinionated.DotNet.CodingStandards" Version="*" />
              </ItemGroup>
            </Project>
            """);

        await project.AddFile("lib/Lib.cs", "// placeholder\r\n");

        await project.AddFile("test.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>exe</OutputType>
                <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
                <ErrorLog>BuildOutput.sarif,version=2.1</ErrorLog>
              </PropertyGroup>
              <ItemGroup>
                <ProjectReference Include="lib/lib.csproj" />
              </ItemGroup>
            </Project>
            """);

        await project.AddFile("Program.cs", "_ = System.DateTime.Now;");

        var buildOutput = await project.BuildAndGetOutput();
        buildOutput.HasError("RS0030").ShouldBeTrue();
    }
}
