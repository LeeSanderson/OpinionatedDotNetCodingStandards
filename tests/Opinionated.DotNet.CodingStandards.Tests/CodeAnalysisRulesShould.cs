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
