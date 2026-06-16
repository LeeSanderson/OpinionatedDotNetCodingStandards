namespace Opinionated.DotNet.CodingStandards.Tooling;

public record AnalyzerPackageInfo(
    string PackageId,
    string EditorConfigPath,
    IReadOnlyList<string> DllPaths);
