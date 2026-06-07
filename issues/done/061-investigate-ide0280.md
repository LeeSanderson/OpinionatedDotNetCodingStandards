## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse IDE0280 ("Use 'nameof'") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseNameofInsteadOfStringLiteral` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0280; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("IDE0280", "Use 'nameof'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0280",
        Untestable = "Formatter-backed rule: emits IDE0055 ('Fix formatting') in build SARIF instead of its own diagnostic ID IDE0280; the enforcement mechanism goes through the Roslyn formatter rather than the analyzer pipeline")]
    public async Task UseNameofInsteadOfStringLiteral()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void ValidateValue(string value)
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("IDE0280").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm the formatter-backed claim by running the existing test code (without the Skip) and capturing the raw build SARIF or MSBuild output to verify whether IDE0055 is emitted in place of IDE0280, or if neither diagnostic fires at all.
2. Attempt to suppress IDE0055 in the generated project by injecting an `.editorconfig` additionalFiles entry (`dotnet_diagnostic.IDE0055.severity = none`) via the test harness, then rebuild and check whether IDE0280 appears on its own in the output.
3. Add a `#pragma warning disable IDE0055` comment around the violation site in `Program.cs` and rebuild, checking whether the suppression causes IDE0280 to surface as the reported diagnostic.
4. Check the Roslyn source (or NuGet package metadata) for the `EnforceOnBuild` property of IDE0280 — confirm whether the rule is declared as `EnforceOnBuild = true` or whether it is intentionally excluded from command-line builds, which would explain why it never fires under `dotnet build`.
5. Downgrade the `Microsoft.CodeAnalysis.NetAnalyzers` (or `Microsoft.CodeAnalysis.CSharp`) package reference in the generated test project to an 8.x version and rerun the build, checking whether the older toolchain surfaces IDE0280 directly rather than routing through IDE0055.
6. Try alternative violation patterns that more explicitly avoid any formatting ambiguity — for example, using the string literal in an attribute constructor or a `nameof`-eligible expression outside an exception constructor — to determine if the routing to IDE0055 is pattern-specific or universal for IDE0280.
7. If none of the above unblocks the test, locate the authoritative Roslyn GitHub issue or source comment that documents IDE0280's build-enforcement behaviour and record its URL as the confirmed source for the Untestable reason.

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
