## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA1419 ("Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `RequireParameterlessConstructorOnSafeHandleSubclass` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA1419 does not fire in NetAnalyzers 10.0.x build analysis for a concrete public SafeHandle subclass without a parameterless constructor; exhaustive probing confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1419.severity = warning configured"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA1419", "Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1419",
        Untestable = "CA1419 does not fire in NetAnalyzers 10.0.x build analysis for a concrete public SafeHandle subclass without a parameterless constructor; exhaustive probing confirms the diagnostic is absent from SARIF output even with dotnet_diagnostic.CA1419.severity = warning configured")]
    public async Task RequireParameterlessConstructorOnSafeHandleSubclass()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Runtime.InteropServices;
            namespace test;
            public class MyHandle : SafeHandle
            {
                public MyHandle(System.IntPtr handle) : base(System.IntPtr.Zero, true)
                {
                    SetHandle(handle);
                }
                public override bool IsInvalid => handle == System.IntPtr.Zero;
                protected override bool ReleaseHandle() { return true; }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA1419").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the `EnforceOnBuild` metadata for CA1419 in the NetAnalyzers source (search `dotnet/roslyn-analyzers` on GitHub for `CA1419` in `Interoperability.CSharp` or `Interoperability` category packages) to confirm whether the rule is flagged as build-enforceable or IDE/live-analysis only. A rule with `EnforceOnBuild = false` will never fire during `dotnet build` regardless of severity configuration.

2. Try alternative violation patterns to see if any produce a diagnostic. Specifically: (a) a class derived from `SafeHandle` where the only constructor is `private` rather than `public`, (b) an `internal` class with no parameterless constructor, (c) a class where the parameterless constructor exists but is `private` while the class is `public`. Run each variant through `BuildAndGetOutput` and inspect SARIF.

3. Test against an older NetAnalyzers version (e.g. 8.0.x) by temporarily pinning the `Microsoft.CodeAnalysis.NetAnalyzers` package version in the generated project (if `CreateProjectBuilder` supports a `packageReferences` or `analyzerVersion` parameter) to determine whether CA1419 fired in older releases and was subsequently changed to IDE-only enforcement.

4. Search the NetAnalyzers GitHub repository for any issues or pull requests that changed CA1419 from `EnforceOnBuild = true` to `EnforceOnBuild = false`, or that added a target-framework guard (e.g. restricting the rule to `net6.0+` or `net7.0+`) that may interact with the test project's TFM. Check whether the test project targets a TFM where `SafeHandle` subclass analysis is active.

5. Check for target-framework guards inside the CA1419 analyzer implementation (the `InteropServicesSafeHandleAnalyzer` or similarly named class in `roslyn-analyzers`) to see if the rule is gated on a specific runtime or SDK version that differs from what the test harness uses.

6. If the rule is confirmed IDE-only (`EnforceOnBuild = false`), update the `Untestable` reason with the specific source location or GitHub issue link that proves this, and leave `[Fact(Skip = "untestable")]` in place with the improved explanation.

7. If a working violation pattern is found in any of the above steps, remove `[Fact(Skip = "untestable")]`, replace the test body with the working pattern, verify the test passes in CI, and confirm that the `RuleReferenceGenerator` coverage test continues to pass.

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
