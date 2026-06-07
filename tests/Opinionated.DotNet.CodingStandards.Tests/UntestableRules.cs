namespace Opinionated.DotNet.CodingStandards.Tests;

[RuleDoc("IDE0001", "Simplify name",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0001",
    Untestable = "IDE0001 (SimplifyNames) is registered with EnforceOnBuild.Never in Roslyn's EnforceOnBuildValues.cs (`public const EnforceOnBuild SimplifyNames = /*IDE0001*/ EnforceOnBuild.Never;`, preceded by the comment \"TODO: Allow enforcing simplify names and related diagnostics on build once we validate their performance charactericstics.\"). The EnforceOnBuild enum documents Never as \"an IDE-only diagnostic that cannot be enforced on build\", so the descriptor is excluded from the command-line/build analyzer set and IDE0001 never appears in MSBuild SARIF output - even though this package sets dotnet_diagnostic.IDE0001.severity = warning, EnforceCodeStyleInBuild=true, and TreatWarningsAsErrors=true. Confirmed by dotnet/roslyn issue #77120 (which names IDE0001 and IDE0049 as affected) and by the same EnforceOnBuild.Never gating IDE0002/IDE0003. Source: src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs and EnforceOnBuild.cs (dotnet/roslyn).")]
[RuleDoc("IDE0002", "Simplify member access",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0002",
    Untestable = "IDE0002's analyzer (SimplifyTypeNamesDiagnosticAnalyzerBase, Roslyn) builds its descriptor as CreateDescriptorWithId(IDEDiagnosticIds.SimplifyMemberAccessDiagnosticId, EnforceOnBuildValues.SimplifyMemberAccess, ..., isUnnecessary: true), and EnforceOnBuildValues.SimplifyMemberAccess == EnforceOnBuild.Never (Source: src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs). EnforceOnBuild.Never is documented as \"an IDE-only diagnostic that cannot be enforced on build\" (Source: src/Analyzers/Core/Analyzers/EnforceOnBuild.cs) and stamps the \"EnforceOnBuild_Never\" custom tag onto the descriptor (Source: src/Analyzers/Core/Analyzers/DiagnosticCustomTags.cs). The command-line/MSBuild analyzer host honours that tag and excludes IDE0002 from build SARIF regardless of dotnet_diagnostic.IDE0002.severity or EnforceCodeStyleInBuild=true, so no code pattern can make HasError(\"IDE0002\") true in this build-output harness. Empirically confirmed: a genuine `return Program.Value();` violation reported by `dotnet format style --diagnostics IDE0002` produced NO IDE0002 in the dotnet build SARIF even at severity=error + EnforceCodeStyleInBuild=true, while a control IDE0004 (EnforceOnBuild.WhenExplicitlyEnabled) DID surface in the same build. Note IDE0002 is isConfigurable:true (severity-configurable), so the gate is the EnforceOnBuild_Never tag, not NotConfigurable. Source: src/Analyzers/Core/Analyzers/SimplifyTypeNames/SimplifyTypeNamesDiagnosticAnalyzerBase.cs.")]
[RuleDoc("IDE0003", "Remove this or Me qualification",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0003",
    Untestable = "IDE0003's analyzer (AbstractSimplifyThisOrMeDiagnosticAnalyzer, src/Features/Core/Portable/SimplifyThisOrMe/AbstractSimplifyThisOrMeDiagnosticAnalyzer.cs) is constructed with EnforceOnBuildValues.RemoveQualification, which is hardcoded to EnforceOnBuild.Never (`public const EnforceOnBuild RemoveQualification = /*IDE0003*/ EnforceOnBuild.Never;` in src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs). The EnforceOnBuild.Never member is documented as \"an IDE-only diagnostic that cannot be enforced on build\" (src/Analyzers/Core/Analyzers/EnforceOnBuild.cs), and DiagnosticCustomTags.Create stamps such descriptors with WellKnownDiagnosticTags.NotConfigurable (src/Analyzers/Core/Analyzers/DiagnosticCustomTags.cs: `Debug.Assert(isConfigurable || enforceOnBuild == EnforceOnBuild.Never)`). Consequently IDE0003 never runs during command-line build and cannot be turned on via editorconfig severity or EnforceCodeStyleInBuild=true. Empirically confirmed: a probe of `this._value` field qualification with EnforceCodeStyleInBuild=true and NoWarn=IDE0055 produced a clean build (0 warnings/0 errors) and an EMPTY SARIF - no IDE0003 at note/warning/error and no IDE0055 substitute (so it is NOT merely formatter-backed). Tracked upstream as dotnet/roslyn#77120.")]
[RuleDoc("IDE0038", "Use pattern matching to avoid is check followed by a cast",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0038",
    Untestable = "IDE0038's analyzer, CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer, ships only in the IDE-only Features assembly (Microsoft.CodeAnalysis.CSharp.Features) at src/Features/CSharp/Portable/UsePatternMatching/CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer.cs, NOT in the build-time Microsoft.CodeAnalysis.CSharp.CodeStyle package; that assembly is not loaded during 'dotnet build', so IDE0038 is never reported. Only the sibling IDE0020 analyzer (CSharpIsAndCastCheckDiagnosticAnalyzer, src/Analyzers/CSharp/Analyzers/UsePatternMatching/) runs at build, and it reports ONLY IDEDiagnosticIds.InlineIsTypeCheckId (IDE0020) because its TryGetPatternPieces requires a LocalDeclarationStatementSyntax (the 'var v = (Type)x;' saved-to-local shape). Verified empirically: building the canonical without-variable violation 'if (obj is int) { if ((int)obj == 1) { ... } }' in the harness with dotnet_diagnostic.IDE0038.severity=error forced via .editorconfig produced ZERO diagnostics. The EnforceOnBuildValues.InlineIsTypeWithoutName=Recommended value is moot because the reporting analyzer is absent at build. Source: src/Features/CSharp/Portable/UsePatternMatching/CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer.cs (dotnet/roslyn).")]
[RuleDoc("IDE0084", "Use pattern matching (IsNot operator)",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0084",
    Untestable = "IDE0084's analyzer (VisualBasicUseIsNotExpressionDiagnosticAnalyzer in src/Analyzers/VisualBasic/Analyzers/UseIsNotExpression/VisualBasicUseIsNotDiagnosticAnalyzer.vb) is declared `<DiagnosticAnalyzer(LanguageNames.VisualBasic)>` and registers context.RegisterSyntaxNodeAction(AddressOf SyntaxNodeAction, SyntaxKind.NotExpression) on the VB-only `Not x Is y` expression, reporting IDEDiagnosticIds.UseIsNotExpressionDiagnosticId (\"IDE0084\"). Because the attribute scopes the analyzer to VisualBasic only, Roslyn never instantiates it for a C# compilation, so it cannot run against the harness's net10.0 C# project. A gh api code search of dotnet/roslyn for UseIsNotExpressionDiagnosticId returns only the Core ID constant (src/Analyzers/Core/Analyzers/IDEDiagnosticIds.cs) and VisualBasic-tree files - there is no C# analyzer that emits IDE0084, and the C# `is not` operator is an unrelated compiler-native construct handled by CSharpUseNotPatternDiagnosticAnalyzer which reports the DISTINCT id IDE0083 (UseNotPatternDiagnosticId), not IDE0084. No PackageReference, NoWarn, LangVersion pin, or target-framework switch can convert the single C#-only project into a VB compilation, so the rule can never fire in this harness. Source: src/Analyzers/VisualBasic/Analyzers/UseIsNotExpression/VisualBasicUseIsNotDiagnosticAnalyzer.vb.")]
[RuleDoc("CA1047", "Do not declare protected member in sealed type",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1047",
    Untestable = "CA1047's analyzer (DoNotDeclareProtectedMembersInSealedTypes, Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines) is attributed [DiagnosticAnalyzer(LanguageNames.VisualBasic)] only (the class comment states \"This rule is not implemented for C# as the compiler warning CS0628 already covers this part\"). Roslyn's build host loads packaged analyzers through AnalyzerFileReference, whose GetExtensions(language)/GetAnalyzerTypeNameMap path returns no analyzer types for a language not declared in the [DiagnosticAnalyzer] attribute, so the analyzer is never instantiated for the harness's C# (net10.0) compilation and CA1047 cannot appear in SARIF. (CS0628 is only a level-4 compiler warning, not an error, so it is not what preempts the rule - the VB-only language registration is the structural gate.) Verified empirically: a sealed class declaring a protected field, protected method, and protected property (CS0628 suppressed via NoWarn) built against the packed package surfaced CA1051/SA1649/IDE0055 but had ZERO CA1047 occurrences in SARIF, and a metadata read of the shipped Microsoft.CodeAnalysis.NetAnalyzers.dll v10.0.102 shows the DiagnosticAnalyzer attribute on this type carries only the language string \"Visual Basic\". The dotnet/sdk unit test's C# case passes only because Microsoft.CodeAnalysis.Testing injects the analyzer instance directly into CompilationWithAnalyzers, bypassing the language filter; the package-based build harness cannot do this and has no VB-project path. Source: src/Microsoft.CodeAnalysis.NetAnalyzers/.../ApiDesignGuidelines/DoNotDeclareProtectedMembersInSealedTypes.cs (dotnet/roslyn-analyzers) and AnalyzerFileReference.cs (dotnet/roslyn).")]
[RuleDoc("CA1873", "Avoid potentially expensive logging",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1873",
    Untestable = "Fires on string concatenation/interpolation in ILogger.LogXxx calls; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2017", "Parameter count mismatch",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2017",
    Untestable = "Fires on ILogger message templates where argument count does not match the number of named placeholders; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2023", "Invalid braces in message template",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2023",
    Untestable = "Fires on ILogger calls with syntactically invalid brace patterns in message templates; Microsoft.Extensions.Logging is not available in the simple single-project build harness")]
[RuleDoc("CA2218", "Override GetHashCode on overriding Equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2218",
    Untestable = "In C#, the compiler issues CS0659 (warning promoted to error by TreatWarningsAsErrors) for any class that overrides Equals without overriding GetHashCode; this compiler diagnostic preempts CA2218, which never appears as a separate analyzer diagnostic")]
[RuleDoc("CA2224", "Override Equals on overloading operator equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2224",
    Untestable = "In C#, the compiler issues CS0660/CS0661 (promoted to errors by TreatWarningsAsErrors) for any class that defines operator== without overriding Equals/GetHashCode; these compiler diagnostics preempt CA2224. Additionally CA1046 fires for reference-type equality operator overloads, and CA2224 never appears as a separate diagnostic")]
[RuleDoc("CA2226", "Operators should have symmetrical overloads",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2226",
    Untestable = "In C#, the compiler enforces paired operators (==,!=; <,>; <=,>=) with CS0216 compile error, making it impossible to define only one of a pair; CA2226 therefore never fires as an analyzer diagnostic in C# projects")]
[RuleDoc("CA2243", "Attribute string literals should parse correctly",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2243",
    Untestable = "All attribute string types validated by CA2243 (GuidAttribute, AssemblyVersionAttribute, AssemblyFileVersionAttribute) are also validated by the C# compiler, which emits hard errors (CS0591, CS0647, CS7035) and/or duplicate-attribute errors before the analyzer diagnostic can appear in SARIF output; CA2243 therefore cannot be triggered in a project where TreatWarningsAsErrors is active")]
[RuleDoc("CA2253", "Named placeholders should not be numeric values",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2253",
    Untestable = "Fires on ILogger structured logging message templates that use numeric placeholder names ({0}) instead of named placeholders ({name}); Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2254", "Template should be a static expression",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2254",
    Untestable = "Fires on ILogger calls where the message template argument is a variable or non-constant expression rather than a string literal; Microsoft.Extensions.Logging is not available in the simple single-project build harness (same constraint as CA1727)")]
[RuleDoc("CA2258", "Providing a 'DynamicInterfaceCastableImplementation' interface in Visual Basic is unsupported",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2258",
    Untestable = "VB.NET-only rule; not applicable in C# projects")]
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
[RuleDoc("CA5382", "Use Secure Cookies In ASP.NET Core",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5382",
    Untestable = "Fires when IResponseCookies.Append is called without setting Secure = true in the CookieOptions; requires Microsoft.AspNetCore.Http which is not included in the simple build harness")]
[RuleDoc("CA5383", "Ensure Use Secure Cookies In ASP.NET Core",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5383",
    Untestable = "Data-flow/taint analysis variant of CA5382 that tracks CookieOptions through variables; same ASP.NET Core dependency and taint-analysis constraint as CA5382")]
[RuleDoc("CA5391", "Use antiforgery tokens in ASP.NET Core MVC controllers",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5391",
    Untestable = "Fires when ASP.NET Core MVC controller action methods lack antiforgery token validation attributes; requires Microsoft.AspNetCore.Mvc which is not included in the simple build harness")]
[RuleDoc("CA5395", "Miss HttpVerb attribute for action methods",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5395",
    Untestable = "Fires when ASP.NET MVC controller action methods lack an HTTP verb attribute ([HttpGet], [HttpPost], etc.); requires System.Web.Mvc or Microsoft.AspNetCore.Mvc which are not included in the simple build harness")]
[RuleDoc("CA5396", "Set HttpOnly to true for HttpCookie",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5396",
    Untestable = "Fires when System.Web.HttpCookie.HttpOnly is set to false or not set; System.Web is not available in .NET Core/5+")]
[RuleDoc("CA5404", "Do not disable token validation checks",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5404",
    Untestable = "Fires when Microsoft.IdentityModel.Tokens.TokenValidationParameters has validation checks disabled (ValidateAudience=false, ValidateIssuer=false, etc.); Microsoft.IdentityModel.Tokens is not included in the simple build harness")]
[RuleDoc("CA5405", "Do not always skip token validation in delegates",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5405",
    Untestable = "Fires when Microsoft.IdentityModel.Tokens validation delegates (AudienceValidator, IssuerValidator, etc.) always return true without checking; requires Microsoft.IdentityModel.Tokens package not included in the simple build harness")]
[RuleDoc("EnableGenerateDocumentationFile", "Set MSBuild property 'GenerateDocumentationFile' to 'true'",
    HelpLink = "https://github.com/dotnet/roslyn/issues/41640",
    Untestable = "Project-configuration recommendation, not a code-pattern violation; fires based on the absence of the GenerateDocumentationFile MSBuild property and cannot be triggered or suppressed by writing code in the test harness")]
public static class UntestableRules { }
