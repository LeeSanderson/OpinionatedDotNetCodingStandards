#!/usr/bin/env dotnet
#:project ../src/Opinionated.DotNet.CodingStandards.Tooling/Opinionated.DotNet.CodingStandards.Tooling.csproj

using System.Text;
using Opinionated.DotNet.CodingStandards.Tooling;

var checkMode = args.Contains("--check");
var rootDir = GetRootDirectory();
var testProjectPath = Path.Combine(rootDir, "tests", "Opinionated.DotNet.CodingStandards.Tests", "Opinionated.DotNet.CodingStandards.Tests.csproj");
var analyzerEditorConfigDir = Path.Combine(rootDir, "packages", "Opinionated.DotNet.CodingStandards", "pkgsrc", "config", "analyzers");

Console.WriteLine(checkMode ? "Checking analyzer editorconfigs..." : "Updating analyzer editorconfigs...");

var packages = await AnalyzerResolver.ResolveAsync(testProjectPath, analyzerEditorConfigDir);

if (packages.Count == 0)
{
    await Console.Error.WriteLineAsync("No analyzer packages resolved. Is the project restored?");
    return 1;
}

var anyDrift = false;

foreach (var package in packages)
{
    var descriptors = DescriptorExtractor.Extract(package.DllPaths);
    var existingText = File.Exists(package.EditorConfigPath)
        ? await File.ReadAllTextAsync(package.EditorConfigPath)
        : string.Empty;

    var result = EditorConfigMergeGenerator.Generate(existingText, descriptors);
    var hasChanges = result.RewrittenText != existingText;

    Console.WriteLine($"\n{Path.GetFileName(package.EditorConfigPath)}:");
    Console.WriteLine($"  Added: {(result.AddedIds.Count > 0 ? string.Join(", ", result.AddedIds) : "(none)")}");
    Console.WriteLine($"  Stale: {(result.StaleIds.Count > 0 ? string.Join(", ", result.StaleIds) : "(none)")}");

    if (hasChanges)
    {
        anyDrift = true;
        if (!checkMode)
        {
            await File.WriteAllTextAsync(package.EditorConfigPath, result.RewrittenText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine("  Written.");
        }
        else
        {
            Console.WriteLine("  DRIFT DETECTED.");
        }
    }
    else
    {
        Console.WriteLine("  Up to date.");
    }
}

if (checkMode && anyDrift)
{
    await Console.Error.WriteLineAsync("\nCheck failed: one or more editorconfig files are out of date.");
    return 1;
}

Console.WriteLine(checkMode ? "\nCheck passed." : "\nDone.");
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
