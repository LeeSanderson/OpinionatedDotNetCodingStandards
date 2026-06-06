## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2302 ("Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `EnsureBinaryFormatterBinderIsSetBeforeDeserialize` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow/taint analysis variant of CA2301 that also requires tracking Binder assignment across statements; the underlying BinaryFormatter is additionally blocked by SYSLIB0011 as described in CA2300"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2302", "Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2302",
        Untestable = "Data-flow/taint analysis variant of CA2301 that also requires tracking Binder assignment across statements; the underlying BinaryFormatter is additionally blocked by SYSLIB0011 as described in CA2300")]
    public async Task EnsureBinaryFormatterBinderIsSetBeforeDeserialize()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            #pragma warning disable SYSLIB0011
            using System.IO;
            using System.Runtime.Serialization.Formatters.Binary;
            namespace test;
            public static class Program
            {
                public static object? Deserialize(Stream s, bool useBinder)
                {
                    var formatter = new BinaryFormatter();
                    if (useBinder)
                        formatter.Binder = null;
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0011
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2302").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where `BinaryFormatter` is constructed and `Deserialize` is called in consecutive statements with no `Binder` assignment at all (eliminating the conditional branch), while keeping `#pragma warning disable SYSLIB0011` in place, and check whether CA2302 fires.
2. Try a pattern where `Binder` is explicitly assigned `null` unconditionally before calling `Deserialize` (i.e. `formatter.Binder = null; formatter.Deserialize(s);`) to confirm whether the rule requires taint-tracking of conditional assignments or fires on any null-Binder path.
3. Confirm whether the SYSLIB0011 suppression is sufficient by checking if CA2302 appears in the build output once SYSLIB0011 is suppressed; if CA2302 is still absent, test on a .NET 8 target framework where `BinaryFormatter` may not yet be marked obsolete and SYSLIB0011 may not interfere.
4. Check the NetAnalyzers source on GitHub (dotnet/roslyn-analyzers) for the CA2302 analyzer implementation to determine whether `EnforceOnBuild` is set to `false` or whether the rule is gated behind a target-framework guard that disables it on modern .NET versions.
5. Search the NetAnalyzers changelog and GitHub issues for CA2302 to find any known regression, intentional disablement, or documentation that the rule requires interprocedural taint analysis beyond what the build-time analyzer performs.
6. If the rule is confirmed to require cross-boundary taint analysis that the build-time analyzer does not implement, document the specific source location (file and line) or GitHub issue link that confirms this, and update the `Untestable` reason accordingly.

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
