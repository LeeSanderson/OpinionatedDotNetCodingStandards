#!/usr/bin/env dotnet
#:project ../tests/Opinionated.DotNet.CodingStandards.Tests/Opinionated.DotNet.CodingStandards.Tests.csproj

using System.Text;
using Opinionated.DotNet.CodingStandards.Tests;
using Opinionated.DotNet.CodingStandards.Tooling;

Console.WriteLine("Generating rule reference...");

var rootDir = GetRootDirectory();
var analyzerDir = Path.Combine(rootDir, "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "config", "analyzers");
var editorConfigPath = Path.Combine(rootDir, "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "config", "Opinionated.editorconfig");
var outputFile = Path.Combine(rootDir, "docs", "rule-reference.md");

var content = RuleReferenceGenerator.Generate(analyzerDir, typeof(RuleDocCoverageShould).Assembly, editorConfigPath);

Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
await File.WriteAllTextAsync(outputFile, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
Console.WriteLine($"Written: {outputFile}");
return 0;

static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}
