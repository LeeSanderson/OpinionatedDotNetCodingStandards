// Copyright (c) Codurance. All rights reserved.

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Opinionated.DotNet.CodingStandards.Tooling;

public static class RuleReferenceGenerator
{
    public static IReadOnlySet<string> CollectActiveRules(string analyzerDir)
        => CollectActiveRules(analyzerDir, null);

    public static IReadOnlySet<string> CollectActiveRules(string analyzerDir, string? editorConfigPath)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.GetFiles(analyzerDir, "*.editorconfig"))
        {
            foreach (var (id, _, _, _) in ParseEnforcedRules(file))
            {
                result.Add(id);
            }
        }

        if (editorConfigPath != null)
        {
            foreach (var (id, _, _, _) in ParseEnforcedRules(editorConfigPath))
            {
                result.Add(id);
            }
        }

        return result;
    }

    public static IReadOnlyList<RuleDocEntry> CollectRuleDocEntries(Assembly assembly)
    {
        var entries = new List<RuleDocEntry>();
        foreach (var type in assembly.GetTypes())
        {
            foreach (var attr in type.GetCustomAttributes<RuleDocAttribute>())
            {
                entries.Add(new RuleDocEntry(attr.RuleId, attr, IsClassLevel: true));
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                foreach (var attr in method.GetCustomAttributes<RuleDocAttribute>())
                {
                    entries.Add(new RuleDocEntry(attr.RuleId, attr, IsClassLevel: false));
                }
            }
        }

        return entries;
    }

    public static ReconciliationResult Reconcile(
        string analyzerDir, Assembly testAssembly)
        => Reconcile(analyzerDir, testAssembly, null);

    public static ReconciliationResult Reconcile(
        string analyzerDir, Assembly testAssembly, string? editorConfigPath)
    {
        var activeRules = CollectActiveRules(analyzerDir, editorConfigPath);
        var docEntries = CollectRuleDocEntries(testAssembly);
        return Reconcile(activeRules, docEntries);
    }

    public static ReconciliationResult Reconcile(
        IReadOnlySet<string> activeRules,
        IReadOnlyList<RuleDocEntry> docEntries)
    {
        var idCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in docEntries)
        {
            idCounts.TryGetValue(entry.RuleId, out var n);
            idCounts[entry.RuleId] = n + 1;
        }

        var duplicateIds = idCounts
            .Where(kvp => kvp.Value > 1)
            .Select(kvp => kvp.Key)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var orphanDocs = docEntries
            .Where(e => !activeRules.Contains(e.RuleId))
            .Select(e => e.RuleId)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var coveredByDoc = new HashSet<string>(docEntries.Select(e => e.RuleId), StringComparer.OrdinalIgnoreCase);

        var uncoveredRules = activeRules
            .Where(r => !coveredByDoc.Contains(r))
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var invariantViolations = new List<string>();
        foreach (var entry in docEntries)
        {
            if (entry.IsClassLevel && string.IsNullOrEmpty(entry.Doc.Untestable))
            {
                invariantViolations.Add($"{entry.RuleId}: class-level [RuleDoc] has empty/null Untestable");
            }
        }

        return new ReconciliationResult(uncoveredRules, orphanDocs, duplicateIds, invariantViolations);
    }

    public static Dictionary<string, RuleDocAttribute> CollectRuleDocs(Assembly assembly)
    {
        var docs = new Dictionary<string, RuleDocAttribute>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in assembly.GetTypes())
        {
            foreach (var attr in type.GetCustomAttributes<RuleDocAttribute>())
            {
                docs[attr.RuleId] = attr;
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
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
        => Generate(analyzerDir, testAssembly, null);

    public static string Generate(string analyzerDir, Assembly testAssembly, string? editorConfigPath)
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
                AppendRuleRow(sb, rule, ruleDocs);
            }

            sb.AppendLine();
        }

        if (editorConfigPath != null)
        {
            var editorRules = ParseEnforcedRules(editorConfigPath);
            if (editorRules.Count > 0)
            {
                sb.AppendLine("## IDE / editor rules");
                sb.AppendLine();
                sb.AppendLine("These rules are configured in the IDE tier. Build enforcement varies: some fire");
                sb.AppendLine("during `dotnet build`, others are IDE-only and not emitted by Roslyn build analyzers.");
                sb.AppendLine();
                sb.AppendLine("| Rule ID | Description | Severity | Help |");
                sb.AppendLine("|---------|-------------|----------|------|");

                foreach (var rule in editorRules.OrderBy(r => r.Id, StringComparer.OrdinalIgnoreCase))
                {
                    AppendRuleRow(sb, rule, ruleDocs);
                }

                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static void AppendRuleRow(
        StringBuilder sb,
        (string Id, string Description, string Severity, string HelpLink) rule,
        Dictionary<string, RuleDocAttribute> ruleDocs)
    {
        ruleDocs.TryGetValue(rule.Id, out var doc);
        var description = doc?.Description ?? "";
        var helpLink = doc?.HelpLink ?? "";

        var helpCell = string.IsNullOrEmpty(helpLink) ? "" : $"[docs]({helpLink})";
        var desc = description.Replace("|", "\\|");
        sb.AppendLine($"| `{rule.Id}` | {desc} | {rule.Severity} | {helpCell} |");
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

            for (var j = i - 1; j >= 0 && lines[j].StartsWith('#'); j--)
            {
                var text = lines[j].TrimStart('#').Trim();

                if (text.StartsWith($"{id}:", StringComparison.Ordinal))
                {
                    description = text[(id.Length + 1)..].Trim();
                }
                else if (text.StartsWith("Help link:", StringComparison.Ordinal))
                {
                    helpLink = text["Help link:".Length..].Trim();
                }
                else
                {
                    // Other comment lines (e.g. "Enabled:" metadata) are not extracted
                }
            }

            rules.Add((id, description, severity, helpLink));
        }

        return rules;
    }
}
