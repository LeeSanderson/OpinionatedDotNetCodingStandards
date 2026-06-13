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
}
