using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules;

[Collection(nameof(PackageCollection))]
public class CodeAnalysisRulesDesignShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("CA1000", "Do not declare static members on generic types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000")]
    public async Task ProhibitStaticMembersOnGenericTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class GenericClass<T>
            {
                public static void StaticMethod() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1000").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1001", "Types that own disposable fields should be disposable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1001")]
    public async Task RequireDisposableOnTypesOwningDisposableFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.IO;
            namespace test;
            public class MyClass
            {
                private readonly Stream _stream = new MemoryStream();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1001").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1003", "Use generic event handler instances",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1003")]
    public async Task RequireGenericEventHandlerInstances()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public delegate void MyEventHandler(object sender, EventArgs e);
            public class MyClass
            {
                public event MyEventHandler? Changed;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1003").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1008", "Enums should have zero value",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1008")]
    public async Task RequireEnumZeroValue()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public enum Status { Active = 1, Inactive = 2 }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1008").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1010", "Generic interface should also be implemented",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1010")]
    public async Task RequireGenericInterfaceImplementation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Collections;
            namespace test;
            public class MyCollection : IEnumerable
            {
                public IEnumerator GetEnumerator() => throw new System.NotImplementedException();
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1010").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1012", "Abstract types should not have public constructors",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1012")]
    public async Task ProhibitPublicConstructorsOnAbstractTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public abstract class AbstractBase
            {
                public AbstractBase() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1012").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1018", "Mark attributes with AttributeUsageAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1018")]
    public async Task RequireAttributeUsageOnAttributes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyAttribute : System.Attribute
            {
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1018").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1019", "Define accessors for attribute arguments",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1019")]
    public async Task RequireAccessorsForAttributeArguments()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            [AttributeUsage(AttributeTargets.Class)]
            public class MyAttribute : Attribute
            {
                public MyAttribute(string name) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1019").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1027", "Mark enums with FlagsAttribute",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1027")]
    public async Task RequireFlagsAttributeOnBitmaskEnums()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public enum Permissions { None = 0, Read = 1, Write = 2, Execute = 4 }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1027").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1028", "Enum Storage should be Int32",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1028")]
    public async Task RequireInt32EnumStorage()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public enum Color : byte { Red, Green, Blue }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1028").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1030", "Use events where appropriate",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1030")]
    public async Task RequireEventsForEventRaisingMethods()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public void RaiseMyEvent() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1033", "Interface methods should be callable by child types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1033")]
    public async Task RequireInterfaceMethodsCallableByChildTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IComparable
            {
                int IComparable.CompareTo(object? obj) => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1033").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1036", "Override methods on comparable types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1036")]
    public async Task RequireOverrideMethodsOnComparable()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IComparable
            {
                public int CompareTo(object? obj) => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1036").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1041", "Provide ObsoleteAttribute message",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1041")]
    public async Task RequireObsoleteAttributeMessage()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                [System.Obsolete]
                public static void OldMethod() { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1041").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1043", "Use Integral Or String Argument For Indexers",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1043")]
    public async Task RequireIntegralOrStringIndexers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyCollection
            {
                public int this[double key] => 0;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1043").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1044", "Properties should not be write only",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1044")]
    public async Task ProhibitWriteOnlyProperties()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                private int _value;
                public int Value { set { _value = value; } }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1044").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1046", "Do not overload equality operator on reference types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1046")]
    public async Task ProhibitEqualityOperatorOnReferenceTypes()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            #pragma warning disable CS0660, CS0661
            public class MyClass
            {
                public static bool operator ==(MyClass? a, MyClass? b) => false;
                public static bool operator !=(MyClass? a, MyClass? b) => true;
            }
            #pragma warning restore CS0660, CS0661
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1046").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1050", "Declare types in namespaces",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1050")]
    public async Task RequireTypesInNamespaces()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1050").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1051", "Do not declare visible instance fields",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1051")]
    public async Task ProhibitVisibleInstanceFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                public int Field;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1051").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1052", "Static holder types should be Static or NotInheritable",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1052")]
    public async Task RequireStaticClassForStaticOnlyMembers()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class Utilities
            {
                public static void DoWork() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1052").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1058", "Types should not extend certain base types",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1058")]
    public async Task ProhibitExtendingApplicationException()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyException : System.ApplicationException
            {
                public MyException(string message) : base(message) { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1058").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1063", "Implement IDisposable Correctly",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1063")]
    public async Task RequireCorrectIDisposableImplementation()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IDisposable
            {
                ~MyClass() { }
                public void Dispose() { }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1063").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1065", "Do not raise exceptions in unexpected locations",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1065")]
    public async Task ProhibitExceptionsInUnexpectedLocations()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class MyClass
            {
                static MyClass() { throw new System.InvalidOperationException("init failed"); }
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1065").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1067", "Override Object.Equals(object) when implementing IEquatable<T>",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1067")]
    public async Task RequireObjectEqualsWithIEquatable()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass : IEquatable<MyClass>
            {
                public bool Equals(MyClass? other) => true;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1067").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1068", "CancellationToken parameters must come last",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1068")]
    public async Task RequireCancellationTokenLast()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System.Threading;
            namespace test;
            public static class Program
            {
                public static void Method(CancellationToken token, int value) { }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1068").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1069", "Enums values should not be duplicated",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1069")]
    public async Task ProhibitDuplicateEnumValues()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public enum MyEnum
            {
                First = 1,
                Second = 1
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1069").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1070", "Do not declare event fields as virtual",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1070")]
    public async Task ProhibitVirtualEventFields()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            using System;
            namespace test;
            public class MyClass
            {
                public virtual event EventHandler? MyEvent;
            }
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1070").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1061", "Do not hide base class methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1061")]
    public async Task ProhibitHidingBaseClassMethodsWithLessDerivedType()
    {
        // The old probe failed for two independent reasons, both confirmed against the analyzer
        // source (DoNotHideBaseClassMethodsAnalyzer.GetMethodsHiddenByMethod):
        //   1. It declared the base method 'virtual'. The analyzer filters base candidates with
        //      '!(x.IsStatic || x.IsVirtual)', so a virtual base method can never be considered
        //      hidden -> CA1061 is structurally impossible on that shape.
        //   2. It marked the derived method 'new'. Because the parameter types differ, the C#
        //      compiler treats the two methods as overloads (not same-signature hiding), so 'new'
        //      is not required and CS0109 fires; under TreatWarningsAsErrors that preempts.
        // The real violation: a NON-virtual base method and a derived overload whose parameter is
        // LESS derived (base 'string' DerivesFrom derived 'object'), with matching name, return
        // type, parameter count and parameter names. No 'new' is needed (and adding it would emit
        // CS0109), and because the signatures differ the compiler emits neither CS0108 nor CS0109,
        // so the analyzer runs cleanly. CA1061 is RuleLevel.IdeSuggestion but the package raises it
        // to severity=warning, so it surfaces as an error under TreatWarningsAsErrors.
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public class Base { public void Method(string s) { } }
            public class Derived : Base { public void Method(object s) { } }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1061").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1066", "Implement IEquatable when overriding Object.Equals",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1066")]
    public async Task RequireIEquatableWhenOverridingObjectEquals()
    {
        // Root cause of the old probe's failure: CA1066 fires ONLY on a struct, never a class.
        // EquatableAnalyzer.AnalyzeSymbol reports ImplementIEquatableDescriptor under the guard
        // `overridesObjectEquals && !implementsEquatable && namedType.TypeKind == TypeKind.Struct`
        // (the class direction is covered by the converse rule CA1067). The old probe used a
        // `public class Point`, so the struct guard was never satisfied and CA1066 was absent
        // from SARIF. CA1066's descriptor is also RuleLevel.Disabled (IsEnabledByDefault = false),
        // but the package's editorconfig sets `dotnet_diagnostic.CA1066.severity = warning`, which
        // re-enables it; the only remaining requirement is the correct (struct) receiver shape.
        // TreatWarningsAsErrors promotes the warning to an error -> assert HasError.
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public struct Point
            {
                public int X { get; set; }
                public int Y { get; set; }
                public override bool Equals(object? obj) => obj is Point p && X == p.X && Y == p.Y;
                public override int GetHashCode() => System.HashCode.Combine(X, Y);
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1066").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("CA1016", "Mark assemblies with assembly version",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1016")]
    public async Task RequireAssemblyVersionAttribute()
    {
        // CA1016 (MarkAssembliesWithAttributesDiagnosticAnalyzer) fires at compilation end when no
        // System.Reflection.AssemblyVersionAttribute is present on the assembly. The SDK normally
        // auto-injects [assembly: AssemblyVersion] (GenerateAssemblyInfo=true by default), which
        // suppresses the rule; setting GenerateAssemblyInfo=false removes that attribute so the
        // assembly has no version attribute and CA1016 is reported. The package raises CA1016 to
        // severity=warning, so it surfaces as a SARIF error under TreatWarningsAsErrors.
        using var project = await CreateProjectBuilderAsync(properties:
        [
            (Name: "GenerateAssemblyInfo", Value: "false"),
        ]);
        await project.AddFileAsync(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("CA1016").ShouldBeTrue();
    }
}
