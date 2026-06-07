## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2243 ("Attribute string literals should parse correctly") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "All attribute string types validated by CA2243 (GuidAttribute, AssemblyVersionAttribute, AssemblyFileVersionAttribute) are also validated by the C# compiler, which emits hard errors (CS0591, CS0647, CS7035) before the analyzer diagnostic can appear in SARIF output"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Step 1: Confirm the compiler-preemption failure mode. Write a minimal inline C# snippet using `[assembly: System.Runtime.InteropServices.GuidAttribute("not-a-guid")]` and verify that the C# compiler emits CS0591 (or CS0647/CS7035) as a hard error before the SARIF output is produced, meaning CA2243 never appears in the diagnostic output.
- Step 2: Check whether any attribute type accepted by CA2243 is validated at runtime rather than compile time. Review the Roslyn analyzer source for CA2243 (dotnet/roslyn-analyzers on GitHub) to enumerate every attribute type the rule inspects, and determine whether any of them can accept an arbitrary string without a compile-time error — for example a custom attribute that derives from one of the checked types or a reflection-based approach.
- Step 3: Investigate whether a VB.NET-style fallback exists. CA2243 was originally a FxCop rule designed partly for VB.NET, where attribute argument validation differs. Check the roslyn-analyzers source to confirm whether the C# walker is present at all, or whether the rule is effectively a no-op in C# because the compiler always fires first.
- Step 4: Attempt a suppression workaround. Try wrapping the violating attribute in a `#pragma warning disable CS0591` (and equivalents) block inside the test project's inline source to see whether suppressing the compiler error allows the CA2243 diagnostic to surface in the SARIF output. If the compiler error is a hard error (not a warning), pragmas will have no effect — document this finding explicitly.
- Step 5: If no testable pattern is found, update `UntestableRules.cs` with a precise, source-cited reason such as: "CA2243 is permanently untestable in C#: GuidAttribute/AssemblyVersionAttribute/AssemblyFileVersionAttribute arguments are validated by the Roslyn compiler (CS0591, CS0647, CS7035 hard errors) before the analyzer diagnostic can appear; the rule has no effect in C# and was originally designed as a VB.NET fallback (confirmed in dotnet/roslyn-analyzers CA2243 source)."

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
