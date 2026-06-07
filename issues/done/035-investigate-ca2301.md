## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA2301 ("Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `ProhibitBinaryFormatterDeserializeWithoutBinder` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; same SYSLIB0011/TreatWarningsAsErrors constraint as CA2300; cannot be tested without suppressing SYSLIB0011 globally"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA2301", "Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2301",
        Untestable = "BinaryFormatter is marked [Obsolete(SYSLIB0011)] in .NET 9+; same SYSLIB0011/TreatWarningsAsErrors constraint as CA2300; cannot be tested without suppressing SYSLIB0011 globally")]
    public async Task ProhibitBinaryFormatterDeserializeWithoutBinder()
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
                public static object? Deserialize(Stream s)
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(s);
                }
                public static int Main() => 0;
            }
            #pragma warning restore SYSLIB0011
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA2301").ShouldBeTrue();
    }
```

## Investigation plan

1. Run the current test without the `Skip` attribute and capture the full build output to confirm whether CA2301 is absent from the SARIF/output or whether SYSLIB0011 prevents compilation entirely, distinguishing between a suppression-gap problem and a "rule does not fire" problem.

2. Verify that the `#pragma warning disable SYSLIB0011` block in the test code actually suppresses the obsolete-API error at build time — check the raw `BuildAndGetOutput` result for CS0618/SYSLIB0011 diagnostics that escape the pragma, and if any remain, try wrapping the entire file body in the suppress/restore pair or passing `<NoWarn>SYSLIB0011</NoWarn>` via the project builder's MSBuild property API.

3. Check whether CA2301 appears in the build output once SYSLIB0011 is fully suppressed — if the rule is absent, inspect the NetAnalyzers 10.x source (search for `CA2301` in `microsoft/roslyn-analyzers` on GitHub) and look for any `[TargetFramework]` guards or `<EnforceOnBuild>` metadata that disables the rule when `BinaryFormatter` is obsolete.

4. Try an explicit violation pattern that sets `Binder` to `null` after construction to confirm whether the analyzer fires on any `BinaryFormatter.Deserialize` call or only when `Binder` is provably unset — this narrows whether the rule has been silently superseded by CA2300 (which fires unconditionally) in .NET 9+ analyzers.

5. Check the `EnforceOnBuild` value for CA2301 in the NetAnalyzers NuGet package shipped with .NET 10 SDK (inspect `%(PackagePath)/build/Microsoft.CodeAnalysis.NetAnalyzers.props` or the SARIF rule metadata) to determine whether the rule is intentionally disabled for build enforcement in recent versions.

6. Test against a .NET 8 target framework (`<TargetFramework>net8.0</TargetFramework>`) where `BinaryFormatter` carries only a warning-level obsolete, not the error-level SYSLIB0011 treatment, to check whether CA2301 fires cleanly on that TFM without any pragma suppression.

7. If CA2301 is confirmed to never fire alongside SYSLIB0011 suppression on .NET 9+ regardless of pattern or TFM, update the `Untestable` reason to cite the specific NetAnalyzers source location or GitHub issue that shows the rule was intentionally subsumed by CA2300 or disabled for .NET 9+ runtimes.

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
