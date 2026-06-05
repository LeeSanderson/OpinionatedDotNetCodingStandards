using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA1000", "Do not declare static members on generic types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000")]
    public async Task ProhibitStaticMembersOnGenericTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class GenericClass<T>
            {
                public static void StaticMethod() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1000").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1001", "Types that own disposable fields should be disposable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1001")]
    public async Task RequireDisposableOnTypesOwningDisposableFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            namespace test;
            public class MyClass
            {
                private readonly Stream _stream = new MemoryStream();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1001").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1010", "Generic interface should also be implemented",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1010")]
    public async Task RequireGenericInterfaceImplementation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections;
            namespace test;
            public class MyCollection : IEnumerable
            {
                public IEnumerator GetEnumerator() => throw new System.NotImplementedException();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1010").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1016", "Mark assemblies with assembly version",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1016")]
    public async Task RequireAssemblyVersionAttribute()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1016").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1018", "Mark attributes with AttributeUsageAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1018")]
    public async Task RequireAttributeUsageOnAttributes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyAttribute : System.Attribute
            {
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1036", "Override methods on comparable types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1036")]
    public async Task RequireOverrideMethodsOnComparable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IComparable
            {
                public int CompareTo(object? obj) => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1036").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1041", "Provide ObsoleteAttribute message",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1041")]
    public async Task RequireObsoleteAttributeMessage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                [System.Obsolete]
                public static void OldMethod() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1041").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1047", "Do not declare protected member in sealed type",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1047")]
    public async Task ProhibitProtectedMembersInSealedTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public sealed class MyClass
            {
                protected void Method() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1047").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1050", "Declare types in namespaces",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1050")]
    public async Task RequireTypesInNamespaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1050").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1051", "Do not declare visible instance fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1051")]
    public async Task ProhibitVisibleInstanceFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public int Field;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1051").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1061", "Do not hide base class methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1061")]
    public async Task ProhibitHidingBaseMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Base
            {
                public void Method() { }
            }
            public class Derived : Base
            {
                public new void Method() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1061").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1067", "Override Object.Equals(object) when implementing IEquatable<T>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1067")]
    public async Task RequireObjectEqualsWithIEquatable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IEquatable<MyClass>
            {
                public bool Equals(MyClass? other) => true;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1067").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1068", "CancellationToken parameters must come last",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1068")]
    public async Task RequireCancellationTokenLast()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            namespace test;
            public static class Program
            {
                public static void Method(CancellationToken token, int value) { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1068").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1069", "Enums values should not be duplicated",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1069")]
    public async Task ProhibitDuplicateEnumValues()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum MyEnum
            {
                First = 1,
                Second = 1
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1069").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1070", "Do not declare event fields as virtual",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1070")]
    public async Task ProhibitVirtualEventFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass
            {
                public virtual event EventHandler? MyEvent;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1070").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1825", "Avoid zero-length array allocations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1825")]
    public async Task RequireAvoidZeroLengthArrays()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int[] GetArray() => new int[0];
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1825").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1826", "Do not use Enumerable methods on indexable collections",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1826")]
    public async Task RequireIndexerOverLinq()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int Get() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.First();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1826").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1827", "Do not use Count() or LongCount() when Any() can be used",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1827")]
    public async Task RequireAnyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool HasItems() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.Count() > 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1827").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1829", "Use Length/Count property instead of Count() when available",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1829")]
    public async Task RequireLengthPropertyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int GetCount() 
                {
                    var array = new[] { 1, 2, 3 };
                    return array.Count();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1829").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1834", "Consider using 'StringBuilder.Append(char)' when applicable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1834")]
    public async Task RequireStringBuilderAppendChar()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text;
            namespace test;
            public static class Program
            {
                public static string Build() 
                {
                    var sb = new StringBuilder();
                    sb.Append("a");
                    return sb.ToString();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1834").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1836", "Prefer IsEmpty over Count",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1836")]
    public async Task RequireIsEmptyOverCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Concurrent;
            namespace test;
            public static class Program
            {
                public static bool HasItems() 
                {
                    var bag = new ConcurrentBag<int>();
                    return bag.Count > 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1836").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1841", "Prefer Dictionary.Contains methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1841")]
    public async Task RequireDictionaryContainsMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool HasKey() 
                {
                    var dict = new Dictionary<string, int>();
                    return dict.Keys.Contains("test");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1841").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1846", "Prefer 'AsSpan' over 'Substring'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1846")]
    public async Task RequireAsSpanOverSubstring()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get() 
                {
                    var text = "hello world";
                    return text.Substring(0, 5);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1846").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1847", "Use char literal for a single character lookup",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1847")]
    public async Task RequireCharLiteralForLookup()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Find() 
                {
                    var text = "hello";
                    return text.IndexOf("e");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1847").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1854", "Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1854")]
    public async Task RequireTryGetValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Get() 
                {
                    var dict = new Dictionary<string, int>();
                    if (dict.ContainsKey("test"))
                        return dict["test"];
                    return 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1854").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1861", "Avoid constant arrays as arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1861")]
    public async Task RequireAvoidConstantArrays()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static bool Contains(int value) 
                {
                    return new[] { 1, 2, 3 }.Contains(value);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1861").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1862", "Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1862")]
    public async Task RequireStringComparisonForCaseInsensitive()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool Compare() 
                {
                    return "hello".ToLower() == "HELLO".ToLower();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1862").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2251", "Use 'string.Equals'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2251")]
    public async Task RequireStringEquals()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool Compare(string a, string b) 
                {
                    return a.ToLower() == b;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2251").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2016", "Forward the 'CancellationToken' parameter to methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2016")]
    public async Task RequireForwardCancellationToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task MethodAsync(CancellationToken cancellationToken) 
                {
                    await Task.Delay(100);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2016").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2250", "Use 'ThrowIfCancellationRequested'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2250")]
    public async Task RequireThrowIfCancellationRequested()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2250").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1511", "Use ArgumentException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1511")]
    public async Task RequireArgumentExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Method(string value) 
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Value cannot be empty");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1511").ShouldBeTrue();
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
    [RuleDoc("CA2200", "Rethrow to preserve stack details",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2200")]
    public async Task RequireRethrowToPreserveStack()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1805", "Do not initialize unnecessarily",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1805")]
    public async Task RequireDoNotInitializeUnnecessarily()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static int _value = 0;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1805").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1806", "Do not ignore method results",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1806")]
    public async Task RequireNotIgnoringMethodResults()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void Method() 
                {
                    "test".Clone();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1806").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1816", "Dispose methods should call SuppressFinalize",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1816")]
    public async Task RequireDisposeCallsSuppressFinalize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IDisposable
            {
                public void Dispose() 
                {
                    // Missing GC.SuppressFinalize(this)
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1816").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1852", "Seal internal types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852")]
    public async Task RequireSealInternalTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            internal class MyClass
            {
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1852").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2208", "Instantiate argument exceptions correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2208")]
    public async Task RequireCorrectExceptionInstantiation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2208").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2241", "Provide correct arguments to formatting methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2241")]
    public async Task RequireCorrectFormatArguments()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2241").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2242", "Test for NaN correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2242")]
    public async Task RequireCorrectNaNTest()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2242").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2245", "Do not assign a property to itself",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2245")]
    public async Task RequireNotAssigningPropertyToItself()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2245").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1003", "Use generic event handler instances",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1003")]
    public async Task RequireGenericEventHandlerInstances()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public delegate void MyEventHandler(object sender, EventArgs e);
            public class MyClass
            {
                public event MyEventHandler? Changed;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1003").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1008", "Enums should have zero value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1008")]
    public async Task RequireEnumZeroValue()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum Status { Active = 1, Inactive = 2 }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1008").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1012", "Abstract types should not have public constructors",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1012")]
    public async Task ProhibitPublicConstructorsOnAbstractTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public abstract class AbstractBase
            {
                public AbstractBase() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1012").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1019", "Define accessors for attribute arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1019")]
    public async Task RequireAccessorsForAttributeArguments()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            [AttributeUsage(AttributeTargets.Class)]
            public class MyAttribute : Attribute
            {
                public MyAttribute(string name) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1027", "Mark enums with FlagsAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1027")]
    public async Task RequireFlagsAttributeOnBitmaskEnums()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum Permissions { None = 0, Read = 1, Write = 2, Execute = 4 }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1027").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1028", "Enum Storage should be Int32",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1028")]
    public async Task RequireInt32EnumStorage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public enum Color : byte { Red, Green, Blue }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1028").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1030", "Use events where appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1030")]
    public async Task RequireEventsForEventRaisingMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public void RaiseMyEvent() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1033", "Interface methods should be callable by child types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1033")]
    public async Task RequireInterfaceMethodsCallableByChildTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IComparable
            {
                int IComparable.CompareTo(object? obj) => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1033").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1043", "Use Integral Or String Argument For Indexers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1043")]
    public async Task RequireIntegralOrStringIndexers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyCollection
            {
                public int this[double key] => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1043").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1044", "Properties should not be write only",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1044")]
    public async Task ProhibitWriteOnlyProperties()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private int _value;
                public int Value { set { _value = value; } }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1044").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1046", "Do not overload equality operator on reference types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1046")]
    public async Task ProhibitEqualityOperatorOnReferenceTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            #pragma warning disable CS0660, CS0661
            public class MyClass
            {
                public static bool operator ==(MyClass? a, MyClass? b) => false;
                public static bool operator !=(MyClass? a, MyClass? b) => true;
            }
            #pragma warning restore CS0660, CS0661
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1046").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1052", "Static holder types should be Static or NotInheritable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1052")]
    public async Task RequireStaticClassForStaticOnlyMembers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Utilities
            {
                public static void DoWork() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1058", "Types should not extend certain base types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1058")]
    public async Task ProhibitExtendingApplicationException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyException : System.ApplicationException
            {
                public MyException(string message) : base(message) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1058").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1063", "Implement IDisposable Correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1063")]
    public async Task RequireCorrectIDisposableImplementation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IDisposable
            {
                ~MyClass() { }
                public void Dispose() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1063").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1065", "Do not raise exceptions in unexpected locations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1065")]
    public async Task ProhibitExceptionsInUnexpectedLocations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                static MyClass() { throw new System.InvalidOperationException("init failed"); }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1065").ShouldBeTrue();
    }

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
    [RuleDoc("CA1810", "Initialize reference type static fields inline",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1810")]
    public async Task RequireInlineStaticFieldInitialization()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Config
            {
                public static readonly string Value;
                static Config() { Value = "hello"; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1810").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1813", "Avoid unsealed attributes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1813")]
    public async Task RequireSealedAttributes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public class MyAttribute : System.Attribute { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1813").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1814", "Prefer jagged arrays over multidimensional",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1814")]
    public async Task PreferJaggedArraysOverMultidimensional()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Grid
            {
                public int[,] Matrix = new int[3, 3];
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1814").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1815", "Override equals and operator equals on value types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1815")]
    public async Task RequireEqualsOverrideOnValueTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public struct Point { public int X; public int Y; }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1815").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1820", "Test for empty strings using string length",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1820")]
    public async Task RequireStringLengthForEmptyStringTests()
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
                    string s = "hello";
                    if (s.Equals(string.Empty)) { }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1820").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1821", "Remove empty Finalizers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1821")]
    public async Task ProhibitEmptyFinalizers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass { ~MyClass() { } }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1821").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1823", "Avoid unused private fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1823")]
    public async Task ProhibitUnusedPrivateFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private string _unused = "value";
                public string GetValue() => "other";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1823").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1830", "Prefer strongly-typed Append and Insert method overloads on StringBuilder",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1830")]
    public async Task RequireStronglyTypedStringBuilderOverloads()
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
                    var sb = new System.Text.StringBuilder();
                    sb.Append(true.ToString());
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1830").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1831", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1831")]
    public async Task RequireAsSpanInsteadOfStringRangeIndexer()
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
                    string text = "hello world";
                    System.ReadOnlySpan<char> slice = text[0..5];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1831").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1832", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1832")]
    public async Task RequireAsSpanInsteadOfArrayRangeIndexerForReadOnlySpan()
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
                    byte[] bytes = new byte[10];
                    System.ReadOnlySpan<byte> slice = bytes[2..];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1832").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1833", "Use AsSpan or AsMemory instead of Range-based indexers when appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1833")]
    public async Task RequireAsMemoryInsteadOfArrayRangeIndexerForReadOnlyMemory()
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
                    byte[] bytes = new byte[10];
                    System.Memory<byte> slice = bytes[2..];
                    return slice.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1833").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1835", "Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1835")]
    public async Task RequireMemoryBasedStreamReadAsyncOverloads()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task<int> Main()
                {
                    using var stream = new MemoryStream();
                    var buffer = new byte[1024];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1835").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1837", "Use 'Environment.ProcessId'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1837")]
    public async Task RequireEnvironmentProcessId()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int pid = Process.GetCurrentProcess().Id;
                    return pid == 0 ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1837").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1838", "Avoid 'StringBuilder' parameters for P/Invokes",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1838")]
    public async Task ProhibitStringBuilderPInvokeParameters()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            using System.Text;
            namespace test;
            internal static class NativeMethods
            {
                [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
                internal static extern int GetTempPath(int nBufferLength, StringBuilder lpBuffer);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1838").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1839", "Use 'Environment.ProcessPath'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1839")]
    public async Task RequireEnvironmentProcessPath()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string path = Process.GetCurrentProcess().MainModule!.FileName;
                    return path == null ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1839").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1840", "Use 'Environment.CurrentManagedThreadId'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1840")]
    public async Task RequireEnvironmentCurrentManagedThreadId()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    return threadId == 0 ? 1 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1840").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1844", "Provide memory-based overrides of async methods when subclassing 'Stream'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1844")]
    public async Task RequireMemoryBasedStreamAsyncOverrides()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Threading;
            using System.Threading.Tasks;
            namespace test;
            public class MyStream : Stream
            {
                public override bool CanRead => true;
                public override bool CanSeek => false;
                public override bool CanWrite => true;
                public override long Length => 0;
                public override long Position { get => 0; set { } }
                public override void Flush() { }
                public override int Read(byte[] buffer, int offset, int count) => 0;
                public override long Seek(long offset, SeekOrigin origin) => 0;
                public override void SetLength(long value) { }
                public override void Write(byte[] buffer, int offset, int count) { }
                public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
                    => Task.FromResult(0);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1844").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1849", "Call async methods when in an async method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1849")]
    public async Task RequireAsyncMethodsInAsyncContext()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task<int> Main()
                {
                    var semaphore = new SemaphoreSlim(1);
                    semaphore.Wait();
                    return await Task.FromResult(0);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1849").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1850", "Prefer static 'HashData' method over 'ComputeHash'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1850")]
    public async Task RequireStaticHashDataMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    byte[] data = new byte[10];
                    using var sha = SHA256.Create();
                    byte[] hash = sha.ComputeHash(data);
                    return hash.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1850").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1851", "Possible multiple enumerations of 'IEnumerable' collection",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1851")]
    public async Task ProhibitMultipleEnumerationsOfIEnumerable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int CountItems(IEnumerable<int> items)
                {
                    if (!items.Any())
                        return 0;
                    return items.Count();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1851").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1855", "Prefer 'Clear' over 'Fill'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1855")]
    public async Task PreferClearOverFillDefault()
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
                    byte[] bytes = new byte[10];
                    System.Span<byte> span = bytes;
                    span.Fill(default);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1855").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1856", "Incorrect usage of ConstantExpected attribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1856")]
    public async Task ProhibitIncorrectConstantExpectedUsage()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public static class Program
            {
                public static int Method([ConstantExpected(Min = 10, Max = 5)] int value) => value;
                public static int Main() => Method(7);
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1856").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1857", "A constant is expected for the parameter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1857")]
    public async Task RequireConstantForConstantExpectedParameter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Diagnostics.CodeAnalysis;
            namespace test;
            public static class Helper
            {
                public static int Double([ConstantExpected] int x) => x * 2;
            }
            public static class Program
            {
                static int _multiplier = 3;
                public static int Main() => Helper.Double(_multiplier);
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1857").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1858", "Use 'StartsWith' instead of 'IndexOf'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1858")]
    public async Task RequireStartsWithInsteadOfIndexOfEqualsZero()
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
                    bool startsWithHello = s.IndexOf("hello") == 0;
                    return startsWithHello ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1858").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1864", "Prefer the 'IDictionary.TryAdd(TKey, TValue)' method",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1864")]
    public async Task RequireTryAddInsteadOfContainsKeyAndAdd()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var dict = new Dictionary<string, int>();
                    if (!dict.ContainsKey("key"))
                        dict.Add("key", 1);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1864").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1865", "Use char overload",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1865")]
    public async Task RequireCharOverloadForStartsWith()
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
                    bool starts = s.StartsWith("h", StringComparison.Ordinal);
                    return starts ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1865").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1868", "Unnecessary call to 'Contains(item)'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1868")]
    public async Task ProhibitUnnecessaryContainsBeforeSetAdd()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var set = new HashSet<string>();
                    if (!set.Contains("item"))
                        set.Add("item");
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1868").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1869", "Cache and reuse 'JsonSerializerOptions' instances",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1869")]
    public async Task RequireCachedJsonSerializerOptions()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.Json;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    string json = JsonSerializer.Serialize(42, new JsonSerializerOptions { WriteIndented = true });
                    return json.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1869").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1871", "Do not pass a nullable struct to 'ArgumentNullException.ThrowIfNull'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1871")]
    public async Task ProhibitNullableStructThrowIfNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main(int? value = null)
                {
                    System.ArgumentNullException.ThrowIfNull(value);
                    return value!.Value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1871").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1872", "Prefer 'Convert.ToHexString' and 'Convert.ToHexStringLower' over call chains based on 'BitConverter.ToString'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1872")]
    public async Task RequireConvertToHexStringOverBitConverter()
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
                    byte[] bytes = new byte[] { 1, 2, 3 };
                    string hex = System.BitConverter.ToString(bytes).Replace("-", "");
                    return hex.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1872").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1874", "Use 'Regex.IsMatch'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1874")]
    public async Task RequireRegexIsMatchInsteadOfMatchesCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.RegularExpressions;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    bool hasDigits = Regex.Match("abc123", @"\d+").Success;
                    return hasDigits ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1874").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1875", "Use 'Regex.Count'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1875")]
    public async Task RequireRegexCountInsteadOfMatchesCount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Text.RegularExpressions;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    int count = Regex.Matches("abc123def456", @"\d+").Count;
                    return count;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA1875").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2002", "Do not lock on objects with weak identity",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2002")]
    public async Task ProhibitLockOnWeakIdentityObjects()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private static readonly string _lock = "lock";
                public static void Method()
                {
                    lock (_lock) { }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2002").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2009", "Do not call ToImmutableCollection on an ImmutableCollection value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2009")]
    public async Task ProhibitRedundantToImmutableCollection()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Immutable;
            namespace test;
            public static class Program
            {
                public static void Method(ImmutableList<int> list)
                {
                    var result = list.ToImmutableList();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2009").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2011", "Avoid infinite recursion",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2011")]
    public async Task ProhibitInfiniteRecursionInPropertySetter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private int _value;
                public int Value
                {
                    get => _value;
                    set => Value = value;
                }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2011").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2012", "Use ValueTasks correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2012")]
    public async Task ProhibitAwaitingSameValueTaskTwice()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task Method()
                {
                    var vt = GetValueAsync();
                    var a = await vt;
                    var b = await vt;
                }
                static ValueTask<int> GetValueAsync() => ValueTask.FromResult(42);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2012").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2013", "Do not use ReferenceEquals with value types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2013")]
    public async Task ProhibitReferenceEqualsWithValueTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static bool AreEqual(int a, int b)
                {
                    return System.Object.ReferenceEquals(a, b);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2013").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2014", "Do not use stackalloc in loops",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2014")]
    public async Task ProhibitStackallocInLoops()
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
                    for (int i = 0; i < 10; i++)
                    {
                        System.Span<int> buffer = stackalloc int[100];
                        buffer[0] = i;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2014").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2015", "Do not define finalizers for types derived from MemoryManager<T>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2015")]
    public async Task ProhibitFinalizersOnMemoryManagerSubclasses()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Buffers;
            namespace test;
            public class MyMemoryManager : MemoryManager<byte>
            {
                ~MyMemoryManager() { }
                public override Memory<byte> Memory => Memory<byte>.Empty;
                public override Span<byte> GetSpan() => Span<byte>.Empty;
                public override MemoryHandle Pin(int elementIndex = 0) => default;
                public override void Unpin() { }
                protected override void Dispose(bool disposing) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2015").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2018", "'Buffer.BlockCopy' expects the number of bytes to be copied for the 'count' argument",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2018")]
    public async Task RequireByteCountForBufferBlockCopy()
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
                    int[] src = { 1, 2, 3 };
                    int[] dst = new int[3];
                    System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2019", "Improper 'ThreadStatic' field initialization",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2019")]
    public async Task ProhibitThreadStaticFieldWithInitializer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class MyClass
            {
                [System.ThreadStatic]
                private static int _counter = 1;
                public static int GetCounter() => _counter;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2019").ShouldBeTrue();
    }


    [Fact]
    [RuleDoc("CA2021", "Do not call Enumerable.Cast<T> or Enumerable.OfType<T> with incompatible types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021")]
    public async Task ProhibitEnumerableCastWithIncompatibleTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Linq;
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    double[] doubles = { 1.0, 2.0 };
                    var ints = doubles.Cast<int>();
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2021").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2022", "Avoid inexact read with 'Stream.Read'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2022")]
    public async Task ProhibitInexactStreamRead()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            namespace test;
            public static class Program
            {
                public static void Process(Stream stream)
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2022").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2024", "Do not use 'StreamReader.EndOfStream' in async methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2024")]
    public async Task ProhibitStreamReaderEndOfStreamInAsyncMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.IO;
            using System.Threading.Tasks;
            namespace test;
            public static class Program
            {
                public static async Task ReadAsync(StreamReader reader)
                {
                    while (!reader.EndOfStream)
                    {
                        await reader.ReadLineAsync();
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2024").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2101", "Specify marshaling for P/Invoke string arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2101")]
    public async Task RequireMarshalingForPInvokeStringArguments()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2101").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2119", "Seal methods that satisfy private interfaces",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2119")]
    public async Task RequireSealedMethodsForPrivateInterfaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2119").ShouldBeTrue();
    }


    [Fact]
    [RuleDoc("CA2201", "Do not raise reserved exception types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2201")]
    public async Task ProhibitThrowingReservedExceptionTypes()
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
                    throw new System.NullReferenceException("test");
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA2201").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2207", "Initialize value type static fields inline",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2207")]
    public async Task RequireValueTypeStaticFieldsInitializedInline()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2207").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2211", "Non-constant fields should not be visible",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2211")]
    public async Task ProhibitVisibleNonConstantStaticFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2211").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2213", "Disposable fields should be disposed",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2213")]
    public async Task RequireDisposalOfDisposableFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2213").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2214", "Do not call overridable methods in constructors",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2214")]
    public async Task ProhibitVirtualMethodCallsInConstructors()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2214").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2215", "Dispose methods should call base class dispose",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2215")]
    public async Task RequireBaseDisposeCallInDerivedDispose()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2215").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2217", "Do not mark enums with FlagsAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2217")]
    public async Task ProhibitFlagsEnumWithNonPowerOfTwoValues()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2217").ShouldBeTrue();
    }


    [Fact]
    [RuleDoc("CA2219", "Do not raise exceptions in finally clauses",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2219")]
    public async Task ProhibitExceptionsInFinallyBlocks()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2219").ShouldBeTrue();
    }



    [Fact]
    [RuleDoc("CA2231", "Overload operator equals on overriding value type Equals",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2231")]
    public async Task RequireOperatorEqualsWhenValueTypeOverridesEquals()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2231").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2235", "Mark all non-serializable fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2235")]
    public async Task RequireNonSerializableFieldsToBeMarked()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2235").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2237", "Mark ISerializable types with SerializableAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2237")]
    public async Task RequireSerializableAttributeOnISerializableTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2237").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2244", "Do not duplicate indexed element initializations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2244")]
    public async Task ProhibitDuplicateIndexedElementInitializations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2244").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2246", "Assigning symbol and its member in the same statement",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2246")]
    public async Task ProhibitAssigningSymbolAndMemberInSameStatement()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2246").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2247", "Argument passed to TaskCompletionSource constructor should be TaskCreationOptions enum instead of TaskContinuationOptions enum",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2247")]
    public async Task RequireTaskCreationOptionsInTaskCompletionSourceConstructor()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2247").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2248", "Provide correct 'enum' argument to 'Enum.HasFlag'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2248")]
    public async Task RequireCorrectEnumTypeForHasFlag()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2248").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2249", "Consider using 'string.Contains' instead of 'string.IndexOf'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2249")]
    public async Task RequireStringContainsInsteadOfIndexOf()
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
                    bool found = "hello world".IndexOf("world") >= 0;
                    return found ? 0 : 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2249").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2252", "This API requires opting into preview features",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2252")]
    public async Task RequireOptInForPreviewFeatureApis()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2252").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2255", "The 'ModuleInitializer' attribute should not be used in libraries",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2255")]
    public async Task ProhibitModuleInitializerInLibraries()
    {
        using var project = await CreateProjectBuilder(properties: [("OutputType", "Library")]);
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2255").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2256", "All members declared in parent interfaces must have an implementation in a DynamicInterfaceCastableImplementation-attributed interface",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2256")]
    public async Task RequireAllParentInterfaceMembersImplementedInDynamicCastableInterface()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2256").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2257", "Members defined on an interface with the 'DynamicInterfaceCastableImplementationAttribute' should be 'static'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2257")]
    public async Task RequireStaticMembersInDynamicCastableImplementationInterface()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2257").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2259", "'ThreadStatic' only affects static fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2259")]
    public async Task ProhibitThreadStaticOnInstanceFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2259").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2260", "Use correct type parameter",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2260")]
    public async Task RequireCorrectSelfReferentialTypeParameter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2260").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2261", "Do not use ConfigureAwaitOptions.SuppressThrowing with Task<TResult>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2261")]
    public async Task ProhibitSuppressThrowingWithGenericTask()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2261").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2262", "Set 'MaxResponseHeadersLength' properly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2262")]
    public async Task RequireProperMaxResponseHeadersLength()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA2262").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2263", "Prefer generic overload when type is known",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2263")]
    public async Task PreferGenericOverloadWhenTypeIsKnown()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasNote("CA2263").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2264", "Do not pass a non-nullable value to 'ArgumentNullException.ThrowIfNull'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2264")]
    public async Task ProhibitNonNullableArgumentForThrowIfNull()
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
                    int value = 42;
                    System.ArgumentNullException.ThrowIfNull(value);
                    return value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2264").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA2265", "Do not compare Span<T> to 'null' or 'default'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2265")]
    public async Task ProhibitComparingSpanToDefault()
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
                    System.Span<int> span = stackalloc int[1];
                    if (span == default)
                        return 1;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2265").ShouldBeTrue();
    }
}
