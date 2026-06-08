## Parent PRD

`issues/prd.md`

## What to build

Add a CI workflow that regenerates the editorconfigs (and the rule reference) on analyzer
version-bump PRs and commits the result back, so new rules and the resulting test failure
surface automatically. See the PRD "Automation around the tool" implementation decision. The
repository has no CI today, so this slice introduces the workflow from scratch.

End-to-end behavior: a workflow triggers on PRs that touch the package-version files
(`Directory.Packages.props` / the `.nuspec`). It restores, runs `UpdateAnalyzerEditorConfigs`
in write mode and `GenerateRuleReference`, commits any changes back to the PR branch using a bot
identity with a skip-when-unchanged guard (no commit/loop when nothing changed), then builds and
runs the tests. The resulting test failure on uncovered new rules is the intended signal; the
job log surfaces the added/stale summary.

This is HITL: it needs the appropriate write-back token/permissions configured and verification
on a real PR, and there is no existing CI to model on.

## Acceptance criteria

- [ ] A workflow triggers on PRs changing `Directory.Packages.props` or the `.nuspec`.
- [ ] It restores, regenerates the editorconfigs + rule reference, and commits changes back to
      the PR branch under a bot identity.
- [ ] When regeneration produces no changes, no commit is made (no loop / no empty commit).
- [ ] After regen, it builds and runs the tests; a bump that introduces new rules produces a
      failing coverage test on the PR.
- [ ] The job log shows the per-file added/stale summary.

## Blocked by

- Blocked by `issues/005-one-time-editorconfig-normalization.md`

## User stories addressed

- User story 15
- User story 16
- User story 17
- User story 19
