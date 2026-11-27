using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class StyleCopAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("SA1649", "File name should match first type name",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md")]
    public async Task RequireFileNameToMatchFirstTypeName()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("sample.cs", "class Program { static int Main() => 0; }");
        // File is named WrongName.cs but the first type is RightName — SA1649 fires.
        await project.AddFileAsync("WrongName.cs", "public class RightName { }");
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1649").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1143", "Conditional expressions must be formatted consistently",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1143.md")]
    public async Task RequireConsistentConditionalExpressionFormatting()
    {
        using var project = await CreateProjectBuilderAsync();
        // '?' and ':' trail their lines instead of leading indented lines — SA1143 fires.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main(string[] args)
                {
                    return args.Length > 0 ?
                        1 :
                        0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1143").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1144", "Zero-width characters in literals must be escaped",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1144.md")]
    public async Task RequireEscapedZeroWidthCharactersInLiterals()
    {
        using var project = await CreateProjectBuilderAsync();
        // The generated string literal contains a raw (unescaped) U+200B zero-width space — SA1144 fires.
        // The test source itself builds the character from its code point so it does not trip SA1144 on this project.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public const string Value = "a{ZWSP}b";

                public static int Main() => Value.Length;
            }
            """.Replace("{ZWSP}", char.ConvertFromUtf32(0x200B), StringComparison.Ordinal));
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1144").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1145", "Do not use assignment as condition",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1145.md")]
    public async Task ProhibitAssignmentAsCondition()
    {
        using var project = await CreateProjectBuilderAsync();
        // The 'if' condition is an assignment expression rather than a comparison — SA1145 fires.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Program
            {
                public static int Main(string[] args)
                {
                    bool hasArgs;
                    if (hasArgs = args.Length > 0)
                    {
                        return 1;
                    }

                    return hasArgs ? 2 : 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1145").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1315", "Parameter names should match inherited names",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1315.md")]
    public async Task RequireParameterNamesToMatchInheritedNames()
    {
        using var project = await CreateProjectBuilderAsync();
        // The override renames 'sound' to 'noise' — SA1315 fires (CA1725 also fires; not asserted here).
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public abstract class Animal
            {
                public abstract void Speak(string sound);
            }

            public class Dog : Animal
            {
                public override void Speak(string noise) { }
            }

            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1315").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1317", "Identifier should be named only with Latin letters",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1317.md")]
    public async Task RequireLatinLettersInIdentifiers()
    {
        using var project = await CreateProjectBuilderAsync();
        // The local is named with a Greek alpha (emitted via escape to keep this file ASCII) — SA1317 fires.
        await project.AddFileAsync("Program.cs", $$"""
            namespace test;

            public static class Program
            {
                public static int Main()
                {
                    var value{{"α"}} = 0;
                    return value{{"α"}};
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("SA1317").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1653", "Only one partial declaration should contain summary documentation",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1653.md")]
    public async Task RequireSummaryOnOnlyOnePartialDeclaration()
    {
        using var project = await CreateProjectBuilderAsync();
        // Both partial declarations of Program carry a <summary> — SA1653 fires.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            /// <summary>
            /// The program entry point.
            /// </summary>
            public static partial class Program
            {
                /// <summary>
                /// Runs the program.
                /// </summary>
                /// <returns>The exit code.</returns>
                public static int Main() => 0;
            }

            /// <summary>
            /// More documentation for the program.
            /// </summary>
            public static partial class Program
            {
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1653").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1654", "Properly encode generic brackets",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1654.md")]
    public async Task RequireEncodedGenericBracketsInDocumentation()
    {
        using var project = await CreateProjectBuilderAsync();
        // The cref uses escaped angle brackets (&lt;T&gt;) instead of braces ({T}) — SA1654 fires.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            /// <summary>
            /// See <see cref="System.Collections.Generic.List&lt;T&gt;"/>.
            /// </summary>
            public static class Program
            {
                /// <summary>
                /// Runs the program.
                /// </summary>
                /// <returns>The exit code.</returns>
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1654").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SA1655", "Documentation element tags must be consistently placed",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1655.md")]
    public async Task RequireConsistentlyPlacedDocumentationElementTags()
    {
        using var project = await CreateProjectBuilderAsync();
        // The opening <summary> tag sits on its own line but the closing tag does not — SA1655 fires.
        await project.AddFileAsync("Program.cs", """
            namespace test;

            /// <summary>
            /// The program entry point.</summary>
            public static class Program
            {
                /// <summary>
                /// Runs the program.
                /// </summary>
                /// <returns>The exit code.</returns>
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("SA1655").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("SX1412", "Store files as UTF-8 without byte order mark",
        HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1412.md")]
    public async Task RequireUtf8WithoutByteOrderMark()
    {
        using var project = await CreateProjectBuilderAsync();
        // A leading U+FEFF (kept as an ASCII escape) is written as the EF BB BF BOM bytes — SX1412 fires.
        await project.AddFileAsync("Program.cs", "\uFEFF" + """
            namespace test;
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasNote("SX1412").ShouldBeTrue();
    }
}
