# Opinionated DotNet CodingStandards

A NuGet development-dependency package that bundles five Roslyn analyzer packages
together with a curated set of editorconfig rules, MSBuild props, and targets so
every project that references it gets consistent code-quality enforcement out of the
box.

[![Build Status](https://github.com/LeeSanderson/OpinionatedDotNetCodingStandards/actions/workflows/ci.yml/badge.svg)](https://github.com/LeeSanderson/OpinionatedDotNetCodingStandards/actions/workflows/ci.yml)
![Nuget](https://img.shields.io/nuget/dt/Opinionated.DotNet.CodingStandards)
![Nuget](https://img.shields.io/nuget/v/Opinionated.DotNet.CodingStandards)


**Bundled analyzers**

| Package | Purpose | License |
|---------|---------|---------|
| [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer) | Best-practice and correctness rules | MIT |
| [Microsoft.CodeAnalysis.BannedApiAnalyzers](https://github.com/dotnet/roslyn-analyzers) | Banned-API enforcement | MIT |
| [Microsoft.CodeAnalysis.NetAnalyzers](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview) | .NET platform quality rules | MIT |
| [SonarAnalyzer.CSharp](https://github.com/SonarSource/sonar-dotnet) | Complexity metrics and maintainability rules | LGPL-3.0 |
| [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) | Style and naming conventions | MIT |

> **License note:** `SonarAnalyzer.CSharp` is licensed under LGPL-3.0. It runs at build
> time only and is not linked into or redistributed with your shipped binaries. The
> package's own license remains MIT.

See [`docs/rule-reference.md`](docs/rule-reference.md) for the full list of enforced
rules and their severities.

---

## Building and testing locally

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# Restore, build, and run all tests
dotnet build
dotnet test

# Pack the NuGet package (output goes to src/.../bin/Debug/)
dotnet pack src/Opinionated.DotNet.CodingStandards/Opinionated.DotNet.CodingStandards.csproj
```

### Maintenance scripts

Run these from the repository root:

```bash
# Verify NuGet dependency versions in nuspec match Directory.Packages.props
dotnet ./scripts/CheckNugetDependenciesMatchProps.cs

# Regenerate the analyzer editorconfigs after bumping an analyzer package
dotnet ./scripts/UpdateAnalyzerEditorConfigs.cs

# Regenerate docs/rule-reference.md after editing an analyzer editorconfig
dotnet ./scripts/GenerateRuleReference.cs

# Verify the editorconfigs and docs/rule-reference.md are in sync (same check as CI):
# regenerate both, then confirm nothing changed
dotnet ./scripts/UpdateAnalyzerEditorConfigs.cs
dotnet ./scripts/GenerateRuleReference.cs
git diff --exit-code -- packages/Opinionated.DotNet.CodingStandards/pkgsrc/config/analyzers/ docs/

# Verify MSBuild import paths resolve with correct casing
dotnet ./scripts/CheckImportPathCasing.cs
```

Both generators are idempotent, so regenerating and diffing is the reliable freshness check —
that is exactly what CI asserts. `GenerateRuleReference.cs` has no check mode (it ignores any
arguments and always rewrites). `UpdateAnalyzerEditorConfigs.cs` does accept `--check`, but on a
Windows checkout with `core.autocrlf=true` it reports drift unconditionally: it compares its
LF output against the CRLF file in the working tree. Prefer the regenerate-and-diff form above.

---

## Repository layout

```
.github/workflows/
    ci.yml              Per-PR build, test, pack, and freshness checks
    release.yml         Tag-triggered (v*) build, pack, and publish to NuGet.org

src/
    Opinionated.DotNet.CodingStandards/
                        MSBuild project that produces the NuGet package

packages/
    Opinionated.DotNet.CodingStandards/
        pkgsrc/
            build/      MSBuild .props and .targets injected at build time
            buildTransitive/
                        MSBuild assets that flow through ProjectReferences
            config/
                analyzers/
                        Per-analyzer .editorconfig files that set rule severities
            README.md   Consumer-facing README rendered on NuGet.org

docs/
    rule-reference.md   Generated table of every enforced rule (do not edit by hand;
                        run scripts/GenerateRuleReference.cs to refresh)

scripts/
    CheckImportPathCasing.cs
    CheckNugetDependenciesMatchProps.cs
    GenerateRuleReference.cs
    UpdateAnalyzerEditorConfigs.cs
    New-ReleaseTag.ps1
    Remove-LastReleaseTag.ps1
    Report-OutdatedPackages.ps1

tests/
    Opinionated.DotNet.CodingStandards.Tests/
                        Integration tests that pack the package and run it against
                        real .NET projects to verify analyzer behaviour
```

---

## Release process

Releases are tag-driven. Pushing a `v*` tag (e.g. `v1.2.3`) triggers the release
stage in CI which:

1. Injects the version from the tag into the NuGet package via the `NuspecProperties`
   mechanism.
2. Builds and tests the solution; publishing is skipped if either fails.
3. Pushes the package to NuGet.org.
4. Creates a GitHub Release populated from `CHANGELOG.md`.

To cut a release:

```bash
git tag v1.2.3
git push origin v1.2.3
```

Update `CHANGELOG.md` before tagging to ensure the GitHub Release notes are accurate.
