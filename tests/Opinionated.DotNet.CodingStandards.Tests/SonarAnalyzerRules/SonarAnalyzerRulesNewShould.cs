using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

public class SonarAnalyzerRulesNewShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S6617", "\"Contains\" should be used instead of \"Any\" for simple equality checks",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6617/")]
    public async Task UseContainsInsteadOfAnyForSimpleEqualityChecks()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public class C
            {
                public bool M(List<string> list) => list.Any(x => x == "value");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6617").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6618", "\"string.Create\" should be used instead of \"FormattableString\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6618/")]
    public async Task UseStringCreateInsteadOfFormattableString()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public string M(int value) => System.FormattableString.Invariant($"{value}");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6618").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6667", "Logging in a catch clause should pass the caught exception as a parameter",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6667/")]
    public async Task LoggingInCatchShouldPassCaughtException()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class C
            {
                public void M(ILogger logger)
                {
                    try { System.Console.WriteLine(); }
                    catch (System.Exception ex)
                    {
                        logger.LogError("An error occurred: {Message}", ex.Message);
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6667").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6668", "Logging arguments should be passed to the correct parameter",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6668/")]
    public async Task LoggingArgumentsShouldBePassedToCorrectParameter()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class C
            {
                public void M(ILogger logger, System.Exception ex)
                {
                    logger.LogError("Error: {Exception}", ex);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6668").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6670", "\"Trace.Write\" and \"Trace.WriteLine\" should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6670/")]
    public async Task ShouldNotUseTraceWrite()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void M() => System.Diagnostics.Trace.WriteLine("debug message");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6670").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6677", "Message template placeholders should be unique",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6677/")]
    public async Task MessageTemplatePlaceholdersShouldBeUnique()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class C
            {
                public void M(ILogger logger, int a, int b)
                {
                    logger.LogInformation("{Value} {Value}", a, b);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6677").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6798", "[JSInvokable] attribute should only be used on public methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6798/")]
    public async Task JSInvokableAttributeShouldOnlyBeUsedOnPublicMethods()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace Microsoft.JSInterop
            {
                public sealed class JSInvokableAttribute : System.Attribute { }
            }
            namespace test
            {
                public class C
                {
                    [Microsoft.JSInterop.JSInvokable]
                    private void M() { }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6798").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6664", "The code block contains too many logging calls",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6664/")]
    public async Task CodeBlockContainsTooManyLoggingCalls()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public class C
            {
                public void M(ILogger logger)
                {
                    logger.LogWarning("First warning");
                    logger.LogWarning("Second warning");
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6664").ShouldBeTrue();
    }
}
