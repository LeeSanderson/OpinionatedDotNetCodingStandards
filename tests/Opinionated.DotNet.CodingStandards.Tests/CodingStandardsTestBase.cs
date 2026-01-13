using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class CodingStandardsTestBase(PackageFixture fixture, ITestOutputHelper testOutputHelper)
{
    protected PackageFixture Fixture => fixture;
    protected ITestOutputHelper TestOutputHelper => testOutputHelper;

    internal async Task<ProjectBuilder> CreateProjectBuilder(
        Dictionary<string, string>? properties = null,
        Dictionary<string, string>? packageReferences = null)
    {
        var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFile(properties, packageReferences);
        return project;
    }
}