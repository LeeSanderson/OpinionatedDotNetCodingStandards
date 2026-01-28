using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;


[Collection(nameof(PackageCollection))]
public class CodingStandardsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task IgnoreNameCanBeSimplifiedAsOnlyTreatedAsErrorsInIDE()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private readonly int _i = 1;
                public int Get() => this._i;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0003").ShouldBeFalse();
    }

    [Fact]
    public async Task RequireFileScopedNamespaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0161").ShouldBeTrue(); // Convert to file-scoped namespace
    }

    [Fact]
    public async Task AllowWarningsAsErrorsToBeDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [
            (Name: "TreatWarningsAsErrors", Value: "false"),
            (Name: "MSBuildTreatWarningsAsErrors", Value: "false"),
        ]);
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        // Convert to file-scoped namespace should now be a warning instead of an error
        buildOutput.HasWarning("IDE0161").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireVarInsteadOfExplicitType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    int i = 1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0007").ShouldBeTrue();
    }
}