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
