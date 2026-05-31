#!/usr/bin/env dotnet
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Checking rule reference freshness...");

var rootDir = GetRootDirectory();
var analyzerDir = Path.Combine(rootDir, "packages", "Opinionated.Dotnet.CodingStandards", "pkgsrc", "config", "analyzers");
var committedFile = Path.Combine(rootDir, "docs", "rule-reference.md");

if (!File.Exists(committedFile))
{
    Console.Error.WriteLine("docs/rule-reference.md does not exist. Run scripts/GenerateRuleReference.cs to create it.");
    return 1;
}

var generated = GenerateContent(analyzerDir);
var committed = File.ReadAllText(committedFile);

if (generated != committed)
{
    Console.Error.WriteLine("docs/rule-reference.md is out of date.");
    Console.Error.WriteLine("Run scripts/GenerateRuleReference.cs to regenerate it, then commit the result.");
    return 1;
}

Console.WriteLine("docs/rule-reference.md is up to date.");
return 0;


static string GenerateContent(string analyzerDir)
{
    var files = Directory.GetFiles(analyzerDir, "*.editorconfig").OrderBy(Path.GetFileName).ToArray();
    var sb = new StringBuilder();

    sb.AppendLine("# Rule Reference");
    sb.AppendLine();
    sb.AppendLine("Rules enforced by `Opinionated.DotNet.CodingStandards`.");
    sb.AppendLine("Only rules with severity `warning`, `error`, or `suggestion` are listed.");
    sb.AppendLine("Rules set to `none` or `silent` are omitted.");
    sb.AppendLine();

    foreach (var file in files)
    {
        var analyzerName = Path.GetFileNameWithoutExtension(file).Replace("Analyzer.", "", StringComparison.Ordinal);
        var rules = ParseEnforcedRules(file);
        if (rules.Count == 0)
        {
            continue;
        }

        sb.AppendLine($"## {analyzerName}");
        sb.AppendLine();
        sb.AppendLine("| Rule ID | Description | Severity | Help |");
        sb.AppendLine("|---------|-------------|----------|------|");

        foreach (var rule in rules.OrderBy(r => r.Id, StringComparer.OrdinalIgnoreCase))
        {
            var helpCell = string.IsNullOrEmpty(rule.HelpLink) ? "" : $"[docs]({rule.HelpLink})";
            var desc = rule.Description.Replace("|", "\\|");
            sb.AppendLine($"| `{rule.Id}` | {desc} | {rule.Severity} | {helpCell} |");
        }

        sb.AppendLine();
    }

    return sb.ToString();
}

static List<(string Id, string Description, string Severity, string HelpLink)> ParseEnforcedRules(string filePath)
{
    var rules = new List<(string Id, string Description, string Severity, string HelpLink)>();
    var lines = File.ReadAllLines(filePath);
    var severityRegex = new Regex(@"^dotnet_diagnostic\.(.+?)\.severity\s*=\s*(\S+)$");

    for (var i = 0; i < lines.Length; i++)
    {
        var match = severityRegex.Match(lines[i]);
        if (!match.Success)
        {
            continue;
        }

        var id = match.Groups[1].Value;
        var severity = match.Groups[2].Value;

        if (severity is not ("warning" or "error" or "suggestion"))
        {
            continue;
        }

        var description = "";
        var helpLink = "";

        for (var j = i - 1; j >= 0; j--)
        {
            var commentLine = lines[j];
            if (!commentLine.StartsWith('#'))
            {
                break;
            }

            var text = commentLine.TrimStart('#').Trim();

            if (text.StartsWith($"{id}:", StringComparison.Ordinal))
            {
                description = text[(id.Length + 1)..].Trim();
            }
            else if (text.StartsWith("Help link:", StringComparison.Ordinal))
            {
                helpLink = text["Help link:".Length..].Trim();
            }
        }

        rules.Add((id, description, severity, helpLink));
    }

    return rules;
}

static string GetRootDirectory()
{
    var directory = Environment.CurrentDirectory;
    while (directory != null && !Directory.Exists(Path.Combine(directory, ".git")))
    {
        directory = Path.GetDirectoryName(directory);
    }

    return directory ?? throw new InvalidOperationException("Cannot find the root of the git repository");
}
