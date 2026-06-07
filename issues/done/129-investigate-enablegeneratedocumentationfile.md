## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse EnableGenerateDocumentationFile ("Set MSBuild property GenerateDocumentationFile to true") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Project-configuration recommendation, not a code-pattern violation; fires based on the absence of the GenerateDocumentationFile MSBuild property and cannot be triggered or suppressed by writing code in the test harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Read the Roslyn analyzer source for EnableGenerateDocumentationFile (linked from https://github.com/dotnet/roslyn/issues/41640) to confirm it inspects the MSBuild property `GenerateDocumentationFile` rather than any code pattern; note the exact diagnostic condition.
- Check whether `CreateProjectBuilder` (the test harness helper) exposes a way to set MSBuild properties on the generated project file (e.g. a `properties` or `globalProperties` parameter); if so, attempt to omit or set `GenerateDocumentationFile=false` and see whether the rule fires.
- Search the Roslyn/roslyn-analyzers GitHub repository for any existing test fixture that exercises EnableGenerateDocumentationFile to learn what project setup is required and whether a code-level trigger exists at all.
- Confirm the rule's category (IDE suggestion vs. build-emitted warning) by checking whether the diagnostic severity is configured in the `.editorconfig` or `.globalconfig` shipped with this repo and whether dotnet build alone emits it without an IDE host.
- If no code-level trigger can be found, update the untestable reason in `UntestableRules.cs` to cite the specific confirmed finding (e.g. "rule is implemented as an IDE suggestion that reads the MSBuild property at design time; dotnet build does not emit the diagnostic and no code pattern can trigger or suppress it in the test harness").

## Acceptance criteria

- [ ] Root cause confirmed: the rule is either permanently untestable (documented reason) or a workaround exists
- [ ] One of:
  - [ ] A working violation pattern found → new `[Fact]` test method created with `[RuleDoc]`, class-level entry removed from UntestableRules.cs, test passes in CI; OR
  - [ ] Permanently untestable confirmed → Untestable reason in UntestableRules.cs updated with the specific confirmed reason (e.g. "type removed in .NET Core 1.0; cannot reference System.Web in .NET 5+ projects")
- [ ] No regressions in other tests
- [ ] RuleReferenceGenerator coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
