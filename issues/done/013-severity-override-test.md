# Severity-override test

## Parent PRD

`issues/prd.md` — see *Testing Decisions → Modules to be tested* (the new severity-override test) and User story 23.

## What to build

A new test proving a consumer can override or downgrade an individual rule's severity,
confirming the standards are a layerable starting point rather than a locked ruleset.
This extends the existing warnings-as-errors-disabled test
(`CodingStandardsShould.AllowWarningsAsErrorsToBeDisabled`).

Following the existing project-builder pattern, take code that triggers a known rule
as an error by default, apply a consumer-side override (e.g. an editorconfig
`dotnet_diagnostic.<ID>.severity` entry or an MSBuild `NoWarn`/severity property)
that downgrades or disables that single rule, and assert the diagnostic's level
changes accordingly (downgraded to a warning, or absent) while other rules remain
unaffected.

End-to-end behavior: a consumer's per-rule severity override is respected in the
build output, demonstrating the ruleset is layerable.

## Acceptance criteria

- [x] A test downgrades or disables a single rule's severity via a consumer-side override (`<NoWarn>RS0030</NoWarn>` in the project properties)
- [x] The test asserts the targeted rule's diagnostic level changes (RS0030 absent after NoWarn) while CA1866 (a different rule, IndexOf with single-char string) still fires
- [x] The test passes
- [x] The test reuses the existing `PackageFixture` / `ProjectBuilder` helpers

## Blocked by

None - can start immediately.

## User stories addressed

- User story 23
