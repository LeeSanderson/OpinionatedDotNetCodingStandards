using System.Text;
using System.Text.RegularExpressions;

namespace Opinionated.DotNet.CodingStandards.Tooling;

public static class EditorConfigMergeGenerator
{
    public static EditorConfigMergeResult Generate(string existingText, IReadOnlyList<RuleDescriptor> descriptors)
    {
        var (header, existingBlocks) = ParseExistingFile(existingText);
        var descriptorMap = descriptors.ToDictionary(d => d.Id, d => d, StringComparer.OrdinalIgnoreCase);

        var addedIds = descriptors
            .Where(d => !existingBlocks.ContainsKey(d.Id))
            .Select(d => d.Id)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var staleIds = existingBlocks.Keys
            .Where(id => !descriptorMap.ContainsKey(id))
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var allIds = descriptorMap.Keys
            .Concat(staleIds)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var sb = new StringBuilder();
        sb.Append(header);

        foreach (var id in allIds)
        {
            sb.Append("\n\n");

            if (descriptorMap.TryGetValue(id, out var descriptor))
            {
                var curatedSeverity = existingBlocks.TryGetValue(id, out var existingBlock)
                    ? existingBlock.CuratedSeverity
                    : "warning";

                sb.Append($"# {descriptor.Id}: {descriptor.Title}");
                if (!string.IsNullOrEmpty(descriptor.HelpLink))
                {
                    sb.Append($"\n# Help link: {descriptor.HelpLink}");
                }

                sb.Append($"\n# Enabled: {descriptor.EnabledByDefault}, Severity: {ToSeverityWord(descriptor.DefaultSeverity)}");
                sb.Append($"\ndotnet_diagnostic.{descriptor.Id}.severity = {curatedSeverity}");
            }
            else
            {
                sb.Append(string.Join("\n", existingBlocks[id].RawLines));
            }
        }

        sb.Append('\n');

        return new EditorConfigMergeResult(sb.ToString(), addedIds, staleIds);
    }

    private static (string Header, Dictionary<string, ParsedRuleBlock> Blocks) ParseExistingFile(string text)
    {
        var lines = text.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n').ToList();
        var severityPattern = new Regex(@"^dotnet_diagnostic\.(.+?)\.severity\s*=\s*(\S+)$");
        var blocks = new Dictionary<string, ParsedRuleBlock>(StringComparer.OrdinalIgnoreCase);
        int? firstBlockStart = null;

        for (var i = 0; i < lines.Count; i++)
        {
            var match = severityPattern.Match(lines[i]);
            if (!match.Success)
            {
                continue;
            }

            var id = match.Groups[1].Value;
            var severity = match.Groups[2].Value;

            var blockStart = i;
            while (blockStart > 0 && lines[blockStart - 1].StartsWith('#'))
            {
                blockStart--;
            }

            firstBlockStart ??= blockStart;

            var rawLines = lines.Skip(blockStart).Take(i - blockStart + 1).ToList();
            blocks[id] = new ParsedRuleBlock(id, severity, rawLines);
        }

        var headerEnd = firstBlockStart ?? lines.Count;
        var headerLines = lines.Take(headerEnd).ToList();
        while (headerLines.Count > 0 && string.IsNullOrEmpty(headerLines[^1]))
        {
            headerLines.RemoveAt(headerLines.Count - 1);
        }

        return (string.Join("\n", headerLines), blocks);
    }

    private static string ToSeverityWord(RuleDefaultSeverity severity) => severity switch
    {
        RuleDefaultSeverity.Hidden => "silent",
        RuleDefaultSeverity.Info => "suggestion",
        RuleDefaultSeverity.Warning => "warning",
        RuleDefaultSeverity.Error => "error",
        _ => "silent",
    };

    private record ParsedRuleBlock(string Id, string CuratedSeverity, List<string> RawLines);
}
