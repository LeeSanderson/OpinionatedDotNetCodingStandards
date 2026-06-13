using Opinionated.DotNet.CodingStandards.Tests.Helpers;
using Shouldly;
using Xunit.Abstractions;

namespace Opinionated.DotNet.CodingStandards.Tests;

[Collection(nameof(PackageCollection))]
public class BannedApiAnalyzersShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("RS0031", "The list of banned symbols contains a duplicate",
        HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md")]
    public async Task ReportDuplicateBannedSymbolEntry()
    {
        using var project = await CreateProjectBuilder(additionalFiles: ["BannedSymbols.txt"]);
        // P:System.DateTime.Now is already in the package's BannedSymbols.NonUtcDates.txt — adding it
        // again in a second file triggers RS0031.
        await project.AddFile("BannedSymbols.txt", "P:System.DateTime.Now;Duplicate entry");
        await project.AddFile("sample.cs", "class Program { static int Main() => 0; }");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0031").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("RS0030", "Do not use banned APIs",
        HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md")]
    public async Task BanNonUtcDates()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanNonUtcDatesWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanNonUtcDateApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = System.DateTime.Now;");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task NotBanInvariantCultureStringComparisonWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanInvariantCultureStringComparisonApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = "a test".IndexOf("test", StringComparison.InvariantCulture);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanInvariantCultureStringComparision()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", """_ = "a test".IndexOf("test", StringComparison.InvariantCulture);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanEnumTryParseWithoutIgnoreCaseWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanEnumTryParseWithoutIgnoreCaseApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = Enum.TryParse<StringComparison>("StringComparison.Ordinal", out _);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanEnumTryParseWithoutIgnoreCase()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile("sample.cs", """_ = Enum.TryParse<StringComparison>("StringComparison.Ordinal", out _);""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanRoundWithoutMidpointRoundingWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanRoundWithoutMidpointRoundingApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = Math.Round(0.4);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanRoundWithoutMidpointRoundingArgument()
    {
        using var project = await CreateProjectBuilder();

        // Replace with Math.Round(0.4, MidpointRounding.ToZero)
        await project.AddFile("sample.cs", "_ = Math.Round(0.4);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanUseOfCultureInfoConstructorWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanUseOfCultureInfoConstructorApis", Value: "false")]);
        await project.AddFile("sample.cs", """_ = new System.Globalization.CultureInfo("en-UK");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanUseOfCultureInfoConstructor()
    {
        using var project = await CreateProjectBuilder();

        // Replace with CultureInfo.GetCultureInfo("en-UK")
        await project.AddFile("sample.cs", """_ = new System.Globalization.CultureInfo("en-UK");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanUseOfTupleInFavourOfValueTupleWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(properties: [(Name: "BanUseOfTupleInFavourOfValueTupleApis", Value: "false")]);
        await project.AddFile("sample.cs", "_ = new Tuple<int, int>(1, 2);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanUseOfTupleInFavourOfValueTuple()
    {
        using var project = await CreateProjectBuilder();

        await project.AddFile("sample.cs", "_ = new Tuple<int, int>(1, 2);");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    public async Task NotBanUseOfNewtonsoftJsonWhenPropertyDisabled()
    {
        using var project = await CreateProjectBuilder(
            properties: [(Name: "BanUseOfNewtonsoftJsonApis", Value: "false")],
            packageReferences: [(Name: "Newtonsoft.Json", Version: "13.0.4")]);
        await project.AddFile("sample.cs", """_ = Newtonsoft.Json.JsonConvert.SerializeObject("test");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeFalse();
    }

    [Fact]
    public async Task BanUseOfNewtonSoftJson()
    {
        using var project = await CreateProjectBuilder(packageReferences: [(Name: "Newtonsoft.Json", Version: "13.0.4")]);

        await project.AddFile("sample.cs", """_ = Newtonsoft.Json.JsonConvert.SerializeObject("test");""");
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0030").ShouldBeTrue();
    }

    [Fact]
    [RuleDoc("RS0035", "External access to internal symbols outside the restricted namespace(s) is prohibited",
        HelpLink = "https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md")]
    public async Task ProhibitExternalAccessToInternalSymbolsOutsideRestrictedNamespace()
    {
        // A referenced library grants InternalsVisibleTo to the consumer (so GivesAccessTo is true and the
        // consumer can compile against the internal), but uses [RestrictedInternalsVisibleTo] to allow only
        // the "Lib.Api" namespace. The internal type lives in "Lib.Internal", so the consumer accessing it
        // from a referenced assembly outside the allowed namespace fires RS0035.
        using var project = new ProjectBuilder(Fixture, TestOutputHelper);

        await project.AddFile("lib/lib.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
                <AssemblyName>RestrictedLib</AssemblyName>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            </Project>
            """);

        await project.AddFile("lib/Lib.cs",
            """
            [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Consumer")]
            [assembly: System.Runtime.CompilerServices.RestrictedInternalsVisibleTo("Consumer", "Lib.Api")]

            namespace System.Runtime.CompilerServices
            {
                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
                internal sealed class RestrictedInternalsVisibleToAttribute : Attribute
                {
                    public RestrictedInternalsVisibleToAttribute(string assemblyName, params string[] restrictedToNamespaces)
                    {
                    }
                }
            }

            namespace Lib.Internal
            {
                internal static class Secret
                {
                    internal static int Value => 42;
                }
            }
            """);

        await project.AddFile("test.csproj",
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>exe</OutputType>
                <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
                <AssemblyName>Consumer</AssemblyName>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
                <ErrorLog>BuildOutput.sarif,version=2.1</ErrorLog>
              </PropertyGroup>
              <ItemGroup>
                <!-- test.csproj sits at the root, so the SDK's default **/*.cs glob would otherwise
                     pull lib/Lib.cs (and lib/obj generated files) into the consumer compilation,
                     producing CS0436 type clashes and analyzing the lib source. Exclude it so the
                     lib is consumed purely as a referenced assembly. -->
                <Compile Remove="lib/**/*.cs" />
              </ItemGroup>
              <ItemGroup>
                <PackageReference Include="Opinionated.DotNet.CodingStandards" Version="*" />
              </ItemGroup>
              <ItemGroup>
                <ProjectReference Include="lib/lib.csproj" />
              </ItemGroup>
            </Project>
            """);

        await project.AddFile("Program.cs", "_ = Lib.Internal.Secret.Value;");

        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("RS0035").ShouldBeTrue();
    }
}