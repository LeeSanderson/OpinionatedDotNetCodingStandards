## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5388 ("Ensure Sufficient Iteration Count When Using Weak Key Derivation Function") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `EnsureSufficientIterationCountInKeyDerivation` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow/taint analysis variant of CA5387: fires when the iteration count passed to Rfc2898DeriveBytes comes from a variable rather than a literal and cannot be proven to exceed the threshold; requires inter-procedural taint analysis not triggerable from a single-project build harness"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5388", "Ensure Sufficient Iteration Count When Using Weak Key Derivation Function",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5388",
        Untestable = "Data-flow/taint analysis variant of CA5387: fires when the iteration count passed to Rfc2898DeriveBytes comes from a variable rather than a literal and cannot be proven to exceed the threshold; requires inter-procedural taint analysis not triggerable from a single-project build harness")]
    public async Task EnsureSufficientIterationCountInKeyDerivation()
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
                public static byte[] DeriveKey(string password, byte[] salt, int iterations)
                {
                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                    return pbkdf2.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0060
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5388").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where the iteration count variable is assigned a low literal in the same method body (e.g., `int iterations = 1;`) and passed immediately to `Rfc2898DeriveBytes`, removing any parameter boundary — this is the closest single-method approximation of a taint source and tests whether intra-procedural flow is sufficient to fire CA5388.

2. Try a pattern where the iteration count is a field or property with a known low constant value (e.g., `private const int Iterations = 1;`) used directly in the constructor call, to determine whether constant folding causes CA5388 to fire instead of CA5387 (which requires a literal) or whether neither fires.

3. Confirm via the NetAnalyzers GitHub source (`src/NetAnalyzers/Core/Microsoft.NetCore.Analyzers/Security/UseSecureCookiesASPNetCore.cs` and the adjacent CA5387/CA5388 implementation files) whether CA5388 is gated on cross-method or cross-assembly taint analysis, and note the exact `EnforceOnBuild` metadata value — if it is `false` or absent, the rule will never surface as a build error regardless of the violation pattern.

4. Check the NetAnalyzers source for any target-framework guards (e.g., `#if NET` or `targetFramework` conditions) that might prevent CA5388 diagnostics from being emitted on the .NET version used by the test harness, and record the minimum supported framework.

5. Run the current test without `[Fact(Skip = ...)]` and capture the raw build output (stdout and stderr) to verify whether any diagnostic resembling CA5388 appears at a lower severity (warning vs. error) or under a different rule ID, which would indicate a configuration mismatch rather than a genuine taint-analysis limitation.

6. If none of the above patterns produce a CA5388 diagnostic, search the NetAnalyzers issue tracker and changelog for any known decision to restrict CA5388 to IDE-only analysis (i.e., not enforced on build), and record the GitHub issue or PR URL as the authoritative source for updating the Untestable reason.

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
