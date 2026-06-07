## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5385 ("Use Rivest-Shamir-Adleman (RSA) Algorithm With Sufficient Key Size") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RequireSufficientRsaKeySize` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for RSA.Create(int) nor for RSA.Create() + KeySize assignment patterns; the abstract factory and property setter approaches do not trigger the key-size diagnostic"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5385", "Use Rivest-Shamir-Adleman (RSA) Algorithm With Sufficient Key Size",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5385",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for RSA.Create(int) nor for RSA.Create() + KeySize assignment patterns; the abstract factory and property setter approaches do not trigger the key-size diagnostic")]
    public async Task RequireSufficientRsaKeySize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static RSA CreateWeakRsaKey() => RSA.Create(512);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5385").ShouldBeTrue();
    }
```

## Investigation plan

1. Try alternative violation patterns that use `new RSACryptoServiceProvider(512)` (a concrete constructor rather than the abstract factory `RSA.Create(int)`) and check whether CA5385 fires for that pattern, since the current reason confirms the factory-method approach does not trigger the diagnostic.
2. Check the `EnforceOnBuild` metadata for CA5385 in the NetAnalyzers source (look in `src/NetAnalyzers/Core/AnalyzerReleases.Shipped.md` and the rule descriptor in `DoNotUseWeakKEYSizeAlgorithmAnalyzer` or equivalent) to confirm whether the rule is marked `Enabled` and `EnforceOnBuild = true` for the relevant severity, since absent or `false` metadata would explain why no diagnostic appears at build time.
3. Test on an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning `Microsoft.CodeAnalysis.NetAnalyzers` in the generated test project to confirm whether the rule regressed or was never enforced at build time in recent releases.
4. Check the analyzer source for target-framework guards (e.g. `#if NET` or `TargetFrameworkAttribute` checks) that might prevent the rule from running when the generated project targets `net9.0` or `net10.0`, given that some cryptography analyzers skip analysis on newer TFMs where safer APIs are enforced elsewhere.
5. Try a `new RSACng(512)` pattern (another concrete RSA subclass with an explicit key-size constructor) as a further alternative violation source, and record whether that triggers CA5385.
6. Inspect the SARIF/build output for any suppression or info-level diagnostic referencing CA5385 (e.g. the rule might fire at `Info` or `Warning` severity rather than `Error`, causing `HasError("CA5385")` to return false even when the diagnostic is present) — if so, update the assertion to `HasWarning` or lower the expected severity.
7. If none of the above patterns produce a diagnostic, locate the NetAnalyzers GitHub issue tracker for CA5385 regression reports and document the canonical issue URL in the Untestable reason so future maintainers have a direct reference.

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
