## Problem Statement

Every published version of `Opinionated.DotNet.CodingStandards` (`0.0.1` through `0.0.7`) fails to
build on case-sensitive filesystems (e.g. `ubuntu-latest` GitHub Actions runners) with `MSB4019`.
The root cause is a casing mismatch between a file name shipped inside the package and the
`<Import Project="...">` path that references it: the import says `DotNet`, the shipped file is
named `Dotnet`. NTFS/APFS resolve this case-insensitively, so it builds fine on Windows/macOS —
the maintainer's own dev machine and this repo's own CI/release runners (`windows-latest`) never
see the failure. A consumer only discovers it when their own CI happens to run on Linux.

GitHub issue #20 reported this and assumed it was already fixed on `main` by commit `416b0a4`
("Standardize on DotNet casing throughout repository"), just never released. Investigation proved
that assumption wrong: `416b0a4` rewrote file *contents* to reference the new `DotNet` casing, but
because it was authored on a case-insensitive filesystem (`core.ignorecase=true`), the case-only
directory/file *renames* were never actually recorded in git's tracked tree. `git ls-tree -r HEAD`
today still shows the old `Dotnet` casing for the solution file, the main `src` project, and the
entire `packages/` payload — while those same tracked files' contents now point at the `DotNet`
paths. The bug is still present on `main`, just inverted, and is invisible to every check this repo
currently runs because none of them use a case-sensitive filesystem.

Three more stray old-casing string references (same root typo, independent of the git-tracked path
issue) were also found during investigation: a cosmetic line in `README.md`, a hardcoded path in
`scripts/GenerateRuleReference.cs`, and — critically — a hardcoded path in an actual test
(`RuleDocCoverageShould.cs`) that would itself fail once the path casing is corrected, if left
unfixed.

There is currently no mechanism, anywhere in this repository's tooling or CI, that can detect an
import-path casing mismatch before a consumer's case-sensitive build breaks.

## Solution

1. Correct the git-tracked path casing for every affected file (the `.slnx`, the `src` project
   directory and `.csproj`, and the entire `packages/` payload directory including the `.nuspec`
   and all six `.props`/`.targets` files), verified against `git ls-tree` — not `git status`, which
   cannot detect this class of problem on a case-insensitive checkout.
2. Fix the three stray old-casing string references found during investigation.
3. Add a new, dedicated verification script (following this repo's existing `scripts/Check*.cs`
   convention) that parses every `<Import Project="...">` in the repo's `.props`/`.targets` files
   and confirms each resolves via an **exact, case-sensitive** string comparison against the actual
   directory entries — not `File.Exists`, which is case-insensitive on Windows and would miss the
   exact bug this PRD fixes. This runs in seconds, on any OS, and directly targets the root cause
   rather than waiting for a full build to incidentally surface it.
4. Add a new `ubuntu-latest` job to `ci.yml` mirroring the existing `windows-latest` job (full
   `dotnet restore`/`build`/`test`/`pack`), as a broader belt-and-suspenders regression guard for
   any future cross-platform packaging issue, not just casing.
5. Wire the new script into `scripts/New-ReleaseTag.ps1`'s existing pre-flight checks, alongside
   `CheckNugetDependenciesMatchProps.cs` and `GenerateRuleReference.cs --check`, so a release can
   never be cut while this class of bug is present.
6. Document the fix in `CHANGELOG.md` and cut a patch release (`v0.0.8`) once merged and green.

## User Stories

1. As a consumer building on Linux CI, I want the package's shipped MSBuild imports to resolve
   correctly on a case-sensitive filesystem, so that my build doesn't fail with `MSB4019`.
2. As a consumer building on Windows or macOS, I want the fix to be invisible to me — no behavior
   change, no new required configuration — so that upgrading is a drop-in version bump.
3. As the maintainer, I want `git ls-tree` (not `git status`) to be the source of truth I check
   when verifying a path-casing fix, so that I don't repeat the same false-negative that let this
   bug persist through commit `416b0a4`.
4. As the maintainer, I want a script that directly validates import-path casing, so that I can
   catch this exact defect class in seconds on my own Windows machine, without needing a Linux
   runner or a full build/pack/test cycle.
5. As the maintainer, I want that casing-validation script wired into the release pre-flight
   checks, so that a new version can never be tagged while an import path is miscased.
6. As a contributor, I want a `ubuntu-latest` CI job that mirrors the existing `windows-latest` job,
   so that any future cross-platform packaging regression — casing or otherwise — is caught on
   every pull request, before merge.
7. As a contributor reading `README.md`, I want the repository-layout diagram to show the actual,
   correct directory name, so that the docs aren't self-contradictory.
8. As a contributor running `scripts/GenerateRuleReference.cs` locally, I want it to reference the
   real, correctly-cased `packages/` path, so that the script doesn't silently rely on
   case-insensitive path resolution.
9. As a contributor running the test suite, I want `RuleDocCoverageShould.cs` to reference the
   correctly-cased `packages/` path, so that the test doesn't fail once the tracked directory
   casing is corrected.
10. As the maintainer, I want a local `issues/` PRD and its broken-down issues to document this fix
    (root cause, decisions, and rationale), so that future contributors understand why the casing
    was fixed at the git-tree level rather than by only editing file contents.
11. As the maintainer, I want the `CHANGELOG.md` `[Unreleased]` section to describe this fix in
    consumer-facing terms, so that anyone reading the next release notes understands what changed
    and why it matters for Linux users.
12. As the maintainer, I want the next version released as a patch bump (`v0.0.8`), consistent with
    this repo's existing practice of always using `-Patch` pre-1.0, so that the versioning scheme
    stays predictable.
13. As the maintainer, I want the fix verified via the new `ubuntu-latest` CI job before merging,
    rather than requiring a local Docker/WSL container check, so that verification happens through
    the same pipeline that will protect the repo going forward.

## Implementation Decisions

- **Git path re-casing.** Correct the *tracked* casing (not just working-directory casing, which is
  already correct on this Windows checkout) for:
  - `Opinionated.Dotnet.CodingStandards.slnx` → `Opinionated.DotNet.CodingStandards.slnx`
  - `src/Opinionated.Dotnet.CodingStandards/` (directory and its `.csproj`) →
    `src/Opinionated.DotNet.CodingStandards/`
  - `packages/Opinionated.Dotnet.CodingStandards/` (directory, `.nuspec`, and all six
    `pkgsrc/{build,buildMultiTargeting,buildTransitive}/*.{props,targets}` files) →
    `packages/Opinionated.DotNet.CodingStandards/`

  A case-only rename is invisible to git on a case-insensitive checkout (`core.ignorecase=true`)
  unless done through an intermediate, differently-spelled name (or by untracking and re-adding
  fresh from the already-correct working directory). Whichever mechanism is used, correctness must
  be verified with `git ls-tree -r HEAD` before and after — `git status`/`git diff` cannot detect
  this bug class on this machine.
- **Stray content fixes**, independent of the path re-casing above:
  - `README.md` — correct the repository-layout diagram line.
  - `scripts/GenerateRuleReference.cs` — correct the hardcoded `packages/Opinionated.Dotnet...`
    path.
  - `tests/Opinionated.DotNet.CodingStandards.Tests/RuleDocCoverageShould.cs` — correct the
    hardcoded `packages/Opinionated.Dotnet...` path (this one is load-bearing: left unfixed, this
    test breaks the moment the tracked directory casing is corrected).
- **New verification script**, `scripts/CheckImportPathCasing.cs`, following the existing
  `#!/usr/bin/env dotnet` file-based script convention used by `CheckNugetDependenciesMatchProps.cs`:
  - Scope: every `.props` and `.targets` file in the repository (at minimum `Directory.Build.props`/
    `.targets` and everything under `packages/**/pkgsrc/`).
  - For each `<Import Project="...">` value, resolve it relative to the importing file's directory,
    then confirm the resolved path exists using an exact, case-sensitive comparison against the
    real directory entries (e.g. enumerate the parent directory and compare strings with ordinal
    casing — not `File.Exists`, which is case-insensitive on Windows).
  - Exit non-zero with a clear message identifying the mismatched import and file, mirroring the
    error-reporting style of the existing check scripts.
- **CI wiring**:
  - Add a new step in `ci.yml` running `dotnet ./scripts/CheckImportPathCasing.cs` on the existing
    `windows-latest` job (cheap, catches the defect immediately without waiting on a full build).
  - Add a new `ubuntu-latest` job to `ci.yml` mirroring the existing job's full
    `restore`/`build`/`test`/`pack` sequence.
- **Release pre-flight**: add `dotnet ./scripts/CheckImportPathCasing.cs` to
  `scripts/New-ReleaseTag.ps1`, alongside the existing `CheckNugetDependenciesMatchProps.cs` and
  `GenerateRuleReference.cs --check` calls.
- **Changelog and version**: add a `[Unreleased]` entry to `CHANGELOG.md` describing the fix in
  consumer-facing terms; release as `v0.0.8` via `New-ReleaseTag.ps1 -Patch` once merged.
- **Process**: this PRD lives at `issues/prd-fix-case-sensitive-build.md`; broken-down issues live
  alongside as `issues/NNN-*.md`; work commits to `feat/fix-case-sensitive-build` per AGENTS.md's
  PRD-linked branch-naming convention.

## Testing Decisions

- **Primary regression test**: `scripts/CheckImportPathCasing.cs` is the deep module here — a
  single, simple entry point (exit code) that hides real complexity (XML parsing, relative path
  resolution, case-sensitive comparison across every props/targets file in the repo). It is the
  fastest and most direct test for this exact defect class, runs on any OS including this Windows
  dev machine, and should be exercised in isolation before wiring it into CI.
- **Secondary regression guard**: the new `ubuntu-latest` CI job runs the full existing
  `dotnet test` suite unmodified on a real case-sensitive filesystem, which also validates the
  path re-casing end-to-end (a real `dotnet pack` + consumer-project build, exactly like the
  original bug report's repro) and catches any other future cross-platform issue the casing script
  doesn't cover.
- **Existing suite**: no behavior of the analyzers/rules changes, so the existing test suite
  should pass unmodified on Windows once `RuleDocCoverageShould.cs`'s hardcoded path is corrected.
  Follow AGENTS.md's existing guidance — verify affected tests in isolation with `--filter` before
  running the full suite.
- Prior art for the new script's structure and error-reporting style: `scripts/CheckNugetDependenciesMatchProps.cs`
  (in this same repo) — a file-based `dotnet` script with a `GetRootDirectory()` helper, discrete
  `Load*`/`Check*` functions, `Console.Error` messages on failure, and a non-zero exit code.
- No local Docker/WSL container verification is planned — the maintainer has explicitly chosen to
  rely on the new `ubuntu-latest` CI job (and the cross-platform casing script) as the source of
  truth for verification, rather than a manual container check.

## Out of Scope

- No new analyzer rules, editorconfig changes, or changes to any rule's severity.
- No changes to the packaging mechanism itself (`.nuspec` structure, `NuSpecFile` wiring,
  `GeneratePackageOnBuild`) beyond correcting the casing of the paths involved.
- No migration of this repo's own CI/release runners away from `windows-latest` as the primary
  runner — `ubuntu-latest` is added as an additional job, not a replacement.
- No general audit of every path reference in the repository beyond `.props`/`.targets` imports and
  the three stray content references found during investigation — e.g. the `.slnx`'s own
  `<Project>`/`<File>` paths and the `.csproj`'s `<NuSpecFile>` property are covered incidentally
  by the git path re-casing and by the new `ubuntu-latest` job (a wrong-cased `NuSpecFile` would
  fail `dotnet pack` there), not by a dedicated new check.
- No retroactive fix or re-publish of the already-broken `0.0.1`–`0.0.7` versions on nuget.org;
  the fix ships forward as `v0.0.8`.

## Further Notes

- The key lesson for future contributors: on a case-insensitive/case-preserving filesystem (the
  default on Windows and macOS), a directory or file rename that changes *only* casing is silently
  a no-op from git's perspective (`core.ignorecase=true` makes `git status` report clean even
  though the tracked tree still has the old casing). Verifying such a rename requires
  `git ls-tree -r HEAD`, not `git status`/`git diff`. A reliable way to perform the rename is
  through an intermediate, differently-spelled name (or by `git rm --cached` followed by a fresh
  `git add` from the already-correctly-cased working directory).
- Neither `ci.yml` nor `release.yml` runs on a case-sensitive filesystem today; that is precisely
  why this bug shipped in all seven prior releases and why the "already fixed" assumption in
  GitHub issue #20 was wrong. The `ubuntu-latest` job closes that blind spot going forward.
