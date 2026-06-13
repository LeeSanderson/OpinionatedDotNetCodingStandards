// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesNamingShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S100", "Methods and properties should be named in PascalCase",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-100/")]
    public async Task WarnOnNonPascalCaseMethodName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyClass
            {
                public static int doSomething() => 0;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S100").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S101", "Types should be named in PascalCase",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-101/")]
    public async Task WarnOnNonPascalCaseTypeName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class myClass { public int Value { get; set; } }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S101").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1117", "Local variables should not shadow class fields or properties",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1117/")]
    public async Task ProhibitLocalVariableShadowingClassField()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyClass
            {
                private int value = 0;
                public int Method()
                {
                    int value = 42;
                    return value;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1117").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2166", "Classes named like \"Exception\" should extend \"Exception\" or a subclass",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2166/")]
    public async Task WarnOnExceptionNamedClassNotExtendingException()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyException
            {
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2166").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2306", "“async” and “await” should not be used as identifiers",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2306/")]
    public async Task ProhibitAsyncAwaitAsIdentifiers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public static class C
            {
                public static void Method(int async, int await)
                {
                    _ = async + await;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2306").ShouldBeTrue();
    }
}
