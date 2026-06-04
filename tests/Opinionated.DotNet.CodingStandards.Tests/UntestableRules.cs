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
[RuleDoc("IDE0030", "Use coalesce expression",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0030",
    Untestable = "In .NET 10 Roslyn the build-time analyzer does not fire IDE0030 for nullable value type coalesce patterns; IDE0055 fires instead and IDE0030 is absent from SARIF output even when IDE0055 is suppressed. IDE0031 has the same symptom")]
[RuleDoc("IDE0064", "Make struct fields writable",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0064",
    Untestable = "In C# 12+ assigning a readonly field after a this() constructor call is compile error CS0191, not an IDE diagnostic")]
[RuleDoc("IDE0074", "Use compound assignment",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0074",
    Untestable = "In .NET 10 Roslyn, x = x ?? y (null-coalescing compound assignment) fires as IDE0054 (general compound assignment) not IDE0074; the two rules share the same diagnostic trigger in this analyzer version")]
[RuleDoc("IDE0070", "Use 'System.HashCode'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0070",
    Untestable = "In .NET 10 Roslyn, override GetHashCode() returning a manual combination fires IDE0055 (formatter) instead of IDE0070; the style fix triggers a formatter interaction that prevents IDE0070 from appearing in build SARIF")]
[RuleDoc("IDE0079", "Remove unnecessary suppression",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0079",
    Untestable = "In .NET 10 Roslyn, unnecessary SuppressMessage attributes fire IDE0055 (formatter) instead of IDE0079; the style fix triggers a formatter interaction that prevents IDE0079 from appearing in build SARIF")]
[RuleDoc("IDE0260", "Use pattern matching",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260",
    Untestable = "In .NET 10 Roslyn, 'obj as T != null' null-check pattern fires IDE0055 (formatter) instead of IDE0260; the style fix triggers a formatter interaction that prevents IDE0260 from appearing in build SARIF")]
[RuleDoc("IDE0302", "Simplify collection initialization",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302",
    Untestable = "In .NET 10 Roslyn, empty collection factory methods (Array.Empty, Enumerable.Empty, ImmutableArray<T>.Empty) fire as IDE0301 (collection initialization) not IDE0302; the empty-specific rule is subsumed by IDE0301 in the build analyzer")]
[RuleDoc("IDE0304", "Simplify collection initialization",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304",
    Untestable = "In .NET 10 Roslyn, ImmutableArray<T>.Empty fires as IDE0301 (collection initialization) not IDE0304; the ImmutableArray-specific empty collection rule is subsumed by IDE0301 in the build analyzer")]
public static class UntestableRules { }
