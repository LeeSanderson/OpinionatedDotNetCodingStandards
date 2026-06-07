## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5387 ("Do Not Use Weak Key Derivation Function With Insufficient Iteration Count") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitWeakKeyDerivationFunctionWithInsufficientIterations` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "All Rfc2898DeriveBytes constructors are marked [Obsolete(SYSLIB0060)] in .NET 10; SYSLIB0060 fires but CA5387 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5387", "Do Not Use Weak Key Derivation Function With Insufficient Iteration Count",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5387",
        Untestable = "All Rfc2898DeriveBytes constructors are marked [Obsolete(SYSLIB0060)] in .NET 10; SYSLIB0060 fires but CA5387 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
    public async Task ProhibitWeakKeyDerivationFunctionWithInsufficientIterations()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0060
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] DeriveKey(string password, byte[] salt)
                {
                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100, HashAlgorithmName.SHA256);
                    return pbkdf2.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0060
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5387").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm the current suppression approach works: verify that `#pragma warning disable SYSLIB0060` around the `Rfc2898DeriveBytes` call successfully suppresses the SYSLIB0060 obsolete diagnostic, then check whether CA5387 appears in the build output alongside or instead of SYSLIB0060 when the test is run without the `[Fact(Skip)]` attribute.

2. Try additional SYSLIB suppression variants: add `#pragma warning disable SYSLIB0041` (an earlier iteration-count deprecation) in addition to SYSLIB0060, and also try `<NoWarn>SYSLIB0060</NoWarn>` as a project-level suppression via the test harness's project configuration, to rule out pragma scope issues masking the CA diagnostic.

3. Test on .NET 8 target framework: configure the generated project to target `net8.0` (where `Rfc2898DeriveBytes` constructors with an explicit iteration count may not yet be fully obsoleted as SYSLIB0060), run the same violation pattern, and check whether CA5387 fires on the older TFM.

4. Check the NetAnalyzers source for target-framework guards: locate the CA5387 analyzer implementation in the `dotnet/roslyn-analyzers` GitHub repository (search for `DoNotUseWeakKDFInsufficientIterationCount` or `CA5387`) and inspect whether the diagnostic is suppressed or skipped when `Rfc2898DeriveBytes` is also flagged as obsolete, or when targeting .NET 10+.

5. Try an alternative violation pattern using `PasswordDeriveBytes`: CA5387 may also apply to `System.Security.Cryptography.PasswordDeriveBytes` with a low iteration count. Write a test snippet using that class (which may not carry a SYSLIB0060 obsolete marker) and check whether CA5387 fires.

6. Inspect the `EnforceOnBuild` metadata for CA5387 in the NetAnalyzers 10.0.x package: open the `Microsoft.CodeAnalysis.NetAnalyzers.sarif` or the `AnalyzerReleases.Shipped.md` file in the NuGet package cache and confirm that CA5387 has `EnforceOnBuild = Recommended` or `Enabled`; if it is listed as `Never`, the diagnostic cannot be promoted to an error via `<AnalysisMode>All</AnalysisMode>` alone.

7. Document findings and decide: if a working pattern is found, remove `[Fact(Skip = "untestable")]` and the `Untestable` property, update the test, and confirm it passes in CI. If the rule is confirmed permanently suppressed on .NET 10 (e.g., the analyzer source shows an explicit guard), update the `Untestable` reason to cite the specific source file and line number or the relevant GitHub issue in `dotnet/roslyn-analyzers`.

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
