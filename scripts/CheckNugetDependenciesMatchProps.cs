#!/usr/bin/env dotnet
using System.Xml;

Console.WriteLine("Checking that NuGet dependencies match .props file...");

var rootDirectory = GetRootDirectory();
var packagePropsFileName = Path.Combine(rootDirectory, "Directory.Packages.props");
if (!File.Exists(packagePropsFileName))
{
    Console.Error.WriteLine($"Could not find {packagePropsFileName}");
    return 1;
}

XmlDocument packageDoc = new();
packageDoc.Load(packagePropsFileName);
var packageReferences = packageDoc.GetElementsByTagName("PackageReference");
var packageDict = new Dictionary<string, string>();
foreach (XmlNode packageReference in packageReferences)
{
    var includeAttr = packageReference.Attributes?["Include"]?.Value;
    var versionAttr = packageReference.Attributes?["version"]?.Value;
    if (includeAttr != null && versionAttr != null)
    {
        packageDict[includeAttr] = versionAttr;
        Console.WriteLine($"Found package in props: {includeAttr} - {versionAttr}");
    }
}

var nuspecFileName =
    Path.Combine(
        rootDirectory,
        "packages",
        "Opinionated.DotNet.CodingStandards",
        "Opinionated.DotNet.CodingStandards.nuspec");
if (!File.Exists(nuspecFileName))
{
    Console.Error.WriteLine($"Could not find {nuspecFileName}");
    return 1;
}

XmlDocument nuspecDoc = new();
nuspecDoc.Load(nuspecFileName);
var nuspecMetadata = nuspecDoc.GetElementsByTagName("metadata")[0];
if (nuspecMetadata == null)
{
    Console.Error.WriteLine($"Could not find <metadata> in .nuspec file {nuspecFileName}");
    return 1;
}

var nuspecDependencies = nuspecMetadata.ChildNodes
    .OfType<XmlNode>()
    .FirstOrDefault(n => n.Name == "dependencies")?
    .ChildNodes
    .OfType<XmlNode>()
    .Where(n => n.Name == "dependency") ?? Enumerable.Empty<XmlNode>();
var nuspecDict = new Dictionary<string, string>();
foreach (var dependency in nuspecDependencies)
{
    var idAttr = dependency.Attributes?["id"]?.Value;
    var versionAttr = dependency.Attributes?["version"]?.Value;
    if (idAttr != null && versionAttr != null)
    {
        nuspecDict[idAttr] = versionAttr;
        Console.WriteLine($"Found package in nuspec: {idAttr} - {versionAttr}");
    }
}

foreach (var kvp in packageDict)
{
    var packageName = kvp.Key;
    var packageVersion = kvp.Value;
    if (nuspecDict.TryGetValue(packageName, out var nuspecVersion))
    {
        if (nuspecVersion != packageVersion)
        {
            Console.Error.WriteLine($"Version mismatch for package {packageName}: props version {packageVersion}, nuspec version {nuspecVersion}");
            return 1;
        }
    }
    else
    {
        Console.Error.WriteLine($"Package {packageName} found in props but not in nuspec");
        return 1;
    }
}

return 0;


static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}
