## Parent PRD

`issues/prd.md`

## What to build

The coverage gate: a new xUnit test that calls `RuleReferenceGenerator`'s reconciliation to assert
that every active rule is accounted for. See PRD "Modules" (Coverage unit test, `KnownUncovered`
allowlist) and "Source of truth and field ownership".

The test must assert: every active rule (from the `analyzers/` directives) has exactly one canonical
`[RuleDoc]` **or** appears on the `KnownUncovered` allowlist; no `[RuleDoc]` names an inactive rule
(orphan check); each `RuleId` is documented exactly once (uniqueness); and the method/class
invariants hold (method-level ⇒ `Untestable` null; class-level ⇒ `Untestable` non-empty). Seed
`KnownUncovered` with every currently active-but-untagged rule (i.e. all active rules except
`CA1000`). The `Untestable` invariant logic is implemented here even though no exempt rules exist
yet (they arrive in `issues/004`).

## Acceptance criteria

- [ ] A new coverage test passes with `CA1000` covered and all other active rules on the allowlist.
- [ ] Introducing a `[RuleDoc]` for a non-active rule id fails the test (orphan check).
- [ ] Removing a rule from the allowlist without adding its `[RuleDoc]` fails the test.
- [ ] Two `[RuleDoc]`s for the same rule id fail the test (uniqueness).
- [ ] A method-level `[RuleDoc]` with a non-null `Untestable`, or a class-level one with an empty `Untestable`, fails the test (invariants).
- [ ] `dotnet test` is green.

## Blocked by

- Blocked by `issues/001-ruledoc-attribute-and-generator-tracer-bullet.md`

## User stories addressed

- User story 2
- User story 3
- User story 4
- User story 5
- User story 15
- User story 16
- User story 22
- User story 24
