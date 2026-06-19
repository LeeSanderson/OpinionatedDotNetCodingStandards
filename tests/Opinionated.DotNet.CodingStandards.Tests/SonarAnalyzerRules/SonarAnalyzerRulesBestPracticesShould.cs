using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBestPracticesShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1006", "Method overrides should not change parameter defaults",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1006/")]
    public async Task WarnOnOverrideChangingParameterDefault()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Base
            {
                public virtual void DoWork(int value = 0) { }
            }
            public class Derived : Base
            {
                public override void DoWork(int value = 42) { }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1006").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S106", "Standard outputs should not be used directly to log anything",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-106/")]
    public async Task ProhibitDirectStandardOutputLogging()
    {
        using var project = await CreateProjectBuilderAsync(properties: [("OutputType", "Library")]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void Log(string message)
                {
                    Console.WriteLine(message);
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S106").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1066", "Mergeable \"if\" statements should be combined",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1066/")]
    public async Task WarnOnMergeableIfStatements()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                public static void Method(int a, int b)
                {
                    if (a > 0)
                    {
                        if (b > 0)
                        {
                            System.Console.WriteLine(a + b);
                        }
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1066").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1075", "URIs should not be hardcoded",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1075/")]
    public async Task ProhibitHardcodedUris()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    string url = "https://www.example.com/api/v1/data";
                    return url.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1075").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S108", "Nested blocks of code should not be left empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-108/")]
    public async Task ProhibitEmptyNestedBlock()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method(int[] items)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S108").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S109", "Magic numbers should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-109/")]
    public async Task WarnOnMagicNumbers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Calculator
            {
                public static int Compute(int a) => a * 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S109").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1110", "Redundant pairs of parentheses should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1110/")]
    public async Task ProhibitRedundantParentheses()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Calculator
            {
                public static int Add(int a, int b) => ((a + b));
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1110").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1116", "Empty statements should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1116/")]
    public async Task ProhibitEmptyStatements()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                public static void Method()
                {
                    ; // empty statement — S1116
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1116").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1121", "Assignments should not be made from within sub-expressions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1121/")]
    public async Task ProhibitAssignmentInSubExpression()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int GetValue() => 0;

                public static void Use(int v) { _ = v; }

                public static int Main()
                {
                    int x = 0;
                    Use(x = GetValue());
                    return x;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1121").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1125", "Boolean literals should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1125/")]
    public async Task ProhibitRedundantBooleanLiterals()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Guard
            {
                public static bool IsPositive(int value) => value > 0 == true;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1125").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1128", "Unnecessary \"using\" should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1128/")]
    public async Task WarnOnUnnecessaryUsingDirective()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Text;

            namespace test;

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1128").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1133", "Deprecated code should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1133/")]
    public async Task WarnOnObsoleteDeclaration()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class MyService
            {
                [System.Obsolete("Use NewMethod instead")]
                public static void OldMethod() { }

                public static void NewMethod() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1133").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1134", "Track uses of \"FIXME\" tags",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1134/")]
    public async Task WarnOnFixmeTag()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Calculator
            {
                // FIXME: this method is broken and needs to be reimplemented
                public static int Add(int a, int b) => a + b;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1134").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1135", "Track uses of \"TODO\" tags",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1135/")]
    public async Task WarnOnTodoComments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Greeter
            {
                // TODO: implement proper greeting logic
                public static string Greet(string name) => name;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1135").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1144", "Unused private types or members should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1144/")]
    public async Task WarnOnUnusedPrivateMembers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Worker
            {
                public int Run() => 42;

                private int UnusedHelper() => 0;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1144").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1147", "Exit methods should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1147/")]
    public async Task ProhibitExitMethodCalls()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Exiter
            {
                public static void Quit() => System.Environment.Exit(0);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1147").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1168", "Empty arrays and collections should be returned instead of null",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1168/")]
    public async Task ProhibitReturningNullCollections()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            namespace test;
            public static class Collector
            {
                public static IEnumerable<int> GetItems(bool flag)
                {
                    if (flag)
                    {
                        return null;
                    }
                    return new List<int> { 1, 2, 3 };
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1168").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1172", "Unused method parameters should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1172/")]
    public async Task WarnOnUnusedMethodParameters()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Calculator
            {
                public int Add(int a, int b) => AddImpl(a, b, 0);

                private int AddImpl(int a, int b, int unused)
                {
                    return a + b;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1172").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1186", "Methods should not be empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1186/")]
    public async Task ProhibitEmptyMethods()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyService
            {
                public void DoWork() { }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1186").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1192", "String literals should not be duplicated",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1192/")]
    public async Task WarnOnDuplicatedStringLiterals()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Greeter
            {
                public static void A() => System.Console.WriteLine("hello-world");
                public static void B() => System.Console.WriteLine("hello-world");
                public static void C() => System.Console.WriteLine("hello-world");
                public static void D() => System.Console.WriteLine("hello-world");
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1192").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1199", "Nested code blocks should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1199/")]
    public async Task ProhibitNestedCodeBlocks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method()
                {
                    {
                        _ = 0;
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1199").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S121", "Control structures should use curly braces",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-121/")]
    public async Task ProhibitControlStructuresWithoutCurlyBraces()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Check(int n)
                {
                    if (n > 0)
                        System.Console.WriteLine(n);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S121").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1215", "“GC.Collect” should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1215/")]
    public async Task ProhibitExplicitGarbageCollection()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Cleaner
            {
                public void ForceCollect()
                {
                    GC.Collect();
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1215").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1226", "Method parameters, caught exceptions and foreach variables' initial values should not be ignored",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1226/")]
    public async Task WarnOnParameterInitialValueIgnored()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Calculator
            {
                public static int Add(int a, int b)
                {
                    a = 0;
                    return a + b;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1226").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1227", "break statements should not be used except for switch cases",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1227/")]
    public async Task ProhibitBreakInLoops()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static int FindFirst(int[] items, int target)
                {
                    int index = -1;
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == target)
                        {
                            index = i;
                            break;
                        }
                    }
                    return index;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1227").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S125", "Sections of code should not be commented out",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-125/")]
    public async Task ProhibitCommentedOutCode()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Calculator
            {
                public static int Add(int a, int b)
                {
                    // int result = a - b;
                    // if (result < 0)
                    // {
                    //     result = 0;
                    // }
                    // return result;
                    return a + b;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S125").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S126", "“if ... else if” constructs should end with “else” clauses",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-126/")]
    public async Task WarnOnIfElseIfWithoutFinalElse()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                public static string Classify(int n)
                {
                    if (n < 0)
                    {
                        return "negative";
                    }
                    else if (n == 0)
                    {
                        return "zero";
                    }
                    else if (n < 10)
                    {
                        return "small";
                    }
                    return "large";
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S126").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1264", "A \"while\" loop should be used instead of a \"for\" loop",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1264/")]
    public async Task WarnOnForLoopUsedAsWhileLoop()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class LoopExample
            {
                public static int CountDown(int start)
                {
                    int n = start;
                    for (; n > 0; )
                    {
                        n--;
                    }
                    return n;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1264").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1301", "\"switch\" statements should have at least 3 \"case\" clauses",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1301/")]
    public async Task WarnOnSwitchWithFewerThanThreeCases()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public static string Describe(int x)
                {
                    switch (x)
                    {
                        case 0:
                            return "zero";
                        default:
                            return "other";
                    }
                }

                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1301").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1309", "Track uses of in-source issue suppressions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1309/")]
    public async Task WarnOnInSourceIssueSuppressions()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            #pragma warning disable CS0168
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1309").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S131", "“switch/Select” statements should contain a “default/Case Else” clause",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-131/")]
    public async Task WarnOnSwitchStatementMissingDefault()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public enum Direction { North, South, East, West }
            public static class Checker
            {
                public static string Describe(Direction d)
                {
                    switch (d)
                    {
                        case Direction.North: return "up";
                        case Direction.South: return "down";
                        case Direction.East:  return "right";
                        case Direction.West:  return "left";
                    }
                    return "unknown";
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S131").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1312", "Logger fields should be 'private static readonly'",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1312/")]
    public async Task WarnOnNonPrivateStaticReadonlyLoggerField()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class MyService
            {
                public ILogger Logger;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1312").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1481", "Unused local variables should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1481/")]
    public async Task ProhibitUnusedLocalVariables()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Calculator
            {
                public static int Add(int a, int b)
                {
                    int unused = a * b;
                    return a + b;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1481").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1607", "Tests should not be ignored",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1607/")]
    public async Task WarnOnIgnoredTestWithoutReason()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "4.2.3")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            namespace test;
            [TestClass]
            public sealed class MyTests
            {
                [TestMethod]
                [Ignore]
                public void IgnoredTest() { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1607").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1643", "Strings should not be concatenated using '+' in a loop",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1643/")]
    public async Task ProhibitStringConcatenationInLoop()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class StringHelper
            {
                public static string Join(string[] items)
                {
                    string result = string.Empty;
                    foreach (var item in items)
                    {
                        result += item;
                    }
                    return result;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1643").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1659", "Multiple variables should not be declared on the same line",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1659/")]
    public async Task ProhibitMultipleVariableDeclarationsOnSameLine()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Method()
                {
                    int a = 1, b = 2, c = 3;
                    _ = a + b + c;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1659").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1871", "Two branches in a conditional structure should not have exactly the same implementation",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1871/")]
    public async Task WarnOnDuplicateConditionalBranchImplementation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Grader
            {
                public static string Classify(string category)
                {
                    if (category == "A")
                    {
                        Console.WriteLine("grade A");
                        Console.WriteLine("excellent");
                        return "pass";
                    }
                    else if (category == "B")
                    {
                        Console.WriteLine("grade B");
                        Console.WriteLine("good");
                        return "pass";
                    }
                    else if (category == "C")
                    {
                        Console.WriteLine("grade A");
                        Console.WriteLine("excellent");
                        return "pass";
                    }
                    else
                    {
                        return "fail";
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1871").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1905", "Redundant casts should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1905/")]
    public async Task WarnOnRedundantCast()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static string GetValue()
                {
                    string s = "hello";
                    return (string)s; // S1905: cast to string is redundant, s is already string
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S1905").ShouldBeTrue();
    }
}
