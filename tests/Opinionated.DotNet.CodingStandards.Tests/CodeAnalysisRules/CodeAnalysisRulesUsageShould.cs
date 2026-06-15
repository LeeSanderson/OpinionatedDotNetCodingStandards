// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesUsageShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA2101", "Specify marshaling for P/Invoke string arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2101")]
    public async Task RequireMarshalingForPInvokeStringArguments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public static class Program
            {
                [DllImport("kernel32.dll")]
                public static extern bool CreateDirectory(string lpPathName, System.IntPtr lpSecurityAttributes);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2101").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2017", "Parameter count mismatch",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2017")]
    public async Task RequireMatchingLogMessageParameterCount()
    {
        // Microsoft.Extensions.Logging.Abstractions supplies ILogger/LoggerExtensions so the analyzer registers.
        // The message template has two named placeholders ({Name}, {Time}) but only one argument is supplied,
        // so paramsCount (1) != ValueNames.Count (2) and CA2017 fires.
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync(
            "Program.cs",
            """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Program
            {
                public static void Log(ILogger logger)
                {
                    logger.LogInformation("User {Name} logged in at {Time}", "Alice");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2017").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2254", "Template should be a static expression",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2254")]
    public async Task RequireStaticExpressionForLogMessageTemplate()
    {
        // Microsoft.Extensions.Logging.Abstractions supplies ILogger/LoggerExtensions so the analyzer registers.
        // CA2254 fires: the message template is a non-constant variable, so the LoggerExtensions.LogInformation
        // call has no static template text (TryGetFormatText returns null with usingLoggerExtensionsTypes == true).
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync(
            "Program.cs",
            """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Program
            {
                public static void Log(ILogger logger, string message)
                {
                    logger.LogInformation(message);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2254").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2119", "Seal methods that satisfy private interfaces",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2119")]
    public async Task RequireSealedMethodsForPrivateInterfaces()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            internal interface IPrivate
            {
                void DoWork();
            }
            public class MyClass : IPrivate
            {
                public virtual void DoWork() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2119").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2200", "Rethrow to preserve stack details",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2200")]
    public async Task RequireRethrowToPreserveStack()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Method() 
                {
                    try 
                    {
                        throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2201", "Do not raise reserved exception types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2201")]
    public async Task ProhibitThrowingReservedExceptionTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    throw new System.NullReferenceException("test");
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("CA2201").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2207", "Initialize value type static fields inline",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2207")]
    public async Task RequireValueTypeStaticFieldsInitializedInline()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public struct Counter
            {
                public static int Value;
                static Counter()
                {
                    Value = 42;
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2207").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2208", "Instantiate argument exceptions correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2208")]
    public async Task RequireCorrectExceptionInstantiation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Method(string value) 
                {
                    throw new ArgumentNullException("wrong parameter name");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2208").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2211", "Non-constant fields should not be visible",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2211")]
    public async Task ProhibitVisibleNonConstantStaticFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public static int Count = 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2211").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2213", "Disposable fields should be disposed",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2213")]
    public async Task RequireDisposalOfDisposableFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.IO;
            namespace test;
            public class MyClass : System.IDisposable
            {
                private readonly Stream _stream = new MemoryStream();
                public void Dispose() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2213").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2214", "Do not call overridable methods in constructors",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2214")]
    public async Task ProhibitVirtualMethodCallsInConstructors()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public MyClass()
                {
                    Initialize();
                }
                public virtual void Initialize() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2214").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2215", "Dispose methods should call base class dispose",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2215")]
    public async Task RequireBaseDisposeCallInDerivedDispose()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class Base : System.IDisposable
            {
                public virtual void Dispose() { }
            }
            public class Derived : Base
            {
                public override void Dispose() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2215").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2217", "Do not mark enums with FlagsAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2217")]
    public async Task ProhibitFlagsEnumWithNonPowerOfTwoValues()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            [System.Flags]
            public enum Days { Mon = 1, Tue = 2, Wed = 4, Thu = 8, Fri = 16, Weekend = 96 }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2217").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2219", "Do not raise exceptions in finally clauses",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2219")]
    public async Task ProhibitExceptionsInFinallyBlocks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    try
                    {
                        return 0;
                    }
                    finally
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2219").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2231", "Overload operator equals on overriding value type Equals",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2231")]
    public async Task RequireOperatorEqualsWhenValueTypeOverridesEquals()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public struct Point
            {
                public int X, Y;
                public override bool Equals(object? obj) => obj is Point p && p.X == X && p.Y == Y;
                public override int GetHashCode() => System.HashCode.Combine(X, Y);
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2231").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2235", "Mark all non-serializable fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2235")]
    public async Task RequireNonSerializableFieldsToBeMarked()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class NotSerializable { }
            [System.Serializable]
            public class Container
            {
                private readonly NotSerializable _field = new();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2235").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2237", "Mark ISerializable types with SerializableAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2237")]
    public async Task RequireSerializableAttributeOnISerializableTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Runtime.Serialization;
            namespace test;
            public class MyData : ISerializable
            {
                public void GetObjectData(SerializationInfo info, StreamingContext context) { }
                protected MyData(SerializationInfo info, StreamingContext context) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2237").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2241", "Provide correct arguments to formatting methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2241")]
    public async Task RequireCorrectFormatArguments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Format() 
                {
                    return string.Format("{0} {1}", "one");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2241").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2242", "Test for NaN correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2242")]
    public async Task RequireCorrectNaNTest()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool IsNaN(double value) 
                {
                    return value == double.NaN;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2242").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2243", "Attribute string literals should parse correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2243")]
    public async Task RequireAttributeStringLiteralsToParseCorrectly()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            // CA2243 matches any attribute argument whose parameter name contains the word
            // "url"/"uri"/"urn"/"guid" and is of type string; a custom attribute is not
            // compiler-validated, so the malformed URI literal fires the analyzer (not a CS error).
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class EndpointAttribute(string url) : Attribute
            {
                public string Url { get; } = url;
            }
            [Endpoint("http://not a valid uri")]
            public sealed class Service { }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2243").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2244", "Do not duplicate indexed element initializations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2244")]
    public async Task ProhibitDuplicateIndexedElementInitializations()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var dict = new Dictionary<int, string>
                    {
                        [1] = "first",
                        [1] = "second",
                    };
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2244").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2245", "Do not assign a property to itself",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2245")]
    public async Task RequireNotAssigningPropertyToItself()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public int Value { get; set; }
                public void Method() 
                {
                    Value = Value;
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2245").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2246", "Assigning symbol and its member in the same statement",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2246")]
    public async Task ProhibitAssigningSymbolAndMemberInSameStatement()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class Node
            {
                public Node? Next { get; set; }
            }
            public static class Program
            {
                public static int Main()
                {
                    var node = new Node();
                    node.Next = node = new Node();
                    return node is null ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2246").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2247", "Argument passed to TaskCompletionSource constructor should be TaskCreationOptions enum instead of TaskContinuationOptions enum",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2247")]
    public async Task RequireTaskCreationOptionsInTaskCompletionSourceConstructor()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var tcs = new TaskCompletionSource<int>(TaskContinuationOptions.None);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2247").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2248", "Provide correct 'enum' argument to 'Enum.HasFlag'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2248")]
    public async Task RequireCorrectEnumTypeForHasFlag()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            [System.Flags]
            public enum Color { Red = 1, Green = 2 }
            [System.Flags]
            public enum Size { Small = 1, Large = 2 }
            public static class Program
            {
                public static int Main()
                {
                    Color c = Color.Red;
                    bool result = c.HasFlag(Size.Small);
                    return result ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2248").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2249", "Consider using 'string.Contains' instead of 'string.IndexOf'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2249")]
    public async Task RequireStringContainsInsteadOfIndexOf()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool found = "hello world".IndexOf("world") >= 0;
                    return found ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2249").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2250", "Use 'ThrowIfCancellationRequested'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2250")]
    public async Task RequireThrowIfCancellationRequested()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Threading;
            namespace test;
            public static class Program
            {
                public static void Method(CancellationToken cancellationToken) 
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new System.OperationCanceledException();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2250").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2251", "Use 'string.Equals'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2251")]
    public async Task RequireStringEquals()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool Compare(string a, string b)
                {
                    return string.Compare(a, b, System.StringComparison.OrdinalIgnoreCase) == 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2251").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2252", "This API requires opting into preview features",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2252")]
    public async Task RequireOptInForPreviewFeatureApis()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Runtime.Versioning;
            namespace test;
            public static class PreviewApis
            {
                [RequiresPreviewFeatures]
                public static void UsePreview() { }
            }
            public static class Program
            {
                public static int Main()
                {
                    PreviewApis.UsePreview();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2252").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2253", "Named placeholders should not be numeric values",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2253")]
    public async Task ProhibitNumericLogMessagePlaceholders()
    {
        // CA2253 (LoggerMessageDefineAnalyzer) registers its OperationKind.Invocation action
        // only when ILogger, LoggerExtensions and LoggerMessage all resolve - all three live in
        // Microsoft.Extensions.Logging.Abstractions, which the harness CAN add as a PackageReference
        // (same approach already proven by the CA1727 test). For a LoggerExtensions call with a
        // constant template containing a numeric placeholder ("{0}"), LogValuesFormatter parses
        // ValueNames = ["0"] and the rule reports because int.TryParse("0", out _) succeeds. One
        // placeholder + one argument keeps the arg count matched (no CA2017), and "0" is not a
        // lower-cased name so CA1727 does not fire. CA2253 ships at Info but the package editorconfig
        // sets dotnet_diagnostic.CA2253.severity = warning, so TreatWarningsAsErrors surfaces it as
        // a SARIF error.
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync(
            "Program.cs",
            """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    Log(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
                    return 0;
                }
                private static void Log(ILogger logger)
                {
                    logger.LogInformation("Processed {0}", 42);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2253").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2255", "The 'ModuleInitializer' attribute should not be used in libraries",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2255")]
    public async Task ProhibitModuleInitializerInLibraries()
    {
        using var project = await CreateProjectBuilderAsync(properties: [("OutputType", "Library")]);
        await project.AddFileAsync(
            "Startup.cs",
            """
            using System.Runtime.CompilerServices;
            namespace test;
            public static class Startup
            {
                [ModuleInitializer]
                public static void Init() { }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2255").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2256", "All members declared in parent interfaces must have an implementation in a DynamicInterfaceCastableImplementation-attributed interface",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2256")]
    public async Task RequireAllParentInterfaceMembersImplementedInDynamicCastableInterface()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public interface IBase
            {
                void Method();
            }
            [DynamicInterfaceCastableImplementation]
            public interface IImpl : IBase
            {
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2256").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2257", "Members defined on an interface with the 'DynamicInterfaceCastableImplementationAttribute' should be 'static'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2257")]
    public async Task RequireStaticMembersInDynamicCastableImplementationInterface()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            [DynamicInterfaceCastableImplementation]
            public interface IMyInterface
            {
                void InstanceMethod() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2257").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2259", "'ThreadStatic' only affects static fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2259")]
    public async Task ProhibitThreadStaticOnInstanceFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                [System.ThreadStatic]
                private int _counter;
                public int GetCounter() => _counter;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2259").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2260", "Use correct type parameter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2260")]
    public async Task RequireCorrectSelfReferentialTypeParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Numerics;
            namespace test;
            public class MyClass : IMinMaxValue<int>
            {
                public static int MinValue => 0;
                public static int MaxValue => 100;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2260").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2261", "Do not use ConfigureAwaitOptions.SuppressThrowing with Task<TResult>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2261")]
    public async Task ProhibitSuppressThrowingWithGenericTask()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task Method()
                {
                    _ = await Task.FromResult(42).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2261").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2262", "Set 'MaxResponseHeadersLength' properly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2262")]
    public async Task RequireProperMaxResponseHeadersLength()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Net.Http;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var handler = new HttpClientHandler();
                    handler.MaxResponseHeadersLength = 64 * 1024;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("CA2262").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2263", "Prefer generic overload when type is known",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2263")]
    public async Task PreferGenericOverloadWhenTypeIsKnown()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var values = Enum.GetValues(typeof(DayOfWeek));
                    return values.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("CA2263").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2264", "Do not pass a non-nullable value to 'ArgumentNullException.ThrowIfNull'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2264")]
    public async Task ProhibitNonNullableArgumentForThrowIfNull()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int value = 42;
                    System.ArgumentNullException.ThrowIfNull(value);
                    return value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2264").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2265", "Do not compare Span<T> to 'null' or 'default'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2265")]
    public async Task ProhibitComparingSpanToDefault()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    System.Span<int> span = stackalloc int[1];
                    if (span == default)
                        return 1;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA2265").ShouldBeTrue();
    }
}
