## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2328 ("Ensure that JsonSerializerSettings are secure") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant of CA2327 that tracks insecure settings through variables; same Newtonsoft.Json dependency as CA2326"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter can be used to add `Newtonsoft.Json` as a NuGet package reference in the test harness, and confirm that a stable version is available on nuget.org.
- Write a minimal test that adds `Newtonsoft.Json` via `packageReferences`, declares a `JsonSerializerSettings` variable with `TypeNameHandling` set to a non-`None` value, and passes it to `JsonConvert.DeserializeObject` through a local variable (rather than inline) to exercise the taint-tracking path that distinguishes CA2328 from CA2327.
- If the minimal test does not fire CA2328, check the NetAnalyzers source (or the rule documentation at `https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2328`) to confirm whether the data-flow analysis requires the insecure assignment and the deserialize call to be in the same method scope, or whether any additional conditions (e.g. `SerializationBinder` being null) must hold for the diagnostic to fire.
- Cross-check against the feedback memo at `memory/feedback-ca-rules-not-firing.md` to see whether CA2328 is already listed among the CA security rules confirmed not to produce diagnostics in NetAnalyzers 10.0.x; if so, record the version-specific confirmation.
- If no pattern produces a diagnostic, document as permanently untestable: update the `Untestable` reason in `UntestableRules.cs` to state specifically that (a) Newtonsoft.Json can be added via `packageReferences` but (b) CA2328's inter-statement taint analysis does not fire in NetAnalyzers 10.0.x, citing the rule help link and the feedback memo.

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
