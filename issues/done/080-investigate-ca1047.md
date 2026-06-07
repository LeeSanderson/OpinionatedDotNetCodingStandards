## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1047 ("Do not declare protected member in sealed type") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "CS0628 (protected member in sealed type) is a C# compiler error that fires before Roslyn analyzers run; the build fails with a compiler error and CA1047 never appears in SARIF output"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether the CS0628 compiler error is a hard error or a warning in modern C# versions (C# 10+); if it was downgraded to a warning in any language version, test whether CA1047 can fire alongside it in the build harness.
- Check the Roslyn analyzer source for CA1047 to determine whether it is intentionally designed as a VB.NET-only fallback (where `Protected` in a sealed class is not a compiler error), which would confirm it is permanently untestable in C#.
- Attempt a test with `#pragma warning disable CS0628` wrapping the protected member in sealed type pattern to see if suppressing the compiler warning allows CA1047 to appear in SARIF output.
- Check whether setting `<LangVersion>` to an older value (e.g. `7.3`) in the test project via `CreateProjectBuilder` properties changes CS0628 from an error to a warning, enabling CA1047 to fire.
- If none of the above produce a CA1047 diagnostic, document as permanently untestable with the confirmed reason: CS0628 preempts the analyzer in C# regardless of language version or suppression strategy, and the rule is a VB.NET-only diagnostic in practice.

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
