## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA1511 ("Use ArgumentException throw helper") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseArgumentExceptionThrowHelper` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not produce its own diagnostic ID in build SARIF in NetAnalyzers 10.0.x; only IDE0055 fires at the class declaration level for the standard 'if (string.IsNullOrEmpty) throw new ArgumentException' pattern, consistent with formatter-backed diagnostic routing"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA1511", "Use ArgumentException throw helper",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1511",
        Untestable = "Rule does not produce its own diagnostic ID in build SARIF in NetAnalyzers 10.0.x; only IDE0055 fires at the class declaration level for the standard 'if (string.IsNullOrEmpty) throw new ArgumentException' pattern, consistent with formatter-backed diagnostic routing")]
    public async Task UseArgumentExceptionThrowHelper()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void Validate(string value)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Value cannot be null or empty", nameof(value));
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1511").ShouldBeTrue();
    }
```

## Investigation plan

1. Try to suppress IDE0055 in the generated project by adding an `additionalFiles` editorconfig entry (e.g. `dotnet_diagnostic.IDE0055.severity = none`) to the temp project, then rebuild and check whether CA1511 now appears in the build SARIF output independently.

2. Add `#pragma warning disable IDE0055` immediately around the violation pattern in `Program.cs` and re-run the build to determine whether suppressing the formatter diagnostic allows CA1511 to surface on its own.

3. Check the Roslyn/NetAnalyzers source for the `CA1511` descriptor — specifically look for the `EnforceOnBuild` metadata property. If `EnforceOnBuild` is `false` or absent, the rule is an IDE-only (refactoring) diagnostic that intentionally never fires during `dotnet build`, confirming the untestable status permanently.

4. Try alternative violation patterns that do not involve `string.IsNullOrEmpty` (e.g. an explicit `== null` null-check followed by `throw new ArgumentNullException`, or `string.IsNullOrWhiteSpace`) to determine whether a different pattern bypasses the IDE0055 subsumption and allows CA1511 to fire directly.

5. Test the same violation pattern against an older NetAnalyzers version (e.g. 8.x) by temporarily pinning the package reference in the temp project, to determine whether the formatter-backed routing behaviour was introduced in a specific version and whether CA1511 fired independently in earlier releases.

6. Review the NetAnalyzers GitHub repository (dotnet/roslyn-analyzers) changelog, issues, and pull requests for CA1511 to find any explicit statement that this rule is routed through the formatter pipeline and is therefore not `EnforceOnBuild`, or to find any known workaround patterns that produce the diagnostic during build.

7. If all above steps confirm CA1511 cannot fire during build regardless of pattern or version, update the `Untestable` reason in the `[RuleDoc]` attribute with the specific root cause — including the `EnforceOnBuild` value found in source, and a link to the relevant Roslyn source file or GitHub issue.

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
