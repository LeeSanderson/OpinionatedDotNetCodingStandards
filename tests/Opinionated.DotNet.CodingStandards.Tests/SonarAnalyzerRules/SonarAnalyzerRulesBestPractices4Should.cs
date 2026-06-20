using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests.SonarAnalyzerRules;

public class SonarAnalyzerRulesBestPractices4Should(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("S6930", "Backslash should be avoided in route templates",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6930/")]
    public async Task WarnOnBackslashInRouteTemplate()
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

            [Route(@"api\controller")]
            public class HomeController : Controller { }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6930").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6932", "Use model binding instead of reading raw request data",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6932/")]
    public async Task WarnOnRawRequestDataAccess()
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
                [HttpGet]
                public IActionResult Index()
                {
                    var name = Request.Query["name"];   // S6932: use model binding instead
                    return Ok(name);
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6932").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6962", "Pool HTTP connections with HttpClientFactory — flags new HttpClient() created inside a public action method of an ASP.NET Core controller class (deriving from ControllerBase or Controller), where the instance is not stored for reuse (e.g. not a field initializer, not assigned to a static member, not in a constructor).",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6962/")]
    public async Task WarnOnHttpClientInstantiationInControllerAction()
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

            namespace test
            {
                public class HomeController : ControllerBase
                {
                    public string Get()
                    {
                        using var client = new System.Net.Http.HttpClient();
                        return client.GetStringAsync("https://example.com").Result;
                    }
                }

                public static class Program { public static int Main() => 0; }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6962").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S6967", "ModelState.IsValid should be called in controller actions",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-6967/")]
    public async Task WarnWhenControllerActionOmitsModelStateValidation()
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
            namespace test;
            using System.ComponentModel.DataAnnotations;
            using Microsoft.AspNetCore.Mvc;

            public class HomeController : ControllerBase
            {
                [HttpPost]
                public IActionResult Create([Required] int id)
                {
                    return Ok(id);
                }
            }

            public static class Program { public static int Main() => 0; }

            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S6967").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S8380", "Return types named \"partial\" should be escaped with \"@\"",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-8380/")]
    public async Task WarnOnUnescapedPartialReturnType()
    {
        using var project = await CreateProjectBuilderAsync(
            properties:
            [
                ("LangVersion", "13"),
            ]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class @partial { }
            public class C
            {
                public partial Method(int a) => null;  // S8380: partial is the return type
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S8380").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S8381", "\"scoped\" should be escaped when used as an identifier or type name in parenthesized lambda parameter lists",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-8381/")]
    public async Task WarnOnUnescapedScopedInLambdaParameter()
    {
        using var project = await CreateProjectBuilderAsync(
            properties:
            [
                ("LangVersion", "13"),
            ]);
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class @scoped { }
            public static class C
            {
                public static void Use(System.Action<@scoped> f) { }
                public static void Trigger() => Use((scoped a) => { });
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S8381").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S881", "Increment (++) and decrement (--) operators should not be used in a method call or mixed with other operators in an expression",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-881/")]
    public async Task ProhibitIncrementDecrementOperatorsInExpressions()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public static class Program
            {
                public static int Main()
                {
                    var i = 0;
                    var result = i++ + 1;
                    return result;
                }
            }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S881").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("S907", "goto statement should not be used",
        HelpLink = "https://rules.sonarsource.com/csharp/RSPEC-907/")]
    public async Task ProhibitGotoStatement()
    {
        using var project = await CreateProjectBuilderAsync();
        await project.AddFileAsync("Program.cs", """
            namespace test;
            public class C
            {
                public static int Compute(int x)
                {
                    goto end;
                    end:
                    return x;
                }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();

        buildOutput.HasError("S907").ShouldBeTrue();
    }
}
