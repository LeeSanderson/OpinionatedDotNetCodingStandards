## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA2362 ("Unsafe DataSet or DataTable in auto-generated serializable type can be vulnerable to remote code execution attacks") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on auto-generated typed DataSet/DataTable serializable classes produced by DataSet Designer or xsd.exe; same auto-generated-code constraint as CA2361"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if manually-written code that mimics the auto-generated pattern triggers the rule: write a C# class decorated with `[System.ComponentModel.DesignerCategory("code")]` and `[Serializable]` that inherits from `System.Data.DataSet` or `System.Data.DataTable`, matching the shape that xsd.exe and DataSet Designer emit, and run it through the test harness to see if CA2362 fires.
- Inspect the Roslyn analyzer source for CA2362 (in the `dotnet/roslyn-analyzers` repository, file `DataSetDataTableInSerializableTypeAnalyzer.cs`) to identify the exact syntactic or semantic conditions checked — specifically whether the rule keys on generated-code attributes (such as `[GeneratedCode]` or `[DebuggerNonUserCode]`) or on the presence of `InitClass`/`InitVars` helper methods that xsd.exe emits, and determine whether those conditions can be reproduced in hand-written test code.
- If the analyzer requires `[GeneratedCode("System.Data.Design.TypedDataSetGenerator", ...)]` to be present on the class, add that attribute to the test snippet and re-run to confirm whether the rule fires with that decoration alone.
- Check whether the rule is suppressed for files that do not carry a `[GeneratedCode]` attribute — if the analyzer intentionally skips non-generated files, document this as the confirmed permanent reason: "CA2362 only fires on files marked with [GeneratedCode] by xsd.exe or DataSet Designer; the test harness cannot replicate the generated-code file-origin flag that triggers the rule."
- If none of the above patterns produce a diagnostic, confirm permanently untestable and update the reason in `UntestableRules.cs` to the specific confirmed explanation found in the analyzer source.

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
