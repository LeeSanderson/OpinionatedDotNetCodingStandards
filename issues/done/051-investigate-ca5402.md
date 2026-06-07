## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5402 ("Use CreateEncryptor with the default IV") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `UseCreateEncryptorWithDefaultIv` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the parameterless Aes.CreateEncryptor() overload; unlike the sibling rule CA5401 (which fires for the 2-argument overload), CA5402 produces no diagnostic even when the encryptor is actively used"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5402", "Use CreateEncryptor with the default IV",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5402",
        Untestable = "Rule does not fire in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x for the parameterless Aes.CreateEncryptor() overload; unlike the sibling rule CA5401 (which fires for the 2-argument overload), CA5402 produces no diagnostic even when the encryptor is actively used")]
    public async Task UseCreateEncryptorWithDefaultIv()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static ICryptoTransform CreateEncryptor()
                {
                    using var aes = Aes.Create();
                    return aes.CreateEncryptor();
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5402").ShouldBeTrue();
    }
```

## Investigation plan

1. Try alternative violation patterns that pass explicit `null` or a default/known-value IV to `CreateEncryptor(byte[] rgbKey, byte[] rgbIV)` — the rule description says "default IV" so check whether the analyzer actually targets the two-argument overload called with a null or default IV rather than the zero-argument overload.
2. Check the `EnforceOnBuild` metadata for CA5402 in the NetAnalyzers source (roslyn-analyzers repo, `src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/DoNotUseCreateEncryptorWithNonDefaultIV.cs` or equivalent) to determine whether the rule is flagged as not build-enforced, which would explain why no diagnostic appears in the MSBuild output.
3. Inspect the SARIF or binary log output produced by the test harness for the current violation pattern to confirm CA5402 is truly absent (not just suppressed or renamed) in NetAnalyzers 10.0.x.
4. Test the same violation pattern against an older NetAnalyzers version (e.g. 8.x or 9.x) by temporarily pinning the package reference in the generated test project, to determine whether the absence is a regression or has always been the case.
5. Check for target-framework or language-version guards inside the analyzer implementation that might suppress the diagnostic when the generated project targets a specific TFM (e.g. net9.0 or net10.0).
6. Search the roslyn-analyzers GitHub issues and changelog for CA5402 to find any recorded decision to disable or limit the rule, and capture the issue or PR URL as the authoritative source for the Untestable reason if no working pattern is found.
7. If a working violation pattern is discovered in any of the above steps, update the test code to use that pattern, remove the `Skip` attribute, and confirm the test passes in CI before closing the issue.

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
