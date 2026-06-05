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
    Untestable = "In .NET 10 Roslyn build analysis, IDE0070 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: the XOR GetHashCode pattern triggers IDE0055 across every file in the compilation, and replacing it with HashCode.Combine removes IDE0055 entirely. The rule uses the formatter as its build-mode enforcement mechanism.")]
[RuleDoc("IDE0079", "Remove unnecessary suppression",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0079",
    Untestable = "In .NET 10 Roslyn build analysis, IDE0079 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: an unnecessary SuppressMessage triggers IDE0055 while an equivalent necessary suppression does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
[RuleDoc("IDE0260", "Use pattern matching",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260",
    Untestable = "In .NET 10 Roslyn build analysis, IDE0260 emits IDE0055 at the containing type declaration instead of its own diagnostic ID; confirmed by control/violation probes: 'obj as T != null' triggers IDE0055 while the equivalent 'obj is T' does not. The rule uses the formatter as its build-mode enforcement mechanism.")]
[RuleDoc("IDE0302", "Simplify collection initialization",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302",
    Untestable = "In .NET 10 Roslyn, empty collection factory methods (Array.Empty, Enumerable.Empty, ImmutableArray<T>.Empty) fire as IDE0301 (collection initialization) not IDE0302; the empty-specific rule is subsumed by IDE0301 in the build analyzer")]
[RuleDoc("IDE0304", "Simplify collection initialization",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304",
    Untestable = "In .NET 10 Roslyn, ImmutableArray<T>.Empty fires as IDE0301 (collection initialization) not IDE0304; the ImmutableArray-specific empty collection rule is subsumed by IDE0301 in the build analyzer")]
[RuleDoc("CA1066", "Implement IEquatable when overriding Object.Equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1066",
    Untestable = "CA1066 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern where a class overrides Object.Equals(object) without implementing IEquatable<T>; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1066.severity = warning configured")]
[RuleDoc("CA1419", "Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1419",
    Untestable = "CA1419 does not fire in NetAnalyzers 10.0.x build analysis for a concrete public SafeHandle subclass without a parameterless constructor; exhaustive probing confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1419.severity = warning configured")]
[RuleDoc("CA1420", "Property, type, or attribute requires runtime marshalling",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1420",
    Untestable = "Requires [assembly: DisableRuntimeMarshalling] which has assembly-wide impact on all string and reference-type P/Invoke marshalling; cannot be applied in isolation in a single-project test without breaking unrelated code")]
[RuleDoc("CA1421", "This method uses runtime marshalling even when the 'DisableRuntimeMarshallingAttribute' is applied",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1421",
    Untestable = "Requires [assembly: DisableRuntimeMarshalling] which has assembly-wide impact on all string and reference-type P/Invoke marshalling; cannot be applied in isolation in a single-project test without breaking unrelated code")]
[RuleDoc("CA1516", "Use cross-platform intrinsics",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1516",
    Untestable = "Fires when platform-specific hardware intrinsics (SSE2, AVX2, etc.) are used where cross-platform Vector128/Vector256 alternatives exist; no realistic test-harness violation pattern exists without low-level SIMD code")]
[RuleDoc("CA1727", "Use PascalCase for named placeholders",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1727",
    Untestable = "Fires on non-PascalCase named placeholders in structured logging message templates (ILogger extension methods); Microsoft.Extensions.Logging is not available in the simple single-project build harness")]
[RuleDoc("CA1824", "Mark assemblies with NeutralResourcesLanguageAttribute",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1824",
    Untestable = "Fires only when an assembly contains embedded .resx resources but lacks NeutralResourcesLanguageAttribute; the test harness does not support adding EmbeddedResource items to the csproj")]
[RuleDoc("CA1828", "Do not use CountAsync() or LongCountAsync() when AnyAsync() can be used",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1828",
    Untestable = "Fires only for Entity Framework Core's CountAsync()/LongCountAsync() extension methods on IQueryable<T>; the full EF Core package is not practical to add to a standalone single-project harness")]
[RuleDoc("CA1873", "Avoid potentially expensive logging",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1873",
    Untestable = "Fires on string concatenation/interpolation in ILogger.LogXxx calls; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA1845", "Use span-based 'string.Concat'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1845",
    Untestable = "In .NET 10 NetAnalyzers build analysis, the canonical violation pattern string.Concat(s.Substring(0, 5), \"!\") fires IDE0057 (Substring can be simplified to a range indexer) instead of CA1845; CA1845 is absent from SARIF output even when IDE0057 is suppressed")]
[RuleDoc("CA1842", "Do not use 'WhenAll' with a single task",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1842",
    Untestable = "In .NET 10 NetAnalyzers build analysis, CA1842 does not fire for Task.WhenAll(singleTask) with either generic Task<T> or non-generic Task arguments; exhaustive probing with multiple patterns confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1842.severity = warning configured")]
[RuleDoc("CA1843", "Do not use 'WaitAll' with a single task",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1843",
    Untestable = "In .NET 10 NetAnalyzers build analysis, CA1843 does not fire for Task.WaitAll(singleTask) with either generic Task<T> or non-generic Task arguments; exhaustive probing with multiple patterns confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1843.severity = warning configured")]
[RuleDoc("CA1867", "Use char overload",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1867",
    Untestable = "In .NET 10 NetAnalyzers, CA1866 fires for both string.StartsWith(string) and string.EndsWith(string) single-char patterns without StringComparison (subsuming CA1867), and CA1865 fires for both with StringComparison.Ordinal; CA1867 never fires independently for any tested EndsWith pattern")]
[RuleDoc("CA1802", "Use literals where appropriate",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1802",
    Untestable = "CA1802 does not fire in NetAnalyzers 10.0.x build analysis for public static readonly fields initialized with compile-time constants (string, int, bool) in either static or instance classes; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1802.severity = warning configured")]
[RuleDoc("CA1853", "Unnecessary call to 'Dictionary.ContainsKey(key)'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1853",
    Untestable = "CA1853 does not fire in NetAnalyzers 10.0.x build analysis for ContainsKey followed by TryGetValue or TryAdd patterns; the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1853.severity = warning configured. CA1864 (prefer TryAdd over ContainsKey+Add) fires correctly and is covered separately")]
[RuleDoc("CA1870", "Use a cached 'SearchValues' instance",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1870",
    Untestable = "CA1870 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern where SearchValues.Create is called inline in an IndexOfAny/ContainsAny call; exhaustive probing with multiple patterns confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1870.severity = warning configured")]
[RuleDoc("CA2020", "Prevent behavioral change caused by built-in operators of IntPtr and UIntPtr",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2020",
    Untestable = "CA2020 does not fire in NetAnalyzers 10.0.x build analysis for nint/nuint arithmetic in checked or unchecked expressions; the rule targets behavioral changes introduced between .NET 5 and .NET 7 but does not produce diagnostics for projects already targeting .NET 7+")]
[RuleDoc("CA2153", "Do Not Catch Corrupted State Exceptions",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2153",
    Untestable = "CA2153 does not fire in NetAnalyzers 10.0.x build analysis for catch blocks that catch AccessViolationException or other corrupted-state exceptions; in .NET 6+ the runtime changed CSE behavior and the analyzer does not emit this diagnostic for modern targets")]
[RuleDoc("CA2218", "Override GetHashCode on overriding Equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2218",
    Untestable = "In C#, the compiler issues CS0659 (warning promoted to error by TreatWarningsAsErrors) for any class that overrides Equals without overriding GetHashCode; this compiler diagnostic preempts CA2218, which never appears as a separate analyzer diagnostic")]
[RuleDoc("CA2224", "Override Equals on overloading operator equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2224",
    Untestable = "In C#, the compiler issues CS0660/CS0661 (promoted to errors by TreatWarningsAsErrors) for any class that defines operator== without overriding Equals/GetHashCode; these compiler diagnostics preempt CA2224. Additionally CA1046 fires for reference-type equality operator overloads, and CA2224 never appears as a separate diagnostic")]
[RuleDoc("CA2226", "Operators should have symmetrical overloads",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2226",
    Untestable = "In C#, the compiler enforces paired operators (==,!=; <,>; <=,>=) with CS0216 compile error, making it impossible to define only one of a pair; CA2226 therefore never fires as an analyzer diagnostic in C# projects")]
[RuleDoc("CA2017", "Parameter count mismatch",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2017",
    Untestable = "Fires on ILogger message templates where argument count does not match the number of named placeholders; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2023", "Invalid braces in message template",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2023",
    Untestable = "Fires on ILogger calls with syntactically invalid brace patterns in message templates; Microsoft.Extensions.Logging is not available in the simple single-project build harness")]
[RuleDoc("CA2100", "Review SQL queries for security vulnerabilities",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2100",
    Untestable = "Requires data-flow taint analysis to track untrusted input from parameter to SQL string; build-based harness cannot trigger inter-procedural data-flow rules that require full program analysis")]
[RuleDoc("CA2253", "Named placeholders should not be numeric values",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2253",
    Untestable = "Fires on ILogger structured logging message templates that use numeric placeholder names ({0}) instead of named placeholders ({name}); Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2254", "Template should be a static expression",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2254",
    Untestable = "Fires on ILogger calls where the message template argument is a variable or non-constant expression rather than a string literal; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2258", "Providing a 'DynamicInterfaceCastableImplementation' interface in Visual Basic is unsupported",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2258",
    Untestable = "VB.NET-only rule; not applicable in C# projects")]
[RuleDoc("CA2243", "Attribute string literals should parse correctly",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2243",
    Untestable = "All attribute string types validated by CA2243 (GuidAttribute, AssemblyVersionAttribute, AssemblyFileVersionAttribute) are also validated by the C# compiler, which emits hard errors (CS0591, CS0647, CS7035) and/or duplicate-attribute errors before the analyzer diagnostic can appear in SARIF output; CA2243 therefore cannot be triggered in a project where TreatWarningsAsErrors is active")]
[RuleDoc("CA2216", "Disposable types should declare finalizer",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2216",
    Untestable = "CA2216 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern: IDisposable classes with IntPtr, UIntPtr, or HandleRef fields but no finalizer emit CA1063 (wrong Dispose pattern) instead, and even with a correct Dispose(bool) pattern, CA2216 is never emitted in the SARIF output; the rule appears suppressed for .NET 10 targets")]
public static class UntestableRules { }
