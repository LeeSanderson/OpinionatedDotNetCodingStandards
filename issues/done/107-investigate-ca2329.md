## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2329 ("Do not deserialize with JsonSerializer using an insecure configuration") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant that fires on Newtonsoft.Json JsonSerializer created with insecure settings; same Newtonsoft.Json dependency as CA2326"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Check whether `CreateProjectBuilder`'s `packageReferences` parameter can be used to add `Newtonsoft.Json` as a NuGet package reference, and verify that a stable version is available on nuget.org.
2. Write a minimal test that adds `Newtonsoft.Json` via `packageReferences` and attempts to trigger CA2329 by constructing a `Newtonsoft.Json.JsonSerializer` with `TypeNameHandling` set to a value other than `None`, then calling `Deserialize` on it — matching the direct (non-taint) pattern the rule documents.
3. If the direct pattern does not fire CA2329, check whether the rule's data-flow/taint analysis requires the insecure `JsonSerializer` to be created in one statement and passed to `Deserialize` in another; write a two-statement version that separates construction from use to exercise the taint path.
4. If neither pattern fires, read the roslyn-analyzers source for CA2329 (GitHub: dotnet/roslyn-analyzers, `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseInsecureDeserializerJsonNetWithoutBinder.cs` or equivalent) to confirm which exact API surface and call graph it monitors, and check whether the NetAnalyzers 10.0.x version ships this rule enabled by default.
5. If adding `Newtonsoft.Json` as a package reference is not feasible in the harness (network dependency, version pinning, or harness limitation), document this as permanently untestable with the specific reason, and update the `Untestable` string in `UntestableRules.cs` accordingly.

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
