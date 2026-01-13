using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class CodingStandardsTestBase(PackageFixture fixture, ITestOutputHelper testOutputHelper)
{
    protected PackageFixture Fixture => fixture;
    protected ITestOutputHelper TestOutputHelper => testOutputHelper;

    internal async Task<ProjectBuilder> CreateProjectBuilder(
        (string Name, string Value)[]? properties = null,
        (string Name, string Version)[]? packageReferences = null)
    {
        var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile(properties, packageReferences);
        return project;
    }
}