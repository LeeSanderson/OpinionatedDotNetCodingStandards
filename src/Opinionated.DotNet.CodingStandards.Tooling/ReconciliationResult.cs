namespace Opinionated.DotNet.CodingStandards.Tooling;

public record ReconciliationResult(
    IReadOnlyList<string> UncoveredRules,
    IReadOnlyList<string> OrphanDocs,
    IReadOnlyList<string> DuplicateIds,
    IReadOnlyList<string> InvariantViolations);
