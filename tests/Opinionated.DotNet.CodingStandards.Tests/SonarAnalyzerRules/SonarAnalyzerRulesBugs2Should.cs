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
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2737").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2757", "Non-existent operators like '=+' should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2757/")]
    public async Task DetectNonExistentOperators()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2757").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2760", "Sequential tests should not check the same condition",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2760/")]
    public async Task DetectSequentialTestsWithSameCondition()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2760").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2761", "Doubled prefix operators '!!' and '~~' should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2761/")]
    public async Task ProhibitDoubledPrefixOperators()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Sample
            {
                public static bool DoubleNot(bool x) => !!x;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2761").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2857", "SQL keywords should be delimited by whitespace",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2857/")]
    public async Task DetectSqlKeywordsNotDelimitedByWhitespace()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Data;

            namespace test;

            internal static class SqlQueryBuilder
            {
                internal static string Build() =>
                    "SELECT" + "Id FROM Customers";
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2857").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2930", "IDisposables should be disposed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2930/")]
    public async Task WarnOnUndisposedIDisposable()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2930").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2934", "Property assignments should not be made for 'readonly' fields not constrained to reference types",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2934/")]
    public async Task WarnOnPropertyAssignmentToReadonlyUnconstrainedGenericField()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2934").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2955", "Generic parameters not constrained to reference types should not be compared to \"null\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2955/")]
    public async Task WarnOnNullComparisonOfUnconstrainedGenericParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Checker
            {
                public static bool IsNull<T>(T value) => value == null;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2955").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2970", "Assertions should be complete",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2970/")]
    public async Task DetectIncompleteFluentAssertion()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "FluentAssertions", Version: "8.10.0")]);
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2970").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2996", "ThreadStatic fields should not be initialized",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2996/")]
    public async Task WarnOnThreadStaticFieldWithInitializer()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                [System.ThreadStatic]
                private static int _counter = 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2996").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2997", "IDisposables created in a 'using' statement should not be returned",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2997/")]
    public async Task DetectDisposableReturnedFromUsingStatement()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2997").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3005", "ThreadStatic should not be used on non-static fields",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3005/")]
    public async Task WarnOnThreadStaticOnInstanceField()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Example
            {
                [System.ThreadStatic]
                public int NonStaticThreadStaticField;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3010", "Static fields should not be updated in constructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3010/")]
    public async Task WarnOnStaticFieldAssignmentInInstanceConstructor()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3010").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3168", "async methods should not return void",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3168/")]
    public async Task WarnOnAsyncVoidMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyClass
            {
                public async void DoWork() { await System.Threading.Tasks.Task.Delay(1); }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3168").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3172", "Delegates should not be subtracted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3172/")]
    public async Task DetectDelegateSubtraction()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3172").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3234", "GC.SuppressFinalize should not be invoked for types without destructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3234/")]
    public async Task WarnOnSuppressFinalizeWithoutDestructor()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3234").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3237", "\"value\" contextual keyword should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3237/")]
    public async Task WarnWhenSetterIgnoresValueKeyword()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3237").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3249", "Classes directly extending \"object\" should not call \"base\" in \"GetHashCode\" or \"Equals\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3249/")]
    public async Task WarnOnBaseCallInGetHashCodeOrEqualsForDirectObjectSubclass()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3249").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3263", "Static fields should appear in the order they must be initialized",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3263/")]
    public async Task WarnOnStaticFieldInitializerOutOfOrder()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                static int A = B + 1;
                static int B = 10;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3263").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3264", "Events should be invoked",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3264/")]
    public async Task WarnOnEventNeverInvoked()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3264").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3265", "Non-flags enums should not be used in bitwise operations",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3265/")]
    public async Task WarnOnBitwiseOperationsOnNonFlagsEnum()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public enum Direction { North, South, East, West }

            public static class Checker
            {
                public static Direction Combine(Direction a, Direction b) => a | b;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3265").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3346", "Expressions used in \"Debug.Assert\" should not produce side effects",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3346/")]
    public async Task WarnOnSideEffectsInDebugAssert()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3346").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3363", "Date and time should not be used as a type for primary keys",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3363/")]
    public async Task WarnOnDateTimeUsedAsPrimaryKey()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace Microsoft.EntityFrameworkCore
            {
                public class DbContext { }
            }
            namespace test
            {
                public class Order
                {
                    public System.DateTime OrderId { get; set; }
                }
                public static class Program { public static int Main() => 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3363").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3366", "this should not be exposed from constructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3366/")]
    public async Task WarnOnThisExposedFromConstructor()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Registry
            {
                private object? _instance;
                public void Register(object obj) { _instance = obj; }
            }

            public class MyClass
            {
                public MyClass(Registry registry)
                {
                    registry.Register(this); // S3366: 'this' exposed before construction completes
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3366").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3397", "base.Equals should not be used to check for reference equality in Equals if base is not object",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3397/")]
    public async Task WarnOnBaseEqualsUsedForReferenceEqualityInDerivedEquals()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Base
            {
                private readonly int _value;
                public Base(int value) => _value = value;
                public override bool Equals(object? obj) => obj is Base other && _value == other._value;
                public override int GetHashCode() => _value.GetHashCode();
            }

            public class Derived : Base
            {
                private readonly int _extra;
                public Derived(int value, int extra) : base(value) { _extra = extra; }

                public override bool Equals(object? obj)
                {
                    // S3397: base.Equals is used as a guard condition, but Base.Equals does structural
                    // equality — not reference equality as the developer likely intended.
                    if (base.Equals(obj)) return true;
                    if (obj is not Derived other) return false;
                    return _extra == other._extra;
                }

                public override int GetHashCode() => base.GetHashCode();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3397").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3415", "Assertion arguments should be passed in the correct order",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3415/")]
    public async Task WarnOnSwappedAssertionArguments()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "NUnit", Version: "3.14.0")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void M(int value)
                {
                    NUnit.Framework.Assert.AreEqual(value, 42);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3415").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3447", "[Optional] should not be used on ref or out parameters",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3447/")]
    public async Task ProhibitOptionalOnRefOrOutParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Runtime.InteropServices;

            namespace test;

            public static class Problematic
            {
                public static void Fill([Optional] ref int value)
                {
                    value = 42;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3447").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3449", "Right operands of shift operators should be integers",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3449/")]
    public async Task WarnOnShiftWithNonIntegerRightOperand()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method()
                {
                    dynamic d = 42;
                    var x = d >> 5.4;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3449").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3466", "Optional parameters should be passed to \"base\" calls",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3466/")]
    public async Task WarnOnOptionalParameterNotPassedToBaseCall()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Base
            {
                public virtual void Method(int required, string optional = "default")
                {
                }
            }

            public class Derived : Base
            {
                public override void Method(int required, string optional = "default")
                {
                    base.Method(required); // S3466: optional not forwarded to base
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3466").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3598", "One-way \"OperationContract\" methods should have \"void\" return type",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3598/")]
    public async Task WarnOnNonVoidOneWayOperationContract()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.ServiceModel.Primitives", Version: "10.0.652802")]);
        await project.AddFileAsync("Program.cs", """
            using System.ServiceModel;

            namespace test;

            [ServiceContract]
            public interface IMyService
            {
                [OperationContract(IsOneWay = true)]
                int FireAndForgetButReturnsInt(); // Noncompliant: one-way operation cannot return a value
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3598").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3603", "Methods with \"Pure\" attribute should return a value",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3603/")]
    public async Task DetectPureAttributeOnVoidMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics.Contracts;
            namespace test;
            public class C
            {
                [Pure]
                public static void DoNothing() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3603").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3610", "Nullable type comparison should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3610/")]
    public async Task DetectRedundantNullableTypeComparison()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Checker
            {
                public static void DoChecks<T>(System.Nullable<T> value) where T : struct
                {
                    var areEqual = value.GetType() == typeof(System.Nullable<int>);
                    _ = areEqual;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3610").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3869", "SafeHandle.DangerousGetHandle should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3869/")]
    public async Task WarnOnDangerousGetHandleCall()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Runtime.InteropServices;
            namespace test;
            public sealed class MyHandle : SafeHandle
            {
                public MyHandle() : base(System.IntPtr.Zero, true) { }
                public override bool IsInvalid => handle == System.IntPtr.Zero;
                protected override bool ReleaseHandle() { return true; }
            }
            public static class Usage
            {
                public static System.IntPtr GetRaw(MyHandle h) => h.DangerousGetHandle();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3869").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3889", "Thread.Resume and Thread.Suspend should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3889/")]
    public async Task ProhibitThreadSuspendAndResume()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Threading;
            namespace test;
            public static class Example
            {
                public static void Run()
                {
                    var t = new Thread(() => { });
                    t.Start();
            #pragma warning disable CS0618
                    t.Suspend();
            #pragma warning restore CS0618
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3889").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3923", "All branches in a conditional structure should not have exactly the same implementation",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3923/")]
    public async Task DetectIdenticalImplementationInAllBranches()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Checker
            {
                public static string Classify(int n)
                {
                    if (n > 0)
                    {
                        return "positive";
                    }
                    else if (n < 0)
                    {
                        return "positive";
                    }
                    else
                    {
                        return "positive";
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3923").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3926", "Deserialization methods should be provided for OptionalField members",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3926/")]
    public async Task WarnOnOptionalFieldWithoutDeserializationCallback()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System;
            using System.Runtime.Serialization;

            namespace test;

            [Serializable]
            public class MyData
            {
                public string Name { get; set; } = string.Empty;

                [OptionalField]
                public string? Description;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3926").ShouldBeTrue();
    }
}
