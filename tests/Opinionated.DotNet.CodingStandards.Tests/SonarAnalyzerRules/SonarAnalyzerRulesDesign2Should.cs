// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesDesign2Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S3995", "URI return values should not be strings",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3995/")]
    public async Task ProhibitStringReturnValueForUriNamedMethod()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class UriProvider
            {
                public string GetUri() => "https://example.com";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3995").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3996", "URI properties should not be strings",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3996/")]
    public async Task WarnOnStringUriProperty()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Resource
            {
                public string Url { get; set; } = string.Empty;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3996").ShouldBeTrue();
    }
}
