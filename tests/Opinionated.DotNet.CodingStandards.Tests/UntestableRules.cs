namespace Opinionated.DotNet.CodingStandards.Tests;

[RuleDoc("RS0035", "External access to internal symbols outside the restricted namespace(s) is prohibited",
    HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md",
    Untestable = "Requires cross-assembly setup with RestrictedInternalsVisibleToAttribute; not triggerable from a single-project build")]
[RuleDoc("IDE0001", "Simplify name",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0001",
    Untestable = "IDE-only; not emitted by build analysis")]
[RuleDoc("IDE0002", "Simplify member access",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0002",
    Untestable = "IDE-only; not emitted by build analysis")]
[RuleDoc("IDE0003", "Remove this or Me qualification",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0003",
    Untestable = "IDE-only; not emitted by build analysis")]
[RuleDoc("IDE0038", "Use pattern matching to avoid is check followed by a cast",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0038",
    Untestable = "IDE-only; not emitted by build analysis")]
[RuleDoc("IDE0084", "Use pattern matching (IsNot operator)",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0084",
    Untestable = "VB.NET-only operator; not applicable in C# projects")]
[RuleDoc("MA0023", "Add RegexOptions.ExplicitCapture",
    HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0023.md",
    Untestable = "SYSLIB1045 fires for all runtime Regex construction and appears to suppress MA0023 in Meziantou.Analyzer 2.0.286")]
[RuleDoc("MA0054", "Embed the caught exception as innerException",
    HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0054.md",
    Untestable = "MA0054 does not fire in the build harness for any catch-and-rethrow pattern; the analyzer's data-flow conditions are not met by single-project builds")]
[RuleDoc("MA0070", "Obsolete attributes should include explanations",
    HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0070.md",
    Untestable = "CA1041 covers the same null/empty ObsoleteAttribute message condition and fires instead of MA0070 in this harness")]
[RuleDoc("MA0130", "GetType() should not be used on System.Type instances",
    HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0130.md",
    Untestable = "Diagnostic ID is registered in the Meziantou.Analyzer 2.0.286 editorconfig but the analyzer implementation is absent in this version; the rule never fires")]
public static class UntestableRules { }
