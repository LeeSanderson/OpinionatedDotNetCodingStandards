using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;


[Collection(nameof(PackageCollection))]
public class CodingStandardsShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    public async Task IgnoreNameCanBeSimplifiedAsOnlyTreatedAsErrorsInIDE()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private readonly int _i = 1;
                public int Get() => this._i;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0003").ShouldBeFalse();
    }

    [Fact]
    public async Task RequireFileScopedNamespaces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0161").ShouldBeTrue(); // Convert to file-scoped namespace
    }

    [Fact]
    public async Task AllowWarningsAsErrorsToBeDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [
            (Name: "TreatWarningsAsErrors", Value: "false"),
            (Name: "MSBuildTreatWarningsAsErrors", Value: "false"),
        ]);
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        // Convert to file-scoped namespace should now be a warning instead of an error
        buildOutput.HasWarning("IDE0161").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireVarInsteadOfExplicitType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    int i = 1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0007").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireRemovalOfUnnecessaryCast()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    var i = (int)1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0004").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireRemovalOfUnnecessaryUsings()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Collections.Generic;
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0005").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireBracesForControlFlow()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    if (true)
                        return 1;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0011").ShouldBeTrue();
    }

    [Fact]
    public async Task RequirePatternMatchingForIsCheck()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    object obj = "test";
                    if (obj is string)
                    {
                        var s = (string)obj;
                        return s.Length;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0020").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireCoalesceExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get(string? input) 
                {
                    return input != null ? input : "default";
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0029").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNullPropagation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Get(string? input) 
                {
                    return input != null ? input.Length : 0;
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0031").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireCorrectModifierOrder()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                static public int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0036").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireAccessibilityModifiers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Program
            {
                void Method() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0040").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireReadonlyModifierOnFields()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Program
            {
                private int _value = 1;
                public int GetValue() => _value;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0044").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireLanguageKeywordsOverFrameworkTypes()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static Int32 Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0049").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireCompoundAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    var i = 1;
                    i = i + 1;
                    return i;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0054").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireProperFormatting()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
            public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0055").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireRemovalOfUnnecessaryValueAssignment()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    var i = 1;
                    i = 2;
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0059").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireStaticLocalFunctions()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    return LocalFunc();
                    int LocalFunc() => 1;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0062").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireUsingDirectivesOutsideNamespace()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test
            {
                using System;
                public static class Program
                {
                    public static int Main() => 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0065").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireSimplifiedInterpolation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get() 
                {
                    var value = 123;
                    return $"{value.ToString()}";
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0071").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireRemovalOfUnnecessarySuppressionOperator()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    var value = "test";
                    return value!.Length;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0080").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNameofOverTypeof()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static string Get() => typeof(Program).Name;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0082").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireRemovalOfUnnecessaryDiscard()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    _ = GetValue();
                    return 0;
                }
                private static int GetValue() => 1;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0110").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireSimplifiedPropertyPattern()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Person
            {
                public string Name { get; set; } = "";
            }
            public static class Program
            {
                public static bool Check(Person p) 
                {
                    return p is { Name: { Length: > 0 } };
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0170").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNameofForStrings()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void CheckArg(string value)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new System.ArgumentException("value");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0280").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireSimplifiedDelegateInvocation()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static void Invoke(System.Action? action) 
                {
                    if (action != null)
                        action();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1005").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireCorrectNamingStyle()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                private int Field = 1;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE1006").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireEmbeddedStatementsOnOwnLine()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() { if (true) { return 1; } return 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2001").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNoBlankLineBetweenBraces()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {

                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2002").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireBlankLineAfterBlock()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    if (true)
                    {
                        var x = 1;
                    }
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2003").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNoBlankLineAfterConstructorColon()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Base
            {
                public Base(int x) { }
            }
            public class Derived : Base
            {
                public Derived() :
            
                    base(1) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2004").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNoBlankLineAfterConditionalToken()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() 
                {
                    var result = true ?
            
                        1 : 0;
                    return result;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2005").ShouldBeTrue();
    }

    [Fact]
    public async Task RequireNoBlankLineAfterArrowExpression()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Get() =>
            
                    1;
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE2006").ShouldBeTrue();
    }
}