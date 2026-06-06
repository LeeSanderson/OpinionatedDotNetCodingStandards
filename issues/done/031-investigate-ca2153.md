## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2153 ("Do Not Catch Corrupted State Exceptions") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitCatchingCorruptedStateExceptions` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA2153 does not fire in NetAnalyzers 10.0.x build analysis for catch blocks that catch AccessViolationException or other corrupted-state exceptions; in .NET 6+ the runtime changed CSE behavior and the analyzer does not emit this diagnostic for modern targets"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA2153", "Do Not Catch Corrupted State Exceptions",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2153",
        Untestable = "CA2153 does not fire in NetAnalyzers 10.0.x build analysis for catch blocks that catch AccessViolationException or other corrupted-state exceptions; in .NET 6+ the runtime changed CSE behavior and the analyzer does not emit this diagnostic for modern targets")]
    public async Task ProhibitCatchingCorruptedStateExceptions()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            namespace test;
            public static class Program
            {
                public static void M()
                {
                    try { }
                    catch (AccessViolationException) { }
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2153").ShouldBeTrue();
    }
```

## Investigation plan

1. Verify the exact diagnostic output by running the test project build with `--verbosity diagnostic` and checking whether CA2153 appears anywhere in the build log, SARIF file, or is silently suppressed.
2. Try alternative violation patterns beyond catching `AccessViolationException` directly: catch `Exception` with a `HandleProcessCorruptedStateExceptions` attribute on the method (the legacy .NET Framework trigger), and a bare `catch` clause, to determine if any pattern produces the CA2153 diagnostic in the current analyzer version.
3. Check the NetAnalyzers source on GitHub (dotnet/roslyn-analyzers) for the CA2153 analyzer implementation — specifically look for target-framework guards (`IsFrameworkType`, TFM version checks, or `#if` conditions) that may disable the diagnostic for `net6.0` and later targets, and note the source file and line.
4. Check the `EnforceOnBuild` metadata for CA2153 in the NetAnalyzers package (look in `Microsoft.CodeAnalysis.NetAnalyzers.sarif` or `RulesMissingDocumentation.md` in the package) to confirm whether the rule is marked as `EnforceOnBuild = Never`, which would explain why build analysis never emits it.
5. Test on an older NetAnalyzers version (e.g., 8.x) by temporarily pinning the package reference in the generated project, and check whether CA2153 fires there — this isolates whether the suppression is version-specific or TFM-specific.
6. Search the dotnet/roslyn-analyzers GitHub issues and changelog for any explicit decision to disable CA2153 for .NET 6+ targets or modern TFMs, and record the issue or PR URL as evidence for the Untestable reason.
7. If a firing pattern is found in any of the above steps, update the test to use that pattern, remove the `Skip`, and confirm the test passes. If no pattern fires, update the `Untestable` string with the specific root cause (source file path or GitHub issue link) found in steps 3–6.

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
