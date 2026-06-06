## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5405 ("Do not always skip token validation in delegates") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when Microsoft.IdentityModel.Tokens validation delegates always return true without checking; requires Microsoft.IdentityModel.Tokens package not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder`'s `packageReferences` parameter allows adding `Microsoft.IdentityModel.Tokens` as an extra NuGet reference for the test project.
- Confirm that `Microsoft.IdentityModel.Tokens` is available on nuget.org with a stable version compatible with the target framework used by the build harness.
- Write a minimal test that adds `Microsoft.IdentityModel.Tokens` via `packageReferences`, authors a delegate that always returns `true` without inspecting the token (the documented violation pattern for CA5405), and asserts the diagnostic fires.
- If the package resolves but the diagnostic does not fire, check the NetAnalyzers source for CA5405 to confirm which exact delegate signatures and return patterns it recognises, and adjust the violation code accordingly.
- If adding the package is not supported by `CreateProjectBuilder`, or if the package cannot be restored in the harness environment, document as permanently untestable with the specific blocker (e.g. "Microsoft.IdentityModel.Tokens cannot be added as a packageReference in CreateProjectBuilder; rule requires third-party token-validation types unavailable in the simple build harness").

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
