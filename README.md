# Opinionated.DotNet.CodingStandards

A NuGet development-dependency package that bundles four Roslyn analyzer packages
together with a curated set of editorconfig rules, MSBuild props, and targets so
every project that references it gets consistent code-quality enforcement out of the
box.

**Bundled analyzers**

| Package | Purpose |
|---------|---------|
| [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer) | Best-practice and correctness rules |
| [Microsoft.CodeAnalysis.BannedApiAnalyzers](https://github.com/dotnet/roslyn-analyzers) | Banned-API enforcement |
| [Microsoft.CodeAnalysis.NetAnalyzers](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview) | .NET platform quality rules |
| [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) | Style and naming conventions |

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

# Regenerate docs/rule-reference.md after editing an analyzer editorconfig
dotnet ./scripts/GenerateRuleReference.cs

# Verify docs/rule-reference.md is in sync with the editorconfigs (same check as CI)
dotnet ./scripts/CheckRuleReferenceFreshness.cs
```

---

## Repository layout

```
.azure-pipelines/
    ci.yml              Per-commit build, test, pack, and freshness checks
    outdated.yml        Scheduled weekly pipeline to report stale NuGet packages

src/
    Opinionated.DotNet.CodingStandards/
                        MSBuild project that produces the NuGet package

packages/
    Opinionated.Dotnet.CodingStandards/
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
    CheckNugetDependenciesMatchProps.cs
    CheckRuleReferenceFreshness.cs
    GenerateRuleReference.cs

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
