## Parent PRD

`issues/prd.md`

## What to build

Bring the 6 editor-tier rules into scope and add the untestable mechanism. See PRD "Active set scope
and grouping" and "Untestable rules". Extend `RuleReferenceGenerator` to also scan
`config/Opinionated.editorconfig` and render its active rules
(`IDE0001, IDE0002, IDE0003, IDE0038, IDE0049, IDE0084`) in their own section with a neutral,
honest header (e.g. "IDE / editor rules") noting build enforcement varies per rule. Add these 6 to
the active set so the coverage test now requires them.

Create the `UntestableRules` class carrying class-level `[RuleDoc]`s. **Probe each of the 6** with
the existing build harness: rules that fire at build (at least `IDE0049`) get a real positive test
with a method-level `[RuleDoc]`; rules that don't (e.g. `IDE0003`, build-disabled) get an
`UntestableRules` entry reasoned "IDE-only; not emitted by build analysis". Remove the 6 from the
`KnownUncovered` allowlist.

## Acceptance criteria

- [ ] The regenerated doc gains an "IDE / editor rules" section with the 6 rules (git diff shows the added section).
- [ ] Each of the 6 is either a tagged positive test or an `UntestableRules` entry with a written reason.
- [ ] `IDE0049` is verified as build-firing and covered by a positive test.
- [ ] The coverage test passes with the 6 removed from the allowlist.
- [ ] `dotnet test` is green.

## Blocked by

- Blocked by `issues/001-ruledoc-attribute-and-generator-tracer-bullet.md`
- Blocked by `issues/002-coverage-test-and-allowlist.md`

## User stories addressed

- User story 7
- User story 13
- User story 14
