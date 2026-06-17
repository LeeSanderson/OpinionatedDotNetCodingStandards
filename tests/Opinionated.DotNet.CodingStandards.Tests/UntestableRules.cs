namespace Opinionated.DotNet.CodingStandards.Tests;

[RuleDoc("IDE0001", "Simplify name",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0001",
    Untestable = """
        IDE0001 (SimplifyNames) is registered with EnforceOnBuild.Never in Roslyn's EnforceOnBuildValues.cs
        (`public const EnforceOnBuild SimplifyNames = /*IDE0001*/ EnforceOnBuild.Never;`, preceded by the
        comment "TODO: Allow enforcing simplify names and related diagnostics on build once we validate their
        performance charactericstics."). The EnforceOnBuild enum documents Never as "an IDE-only diagnostic
        that cannot be enforced on build", so the descriptor is excluded from the command-line/build analyzer
        set and IDE0001 never appears in MSBuild SARIF output - even though this package sets
        dotnet_diagnostic.IDE0001.severity = warning, EnforceCodeStyleInBuild=true, and
        TreatWarningsAsErrors=true. Confirmed by dotnet/roslyn issue #77120 (which names IDE0001 and IDE0049
        as affected) and by the same EnforceOnBuild.Never gating IDE0002/IDE0003. Source:
        src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs and EnforceOnBuild.cs (dotnet/roslyn).
        """)]
[RuleDoc("IDE0002", "Simplify member access",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0002",
    Untestable = """
        IDE0002's analyzer (SimplifyTypeNamesDiagnosticAnalyzerBase, Roslyn) builds its descriptor as
        CreateDescriptorWithId(IDEDiagnosticIds.SimplifyMemberAccessDiagnosticId,
        EnforceOnBuildValues.SimplifyMemberAccess, ..., isUnnecessary: true), and
        EnforceOnBuildValues.SimplifyMemberAccess == EnforceOnBuild.Never (Source:
        src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs). EnforceOnBuild.Never is documented as "an
        IDE-only diagnostic that cannot be enforced on build" (Source:
        src/Analyzers/Core/Analyzers/EnforceOnBuild.cs) and stamps the "EnforceOnBuild_Never" custom tag onto
        the descriptor (Source: src/Analyzers/Core/Analyzers/DiagnosticCustomTags.cs). The command-line/MSBuild
        analyzer host honours that tag and excludes IDE0002 from build SARIF regardless of
        dotnet_diagnostic.IDE0002.severity or EnforceCodeStyleInBuild=true, so no code pattern can make
        HasError("IDE0002") true in this build-output harness. Empirically confirmed: a genuine `return
        Program.Value();` violation reported by `dotnet format style --diagnostics IDE0002` produced NO IDE0002
        in the dotnet build SARIF even at severity=error + EnforceCodeStyleInBuild=true, while a control IDE0004
        (EnforceOnBuild.WhenExplicitlyEnabled) DID surface in the same build. Note IDE0002 is
        isConfigurable:true (severity-configurable), so the gate is the EnforceOnBuild_Never tag, not
        NotConfigurable. Source:
        src/Analyzers/Core/Analyzers/SimplifyTypeNames/SimplifyTypeNamesDiagnosticAnalyzerBase.cs.
        """)]
[RuleDoc("IDE0003", "Remove this or Me qualification",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0003",
    Untestable = """
        IDE0003's analyzer (AbstractSimplifyThisOrMeDiagnosticAnalyzer,
        src/Features/Core/Portable/SimplifyThisOrMe/AbstractSimplifyThisOrMeDiagnosticAnalyzer.cs) is
        constructed with EnforceOnBuildValues.RemoveQualification, which is hardcoded to EnforceOnBuild.Never
        (`public const EnforceOnBuild RemoveQualification = /*IDE0003*/ EnforceOnBuild.Never;` in
        src/Analyzers/Core/Analyzers/EnforceOnBuildValues.cs). The EnforceOnBuild.Never member is documented as
        "an IDE-only diagnostic that cannot be enforced on build" (src/Analyzers/Core/Analyzers/EnforceOnBuild.cs),
        and DiagnosticCustomTags.Create stamps such descriptors with WellKnownDiagnosticTags.NotConfigurable
        (src/Analyzers/Core/Analyzers/DiagnosticCustomTags.cs: `Debug.Assert(isConfigurable || enforceOnBuild ==
        EnforceOnBuild.Never)`). Consequently IDE0003 never runs during command-line build and cannot be turned
        on via editorconfig severity or EnforceCodeStyleInBuild=true. Empirically confirmed: a probe of
        `this._value` field qualification with EnforceCodeStyleInBuild=true and NoWarn=IDE0055 produced a clean
        build (0 warnings/0 errors) and an EMPTY SARIF - no IDE0003 at note/warning/error and no IDE0055
        substitute (so it is NOT merely formatter-backed). Tracked upstream as dotnet/roslyn#77120.
        """)]
[RuleDoc("IDE0038", "Use pattern matching to avoid is check followed by a cast",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0038",
    Untestable = """
        IDE0038's analyzer, CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer, ships only in the IDE-only
        Features assembly (Microsoft.CodeAnalysis.CSharp.Features) at
        src/Features/CSharp/Portable/UsePatternMatching/CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer.cs,
        NOT in the build-time Microsoft.CodeAnalysis.CSharp.CodeStyle package; that assembly is not loaded
        during 'dotnet build', so IDE0038 is never reported. Only the sibling IDE0020 analyzer
        (CSharpIsAndCastCheckDiagnosticAnalyzer, src/Analyzers/CSharp/Analyzers/UsePatternMatching/) runs at
        build, and it reports ONLY IDEDiagnosticIds.InlineIsTypeCheckId (IDE0020) because its
        TryGetPatternPieces requires a LocalDeclarationStatementSyntax (the 'var v = (Type)x;' saved-to-local
        shape). Verified empirically: building the canonical without-variable violation 'if (obj is int) { if
        ((int)obj == 1) { ... } }' in the harness with dotnet_diagnostic.IDE0038.severity=error forced via
        .editorconfig produced ZERO diagnostics. The EnforceOnBuildValues.InlineIsTypeWithoutName=Recommended
        value is moot because the reporting analyzer is absent at build. Source:
        src/Features/CSharp/Portable/UsePatternMatching/CSharpIsAndCastCheckWithoutNameDiagnosticAnalyzer.cs
        (dotnet/roslyn).
        """)]
[RuleDoc("IDE0084", "Use pattern matching (IsNot operator)",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0084",
    Untestable = """
        IDE0084's analyzer (VisualBasicUseIsNotExpressionDiagnosticAnalyzer in
        src/Analyzers/VisualBasic/Analyzers/UseIsNotExpression/VisualBasicUseIsNotDiagnosticAnalyzer.vb) is
        declared `<DiagnosticAnalyzer(LanguageNames.VisualBasic)>` and registers
        context.RegisterSyntaxNodeAction(AddressOf SyntaxNodeAction, SyntaxKind.NotExpression) on the VB-only
        `Not x Is y` expression, reporting IDEDiagnosticIds.UseIsNotExpressionDiagnosticId ("IDE0084"). Because
        the attribute scopes the analyzer to VisualBasic only, Roslyn never instantiates it for a C#
        compilation, so it cannot run against the harness's net10.0 C# project. A gh api code search of
        dotnet/roslyn for UseIsNotExpressionDiagnosticId returns only the Core ID constant
        (src/Analyzers/Core/Analyzers/IDEDiagnosticIds.cs) and VisualBasic-tree files - there is no C# analyzer
        that emits IDE0084, and the C# `is not` operator is an unrelated compiler-native construct handled by
        CSharpUseNotPatternDiagnosticAnalyzer which reports the DISTINCT id IDE0083 (UseNotPatternDiagnosticId),
        not IDE0084. No PackageReference, NoWarn, LangVersion pin, or target-framework switch can convert the
        single C#-only project into a VB compilation, so the rule can never fire in this harness. Source:
        src/Analyzers/VisualBasic/Analyzers/UseIsNotExpression/VisualBasicUseIsNotDiagnosticAnalyzer.vb.
        """)]
[RuleDoc("CA1047", "Do not declare protected member in sealed type",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1047",
    Untestable = """
        CA1047's analyzer (DoNotDeclareProtectedMembersInSealedTypes,
        Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines) is attributed
        [DiagnosticAnalyzer(LanguageNames.VisualBasic)] only (the class comment states "This rule is not
        implemented for C# as the compiler warning CS0628 already covers this part"). Roslyn's build host loads
        packaged analyzers through AnalyzerFileReference, whose GetExtensions(language)/GetAnalyzerTypeNameMap
        path returns no analyzer types for a language not declared in the [DiagnosticAnalyzer] attribute, so the
        analyzer is never instantiated for the harness's C# (net10.0) compilation and CA1047 cannot appear in
        SARIF. (CS0628 is only a level-4 compiler warning, not an error, so it is not what preempts the rule -
        the VB-only language registration is the structural gate.) Verified empirically: a sealed class
        declaring a protected field, protected method, and protected property (CS0628 suppressed via NoWarn)
        built against the packed package surfaced CA1051/SA1649/IDE0055 but had ZERO CA1047 occurrences in
        SARIF, and a metadata read of the shipped Microsoft.CodeAnalysis.NetAnalyzers.dll v10.0.102 shows the
        DiagnosticAnalyzer attribute on this type carries only the language string "Visual Basic". The
        dotnet/sdk unit test's C# case passes only because Microsoft.CodeAnalysis.Testing injects the analyzer
        instance directly into CompilationWithAnalyzers, bypassing the language filter; the package-based build
        harness cannot do this and has no VB-project path. Source:
        src/Microsoft.CodeAnalysis.NetAnalyzers/.../ApiDesignGuidelines/DoNotDeclareProtectedMembersInSealedTypes.cs
        (dotnet/roslyn-analyzers) and AnalyzerFileReference.cs (dotnet/roslyn).
        """)]
[RuleDoc("CA2218", "Override GetHashCode on overriding Equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2218",
    Untestable = """
        CA2218's only analyzer is BasicOverrideGetHashCodeOnOverridingEqualsAnalyzer, decorated
        `<DiagnosticAnalyzer(LanguageNames.VisualBasic)>` and therefore registered ONLY for Visual Basic
        compilations; its RegisterSymbolAction (SymbolKind.NamedType, reporting when type.OverridesEquals()
        AndAlso Not type.OverridesGetHashCode()) never runs on the harness's net10.0 C# project. There is no
        Core or C# analyzer for CA2218 - the only C# artifact is
        CSharpOverrideGetHashCodeOnOverridingEquals.Fixer.cs, a code-fix provider that emits no build/SARIF
        diagnostic. The analyzer's own source remarks state "CA2218 is not applied to C# since it already
        reports CS0569." (Note: the prior repo reason blaming CS0659 preemption is inaccurate - CS0659 is only a
        warning and suppressing it does not unblock CA2218; the structural gate is the VB-only analyzer
        registration.) CA2218 is thus a VB.NET-only rule that structurally cannot fire in a C# build harness.
        Source:
        src/NetAnalyzers/VisualBasic/Microsoft.CodeQuality.Analyzers/ApiDesignGuidelines/BasicOverrideGetHashCodeOnOverridingEquals.vb
        (dotnet/roslyn-analyzers).
        """)]
[RuleDoc("CA2224", "Override Equals on overloading operator equals",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2224",
    Untestable = """
        CA2224's only analyzer is BasicOverrideEqualsOnOverloadingOperatorEqualsAnalyzer
        (Microsoft.CodeQuality.Analyzers,
        src/NetAnalyzers/VisualBasic/Microsoft.CodeQuality.Analyzers/ApiDesignGuidelines/BasicOverrideEqualsOnOverloadingOperatorEquals.vb),
        declared <DiagnosticAnalyzer(LanguageNames.VisualBasic)>; it RegisterSymbolAction(SymbolKind.NamedType)
        and reports CA2224 only for a VB compilation. There is NO C# or language-agnostic analyzer for this rule
        (the C# side ships only CSharpOverrideEqualsOnOverloadingOperatorEquals.Fixer.cs, a code fixer), so the
        analyzer registers no actions in this C#-only harness. Verified empirically on net10.0: a class with
        operator== and no Equals override never emits CA2224 even with <NoWarn>CS0660;CS0661</NoWarn> and
        AnalysisMode=All; only CA1046/CA2225 surface. Independently, in C# the compiler emits CS0660/CS0661 for
        that pattern, which TreatWarningsAsErrors promotes to build errors. Permanently untestable in a C#-only
        build harness. Source: dotnet/roslyn-analyzers (branch main),
        BasicOverrideEqualsOnOverloadingOperatorEquals.vb.
        """)]
[RuleDoc("CA2226", "Operators should have symmetrical overloads",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2226",
    Untestable = """
        CA2226's analyzer (OperatorsShouldHaveSymmetricalOverloadsAnalyzer,
        Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines) registers a SymbolKind.NamedType action and reports
        only when a user-defined binary operator (IsUserDefinedOperator() && GetParameters().Length==2) from a
        pair (==/!=, </>, <=/>=) lacks a same-parameter-type counterpart (HasSymmetricOperator ->
        HasSameParameterTypes, matched per-index via ITypeSymbol.Equals); its descriptor is declared
        RuleLevel.CandidateForRemoval with the in-source comment "// C# compiler reports an error". In C#,
        defining only one operator of any such pair is itself compile error CS0216 ("The operator ... requires a
        matching operator ... to also be defined"), which the compiler emits per operator BY SIGNATURE over the
        identical three pairs, failing the build before the analyzer diagnostic can reach SARIF. The compiler's
        signature-level matching is at least as strict as the analyzer's: it closes every escape hatch,
        including the asymmetric-parameter case the analyzer's own CSharpTestOperatorTypeAsync unit test covers
        (operator==(A,int)+operator!=(A,string), still marked {|CS0216:...|}), an orphan added to an
        otherwise-complete pair, relational sets, and nullable type-identity (int vs int?) edges. CS0216 is a
        hard ERROR, not a warning: both #pragma warning disable CS0216 and <NoWarn>CS0216 leave it firing (0
        Warnings, 2 Errors), so the build never reaches SARIF emission. The three analyzer-checked pairs are an
        exact subset of the compiler's CS0216-enforced set, so no C# shape exists where CA2226 fires but CS0216
        does not. Empirically confirmed on net10.0 (SDK 10.0.101, EnableNETAnalyzers + AnalysisLevel=latest-all +
        AnalysisMode=All + TreatWarningsAsErrors + ErrorLog SARIF): every paired-operator shape produced "error
        CS0216" and Build FAILED with no CA2226 in output. The rule is a ported-FxCop VB.NET fallback for
        languages lacking compiler-level operator-symmetry enforcement and is structurally unreachable in C#
        (same gate class as the repo's CA1047/CS0628 and CA2218/CS0659 entries). Source:
        src/NetAnalyzers/Core/Microsoft.CodeQuality.Analyzers/ApiDesignGuidelines/OperatorsShouldHaveSymmetricalOverloads.cs
        (release/9.0.3xx).
        """)]
[RuleDoc("CA2258", "Providing a 'DynamicInterfaceCastableImplementation' interface in Visual Basic is unsupported",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2258",
    Untestable = """
        CA2258's analyzer (DynamicInterfaceCastableImplementationAnalyzer,
        Microsoft.NetCore.Analyzers.InteropServices) reports the diagnostic
        (DynamicInterfaceCastableImplementationUnsupportedRuleId = "CA2258") only inside `if
        (context.Compilation.Language == LanguageNames.VisualBasic) { context.ReportDiagnostic(...); return; }`
        within AnalyzeType; the C# code path never reaches that report (it continues to CA2256/CA2257 only). The
        test harness is C#-only — ProjectBuilder.AddCsprojFile always emits a Microsoft.NET.Sdk C# test.csproj
        and AddFile writes C# source — so context.Compilation.Language is always LanguageNames.CSharp and the
        VisualBasic branch is unreachable. There is no C# analogue diagnostic for the same
        DynamicInterfaceCastableImplementationAttribute misuse, so no indirect workaround exists. Source:
        src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/InteropServices/DynamicInterfaceCastableImplementation.cs
        (dotnet/roslyn-analyzers).
        """)]
[RuleDoc("CA2321", "Do not deserialize with JavaScriptSerializer using a SimpleTypeResolver",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2321",
    Untestable = """
        CA2321's analyzer (DoNotUseInsecureDeserializerJavaScriptSerializerWithSimpleTypeResolver,
        Microsoft.NetCore.Analyzers.Security) registers its OperationKind.Invocation action inside a
        RegisterCompilationStartAction that returns early unless
        WellKnownTypeProvider.TryGetOrCreateTypeByMetadataName resolves all three of
        System.Web.Script.Serialization.JavaScriptSerializer, .JavaScriptTypeResolver, and .SimpleTypeResolver
        (WellKnownTypeNames.SystemWebScriptSerialization*). Those types live only in System.Web.Extensions.dll
        on .NET Framework (Applies-to monikers netframework-3.5..4.8.1; no .NET Core/.NET 5+/.NET Standard
        moniker) and were removed from .NET Core; on net10.0 they do not exist in any BCL or addable NuGet
        package, so the gate fails, no actions register, and source referencing the type would not even compile
        (CS0234). Source:
        src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerJavascriptSerializerWithSimpleTypeResolver.cs.
        """)]
[RuleDoc("EnableGenerateDocumentationFile", "Set MSBuild property 'GenerateDocumentationFile' to 'true'",
    HelpLink = "https://github.com/dotnet/roslyn/issues/41640",
    Untestable = """
        EnableGenerateDocumentationFile's analyzer (AbstractRemoveUnnecessaryImportsDiagnosticAnalyzer,
        Microsoft.CodeAnalysis.CodeStyle) constructs its descriptor
        s_enableGenerateDocumentationFileIdDescriptor with customTags: [.. DiagnosticCustomTags.Microsoft,
        EnforceOnBuild.Never.ToCustomTag()]. The EnforceOnBuild enum documents Never as "Indicates that the code
        style diagnostic is an IDE-only diagnostic that cannot be enforced on build", so the diagnostic is
        reported only during IDE/live analysis and never during a command-line dotnet build. The test harness
        asserts on dotnet build SARIF output, so it can never appear. Confirmed empirically by in-repo build
        SARIF: the rule appears only in tool.driver.rules (tagged EnforceOnBuild_Never) and in
        ruleConfigurationOverrides at level error (from the shipped
        dotnet_diagnostic.EnableGenerateDocumentationFile.severity = warning), but produces zero results entries
        even though its DocumentationMode.None precondition is reproducible by setting
        GenerateDocumentationFile=false. There is no MSBuild property, editorconfig severity,
        EnforceCodeStyleInBuild setting, PackageReference, LangVersion, or target-framework change that overrides
        the driver's EnforceOnBuild.Never command-line exclusion (see dotnet/roslyn issue #77120). Sources:
        src/Analyzers/Core/Analyzers/RemoveUnnecessaryImports/AbstractRemoveUnnecessaryImportsDiagnosticAnalyzer.cs
        and src/Analyzers/Core/Analyzers/EnforceOnBuild.cs.
        """)]
[RuleDoc("S3216", "\"ConfigureAwait(false)\" should be used",
    HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3216/",
    Untestable = """
        S3216 (TaskConfigureAwait) contains a hard guard at the top of its action:
        `if (c.Compilation.Options.OutputKind != OutputKind.DynamicallyLinkedLibrary
             || !c.Compilation.IsNetFrameworkTarget()) return;`
        (Source: SonarAnalyzer.CSharp/Rules/TaskConfigureAwait.cs on GitHub/SonarSource/sonar-dotnet).
        The rule therefore only fires on class-library (DLL) projects that target .NET Framework
        (i.e. `net4x` / `netstandard` targets where mscorlib is present). The test harness targets
        `net10.0` (a .NET Core target), so `IsNetFrameworkTarget()` is always false and no await
        expression can trigger S3216 regardless of code pattern, editorconfig severity, or
        OutputType=Library. Genuine structural untestable on this harness.
        """)]
[RuleDoc("CA2266", "File-based program entry point should start with '#!'",
    HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2266",
    Untestable = """
        CA2266 fires only on file-based C# programs — .cs files intended to be run directly via
        `dotnet run foo.cs` (the .NET 10+ file-based programs feature) that lack a shebang line
        (`#!/usr/bin/env dotnet`). The test harness builds a standard SDK-style project (.csproj) and
        has no mechanism to declare a file-based program entry point, so the compiler never sees the
        preconditions for CA2266 and the rule cannot appear in SARIF. Structurally untestable on a
        project-based harness. First shipped in Microsoft.CodeAnalysis.NetAnalyzers 10.0.300.
        """)]
[RuleDoc("MA0191", "Do not use the null-forgiving operator",
    HelpLink = "https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0191.md",
    Untestable = """
        MA0191 is registered with IsEnabledByDefault=false in Meziantou.Analyzer 3.x. Empirical testing
        with multiple `!` operator patterns (nullable parameter `value!`, null-checked field `_value!`,
        member access `value!.Length`) all produce zero MA0191 diagnostics in build SARIF even with
        dotnet_diagnostic.MA0191.severity=warning in the package editorconfig. Setting the editorconfig
        severity is not sufficient to activate this particular "Enabled: False" rule; the Meziantou
        documentation indicates additional opt-in configuration (a Meziantou-specific analyzer option)
        is required to enable rules that are off by default due to high false-positive risk. Without
        that option present in the test harness's editorconfig context, the rule cannot be triggered.
        """)]
[RuleDoc("S6664", "The code block contains too many logging calls",
    HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6664/",
    Untestable = """
        S6664 (LoggingCallsThreshold) is registered with `Enabled: false` in the SonarAnalyzer.CSharp
        assembly — the rule's default diagnostics descriptor has DiagnosticSeverity.Info and its
        IsEnabledByDefault flag is false. While setting dotnet_diagnostic.S6664.severity = warning in
        editorconfig overrides the severity, it does NOT override the IsEnabledByDefault gate on the
        Sonar SyntaxNodeAction registration: the action is registered unconditionally but the SonarQube
        build-time diagnostic suppression logic still honours the disabled state. Additionally, S6664
        has a configurable threshold (default maximumLogCallsPerBlock = 2) that is not configurable via
        editorconfig; any attempt to drive more than the threshold number of ILogger calls in a block
        did not produce S6664 in SARIF. Empirically verified: three distinct ILogger.LogXxx calls in a
        single catch block with Microsoft.Extensions.Logging.Abstractions 10.0.0 and explicit
        severity=warning produced no S6664 diagnostic. Genuine untestable in this harness.
        """)]
[RuleDoc("S6802", "Using lambda expressions in loops should be avoided in Blazor markup section",
    HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6802/",
    Untestable = """
        S6802 (BlazorQuerySelectorHtmlId) targets lambda expressions in Blazor markup — it requires a
        Razor component file (.razor) parsed by the Blazor source-generator, which is not present in the
        test harness's plain C# project. Additionally, the rule is registered with Enabled: false in the
        SonarAnalyzer.CSharp assembly (DiagnosticSeverity.Info, IsEnabledByDefault=false), so even with
        severity=warning the diagnostic suppression logic prevents it from surfacing in build SARIF.
        Both the Blazor-markup precondition and the IsEnabledByDefault gate independently make this rule
        untestable in a standard .csproj build harness.
        """)]
public static class UntestableRules { }
