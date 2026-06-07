## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5373 ("Do not use obsolete key derivation function") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitObsoleteKeyDerivationFunction` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "PasswordDeriveBytes is marked [Obsolete(SYSLIB0041)] in .NET 9+; SYSLIB0041 fires as a build error but CA5373 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5373", "Do not use obsolete key derivation function",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5373",
        Untestable = "PasswordDeriveBytes is marked [Obsolete(SYSLIB0041)] in .NET 9+; SYSLIB0041 fires as a build error but CA5373 does not appear alongside it in Microsoft.CodeAnalysis.NetAnalyzers 10.0.x — the SYSLIB deprecation supersedes the CA diagnostic")]
    public async Task ProhibitObsoleteKeyDerivationFunction()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0041
            using System.Security.Cryptography;
            namespace test;
            public static class Program
            {
                public static byte[] DeriveKey(string password, byte[] salt)
                {
                    using var pdb = new PasswordDeriveBytes(password, salt);
                    return pdb.GetBytes(32);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0041
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5373").ShouldBeTrue();
    }
```

## Investigation plan

1. Confirm whether `#pragma warning disable SYSLIB0041` is sufficient to suppress the SYSLIB diagnostic at the call site: run the existing test code as-is (with `Skip` temporarily removed) and capture the full build output to see which diagnostics are emitted and whether CA5373 appears alongside or instead of SYSLIB0041.

2. Check whether the `#pragma warning disable` must wrap the specific offending lines rather than the whole file: move the `#pragma warning disable SYSLIB0041` to immediately before the `new PasswordDeriveBytes(...)` call and `#pragma warning restore SYSLIB0041` immediately after, then rebuild to see if CA5373 surfaces when the suppression is tightly scoped.

3. Test on a .NET 8 target framework where `PasswordDeriveBytes` may not yet be marked `[Obsolete(SYSLIB0041)]`: change the generated project's target framework to `net8.0` (if `CreateProjectBuilder` supports a target-framework parameter) and re-run to determine whether CA5373 fires without the SYSLIB diagnostic competing.

4. Check the NetAnalyzers source (or the packaged analyzer DLL via ILSpy/dotnet-ilspy) for `CA5373` to confirm whether the analyzer has a target-framework guard that skips execution on .NET 9+ or when the `[Obsolete]` attribute is detected, which would explain the diagnostic being suppressed internally before it can fire.

5. Review the `EnforceOnBuild` metadata for CA5373 in the `Microsoft.CodeAnalysis.NetAnalyzers` NuGet package (check `build/config/*.editorconfig` or the analyzer's `DiagnosticDescriptor`) to confirm whether the rule is set to `EnforceOnBuild = false` or otherwise excluded from build enforcement in 10.0.x.

6. Try an alternative violation pattern that avoids the deprecated constructor overload: use `PasswordDeriveBytes` via reflection or a factory method if one exists that does not itself carry `[Obsolete]`, to isolate whether the SYSLIB obsolescence is the sole reason CA5373 is suppressed.

7. Search the dotnet/roslyn-analyzers GitHub issue tracker and changelog for CA5373 to find any documented decision to retire or defer the rule in favour of SYSLIB0041, and record the issue or PR URL as the canonical source for the Untestable reason if found.

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
