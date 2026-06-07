## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA5404 ("Do not disable token validation checks") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires when Microsoft.IdentityModel.Tokens.TokenValidationParameters has validation checks disabled; Microsoft.IdentityModel.Tokens is not included in the simple build harness"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check whether `CreateProjectBuilder`'s `packageReferences` parameter accepts arbitrary NuGet package references, and whether adding `Microsoft.IdentityModel.Tokens` (a stable, widely-used NuGet package) to the builder is feasible without pulling in incompatible transitive dependencies.
- Search nuget.org for a stable `Microsoft.IdentityModel.Tokens` version that targets `net9.0` (or a compatible TFM used by the test harness) and confirm the package is publicly available and does not require authentication or a private feed.
- Write a minimal test that adds `Microsoft.IdentityModel.Tokens` via `packageReferences` and sets `TokenValidationParameters.ValidateAudience = false` (the canonical violation pattern for CA5404); run it against the build harness to see whether CA5404 appears in SARIF output.
- If the diagnostic fires, verify that the complementary `ValidateIssuer = false` and `ValidateLifetime = false` patterns also trigger CA5404, and that a compliant `TokenValidationParameters` (all checks enabled) produces no diagnostic.
- If the package cannot be added (network dependency, TFM mismatch, or transitive conflict), document the specific blocker as the confirmed permanent-untestable reason and update the `Untestable` string in `UntestableRules.cs` accordingly.

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
