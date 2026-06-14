// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesBugs3Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S3927", "Serialization event handlers should be implemented correctly",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3927/")]
    public async Task DetectIncorrectSerializationEventHandlerSignature()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Runtime.Serialization;

            namespace test;

            [Serializable]
            public class Foo
            {
                [OnSerializing]
                public void OnSerializing(StreamingContext context) { } // Noncompliant: must be private

                [OnSerialized]
                private int OnSerialized(StreamingContext context) => 0; // Noncompliant: must return void

                [OnDeserializing]
                private void OnDeserializing() { } // Noncompliant: missing StreamingContext parameter

                [OnDeserialized]
                private void OnDeserialized(StreamingContext context, string extra) { } // Noncompliant: extra parameter
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3927").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3971", "GC.SuppressFinalize should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3971/")]
    public async Task WarnOnGcSuppressFinalizeCalledOutsideDispose()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class MyResource
            {
                public void Initialize()
                {
                    GC.SuppressFinalize(this);
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3971").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3981", "Collection sizes and array length comparisons should make sense",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3981/")]
    public async Task DetectNonsensicalCollectionSizeComparisons()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            using System.Collections.Generic;
            namespace test;
            public static class Check
            {
                public static bool AlwaysFalse()
                {
                    var list = new List<int> { 1, 2, 3 };
                    return list.Count < 0;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S3981").ShouldBeTrue();
    }
}
