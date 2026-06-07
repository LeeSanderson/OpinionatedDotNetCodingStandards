## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA1016 ("Mark assemblies with assembly version") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "The .NET SDK auto-generates AssemblyVersionAttribute from project metadata (GenerateAssemblyInfo=true by default); CA1016 can never fire in the test harness because the attribute is always present"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

- Check if `CreateProjectBuilder` exposes a `properties` parameter (or similar) that allows passing MSBuild properties to the temp project, and if so, whether passing `GenerateAssemblyInfo=false` causes CA1016 to fire on a minimal assembly with no explicit `[assembly: AssemblyVersion(...)]` attribute.
- If `CreateProjectBuilder` does not support arbitrary MSBuild properties directly, inspect the test harness source to find whether the generated `.csproj` can be patched (e.g. via a `Directory.Build.props` override or a custom project template) to set `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` before compilation.
- Write a minimal probe test that sets `GenerateAssemblyInfo=false` and compiles a single empty `class Foo {}` file; confirm whether the CA1016 diagnostic is emitted in the result and whether the test passes end-to-end in CI.
- If neither approach works, consult the Roslyn analyzer source for CA1016 (in `dotnet/roslyn-analyzers`) to verify it targets `IAssemblySymbol` and fires only when `AssemblyVersionAttribute` is absent from the compiled assembly symbol — confirming the SDK auto-generation is the sole blocker.
- Document the outcome: if `GenerateAssemblyInfo=false` makes the rule fire, create a `[Fact]` test method with `[RuleDoc]` and remove the class-level entry from `UntestableRules.cs`; otherwise update the untestable reason in `UntestableRules.cs` with the precise confirmed explanation (e.g. "GenerateAssemblyInfo cannot be disabled in the test harness; the SDK always injects AssemblyVersionAttribute before Roslyn analysis runs").

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
