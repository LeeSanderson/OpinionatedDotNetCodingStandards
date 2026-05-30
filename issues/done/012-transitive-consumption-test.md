# Transitive-consumption test

## Parent PRD

`issues/prd.md` — see *Testing Decisions → Modules to be tested* (the new transitive-consumption test) and User story 22.

## What to build

A new test proving the standards are still enforced when the package is referenced
**transitively** (through a dependency) rather than directly, validating the
multi-folder MSBuild wiring (`build` / `buildTransitive` / `buildMultiTargeting`).

Following the existing project-builder pattern, set up a project that depends on the
coding-standards package transitively (e.g. a library that references the package and
an application that references the library, where the application itself does not
reference the package directly), then assert that a known rule still fires in the
downstream project's build output.

End-to-end behavior: building the transitively-referencing project produces the
expected diagnostic, confirming the standards flow through the dependency graph.

## Acceptance criteria

- [x] A test constructs a transitive reference chain to the packed package
- [x] The test asserts a known rule still fires in the downstream project that references the package only transitively
- [x] The test passes, validating the `buildTransitive` wiring
- [x] The test reuses the existing `PackageFixture` / `ProjectBuilder` helpers (extended `AddFile` to create parent directories, enabling subdirectory project layouts)

## Blocked by

None - can start immediately.

## User stories addressed

- User story 22
