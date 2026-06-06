## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2330 ("Ensure that JsonSerializer has a secure configuration when deserializing") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Data-flow/taint analysis variant of CA2329 that tracks insecure serializer through variables; same Newtonsoft.Json dependency as CA2326"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter can be used to add `Newtonsoft.Json` as a NuGet package reference, and verify that a stable version is available on nuget.org.
- Write a minimal test that adds `Newtonsoft.Json` via `packageReferences` and uses a `JsonSerializer` variable assigned with an insecure `TypeNameHandling` setting, then passed to `JsonSerializer.Deserialize`; confirm whether CA2330 fires (the taint must flow through a local variable, not inline, to distinguish it from CA2329).
- If the first attempt does not fire, consult the roslyn-analyzers source for CA2330 (search `CA2330` in the `dotnet/roslyn-analyzers` GitHub repo) to identify the exact taint-graph pattern required — specifically whether the serializer must be assigned in a separate statement and whether the `Deserialize` call must be in a different scope.
- If CA2330 does fire with the Newtonsoft.Json package reference approach, check whether adding `Newtonsoft.Json` causes any transitive package conflicts or network dependency issues in CI; if acceptable, remove the class-level entry from `UntestableRules.cs` and add a `[Fact]` test method with `[RuleDoc]`.
- If CA2330 never fires regardless of taint pattern (e.g. the analyzer requires a minimum Newtonsoft.Json version or specific assembly identity), document the exact confirmed reason in `UntestableRules.cs` — for example "Newtonsoft.Json package reference required but introduces unacceptable network/transitive dependency in the build harness" or "taint-analysis engine does not fire CA2330 without the exact auto-detected package version".

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
