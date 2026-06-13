// Copyright (c) Codurance. All rights reserved.

namespace Opinionated.DotNet.CodingStandards.Tooling;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class RuleDocAttribute(string ruleId, string description) : Attribute
{
    public string RuleId { get; } = ruleId;
    public string Description { get; } = description;
    public string? HelpLink { get; init; }
    public string? Untestable { get; init; }
}
