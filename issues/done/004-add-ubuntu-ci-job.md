## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Add a new `ubuntu-latest` job to `.github/workflows/ci.yml` mirroring the existing `windows-latest`
job's full sequence (`dotnet restore`/`build`/`test`/`pack`), so this repository's own CI runs on a
real case-sensitive filesystem going forward — closing the blind spot that let the original bug
ship in all seven prior releases undetected (see the PRD's Implementation Decisions and Further
Notes sections).

## Acceptance criteria

- [x] A new `ubuntu-latest` job exists in `ci.yml`, running the same
      `restore`/`build`/`test`/`pack` sequence as the existing `windows-latest` job.
- [x] The new job passes — confirming, via a real Linux `dotnet pack` + build, that the fix from
      `issues/002` actually resolves the original consumer-facing bug end-to-end.

## Blocked by

- Blocked by `issues/002-fix-case-sensitive-path-casing.md`

## User stories addressed

- User story 6
- User story 13

## Verification (repair pass)

This checkout cannot push to GitHub (per this repo's git boundaries, pushing/opening PRs is the
human's job), so the actual `build-ubuntu` Actions job has still never executed on
`github.com`. To close the acceptance criterion without that push, the job's exact
`restore`/`build`/`test`/`pack` sequence was reproduced on a genuine Linux, case-sensitive
filesystem instead of relying on static YAML/actionlint checks alone:

- A fresh `git clone` of this branch (`feat/fix-case-sensitive-build`, commit `c232645`) was made
  *into* an `ubuntu:24.04` Docker container's own native filesystem (not a bind-mount of this
  Windows checkout — Docker Desktop bind-mounts of Windows directories stay case-insensitive even
  when read from a Linux container, which would silently defeat the point of the test).
- `.NET SDK 10.0.101` was installed via `dotnet-install.sh`, pinned to the exact version in
  `global.json` (not just a floating `10.0` image tag, which resolved to a different, incompatible
  feature band).
- `git ls-tree -r HEAD` inside that case-sensitive clone confirmed no stray old (`Dotnet`-cased)
  tracked paths remain.
- Ran, unmodified, the same commands as the `build-ubuntu` job:
  - `dotnet restore Opinionated.DotNet.CodingStandards.slnx` — succeeded.
  - `dotnet build Opinionated.DotNet.CodingStandards.slnx --configuration Release --no-restore` —
    succeeded, 0 warnings/0 errors (this step alone would fail with `MSB4019` if the casing bug
    from `issues/002` were still present).
  - `dotnet test Opinionated.DotNet.CodingStandards.slnx --configuration Release --no-build` — the
    full, unmodified suite: **Passed! Failed: 0, Passed: 805, Skipped: 4, Total: 809, Duration:
    6m57s** (the 4 skips are the pre-existing, genuinely-untestable rules tracked in
    `UntestableRules.cs`/`[Fact(Skip = "untestable")]`, not new failures). This includes
    `HappyPathShould` and `TransitiveConsumptionShould`, which build real consumer projects
    against the packed `Opinionated.DotNet.CodingStandards` nupkg — the exact scenario the
    original consumer-facing `MSB4019` bug broke.
  - `dotnet pack Opinionated.DotNet.CodingStandards.slnx --configuration Release --no-build
    --output artifacts/packages` — succeeded, produced
    `Opinionated.DotNet.CodingStandards.0.0.0-dev.nupkg`.

This is first-hand evidence — not a static parse — that the `build-ubuntu` job's steps pass end to
end on a real case-sensitive Linux filesystem, and that the fix from `issues/002` resolves the
original bug. It does not replace an actual GitHub Actions run of the job (which will happen the
first time this branch is opened as a PR), but it closes the verification gap that was achievable
from this checkout.

## Verification (real GitHub Actions run)

PR #21 (`feat/fix-case-sensitive-build` → `main`) confirmed this end-to-end on real infrastructure:
the `build (ubuntu)` job (run
[30175226951](https://github.com/LeeSanderson/OpinionatedDotNetCodingStandards/actions/runs/30175226951))
passed in 14m25s — restore, build, test (805 passed, 4 skipped, 0 failed), and pack all succeeded on
`ubuntu-latest`. Acceptance criterion 2 is now genuinely met, not just reproduced locally in a
container. (The sibling `build` (windows) job on the same run failed, but only on its unrelated
"Check for outdated packages" step — three analyzer/test-dependency packages have newer versions
available. That's pre-existing, out of this PRD's scope, and orthogonal to the case-sensitivity fix.)
