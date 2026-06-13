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
}
