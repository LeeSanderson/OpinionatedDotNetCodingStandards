## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that the analyzer packages have been bumped
(Meziantou.Analyzer 3.0.115 → 3.0.121, Microsoft.CodeAnalysis.BannedApiAnalyzers 4.14.0 → 5.6.0,
SonarAnalyzer.CSharp 10.27.0.140913 → 10.28.0.143324). This bump introduced no new rule IDs, but
several SonarAnalyzer rule titles/descriptions changed upstream, so the reference doc needs to be
regenerated to pick up any resulting text changes and confirm the doc still matches the current
`[RuleDoc]` coverage.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated
- [ ] The file is committed
- [ ] `dotnet build Opinionated.DotNet.CodingStandards.slnx` still succeeds with 0 warnings/errors

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Review the diff in `docs/rule-reference.md` (expect only cosmetic rule-title text changes from
the SonarAnalyzer.CSharp bump, if any), then commit.

## Blocked by

None — this PRD introduced no per-rule test issues (the package bump added zero new rule IDs),
so this is the only issue and can start immediately.

## User stories addressed

- User story 2 (regenerate the rule reference doc so changed rule titles/descriptions are
  reflected accurately)
- User story 3 (test suite remains green and package remains releasable)
