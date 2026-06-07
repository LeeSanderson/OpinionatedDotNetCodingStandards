## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0084 ("Use pattern matching (IsNot operator)") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "VB.NET-only operator; not applicable in C# projects"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that the `IsNot` operator is a VB.NET-only language construct with no C# equivalent by checking the Roslyn source for IDE0084 (search `dotnet/roslyn` for `IDE0084` or `UsePatternMatchingIsNotAnalyzer`).
- Verify that the analyzer is registered only for VB.NET (`LanguageNames.VisualBasic`) and not for C# (`LanguageNames.CSharp`) by reading the analyzer's `[DiagnosticAnalyzer]` attribute in the Roslyn source.
- Confirm the rule cannot fire in a C# test project by checking whether any C# construct (e.g. negated `is` patterns, `is not null`) is mapped to IDE0084 in any version of the Roslyn analyzer documentation or release notes.
- Document as permanently untestable with the specific confirmed reason: "IDE0084 targets the VB.NET `IsNot` operator, which has no C# equivalent; the analyzer is registered for VisualBasic only and will never fire in a C# project."

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
