## Parent PRD

`issues/prd.md`

## What to build

`CodingStandardsShould.cs` (1,709 lines) exceeds the 1000-line file-size convention. Split it
into logically-grouped files under a `CodingStandards/` folder, then delete the original file.

See issue 017 for the file-organisation convention that governs all split work.

### Target files

| File | Class | Rules | Est. lines |
|------|-------|-------|-----------|
| `CodingStandards/CodingStandardsStyleShould.cs` | `CodingStandardsStyleShould` | IDE0004–IDE0066 (33 rules) | ~839 |
| `CodingStandards/CodingStandardsModernSyntaxShould.cs` | `CodingStandardsModernSyntaxShould` | IDE0071–IDE2006 (33 rules) | ~900 |

`CodingStandardsStyleShould` covers expression-level preferences, modifier preferences, and
early code-cleanup rules. `CodingStandardsModernSyntaxShould` covers modern C# language features,
collection expressions, nullable handling, and whitespace rules (IDE2xxx).

### Per-file conventions

- Namespace: `Opinionated.DotNet.CodingStandards.Tests.CodingStandards`
- Class decoration: `[Collection(nameof(PackageCollection))]`, inherits `CodingStandardsTestBase`
- Tests sorted by rule ID within each file

## Acceptance criteria

- [ ] `CodingStandardsShould.cs` is deleted
- [ ] `CodingStandards/` contains exactly the 2 files listed above
- [ ] Each file declares its class in namespace `Opinionated.DotNet.CodingStandards.Tests.CodingStandards`
- [ ] Tests within each file are sorted by rule ID
- [ ] No file in `CodingStandards/` exceeds 1000 lines
- [ ] `dotnet test` passes with no changes to test logic
- [ ] Coverage test (`RuleDocCoverageShould`) remains green
