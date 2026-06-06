## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA1061 ("Do not hide base class methods") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitHidingBaseClassMethodsWithLessDerivedType` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "With 'new', CS0109 fires (the compiler considers types with different parameter types as overloads, not hiding, so 'new' is not required); without 'new', the overload pattern does not fire CA1061 in build SARIF in NetAnalyzers 10.0.x — only IDE0055 appears at the class declaration"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA1061", "Do not hide base class methods",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1061",
        Untestable = "With 'new', CS0109 fires (the compiler considers types with different parameter types as overloads, not hiding, so 'new' is not required); without 'new', the overload pattern does not fire CA1061 in build SARIF in NetAnalyzers 10.0.x — only IDE0055 appears at the class declaration")]
    public async Task ProhibitHidingBaseClassMethodsWithLessDerivedType()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public class Base { public virtual void Method(string s) { } }
            public class Derived : Base { public new void Method(object s) { } }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1061").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm the compiler-error blocker: verify that every C# pattern that uses `new` to hide a base method with a less-derived parameter type (e.g. `string` → `object`) triggers CS0109 ("The member does not hide an inherited member; the new keyword is not required"), and that omitting `new` still produces no CA1061 diagnostic in build SARIF — reproduce both outcomes with the current test harness.

2. Check whether CA1061 is a VB.NET-only rule by design: read the NetAnalyzers source for `DoNotHideBaseClassMethodsAnalyzer` (or equivalent) and inspect its `SupportedDiagnostics`, registered language(s), and any `#if` target-framework guards; confirm whether C# is a supported language for this rule.

3. Check `EnforceOnBuild` metadata in the NetAnalyzers source: locate the rule's `DiagnosticDescriptor` and confirm whether `EnforceOnBuild` is set to `Never` or is absent — which would explain why the diagnostic never appears in build SARIF even when the violation is present in IDE analysis.

4. Try suppressing IDE0055 in the generated project: add an `.editorconfig` `additionalFiles` entry via the test harness that sets `dotnet_diagnostic.IDE0055.severity = none`, then rebuild and check whether CA1061 surfaces in the SARIF output without the IDE0055 noise.

5. Try a `#pragma warning disable CS0109` workaround: add `#pragma warning disable CS0109` around the hiding declaration so the compiler error is suppressed, then check whether CA1061 fires in the build output — this isolates whether CS0109 is actively blocking the CA rule or whether they are independent.

6. Test against an older NetAnalyzers version: temporarily pin the `Microsoft.CodeAnalysis.NetAnalyzers` package to version 8.x in the generated project (if `CreateProjectBuilder` supports a `packageReferences` override) and rerun the violation pattern to determine whether CA1061 was build-enforceable in earlier versions and was subsequently downgraded or removed.

7. Search the NetAnalyzers GitHub repository for CA1061 issues or changelogs: look for any issue or PR that changed `EnforceOnBuild` to `Never`, restricted the rule to VB.NET, or documented that C# cannot trigger CA1061 due to the language's overload-resolution semantics; record the direct URL as the sourced reason if found.

## Acceptance criteria

- [ ] Root cause identified and documented
- [ ] One of:
  - [ ] A working violation pattern found → test updated, Skip removed, test passes in CI; OR
  - [ ] Confirmed permanently untestable → Untestable reason updated with the specific root cause (source location or GitHub issue link)
- [ ] No regressions in other tests in the same test file
- [ ] If the test is promoted, RuleReferenceGenerator coverage test continues to pass

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2: every build-enforced rule backed by at least one test
- User story 7: rules that genuinely cannot be triggered documented with a written reason
