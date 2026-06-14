// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBestPractices3Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S3456", "string.ToCharArray() and ReadOnlySpan&lt;T&gt;.ToArray() should not be called redundantly",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3456/")]
    public async Task WarnOnRedundantToCharArrayCall()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static int CountVowels(string s)
                {
                    int count = 0;
                    foreach (char c in s.ToCharArray()) // S3456: ToCharArray() is redundant; foreach can iterate over a string directly
                    {
                        if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') count++;
                    }
                    return count;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3456").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3458", "Empty 'case' clauses that fall through to the 'default' should be omitted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3458/")]
    public async Task WarnOnEmptyCaseClausesFallingThroughToDefault()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Switcher
            {
                public static void Classify(int value)
                {
                    string result;
                    switch (value)
                    {
                        case 1:
                            result = "one";
                            break;
                        case 2:
                        case 3:
                        default:
                            result = "other";
                            break;
                    }
                    _ = result;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3458").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3459", "Unassigned members should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3459/")]
    public async Task WarnOnUnassignedMembers()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class MyClass
            {
                private int _neverAssigned;

                public int GetValue() => _neverAssigned;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3459").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3532", "Empty default clauses should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3532/")]
    public async Task WarnOnEmptyDefaultClause()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class C
            {
                public static string Classify(int n)
                {
                    switch (n)
                    {
                        case 1:
                            return "one";
                        case 2:
                            return "two";
                        default:
                            break;
                    }
                    return "other";
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3532").ShouldBeTrue();
    }
}
