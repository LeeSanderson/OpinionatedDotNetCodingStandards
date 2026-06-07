## Parent PRD

`issues/prd.md`

## Type

AFK — investigation can be done by reading analyzer source and checking for workarounds.

## What to build

Deep-analyse CA3061 ("Do Not Add Schema By URL") to either find a pattern that makes it testable in the build harness, or confirm with a well-sourced reason that it is permanently untestable.

**Current state:** This rule has a class-level `[RuleDoc]` entry in `UntestableRules.cs` (no test method exists yet). Current untestable reason: "Fires on XmlSchemaCollection.Add(string, string) where the second argument is a URL; XmlSchemaCollection is a .NET Framework 1.x type that was replaced by XmlSchemaSet in .NET 2.0 and is not available in .NET Core/5+"

**Location:** `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs`

## Investigation plan

1. Confirm that `System.Xml.Schema.XmlSchemaCollection` is absent from the .NET Core/5+ BCL by checking the official .NET API browser (https://learn.microsoft.com/dotnet/api/system.xml.schema.xmlschemacollection) and noting whether the page lists any .NET (non-Framework) target frameworks in the "Applies to" section.

2. Check the NetAnalyzers CA3061 analyzer source (https://github.com/dotnet/roslyn-analyzers/blob/main/src/NetAnalyzers/Core/Microsoft.NetFramework.Analyzers/DoNotUseInsecureXmlAlgorithmAnalyzer.cs or the equivalent CA3061 file) to confirm the exact type and method the rule targets — verify it is `XmlSchemaCollection.Add(string, string)` and not any .NET Core-compatible overload or interface.

3. Determine whether `XmlSchemaSet` (the .NET Core replacement for `XmlSchemaCollection`) has any `Add` overload that accepts a URL string and whether CA3061 was extended to cover it; if such coverage exists, write a minimal violation pattern using `XmlSchemaSet` and check whether it triggers the diagnostic in the test harness.

4. Search NuGet for any compatibility shim package (e.g. `System.Xml.XmlDocument` or a `Microsoft.Bcl.*` package) that re-exposes `XmlSchemaCollection` on .NET 5+ and, if one exists, check whether adding it via the `packageReferences` parameter of `CreateProjectBuilder` produces a compilable violation and fires the diagnostic.

5. If steps 3 and 4 both yield no testable path, update the `Untestable` reason in `UntestableRules.cs` to be more specific, e.g. "XmlSchemaCollection was removed from .NET Core 1.0 and has no .NET 5+ equivalent; XmlSchemaSet (the replacement) is not covered by CA3061; no compatibility shim restores the type on .NET 5+; permanently untestable."

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
