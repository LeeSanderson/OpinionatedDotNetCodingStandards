using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesGlobInteropMaintNamingShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA1304", "Specify CultureInfo",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1304")]
    public async Task RequireCultureInfoInStringComparisons()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    _ = string.Compare("a", "b", true);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1304").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1305", "Specify IFormatProvider",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1305")]
    public async Task RequireIFormatProviderInFormatting()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int n = 42;
                    _ = n.ToString();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1305").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1307", "Specify StringComparison for clarity",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1307")]
    public async Task RequireStringComparisonForClarity()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool eq = "hello".Equals("HELLO");
                    return eq ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1307").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1309", "Use ordinal string comparison",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1309")]
    public async Task RequireOrdinalStringComparison()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool eq = "hello".Equals("hello", System.StringComparison.CurrentCulture);
                    return eq ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1309").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1310", "Specify StringComparison for correctness",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1310")]
    public async Task RequireStringComparisonForCorrectness()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool sw = "hello world".StartsWith("hello");
                    return sw ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1310").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1311", "Specify a culture or use an invariant version",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1311")]
    public async Task RequireCultureInStringCaseConversion()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string lower = "HELLO".ToLower();
                    return lower.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1311").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1401", "P/Invokes should not be visible",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1401")]
    public async Task ProhibitPublicPInvokeDeclarations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public class NativeMethods
            {
                [DllImport("kernel32.dll")]
                public static extern int GetCurrentThreadId();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1401").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1416", "Validate platform compatibility",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416")]
    public async Task RequirePlatformGuardForPlatformSpecificCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Versioning;
            namespace test;
            public static class Platform
            {
                [SupportedOSPlatform("windows")]
                public static void WindowsOnly() { }
                public static void CallFromAnywhere() { WindowsOnly(); }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1416").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1417", "Do not use 'OutAttribute' on string parameters for P/Invokes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1417")]
    public async Task ProhibitOutAttributeOnStringPInvokeParameters()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public class NativeMethods
            {
                [DllImport("kernel32.dll")]
                public static extern void GetText([Out] string buffer);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1417").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1418", "Use valid platform string",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1418")]
    public async Task RequireValidPlatformStringInAttributes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Versioning;
            namespace test;
            [SupportedOSPlatform("invalid-os")]
            public static class MyPlatformClass { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1418").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1422", "Validate platform compatibility",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1422")]
    public async Task RequirePlatformVersionGuardForObsoletedPlatformApis()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.Versioning;
            namespace test;
            public static class LegacyApi
            {
                [ObsoletedOSPlatform("windows10.0.19041")]
                public static void OldMethod() { }
            }
            public static class Program
            {
                public static int Main()
                {
                    LegacyApi.OldMethod();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1422").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1510", "Use ArgumentNullException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1510")]
    public async Task RequireArgumentNullExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    Validate("hello");
                    return 0;
                }
                static void Validate(string? value)
                {
                    if (value == null) throw new System.ArgumentNullException(nameof(value));
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1510").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1512", "Use ArgumentOutOfRangeException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1512")]
    public async Task RequireArgumentOutOfRangeExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Method(int value) 
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1512").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1513", "Use ObjectDisposedException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1513")]
    public async Task RequireObjectDisposedExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IDisposable
            {
                private bool _disposed;
                public void Method() 
                {
                    if (_disposed)
                        throw new ObjectDisposedException(nameof(MyClass));
                }
                public void Dispose() => _disposed = true;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1513").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1514", "Avoid redundant length argument",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1514")]
    public async Task ProhibitRedundantLengthArgument()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string s = "hello world";
                    _ = s.Substring(6, s.Length - 6);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1514").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1700", "Do not name enum values 'Reserved'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1700")]
    public async Task ProhibitReservedEnumValues()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum Status { Active = 1, Reserved = 2 }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1700").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1708", "Identifiers should differ by more than case",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1708")]
    public async Task RequireIdentifiersToDifferByMoreThanCase()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Utilities { }
            public class utilities { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1708").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1712", "Do not prefix enum values with type name",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1712")]
    public async Task ProhibitEnumValuesPrefixedWithTypeName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum Status { StatusActive = 1, StatusInactive = 2 }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1712").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1713", "Events should not have 'Before' or 'After' prefix",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1713")]
    public async Task ProhibitBeforeOrAfterPrefixOnEvents()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Order
            {
                public event System.EventHandler? BeforeSave;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1713").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1715", "Identifiers should have correct prefix",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1715")]
    public async Task RequireIPrefixOnInterfaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public interface Vehicle { void Move(); }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1715").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1721", "Property names should not match get methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1721")]
    public async Task ProhibitPropertyNameMatchingGetMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Config
            {
                public string Timeout { get; set; } = "";
                public string GetTimeout() => Timeout;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1721").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1725", "Parameter names should match base declaration",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1725")]
    public async Task RequireParameterNamesToMatchBaseDeclaration()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public abstract class Animal
            {
                public abstract void Speak(string sound);
            }
            public class Dog : Animal
            {
                public override void Speak(string noise) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1725").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1419", "Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1419")]
    public async Task RequireParameterlessConstructorOnSafeHandleSubclass()
    {
        // The old probe declared a SafeHandle subclass with ONLY a parameterized
        // constructor (no parameterless one) and asserted CA1419 should fire. That is
        // exactly the case the analyzer does NOT flag. The analyzer
        // (ProvidePublicParameterlessSafeHandleConstructorAnalyzer in roslyn-analyzers)
        // registers a SymbolAction on NamedType and, for a concrete class inheriting
        // SafeHandle, iterates type.InstanceConstructors looking ONLY for a constructor
        // whose Parameters.Length == 0; it reports only when such a parameterless
        // constructor exists AND is less visible than the type. With no parameterless
        // constructor at all the loop body never matches, so nothing is emitted.
        // The real violation is a public SafeHandle subclass whose parameterless
        // constructor is LESS visible (here private) than the public containing type.
        // CA1419 ships at RuleLevel.IdeSuggestion, but the package's editorconfig sets
        // dotnet_diagnostic.CA1419.severity = warning, and TreatWarningsAsErrors=true
        // promotes it to an error in SARIF -> HasError.
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public class MyHandle : SafeHandle
            {
                private MyHandle() : base(System.IntPtr.Zero, true) { }
                public MyHandle(System.IntPtr handle) : base(System.IntPtr.Zero, true)
                {
                    SetHandle(handle);
                }
                public override bool IsInvalid => handle == System.IntPtr.Zero;
                protected override bool ReleaseHandle() { return true; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1419").ShouldBeTrue();
    }

    [Fact(Skip = "untestable")]
    [RuleDoc("CA1511", "Use ArgumentException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1511",
        Untestable = "Rule does not produce its own diagnostic ID in build SARIF in NetAnalyzers 10.0.x; only IDE0055 fires at the class declaration level for the standard 'if (string.IsNullOrEmpty) throw new ArgumentException' pattern, consistent with formatter-backed diagnostic routing")]
    public async Task UseArgumentExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Validate(string value)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Value cannot be null or empty", nameof(value));
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1511").ShouldBeTrue();
    }
}
