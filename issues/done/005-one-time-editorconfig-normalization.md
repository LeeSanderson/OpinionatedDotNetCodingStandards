## Parent PRD

`issues/prd.md`

## What to build

Run the regeneration script (from slice 004) in write mode against the currently-pinned
analyzer versions, review the resulting diff, and commit the normalized editorconfig files.
See the PRD "One-time normalization" implementation decision and user story 18.

This is a HITL slice: the diff must be reviewed by a human to confirm that **no curated
`dotnet_diagnostic.<id>.severity` values changed** — the only changes should be comment
normalization and any genuinely new rules added at `warning` (which can occur if the committed
files were generated from older analyzer versions than the ones now pinned).

If new rules surface as `warning`, the coverage test (`RuleDocCoverageShould`) will fail until
each is resolved — either by adding a test + `[RuleDoc]`, deliberately downgrading the rule, or
recording it as untestable (a class-level `[RuleDoc]` with a reason in `UntestableRules`). That
resolution is part of completing this slice so the suite ends green.

After normalization, establish the idempotency guard: regenerating against the pinned versions
is a no-op (`--check` exits 0).

## Acceptance criteria

- [ ] The four editorconfig files are regenerated and committed in canonical form.
- [ ] The diff is reviewed; no curated severity value changed (verified line-by-line).
- [ ] Any newly-surfaced `warning` rules are resolved (test + `[RuleDoc]`, deliberate
      downgrade, or `UntestableRules` entry) so the full test suite is green.
- [ ] `--check` exits 0 after normalization (idempotency guard for the pinned versions).

## Blocked by

- Blocked by `issues/004-regeneration-script-and-check-mode.md`

## User stories addressed

- User story 1
- User story 7
- User story 18
- User story 19
