## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Correct the git-tracked path casing throughout the repository so the tracked tree is
self-consistent under a case-sensitive filesystem (see the PRD's Problem Statement and
Implementation Decisions sections). This means actually re-recording git's tracked paths — not
just editing file contents, which is what commit `416b0a4` did and why the bug is still present
on `main` — for:

- `Opinionated.Dotnet.CodingStandards.slnx` → `Opinionated.DotNet.CodingStandards.slnx`
- `src/Opinionated.Dotnet.CodingStandards/` (directory and its `.csproj`) →
  `src/Opinionated.DotNet.CodingStandards/`
- `packages/Opinionated.Dotnet.CodingStandards/` (directory, `.nuspec`, and all six
  `pkgsrc/{build,buildMultiTargeting,buildTransitive}/*.{props,targets}` files) →
  `packages/Opinionated.DotNet.CodingStandards/`

Because a case-only rename is invisible to git on this machine's case-insensitive checkout
(`core.ignorecase=true`), verify the result with `git ls-tree -r HEAD` — not `git status`/`git diff`,
which will report clean either way.

Also fix the three stray old-casing content references found during PRD investigation —
independent of the path rename above, but the same root typo:

- `README.md` — repository-layout diagram line
- `scripts/GenerateRuleReference.cs` — hardcoded `packages/Opinionated.Dotnet...` path
- `tests/Opinionated.DotNet.CodingStandards.Tests/RuleDocCoverageShould.cs` — hardcoded
  `packages/Opinionated.Dotnet...` path (load-bearing: left unfixed, this test breaks the moment
  the tracked directory casing above is corrected)

## Acceptance criteria

- [ ] `git ls-tree -r HEAD` shows `DotNet` casing consistently for the `.slnx`, the `src` project,
      and the entire `packages/` tree — no remaining `Dotnet` (lowercase `otnet`) tracked paths.
- [ ] `dotnet ./scripts/CheckImportPathCasing.cs` (from `issues/001`) now exits 0.
- [ ] The full existing test suite passes on Windows, including `RuleDocCoverageShould.cs` with its
      corrected path.
- [ ] `README.md`'s repository-layout diagram and `scripts/GenerateRuleReference.cs`'s hardcoded
      path both read `DotNet`.

## Blocked by

- Blocked by `issues/001-add-import-path-casing-check-script.md`

## User stories addressed

- User story 1
- User story 2
- User story 3
- User story 7
- User story 8
- User story 9
