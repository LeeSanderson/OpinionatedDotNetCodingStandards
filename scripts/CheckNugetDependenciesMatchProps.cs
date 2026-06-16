#!/usr/bin/env dotnet
using System.Xml;

await Console.Out.WriteLineAsync("Checking that NuGet dependencies match .props file...");
var rootDirectory = GetRootDirectory();
var packageDict = await LoadPropsPackagesAsync(rootDirectory);
if (packageDict == null)
{
    return 1;
}

var nuspecDict = await LoadNuspecDependenciesAsync(rootDirectory);
if (nuspecDict == null)
{
    return 1;
}

return await ComparePackagesAsync(packageDict, nuspecDict) ? 0 : 1;

static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}

static async Task<Dictionary<string, string>?> LoadPropsPackagesAsync(string rootDirectory)
{
    var packagePropsFileName = Path.Combine(rootDirectory, "Directory.Packages.props");
    if (!File.Exists(packagePropsFileName))
    {
        await Console.Error.WriteLineAsync($"Could not find {packagePropsFileName}");
        return null;
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

    return packageDict;
}

static async Task<Dictionary<string, string>?> LoadNuspecDependenciesAsync(string rootDirectory)
{
    var nuspecFileName = Path.Combine(
        rootDirectory,
        "packages",
        "Opinionated.DotNet.CodingStandards",
        "Opinionated.DotNet.CodingStandards.nuspec");
    if (!File.Exists(nuspecFileName))
    {
        await Console.Error.WriteLineAsync($"Could not find {nuspecFileName}");
        return null;
    }

    XmlDocument nuspecDoc = new();
    nuspecDoc.Load(nuspecFileName);
    var nuspecMetadata = nuspecDoc.GetElementsByTagName("metadata")[0];
    if (nuspecMetadata == null)
    {
        await Console.Error.WriteLineAsync($"Could not find <metadata> in .nuspec file {nuspecFileName}");
        return null;
    }

    var nuspecDependencies = nuspecMetadata.ChildNodes
        .OfType<XmlNode>()
        .FirstOrDefault(n => n.Name == "dependencies")?
        .ChildNodes
        .OfType<XmlNode>()
        .Where(n => n.Name == "dependency") ?? [];

    var nuspecDict = nuspecDependencies
        .Select(d => (id: d.Attributes?["id"]?.Value, version: d.Attributes?["version"]?.Value))
        .Where(p => p.id != null && p.version != null)
        .ToDictionary(p => p.id!, p => p.version!);

    foreach (var (id, version) in nuspecDict)
    {
        Console.WriteLine($"Found package in nuspec: {id} - {version}");
    }

    return nuspecDict;
}

static async Task<bool> ComparePackagesAsync(Dictionary<string, string> packageDict, Dictionary<string, string> nuspecDict)
{
    foreach (var (packageName, packageVersion) in packageDict)
    {
        if (nuspecDict.TryGetValue(packageName, out var nuspecVersion))
        {
            if (nuspecVersion != packageVersion)
            {
                await Console.Error.WriteLineAsync($"Version mismatch for package {packageName}: props version {packageVersion}, nuspec version {nuspecVersion}");
                return false;
            }
        }
        else
        {
            await Console.Error.WriteLineAsync($"Package {packageName} found in props but not in nuspec");
            return false;
        }
    }

    return true;
}
