using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Opinionated.DotNet.CodingStandards.Tests;

public static class RuleReferenceGenerator
{
    public static Dictionary<string, RuleDocAttribute> CollectRuleDocs(Assembly assembly)
    {
        var docs = new Dictionary<string, RuleDocAttribute>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in assembly.GetTypes())
        {
            foreach (var attr in type.GetCustomAttributes<RuleDocAttribute>())
            {
                docs[attr.RuleId] = attr;
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                foreach (var attr in method.GetCustomAttributes<RuleDocAttribute>())
                {
                    docs[attr.RuleId] = attr;
                }
            }
        }

        return docs;
    }

    public static string Generate(string analyzerDir, Assembly testAssembly)
    {
        var ruleDocs = CollectRuleDocs(testAssembly);
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
                string description;
                string helpLink;

                if (ruleDocs.TryGetValue(rule.Id, out var doc))
                {
                    description = doc.Description;
                    helpLink = doc.HelpLink ?? "";
                }
                else
                {
                    description = rule.Description;
                    helpLink = rule.HelpLink;
                }

                var helpCell = string.IsNullOrEmpty(helpLink) ? "" : $"[docs]({helpLink})";
                var desc = description.Replace("|", "\\|");
                sb.AppendLine($"| `{rule.Id}` | {desc} | {rule.Severity} | {helpCell} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static List<(string Id, string Description, string Severity, string HelpLink)> ParseEnforcedRules(string filePath)
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
}
