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
}
