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
        "CA1003", "CA1008", "CA1012", "CA1019",
        "CA1027", "CA1028", "CA1030", "CA1033", "CA1043", "CA1044",
        "CA1046", "CA1052", "CA1058", "CA1063",
        "CA1065", "CA1066",
        "CA1304", "CA1305", "CA1307", "CA1309", "CA1310", "CA1311",
        "CA1401", "CA1416", "CA1417", "CA1418", "CA1419", "CA1420", "CA1421", "CA1422",
        "CA1510", "CA1514", "CA1516",
        "CA1700", "CA1708", "CA1712", "CA1713", "CA1715", "CA1721", "CA1725", "CA1727",
        "CA1802", "CA1810", "CA1813", "CA1814", "CA1815",
        "CA1820", "CA1821", "CA1823", "CA1824", "CA1828",
        "CA1830", "CA1831", "CA1832", "CA1833", "CA1835",
        "CA1837", "CA1838", "CA1839", "CA1840", "CA1842", "CA1843", "CA1844",
        "CA1845", "CA1849", "CA1850", "CA1851", "CA1853",
        "CA1855", "CA1856", "CA1857", "CA1858", "CA1864",
        "CA1865", "CA1867", "CA1868", "CA1869", "CA1870", "CA1871", "CA1872",
        "CA1873", "CA1874", "CA1875",
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
        "IDE0010", "IDE0017", "IDE0018",
        "IDE0019", "IDE0028", "IDE0030", "IDE0033",
        "IDE0034", "IDE0039", "IDE0043", "IDE0045",
        "IDE0046", "IDE0051", "IDE0052", "IDE0056", "IDE0057",
        "IDE0060", "IDE0064", "IDE0066", "IDE0070",
        "IDE0074", "IDE0076", "IDE0078", "IDE0079",
        "IDE0083", "IDE0100", "IDE0130", "IDE0180",
        "IDE0200", "IDE0230", "IDE0240", "IDE0241", "IDE0250", "IDE0260", "IDE0270",
        "IDE0300", "IDE0301", "IDE0302", "IDE0303", "IDE0304", "IDE0305",
        "IDE0330",
        "IDE2000",
        "MA0015", "MA0017", "MA0019", "MA0022", "MA0023", "MA0027", "MA0029", "MA0030",
        "MA0035", "MA0037", "MA0040", "MA0042", "MA0044", "MA0052", "MA0054", "MA0055",
        "MA0056", "MA0060", "MA0063", "MA0067", "MA0068", "MA0070", "MA0072", "MA0073",
        "MA0079", "MA0082", "MA0085", "MA0086", "MA0087", "MA0088", "MA0090", "MA0093",
        "MA0099", "MA0100", "MA0103", "MA0108", "MA0113", "MA0114", "MA0128", "MA0129",
        "MA0130", "MA0134", "MA0140", "MA0143", "MA0144", "MA0145", "MA0146", "MA0151",
        "MA0152", "MA0158", "MA0159", "MA0160", "MA0166", "MA0173", "MA0176", "MA0178",
        "RS0031", "RS0035",
        "SA1649",
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
