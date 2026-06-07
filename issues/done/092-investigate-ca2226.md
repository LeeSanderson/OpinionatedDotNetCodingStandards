## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2226 ("Operators should have symmetrical overloads") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "In C#, the compiler enforces paired operators (==,!=; <,>; <=,>=) with CS0216 compile error, making it impossible to define only one of a pair; CA2226 therefore never fires as an analyzer diagnostic in C# projects"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check the Roslyn analyzer source for CA2226 to confirm which languages it targets; verify whether the rule is registered for C# only, VB.NET only, or both, and whether there is a language guard that skips C# entirely because CS0216 pre-empts it.
- Attempt to craft a C# class with only one operator of a paired set (e.g. `operator ==` without `operator !=`) and observe whether the build output shows CS0216 before CA2226 can fire; record the exact compiler error message and confirm CA2226 is absent from the diagnostic output.
- Check whether any C# pattern avoids CS0216 while still satisfying the conditions CA2226 checks — for example, an `implicit` or `explicit` conversion operator, or a custom operator overload that is not part of a compiler-enforced pair — to see if there is a partial-overload scenario the analyzer still catches.
- Review the official Microsoft documentation and any linked GitHub issues for CA2226 to determine if the rule is explicitly described as VB.NET-only or as a fallback for languages that do not enforce symmetric operators at the compiler level.
- If no C# workaround is found, update the untestable reason in `UntestableRules.cs` with the confirmed specific wording, e.g. "CA2226 is permanently untestable in C#: the compiler enforces paired operators via CS0216, which fires before any analyzer diagnostic; the rule exists as a VB.NET fallback where no such compiler enforcement exists".

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
