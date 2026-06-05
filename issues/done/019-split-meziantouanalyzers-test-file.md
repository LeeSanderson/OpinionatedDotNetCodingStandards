## Parent PRD

`issues/prd.md`

## What to build

`MeziantouAnalyzersShould.cs` (1,095 lines) exceeds the 1000-line file-size convention. Split it
into two files under a `MeziantouAnalyzers/` folder, then delete the original file.

See issue 017 for the file-organisation convention that governs all split work.

There is no meaningful semantic boundary within the MA rule set; the two files are distinguished
by rule-number range.

### Target files

| File | Class | Rules | Est. lines |
|------|-------|-------|-----------|
| `MeziantouAnalyzers/MeziantouAnalyzersCoreShould.cs` | `MeziantouAnalyzersCoreShould` | MA0015–MA0079 (22 rules) | ~470 |
| `MeziantouAnalyzers/MeziantouAnalyzersExtendedShould.cs` | `MeziantouAnalyzersExtendedShould` | MA0082–MA0178 (30 rules) | ~655 |

### Per-file conventions

- Namespace: `Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers`
- Class decoration: `[Collection(nameof(PackageCollection))]`, inherits `CodingStandardsTestBase`
- Tests sorted by rule ID within each file

## Acceptance criteria

- [ ] `MeziantouAnalyzersShould.cs` is deleted
- [ ] `MeziantouAnalyzers/` contains exactly the 2 files listed above
- [ ] Each file declares its class in namespace `Opinionated.DotNet.CodingStandards.Tests.MeziantouAnalyzers`
- [ ] Tests within each file are sorted by rule ID
- [ ] No file in `MeziantouAnalyzers/` exceeds 1000 lines
- [ ] `dotnet test` passes with no changes to test logic
- [ ] Coverage test (`RuleDocCoverageShould`) remains green
