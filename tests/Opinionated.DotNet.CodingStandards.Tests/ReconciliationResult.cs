namespace Opinionated.DotNet.CodingStandards.Tests;

public record ReconciliationResult(
    IReadOnlyList<string> UncoveredRules,
    IReadOnlyList<string> OrphanDocs,
    IReadOnlyList<string> DuplicateIds,
    IReadOnlyList<string> InvariantViolations);
