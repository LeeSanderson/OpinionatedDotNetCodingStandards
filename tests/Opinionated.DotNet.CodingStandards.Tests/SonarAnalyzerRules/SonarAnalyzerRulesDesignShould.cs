// Copyright (c) Codurance. All rights reserved.

using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

[Collection(nameof(PackageCollection))]
public class SonarAnalyzerRulesDesignShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S110", "Inheritance tree of classes should not be too deep",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-110/")]
    public async Task ProhibitExcessiveInheritanceDepth()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class A { }
            public class B : A { }
            public class C : B { }
            public class D : C { }
            public class E : D { }
            public class F : E { }
            public class G : F { }
            public class H : G { }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S110").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1185", "Overriding members should do more than simply call the same member in the base class",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1185/")]
    public async Task WarnOnTrivialPassThroughOverride()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Base
            {
                public virtual int Compute(int x) => x * 2;
                public virtual string Describe(string s) => s;
            }

            public class Derived : Base
            {
                public override int Compute(int x) => base.Compute(x);
                public override string Describe(string s) => base.Describe(s);
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1185").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1200", "Classes should not be coupled to too many other classes",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1200/")]
    public async Task ProhibitExcessiveClassCoupling()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public sealed class A01 { public int Value; }
            public sealed class A02 { public int Value; }
            public sealed class A03 { public int Value; }
            public sealed class A04 { public int Value; }
            public sealed class A05 { public int Value; }
            public sealed class A06 { public int Value; }
            public sealed class A07 { public int Value; }
            public sealed class A08 { public int Value; }
            public sealed class A09 { public int Value; }
            public sealed class A10 { public int Value; }
            public sealed class A11 { public int Value; }
            public sealed class A12 { public int Value; }
            public sealed class A13 { public int Value; }
            public sealed class A14 { public int Value; }
            public sealed class A15 { public int Value; }
            public sealed class A16 { public int Value; }
            public sealed class A17 { public int Value; }
            public sealed class A18 { public int Value; }
            public sealed class A19 { public int Value; }
            public sealed class A20 { public int Value; }
            public sealed class A21 { public int Value; }
            public sealed class A22 { public int Value; }
            public sealed class A23 { public int Value; }
            public sealed class A24 { public int Value; }
            public sealed class A25 { public int Value; }
            public sealed class A26 { public int Value; }
            public sealed class A27 { public int Value; }
            public sealed class A28 { public int Value; }
            public sealed class A29 { public int Value; }
            public sealed class A30 { public int Value; }
            public sealed class A31 { public int Value; }

            // References 31 distinct non-primitive types: exceeds the default threshold of 30
            public sealed class Coupled
            {
                private static readonly System.Type T01 = typeof(A01);
                private static readonly System.Type T02 = typeof(A02);
                private static readonly System.Type T03 = typeof(A03);
                private static readonly System.Type T04 = typeof(A04);
                private static readonly System.Type T05 = typeof(A05);
                private static readonly System.Type T06 = typeof(A06);
                private static readonly System.Type T07 = typeof(A07);
                private static readonly System.Type T08 = typeof(A08);
                private static readonly System.Type T09 = typeof(A09);
                private static readonly System.Type T10 = typeof(A10);
                private static readonly System.Type T11 = typeof(A11);
                private static readonly System.Type T12 = typeof(A12);
                private static readonly System.Type T13 = typeof(A13);
                private static readonly System.Type T14 = typeof(A14);
                private static readonly System.Type T15 = typeof(A15);
                private static readonly System.Type T16 = typeof(A16);
                private static readonly System.Type T17 = typeof(A17);
                private static readonly System.Type T18 = typeof(A18);
                private static readonly System.Type T19 = typeof(A19);
                private static readonly System.Type T20 = typeof(A20);
                private static readonly System.Type T21 = typeof(A21);
                private static readonly System.Type T22 = typeof(A22);
                private static readonly System.Type T23 = typeof(A23);
                private static readonly System.Type T24 = typeof(A24);
                private static readonly System.Type T25 = typeof(A25);
                private static readonly System.Type T26 = typeof(A26);
                private static readonly System.Type T27 = typeof(A27);
                private static readonly System.Type T28 = typeof(A28);
                private static readonly System.Type T29 = typeof(A29);
                private static readonly System.Type T30 = typeof(A30);
                private static readonly System.Type T31 = typeof(A31);
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1200").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1450", "Private fields only used as local variables in methods should become local variables",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1450/")]
    public async Task DetectPrivateFieldUsedOnlyAsLocalVariable()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class C
            {
                private int _value;

                public void Method()
                {
                    _value = 42;
                    System.Console.Write(_value);
                }
            }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1450").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1694", "An abstract class should have both abstract and concrete methods",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1694/")]
    public async Task WarnOnAbstractClassWithOnlyAbstractMethods()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public abstract class AllAbstract
            {
                public abstract void MethodOne();
                public abstract void MethodTwo();
                public abstract int MethodThree();
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1694").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S1939", "Inheritance list should not be redundant",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-1939/")]
    public async Task WarnOnRedundantInheritanceListEntry()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public static class Program { public static int Main() => 0; }

            public interface IFoo { void DoWork(); }

            public class Base : IFoo { public void DoWork() { } }

            public class Derived : Base, IFoo { }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S1939").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2094", "Classes should not be empty",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2094/")]
    public async Task WarnOnEmptyClass()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;

            public class Empty { }

            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2094").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2156", "“sealed” classes should not have “protected” members",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2156/")]
    public async Task ProhibitProtectedMembersInSealedClass()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public sealed class MySealedClass
            {
                protected int Value { get; set; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2156").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S2292", "Trivial properties should be auto-implemented",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-2292/")]
    public async Task WarnOnTrivialPropertyWithBackingField()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("Program.cs", """
            namespace test;
            public class Sample
            {
                private int _value;
                public int Value
                {
                    get { return _value; }
                    set { _value = value; }
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("S2292").ShouldBeTrue();
    }
}
