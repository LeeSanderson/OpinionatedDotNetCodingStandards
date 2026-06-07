## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2361 ("Ensure auto-generated class containing DataSet.ReadXml() is not used with untrusted data") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on auto-generated typed DataSet classes (produced by the DataSet Designer or xsd.exe) that call ReadXml internally; not replicable from hand-written code in the test harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if manually writing a class that inherits from `System.Data.DataSet` and calls `ReadXml` internally (mimicking what the DataSet Designer or xsd.exe generates) is sufficient to trigger CA2361, by authoring a minimal hand-written typed DataSet subclass in the test harness and observing whether the diagnostic fires.
- Read the Roslyn analyzer source for CA2361 (in the `dotnet/roslyn-analyzers` repository) to determine the exact pattern the rule matches — specifically whether it requires a code-generated attribute (such as `[GeneratedCode]`), a specific base-class shape, or any other marker that hand-written code cannot satisfy.
- If the analyzer requires a `[GeneratedCode]` attribute, add `[System.CodeDom.Compiler.GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]` to a hand-written DataSet subclass and re-run the test harness to check if the diagnostic now fires.
- If the diagnostic still does not fire, confirm whether the rule is intentionally restricted to output produced by the DataSet Designer toolchain (i.e. a deliberate design decision documented in the rule's GitHub issue or PR), and record the source URL as evidence.
- Based on findings: if a working violation pattern is found, write a `[Fact]` test method with `[RuleDoc]` and remove the class-level entry from `UntestableRules.cs`; if permanently untestable is confirmed, update the untestable reason in `UntestableRules.cs` with the specific confirmed reason and a source reference.

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
