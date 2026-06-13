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
}
