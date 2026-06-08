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
            ["stylecop.analyzers.unstable"] = "Analyzer.StyleCop.Analyzers.Unstable.editorconfig",
        };

    public static async Task<IReadOnlyList<AnalyzerPackageInfo>> ResolveAsync(
        string projectPath,
        string analyzerEditorConfigDir,
        CancellationToken cancellationToken = default)
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
            FileName = "dotnet",
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
            var selected = SelectBestRoslynVersion(candidates);

            if (selected.Count == 0)
            {
                continue;
            }

            var dlls = new List<string>();
            foreach (var relPath in selected)
            {
                var relativePart = Path.Combine(
                    packageId.ToLowerInvariant(),
                    version,
                    relPath.Replace('/', Path.DirectorySeparatorChar));

                // Try each package folder in order; use the first that has the file.
                foreach (var packageFolder in packageFolders)
                {
                    var fullPath = Path.Combine(packageFolder, relativePart);
                    if (File.Exists(fullPath))
                    {
                        dlls.Add(fullPath);
                        break;
                    }
                }
            }

            if (dlls.Count > 0)
            {
                result[packageId] = dlls;
            }
        }

        return result;
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

    private static readonly Regex RoslynVersionPattern = new(@"^roslyn(\d+)\.(\d+)$", RegexOptions.IgnoreCase);

    // Returns relative paths (forward-slash) for C# analyzer DLLs in the package.
    // Accepted patterns:
    //   analyzers/dotnet/<name>.dll              (platform-neutral)
    //   analyzers/dotnet/cs/<name>.dll           (direct C#, no sub-folder)
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
            if (parts.Length < 3 || parts[0] != "analyzers" || parts[1] != "dotnet")
            {
                continue;
            }

            if (parts.Length == 3)
            {
                // Platform-neutral: analyzers/dotnet/<name>.dll
                result.Add(filePath);
            }
            else if (parts.Length == 4 && parts[2] == "cs")
            {
                // Direct C#: analyzers/dotnet/cs/<name>.dll
                result.Add(filePath);
            }
            else if (parts.Length == 5 && RoslynVersionPattern.IsMatch(parts[2]) && parts[3] == "cs")
            {
                // Roslyn-versioned C#: analyzers/dotnet/roslynX.Y/cs/<name>.dll
                result.Add(filePath);
            }
        }

        return result;
    }

    // When there are roslyn-versioned DLLs, keeps only those from the highest roslyn version.
    // Platform-neutral DLLs (analyzers/dotnet/<name>.dll) are always included.
    private static List<string> SelectBestRoslynVersion(List<string> candidates)
    {
        var platformNeutral = candidates.Where(p => p.Split('/').Length == 3).ToList();
        var versioned = candidates.Where(p => p.Split('/').Length == 5).ToList();
        var direct = candidates.Where(p => p.Split('/').Length == 4).ToList();

        if (versioned.Count == 0)
        {
            return [.. platformNeutral, .. direct];
        }

        var bestFolder = versioned
            .Select(p => p.Split('/')[2])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(folder =>
            {
                var m = RoslynVersionPattern.Match(folder);
                return m.Success
                    ? (folder, major: int.Parse(m.Groups[1].Value), minor: int.Parse(m.Groups[2].Value))
                    : (folder, major: -1, minor: -1);
            })
            .Where(x => x.major >= 0)
            .OrderByDescending(x => x.major)
            .ThenByDescending(x => x.minor)
            .Select(x => x.folder)
            .FirstOrDefault();

        if (bestFolder is null)
        {
            return direct.Count > 0 ? direct : versioned;
        }

        return [
            .. platformNeutral,
            .. versioned.Where(p => p.Split('/')[2].Equals(bestFolder, StringComparison.OrdinalIgnoreCase)),
        ];
    }
}
