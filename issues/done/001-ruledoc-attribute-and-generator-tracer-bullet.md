## Parent PRD

`issues/prd.md`

## What to build

The end-to-end tracer bullet that proves the whole architecture on a single rule. Define the
`RuleDocAttribute` (see PRD "The `[RuleDoc]` attribute"), build the shared `RuleReferenceGenerator`
deep module (see PRD "Modules"), and rewrite `scripts/GenerateRuleReference.cs` as a thin entry
point that adds a `#:project` reference to the test project and calls the generator to write
`docs/rule-reference.md`.

For this slice the generator parses the `analyzers/Analyzer.*.editorconfig` directives for the
active set, severity, and group; reflects over the test assembly for `[RuleDoc]`; and uses the
**editorconfig-comment fallback** for every not-yet-tagged rule (see PRD "Migration" / "Transition
completeness decision"). Tag exactly **one** existing canonical test — `CA1000`
(`ProhibitStaticMembersOnGenericTypes`) — with its `[RuleDoc]`, sourcing `CA1000`'s description and
help from the attribute and all other rows from the fallback.

This slice does NOT add the coverage test, touch CI, or change the doc's content.

## Acceptance criteria

- [ ] `RuleDocAttribute` exists in the test project with the fields and `AttributeUsage` from the PRD.
- [ ] `RuleReferenceGenerator` is a public class in the test project; the script references the test project via `#:project` and reflects over the same compiled assembly.
- [ ] Running `dotnet scripts/GenerateRuleReference.cs` regenerates `docs/rule-reference.md` **byte-identical** to the currently committed file.
- [ ] `CA1000`'s description/help in the regenerated doc come from its `[RuleDoc]`; all other rows come from the comment fallback.
- [ ] The pre-existing `CheckRuleReferenceFreshness.cs` still passes against the unchanged committed doc (CI untouched in this slice).

## Blocked by

None - can start immediately.

## User stories addressed

- User story 1
- User story 6
- User story 8
- User story 9
- User story 10
- User story 12
- User story 20
