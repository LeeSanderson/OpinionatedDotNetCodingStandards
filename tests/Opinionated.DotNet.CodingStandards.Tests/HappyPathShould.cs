// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class HappyPathShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task ProduceZeroDiagnosticsForFullyCompliantCode()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", "return;\r\n");
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.AllResults().ShouldBeEmpty();
    }
}
