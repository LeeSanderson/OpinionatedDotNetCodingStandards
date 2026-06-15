// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class PackageVersionShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public void ReflectNuspecPropertiesVersionOverride()
    {
        var nupkgFile = Directory.GetFiles(Fixture.PackageDirectory, "*.nupkg").Single();
        Path.GetFileNameWithoutExtension(nupkgFile).ShouldBe("Opinionated.DotNet.CodingStandards.999.9.9");
    }
}
