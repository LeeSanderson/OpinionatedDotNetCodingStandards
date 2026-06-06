## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1516 ("Use cross-platform intrinsics") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when platform-specific hardware intrinsics (SSE2, AVX2, etc.) are used where cross-platform Vector128/Vector256 alternatives exist; no realistic test-harness violation pattern exists without low-level SIMD code"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Read the CA1516 analyzer source (in the `dotnet/roslyn-analyzers` repository, under `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Performance/`) to confirm exactly which API calls or patterns trigger a diagnostic — identify the minimum code construct that fires the rule.

2. Attempt to write a minimal C# test method using `CreateProjectBuilder` that calls a platform-specific intrinsic (e.g. `System.Runtime.Intrinsics.X86.Sse2.Add`) where a cross-platform `Vector128` equivalent exists, and verify whether the analyzer produces a diagnostic in the test harness environment (the harness runs analysis on in-memory source, not at JIT time, so hardware availability should be irrelevant).

3. If the intrinsic types are not resolvable in the test harness, check whether adding `System.Runtime.Intrinsics` (or the relevant NuGet package if it is not inbox) via the `packageReferences` parameter of `CreateProjectBuilder` makes the types resolvable and causes the rule to fire.

4. Check the rule's `editorconfig` severity configuration in the project's `.editorconfig` to confirm CA1516 is enabled at `warning` or `error` level; if it is disabled or not listed, enable it in a local test run and re-attempt step 2.

5. If no violation pattern fires after steps 2–4, cross-reference the rule's reported diagnostic IDs in the `dotnet/roslyn-analyzers` issue tracker and the official CA1516 documentation to determine whether the rule has known limitations (e.g. only fires on specific TFMs, requires unsafe context, or is gated behind a compiler flag), then update the `UntestableRules.cs` entry with a precise, sourced reason.

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
