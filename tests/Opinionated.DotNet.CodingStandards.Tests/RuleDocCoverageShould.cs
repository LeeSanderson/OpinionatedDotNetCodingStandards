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
        "CA2002", "CA2009", "CA2011", "CA2012", "CA2013", "CA2014", "CA2015",
        "CA2017", "CA2018", "CA2019", "CA2020", "CA2021", "CA2022", "CA2023", "CA2024",
        "CA2100", "CA2101", "CA2119", "CA2153",
        "CA2201", "CA2207", "CA2211", "CA2213", "CA2214", "CA2215",
        "CA2216", "CA2217", "CA2218", "CA2219", "CA2224", "CA2226", "CA2231", "CA2235",
        "CA2237", "CA2243", "CA2244", "CA2246", "CA2247",
        "CA2248", "CA2249", "CA2252", "CA2253", "CA2254", "CA2255",
        "CA2256", "CA2257", "CA2258", "CA2259", "CA2260", "CA2261", "CA2262", "CA2263",
        "CA2264", "CA2265",
        "CA2300", "CA2301", "CA2302", "CA2305", "CA2310", "CA2311", "CA2312", "CA2315",
        "CA2321", "CA2322", "CA2326", "CA2327", "CA2328", "CA2329", "CA2330",
        "CA2350", "CA2351", "CA2352", "CA2353", "CA2354", "CA2355", "CA2356",
        "CA2361", "CA2362",
        "CA3061", "CA3075", "CA3077", "CA3147",
        "CA5350", "CA5351", "CA5358", "CA5359", "CA5360", "CA5361", "CA5363", "CA5364",
        "CA5365", "CA5366", "CA5367", "CA5368", "CA5369", "CA5370", "CA5371", "CA5372",
        "CA5373", "CA5374", "CA5375", "CA5376", "CA5377", "CA5378", "CA5379", "CA5380",
        "CA5381", "CA5382", "CA5383", "CA5384", "CA5385", "CA5386", "CA5387", "CA5388",
        "CA5390", "CA5391", "CA5392", "CA5393", "CA5395", "CA5396", "CA5397", "CA5398",
        "CA5401", "CA5402", "CA5403", "CA5404", "CA5405",
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
