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
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
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
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3456").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3458", "Empty 'case' clauses that fall through to the 'default' should be omitted",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3458/")]
    public async Task WarnOnEmptyCaseClausesFallingThroughToDefault()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class Switcher
            {
                public static void Classify(int value)
                {
                    string result;
                    switch (value)
                    {
                        case 1:
                            result = "one";
                            break;
                        case 2:
                        case 3:
                        default:
                            result = "other";
                            break;
                    }
                    _ = result;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3458").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3459", "Unassigned members should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3459/")]
    public async Task WarnOnUnassignedMembers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyClass
            {
                private int _neverAssigned;

                public int GetValue() => _neverAssigned;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3459").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3532", "Empty default clauses should be removed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3532/")]
    public async Task WarnOnEmptyDefaultClause()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static string Classify(int n)
                {
                    switch (n)
                    {
                        case 1:
                            return "one";
                        case 2:
                            return "two";
                        default:
                            break;
                    }
                    return "other";
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3532").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3597", "ServiceContract and OperationContract attributes should be used together",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3597/")]
    public async Task WarnOnMissingOperationContractOnServiceContractInterface()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.ServiceModel.Primitives", Version: "10.0.652802")]);
        await project.AddFileAsync("Program.cs", """
            using System.ServiceModel;
            namespace test;
            [ServiceContract]
            public interface IMyService
            {
                string GetData(int value);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3597").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3600", "params should not be introduced on overrides",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3600/")]
    public async Task WarnOnParamsIntroducedOnOverride()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public abstract class Base
            {
                public abstract void DoWork(int[] values);
            }

            public class Derived : Base
            {
                public override void DoWork(params int[] values) { }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3600").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3604", "Member initializer values should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3604/")]
    public async Task WarnOnRedundantMemberInitializer()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            #pragma warning disable S3052, CA1805, S2933, IDE0044, SA1649, S109, S3400
            namespace test;

            public sealed class Widget
            {
                private int _value = 5;

                public Widget(int value)
                {
                    _value = value;
                }

                public Widget()
                {
                    _value = 1;
                }

                public int Value => _value;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3604").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3626", "Jump statements should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3626/")]
    public async Task WarnOnRedundantJumpStatement()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void Method()
                {
                    System.Console.WriteLine("hello");
                    return; // redundant: return at end of void method triggers S3626
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3626").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3717", "Track use of \"NotImplementedException\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3717/")]
    public async Task WarnOnNotImplementedException()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Service
            {
                public void DoWork() => throw new System.NotImplementedException();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3717").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3878", "Arrays should not be created for params parameters",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3878/")]
    public async Task ProhibitArrayCreationForParamsParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Helper
            {
                public static void Accept(params string[] values) { }
            }
            public static class Program
            {
                public static int Main()
                {
                    Helper.Accept(new string[] { "a", "b", "c" });
                    return 0;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3878").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3885", "Assembly.Load should be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3885/")]
    public async Task WarnOnAssemblyLoadFromOrLoadFile()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Reflection;
            namespace test;
            public static class Loader
            {
                public static Assembly Load(string path) =>
                    Assembly.LoadFrom(path); // S3885: should use Assembly.Load
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3885").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3902", "Assembly.GetExecutingAssembly should not be called",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3902/")]
    public async Task WarnOnAssemblyGetExecutingAssembly()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Reflection;
            namespace test;
            public static class Loader
            {
                public static Assembly GetAssembly() => Assembly.GetExecutingAssembly();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3902").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3937", "Number patterns should be regular",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3937/")]
    public async Task WarnOnIrregularNumberLiteralPattern()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Numbers
            {
                public const int Irregular = 1_2_3_4_5;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3937").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4005", "System.Uri arguments should be used instead of strings",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4005/")]
    public async Task WarnOnStringUriArgumentWhenUriOverloadExists()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            using System;

            public class MyClient
            {
                public void Download(string url) { }
                public void Download(Uri url) { }
            }

            public class Caller
            {
                public static void Run()
                {
                    var client = new MyClient();
                    client.Download("http://example.com");
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4005").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4040", "Strings should be normalized to uppercase",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4040/")]
    public async Task WarnOnToLowerStringNormalization()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class StringNormalizer
            {
                public static bool AreEqual(string a, string b)
                    => a.ToLowerInvariant() == b.ToLowerInvariant();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4040").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4055", "Literals should not be passed as localized parameters",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4055/")]
    public async Task WarnOnLiteralPassedToLocalizedParameter()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Dialog
            {
                public static void Show(string message) { }
            }
            public static class Program { public static int Main() { Dialog.Show("Hello, world!"); return 0; } }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4055").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4057", "Locales should be set for data types",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4057/")]
    public async Task WarnOnDataTableWithoutLocale()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Data;
            using System.Globalization;

            namespace test;

            public static class Factory
            {
                public static DataTable Create()
                {
                    var table = new DataTable();
                    // Locale is never set — S4057 fires here
                    return table;
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4057").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4061", "“params” should be used instead of “varargs”",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4061/")]
    public async Task ProhibitVarargsInsteadOfParams()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public void Method(int x, __arglist) { }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4061").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4201", "Null checks should not be combined with 'is' operator checks",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4201/")]
    public async Task WarnOnRedundantNullCheckWithIsOperator()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                public static string Describe(object obj)
                {
                    if (obj != null && obj is string)
                    {
                        return "string";
                    }
                    return "other";
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4201").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4220", "Events should have proper arguments",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4220/")]
    public async Task WarnOnImproperEventArguments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyClass
            {
                public event System.EventHandler MyEvent;
                public void Raise() => MyEvent?.Invoke(this, null);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4220").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4428", "PartCreationPolicyAttribute should be used with ExportAttribute",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4428/")]
    public async Task WarnOnPartCreationPolicyWithoutExport()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.ComponentModel.Composition", Version: "8.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using System.ComponentModel.Composition;

            namespace test;

            [PartCreationPolicy(CreationPolicy.Shared)]
            public class MyComponent
            {
                public int Value => 42;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4428").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4457", "Parameter validation in \"async\"/\"await\" methods should be wrapped",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4457/")]
    public async Task WarnOnParameterValidationInAsyncMethodNotWrapped()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class C
            {
                public static async System.Threading.Tasks.Task<string> ProcessAsync(string value)
                {
                    if (value == null) { throw new System.ArgumentNullException(nameof(value)); }
                    await System.Threading.Tasks.Task.Delay(1);
                    return value + "processed";
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4457").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4524", "“default” clauses should be first or last",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4524/")]
    public async Task WarnOnDefaultClauseNotFirstOrLast()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static string Classify(int n)
                {
                    switch (n)
                    {
                        case 1:
                            return "one";
                        default:
                            return "other";
                        case 2:
                            return "two";
                    }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4524").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4635", "Start index should be used instead of calling Substring",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4635/")]
    public async Task WarnOnSubstringWithIndexOfStartIndex()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class StringHelper
            {
                public static int FindAfterOffset(string input, string separator, int offset)
                {
                    return input.Substring(offset).IndexOf(separator);
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4635").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4663", "Comments should not be empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4663/")]
    public async Task ProhibitEmptyComments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Program
            {
                //
                public static int Main() => 0;
            }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4663").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6354", "Use a testable date/time provider",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6354/")]
    public async Task WarnOnDirectDateTimeNowAccess()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class TimeService
            {
                public string GetCurrentTime() =>
                    $"The time is {DateTime.UtcNow}";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6354").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6513", "ExcludeFromCodeCoverage attributes should include a justification",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6513/")]
    public async Task WarnOnExcludeFromCodeCoverageWithoutJustification()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Diagnostics.CodeAnalysis;

            namespace test;

            [ExcludeFromCodeCoverage]
            public class UncoveredClass
            {
                public int Value { get; set; }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6513").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6561", "Avoid using \"DateTime.Now\" for benchmarking or timing operations",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6561/")]
    public async Task WarnOnDateTimeNowUsedForTiming()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Benchmarker
            {
                public static System.TimeSpan Measure(System.Action action)
                {
                    var start = System.DateTime.Now;
                    action();
                    return System.DateTime.Now - start;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6561").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6562", "Always set the DateTimeKind when creating new DateTime instances",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6562/")]
    public async Task DetectDateTimeWithoutKind()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static System.DateTime GetDate() => new System.DateTime(2024, 1, 15);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6562").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6563", "Use UTC when recording DateTime instants",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6563/")]
    public async Task WarnOnLocalDateTimeInstant()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Recorder
            {
                public static System.DateTime CapturedAt = System.DateTime.Now;
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6563").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6566", "Use \"DateTimeOffset\" instead of \"DateTime\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6566/")]
    public async Task WarnOnDateTimeUsedInsteadOfDateTimeOffset()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Recorder
            {
                public void Record()
                {
                    var now = System.DateTime.UtcNow; // S6566: use DateTimeOffset instead
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6566").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6575", "Use \"TimeZoneInfo.FindSystemTimeZoneById\" without converting the timezones with \"TimezoneConverter\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6575/")]
    public async Task WarnOnTimeZoneConverterUsage()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "TimeZoneConverter", Version: "7.2.0")]);
        await project.AddFileAsync("Program.cs", """
            using TimeZoneConverter;
            namespace test;
            public static class Example
            {
                public static string Convert(string ianaId) =>
                    TZConvert.IanaToWindows(ianaId);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6575").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6585", "Don't hardcode the format when turning dates and times to strings",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6585/")]
    public async Task WarnOnHardcodedDateTimeFormatString()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class DateFormatter
            {
                public static string Format(System.DateTime dt) =>
                    dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6585").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6602", "Find method should be used instead of the FirstOrDefault extension on List<T>",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6602/")]
    public async Task WarnOnFirstOrDefaultExtensionOnList()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            public static class Example
            {
                public static int? GetFirstEven(List<int> numbers)
                    => numbers.FirstOrDefault(n => n % 2 == 0);
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6602").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6603", "The collection-specific TrueForAll method should be used instead of the All extension on List<T>",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6603/")]
    public async Task WarnOnAllExtensionInsteadOfTrueForAll()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            public static class C
            {
                public static bool Check(List<int> items) =>
                    items.All(x => x > 0);
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6603").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6605", "Collection-specific \"Exists\" method should be used instead of the \"Any\" extension on List<T>",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6605/")]
    public async Task WarnOnAnyExtensionInsteadOfListExists()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            public static class Example
            {
                public static bool Check(List<int> items) =>
                    items.Any(x => x > 0);
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6605").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6609", "Min/Max properties of Set types should be used instead of the Enumerable extension methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6609/")]
    public async Task WarnOnEnumerableMinMaxOnSetTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;

            namespace test;

            public static class SetMinMaxUsage
            {
                public static int GetMin()
                {
                    var set = new SortedSet<int> { 3, 1, 4, 1, 5, 9 };
                    return set.Min(); // S6609: should use set.Min property, not Enumerable.Min()
                }

                public static int GetMax()
                {
                    var set = new SortedSet<int> { 3, 1, 4, 1, 5, 9 };
                    return set.Max(); // S6609: should use set.Max property, not Enumerable.Max()
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6609").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6612", "The lambda parameter should be used instead of capturing arguments in ConcurrentDictionary methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6612/")]
    public async Task WarnOnCapturedArgumentInConcurrentDictionaryLambda()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Concurrent;

            namespace test;

            public static class Cache
            {
                private static readonly ConcurrentDictionary<string, int> store = new();

                public static int GetOrCreate(string key)
                    => store.GetOrAdd(key, _ => key.Length); // S6612: use the lambda parameter instead of capturing 'key'
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6612").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6613", "“First” and “Last” properties of “LinkedList” should be used instead of the “First()” and “Last()” extension methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6613/")]
    public async Task WarnOnLinkedListExtensionMethodsInsteadOfProperties()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Collections.Generic;
            using System.Linq;
            namespace test;
            public class C
            {
                public static void Method()
                {
                    var list = new LinkedList<int>(new[] { 1, 2, 3 });
                    var first = list.First();
                    var last = list.Last();
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6613").ShouldBeTrue();
    }
}
