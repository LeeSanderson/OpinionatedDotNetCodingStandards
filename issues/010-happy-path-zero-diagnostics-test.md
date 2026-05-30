# Happy-path test: fully compliant code produces zero diagnostics

## Parent PRD

`issues/prd.md` — see *Testing Decisions → Modules to be tested* (the new "happy-path" test) and User story 20.

## What to build

A new test, following the existing project-builder/`PackageFixture` pattern, that
builds a throwaway project of fully compliant code referencing the packed package and
asserts the resulting build produces **zero diagnostics** (no errors, and no
warnings/notes from the package's rules). This guards against the package becoming
accidentally over-aggressive — a false positive would break every consumer's build.

Use the same SARIF-based `BuildOutputFile` helpers as the existing tests
(`AllResults()` / `HasError()`), asserting the result set is empty for compliant
input.

End-to-end behavior: running the focused test packs the real package, builds a
clean sample project against it, and the SARIF output contains no results.

## Acceptance criteria

- [ ] A new `Should`-style test builds a compliant sample project against the packed package
- [ ] The test asserts the SARIF output contains zero diagnostics (no error, warning, or note results from the package's rules)
- [ ] The test passes against the current ruleset
- [ ] The test reuses the existing `PackageFixture` / `ProjectBuilder` helpers

## Blocked by

None - can start immediately.

## User stories addressed

- User story 20
