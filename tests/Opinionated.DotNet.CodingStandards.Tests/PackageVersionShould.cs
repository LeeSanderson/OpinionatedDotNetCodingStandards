using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

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
