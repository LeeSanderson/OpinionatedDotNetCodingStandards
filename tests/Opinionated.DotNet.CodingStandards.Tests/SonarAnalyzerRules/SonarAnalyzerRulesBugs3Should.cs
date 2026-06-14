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
}
