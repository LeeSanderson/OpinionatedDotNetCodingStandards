---
name: update-nuget-packages
description: Check NuGet analyzer packages for newer versions, update Directory.Packages.props and .nuspec, regenerate editorconfigs, then write a PRD and per-rule issues for every new rule discovered. Use when the user wants to bump analyzer dependencies or asks "are there any NuGet updates?".
---

# Update NuGet Packages

Check the five analyzer NuGet packages for new versions, apply any updates, regenerate editorconfigs, and create work items for every newly-added rule.

## 1. Read current versions

Read `Directory.Packages.props` to capture the current versions of all five analyzer packages:

- `Meziantou.Analyzer`
- `Microsoft.CodeAnalysis.BannedApiAnalyzers`
- `Microsoft.CodeAnalysis.NetAnalyzers`
- `SonarAnalyzer.CSharp`
- `StyleCop.Analyzers`

## 2. Check NuGet for newer versions

For each package, fetch its version list from NuGet:

```
GET https://api.nuget.org/v3-flatcontainer/{package-id-lowercased}/index.json
```

Parse the `versions` array. Strategy per package:

- **Stable packages** (`Meziantou.Analyzer`, `Microsoft.CodeAnalysis.BannedApiAnalyzers`, `Microsoft.CodeAnalysis.NetAnalyzers`, `SonarAnalyzer.CSharp`): select the highest version that has no pre-release suffix (no `-` in the version string).
- **Pre-release packages** (`StyleCop.Analyzers`): select the highest overall version (including pre-release), because the project intentionally tracks pre-release builds.

Compare to current. Collect a list of packages that have a newer version available.

If no packages need updating, output a summary and stop:
```
All analyzer packages are already up to date:
  Meziantou.Analyzer: 3.0.104 (current)
  Microsoft.CodeAnalysis.BannedApiAnalyzers: 4.14.0 (current)
  ...
```

## 3. Create and switch to a feature branch

Before modifying any files, create a branch for this update run:

1. Derive today's date in `YYYY-MM-DD` format (use the `currentDate` from the system context or run `Get-Date -Format "yyyy-MM-dd"`).
2. Set the branch name to `feat/bump-analyzers-YYYY-MM-DD` (substituting the real date).
3. Check whether the branch already exists:
   ```powershell
   git show-ref --verify --quiet refs/heads/feat/bump-analyzers-YYYY-MM-DD
   ```
4. If the branch **does not exist** → create it and switch:
   ```powershell
   git checkout -b feat/bump-analyzers-YYYY-MM-DD
   ```
5. If the branch **already exists** (e.g. a prior run on the same day) → switch to it without creating a duplicate:
   ```powershell
   git checkout feat/bump-analyzers-YYYY-MM-DD
   ```

Then continue with the rest of the workflow on this branch.

> **This step is skipped entirely when no updates were found in step 2.**

## 4. Update version files

For each package with a new version, update **both** files in a single pass — they must never drift:

### `Directory.Packages.props`

Change the `version=` attribute on the matching `<PackageReference>` element.

### `packages/Opinionated.DotNet.CodingStandards/Opinionated.DotNet.CodingStandards.nuspec`

Change the `version=` attribute on the matching `<dependency>` element inside `<metadata>/<dependencies>`.

## 5. Run the editorconfig update script

```powershell
dotnet ./scripts/UpdateAnalyzerEditorConfigs.cs
```

Capture stdout. The output reports, for each editorconfig, what rule IDs were added and removed:

```
SonarAnalyzer.CSharp.editorconfig:
  Added: S1001, S1234, ...
  Stale: S9999
  Written.
```

Parse all `Added:` lines across every editorconfig to collect the complete set of newly-added rule IDs.

If the script errors with "No analyzer packages resolved", restore first:

```powershell
dotnet restore && dotnet ./scripts/UpdateAnalyzerEditorConfigs.cs
```

## 6. Verify package-version sync

```powershell
dotnet ./scripts/CheckNugetDependenciesMatchProps.cs
```

If this fails, the `Directory.Packages.props` and `.nuspec` versions are still mismatched — fix before proceeding.

## 7. Build to confirm no regressions

```powershell
dotnet build
```

If the build fails because a newly-added rule fires on the repo's own code, the editorconfig severity for that rule needs adjusting before proceeding. Only suppress a rule in editorconfig deliberately — never inline.

## 8. Write the PRD

Check whether any open PRD file exists in `issues/` (exclude `issues/done/`). If none, write at `issues/prd.md`. If `issues/prd.md` already exists, write at `issues/prd-update-YYYY-MM-DD.md` (use today's date from the system).

Use this template (fill in the real package and rule data):

```markdown
## Problem Statement

The analyzer packages included in `Opinionated.DotNet.CodingStandards` have been updated to
newer versions. The updated packages expose new diagnostic rules that are not yet covered by
the test suite.

## Solution

Update the five analyzer package versions in `Directory.Packages.props` and `.nuspec`,
regenerate all analyzer editorconfigs, and add test coverage for each newly-discovered rule.

## Updated Packages

| Package | Old Version | New Version |
|---------|------------|------------|
| Meziantou.Analyzer | x.y.z | a.b.c |

## Newly Discovered Rules

| Rule ID | Editorconfig | Status |
|---------|-------------|--------|
| S1234 | SonarAnalyzer.CSharp.editorconfig | Added |

## User Stories

1. As a maintainer, I want the analyzer packages updated so that new rules are enforced on
   consuming projects.
2. As a maintainer, I want each new rule covered by a test so that the package's rule coverage
   is verified.
3. As a maintainer, I want the test suite to remain green after the update so that the package
   remains releasable.

## Implementation Decisions

- `Directory.Packages.props` and `.nuspec` versions are updated in lockstep (the
  `CheckNugetDependenciesMatchProps.cs` script enforces this).
- `scripts/UpdateAnalyzerEditorConfigs.cs` is re-run after each package bump to pick up
  added/stale rule IDs in the editorconfig files.
- Each new rule gets its own issue with guidance on how to write the test and which confounders
  to exhaust before marking a rule untestable.

## Testing Decisions

- Each new rule needs exactly one `[RuleDoc]` attribute — either a method-level one on a
  `[Fact]` test, or a class-level one in `UntestableRules.cs`.
- Before marking any rule untestable, exhaust the confounder playbook (see AGENTS.md and each
  per-rule issue).
- Run new tests in isolation: `dotnet test --no-build --filter "FullyQualifiedName~MyNewTest"`.
- Only run the full suite if shared helpers or package content changed.

## Out of Scope

- Bumping non-analyzer dependencies (e.g., `xunit`, `CliWrap`).
- Changing rule severities for existing rules — that is a separate, deliberate change.

## Further Notes

The editorconfig update script adds new rules at their default/suggested severity. Review the
`Added:` output to identify rules that might warrant a different severity.
```

## 9. Create one issue per new rule

For each rule ID collected in step 4, first check whether a `[RuleDoc]` already exists anywhere in the test assembly:

```powershell
Select-String -Path "tests\**\*.cs" -Pattern '"RULEID"' -Recurse
```

If a `[RuleDoc]` already exists for the rule, skip it — no new issue needed.

For each uncovered rule, determine the following before writing the issue:

### Target test file

| Rule prefix | Default target |
|-------------|---------------|
| `S` | `tests/.../SonarAnalyzerRules/` — pick a `SonarAnalyzerRules*Should.cs` file under 1000 lines |
| `CA` | `tests/.../CodeAnalysisRules/` — pick a `CodeAnalysisRules*Should.cs` file under 1000 lines |
| `MA` | A `MeziantouAnalyzers*Should.cs` file |
| `SA` | `StyleCopAnalyzersShould.cs` |
| `IDE` | `tests/.../CodingStandards/` — pick the appropriate split file |

If the target file is at or near 1000 lines, create a new split file following the `<OriginalClass><Group>Should` naming convention from AGENTS.md.

### HelpLink

| Rule prefix | HelpLink pattern |
|-------------|-----------------|
| `CA` | `https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/{ruleid-lowercase}` |
| `S` | `https://rules.sonarsource.com/csharp/{RULEID}/` |
| `MA` | `https://www.meziantou.net/analyzer/rules/{number}` (MA rule number without leading zeros) |
| `SA` | `https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/{RULEID}.md` |
| `IDE` | `https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/{ruleid-lowercase}` |

### Suggested method name

Convert the rule description to PascalCase. For example, "Do not use string.Empty" → `ProhibitStringEmpty`.

### Issue file

Number issues sequentially starting from the next available number (scan `issues/` for the highest `NNN-` prefix, then increment). Write each file at `issues/NNN-test-{RULEID-lowercase}.md`.

Use this template:

```markdown
## Parent PRD

`issues/prd.md`

## What to build

Add a test (or untestable declaration) for rule **{RULEID}** — *{rule description}* — from the
`{package name}` analyzer.

## Acceptance criteria

- [ ] Either a `[Fact]` with `[RuleDoc("{RULEID}", ...)]` exists in `{target test file}`, or a
      class-level `[RuleDoc("{RULEID}", ..., Untestable = "...")]` exists in `UntestableRules.cs`
- [ ] `RuleDocCoverageShould` passes (no duplicate or missing `[RuleDoc]` entries)
- [ ] If testable: `dotnet test --no-build --filter "FullyQualifiedName~{SuggestedMethodName}"` passes

## How to implement the test

Add a `[Fact]` to `{target test file}`:

```csharp
[Fact]
[RuleDoc("{RULEID}", "{rule description}",
    HelpLink = "{helplink}")]
public async Task {SuggestedMethodName}()
{
    using var project = await CreateProjectBuilderAsync();
    await project.AddFileAsync("Program.cs", """
        namespace test;
        // TODO: add code that triggers {RULEID}
        public static class Program { public static int Main() => 0; }
        """);
    var buildOutput = await project.BuildAndGetOutputAsync();
    buildOutput.HasError("{RULEID}").ShouldBeTrue();
}
```

## Confounder playbook (exhaust before marking untestable)

Work through these in order:

1. **Add a package reference** if the rule guards on types in an external package:
   - Logging rules (`CA1727`, `CA1848`, `CA2253`, `CA2254`, `CA2017`, `CA1873`, `CA2023`):
     add `("Microsoft.Extensions.Logging.Abstractions", "10.0.0")` to `packageReferences`.
   - ASP.NET Core MVC rules (`CA5391`, `CA5395`):
     add `("Microsoft.AspNetCore.Mvc", "2.3.10")` and set
     `properties: [("NuGetAudit", "false"), ("NoWarn", "NU1903;NU1902;CA1515;CA1822")]`.

2. **Use stub types** if the rule merely requires a type to exist in the compilation (no real
   package needed — just declare a matching type with the right name/namespace in `Program.cs`).

3. **Pin `LangVersion=12`** if C# 13 might route single-argument calls to a new `ReadOnlySpan`
   overload that the analyzer can't match (known to affect `CA1842`, `CA1843`).

4. **Set `ignore_internalsvisibleto=true`** on `CreateProjectBuilderAsync` if the rule is
   friend-assembly-sensitive (e.g., `CA1852`).

5. **Check `EnforceOnBuild` in Roslyn source** — rules tagged `EnforceOnBuild.Never` are
   structurally silent at `dotnet build` regardless of editorconfig severity. If confirmed,
   mark untestable and cite the source file + the `EnforceOnBuild.cs` / `EnforceOnBuildValues.cs`
   reference.

6. **Check the reporting analyzer's assembly path** — if the reporting class lives in a
   `src/Features/` path (not `src/Analyzers/`), it is IDE/LSP-only and cannot run at build time.
   Mark untestable and cite the path.

7. **Check mscorlib gate** — rules that assert `System.String` is in `mscorlib` cannot fire on
   `net10.0` (it lives in `System.Private.CoreLib`). Mark untestable with that explanation.

Only move the rule to `UntestableRules.cs` after exhausting all of the above.

## Blocked by

None — can start immediately.

## User stories addressed

- User story 2 (add test coverage for each newly-discovered rule)
```

After writing the last per-rule issue, append one final issue numbered `NNN+1` (the next sequence number after the last per-rule issue):

```markdown
## Parent PRD

`issues/prd.md`

## What to build

Regenerate `docs/rule-reference.md` now that all new rule tests from this bump have been
written (or declared untestable). The reference is generated from the editorconfig files and
the test assembly's `[RuleDoc]` attributes, so it must be regenerated **after** all per-rule
issues are complete to include the correct test links.

## Acceptance criteria

- [ ] `docs/rule-reference.md` has been regenerated and the new rule IDs appear in it
- [ ] The file is committed

## How to implement

Run the generation script:

```powershell
dotnet ./scripts/GenerateRuleReference.cs
```

Verify the new rule IDs appear in `docs/rule-reference.md`, then commit.

## Blocked by

All per-rule test issues for this PRD (issues NNN through NNN).

## User stories addressed

- User story 3 (test suite remains green and package remains releasable)
```

> **Important:** this issue must be the **last** issue listed in the PRD and must always be created, even if there is only one per-rule issue. It is the gate that confirms the docs are up to date before the PRD is closed.

## 10. Output a summary

After all files are written, print:

```
Updated packages:
  SonarAnalyzer.CSharp: 10.27.0.140913 → 10.28.0.XXXXXX

Created PRD: issues/prd.md
Created 13 issues:
  issues/001-test-s1234.md        (S1234 — Description of rule)
  issues/002-test-ca1234.md       (CA1234 — Description of rule)
  ...
  issues/013-update-rule-reference.md  (regenerate docs/rule-reference.md)

Next step: run `dotnet test` to verify the full suite is still green, then commit.
```

## Rules

- Always update `Directory.Packages.props` and `.nuspec` together — never drift.
- Always run `CheckNugetDependenciesMatchProps.cs` after editing versions.
- Always run the editorconfig update script and build before writing issues.
- Always create a final `NNN-update-rule-reference.md` issue as the last issue in the PRD.
- Never create an issue for a rule that already has a `[RuleDoc]`.
- Never mark a rule untestable without exhausting the confounder playbook.
- Do not commit — the user reviews and commits when ready.
