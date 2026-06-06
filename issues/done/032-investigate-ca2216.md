## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2216 ("Disposable types should declare finalizer") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `DisposableTypesShouldDeclareFinalizer` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA2216 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern: IDisposable classes with IntPtr, UIntPtr, or HandleRef fields but no finalizer emit CA1063 (wrong Dispose pattern) instead, and even with a correct Dispose(bool) pattern, CA2216 is never emitted in the SARIF output; the rule appears suppressed for .NET 10 targets"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA2216", "Disposable types should declare finalizer",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2216",
        Untestable = "CA2216 does not fire in NetAnalyzers 10.0.x build analysis for any tested code pattern: IDisposable classes with IntPtr, UIntPtr, or HandleRef fields but no finalizer emit CA1063 (wrong Dispose pattern) instead, and even with a correct Dispose(bool) pattern, CA2216 is never emitted in the SARIF output; the rule appears suppressed for .NET 10 targets")]
    public async Task DisposableTypesShouldDeclareFinalizer()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System;
            using System.Runtime.InteropServices;
            namespace test;
            public class NativeResource : IDisposable
            {
                private IntPtr _handle;
                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
                protected virtual void Dispose(bool disposing) { }
            }
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2216").ShouldBeTrue();
    }
```

## Investigation plan

1. Understand which rule fires instead: run the current test code (without the Skip) and capture the full SARIF/build output to confirm that CA1063 is the rule that fires and note exactly which field type and Dispose pattern triggers it, establishing the subsuming relationship.

2. Find a pattern that triggers CA2216 but not CA1063: try violation patterns where the Dispose implementation is already correct enough to satisfy CA1063 (i.e. it correctly calls `GC.SuppressFinalize` and follows the full pattern) but the class still lacks a finalizer — vary field types across `IntPtr`, `UIntPtr`, and `HandleRef` to see if any combination bypasses CA1063 and surfaces CA2216.

3. Check the NetAnalyzers source for the CA2216 analyzer and CA1063 analyzer to determine whether CA2216 is explicitly suppressed when CA1063 would also fire, or whether CA2216 has a target-framework guard that disables it for `net10.0` targets — look for `EnforceOnBuild` metadata and any `TargetFramework` conditionals in the analyzer implementation or its descriptor.

4. Test against an older NetAnalyzers version: temporarily pin `Microsoft.CodeAnalysis.NetAnalyzers` to `8.0.x` in the generated project (via `CreateProjectBuilder` options or a direct `PackageReference` override) and run the same violation pattern to determine whether CA2216 was ever emittable in build analysis mode, establishing whether this is a regression or a longstanding gap.

5. Check the NetAnalyzers GitHub changelog and issue tracker for CA2216 to find any deliberate decision to suppress or retire the rule for modern .NET targets — search for issues referencing CA2216 and any milestone that changed its `EnforceOnBuild` value from `true` to `false` or `WhenExplicitlyConfigured`.

6. Attempt explicit severity escalation: add `dotnet_diagnostic.CA2216.severity = error` to the generated project's `.editorconfig` (via the test harness's editorconfig injection) and re-run to determine whether the rule is present but set to a non-error severity by default, or truly absent from the analysis output.

7. Document the confirmed root cause: based on findings from steps 1–6, either update the test with a working violation pattern and remove `Skip`, or replace the Untestable reason with the specific source location (file and line in the NetAnalyzers repo) or GitHub issue URL that confirms the rule is permanently suppressed for the target framework.

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
