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
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3927").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3971", "GC.SuppressFinalize should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3971/")]
    public async Task WarnOnGcSuppressFinalizeCalledOutsideDispose()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3971").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3981", "Collection sizes and array length comparisons should make sense",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3981/")]
    public async Task DetectNonsensicalCollectionSizeComparisons()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3981").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3984", "Exceptions should not be created without being thrown",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3984/")]
    public async Task WarnOnExceptionCreatedWithoutBeingThrown()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Factory
            {
                public static void CreateAndIgnoreException()
                {
                    new System.InvalidOperationException("This exception is created but never thrown");
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3984").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4143", "Collection elements should not be replaced unconditionally",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4143/")]
    public async Task DetectUnconditionalCollectionElementOverwrite()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using System.Collections.Generic;
            public class C
            {
                public static Dictionary<string, int> Build()
                {
                    var map = new Dictionary<string, int>();
                    map["key"] = 1;
                    map["key"] = 2; // S4143: same key overwritten unconditionally
                    return map;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4143").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4144", "Methods should not have identical implementations",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4144/")]
    public async Task DetectIdenticalMethodImplementations()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Calculator
            {
                private readonly int _offset;

                public Calculator(int offset)
                {
                    _offset = offset;
                }

                public int Add(int a, int b)
                {
                    var result = a + b;
                    return result + _offset;
                }

                public int Sum(int a, int b)
                {
                    var result = a + b;
                    return result + _offset;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4144").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4159", "Classes should implement their \"ExportAttribute\" interfaces",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4159/")]
    public async Task WarnOnExportAttributeWithUnimplementedInterface()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.ComponentModel.Composition", Version: "8.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using System.ComponentModel.Composition;
            namespace test;
            public interface IMyService { }
            [Export(typeof(IMyService))]
            public class MyService { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4159").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4260", "ConstructorArgument parameters should exist in constructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4260/")]
    public async Task DetectInvalidConstructorArgumentParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace System.Windows.Markup
            {
                [AttributeUsage(AttributeTargets.Property)]
                public sealed class ConstructorArgumentAttribute : Attribute
                {
                    public ConstructorArgumentAttribute(string argumentName) { }
                }
            }

            namespace test
            {
                using System.Windows.Markup;

                [AttributeUsage(AttributeTargets.Class)]
                public sealed class MyAttribute : Attribute
                {
                    [ConstructorArgument("nonExistentParam")]
                    public string Value { get; }

                    public MyAttribute(string actualParam)
                    {
                        Value = actualParam;
                    }
                }

                public static class Program { public static int Main() => 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4260").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4275", "Getters and setters should access the expected fields",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4275/")]
    public async Task DetectGetterAccessingWrongField()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Counter
            {
                private int _count;
                private int _total;

                public int Count
                {
                    get { return _total; }
                    set { _count = value; }
                }

                public int Total
                {
                    get { return _count; }
                    set { _total = value; }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4275").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4456", "Parameter validation in yielding methods should be wrapped",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4456/")]
    public async Task WarnOnParameterValidationInYieldingMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System;
            using System.Collections.Generic;

            namespace test;

            public static class Items
            {
                public static IEnumerable<int> GetPositive(int[] values)
                {
                    if (values == null) throw new ArgumentNullException(nameof(values));
                    foreach (var v in values)
                    {
                        if (v > 0)
                            yield return v;
                    }
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4456").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4583", "Calls to delegate's method \"BeginInvoke\" should be paired with calls to \"EndInvoke\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4583/")]
    public async Task WarnOnUnpairedBeginInvoke()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public delegate int Calculate(int x, int y);

            public static class DelegateUser
            {
                public static void Run()
                {
                    Calculate calc = (x, y) => x + y;
                    // BeginInvoke called without a matching EndInvoke — S4583 violation
                    calc.BeginInvoke(1, 2, null, null);
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4583").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S5856", "Regular expressions should be syntactically valid",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-5856/")]
    public async Task DetectInvalidRegexSyntax()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Text.RegularExpressions;
            namespace test;
            public sealed class C
            {
                public static Regex BadPattern() => new Regex("[A");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S5856").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6507", "Blocks should not be synchronized on local variables",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6507/")]
    public async Task DetectLockOnLocalVariable()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void DoSomething()
                {
                    object local = new object();
                    lock (local) // S6507: locking on a local — each thread gets its own instance
                    {
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6507").ShouldBeTrue();
    }
}
