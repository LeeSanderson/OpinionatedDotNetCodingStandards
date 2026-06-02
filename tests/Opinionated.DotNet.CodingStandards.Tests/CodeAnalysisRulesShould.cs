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
}
