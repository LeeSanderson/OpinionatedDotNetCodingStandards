using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

public class SonarAnalyzerRulesBestPractices2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1940", "Boolean checks should not be inverted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1940/")]
    public async Task ProhibitInvertedBooleanCheck()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static bool IsNotEqual(int a, int b) => !(a == b);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1940").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2148", "Underscores should be used to make large numbers readable",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2148/")]
    public async Task WarnOnUnderscorelessLargeNumbers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Constants
            {
                public const int MaxUsers = 1000000;
                public const long MaxBytes = 10000000000;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2148").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2187", "Test classes should contain at least one test case",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2187/")]
    public async Task WarnOnEmptyTestClass()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "4.2.3")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            namespace test;
            [TestClass]
            public class EmptyTestSuite { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2187").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2219", "Runtime type checking should be simplified",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2219/")]
    public async Task WarnOnVerboseRuntimeTypeChecking()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Base { }
            public class Derived : Base { }
            public static class Checker
            {
                public static bool Check(object obj)
                {
                    return typeof(Derived).IsAssignableFrom(obj.GetType());
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2219").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2302", "nameof should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2302/")]
    public async Task WarnOnHardcodedNameofString()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Validate(string value)
                {
                    if (value == null)
                        throw new System.ArgumentNullException("value");
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2302").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2327", "“try” statements with identical “catch” and/or “finally” blocks should be merged",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2327/")]
    public async Task WarnOnDuplicateTryCatchBlocks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method(int x)
                {
                    try
                    {
                        _ = x + 1;
                    }
                    catch (System.InvalidOperationException)
                    {
                        // handle
                    }

                    try
                    {
                        _ = x + 2;
                    }
                    catch (System.InvalidOperationException)
                    {
                        // handle
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2327").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2333", "Redundant modifiers should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2333/")]
    public async Task WarnOnRedundantModifiers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Base
            {
                public virtual void Method() { }
            }

            public sealed class Derived : Base
            {
                public sealed override void Method() { }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2333").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2365", "Properties should not make collection or array copies",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2365/")]
    public async Task WarnOnPropertyMakingCollectionCopy()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public sealed class MyCollection
            {
                private readonly System.Collections.Generic.List<int> _items = [1, 2, 3];

                public System.Collections.Generic.List<int> Items => _items.ToList();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2365").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2479", "Whitespace and control characters in string literals should be explicit",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2479/")]
    public async Task WarnOnRawWhitespaceInStringLiteral()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Greeter
            {
                public static string GetMessage() => "Hello	World";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2479").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2699", "Tests should include assertions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2699/")]
    public async Task WarnOnTestMethodWithNoAssertions()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "xunit", Version: "2.9.3")]);
        await project.AddFileAsync("Program.cs", """
            using Xunit;
            namespace test;
            public sealed class SomeTests
            {
                [Fact]
                public void DoesNothing()
                {
                    var x = 1 + 1;
                }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2699").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2701", "Literal boolean values should not be used in assertions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2701/")]
    public async Task ProhibitLiteralBooleanInAssertions()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "4.2.3")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            public static class MyTests
            {
                public static void Run() => Assert.IsTrue(true);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2701").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2925", "Thread.Sleep should not be used in tests",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2925/")]
    public async Task WarnOnThreadSleepInTests()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "xunit", Version: "2.9.3")]);
        await project.AddFileAsync("Program.cs", """
            using Xunit;
            namespace test;
            public class MyTests
            {
                [Fact]
                public void SlowTest()
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2925").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2933", "Fields that are only assigned in the constructor should be 'readonly'",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2933/")]
    public async Task WarnOnNonReadonlyConstructorOnlyFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                private int _value;
                public C(int value) { _value = value; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2933").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2971", "LINQ expressions should be simplified",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2971/")]
    public async Task WarnOnUnsimplifiedLinqExpressions()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            internal static class LinqSimplification
            {
                internal static bool HasNulls(List<string> items) =>
                    items.Where(x => x == null).Any();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S2971").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3052", "Members should not be initialized to default values",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3052/")]
    public async Task ProhibitDefaultValueInitialization()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                private int _count = 0;
                private bool _active = false;
                private string? _name = null;
                public int GetCount() => _count;
                public bool IsActive() => _active;
                public string? GetName() => _name;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3060", "\"is\" should not be used with \"this\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3060/")]
    public async Task WarnOnIsCheckWithThis()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Base
            {
                public string Describe()
                {
                    if (this is Derived)
                    {
                        return "derived";
                    }
                    return "base";
                }
            }
            public class Derived : Base { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3060").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3063", "“StringBuilder” data should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3063/")]
    public async Task WarnOnUnusedStringBuilderData()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Text;

            namespace test;

            public static class Example
            {
                public static void Build()
                {
                    var sb = new StringBuilder();
                    sb.Append("hello");
                    sb.Append(" world");
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3063").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3217", "Explicit conversions of foreach loops should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3217/")]
    public async Task WarnOnExplicitForeachConversion()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            namespace test;
            public static class C
            {
                public static void Method()
                {
                    var list = new List<long> { 1L, 2L, 3L };
                    foreach (int item in list)
                    {
                        _ = item + 1;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3217").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3220", "Method calls should not resolve ambiguously to overloads with \"params\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3220/")]
    public async Task WarnOnAmbiguousParamsOverloadResolution()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Helper
            {
                public static void Log(int level, params object[] messages) { _ = level; _ = messages; }
                public static void Log(double level, object message) { _ = level; _ = message; }
            }

            public static class Caller
            {
                public static void Run()
                {
                    Helper.Log(1, null);
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3220").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3235", "Redundant parentheses should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3235/")]
    public async Task WarnOnRedundantParentheses()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyClass
            {
                public int Value { get; set; }
            }
            public static class Program
            {
                public static int Main()
                {
                    var obj = new MyClass() { Value = 1 };
                    return obj.Value;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3235").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3236", "Caller information arguments should not be provided explicitly",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3236/")]
    public async Task ProhibitExplicitCallerInfoArguments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Runtime.CompilerServices;

            namespace test;

            public static class Logger
            {
                public static void Trace(
                    string message,
                    [CallerFilePath] string file = "",
                    [CallerLineNumber] int line = 0)
                {
                    _ = message; _ = file; _ = line;
                }
            }

            public static class Usage
            {
                public static void Run()
                {
                    Logger.Trace("hello", "MyFile.cs", 42); // S3236: explicit args for CallerFilePath and CallerLineNumber
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3236").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3240", "The simplest possible condition syntax should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3240/")]
    public async Task WarnOnUnnecessarilyVerboseConditionSyntax()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static object Simplify(object a, object b)
                {
                    if (a == null) { a = b; }
                    return a;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3240").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3241", "Methods should not return values that are never used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3241/")]
    public async Task WarnOnMethodReturnValueNeverUsed()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Program
            {
                private static int Compute(int x, int y) => x + y;
                public static int Main()
                {
                    Compute(1, 2);
                    return 0;
                }
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3241").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3247", "Duplicate casts should not be made",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3247/")]
    public async Task WarnOnDuplicateCast()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Base { }
            public class Derived : Base { public int Value => 42; }

            public class Checker
            {
                public static int Check(Base b)
                {
                    if (b is Derived)
                    {
                        var d = (Derived)b; // S3247: duplicate cast — 'is' already performed the cast
                        return d.Value;
                    }
                    return 0;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3247").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3251", "Implementations should be provided for \"partial\" methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3251/")]
    public async Task WarnOnPartialMethodsWithoutImplementation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public partial class C
            {
                partial void DoSomething();

                public void Run()
                {
                    DoSomething();
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3251").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3253", "Constructor and destructor declarations should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3253/")]
    public async Task WarnOnRedundantConstructorDeclaration()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class MyService
            {
                public MyService() { }

                public int GetValue() => 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3253").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3256", "string.IsNullOrEmpty should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3256/")]
    public async Task WarnOnManualNullOrEmptyStringCheck()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Program
            {
                public static bool IsEmpty(string s)
                {
                    return s.Equals(string.Empty);
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3256").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3257", "Declarations and initializations should be as concise as possible",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3257/")]
    public async Task WarnOnRedundantDeclarations()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class C
            {
                public static int[] GetNumbers() => new int[] { 1, 2, 3 };
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3257").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3261", "Namespaces should not be empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3261/")]
    public async Task ProhibitEmptyNamespace()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test
            {
                public static class Program { public static int Main() => 0; }
            }
            namespace empty { }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3261").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3262", "\"params\" should be used on overrides",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3262/")]
    public async Task WarnWhenOverrideDropsParamsModifier()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public abstract class Base
            {
                public abstract void Method(params int[] values);
            }
            public class Derived : Base
            {
                public override void Method(int[] values) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3262").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3267", "Loops should be simplified with \"LINQ\" expressions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3267/")]
    public async Task WarnOnLoopsThatShouldUseLinq()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;

            namespace test;

            public static class Example
            {
                public static List<int> FilterPositive(IEnumerable<int> source)
                {
                    var result = new List<int>();
                    foreach (var item in source)
                    {
                        if (item > 0)
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3267").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3343", "Caller information parameters should come at the end of the parameter list",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3343/")]
    public async Task WarnOnCallerInfoParameterNotAtEnd()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Runtime.CompilerServices;

            namespace test;

            public static class Logger
            {
                public static void Log(
                    [CallerMemberName] string memberName = "",
                    string message = "")
                {
                    _ = memberName;
                    _ = message;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3343").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3353", "Unchanged variables should be marked as \"const\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3353/")]
    public async Task WarnOnUnchangedVariableNotMarkedConst()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Example
            {
                public static string Describe()
                {
                    string greeting = "Hello, World!";
                    return greeting;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3353").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3400", "Methods should not return constants",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3400/")]
    public async Task WarnOnMethodReturningConstant()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Calculator
            {
                public string GetVersion() => "1.0.0";
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3400").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3431", "[ExpectedException] attribute should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3431/")]
    public async Task ProhibitExpectedExceptionAttribute()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "2.2.10")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public class MyTests
            {
                [TestMethod]
                [ExpectedException(typeof(System.InvalidOperationException))]
                public void ShouldThrow()
                {
                    var x = 0;
                    _ = 1 / x;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3431").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3433", "Test method signatures should be correct",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3433/")]
    public async Task WarnOnIncorrectTestMethodSignature()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "NUnit", Version: "3.14.0")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using NUnit.Framework;
            public class SomeTests
            {
                [Test]
                private void PrivateTestMethod() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3433").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3440", "Variables should not be checked against the values they're about to be assigned",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3440/")]
    public async Task WarnOnRedundantConditionalAroundAssignment()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                private int _value;
                public void SetValue(int v)
                {
                    if (_value != v)
                    {
                        _value = v;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3440").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3441", "Redundant property names should be omitted in anonymous classes",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3441/")]
    public async Task WarnOnRedundantPropertyNamesInAnonymousClasses()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public object Project(string name, int age)
                {
                    // S3441: property name 'name' and 'age' are redundant — C# allows: new { name, age }
                    return new { name = name, age = age };
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3441").ShouldBeTrue();
    }
}
