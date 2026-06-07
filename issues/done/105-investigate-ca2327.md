## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2327 ("Do not use insecure JsonSerializerSettings") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when Newtonsoft.Json JsonSerializerSettings with TypeNameHandling != None is passed to JsonConvert.DeserializeObject; same Newtonsoft.Json dependency as CA2326"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter accepts arbitrary NuGet package identifiers, and whether passing `Newtonsoft.Json` (latest stable) alongside the existing analyzer package references compiles cleanly in the test harness.
- Visit nuget.org to confirm that `Newtonsoft.Json` has a stable published version compatible with `net9.0` (or whatever TFM the harness uses), and note the exact version to pin in the test.
- Write a minimal inline C# snippet that creates a `JsonSerializerSettings` with `TypeNameHandling = TypeNameHandling.All` and passes it directly to `JsonConvert.DeserializeObject<object>(json, settings)`, then compile it inside `CreateProjectBuilder` with the Newtonsoft.Json package reference added, and observe whether CA2327 appears in the SARIF/diagnostic output.
- If the direct-call pattern does not fire, check the NetAnalyzers 10.0.x source for CA2327 (roslyn-analyzers repo, `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/`) to understand exactly which API call sites it monitors, and adjust the violation snippet to match the documented trigger site.
- If adding the package reference introduces network/restore issues in the CI environment, document that constraint explicitly and confirm the rule is permanently untestable for the same infrastructure reason as CA2326; update the `Untestable` reason in `UntestableRules.cs` to name both the Newtonsoft.Json dependency and the network-restore constraint.

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
