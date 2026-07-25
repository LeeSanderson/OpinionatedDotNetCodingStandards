#!/usr/bin/env dotnet
using System.Diagnostics;
using System.Xml;

await Console.Out.WriteLineAsync("Checking that MSBuild import paths resolve with correct casing...");
var rootDirectory = GetRootDirectory();

var trackedPaths = await LoadTrackedPathsAsync(rootDirectory);
if (trackedPaths == null)
{
    return 1;
}

var propsAndTargetsFiles = FindPropsAndTargetsFiles(rootDirectory).ToList();
await Console.Out.WriteLineAsync($"Found {propsAndTargetsFiles.Count} .props/.targets file(s) to check.");

// Materialize (not lazily "All") so every file is checked and reported, not just the first failure.
var checkResults = propsAndTargetsFiles.Select(file => CheckImports(rootDirectory, file, trackedPaths)).ToList();
var succeeded = checkResults.TrueForAll(result => result);

if (succeeded)
{
    await Console.Out.WriteLineAsync("All MSBuild imports resolve with correct, case-sensitive casing.");
}

return succeeded ? 0 : 1;

static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}

static IEnumerable<string> FindPropsAndTargetsFiles(string rootDirectory)
{
    string[] searchPatterns = ["*.props", "*.targets"];
    return searchPatterns
        .SelectMany(pattern => Directory.EnumerateFiles(rootDirectory, pattern, SearchOption.AllDirectories))
        .Where(file => !IsExcludedFromScan(rootDirectory, file));
}

static bool IsExcludedFromScan(string rootDirectory, string file)
{
    var relativeSegments = Path.GetRelativePath(rootDirectory, file).Replace('\\', '/').Split('/');
    return Array.Exists(relativeSegments, segment => segment is "bin" or "obj" or ".git");
}

static string ResolveExecutablePath(string executableName)
{
    string[] candidateNames = OperatingSystem.IsWindows()
        ? [executableName + ".exe", executableName + ".cmd", executableName]
        : [executableName];

    var searchDirectories = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
        .Split(Path.PathSeparator);

    var resolvedPath = searchDirectories
        .SelectMany(_ => candidateNames, (directory, candidateName) => Path.Combine(directory, candidateName))
        .FirstOrDefault(File.Exists);

    return resolvedPath
        ?? throw new InvalidOperationException($"Could not locate '{executableName}' on the PATH.");
}

static async Task<HashSet<string>?> LoadTrackedPathsAsync(string rootDirectory)
{
    // Compare against the git-*tracked* tree (`git ls-tree`), not OS directory enumeration or
    // `git status`. On a case-insensitive/case-preserving filesystem (the default on Windows and
    // macOS), a case-only rename is invisible to `git status` (core.ignorecase=true masks it) even
    // though `git ls-tree` still reports the old, mismatched casing. Comparing against
    // OS-enumerated directory entries would silently pass even when the tracked tree disagrees
    // with the import paths - exactly the bug this script exists to catch.
    var startInfo = new ProcessStartInfo(ResolveExecutablePath("git"), "ls-tree -r HEAD --name-only")
    {
        WorkingDirectory = rootDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
    };

    using var process = Process.Start(startInfo);
    if (process == null)
    {
        await Console.Error.WriteLineAsync("Failed to start 'git ls-tree -r HEAD --name-only'.");
        return null;
    }

    var output = await process.StandardOutput.ReadToEndAsync();
    var error = await process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
    {
        await Console.Error.WriteLineAsync($"'git ls-tree' failed: {error}");
        return null;
    }

    return output
        .Split('\n')
        .Select(line => line.Trim('\r', '\n', ' '))
        .Where(line => line.Length > 0)
        .ToHashSet(StringComparer.Ordinal);
}

static bool CheckImports(string rootDirectory, string propsOrTargetsFile, HashSet<string> trackedPaths)
{
    var importingDirectory = Path.GetDirectoryName(propsOrTargetsFile)
        ?? throw new InvalidOperationException($"Cannot determine directory of {propsOrTargetsFile}");

    XmlDocument document = new();
    document.Load(propsOrTargetsFile);

    var succeeded = true;
    foreach (XmlNode import in document.GetElementsByTagName("Import"))
    {
        var projectAttr = import.Attributes?["Project"]?.Value;
        if (string.IsNullOrWhiteSpace(projectAttr) || projectAttr.Contains('*'))
        {
            continue;
        }

        var resolvedRelativePath = ResolveImportPath(rootDirectory, importingDirectory, projectAttr);
        if (trackedPaths.Contains(resolvedRelativePath))
        {
            continue;
        }

        var relativeImportingFile = Path.GetRelativePath(rootDirectory, propsOrTargetsFile).Replace('\\', '/');
        Console.Error.WriteLine(
            $"Casing mismatch: {relativeImportingFile} imports \"{projectAttr}\", which resolves to " +
            $"\"{resolvedRelativePath}\", but that exact, case-sensitive path is not present in the " +
            "git-tracked tree (git ls-tree -r HEAD).");
        succeeded = false;
    }

    return succeeded;
}

static string ResolveImportPath(string rootDirectory, string importingDirectory, string projectAttributeValue)
{
    var importingDirectorySlash = importingDirectory.Replace('\\', '/');
    var value = projectAttributeValue.Replace('\\', '/');
    value = value.Replace(
        "$(MSBuildThisFileDirectory)",
        importingDirectorySlash + "/",
        StringComparison.OrdinalIgnoreCase);

    var absolutePath = Path.GetFullPath(value, importingDirectory);
    return Path.GetRelativePath(rootDirectory, absolutePath).Replace('\\', '/');
}
