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
[RuleDoc("CA2300", "Do not use insecure deserializer BinaryFormatter",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2300",
    Untestable = "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; with TreatWarningsAsErrors=true the SYSLIB0011 diagnostic becomes an error and the build fails before CA2300 fires as a distinct diagnostic; the rule cannot be tested without suppressing SYSLIB0011 across the test harness")]
[RuleDoc("CA2301", "Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2301",
    Untestable = "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; same SYSLIB0011/TreatWarningsAsErrors constraint as CA2300; cannot be tested without suppressing SYSLIB0011 globally")]
[RuleDoc("CA2302", "Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2302",
    Untestable = "Data-flow/taint analysis variant of CA2301 that also requires tracking Binder assignment across statements; the underlying BinaryFormatter is additionally blocked by SYSLIB0011 as described in CA2300")]
[RuleDoc("CA2305", "Do not use insecure deserializer LosFormatter",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2305",
    Untestable = "LosFormatter is in System.Web (ASP.NET classic); the type is not available in .NET Core/5+ and cannot be referenced in the simple build harness")]
[RuleDoc("CA2310", "Do not use insecure deserializer NetDataContractSerializer",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2310",
    Untestable = "NetDataContractSerializer was removed from .NET Core; the type does not exist in .NET 5+ BCL and cannot be referenced in the test harness")]
[RuleDoc("CA2311", "Do not deserialize without first setting NetDataContractSerializer.Binder",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2311",
    Untestable = "NetDataContractSerializer was removed from .NET Core; the type does not exist in .NET 5+ BCL (same unavailability as CA2310)")]
[RuleDoc("CA2312", "Ensure NetDataContractSerializer.Binder is set before deserializing",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2312",
    Untestable = "Data-flow/taint analysis variant of CA2311; additionally requires taint analysis across statements; same NetDataContractSerializer unavailability as CA2310")]
[RuleDoc("CA2315", "Do not use insecure deserializer ObjectStateFormatter",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2315",
    Untestable = "ObjectStateFormatter is in System.Web.UI (ASP.NET classic WebForms); the type is not available in .NET Core/5+ and cannot be referenced in the test harness")]
[RuleDoc("CA2321", "Do not deserialize with JavaScriptSerializer using a SimpleTypeResolver",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2321",
    Untestable = "JavaScriptSerializer is in System.Web.Script.Serialization (.NET Framework only); the type is not available in .NET Core/5+ and cannot be referenced in the test harness")]
[RuleDoc("CA2322", "Ensure JavaScriptSerializer is not initialized with SimpleTypeResolver before deserializing",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2322",
    Untestable = "Data-flow/taint analysis variant of CA2321; additionally requires taint analysis tracking of the resolver; same JavaScriptSerializer unavailability as CA2321")]
[RuleDoc("CA2326", "Do not use TypeNameHandling values other than None",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2326",
    Untestable = "Fires on Newtonsoft.Json TypeNameHandling != None patterns; Newtonsoft.Json is not included in the simple single-project build harness and adding it as a transitive package reference would introduce a network dependency in test infrastructure")]
[RuleDoc("CA2327", "Do not use insecure JsonSerializerSettings",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2327",
    Untestable = "Fires when Newtonsoft.Json JsonSerializerSettings with TypeNameHandling != None is passed to JsonConvert.DeserializeObject; same Newtonsoft.Json dependency as CA2326")]
[RuleDoc("CA2328", "Ensure that JsonSerializerSettings are secure",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2328",
    Untestable = "Data-flow/taint analysis variant of CA2327 that tracks insecure settings through variables; same Newtonsoft.Json dependency as CA2326")]
[RuleDoc("CA2329", "Do not deserialize with JsonSerializer using an insecure configuration",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2329",
    Untestable = "Data-flow/taint analysis variant that fires on Newtonsoft.Json JsonSerializer created with insecure settings; same Newtonsoft.Json dependency as CA2326")]
[RuleDoc("CA2330", "Ensure that JsonSerializer has a secure configuration when deserializing",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2330",
    Untestable = "Data-flow/taint analysis variant of CA2329 that tracks insecure serializer through variables; same Newtonsoft.Json dependency as CA2326")]
[RuleDoc("CA2350", "Do not use DataTable.ReadXml() with untrusted data",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2350",
    Untestable = "Data-flow/taint analysis rule: fires only when untrusted input (method parameter, user-controlled source) reaches DataTable.ReadXml(); the build harness cannot trigger inter-procedural taint analysis without a full program analysis configuration")]
[RuleDoc("CA2351", "Do not use DataSet.ReadXml() with untrusted data",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2351",
    Untestable = "Data-flow/taint analysis rule: fires only when untrusted input reaches DataSet.ReadXml(); same taint-analysis constraint as CA2350")]
[RuleDoc("CA2354", "Unsafe DataSet or DataTable in deserialized object graph can be vulnerable to remote code execution attacks",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2354",
    Untestable = "Data-flow analysis rule: fires when a DataSet/DataTable type appears in the deserialization graph of a call to a generic deserialization API; requires inter-procedural type-graph analysis not triggerable from a single-project build harness")]
[RuleDoc("CA2355", "Unsafe DataSet or DataTable type found in deserializable object graph",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2355",
    Untestable = "Data-flow analysis rule: fires when DataSet/DataTable appears in a potentially-deserialized type hierarchy; same inter-procedural type-graph constraint as CA2354")]
[RuleDoc("CA2356", "Unsafe DataSet or DataTable type in web deserializable object graph",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2356",
    Untestable = "Fires on ASP.NET Web API / WCF action methods whose return type or parameters include a DataSet or DataTable in an unsafe deserialization context; requires System.Web or ASP.NET Core web framework not available in the simple build harness")]
[RuleDoc("CA2361", "Ensure auto-generated class containing DataSet.ReadXml() is not used with untrusted data",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2361",
    Untestable = "Fires on auto-generated typed DataSet classes (produced by the DataSet Designer or xsd.exe) that call ReadXml internally; not replicable from hand-written code in the test harness")]
[RuleDoc("CA2362", "Unsafe DataSet or DataTable in auto-generated serializable type can be vulnerable to remote code execution attacks",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2362",
    Untestable = "Fires on auto-generated typed DataSet/DataTable serializable classes produced by DataSet Designer or xsd.exe; same auto-generated-code constraint as CA2361")]
[RuleDoc("CA3061", "Do Not Add Schema By URL",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3061",
    Untestable = "Fires on XmlSchemaCollection.Add(string, string) where the second argument is a URL; XmlSchemaCollection is a .NET Framework 1.x type that was replaced by XmlSchemaSet in .NET 2.0 and is not available in .NET Core/5+")]
[RuleDoc("CA3147", "Mark Verb Handlers With Validate Antiforgery Token",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3147",
    Untestable = "Requires ASP.NET MVC (System.Web.Mvc) controller action methods decorated with HTTP verb attributes; System.Web.Mvc is not available in .NET Core and ASP.NET Core MVC is not included in the simple build harness")]
[RuleDoc("CA5363", "Do Not Disable Request Validation",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5363",
    Untestable = "Fires on ASP.NET [ValidateInput(false)] attribute on MVC action methods; System.Web.Mvc is not available in .NET Core/5+")]
[RuleDoc("CA5365", "Do Not Disable HTTP Header Checking",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5365",
    Untestable = "Fires when HttpRuntimeSection.EnableHeaderChecking is set to false; requires System.Web.Configuration which is not available in .NET Core/5+")]
[RuleDoc("CA5367", "Do Not Serialize Types With Pointer Fields",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5367",
    Untestable = "CA5367 does not fire in NetAnalyzers 10.0.x build analysis for [Serializable] types with unsafe pointer fields; exhaustive probing with AllowUnsafeBlocks=true and a [Serializable] class containing int* fields confirmed the diagnostic is absent from SARIF output")]
[RuleDoc("CA5368", "Set ViewStateUserKey For Classes Derived From Page",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5368",
    Untestable = "Fires when a class derived from System.Web.UI.Page does not set ViewStateUserKey in Page_Init; System.Web.UI is not available in .NET Core/5+")]
[RuleDoc("CA5370", "Use XmlReader for XmlValidatingReader constructor",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5370",
    Untestable = "XmlValidatingReader is a .NET Framework 1.x type not available in .NET Core/5+; the type was removed (not just deprecated) from the cross-platform BCL")]
[RuleDoc("CA5374", "Do Not Use XslTransform",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5374",
    Untestable = "XslTransform is a .NET Framework 1.x type not available in .NET Core/5+; the type was removed (not just deprecated) from the cross-platform BCL")]
[RuleDoc("CA5375", "Do Not Use Account Shared Access Signature",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5375",
    Untestable = "Fires on Azure Storage SDK (WindowsAzure.Storage / Azure.Storage.Blobs) CloudStorageAccount.GetSharedAccessSignature calls; the Azure Storage SDK is not included in the simple build harness")]
[RuleDoc("CA5376", "Use SharedAccessProtocol HttpsOnly",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5376",
    Untestable = "Fires when Azure Storage SDK SharedAccessPolicy uses HTTP instead of HTTPS-only; the Azure Storage SDK is not included in the simple build harness")]
[RuleDoc("CA5377", "Use Container Level Access Policy",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5377",
    Untestable = "Fires when Azure Blob Storage container SAS tokens use an ad-hoc policy without a stored access policy; the Azure Storage SDK is not included in the simple build harness")]
[RuleDoc("CA5381", "Ensure Certificates Are Not Added To Root Certificate Store",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5381",
    Untestable = "Data-flow/taint analysis variant of CA5380: fires when a certificate is added to a store whose StoreName comes through a variable rather than a constant; the build harness cannot trigger inter-procedural taint analysis tracing the store name through assignments")]
[RuleDoc("CA5382", "Use Secure Cookies In ASP.NET Core",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5382",
    Untestable = "Fires when IResponseCookies.Append is called without setting Secure = true in the CookieOptions; requires Microsoft.AspNetCore.Http which is not included in the simple build harness")]
[RuleDoc("CA5383", "Ensure Use Secure Cookies In ASP.NET Core",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5383",
    Untestable = "Data-flow/taint analysis variant of CA5382 that tracks CookieOptions through variables; same ASP.NET Core dependency and taint-analysis constraint as CA5382")]
[RuleDoc("CA5388", "Ensure Sufficient Iteration Count When Using Weak Key Derivation Function",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5388",
    Untestable = "Data-flow/taint analysis variant of CA5387: fires when the iteration count passed to Rfc2898DeriveBytes comes from a variable rather than a literal and cannot be proven to exceed the threshold; requires inter-procedural taint analysis not triggerable from a single-project build harness")]
[RuleDoc("CA5391", "Use antiforgery tokens in ASP.NET Core MVC controllers",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5391",
    Untestable = "Fires when ASP.NET Core MVC controller action methods lack antiforgery token validation attributes; requires Microsoft.AspNetCore.Mvc which is not included in the simple build harness")]
[RuleDoc("CA5395", "Miss HttpVerb attribute for action methods",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5395",
    Untestable = "Fires when ASP.NET MVC controller action methods lack an HTTP verb attribute ([HttpGet], [HttpPost], etc.); requires System.Web.Mvc or Microsoft.AspNetCore.Mvc which are not included in the simple build harness")]
[RuleDoc("CA5396", "Set HttpOnly to true for HttpCookie",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5396",
    Untestable = "Fires when System.Web.HttpCookie.HttpOnly is set to false or not set; System.Web is not available in .NET Core/5+")]
[RuleDoc("CA3075", "Insecure DTD processing in XML",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3075",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlReaderSettings { DtdProcessing = DtdProcessing.Parse } + XmlReader.Create patterns; the XmlTextReader approach requires System.Xml.XmlTextReader which is not in .NET 10")]
[RuleDoc("CA3077", "Insecure Processing in API Design, XmlDocument and XmlTextReader",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3077",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for XmlDocument with XmlResolver + LoadXml patterns; appears to require specific API design patterns (method accepting XmlDocument parameter) not replicable in a simple harness")]
[RuleDoc("CA5360", "Do Not Call Dangerous Methods In Deserialization",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5360",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the standard ISerializable constructor + Process.Start pattern documented in the rule's official examples; likely requires inter-procedural data-flow analysis not available in the single-project build harness")]
[RuleDoc("CA5373", "Do not use obsolete key derivation function",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5373",
    Untestable = "PasswordDeriveBytes is marked [Obsolete(SYSLIB0041)] in .NET 9+; SYSLIB0041 fires as a build error but CA5373 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
[RuleDoc("CA5384", "Do Not Use Digital Signature Algorithm (DSA)",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5384",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for DSA.Create() nor for DSA.SignData() calls; the abstract factory pattern for DSA appears to not trigger the diagnostic in this analyzer version")]
[RuleDoc("CA5385", "Use Rivest-Shamir-Adleman (RSA) Algorithm With Sufficient Key Size",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5385",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for RSA.Create(int) nor for RSA.Create() + KeySize assignment patterns; the abstract factory and property setter approaches do not trigger the key-size diagnostic")]
[RuleDoc("CA5387", "Do Not Use Weak Key Derivation Function With Insufficient Iteration Count",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5387",
    Untestable = "All Rfc2898DeriveBytes constructors are marked [Obsolete(SYSLIB0060)] in .NET 10; SYSLIB0060 fires but CA5387 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
[RuleDoc("CA5402", "Use CreateEncryptor with the default IV",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5402",
    Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the parameterless Aes.CreateEncryptor() overload; unlike the sibling rule CA5401 (which fires for the 2-argument overload), CA5402 produces no diagnostic even when the encryptor is actively used")]
[RuleDoc("CA5404", "Do not disable token validation checks",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5404",
    Untestable = "Fires when Microsoft.IdentityModel.Tokens.TokenValidationParameters has validation checks disabled (ValidateAudience=false, ValidateIssuer=false, etc.); Microsoft.IdentityModel.Tokens is not included in the simple build harness")]
[RuleDoc("CA5405", "Do not always skip token validation in delegates",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5405",
    Untestable = "Fires when Microsoft.IdentityModel.Tokens validation delegates (AudienceValidator, IssuerValidator, etc.) always return true without checking; requires Microsoft.IdentityModel.Tokens package not included in the simple build harness")]
public static class UntestableRules { }
