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

    [Fact]
    [RuleDoc("S6618", "string.Create should be used instead of FormattableString",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6618/")]
    public async Task WarnOnFormattableStringInsteadOfStringCreate()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Example
            {
                public static string GetInvariant(int value) =>
                    FormattableString.Invariant($"value={value}");

                public static string GetCurrentCulture(double value) =>
                    FormattableString.CurrentCulture($"value={value}");
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6618").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6664", "The code block contains too many logging calls",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6664/")]
    public async Task ProhibitExcessiveLoggingCallsInBlock()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Service
            {
                public static void Process(ILogger logger, string a, string b, string c)
                {
                    logger.LogInformation("Starting {Name}", a);
                    logger.LogInformation("Processing {Name}", b);
                    logger.LogInformation("Finished {Name}", c);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6664").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6667", "Logging in a catch clause should pass the caught exception as a parameter",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6667/")]
    public async Task WarnOnMissingExceptionInCatchLogging()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Program
            {
                public static void Run(ILogger logger)
                {
                    try { _ = int.Parse("x"); }
                    catch (Exception)
                    {
                        logger.LogError("Something went wrong");
                    }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6667").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6668", "Logging arguments should be passed to the correct parameter",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6668/")]
    public async Task WarnOnLoggingArgumentsPassedToWrongParameter()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class C
            {
                public static void Log(ILogger logger)
                {
                    var ex = new System.Exception("boom");
                    // Passing Exception into the params args slot instead of the dedicated exception parameter
                    logger.LogInformation("Unexpected error: {Message}", ex);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6668").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6670", "Trace.Write and Trace.WriteLine should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6670/")]
    public async Task ProhibitTraceWriteAndWriteLine()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics;

            namespace test;

            public static class Logger
            {
                public static void Log(string message)
                {
                    Trace.Write(message);
                    Trace.WriteLine(message);
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6670").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6677", "Message template placeholders should be unique",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6677/")]
    public async Task WarnOnDuplicateMessageTemplatePlaceholders()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.Extensions.Logging;
            namespace test;
            public static class Program
            {
                public static void Run(ILogger logger)
                {
                    // S6677: placeholder {foo} appears twice in the same template
                    logger.LogInformation("Value {foo} and again {foo}", 1, 2);
                }
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6677").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6798", "[JSInvokable] attribute should only be used on public methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6798/")]
    public async Task ProhibitJSInvokableOnNonPublicMethod()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "Microsoft.JSInterop", Version: "10.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using Microsoft.JSInterop;

            namespace test;

            public class MyComponent
            {
                [JSInvokable]
                private void CallFromJs() { }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6798").ShouldBeTrue();
    }
}
