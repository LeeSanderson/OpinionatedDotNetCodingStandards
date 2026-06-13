// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBugs2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S2737", "catch clauses should do more than rethrow",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2737/")]
    public async Task WarnOnCatchClauseThatOnlyRethrows()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Example
            {
                public static void Run()
                {
                    try
                    {
                        System.Console.WriteLine("work");
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2737").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2757", "Non-existent operators like '=+' should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2757/")]
    public async Task DetectNonExistentOperators()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class C
            {
                public static int Compute(int x, int y)
                {
                    x =+ y;
                    return x;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2757").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2760", "Sequential tests should not check the same condition",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2760/")]
    public async Task DetectSequentialTestsWithSameCondition()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int a;
                public static int b;
                public static void DoSomething() { }

                public static void Method()
                {
                    if (a == b)
                    {
                        DoSomething();
                    }
                    if (a == b) // S2760: same condition as the preceding if, no update in between
                    {
                        DoSomething();
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2760").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2761", "Doubled prefix operators '!!' and '~~' should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2761/")]
    public async Task ProhibitDoubledPrefixOperators()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Sample
            {
                public static bool DoubleNot(bool x) => !!x;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2761").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2857", "SQL keywords should be delimited by whitespace",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2857/")]
    public async Task DetectSqlKeywordsNotDelimitedByWhitespace()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Data;

            namespace test;

            internal static class SqlQueryBuilder
            {
                internal static string Build() =>
                    "SELECT" + "Id FROM Customers";
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2857").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2930", "IDisposables should be disposed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2930/")]
    public async Task WarnOnUndisposedIDisposable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var fs = new System.IO.FileStream("test.txt", System.IO.FileMode.Create);
                    fs.WriteByte(1);
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2930").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2934", "Property assignments should not be made for 'readonly' fields not constrained to reference types",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2934/")]
    public async Task WarnOnPropertyAssignmentToReadonlyUnconstrainedGenericField()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public interface IPoint { int X { get; set; } }

            public class Container<T> where T : IPoint
            {
                private readonly T _point;
                public Container(T point) { _point = point; }
                public void Update() { _point.X = 42; }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2934").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2955", "Generic parameters not constrained to reference types should not be compared to \"null\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2955/")]
    public async Task WarnOnNullComparisonOfUnconstrainedGenericParameter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Checker
            {
                public static bool IsNull<T>(T value) => value == null;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2955").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2970", "Assertions should be complete",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2970/")]
    public async Task DetectIncompleteFluentAssertion()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "FluentAssertions", Version: "8.10.0")]);
        await project.AddFile("Program.cs", """
            using FluentAssertions;
            namespace test;
            public static class TestSuite
            {
                public static void IncompleteAssertion()
                {
                    int value = 42;
                    value.Should();
                }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2970").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2996", "ThreadStatic fields should not be initialized",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2996/")]
    public async Task WarnOnThreadStaticFieldWithInitializer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class C
            {
                [System.ThreadStatic]
                private static int _counter = 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2996").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2997", "IDisposables created in a 'using' statement should not be returned",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2997/")]
    public async Task DetectDisposableReturnedFromUsingStatement()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.IO;

            namespace test;

            public sealed class Factory
            {
                public static Stream Create()
                {
                    using var stream = new MemoryStream();
                    return stream;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2997").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3005", "ThreadStatic should not be used on non-static fields",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3005/")]
    public async Task WarnOnThreadStaticOnInstanceField()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Example
            {
                [System.ThreadStatic]
                public int NonStaticThreadStaticField;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3010", "Static fields should not be updated in constructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3010/")]
    public async Task WarnOnStaticFieldAssignmentInInstanceConstructor()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Counter
            {
                private static int _count;
                public Counter()
                {
                    _count++;
                }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3010").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3168", "async methods should not return void",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3168/")]
    public async Task WarnOnAsyncVoidMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyClass
            {
                public async void DoWork() { await System.Threading.Tasks.Task.Delay(1); }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3168").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3172", "Delegates should not be subtracted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3172/")]
    public async Task DetectDelegateSubtraction()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public delegate void MyDelegate();
            public static class Program
            {
                public static int Main()
                {
                    MyDelegate first = () => { };
                    MyDelegate second = () => { };
                    MyDelegate chain = first + second;
                    chain -= first + second;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3172").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3234", "GC.SuppressFinalize should not be invoked for types without destructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3234/")]
    public async Task WarnOnSuppressFinalizeWithoutDestructor()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public sealed class NoFinalizer : System.IDisposable
            {
                public void Dispose()
                {
                    GC.SuppressFinalize(this);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3234").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3237", "\"value\" contextual keyword should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3237/")]
    public async Task WarnWhenSetterIgnoresValueKeyword()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Counter
            {
                private int _count;

                public int Count
                {
                    get => _count;
                    set { _count = 0; } // S3237: 'value' is not used; assignment is silently discarded
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3237").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3249", "Classes directly extending \"object\" should not call \"base\" in \"GetHashCode\" or \"Equals\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3249/")]
    public async Task WarnOnBaseCallInGetHashCodeOrEqualsForDirectObjectSubclass()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class MyValue
            {
                private readonly int _id;

                public MyValue(int id) { _id = id; }

                public override bool Equals(object? obj)
                {
                    if (obj is not MyValue other) return false;
                    return base.Equals(other);
                }

                public override int GetHashCode() => base.GetHashCode();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3249").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3263", "Static fields should appear in the order they must be initialized",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3263/")]
    public async Task WarnOnStaticFieldInitializerOutOfOrder()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                static int A = B + 1;
                static int B = 10;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3263").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3264", "Events should be invoked",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3264/")]
    public async Task WarnOnEventNeverInvoked()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Counter
            {
                public event EventHandler? ThresholdReached;
                private int _count;
                public void Increment()
                {
                    _count++;
                    // ThresholdReached is never invoked — S3264 fires here
                }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3264").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3265", "Non-flags enums should not be used in bitwise operations",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3265/")]
    public async Task WarnOnBitwiseOperationsOnNonFlagsEnum()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public enum Direction { North, South, East, West }

            public static class Checker
            {
                public static Direction Combine(Direction a, Direction b) => a | b;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3265").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3346", "Expressions used in \"Debug.Assert\" should not produce side effects",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3346/")]
    public async Task WarnOnSideEffectsInDebugAssert()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Diagnostics;

            namespace test;

            public class C
            {
                private int _count;

                public bool Add(int x)
                {
                    _count += x;
                    return _count > 0;
                }
            }

            public static class Program
            {
                public static void Method(int x)
                {
                    var c = new C();
                    Debug.Assert(c.Add(x));
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3346").ShouldBeTrue();
    }
}
