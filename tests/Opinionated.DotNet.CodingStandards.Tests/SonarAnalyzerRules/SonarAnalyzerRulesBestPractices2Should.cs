// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBestPractices2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S1940", "Boolean checks should not be inverted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1940/")]
    public async Task ProhibitInvertedBooleanCheck()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static bool IsNotEqual(int a, int b) => !(a == b);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1940").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2148", "Underscores should be used to make large numbers readable",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2148/")]
    public async Task WarnOnUnderscorelessLargeNumbers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Constants
            {
                public const int MaxUsers = 1000000;
                public const long MaxBytes = 10000000000;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2148").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2187", "Test classes should contain at least one test case",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2187/")]
    public async Task WarnOnEmptyTestClass()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "4.2.3")]);
        await project.AddFile("Program.cs", """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            namespace test;
            [TestClass]
            public class EmptyTestSuite { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2187").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2219", "Runtime type checking should be simplified",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2219/")]
    public async Task WarnOnVerboseRuntimeTypeChecking()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2219").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2302", "nameof should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2302/")]
    public async Task WarnOnHardcodedNameofString()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2302").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2327", "“try” statements with identical “catch” and/or “finally” blocks should be merged",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2327/")]
    public async Task WarnOnDuplicateTryCatchBlocks()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2327").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2333", "Redundant modifiers should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2333/")]
    public async Task WarnOnRedundantModifiers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2333").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2365", "Properties should not make collection or array copies",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2365/")]
    public async Task WarnOnPropertyMakingCollectionCopy()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public sealed class MyCollection
            {
                private readonly System.Collections.Generic.List<int> _items = [1, 2, 3];

                public System.Collections.Generic.List<int> Items => _items.ToList();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2365").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2479", "Whitespace and control characters in string literals should be explicit",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2479/")]
    public async Task WarnOnRawWhitespaceInStringLiteral()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class Greeter
            {
                public static string GetMessage() => "Hello	World";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2479").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2699", "Tests should include assertions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2699/")]
    public async Task WarnOnTestMethodWithNoAssertions()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "xunit", Version: "2.9.3")]);
        await project.AddFile("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2699").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2701", "Literal boolean values should not be used in assertions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2701/")]
    public async Task ProhibitLiteralBooleanInAssertions()
    {
        using var project = await CreateProjectBuilder(
            packageReferences: [(Name: "MSTest.TestFramework", Version: "4.2.3")]);
        await project.AddFile("Program.cs", """
            namespace test;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            public static class MyTests
            {
                public static void Run() => Assert.IsTrue(true);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2701").ShouldBeTrue();
    }
}
