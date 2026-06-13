// Copyright (c) Codurance. All rights reserved.

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Opinionated.DotNet.CodingStandards.Tooling;

public static class DescriptorExtractor
{
    public static IReadOnlyList<RuleDescriptor> Extract(IEnumerable<string> dllPaths)
    {
        var paths = dllPaths.ToList();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var descriptors = new List<RuleDescriptor>();
        var loader = new SimpleAnalyzerAssemblyLoader(paths);

        // Pre-load all DLLs so that inter-DLL dependencies (e.g. CSharp.NetAnalyzers →
        // NetAnalyzers) resolve when GetAnalyzers() is called later.
        foreach (var path in paths)
        {
            try
            {
                loader.LoadFromPath(path);
            }
            catch (BadImageFormatException)
            {
                // Best-effort; dependency may not need all DLLs
            }
            catch (FileLoadException)
            {
                // Best-effort; dependency may not need all DLLs
            }
            catch (IOException)
            {
                // Best-effort; dependency may not need all DLLs
            }
        }

        foreach (var dllPath in paths)
        {
            CollectFromDll(dllPath, loader, seen, descriptors);
        }

        return descriptors;
    }

    private static void CollectFromDll(
        string dllPath,
        IAnalyzerAssemblyLoader loader,
        ISet<string> seen,
        ICollection<RuleDescriptor> descriptors)
    {
        var reference = new AnalyzerFileReference(dllPath, loader);
        reference.AnalyzerLoadFailed += (_, _) => { };

        try
        {
            foreach (var analyzer in reference.GetAnalyzers(LanguageNames.CSharp))
            {
                CollectFromAnalyzer(analyzer, seen, descriptors);
            }
        }
        catch (BadImageFormatException)
        {
            // Skip DLLs or analyzers that fail to load or instantiate
        }
        catch (ReflectionTypeLoadException)
        {
            // Skip DLLs or analyzers that fail to load or instantiate
        }
        catch (InvalidOperationException)
        {
            // Skip DLLs or analyzers that fail to load or instantiate
        }
    }

    private static void CollectFromAnalyzer(
        DiagnosticAnalyzer analyzer,
        ISet<string> seen,
        ICollection<RuleDescriptor> descriptors)
    {
        foreach (var diagnostic in analyzer.SupportedDiagnostics)
        {
            if (!seen.Add(diagnostic.Id))
            {
                continue;
            }

            descriptors.Add(new RuleDescriptor(
                diagnostic.Id,
                diagnostic.Title.ToString(),
                diagnostic.HelpLinkUri ?? string.Empty,
                MapSeverity(diagnostic.DefaultSeverity),
                diagnostic.IsEnabledByDefault));
        }
    }

    private static RuleDefaultSeverity MapSeverity(DiagnosticSeverity severity) => severity switch
    {
        DiagnosticSeverity.Hidden => RuleDefaultSeverity.Hidden,
        DiagnosticSeverity.Info => RuleDefaultSeverity.Info,
        DiagnosticSeverity.Warning => RuleDefaultSeverity.Warning,
        DiagnosticSeverity.Error => RuleDefaultSeverity.Error,
        _ => RuleDefaultSeverity.Hidden,
    };

    private sealed class SimpleAnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        // Index: assembly simple name (OrdinalIgnoreCase) → full path of the DLL.
        // Pre-populated from all DLL paths passed to Extract so that inter-DLL dependencies
        // (e.g. Microsoft.CodeAnalysis.CSharp.NetAnalyzers → Microsoft.CodeAnalysis.NetAnalyzers)
        // resolve even when the dependency lives in a sibling directory.
        private readonly Dictionary<string, string> _knownPaths;

        public SimpleAnalyzerAssemblyLoader(IEnumerable<string> dllPaths)
        {
            _knownPaths = dllPaths
                .GroupBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            // LoadFrom only probes the DLL's own directory for dependencies; hook the resolve
            // event so cross-directory dependencies (platform-neutral → language-specific) work.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        private Assembly? ResolveAssembly(object? sender, ResolveEventArgs args)
        {
            var simpleName = new AssemblyName(args.Name).Name;
            if (simpleName is not null && _knownPaths.TryGetValue(simpleName, out var path))
            {
                return Assembly.LoadFrom(path);
            }

            return null;
        }

        public void AddDependencyLocation(string fullPath)
        {
            var name = Path.GetFileNameWithoutExtension(fullPath);
            _knownPaths.TryAdd(name, fullPath);
        }

        public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
    }
}
