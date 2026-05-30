# AGENTS.md

## What this is

`Opinionated.DotNet.CodingStandards` is a NuGet **development-dependency package** — not
a code library. It bundles four Roslyn analyzer packages (Meziantou.Analyzer,
Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers,
StyleCop.Analyzers) plus `.editorconfig` files and MSBuild `.props`/`.targets`. When
installed it turns on strict compiler settings and enforces code-style, quality, and
banned-API rules on the consuming project.

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
dotnet test  --filter "FullyQualifiedName~CodeAnalysisRules"  # run a single test class
dotnet ./scripts/CheckNugetDependenciesMatchProps.cs          # verify nuspec deps == Directory.Packages.props
```

CI builds with `-c Release`. Tests spin up real `dotnet pack`/`dotnet build` processes,
so they need network access to nuget.org and are slower than typical unit tests.

## Project structure

- `src/` — the package project (uses `NuSpecFile`, `GeneratePackageOnBuild` is on, so
  **every build packs**).
- `packages/.../pkgsrc/` — the actual shipped payload: `build/`, `buildTransitive/`,
  `buildMultiTargeting/` MSBuild logic, and `config/*.editorconfig` + `defaultBannedApis/`.
- `tests/` — integration tests and their helpers (`ProjectBuilder`, `PackageFixture`).
- `scripts/` — the dependency-sync check (run as a `dotnet` file-based app).
- The solution **dogfoods itself** via `Directory.Build.props`/`.targets`, which import
  the package's own props/targets.

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

## Git & boundaries

- Branch from `main`; open a PR. Commit/push only when asked.
- **Never commit secrets** (e.g. the NuGet.org API key) — they belong in CI secrets, not
  the repo.
- Skills under `.claude/skills/` are intentionally **.NET-generic and project-agnostic** —
  keep them free of project-specific class names when editing.
