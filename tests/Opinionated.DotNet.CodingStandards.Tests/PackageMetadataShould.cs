using System.IO.Compression;
using System.Xml.Linq;
using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class PackageMetadataShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public void IncludeReleaseNotesLinkingToChangelog()
    {
        var releaseNotes = ReadNuspecElement("releaseNotes");
        releaseNotes.ShouldNotBeNullOrEmpty();
        releaseNotes.ShouldContain("CHANGELOG");
    }

    [Fact]
    public void IncludeDiscoveryTags()
    {
        var tags = ReadNuspecElement("tags");
        tags.ShouldNotBeNullOrEmpty();
        tags.ShouldContain("analyzers");
        tags.ShouldContain("editorconfig");
    }

    private string ReadNuspecElement(string elementName)
    {
        var nupkgFile = Directory.GetFiles(Fixture.PackageDirectory, "*.nupkg").Single();
        using var zip = ZipFile.OpenRead(nupkgFile);
        var nuspecEntry = zip.Entries.Single(e => e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        using var stream = nuspecEntry.Open();
        var doc = XDocument.Load(stream);
        return doc.Descendants().FirstOrDefault(e => e.Name.LocalName == elementName)?.Value ?? string.Empty;
    }
}
