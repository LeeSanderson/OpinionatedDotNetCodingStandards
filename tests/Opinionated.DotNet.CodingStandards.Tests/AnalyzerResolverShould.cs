using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class AnalyzerResolverShould
{
    private static readonly Version Roslyn56 = new(5, 6);

    [Fact]
    public void SelectHighestRoslynFolderThatIsLoadable()
    {
        // Meziantou.Analyzer 3.0.177 ships roslyn4.8 … roslyn5.9. A tool referencing Roslyn 5.6
        // cannot load the 5.9 build, so 5.6 is the highest usable folder.
        List<string> candidates =
        [
            "analyzers/dotnet/roslyn4.8/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.0/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.9/cs/Meziantou.Analyzer.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, Roslyn56);

        selected.ShouldBe(["analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll"]);
    }

    [Fact]
    public void SelectAllDllsFromTheChosenRoslynFolder()
    {
        List<string> candidates =
        [
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.CodeFixers.dll",
            "analyzers/dotnet/roslyn5.9/cs/Meziantou.Analyzer.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, Roslyn56);

        selected.ShouldBe([
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.CodeFixers.dll",
        ], ignoreOrder: true);
    }

    [Fact]
    public void FallBackToTheLowestFolderWhenEveryFolderIsTooNew()
    {
        List<string> candidates =
        [
            "analyzers/dotnet/roslyn5.9/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn6.0/cs/Meziantou.Analyzer.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, Roslyn56);

        selected.ShouldBe(["analyzers/dotnet/roslyn5.9/cs/Meziantou.Analyzer.dll"]);
    }

    [Fact]
    public void CompareRoslynFolderVersionsNumericallyNotLexically()
    {
        // "roslyn4.14" > "roslyn4.8" numerically, but sorts lower as a string.
        List<string> candidates =
        [
            "analyzers/dotnet/roslyn4.8/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn4.14/cs/Meziantou.Analyzer.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, new Version(4, 14));

        selected.ShouldBe(["analyzers/dotnet/roslyn4.14/cs/Meziantou.Analyzer.dll"]);
    }

    [Fact]
    public void KeepFlatAndPlatformNeutralDllsAlongsideVersionedOnes()
    {
        List<string> candidates =
        [
            "analyzers/SonarAnalyzer.CSharp.dll",
            "analyzers/dotnet/Some.Analyzer.dll",
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll",
            "analyzers/dotnet/roslyn5.9/cs/Meziantou.Analyzer.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, Roslyn56);

        selected.ShouldBe([
            "analyzers/SonarAnalyzer.CSharp.dll",
            "analyzers/dotnet/Some.Analyzer.dll",
            "analyzers/dotnet/roslyn5.6/cs/Meziantou.Analyzer.dll",
        ], ignoreOrder: true);
    }

    [Fact]
    public void ReturnUnversionedCandidatesUnchangedWhenNoRoslynFoldersExist()
    {
        List<string> candidates =
        [
            "analyzers/SonarAnalyzer.CSharp.dll",
            "analyzers/dotnet/cs/Microsoft.CodeAnalysis.NetAnalyzers.dll",
        ];

        var selected = AnalyzerResolver.SelectBestRoslynVersion(candidates, Roslyn56);

        selected.ShouldBe(candidates, ignoreOrder: true);
    }
}
