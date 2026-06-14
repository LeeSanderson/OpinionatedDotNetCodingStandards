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
    [RuleDoc("S6802", "Using lambda expressions in loops should be avoided in Blazor markup section",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6802/")]
    public async Task WarnOnLambdaExpressionsInLoopsInBlazorMarkup()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.AspNetCore.Components", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.AspNetCore.Components.Rendering;

            namespace test;

            public class LoopLambdaComponent
            {
                private static readonly string[] Items = ["a", "b", "c"];

                public static void BuildRenderTree(RenderTreeBuilder builder)
                {
                    foreach (var item in Items)
                    {
                        builder.OpenElement(0, "button");
                        builder.AddAttribute(1, "onclick", (Microsoft.AspNetCore.Components.Web.MouseEventArgs e) => System.Console.WriteLine(item));
                        builder.CloseElement();
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6802").ShouldBeTrue();
    }
}
