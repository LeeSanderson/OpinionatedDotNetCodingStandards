namespace Opinionated.DotNet.CodingStandards.Tooling;

public record RuleDescriptor(string Id, string Title, string HelpLink, RuleDefaultSeverity DefaultSeverity, bool EnabledByDefault);
