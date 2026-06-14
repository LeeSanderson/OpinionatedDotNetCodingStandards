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
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class UriProvider
            {
                public string GetUri() => "https://example.com";
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3995").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3996", "URI properties should not be strings",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3996/")]
    public async Task WarnOnStringUriProperty()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Resource
            {
                public string Url { get; set; } = string.Empty;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3996").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S3997", "String URI overloads should call \"System.Uri\" overloads",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-3997/")]
    public async Task WarnOnStringUriOverloadNotDelegatingToUriOverload()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class ResourceFetcher
            {
                public void FetchResource(string uriString)
                {
                    // Noncompliant: string overload exists alongside Uri overload but does not call it
                    _ = uriString.Length;
                }

                public void FetchResource(Uri uri)
                {
                    _ = uri.AbsolutePath;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S3997").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4004", "Collection properties should be readonly",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4004/")]
    public async Task ProhibitMutableCollectionProperties()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using System.Collections.Generic;
            public class Model
            {
                public List<int> Items { get; set; } = new();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4004").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4015", "Inherited member visibility should not be decreased",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4015/")]
    public async Task ProhibitDecreasingInheritedMemberVisibility()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Base
            {
                public virtual int Value => 1;
            }

            public class Derived : Base
            {
                // No 'new' keyword — decreases visibility of inherited public property — triggers S4015
                private int Value => 2;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4015").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4017", "Method signatures should not contain nested generic types",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4017/")]
    public async Task ProhibitNestedGenericTypesInMethodSignatures()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using System.Collections.Generic;

            public class Api
            {
                public static IEnumerable<IEnumerable<string>> GetNestedGroups(
                    IEnumerable<IEnumerable<int>> input) => new List<List<string>>();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4017").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4018", "All type parameters should be used in the parameter list to enable type inference",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4018/")]
    public async Task WarnOnTypeParameterAbsentFromParameterList()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public class Factory
            {
                // T appears only in the return type, not in any parameter — S4018 fires
                public static T Create<T>() => default!;
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4022", "Enumerations should have \"Int32\" storage",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4022/")]
    public async Task WarnOnNonInt32EnumerationStorage()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public enum Color : byte { Red, Green, Blue }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4022").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4023", "Interfaces should not be empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4023/")]
    public async Task ProhibitEmptyInterfaces()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public interface IMarker { }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4023").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4027", "Exceptions should provide standard constructors",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4027/")]
    public async Task WarnOnExceptionMissingStandardConstructors()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            using System;

            public class CustomException : Exception
            {
                public CustomException(string message) : base(message) { }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4027").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4035", "Classes implementing \"IEquatable<T>\" should be sealed",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4035/")]
    public async Task WarnOnNonSealedIEquatableClass()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class MyValue : System.IEquatable<MyValue>
            {
                private readonly int _value;
                public MyValue(int value) { _value = value; }
                public bool Equals(MyValue? other) => other is not null && _value == other._value;
                public override bool Equals(object? obj) => Equals(obj as MyValue);
                public override int GetHashCode() => _value.GetHashCode();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4035").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4047", "Generics should be used when appropriate",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4047/")]
    public async Task WarnOnRefObjectParameters()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static void Swap(ref object a, ref object b)
                {
                    object temp = a;
                    a = b;
                    b = temp;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4047").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4049", "Properties should be preferred",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4049/")]
    public async Task WarnOnGetMethodThatShouldBeProperty()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Sample
            {
                private readonly string _value = "hello";
                public string GetValue() => _value;
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4049").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4136", "Method overloads should be grouped together",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4136/")]
    public async Task WarnOnUngroupedMethodOverloads()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Calculator
            {
                public int Add(int a, int b) => a + b;

                public int Subtract(int a, int b) => a - b;

                public int Add(int a, int b, int c) => a + b + c;
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4136").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4200", "Native methods should be wrapped",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4200/")]
    public async Task WarnOnPublicNativeMethod()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            using System.Runtime.InteropServices;

            namespace test;

            public static class NativeMethods
            {
                [DllImport("kernel32.dll")]
                public static extern bool Beep(uint dwFreq, uint dwDuration);
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4225", "Extension methods should not extend \"object\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4225/")]
    public async Task ProhibitExtensionMethodsOnObject()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;

            public static class ObjectExtensions
            {
                public static string Describe(this object obj) => obj.GetType().Name;
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4225").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4226", "Extensions should be in separate namespaces",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4226/")]
    public async Task WarnOnExtensionMethodInSameNamespaceAsExtendedType()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class Foo { }
            public static class FooExtensions
            {
                public static void Bar(this Foo foo) { }
            }
            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4226").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S4277", "Shared parts should not be created with new",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-4277/")]
    public async Task WarnOnDirectInstantiationOfSharedPart()
    {
        using var project = await CreateProjectBuilderAsync(
            packageReferences: [(Name: "System.ComponentModel.Composition", Version: "8.0.0")]);
        await project.AddFileAsync("Program.cs", """
            using System.ComponentModel.Composition;

            namespace test;

            [Export]
            [PartCreationPolicy(CreationPolicy.Shared)]
            public class MySharedService
            {
                public int GetValue() => 42;
            }

            public static class Consumer
            {
                public static MySharedService Create() => new MySharedService();
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S4277").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6931", "ASP.NET controller actions should not have a route template starting with \"/\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6931/")]
    public async Task WarnOnControllerActionsWithAbsoluteRoutePaths()
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

            public class HomeController : ControllerBase
            {
                [Route("/home/index")]
                public IActionResult Index() => Ok();

                [Route("/home/about")]
                public IActionResult About() => Ok();
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6931").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6934", "A Route attribute should be added to the controller when a route template is specified at the action level",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6934/")]
    public async Task WarnOnMissingControllerRouteAttributeWhenActionSpecifiesRouteTemplate()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace Microsoft.AspNetCore.Mvc.Routing
            {
                public interface IRouteTemplateProvider
                {
                    string? Template { get; }
                }
                public abstract class HttpMethodAttribute : System.Attribute, IRouteTemplateProvider
                {
                    public string? Template { get; }
                    protected HttpMethodAttribute(string template) { Template = template; }
                    string? IRouteTemplateProvider.Template => Template;
                }
            }
            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class HttpGetAttribute : Routing.HttpMethodAttribute
                {
                    public HttpGetAttribute(string template) : base(template) { }
                }
            }
            namespace test
            {
                public class HomeController : Microsoft.AspNetCore.Mvc.ControllerBase
                {
                    [Microsoft.AspNetCore.Mvc.HttpGet("items")]
                    public int GetItems() => 0;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6934").ShouldBeTrue();
    }
}
