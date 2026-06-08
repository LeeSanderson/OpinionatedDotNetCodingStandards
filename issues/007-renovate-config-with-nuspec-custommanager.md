## Parent PRD

`issues/prd.md`

## What to build

Add a Renovate configuration that keeps the analyzer version in sync across both files it lives
in, so a single bump PR stays consistent with the existing nuspec/props check. See the PRD
"Automation around the tool" implementation decision and user story 14.

End-to-end behavior: a `renovate.json` uses the native NuGet manager for
`Directory.Packages.props` and adds a customManager (regex) that also rewrites the matching
`<dependency id="…" version="…"/>` entries in the `.nuspec`. A Renovate bump for any of the four
analyzer packages then updates props and nuspec atomically in one PR, so
`CheckNugetDependenciesMatchProps` stays green and CI (slice 006) regenerates the editorconfigs
on that same PR.

This is HITL: it requires the Renovate app to be enabled on the repository and verification that
both managers detect the right entries. Renovate can land independently of CI, but only delivers
full value once slice 006 exists.

## Acceptance criteria

- [ ] `renovate.json` exists with the NuGet manager covering `Directory.Packages.props` and a
      customManager (regex) covering the `.nuspec` analyzer `<dependency>` versions.
- [ ] A Renovate dry-run / onboarding PR detects all four analyzer dependencies and matches the
      corresponding nuspec lines.
- [ ] A simulated bump updates props and nuspec to the same version in a single change, leaving
      `CheckNugetDependenciesMatchProps` green.

## Blocked by

- None - can start immediately. (Delivers full value only after
  `issues/006-ci-regenerate-and-commit-back.md`.)

## User stories addressed

- User story 14
