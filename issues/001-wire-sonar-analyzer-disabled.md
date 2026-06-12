# 001 — Wire SonarAnalyzer.CSharp in, fully disabled

## Parent PRD

`issues/prd.md`

## What to build

Add `SonarAnalyzer.CSharp` as a fifth bundled analyzer, wired end-to-end through the existing
analyzer-ingestion pipeline but with **every rule disabled** (`severity = none`), so the
solution build stays green and nothing new is enforced yet. This is the infrastructure tracer
bullet: dependency present → resolver discovers Sonar's DLLs → editorconfig generated listing
all rules → all normalized to `none` → rule-reference regenerated → docs updated → build and
tests green.

See PRD → Implementation Decisions (*Scope*, *Licensing*, *Dependency wiring*,
*Analyzer-resolution allow-map*, *Editorconfig seed*, *Documentation regeneration*) and
PRD → Further Notes (*Resolver-discovery checkpoint*, *Analyzer-bump idempotence*).

Components touched: the package manifest + central package-versions file; the
analyzer-resolution allow-map; the new per-analyzer Sonar editorconfig; the generated
rule-reference doc; README / AGENTS / CHANGELOG. No new code module — the single code change
is one allow-map entry.

## Acceptance criteria

- [ ] `SonarAnalyzer.CSharp` is added to **both** the package manifest's dependency list and
      the central package-versions file, version-matched to the latest stable (~`10.27.x`);
      the dependency-sync check passes.
- [ ] The analyzer-resolution allow-map maps the Sonar package id to
      `Analyzer.SonarAnalyzer.CSharp.editorconfig`.
- [ ] Running the editorconfig regeneration produces a **populated**
      `Analyzer.SonarAnalyzer.CSharp.editorconfig` containing the full Sonar rule set — this
      is the resolver-discovery checkpoint. If the file comes out empty, the resolver's
      DLL-layout handling must be investigated before any further slice proceeds.
- [ ] Every rule in the Sonar editorconfig is set to `none` (one-time normalization).
- [ ] Regenerating the rule-reference adds a `SonarAnalyzer.CSharp` section; because all rules
      are `none` it lists no enforced rules, and the rule-reference freshness check passes.
- [ ] README and AGENTS move from "four analyzers" to "five", list Sonar, and note its
      LGPL-3.0 license; the CHANGELOG `Added` section records the analyzer and its license.
- [ ] The full solution builds and all tests pass — the dogfooding build is green and **no new
      `[RuleDoc]` is required**, because no Sonar rule is active.
- [ ] Re-running the editorconfig and rule-reference regeneration produces **no diff**
      (analyzer-bump idempotence: curated `none` severities are preserved on regeneration).

## Blocked by

None - can start immediately.

## User stories addressed

- User story 5
- User story 7
- User story 8
- User story 9
- User story 10
- User story 11
- User story 15
- User story 16
- User story 17
- User story 19
- User story 20
- User story 21
- User story 26
