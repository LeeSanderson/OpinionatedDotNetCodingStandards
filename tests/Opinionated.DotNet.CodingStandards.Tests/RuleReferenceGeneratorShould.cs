using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleReferenceGeneratorShould
{
    [Fact]
    public void FindRuleDocOnTaggedMethod()
    {
        var docs = RuleReferenceGenerator.CollectRuleDocs(typeof(RuleReferenceGeneratorShould).Assembly);

        docs.ContainsKey("CA1000").ShouldBeTrue();
        docs["CA1000"].Description.ShouldBe("Do not declare static members on generic types");
        docs["CA1000"].HelpLink.ShouldBe("https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000");
    }

    [Fact]
    public void ReconcileDetectsOrphanDoc()
    {
        var activeRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CA1000" };
        var entries = new List<RuleDocEntry>
        {
            new("CA9999", new RuleDocAttribute("CA9999", "Orphan rule"), IsClassLevel: false),
        };

        var result = RuleReferenceGenerator.Reconcile(activeRules, entries);

        result.OrphanDocs.ShouldContain("CA9999");
    }

    [Fact]
    public void ReconcileDetectsUncoveredRule()
    {
        var activeRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CA1000", "CA1001" };
        var entries = new List<RuleDocEntry>
        {
            new("CA1000", new RuleDocAttribute("CA1000", "Static members on generic types"), IsClassLevel: false),
        };

        var result = RuleReferenceGenerator.Reconcile(activeRules, entries);

        result.UncoveredRules.ShouldContain("CA1001");
    }

    [Fact]
    public void ReconcileDetectsDuplicateId()
    {
        var activeRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CA1000" };
        var entries = new List<RuleDocEntry>
        {
            new("CA1000", new RuleDocAttribute("CA1000", "First doc"), IsClassLevel: false),
            new("CA1000", new RuleDocAttribute("CA1000", "Second doc"), IsClassLevel: false),
        };

        var result = RuleReferenceGenerator.Reconcile(activeRules, entries);

        result.DuplicateIds.ShouldContain("CA1000");
    }

    [Fact]
    public void ReconcileDetectsMethodLevelWithUntestable()
    {
        var activeRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CA1000" };
        var entries = new List<RuleDocEntry>
        {
            new("CA1000", new RuleDocAttribute("CA1000", "desc") { Untestable = "Cannot test" }, IsClassLevel: false),
        };

        var result = RuleReferenceGenerator.Reconcile(activeRules, entries);

        result.InvariantViolations.ShouldHaveSingleItem();
        result.InvariantViolations[0].ShouldContain("CA1000");
        result.InvariantViolations[0].ShouldContain("method-level");
    }

    [Fact]
    public void ReconcileDetectsClassLevelWithoutUntestable()
    {
        var activeRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CA1000" };
        var entries = new List<RuleDocEntry>
        {
            new("CA1000", new RuleDocAttribute("CA1000", "desc"), IsClassLevel: true),
        };

        var result = RuleReferenceGenerator.Reconcile(activeRules, entries);

        result.InvariantViolations.ShouldHaveSingleItem();
        result.InvariantViolations[0].ShouldContain("CA1000");
        result.InvariantViolations[0].ShouldContain("class-level");
    }
}
