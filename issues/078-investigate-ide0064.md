## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse IDE0064 ("Make struct fields writable") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "In C# 12+ assigning a readonly field after a this() constructor call is compile error CS0191, not an IDE diagnostic"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Read the Roslyn source for IDE0064 (MakeStructFieldsWritableDiagnosticAnalyzer) to confirm exactly which C# patterns trigger it and which C# language version(s) are required — focus on whether the rule targets older C# struct constructor syntax that is now a hard compiler error in C# 12+.
2. Check whether any C# language version below 12 (e.g. `<LangVersion>10</LangVersion>`) can be forced in `CreateProjectBuilder` to avoid CS0191, and if so write a minimal struct with a `readonly` field assigned after `this()` in a constructor body to see if IDE0064 fires.
3. Check whether the rule fires on a different pattern — e.g. a struct with a `readonly` field mutated via `ref` return, pointer, or unsafe code — that does not involve a `this()` constructor call, and therefore does not hit CS0191.
4. Verify whether IDE0064 is emitted by build analysis at all (some IDE-prefixed rules are IDE-only and never appear in SARIF output regardless of the code pattern); cross-reference the Roslyn repo `EnforceOnBuild` metadata and the Microsoft docs page to confirm the rule's build-analysis status.
5. If no pattern avoids the compiler error and the rule is confirmed build-analysis-capable, update the `Untestable` reason in `UntestableRules.cs` with the specific confirmed reason (e.g. "IDE0064 is a build-analysis rule but every triggering pattern requires assigning a readonly field after this() which is preempted by CS0191 in C# 10+ and the rule is therefore permanently untestable in the harness").

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
