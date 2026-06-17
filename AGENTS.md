# AGENTS.md

## What this is

`Opinionated.DotNet.CodingStandards` is a NuGet **development-dependency package** — not
a code library. It bundles five Roslyn analyzer packages (Meziantou.Analyzer,
Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers,
SonarAnalyzer.CSharp, StyleCop.Analyzers) plus `.editorconfig` files and MSBuild
`.props`/`.targets`. When installed it turns on strict compiler settings and enforces
code-style, quality, banned-API, and complexity rules on the consuming project.
`SonarAnalyzer.CSharp` is LGPL-3.0; it runs at build time only and is not redistributed.

The package emits **no production assembly** — the shipped payload is the files under
`packages/Opinionated.DotNet.CodingStandards/pkgsrc/`.

## Stack

- C# / .NET. The package project targets `netstandard2.0`; the test project targets `net10.0`.
- Packaging is driven by a hand-maintained `.nuspec` (not auto-generated metadata).
- Tests: **xUnit + Shouldly + CliWrap**. They pack the real package, build a throwaway
  project that references it, and assert on the SARIF build output.

## Commands

```powershell
dotnet build Opinionated.DotNet.CodingStandards.slnx          # build (warnings = errors)
dotnet test  Opinionated.DotNet.CodingStandards.slnx          # run the integration tests
dotnet test  --filter "FullyQualifiedName~CodeAnalysisRulesDesign"  # run one test class
dotnet test  --filter "FullyQualifiedName~CodeAnalysisRules"   # run a subdirectory of classes
dotnet ./scripts/CheckNugetDependenciesMatchProps.cs          # verify nuspec deps == Directory.Packages.props
```

CI builds with `-c Release`. Tests spin up real `dotnet pack`/`dotnet build` processes,
so they need network access to nuget.org and are slower than typical unit tests (~40 min full suite).

## Test speed

**Always verify a new test in isolation first** using `--filter` before running the full suite:

```powershell
dotnet build
dotnet test --no-build --filter "FullyQualifiedName~MyNewTestMethod"
```

**Skip the full suite and commit directly** when ALL of the following hold — these changes
cannot regress existing tests:

1. The only source changes are new `[Fact]` methods added to existing `*Should.cs` test classes
   (and/or removing the corresponding `[RuleDoc]` entry from `UntestableRules.cs`)
2. No changes to shared test helpers: `ProjectBuilder`, `PackageFixture`, `PathHelpers`,
   `RuleReferenceGenerator`, `RuleDocAttribute`, or any file under `tests/*/Helpers/`
3. No changes to package content: `pkgsrc/`, `*.editorconfig`, `*.nuspec`, `Directory.Packages.props`

If **any** of those conditions is false, run the full suite (`dotnet test`) before committing.

## Project structure

- `src/` — the package project (uses `NuSpecFile`, `GeneratePackageOnBuild` is on, so
  **every build packs**).
- `packages/.../pkgsrc/` — the actual shipped payload: `build/`, `buildTransitive/`,
  `buildMultiTargeting/` MSBuild logic, and `config/*.editorconfig` + `defaultBannedApis/`.
- `tests/` — integration tests and their helpers (`ProjectBuilder`, `PackageFixture`).
  - Top-level `*Should.cs` files cover cross-cutting concerns (happy path, package metadata,
    StyleCop, banned-API).
  - Large test classes are split into logically-grouped files under a folder named after the
    originator: `CodeAnalysisRules/`, `CodingStandards/`, `MeziantouAnalyzers/`,
    `SonarAnalyzerRules/`. Each split file is named `<OriginalClass><Group>Should.cs` and
    its namespace mirrors the folder.
  - `UntestableRules.cs` holds class-level `[RuleDoc]` entries for rules that cannot be
    triggered by the single-project build harness.
- `scripts/` — the dependency-sync check (run as a `dotnet` file-based app).
- The solution **dogfoods itself** via `Directory.Build.props`/`.targets`, which import
  the package's own props/targets.

## Key file paths

| Purpose | Path |
|---------|------|
| Test base class | `tests/Opinionated.DotNet.CodingStandards.Tests/CodingStandardsTestBase.cs` |
| SARIF assertion helper | `tests/Opinionated.DotNet.CodingStandards.Tests/Helpers/BuildOutputFile.cs` |
| Throwaway project builder | `tests/Opinionated.DotNet.CodingStandards.Tests/Helpers/ProjectBuilder.cs` |
| Untestable rules catalog | `tests/Opinionated.DotNet.CodingStandards.Tests/UntestableRules.cs` |
| Analyzer editorconfigs | `packages/Opinionated.DotNet.CodingStandards/pkgsrc/config/analyzers/` |

## Canonical test pattern

```csharp
[Collection(nameof(PackageCollection))]
public class MyAnalyzerRulesShould(PackageFixture fixture, ITestOutputHelper testOutputHelper)
    : CodingStandardsTestBase(fixture, testOutputHelper)
{
    [Fact]
    [RuleDoc("RULEXX", "Rule description", HelpLink = "https://...")]
    public async Task ShortDescriptionOfViolation()
    {
        using var project = await CreateProjectBuilderAsync(
            // optional: properties: [("NuGetAudit", "false")],
            // optional: packageReferences: [("Microsoft.Extensions.Logging.Abstractions", "10.0.0")]
        );
        await project.AddFileAsync("Program.cs", """
            namespace test;
            // ... code that TRIGGERS the rule ...
            public static class Program { public static int Main() => 0; }
            """);
        var buildOutput = await project.BuildAndGetOutputAsync();
        buildOutput.HasError("RULEXX").ShouldBeTrue();
    }
}
```

**`BuildOutputFile` assertion API** (all methods are on the value returned by `BuildAndGetOutputAsync`):
- `buildOutput.HasError("RULEID")` — rule fires as `error` (most common — package sets most rules to `warning` which the ErrorLog treats as `error`)
- `buildOutput.HasWarning("RULEID")` — rule fires as `warning`
- `buildOutput.HasNote("RULEID")` — rule fires as `note`
- `buildOutput.HasError()` — any error exists (used in happy-path / negative tests)

## How to add a new rule test

1. **Find the right file** — pick the `*Should.cs` file for the rule's analyzer family:
   `CodeAnalysisRules/`, `MeziantouAnalyzers/`, `SonarAnalyzerRules/`, or a top-level file
   (e.g. `StyleCopAnalyzersShould.cs`). Keep files under 1000 lines; create a new split file
   if the target is near that limit.
2. **Write the test** using the canonical pattern above.
3. **Remove from `UntestableRules.cs`** if a class-level `[RuleDoc]` for this rule ID already
   exists there.
4. **Verify in isolation** before committing:
   ```powershell
   dotnet build
   dotnet test --no-build --filter "FullyQualifiedName~MyNewTestMethod"
   ```

## Test conventions

Every active rule must be covered by exactly one `[RuleDoc]` attribute — enforced by
`RuleDocCoverageShould`. Keep these rules in mind when adding or modifying tests:

- **Positive tests** get a method-level `[RuleDoc]`:
  ```csharp
  [Fact]
  [RuleDoc("CA1000", "Do not declare static members on generic types",
      HelpLink = "https://learn.microsoft.com/...")]
  public async Task ProhibitStaticMembersOnGenericTypes() { ... }
  ```
- **Negative and toggle tests** must NOT carry `[RuleDoc]`.
- **Untestable rules** — those that cannot fire in the single-project build harness — get a
  class-level `[RuleDoc]` in `UntestableRules.cs` with a non-null `Untestable` reason.
- **One `[RuleDoc]` per rule ID** across the entire test assembly; the coverage test enforces
  uniqueness.
- **Before marking a rule untestable, exhaust the confounder playbook first:** (1) add a
  real `PackageReference` if the rule guards on a type in an external package; (2) use stub
  types for type-existence gates; (3) pin `LangVersion=12` if C# 13 overload routing might
  redirect calls to a new span overload the analyzer can't match. Only record a rule as
  untestable when a structural reason applies:
  - **EnforceOnBuild.Never** — certain IDE rules are tagged `Never` in the Roslyn source;
    they never emit diagnostics at `dotnet build` regardless of editorconfig severity.
  - **IDE Features-assembly gate** — the reporting analyzer lives in
    `Microsoft.CodeAnalysis.CSharp.Features` (IDE/LSP only), not the build-time CodeStyle
    package. Check the source path: `Analyzers/` = builds; `Features/` = IDE-only.
  - **NetFramework mscorlib gate** — rules that check `System.String` is in `mscorlib`
    structurally cannot fire on `net10.0` (it lives in `System.Private.CoreLib`).
  - **Formatter-backed** — rules that emit `IDE0055` instead of their own ID at build time;
    the rule ID itself never appears in SARIF.
- Keep test files under **1000 lines**. If a file grows past that, split it into a new
  subdirectory using the `<OriginalClass><Group>Should` naming convention above.

## Code style

This repo enforces its own standards — the build treats analyzer warnings as errors.
Don't fight the analyzers; write code that passes them: file-scoped namespaces, `var`,
braces on all control flow, language keywords over BCL type names, `readonly` fields,
correct modifier order. If a rule genuinely shouldn't apply, change the editorconfig
deliberately rather than suppressing inline.

## Critical constraints

- **Keep `.nuspec` `<dependencies>` in sync with `Directory.Packages.props`.** Versions
  must match exactly; the script above and a CI step enforce it. Update both together.
- **A given .NET analysis rule can be configured only once** across all the imported
  global editorconfig files — don't duplicate a `dotnet_diagnostic.<id>.severity` line in
  two files under `config/`.
- The `pkgsrc/**` glob in the `.nuspec` ships everything under `pkgsrc/` — anything added
  there becomes part of the public package.
- **InternalsVisibleTo auto-injection.** The package auto-injects an `InternalsVisibleTo`
  attribute pointing at the test project, silently suppressing friend-assembly-sensitive
  CA rules. Opt back in via `ignore_internalsvisibleto=true` in the `ProjectBuilder` call.

## Git & boundaries

- Branch from `main`; open a PR. Commit/push only when asked.
- **Never commit secrets** (e.g. the NuGet.org API key) — they belong in CI secrets, not
  the repo.
- Skills under `.claude/skills/` are intentionally **.NET-generic and project-agnostic** —
  keep them free of project-specific class names when editing.
