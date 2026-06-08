using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Opinionated.DotNet.CodingStandards.Tooling;

public static class DescriptorExtractor
{
    public static IReadOnlyList<RuleDescriptor> Extract(IEnumerable<string> dllPaths)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var descriptors = new List<RuleDescriptor>();
        var loader = new SimpleAnalyzerAssemblyLoader();

        foreach (var dllPath in dllPaths)
        {
            var reference = new AnalyzerFileReference(dllPath, loader);
            reference.AnalyzerLoadFailed += (_, _) => { };

            try
            {
                foreach (var analyzer in reference.GetAnalyzers(LanguageNames.CSharp))
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
            }
            catch (Exception)
            {
                // Skip DLLs or analyzers that fail to load or instantiate
            }
        }

        return descriptors;
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
        public void AddDependencyLocation(string fullPath) { }
        public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
    }
}
