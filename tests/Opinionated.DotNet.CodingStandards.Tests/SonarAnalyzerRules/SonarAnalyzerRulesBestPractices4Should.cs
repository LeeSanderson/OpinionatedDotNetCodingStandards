// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBestPractices4Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S6617", "Contains should be used instead of Any for simple equality checks",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6617/")]
    public async Task WarnOnAnyUsedForSimpleEqualityCheck()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            public static class Checker
            {
                public static bool HasItem(List<string> items, string value)
                    => items.Any(x => x == value);
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6617").ShouldBeTrue();
    }
}
