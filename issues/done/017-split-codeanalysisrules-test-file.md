## Parent PRD

`issues/prd.md`

## What to build

`CodeAnalysisRulesShould.cs` (4,368 lines) has grown too large to be navigable. Split it into
logically-grouped files under a `CodeAnalysisRules/` folder, then delete the original file.

### File organisation convention (applies to all split issues)

When a test file exceeds 1000 lines it is split into logically-grouped files placed in a folder
named after the original file (without `.cs`). Each split file and its class use the fully-qualified
name `<OriginalClass><Group>Should`. Namespace mirrors the folder structure.

### Target files

| File | Class | Rules | Est. lines |
|------|-------|-------|-----------|
| `CodeAnalysisRules/CodeAnalysisRulesDesignShould.cs` | `CodeAnalysisRulesDesignShould` | CA1000–CA1070 (30 rules) | ~733 |
| `CodeAnalysisRules/CodeAnalysisRulesGlobInteropMaintNamingShould.cs` | `CodeAnalysisRulesGlobInteropMaintNamingShould` | CA1304–CA1725 (23 rules) | ~557 |
| `CodeAnalysisRules/CodeAnalysisRulesPerformanceShould.cs` | `CodeAnalysisRulesPerformanceShould` | CA1805–CA1840 (25 rules) | ~604 |
| `CodeAnalysisRules/CodeAnalysisRulesPerformanceModernShould.cs` | `CodeAnalysisRulesPerformanceModernShould` | CA1841–CA1875 (23 rules) | ~604 |
| `CodeAnalysisRules/CodeAnalysisRulesReliabilityShould.cs` | `CodeAnalysisRulesReliabilityShould` | CA2002–CA2024 (13 rules) | ~365 |
| `CodeAnalysisRules/CodeAnalysisRulesUsageShould.cs` | `CodeAnalysisRulesUsageShould` | CA2101–CA2265 (36 rules) | ~964 |
| `CodeAnalysisRules/CodeAnalysisRulesSecurityShould.cs` | `CodeAnalysisRulesSecurityShould` | CA2352–CA2353 + CA5350–CA5403 (23 rules) | ~618 |

Note: CA2352 and CA2353 are currently placed among Usage rules in the original file but belong in
the Security category per Microsoft's classification. They move to `CodeAnalysisRulesSecurityShould`.

### Per-file conventions

- Namespace: `Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules`
- Class decoration: `[Collection(nameof(PackageCollection))]`, inherits `CodingStandardsTestBase`
- Tests sorted by rule ID within each file
- All `[RuleDoc]` attributes, positive tests, and negative tests for each rule travel together

## Acceptance criteria

- [ ] `CodeAnalysisRulesShould.cs` is deleted
- [ ] `CodeAnalysisRules/` contains exactly the 7 files listed above
- [ ] Each file declares its class in namespace `Opinionated.DotNet.CodingStandards.Tests.CodeAnalysisRules`
- [ ] Tests within each file are sorted by rule ID
- [ ] CA2352 and CA2353 appear in `CodeAnalysisRulesSecurityShould.cs`
- [ ] No file in `CodeAnalysisRules/` exceeds 1000 lines
- [ ] `dotnet test` passes with no changes to test logic
- [ ] Coverage test (`RuleDocCoverageShould`) remains green
