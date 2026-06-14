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
    [RuleDoc("S6930", "Backslash should be avoided in route templates",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6930/")]
    public async Task WarnOnBackslashInRouteTemplate()
    {
        using var project = await CreateProjectBuilderAsync(
            properties:
            [
                ("NuGetAudit", "false"),
                ("NoWarn", "NU1903;NU1902;CA1515;CA1822"),
            ],
            packageReferences:
            [
                (Name: "Microsoft.AspNetCore.Mvc", Version: "2.3.10"),
            ]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Mvc;

            namespace test;

            [Route(@"api\controller")]
            public class HomeController : Controller { }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6930").ShouldBeTrue();
    }
}
