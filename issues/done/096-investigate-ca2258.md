## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2258 ("Providing a DynamicInterfaceCastableImplementation interface in Visual Basic is unsupported") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "VB.NET-only rule; not applicable in C# projects"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Confirm that CA2258 is explicitly documented as VB.NET-only by reading the official Microsoft analyzer source or docs (the rule targets `DynamicInterfaceCastableImplementationAttribute` usage in VB.NET, which lacks the required language support).
- Verify that the test harness only creates C# projects (check `CreateProjectBuilder` and related helpers) and has no mechanism to compile VB.NET source files, confirming no path to trigger this rule in the current harness.
- Check the NetAnalyzers GitHub repository (dotnet/roslyn-analyzers) for the CA2258 diagnostic descriptor to confirm the rule is registered only for VB.NET (`VisualBasic` language) and is never emitted for C# compilations.
- Confirm there is no C# analogue rule or companion diagnostic that fires for the same attribute misuse in C#, ruling out any indirect workaround.
- Update the untestable reason in `UntestableRules.cs` with the specific confirmed reason, e.g. "CA2258 targets VB.NET only; the analyzer descriptor is registered for VisualBasic language exclusively and is never emitted for C# compilations — permanently untestable in this C#-only harness".

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
