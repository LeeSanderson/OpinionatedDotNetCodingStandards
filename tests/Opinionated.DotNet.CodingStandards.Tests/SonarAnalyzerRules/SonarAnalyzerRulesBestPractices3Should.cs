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
}
