// Copyright (c) Codurance. All rights reserved.

using System.Reflection;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class RuleDocAttributeShould
{
    [Fact]
    public void StoreRuleIdAndDescription()
    {
        var attr = new RuleDocAttribute("CA1000", "Do not declare static members on generic types");

        attr.RuleId.ShouldBe("CA1000");
        attr.Description.ShouldBe("Do not declare static members on generic types");
        attr.HelpLink.ShouldBeNull();
        attr.Untestable.ShouldBeNull();
    }

    [Fact]
    public void AcceptOptionalHelpLinkAndUntestable()
    {
        var attr = new RuleDocAttribute("CA1000", "desc")
        {
            HelpLink = "https://example.com",
            Untestable = "Cannot be triggered by the build harness",
        };

        attr.HelpLink.ShouldBe("https://example.com");
        attr.Untestable.ShouldBe("Cannot be triggered by the build harness");
    }

    [Fact]
    public void AllowMultipleOnMethodsAndClasses()
    {
        var usage = typeof(RuleDocAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        usage.ValidOn.ShouldBe(AttributeTargets.Method | AttributeTargets.Class);
        usage.AllowMultiple.ShouldBeTrue();
    }
}
