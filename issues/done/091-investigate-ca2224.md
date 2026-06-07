## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2224 ("Override Equals on overloading operator equals") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "In C#, the compiler issues CS0660/CS0661 (promoted to errors by TreatWarningsAsErrors) for any class that defines operator== without overriding Equals/GetHashCode; these compiler diagnostics preempt CA2224"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the preemption behaviour: write a minimal inline C# snippet (via `CreateProjectBuilder`) that defines `operator==` without overriding `Equals`, and verify that the build fails with CS0660/CS0661 before any CA2224 diagnostic is emitted. Record the exact error output.

2. Check whether suppressing CS0660/CS0661 unblocks CA2224: add `#pragma warning disable CS0660, CS0661` (or `<NoWarn>CS0660;CS0661</NoWarn>` via `CreateProjectBuilder` properties) to the test snippet and re-run to see whether CA2224 now fires. If it fires, a working violation pattern exists and a `[Fact]` test can be written.

3. Inspect the Roslyn / NetAnalyzers source for CA2224: check whether the rule's descriptor targets C# at all, or whether it was intentionally scoped to VB.NET (where the compiler does not emit CS0660/CS0661). The GitHub source is at `dotnet/roslyn-analyzers` — look at `OverrideEqualsOnOverloadingOperatorEqualsAnalyzer` for a `language` filter.

4. Check the rule's documentation and any open issues on `dotnet/roslyn-analyzers` for a stated rationale about C# vs VB.NET applicability, or any known workaround for the compiler-error preemption.

5. If steps 2–4 confirm no C# path forward, update the untestable reason in `UntestableRules.cs` with the specific confirmed finding (e.g. "CA2224 is preempted in C# by CS0660/CS0661 which are promoted to errors; the rule targets VB.NET where no equivalent compiler error exists; permanently untestable in this C#-only harness").

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
