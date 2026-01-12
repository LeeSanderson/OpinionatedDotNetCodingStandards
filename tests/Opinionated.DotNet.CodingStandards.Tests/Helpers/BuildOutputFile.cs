using System.Text.Json.Serialization;

namespace Opinionated.DotNet.CodingStandards.Tests.Helpers;

internal sealed class BuildOutputFile
{
    [JsonPropertyName("runs")]
    public BuildOutputFileRun[]? Runs { get; set; }

    public IEnumerable<BuildOutputFileRunResult> AllResults() => Runs?.SelectMany(r => r.Results ?? []) ?? [];

    public bool HasError() => AllResults().Any(r => r.Level == "error");
    public bool HasError(string ruleId) => AllResults().Any(r => r.Level == "error" && r.RuleId == ruleId);
    public bool HasWarning(string ruleId) => AllResults().Any(r => r.Level == "warning" && r.RuleId == ruleId);
    public bool HasNote(string ruleId) => AllResults().Any(r => r.Level == "note" && r.RuleId == ruleId);

    internal sealed class BuildOutputFileRun
    {
        [JsonPropertyName("results")]
        public BuildOutputFileRunResult[]? Results { get; set; }
    }

    internal sealed class BuildOutputFileRunResult
    {
        [JsonPropertyName("ruleId")]
        public string? RuleId { get; set; }

        [JsonPropertyName("level")]
        public string? Level { get; set; }

        [JsonPropertyName("message")]
        public BuildOutputFileRunResultMessage? Message { get; set; }

        public override string ToString()
        {
            return $"{this.Level}:{this.RuleId} {this.Message}";
        }
    }

    internal sealed class BuildOutputFileRunResultMessage
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        public override string ToString()
        {
            return this.Text ?? "";
        }
    }
}