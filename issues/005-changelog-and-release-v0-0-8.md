## Parent PRD

`issues/prd-fix-case-sensitive-build.md`

## What to build

Add a `CHANGELOG.md` entry describing the case-sensitivity fix in consumer-facing terms, then cut
the release via `scripts/New-ReleaseTag.ps1 -Patch` (see the PRD's Implementation Decisions and
Out of Scope sections — this ships forward as `v0.0.8`; the already-broken `0.0.1`–`0.0.7`
versions on nuget.org are not retroactively fixed or re-published).

**This issue is HITL.** Running `New-ReleaseTag.ps1` pushes a `v*` tag, which triggers
`release.yml` and publishes a real package to nuget.org — an irreversible, externally-visible
action. The maintainer must explicitly confirm before the tag is pushed; do not run this
autonomously.

## Acceptance criteria

- [ ] `CHANGELOG.md` has an entry (e.g. under `v0.0.8`) describing the MSB4019/case-sensitivity fix
      for consumers, in the style of existing entries.
- [ ] The maintainer has explicitly confirmed before `scripts/New-ReleaseTag.ps1 -Patch` is run and
      the resulting tag is pushed.
- [ ] The `v0.0.8` release completes successfully — the release workflow builds, tests (including
      the new `ubuntu-latest` job), and publishes to nuget.org.

## Blocked by

- Blocked by `issues/001-add-import-path-casing-check-script.md`
- Blocked by `issues/002-fix-case-sensitive-path-casing.md`
- Blocked by `issues/003-wire-casing-check-into-ci-and-release.md`
- Blocked by `issues/004-add-ubuntu-ci-job.md`

## User stories addressed

- User story 11
- User story 12
- User story 13

## Progress note

`CHANGELOG.md` now has a `v0.0.8` entry covering:

- The `MSB4019` case-sensitivity fix (consumer-facing `Fixed` section) and the new
  `CheckImportPathCasing.cs` pre-flight guard.
- The `Added` new rules exposed by the analyzer bump (`MA0212`, `S8949`, `S8969`, `S8970`) and the
  new `ubuntu-latest` CI job.
- The `Changed` analyzer version bumps (Meziantou.Analyzer 3.0.123 → 3.0.125, SonarAnalyzer.CSharp
  10.29.0.143774 → 10.30.0.144632).

**Remaining (HITL, not done):** the maintainer still needs to explicitly confirm before
`scripts/New-ReleaseTag.ps1 -Patch` is run and the resulting `v*` tag is pushed, which triggers
`release.yml` and publishes to nuget.org. This issue stays open (not moved to `issues/done/`) until
that release completes successfully.
