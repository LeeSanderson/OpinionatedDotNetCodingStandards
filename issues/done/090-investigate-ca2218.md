## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2218 ("Override GetHashCode on overriding Equals") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "In C#, the compiler issues CS0659 (warning promoted to error by TreatWarningsAsErrors) for any class that overrides Equals without overriding GetHashCode; this compiler diagnostic preempts CA2218, which never appears as a separate analyzer diagnostic"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm the preemption: write a minimal scratch project (outside the harness) that overrides `Equals` without overriding `GetHashCode`, then inspect the diagnostics list to verify CS0659 appears and CA2218 does not — establishing that the compiler wins the race.

2. Check whether suppressing CS0659 unblocks CA2218: add `#pragma warning disable CS0659` (or `<NoWarn>CS0659</NoWarn>`) to the violation pattern and re-run the harness; if the analyzer then fires CA2218, a testable pattern exists. Note that the harness sets `TreatWarningsAsErrors`, so the pragma or NoWarn may be the only path.

3. Consult the Roslyn/NetAnalyzers source for CA2218 (search `dotnet/roslyn-analyzers` on GitHub for `OverrideGetHashCodeOnOverridingEqualsAnalyzer`) to determine whether the rule is intentionally skipped when CS0659 is already present, or whether it genuinely runs independently of the compiler warning.

4. Check whether CA2218 is documented as a VB.NET fallback: VB.NET does not emit CS0659 (no equivalent compiler warning), so the rule may exist solely to cover VB.NET projects. If the analyzer source confirms a C#-only skip, document this as the confirmed permanent reason.

5. If all paths are blocked, update the untestable reason in `UntestableRules.cs` to the most precise confirmed wording, e.g. "CA2218 is permanently preempted in C# by compiler error CS0659; suppressing CS0659 via pragma or NoWarn does not cause the analyzer to fire because the rule skips C# when the compiler already covers the violation; rule exists as a VB.NET fallback only."

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
