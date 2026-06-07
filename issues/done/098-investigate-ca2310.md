## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2310 ("Do not use insecure deserializer NetDataContractSerializer") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "NetDataContractSerializer was removed from .NET Core; the type does not exist in .NET 5+ BCL and cannot be referenced in the test harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check the official Microsoft docs and the Roslyn analyzer source (microsoft/codeanalysis-analyzers on GitHub) to confirm which specific type(s) CA2310 targets and whether any alternative or shim type in a NuGet package could substitute for `NetDataContractSerializer` in .NET 5+.
- Search NuGet.org for any package that re-exposes `System.Runtime.Serialization.NetDataContractSerializer` compatible with `net8.0` or `net9.0`; if one exists, check whether `CreateProjectBuilder`'s `packageReferences` parameter can add it and whether the analyzer fires against the replacement type.
- Confirm by checking the .NET 5+ BCL source (dotnet/runtime on GitHub) that `NetDataContractSerializer` was removed (not merely deprecated) and is absent from all `net5.0`+ target framework monikers, so no in-box reference path exists.
- If no compatible package or replacement type is found, verify that the analyzer's detection logic is tightly bound to `System.Runtime.Serialization.NetDataContractSerializer` by name and would not fire for a hand-written stub class of the same name in the test harness.
- Document the confirmed finding: update the `Untestable` reason in `UntestableRules.cs` to cite the specific .NET version in which the type was removed and the absence of any NuGet shim, or create a `[Fact]` test if a viable workaround is discovered.

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
