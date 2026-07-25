## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Add `scripts/CheckImportPathCasing.cs`, a new file-based `dotnet` script following the existing
convention set by `scripts/CheckNugetDependenciesMatchProps.cs` (see the PRD's Implementation
Decisions and Testing Decisions sections for prior art). The script walks every `.props` and
`.targets` file in the repository (at minimum `Directory.Build.props`/`.targets` and everything
under `packages/**/pkgsrc/`), parses each `<Import Project="...">` value, resolves it relative to
the importing file's directory, and confirms the resolved path exists via an exact,
case-sensitive comparison against the real directory entries — not `File.Exists`, which is
case-insensitive on Windows and would miss the exact bug this script exists to catch. On any
mismatch it prints a clear error identifying the importing file and the bad import, and exits
non-zero; it exits 0 when every import resolves cleanly.

This script is written first, against the repository's *current* (still-broken) state, so it can
prove it detects the real, known bug before anything is fixed. It is not yet wired into any CI
step or release script — that wiring is `issues/003-wire-casing-check-into-ci-and-release.md`.

## Acceptance criteria

- [ ] Running `dotnet ./scripts/CheckImportPathCasing.cs` against the current repository state
      exits non-zero and clearly identifies the real, pre-existing casing mismatch (e.g.
      `Directory.Build.props`'s import of
      `packages/Opinionated.DotNet.CodingStandards/pkgsrc/build/Opinionated.DotNet.CodingStandards.props`
      against the git-tracked `packages/Opinionated.Dotnet.CodingStandards/...` tree).
- [ ] The script exits 0 when every import in scope resolves case-sensitively (this becomes
      demonstrable once `issues/002` lands and corrects the tracked tree).
- [ ] The script is not yet referenced from `ci.yml` or `scripts/New-ReleaseTag.ps1`.

## Blocked by

None - can start immediately

## User stories addressed

- User story 4
