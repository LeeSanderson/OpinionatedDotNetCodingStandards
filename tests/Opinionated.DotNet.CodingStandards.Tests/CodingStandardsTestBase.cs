// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class CodingStandardsTestBase(PackageFixture fixture, ITestOutputHelper testOutputHelper)
{
    protected PackageFixture Fixture => fixture;
    protected ITestOutputHelper TestOutputHelper => testOutputHelper;

    internal async Task<ProjectBuilder> CreateProjectBuilderAsync(
        (string Name, string Value)[]? properties = null,
        (string Name, string Version)[]? packageReferences = null,
        string[]? additionalFiles = null)
    {
        var project = new ProjectBuilder(fixture, testOutputHelper);
        await project.AddCsprojFileAsync(properties, packageReferences, additionalFiles);
        return project;
    }
}