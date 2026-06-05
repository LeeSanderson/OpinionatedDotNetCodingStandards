using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleDocCoverageShould
{
    private static readonly string AnalyzerDir =
        Path.Combine(PathHelpers.GetRootDirectory(), "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "config", "analyzers");

    private static readonly string EditorConfigPath =
        Path.Combine(PathHelpers.GetRootDirectory(), "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "config", "Opinionated.editorconfig");

    // All active rules not yet covered by [RuleDoc]; shrink this list as tests are added.
    private static readonly IReadOnlyCollection<string> KnownUncovered = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CA2017", "CA2023",
        "CA2100",
        "CA2253", "CA2254", "CA2258",
        "EnableGenerateDocumentationFile",
    };

    [Fact]
    public void AllActiveRulesAreCovered()
    {
        var result = RuleReferenceGenerator.Reconcile(AnalyzerDir, typeof(RuleDocCoverageShould).Assembly, KnownUncovered, EditorConfigPath);

        result.UncoveredRules.ShouldBeEmpty(
            $"Active rules not covered by [RuleDoc] or KnownUncovered:{Environment.NewLine}{string.Join(Environment.NewLine, result.UncoveredRules)}");
        result.OrphanDocs.ShouldBeEmpty(
            $"[RuleDoc] attributes referencing inactive rules:{Environment.NewLine}{string.Join(Environment.NewLine, result.OrphanDocs)}");
        result.DuplicateIds.ShouldBeEmpty(
            $"Rule IDs documented more than once:{Environment.NewLine}{string.Join(Environment.NewLine, result.DuplicateIds)}");
        result.InvariantViolations.ShouldBeEmpty(
            $"[RuleDoc] invariant violations:{Environment.NewLine}{string.Join(Environment.NewLine, result.InvariantViolations)}");
    }
}
