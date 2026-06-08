## Parent PRD

`issues/prd.md`

## What to build

Create a new, testable tooling library under `src/` (net10.0, not packable, referencing
`Microsoft.CodeAnalysis.CSharp`) and move the existing rule-reference core into it. See the
PRD "New tooling library" implementation decision.

End-to-end behavior: the rule-reference generator and its supporting types
(`RuleReferenceGenerator`, the `[RuleDoc]` attribute, the rule-doc entry record, and the
reconciliation result) live in the new library instead of the test project. The test project
references the library and continues to apply `[RuleDoc]` to its tests (adding the necessary
`using`). The `GenerateRuleReference.cs` script is re-pointed at the library. The new project
is registered in the solution.

This is the foundation slice — it adds no new feature behavior, but relocates shared code so
later slices (the editorconfig generator, the descriptor extractor) have a home.

## Acceptance criteria

- [ ] New `src/…Tooling` project exists (net10.0, not packable) and is in the solution file.
- [ ] `RuleReferenceGenerator`, `RuleDocAttribute`, `RuleDocEntry`, and `ReconciliationResult`
      now live in the library; the test project references it.
- [ ] All existing tests pass unchanged in behavior (notably `RuleDocCoverageShould` and the
      rule-reference generator test).
- [ ] Running `GenerateRuleReference.cs` produces a byte-identical `docs/rule-reference.md`
      (no diff) against the relocated library.
- [ ] `dotnet build` of the solution is clean (analyzer warnings-as-errors satisfied).

## Blocked by

None - can start immediately.

## User stories addressed

- User story 10
- User story 11
