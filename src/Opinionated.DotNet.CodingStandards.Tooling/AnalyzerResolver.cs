using System.Diagnostics;
using System.Text.Json;

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
        var dllPaths = await GetAnalyzerDllPathsAsync(projectPath, cancellationToken);
        var grouped = GroupByPackage(dllPaths);

        var packages = new List<AnalyzerPackageInfo>();
        foreach (var (packageId, editorConfigFileName) in PackageAllowMap)
        {
            if (grouped.TryGetValue(packageId, out var dlls))
            {
                var editorConfigPath = Path.Combine(analyzerEditorConfigDir, editorConfigFileName);
                packages.Add(new AnalyzerPackageInfo(packageId, editorConfigPath, dlls));
            }
        }

        return packages;
    }

    private static async Task<List<string>> GetAnalyzerDllPathsAsync(
        string projectPath,
        CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\" -getItem:Analyzer --nologo --verbosity:quiet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start dotnet process.");

        var stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        return ParseAnalyzerItemsFromJson(stdout);
    }

    private static List<string> ParseAnalyzerItemsFromJson(string output)
    {
        var jsonStart = output.IndexOf('{');
        if (jsonStart < 0)
        {
            return [];
        }

        try
        {
            using var doc = JsonDocument.Parse(output[jsonStart..]);
            if (!doc.RootElement.TryGetProperty("Items", out var items) ||
                !items.TryGetProperty("Analyzer", out var analyzers))
            {
                return [];
            }

            var paths = new List<string>();
            foreach (var analyzer in analyzers.EnumerateArray())
            {
                if (analyzer.TryGetProperty("Identity", out var identity))
                {
                    var path = identity.GetString();
                    if (path is not null)
                    {
                        paths.Add(path);
                    }
                }
            }

            return paths;
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static Dictionary<string, List<string>> GroupByPackage(List<string> dllPaths)
    {
        var grouped = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var dllPath in dllPaths)
        {
            var packageId = ExtractPackageId(dllPath);
            if (packageId is null || !PackageAllowMap.ContainsKey(packageId))
            {
                continue;
            }

            if (!grouped.TryGetValue(packageId, out var list))
            {
                grouped[packageId] = list = [];
            }

            list.Add(dllPath);
        }

        return grouped;
    }

    private static string? ExtractPackageId(string dllPath)
    {
        var normalized = dllPath.Replace('\\', '/');
        const string marker = "/.nuget/packages/";
        var idx = normalized.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return null;
        }

        var afterPackages = normalized[(idx + marker.Length)..];
        var slashIdx = afterPackages.IndexOf('/');
        return slashIdx < 0 ? null : afterPackages[..slashIdx];
    }
}
