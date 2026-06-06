## Parent PRD

`issues/prd.md`

## Type

AFK — all investigation can be done by running the test harness and reading analyzer/Roslyn source.

## What to build

Deep-analyse CA5381 ("Ensure Certificates Are Not Added To Root Certificate Store") to determine why it is currently untestable, then either fix the test so it passes or update the Untestable note with a confirmed, well-sourced reason.

**Current state:** The test method `EnsureCertificatesAreNotAddedToRootCertificateStore` in the test suite is marked `[Fact(Skip = "untestable")]`. The current Untestable reason is: "Data-flow/taint analysis variant of CA5380: fires when a certificate is added to a store whose StoreName comes through a variable rather than a constant; the build harness cannot trigger inter-procedural taint analysis tracing the store name through assignments"

## Current test code

```csharp
    [Fact(Skip = "untestable")]
    [RuleDoc("CA5381", "Ensure Certificates Are Not Added To Root Certificate Store",
        HelpLink = "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5381",
        Untestable = "Data-flow/taint analysis variant of CA5380: fires when a certificate is added to a store whose StoreName comes through a variable rather than a constant; the build harness cannot trigger inter-procedural taint analysis tracing the store name through assignments")]
    public async Task EnsureCertificatesAreNotAddedToRootCertificateStore()
    {
        using var project = await CreateProjectBuilder();
        await project.AddFile(
            "Program.cs",
            """
            using System.Security.Cryptography.X509Certificates;
            namespace test;
            public static class Program
            {
                public static void AddCertificate(X509Certificate2 cert, StoreName storeName)
                {
                    var store = new X509Store(storeName, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(cert);
                }
                public static int Main() => 0;
            }
            """);
        var buildOutput = await project.BuildAndGetOutput();

        buildOutput.HasError("CA5381").ShouldBeTrue();
    }
```

## Investigation plan

1. Try a same-method direct source-to-sink pattern where `StoreName.Root` is assigned to a local variable and that variable is used in the `X509Store` constructor in the same method body — e.g. `var name = StoreName.Root; var store = new X509Store(name, StoreLocation.LocalMachine);` — to determine whether single-method taint analysis is sufficient to trigger CA5381.
2. Try an even simpler inlined pattern where no variable is used at all and `StoreName.Root` is passed directly as a literal constant to the `X509Store` constructor, to confirm whether CA5381 overlaps with CA5380 on constant inputs or requires a taint path.
3. Check the NetAnalyzers source (under `src/NetAnalyzers/Core/Microsoft.NetFramework.Analyzers/DoNotUseInsecureDtdProcessingAnalyzer.cs` or the CA5381-specific file) for the `EnforceOnBuild` metadata attribute value — if it is `false` or missing the diagnostic will never surface during `dotnet build`.
4. Confirm whether CA5381 requires cross-method (inter-procedural) taint analysis by tracing the call from `AddCertificate`'s `storeName` parameter back to its call sites; if the harness only compiles a single file with no callers, the taint source is unresolvable and the rule will never fire.
5. Test on an older NetAnalyzers version (e.g. 8.x pinned via `<PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.*" />`) to determine whether the taint analysis engine was present and operational in earlier releases, which would distinguish a regression from a design limitation.
6. Search the `dotnet/roslyn-analyzers` GitHub issue tracker for CA5381 to find any documented decision about inter-procedural taint depth limits or intentional scope restrictions that explain why the rule cannot fire in a build-time scenario.
7. If none of the above patterns trigger the diagnostic, update the `Untestable` reason in the `[RuleDoc]` attribute with the confirmed root cause — citing the specific `EnforceOnBuild` value found in NetAnalyzers source or the relevant GitHub issue URL — so the reason is traceable and verifiable.

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
