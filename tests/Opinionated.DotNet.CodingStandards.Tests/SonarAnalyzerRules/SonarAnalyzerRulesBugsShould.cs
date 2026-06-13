// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBugsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1206", "'Equals(Object)' and 'GetHashCode()' should be overridden in pairs",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1206/")]
    public async Task DetectEqualsWithoutGetHashCode()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class MyValue
            {
                private readonly int _x;
                public MyValue(int x) { _x = x; }

                public override bool Equals(object? obj)
                {
                    if (obj is MyValue other)
                        return _x == other._x;
                    return false;
                }
                // GetHashCode intentionally NOT overridden — violates S1206
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1206").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1244", "Floating point numbers should not be tested for equality",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1244/")]
    public async Task WarnOnFloatingPointEqualityComparison()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class FloatComparison
            {
                public static bool AreEqual(double a, double b) => a == b;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1244").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S127", "“for” loop stop conditions should be invariant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-127/")]
    public async Task ProhibitMutatingForLoopStopConditionVariable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var total = 0;
                    for (var i = 0; i < 10; i++)
                    {
                        total += i;
                        i++; // modifies the loop counter variable — S127: stop condition is no longer invariant
                    }

                    return total;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S127").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1656", "Variables should not be self-assigned",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1656/")]
    public async Task DetectSelfAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                private int _value;
                public void SetValue(int value)
                {
                    _value = value;
                    _value = _value;
                }
                public int GetValue() => _value;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1656").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1696", "NullReferenceException should not be caught",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1696/")]
    public async Task ProhibitCatchingNullReferenceException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class NullRefCatcher
            {
                public static void Run()
                {
                    try
                    {
                        _ = ((string)null!).Length;
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1696").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1698", "\"==\" should not be used when \"Equals\" is overridden",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1698/")]
    public async Task WarnOnEqualityOperatorWhenEqualsIsOverridden()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Money
            {
                private readonly int _amount;
                public Money(int amount) { _amount = amount; }
                public override bool Equals(object? obj) => obj is Money m && m._amount == _amount;
                public override int GetHashCode() => _amount.GetHashCode();
            }
            public class Checker
            {
                public static bool AreEqual(Money a, Money b) => a == b;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1698").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1751", "Loops with at most one iteration should be refactored",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1751/")]
    public async Task WarnOnLoopWithAtMostOneIteration()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Example
            {
                public static int First(int[] arr)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        return arr[i]; // unconditional return — loop runs at most once
                    }
                    return -1;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1751").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1764", "Identical expressions should not be used on both sides of operators",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1764/")]
    public async Task DetectIdenticalExpressionsOnBothSidesOfOperator()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Checker
            {
                public static bool IsRedundant(int x)
                {
                    return x == x;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1764").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1848", "Objects should not be created to be dropped immediately without being used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1848/")]
    public async Task ProhibitObjectsCreatedAndDroppedImmediately()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Box
            {
                public int Value { get; }
                public Box(int value) { Value = value; }
            }
            public class Example
            {
                public void Run()
                {
                    new Box(42);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1848").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1854", "Unused assignments should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1854/")]
    public async Task DetectDeadStoreAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Calculator
            {
                public static int Compute(int a, int b)
                {
                    int result = a * 2;   // dead store — overwritten before being read
                    result = a + b;
                    return result;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1854").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1862", "Related 'if/else if' statements should not have the same condition",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1862/")]
    public async Task DetectDuplicateConditionInIfElseIfChain()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Checker
            {
                public static string Classify(int x)
                {
                    if (x > 0)
                    {
                        return "positive";
                    }
                    else if (x < 0)
                    {
                        return "negative";
                    }
                    else if (x > 0)
                    {
                        return "duplicate";
                    }
                    else
                    {
                        return "zero";
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1862").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1944", "Invalid casts should be avoided",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1944/")]
    public async Task WarnOnInvalidCastToInterface()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public interface IFoo { }

            public class FooImpl : IFoo { }

            public class Unrelated { }

            public static class Usage
            {
                public static IFoo Cast(Unrelated u) => (IFoo)u;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1944").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1994", "“for” loop increment clauses should modify the loops’ counters",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1994/")]
    public async Task WarnOnForLoopIncrementNotModifyingCounter()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class C
            {
                public static void Method()
                {
                    int j = 0;
                    for (int i = 0; i < 10; j++)
                    {
                        i++;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1994").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2114", "Collections should not be passed as arguments to their own methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2114/")]
    public async Task WarnOnCollectionPassedToOwnMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var set = new System.Collections.Generic.HashSet<int> { 1, 2, 3 };
                    set.IntersectWith(set);
                    return 0;
                }
            }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2114").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2123", "Values should not be uselessly incremented",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2123/")]
    public async Task DetectUselessPostfixIncrement()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int GetValue(int x)
                {
                    return x++;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2123").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2139", "Exceptions should be either logged or rethrown but not both",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2139/")]
    public async Task DetectLogAndRethrowInSameCatch()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFile("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class Service
            {
                public static void Process(ILogger logger, string data)
                {
                    try
                    {
                        _ = int.Parse(data);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failure");
                        throw;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2139").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2178", "Short-circuit logic should be used in boolean contexts",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2178/")]
    public async Task WarnOnNonShortCircuitBooleanOperators()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static bool IsValid(string s)
                {
                    // S2178: uses & (non-short-circuit) between two boolean expressions;
                    // should use && to avoid evaluating the right-hand side unnecessarily
                    return s != null & s.Length > 0;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2178").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2183", "Integral numbers should not be shifted by zero or more than their number of bits-1",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2183/")]
    public async Task DetectInvalidBitShiftAmount()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int ShiftTooFar(int x)
                {
                    return x << 32;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2183").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2184", "Results of integer division should not be assigned to floating point variables",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2184/")]
    public async Task DetectIntegerDivisionAssignedToFloatingPoint()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Calculator
            {
                public static double GetRatio(int numerator, int denominator)
                {
                    double ratio = numerator / denominator;
                    return ratio;
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2184").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2197", "Modulus results should not be checked for direct equality",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2197/")]
    public async Task WarnOnModulusEqualityCheck()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Calculator
            {
                public static bool IsOdd(int n) => n % 2 == 1;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2197").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2198", "Unnecessary mathematical comparisons should not be made",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2198/")]
    public async Task DetectUnnecessaryMathematicalComparison()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static bool Compare(char c)
                {
                    const int veryBig = int.MaxValue;
                    return c < veryBig;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2198").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2221", "“Exception” should not be caught",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2221/")]
    public async Task ProhibitCatchingBaseException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Thrower
            {
                public static void Do()
                {
                    try
                    {
                        _ = int.Parse("x");
                    }
                    catch (Exception)
                    {
                        // swallows all exceptions — violates S2221
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2221").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2225", "ToString() method should not return null",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2225/")]
    public async Task WarnWhenToStringReturnsNull()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyType
            {
                public override string? ToString() => null;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2225").ShouldBeTrue();
    }
}
