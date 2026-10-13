## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that the analyzer packages have been bumped
(Meziantou.Analyzer 3.0.302, StyleCop.Analyzers 1.3.1-beta) and the analyzer editorconfigs have
been regenerated. The reference is generated from the editorconfig files and the test assembly's
`[RuleDoc]` attributes, so it must be regenerated **after** all per-rule issues are complete to
include the correct test links. This bump added no new rule IDs, so there are no per-rule issues;
the regeneration is still the gate that proves the reference is current before the PRD closes.

Note that `RuleReferenceGenerator` lists only *enforced* rules (severity other than `none`) and
takes each rule's description and help link from the test assembly's `[RuleDoc]` attribute, not
from the editorconfig comments. The regenerated StyleCop editorconfig changed only comments (help
links and the SA1413 title) on rules that are all set to `none` except `SA1649`, whose `[RuleDoc]`
is unchanged. So the expected outcome is **no diff** in `docs/rule-reference.md`; if the script
does produce a diff, inspect it, make sure it is explained by this bump, and commit it.

## Acceptance criteria

- [ ] `dotnet ./scripts/GenerateRuleReference.cs` has been run after the package bump
- [ ] `docs/rule-reference.md` is identical to the regenerated output (either no diff, or the
      diff has been reviewed and committed)
- [ ] `RuleDocCoverageShould` passes (`dotnet test --no-build -- --filter-class "*RuleDocCoverageShould*"`)

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
git status --porcelain docs/rule-reference.md
```

If the file is unchanged, there is nothing to commit for the docs; record that outcome. If it
changed, verify the diff is limited to metadata affected by this bump, then commit it.

## Blocked by

None — this PRD has no per-rule test issues, so this can start immediately.

## User stories addressed

- User story 3 (test suite remains green, rule reference is current, and package remains releasable)
