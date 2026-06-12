# 002 — Sonar gap analysis & overlap disposition (emit promotion issues)

## Parent PRD

`issues/prd.md`

## What to build

Produce the authoritative **disposition** that classifies every Sonar rule as
**enabled / disabled-twin / out-of-scope** under the "existing analyzer wins" policy, then
**emit one per-rule promotion issue file** for each enabled rule that is not already
pre-written (i.e. excluding the complexity anchors in `003`–`007`). This is the foundational
analysis every promotion slice depends on.

The disposition is anchored on the complexity metrics and extends to the broader
maintainability/design set (~50–100 enabled total); exact Sonar twins of rules already
enforced by NetAnalyzers / StyleCop / Meziantou / IDE stay at `none`; whole-program
symbolic-execution rules are de-prioritised on overlap and build-cost grounds. Opinionated
thresholds are chosen for the complexity rules.

Per PRD → Implementation Decisions, this slice **runs AFK within the policy** via the
repository's two-phase batch-orchestration workflow — parallel **read-only** investigators
draft dispositions, an adversarial skeptic verifies twin and untestable calls — with no human
approval gate. See PRD → Implementation Decisions (*Enabled set*, *Overlap policy*,
*Gap analysis*, *Production method*) and PRD → Further Notes (*Build-time cost*).

> Execution note: launching the batch-orchestration workflow is gated on explicit opt-in at
> execution time; respect the harness's git-worktree incompatibility (parallel agents
> read-only; all build/test verification serial on the main checkout).

## Acceptance criteria

- [ ] A disposition classifies **all** Sonar rules into enabled / disabled-twin /
      out-of-scope, applying "existing analyzer wins" (exact twins left at `none`).
- [ ] The enabled set (~50–100 rules) is centred on the complexity metrics plus the broader
      maintainability/design rules; whole-program symbolic-execution rules are de-prioritised.
- [ ] Opinionated thresholds are selected for the complexity rules (cognitive, cyclomatic,
      nesting depth, method length, parameter count) and recorded for slices `003`–`007`.
- [ ] Twin/overlap calls are adversarially verified — each "disabled because existing rule X
      owns it" decision is confirmed by a skeptic; untestable pre-assessments follow the
      "read the analyzer's report predicate first" bar.
- [ ] For each enabled rule **other than** the `003`–`007` anchors, a
      `issues/NNN-promote-<RuleId>.md` file is generated following the same template as the
      anchors, with `Blocked by` pointing at this issue.
- [ ] No per-rule rationale store is created for disabled rules (consistent with the existing
      StyleCop treatment); the overlap *policy* remains recorded in the PRD only.
- [ ] The Sonar editorconfig is **not** yet changed to enforce any rule here — promotion
      happens in the per-rule slices; this slice only decides and emits.

## Blocked by

- Blocked by `issues/001-wire-sonar-analyzer-disabled.md`

## User stories addressed

- User story 4
- User story 12
- User story 13
- User story 22
- User story 23
- User story 27
