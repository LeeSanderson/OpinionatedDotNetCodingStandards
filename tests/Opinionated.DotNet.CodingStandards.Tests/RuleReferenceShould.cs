using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleReferenceShould
{
    private static readonly string RuleReferencePath =
        Path.Combine(PathHelpers.GetRootDirectory(), "docs", "rule-reference.md");

    [Fact]
    public void Exist()
    {
        File.Exists(RuleReferencePath).ShouldBeTrue(
            $"docs/rule-reference.md not found at {RuleReferencePath}. Run scripts/GenerateRuleReference.cs to create it.");
    }

    [Fact]
    public void ContainMarkdownTableHeader()
    {
        var content = File.ReadAllText(RuleReferencePath);
        content.ShouldContain("| Rule ID |");
        content.ShouldContain("| Description |");
        content.ShouldContain("| Severity |");
    }

    [Fact]
    public void IncludeKnownEnforcedRules()
    {
        var content = File.ReadAllText(RuleReferencePath);
        content.ShouldContain("MA0022");   // Meziantou warning
        content.ShouldContain("RS0030");   // BannedApiAnalyzers warning
        content.ShouldContain("CA1000");   // NetAnalyzers warning
    }

    [Fact]
    public void ExcludeRulesSetToNone()
    {
        var content = File.ReadAllText(RuleReferencePath);
        content.ShouldNotContain("MA0001");  // Meziantou none
        content.ShouldNotContain("CA1002");  // NetAnalyzers none
    }

    [Fact]
    public void ExcludeRulesSetToSilent()
    {
        var content = File.ReadAllText(RuleReferencePath);
        content.ShouldNotContain("IDE0005_gen");  // CSharp.CodeStyle silent
    }
}
