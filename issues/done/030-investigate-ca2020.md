## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2020 ("Prevent behavioral change caused by built-in operators of IntPtr and UIntPtr") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `PreventBehavioralChangeFromIntPtrOperators` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "CA2020 does not fire in NetAnalyzers 10.0.x build analysis for nint/nuint arithmetic in checked or unchecked expressions; the rule targets behavioral changes introduced between .NET 5 and .NET 7 but does not produce diagnostics for projects already targeting .NET 7+"

## Current test code

```csharp
[Fact(Skip = "untestable")]
    [RuleDoc("CA2020", "Prevent behavioral change caused by built-in operators of IntPtr and UIntPtr",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2020",
        Untestable = "CA2020 does not fire in NetAnalyzers 10.0.x build analysis for nint/nuint arithmetic in checked or unchecked expressions; the rule targets behavioral changes introduced between .NET 5 and .NET 7 but does not produce diagnostics for projects already targeting .NET 7+")]
    public async Task PreventBehavioralChangeFromIntPtrOperators()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            namespace test;
            public static class Program
            {
                public static nint Add(nint a, nint b) => unchecked(a + b);
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2020").ShouldBeTrue();
    }
```

## Investigation plan

1. Check the NetAnalyzers GitHub source for CA2020 (`OperatorOverloadingAnalyzer` or similar) and locate its `EnforceOnBuild` metadata — confirm whether the rule is marked `Never`, `WhenExplicitlyConfigured`, or `Always`; a `Never` value means it will not fire in build analysis regardless of the violation pattern.
2. Try alternative violation patterns that more closely match the documented behavioral change: use `IntPtr` and `UIntPtr` directly (not `nint`/`nuint` aliases) with explicit arithmetic operators such as `IntPtr.Add`, the `+` operator, or explicit casts between `IntPtr` and `int`, since the rule targets the pre-.NET 7 operator semantics mismatch rather than generic checked/unchecked arithmetic.
3. Check for target-framework guards in the CA2020 analyzer source — the rule exists specifically to catch code that behaves differently between .NET 5/6 and .NET 7+; if the analyzer skips analysis when the project already targets `net7.0` or later (as the test harness likely does), try overriding the target framework to `net6.0` or `net5.0` in `CreateProjectBuilder` to see whether the diagnostic fires on an older TFM.
4. Test on NetAnalyzers 8.x by temporarily downgrading the `Microsoft.CodeAnalysis.NetAnalyzers` package reference in the test project and re-running the build, to determine whether the rule fired in an earlier version and was subsequently changed or removed.
5. Search the NetAnalyzers changelog and GitHub issues for CA2020 to find any explicit decision to disable build enforcement (e.g., a closed issue or PR noting that the rule is analysis-only or IDE-only), and record the URL of any such finding in the Untestable reason.
6. Confirm whether the absence of a diagnostic is limited to `nint`/`nuint` arithmetic or also covers `IntPtr`/`UIntPtr` operator calls by writing a second minimal violation using `checked((IntPtr)int.MaxValue + 1)` and checking whether that pattern appears in SARIF output.
7. If no pattern produces a diagnostic across all tested versions and TFMs, update the `Untestable` reason with the confirmed root cause (the `EnforceOnBuild` value and/or the GitHub issue or source location that explains the design decision) and remove the speculative language from the current note.

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
