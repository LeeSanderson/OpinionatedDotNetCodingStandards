using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Opinionated.DotNet.CodingStandards.Tooling;

public static class AnalyzerResolver
{
    // Maps NuGet package id (lowercase) to the corresponding editorconfig file name.
    // StyleCop ships as a metapackage; its analyzer DLL lives in the transitive
    // stylecop.analyzers.unstable package, so that id is used here.
    private static readonly IReadOnlyDictionary<string, string> PackageAllowMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["meziantou.analyzer"] = "Analyzer.Meziantou.Analyzer.editorconfig",
            ["microsoft.codeanalysis.bannedapianalyzers"] = "Analyzer.Microsoft.CodeAnalysis.BannedApiAnalyzers.editorconfig",
            ["microsoft.codeanalysis.netanalyzers"] = "Analyzer.Microsoft.CodeAnalysis.NetAnalyzers.editorconfig",
            ["sonaranalyzer.csharp"] = "Analyzer.SonarAnalyzer.CSharp.editorconfig",
            ["stylecop.analyzers.unstable"] = "Analyzer.StyleCop.Analyzers.Unstable.editorconfig",
        };

    public static Task<IReadOnlyList<AnalyzerPackageInfo>> ResolveAsync(
        string projectPath,
        string analyzerEditorConfigDir)
        => ResolveAsync(projectPath, analyzerEditorConfigDir, CancellationToken.None);

    public static async Task<IReadOnlyList<AnalyzerPackageInfo>> ResolveAsync(
        string projectPath,
        string analyzerEditorConfigDir,
        CancellationToken cancellationToken)
    {
        var projectDir = Path.GetDirectoryName(Path.GetFullPath(projectPath))!;
        var assetsFile = Path.Combine(projectDir, "obj", "project.assets.json");

        if (!File.Exists(assetsFile))
        {
            await RestoreProjectAsync(projectPath, cancellationToken);
        }

        var dllsByPackage = await GetAnalyzerDllsFromAssetsAsync(assetsFile, cancellationToken);

        var packages = new List<AnalyzerPackageInfo>();
        foreach (var (packageId, editorConfigFileName) in PackageAllowMap)
        {
            if (dllsByPackage.TryGetValue(packageId, out var dlls))
            {
                var editorConfigPath = Path.Combine(analyzerEditorConfigDir, editorConfigFileName);
                packages.Add(new AnalyzerPackageInfo(packageId, editorConfigPath, dlls));
            }
        }

        return packages;
    }

    private static async Task RestoreProjectAsync(string projectPath, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = Environment.ProcessPath ?? "dotnet",
            Arguments = $"restore \"{projectPath}\" --nologo --verbosity:quiet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start dotnet process.");

        await process.WaitForExitAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, List<string>>> GetAnalyzerDllsFromAssetsAsync(
        string assetsFilePath,
        CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(assetsFilePath, cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var packageFolders = ReadPackageFolders(root);
        if (packageFolders.Count == 0)
        {
            return [];
        }

        if (!root.TryGetProperty("libraries", out var libraries))
        {
            return [];
        }

        var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var library in libraries.EnumerateObject())
        {
            var slash = library.Name.IndexOf('/');
            if (slash < 0)
            {
                continue;
            }

            var packageId = library.Name[..slash];
            var version = library.Name[(slash + 1)..];

            if (!PackageAllowMap.ContainsKey(packageId))
            {
                continue;
            }

            if (!library.Value.TryGetProperty("files", out var files))
            {
                continue;
            }

            var candidates = CollectCsharpAnalyzerFiles(files);
            var selected = SelectBestRoslynVersion(candidates, LoadedRoslynVersion);

            if (selected.Count == 0)
            {
                continue;
            }

            var dlls = ResolveFullDllPaths(selected, packageId, version, packageFolders);

            if (dlls.Count > 0)
            {
                result[packageId] = dlls;
            }
        }

        return result;
    }

    private static List<string> ResolveFullDllPaths(
        IEnumerable<string> selected, string packageId, string version, IEnumerable<string> packageFolders)
    {
        var dlls = new List<string>();
        foreach (var relPath in selected)
        {
            var relativePart = Path.Combine(
                packageId,
                version,
                relPath.Replace('/', Path.DirectorySeparatorChar));

            // Try each package folder in order; use the first that has the file.
            var found = packageFolders
                .Select(packageFolder => Path.Combine(packageFolder, relativePart))
                .FirstOrDefault(File.Exists);
            if (found != null)
            {
                dlls.Add(found);
            }
        }

        return dlls;
    }

    private static List<string> ReadPackageFolders(JsonElement root)
    {
        if (!root.TryGetProperty("packageFolders", out var packageFolders))
        {
            return [];
        }

        var folders = new List<string>();
        foreach (var folder in packageFolders.EnumerateObject())
        {
            folders.Add(folder.Name);
        }

        return folders;
    }

    private static readonly Regex RoslynVersionPattern = new(@"^roslyn(\d+)\.(\d+)$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    // Segment counts for the supported analyzer path layouts under the "analyzers/" prefix.
    private const int FlatLegacySegments = 2;          // analyzers/<name>.dll
    private const int PlatformNeutralSegments = 3;     // analyzers/dotnet/<name>.dll
    private const int DirectCsharpSegments = 4;        // analyzers/dotnet/cs/<name>.dll
    private const int RoslynVersionedSegments = 5;     // analyzers/dotnet/roslynX.Y/cs/<name>.dll
    private const int RoslynFolderIndex = 2;           // index of the roslynX.Y segment in a versioned path
    private const int CsharpSegmentIndexInVersioned = 3; // index of "cs" in a roslyn-versioned path

    // Returns relative paths (forward-slash) for C# analyzer DLLs in the package.
    // Accepted patterns:
    //   analyzers/<name>.dll                      (flat legacy layout)
    //   analyzers/dotnet/<name>.dll               (platform-neutral)
    //   analyzers/dotnet/cs/<name>.dll            (direct C#, no sub-folder)
    //   analyzers/dotnet/roslyn<X>.<Y>/cs/<name>.dll  (roslyn-versioned C#)
    private static List<string> CollectCsharpAnalyzerFiles(JsonElement files)
    {
        var result = new List<string>();

        foreach (var fileElement in files.EnumerateArray())
        {
            var filePath = fileElement.GetString();
            if (filePath is null
                || !filePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                || filePath.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parts = filePath.Split('/');
            if (parts.Length >= FlatLegacySegments && parts[0] == "analyzers" && IsCsharpAnalyzerLayout(parts))
            {
                result.Add(filePath);
            }
        }

        return result;
    }

    private static bool IsCsharpAnalyzerLayout(string[] parts)
    {
        if (parts.Length == FlatLegacySegments)
        {
            return true; // flat legacy: analyzers/<name>.dll
        }

        if (parts.Length < PlatformNeutralSegments || parts[1] != "dotnet")
        {
            return false;
        }

        if (parts.Length == PlatformNeutralSegments)
        {
            return true; // platform-neutral: analyzers/dotnet/<name>.dll
        }

        if (parts.Length == DirectCsharpSegments && parts[RoslynFolderIndex] == "cs")
        {
            return true; // direct C#: analyzers/dotnet/cs/<name>.dll
        }

        if (parts.Length == RoslynVersionedSegments && RoslynVersionPattern.IsMatch(parts[RoslynFolderIndex]) && parts[CsharpSegmentIndexInVersioned] == "cs")
        {
            return true; // roslyn-versioned: analyzers/dotnet/roslynX.Y/cs/<name>.dll
        }

        return false;
    }

    // The Roslyn version this tooling process itself loads. A roslyn-versioned analyzer DLL built
    // against a NEWER Roslyn than this cannot be loaded here — AnalyzerFileReference.GetAnalyzers()
    // throws and DescriptorExtractor silently yields no descriptors for the whole package, which
    // reads downstream as "every rule went stale". So selection is capped at this version.
    private static readonly Version LoadedRoslynVersion =
        typeof(Microsoft.CodeAnalysis.SyntaxTree).Assembly.GetName().Version ?? new Version(0, 0);

    // When there are roslyn-versioned DLLs, keeps only those from the highest roslyn version that
    // the caller can actually load (folder version <= maxRoslynVersion). Flat legacy DLLs
    // (analyzers/<name>.dll) and platform-neutral DLLs are always included.
    public static IReadOnlyList<string> SelectBestRoslynVersion(IEnumerable<string> candidates, Version maxRoslynVersion)
    {
        var all = candidates.ToList();
        var flat = all.Where(p => p.Split('/').Length == FlatLegacySegments).ToList();
        var platformNeutral = all.Where(p => p.Split('/').Length == PlatformNeutralSegments).ToList();
        var versioned = all.Where(p => p.Split('/').Length == RoslynVersionedSegments).ToList();
        var direct = all.Where(p => p.Split('/').Length == DirectCsharpSegments).ToList();

        if (versioned.Count == 0)
        {
            return [.. flat, .. platformNeutral, .. direct];
        }

        var parsedFolders = versioned
            .Select(p => p.Split('/')[RoslynFolderIndex])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(folder => (folder, match: RoslynVersionPattern.Match(folder)))
            .Where(x => x.match.Success)
            .Select(x => (
                x.folder,
                version: new Version(int.Parse(x.match.Groups[1].Value), int.Parse(x.match.Groups[2].Value))))
            .OrderByDescending(x => x.version)
            .ToList();

        // Highest folder we can actually load. If every folder targets a newer Roslyn than we have,
        // fall back to the lowest one — it still won't load, but returning the *closest* candidate
        // keeps the failure diagnosable rather than arbitrary.
        var loadable = parsedFolders.Find(x => x.version <= maxRoslynVersion);
        var bestFolder = loadable.folder ?? (parsedFolders.Count > 0 ? parsedFolders[^1].folder : null);

        if (bestFolder is null)
        {
            return direct.Count > 0 ? direct : versioned;
        }

        return [
            .. flat,
            .. platformNeutral,
            .. versioned.Where(p => p.Split('/')[RoslynFolderIndex].Equals(bestFolder, StringComparison.OrdinalIgnoreCase)),
        ];
    }
}
