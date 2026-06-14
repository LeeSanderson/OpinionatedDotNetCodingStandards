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

    [Fact]
    [RuleDoc("S2342", "Enumeration types should comply with a naming convention",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2342/")]
    public async Task WarnOnNonPascalCaseEnumeration()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public enum my_status_code
            {
                active,
                inactive,
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2342").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2344", "Enumeration type names should not have \"Flags\" or \"Enum\" suffixes",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2344/")]
    public async Task ProhibitFlagsOrEnumSuffixOnEnumerationTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public enum StatusEnum { Active, Inactive }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2344").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2346", "Flags enumerations zero-value members should be named \"None\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2346/")]
    public async Task WarnOnFlagsEnumZeroValueMemberNotNamedNone()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            [System.Flags]
            public enum Permissions
            {
                Everything = 0,
                Read = 1,
                Write = 2,
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2346").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3376", "Attribute, EventArgs, and Exception type names should end with the type being extended",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3376/")]
    public async Task WarnOnMissingTypeSuffix()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class BadThing : System.Exception { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3376").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3872", "Parameter names should not duplicate the names of their methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3872/")]
    public async Task WarnOnParameterNameDuplicatingMethodName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int Calculate(int calculate) => calculate;
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3872").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4016", "Enumeration members should not be named \"Reserved\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4016/")]
    public async Task WarnOnReservedEnumMemberName()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public enum Status
            {
                Active = 0,
                Reserved = 1,
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S4016").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4025", "Child class fields should not differ from parent class fields only by capitalization",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4025/")]
    public async Task WarnOnChildFieldNameDifferingFromParentByCapitalizationOnly()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Parent
            {
                protected int value;
            }

            public class Child : Parent
            {
                private int Value; // name differs from Parent.value only by capitalization -> S4025
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S4025").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4041", "Type names should not match namespaces",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4041/")]
    public async Task WarnOnTypeNameMatchingNamespace()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Collections { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S4041").ShouldBeTrue();
    }
}
