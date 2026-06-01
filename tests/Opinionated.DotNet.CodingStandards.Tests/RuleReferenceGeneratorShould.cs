using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleReferenceGeneratorShould
{
    [Fact]
    public void FindRuleDocOnTaggedMethod()
    {
        var docs = RuleReferenceGenerator.CollectRuleDocs(typeof(CodeAnalysisRulesShould).Assembly);

        docs.ContainsKey("CA1000").ShouldBeTrue();
        docs["CA1000"].Description.ShouldBe("Do not declare static members on generic types");
        docs["CA1000"].HelpLink.ShouldBe("https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000");
    }
}
