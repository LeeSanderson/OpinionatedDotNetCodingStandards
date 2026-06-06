## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2326 ("Do not use TypeNameHandling values other than None") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on Newtonsoft.Json TypeNameHandling != None patterns; Newtonsoft.Json is not included in the simple single-project build harness and adding it as a transitive package reference would introduce a network dependency in test infrastructure"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter accepts arbitrary NuGet package references, and whether the test infrastructure already resolves packages from a local cache or feed that would avoid a live network call.
- Write a minimal probe test that adds `Newtonsoft.Json` (latest stable version from nuget.org) as a `packageReferences` entry in `CreateProjectBuilder`, then compiles a snippet that sets `TypeNameHandling` to a value other than `None`, and observe whether CA2326 appears in the SARIF output.
- If the package resolves and the diagnostic fires, promote the probe to a proper `[Fact]` test method with a `[RuleDoc]` attribute and remove the class-level entry from `UntestableRules.cs`.
- If the package fails to resolve (network error, offline CI environment, or deliberate NuGet feed restriction), document this as a confirmed permanent constraint and update the `Untestable` reason in `UntestableRules.cs` to state explicitly that adding Newtonsoft.Json requires a live NuGet feed which is incompatible with the offline/hermetic test infrastructure.
- Cross-check nuget.org to confirm a stable `Newtonsoft.Json` version is available (e.g. 13.x) and note the exact version used, so the confirmed reason references a concrete package coordinate rather than a vague "not included" description.

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
