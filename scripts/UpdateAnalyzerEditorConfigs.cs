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
var anyExtractionFailure = false;

foreach (var package in packages)
{
    var outcome = await ProcessPackageAsync(package, checkMode);
    anyDrift |= outcome.HasDrift;
    anyExtractionFailure |= outcome.ExtractionFailed;
}

if (anyExtractionFailure)
{
    await Console.Error.WriteLineAsync(
        "\nAborted: no rules could be extracted from one or more analyzer packages (see above)."
        + " Those editorconfigs were left untouched.");
    return 1;
}

if (checkMode && anyDrift)
{
    await Console.Error.WriteLineAsync("\nCheck failed: one or more editorconfig files are out of date.");
    return 1;
}

Console.WriteLine(checkMode ? "\nCheck passed." : "\nDone.");
return 0;

static async Task<(bool HasDrift, bool ExtractionFailed)> ProcessPackageAsync(AnalyzerPackageInfo package, bool checkMode)
{
    var descriptors = DescriptorExtractor.Extract(package.DllPaths);

    // Zero descriptors never means "this package has no rules" — it means the analyzer DLL failed
    // to load (typically built against a newer Roslyn than this tool references). Rewriting on that
    // basis would report every existing rule as stale, so refuse instead of writing.
    if (descriptors.Count == 0)
    {
        await ReportExtractionFailureAsync(package);
        return (HasDrift: false, ExtractionFailed: true);
    }

    var existingText = File.Exists(package.EditorConfigPath)
        ? await File.ReadAllTextAsync(package.EditorConfigPath)
        : string.Empty;

    var result = EditorConfigMergeGenerator.Generate(existingText, descriptors);
    var hasChanges = result.RewrittenText != existingText;

    Console.WriteLine($"\n{Path.GetFileName(package.EditorConfigPath)}:");
    Console.WriteLine($"  Added: {(result.AddedIds.Count > 0 ? string.Join(", ", result.AddedIds) : "(none)")}");
    Console.WriteLine($"  Stale: {(result.StaleIds.Count > 0 ? string.Join(", ", result.StaleIds) : "(none)")}");

    if (!hasChanges)
    {
        Console.WriteLine("  Up to date.");
        return (HasDrift: false, ExtractionFailed: false);
    }

    if (!checkMode)
    {
        await File.WriteAllTextAsync(package.EditorConfigPath, result.RewrittenText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    Console.WriteLine(checkMode ? "  DRIFT DETECTED." : "  Written.");
    return (HasDrift: true, ExtractionFailed: false);
}

static async Task ReportExtractionFailureAsync(AnalyzerPackageInfo package)
{
    Console.WriteLine($"\n{Path.GetFileName(package.EditorConfigPath)}:");
    await Console.Error.WriteLineAsync(
        $"  ERROR: extracted 0 rules from '{package.PackageId}'. The selected analyzer DLL most likely"
        + " targets a newer Roslyn than this tool references (see Microsoft.CodeAnalysis.CSharp in"
        + " Opinionated.DotNet.CodingStandards.Tooling.csproj). DLLs inspected:");

    foreach (var dll in package.DllPaths)
    {
        await Console.Error.WriteLineAsync($"    {dll}");
    }
}

static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}
