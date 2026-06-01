## Parent PRD

`issues/prd.md`

## What to build

Backfill the two tiny analyzer groups to full coverage. See PRD "User Stories" 2/7/15 and the
backfill approach in "Further Notes". Cover the remaining BannedApiAnalyzers rules `RS0031` and
`RS0035` (`RS0030` is already tagged in `issues/005`) and the single StyleCop rule `SA1649`. Probe
each: write a positive test + canonical `[RuleDoc]` for those the build harness can trigger; for any
that cannot (e.g. `RS0035` cross-assembly internal access), add an `UntestableRules` entry with a
written reason. Remove all three from the `KnownUncovered` allowlist.

## Acceptance criteria

- [ ] `RS0031`, `RS0035`, and `SA1649` each have a canonical `[RuleDoc]` (positive test or reasoned `UntestableRules` entry).
- [ ] New positive tests follow the existing harness shape (build a sample, assert the diagnostic).
- [ ] All three are removed from the allowlist.
- [ ] The regenerated doc reflects the attribute-sourced descriptions; coverage test and `dotnet test` are green.

## Blocked by

- Blocked by `issues/002-coverage-test-and-allowlist.md`
- Blocked by `issues/004-editor-tier-section-and-untestable-rules.md`

## User stories addressed

- User story 2
- User story 7
- User story 15
