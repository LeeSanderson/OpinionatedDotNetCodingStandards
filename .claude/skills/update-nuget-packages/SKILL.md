---
name: update-nuget-packages
description: Check NuGet analyzer packages for newer versions and, if any are found, autonomously drive the entire release end-to-end — PRD and issues, /implementation, cleanup, changelog, PR with auto-merge, tag, and a published NuGet release. Use when the user wants to bump analyzer dependencies or asks "are there any NuGet updates?".
---

# Update NuGet Packages

Check the five analyzer NuGet packages for new versions and, if any are found, run the whole
release lifecycle autonomously: apply the version bump, regenerate editorconfigs, write a PRD and
per-rule issues, implement them via `/implementation`, clean up, update the changelog, open a PR,
merge it once it's green, and cut + publish the new patch release.

## What "autonomous" means here — read before invoking

Once you decide there's an update to apply, this skill does not stop to ask before each subsequent
step — it commits, pushes, opens a PR, merges to `main`, tags, and triggers a real publish to
NuGet.org, all in one run, because that end-to-end automation is exactly what was asked for. The
checkpoints that remain are the ones spelled out below, not a general "pause and confirm":

- Step 10's commit is **local only** — nothing is pushed until step 12.5, so up to that point a
  human still has a natural window to `git reset`/inspect before anything becomes visible upstream.
- Step 11 only continues into step 12 on an unambiguous, fully-successful `/implementation` run
  (every targeted issue `done`, none failed/skipped/blocked). Anything less and the run stops and
  reports — it does not guess or partially proceed.
- Steps 12.6, 12.8, and 12.9 each have a **stop-and-notify-loudly** condition (PR won't merge,
  `New-ReleaseTag.ps1` fails, the release workflow fails). None of those are ever silently retried,
  forced through, or worked around — see the Rules section.

## 0. Detect context

Before doing anything else, capture two facts that control later steps:

**Current branch:**

```powershell
git rev-parse --abbrev-ref HEAD
```

Set `ON_FEATURE_BRANCH = true` when the result is anything other than `main` or `master`.

**Active PRD:**

List `issues/` (excluding `issues/done/`) and look for any `.md` file whose name begins with `prd` (e.g. `issues/prd.md`, `issues/prd-add-logging-rules.md`).

Set `ACTIVE_PRD_EXISTS = true` and record the path as `ACTIVE_PRD_PATH` when such a file is found.

**Combined flag — extend mode:**

`EXTEND_MODE = ON_FEATURE_BRANCH AND ACTIVE_PRD_EXISTS`

When `EXTEND_MODE` is true, steps 3 and 8 behave differently (detailed below). Steps 1–2, 4–7, 9–12 are unchanged.

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

> **This step is skipped entirely when no updates were found in step 2.**
> **This step is also skipped when `EXTEND_MODE` is true** — the work stays on the current feature branch, which already has a home for the new issues (`ACTIVE_PRD_PATH` from step 0).

When `EXTEND_MODE` is **false** (i.e. we are on `main` or there is no active PRD), create a dedicated branch. The branch name must follow AGENTS.md's PRD-linked naming rule (`feat/<prd-slug>`) — not an ad hoc name — because `/implementation` derives the exact same branch name from the PRD path when it later implements these issues; a mismatch there would make it refuse to proceed.

1. **Decide the PRD path now** (step 8 will only need to write to it, not re-decide it): check whether any open PRD file exists in `issues/` (exclude `issues/done/`). If none, `ACTIVE_PRD_PATH = issues/prd.md`. If `issues/prd.md` already exists (a stray file with no active branch — unusual, but possible), `ACTIVE_PRD_PATH = issues/prd-update-YYYY-MM-DD.md` (today's date, from the `currentDate` system context or `Get-Date -Format "yyyy-MM-dd"`).
2. Derive `<prd-slug>` from `ACTIVE_PRD_PATH` by stripping the `issues/` prefix and `.md` suffix, then set the branch name to `feat/<prd-slug>`.
   Example: `issues/prd.md` → `feat/prd`; `issues/prd-update-2026-07-06.md` → `feat/prd-update-2026-07-06`.
3. Check whether the branch already exists:
   ```powershell
   git show-ref --verify --quiet refs/heads/feat/<prd-slug>
   ```
4. If the branch **does not exist** → create it and switch:
   ```powershell
   git checkout -b feat/<prd-slug>
   ```
5. If the branch **already exists** (e.g. a prior run created it but the PRD wasn't committed) → switch to it without creating a duplicate:
   ```powershell
   git checkout feat/<prd-slug>
   ```

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

## 8. Write or extend the PRD

### When `EXTEND_MODE` is true — extend the existing PRD

Read `ACTIVE_PRD_PATH`. Locate the two Markdown tables inside it:

- **`## Updated Packages`** table — append one new row per package that was bumped in this run.
- **`## Newly Discovered Rules`** table — append one new row per rule added in this run.

Do **not** alter any other section of the PRD. After editing, `ACTIVE_PRD_PATH` is still the PRD for step 9.

If either table is missing from the active PRD (it may have been created by a different process), add the missing table immediately after the section heading.

Skip the rest of this step (the "create new PRD" path below).

### When `EXTEND_MODE` is false — create a new PRD

Write to `ACTIVE_PRD_PATH` — already decided in step 3, alongside the branch name derived from it.

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

Use this template (substituting the real `ACTIVE_PRD_PATH` for the `Parent PRD` field):

```markdown
## Parent PRD

`{ACTIVE_PRD_PATH}`

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

`{ACTIVE_PRD_PATH}`

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

## 10. Commit the setup

`/implementation` refuses to start against a dirty working tree (by design — it's how it
guarantees a clean baseline for its own diffs), and right now the version bump, editorconfigs,
PRD, and issue files are all uncommitted. Check `git status --porcelain` and confirm it shows
exactly `Directory.Packages.props`, the `.nuspec`, the regenerated editorconfigs under
`pkgsrc/config/analyzers/`, `ACTIVE_PRD_PATH`, and the new `issues/NNN-*.md` files — nothing else —
then commit them as one commit on the feature branch:

```powershell
git add Directory.Packages.props packages/Opinionated.DotNet.CodingStandards/Opinionated.DotNet.CodingStandards.nuspec
git add packages/Opinionated.DotNet.CodingStandards/pkgsrc/config/analyzers
git add "<ACTIVE_PRD_PATH>" issues/*.md
git commit -m "Bump analyzer packages and add rule-coverage backlog for the new rules"
```

This is a **local** commit only — nothing is pushed until step 12.5. Print a summary before moving
on:

```
Updated packages:
  SonarAnalyzer.CSharp: 10.27.0.140913 → 10.28.0.XXXXXX

Extended PRD: issues/prd.md          ← use "Extended" when EXTEND_MODE was true
  (or)
Created PRD: issues/prd.md           ← use "Created" when EXTEND_MODE was false

Created 13 issues:
  issues/001-test-s1234.md        (S1234 — Description of rule)
  issues/002-test-ca1234.md       (CA1234 — Description of rule)
  ...
  issues/013-update-rule-reference.md  (regenerate docs/rule-reference.md)

Committed to feat/prd (not yet pushed). Handing off to /implementation...
```

## 11. Hand off to /implementation

> **This step only runs when step 2 found at least one update** — if step 2 stopped early because
> everything was already current, there is no PRD to implement and this step doesn't apply.

Continue straight into implementing the PRD — no confirmation needed here. Hand off completely
rather than reimplementing any of its logic:

```
Skill(implementation, args: "<ACTIVE_PRD_PATH>")
```

`/implementation` re-derives the same `feat/<prd-slug>` branch from `ACTIVE_PRD_PATH` (that's why
step 3 named it that way instead of `feat/bump-analyzers-*`), runs its own read-only investigation
pass and its own plan confirmation, then implements the issues one at a time — see
`.claude/skills/implementation/SKILL.md` for exactly what that does.

Read its final report. **Every targeted issue must be `done`** — none `failed`, `skipped`, or
`blocked-on-human`. (Rule-coverage issues from step 9 never carry a `Type: HITL` marker, so
`blocked-on-human` shouldn't occur here; if it somehow does, treat it exactly like a failure —
this skill's later steps assume an unqualified, complete success.)

- **Full success** → continue to step 12.
- **Anything else** → **stop here.** Leave the branch and PRD exactly as `/implementation` left
  them. Report the failed/skipped issue(s) or the HITL boundary clearly, and note that the human
  can resolve it and re-run `/implementation <ACTIVE_PRD_PATH>` (or this skill) later. Do not clean
  up, do not touch the changelog, do not open a PR on a partially-implemented PRD.

## 12. On full success: clean up, changelog, and release

> Only reached when step 11 reports every targeted issue `done`.

1. **Clean up the PRD and issues.**
   ```
   Skill(clean-up-prd)
   ```
   When invoking, note explicitly that this is an autonomous continuation of `/update-nuget-packages`
   and that `/implementation`'s own report in step 11 already established every issue is complete —
   clean-up-prd's normal "wait for my approval before deleting" step refers to a live human decision,
   and that decision was already made when the user asked for this end-to-end pipeline; proceed with
   the deletion without pausing for a further prompt. It deletes `ACTIVE_PRD_PATH` and every issue
   that referenced it, and commits that deletion itself.

2. **Determine the release version.** List existing tags and find the highest `vMAJOR.MINOR.PATCH`:
   ```powershell
   git tag -l 'v*'
   ```
   Use the exact same selection logic as `scripts/New-ReleaseTag.ps1` (parse `vX.Y.Z`, ignore
   anything that doesn't match, sort numerically, take the highest — default to `v0.0.0` if there
   are none). The release this run will trigger is a **patch** bump of that: `vX.Y.(Z+1)`. Compute
   it here the same way the script will, so the changelog heading matches exactly what
   `New-ReleaseTag.ps1 -Patch` looks for in step 12.8.

3. **Update `CHANGELOG.md`.** Insert a new `## [vX.Y.(Z+1)]` section immediately below
   `## [Unreleased]` (leave `## [Unreleased]` itself empty, matching the existing file), covering
   everything this run changed:
   - `### Added` — one line per newly-enforced rule ID from step 5/9's `Added:` output, in the same
     style as existing entries (severity, one-line rule description, rule ID(s) backticked).
   - `### Changed` — one line per package bump: `Bumped <Package> from <old> to <new>.` If, while
     working through the PRD, you noticed a behavior change to an *existing* rule (not a new rule
     ID) called out in the upstream package's own release notes, add it here too — see the
     `[v0.0.3]` entry for the style.
   - `### Removed` — one line per rule ID that appeared in step 5's `Stale:` output (a rule dropped
     from the newer analyzer version's default set). Omit this heading entirely if nothing went
     stale.
4. **Commit the changelog:**
   ```powershell
   git add CHANGELOG.md
   git commit -m "Update changelog for v<version>"
   ```
5. **Push the branch and open the PR.** If a PR already exists for this branch (possible when
   resuming an `EXTEND_MODE` run), reuse it (`gh pr list --head feat/<prd-slug> --json number`)
   instead of creating a duplicate.
   ```powershell
   git push -u origin feat/<prd-slug>
   gh pr create --base main --title "<summarize the package bumps>" --body "<reuse the PRD's own summary — don't write a second one from scratch>"
   ```
6. **Enable auto-merge and confirm mergeability.**
   ```powershell
   gh pr merge <number> --merge --auto --delete-branch
   ```
   `--merge` matches this repo's existing convention (every past PR here landed as a real merge
   commit, never a squash) — don't switch strategies. The repo already has "delete branch on merge"
   enabled server-side; `--delete-branch` is belt-and-braces.

   Wait for the PR's checks:
   ```powershell
   gh pr checks <number> --watch
   ```
   Then confirm it actually merged:
   ```powershell
   gh pr view <number> --json state,mergedAt,mergeStateStatus,statusCheckRollup
   ```
   - **`state` is `MERGED`** → continue to step 7.
   - **`state` is still `OPEN` immediately after checks complete** → GitHub's auto-merge can take a
     few seconds to actually execute after the last check reports; re-check once after a short
     pause before treating it as blocked.
   - **`mergeStateStatus` is `BEHIND`** → `main` requires PR branches to be up to date
     (`strict: true` in branch protection) and something else landed on `main` while this PR was
     open. Update the branch (`gh pr update-branch <number>`) and re-watch checks once; if it's
     still not merging after that, treat it as blocked.
   - **Any check failed, or it's still `OPEN` after the above** (branch protection blocking it, a
     required check that never reports, a real conflict, anything) → **stop and notify the user
     loudly**, quoting the exact blocking reason from the command output. Do not force-merge, do
     not edit branch protection, do not retry indefinitely. Leave the PR open for a human to
     resolve.
7. **Land locally.**
   ```powershell
   git checkout main
   git pull
   git fetch --prune
   git branch -D feat/<prd-slug>
   ```
   Force-delete (`-D` not `-d`): the PR's confirmed `MERGED` state from step 6 is the real source
   of truth, not git's local "already merged" heuristic, which can be wrong across merge strategies.
8. **Tag and release.** From `main`:
   ```powershell
   pwsh ./scripts/New-ReleaseTag.ps1 -Patch
   ```
   This re-derives the same version computed in step 2 from the now-updated tag list, re-verifies
   the changelog heading matches, runs the full test suite, then tags and pushes — which triggers
   `.github/workflows/release.yml`. If this script fails for **any** reason (dependency sync check,
   stale rule reference, missing changelog heading, failing tests) → **stop and notify the user
   loudly** with its exact error. Do not pass `-Force` to bypass a check it raised on its own.
9. **Confirm the release workflow succeeds.**
   ```powershell
   gh run list --workflow=release.yml --limit 1 --json databaseId,headBranch,status
   gh run watch <databaseId> --exit-status
   ```
   If the run fails → **stop and notify the user loudly**, including the failing step/logs
   (`gh run view <databaseId> --log-failed`). Be explicit that the tag and GitHub push already
   happened even though publishing failed — NuGet.org may not have the new version yet, and that
   needs a human to investigate and possibly re-run the workflow or push a new patch tag.

If every step above completes cleanly, report the final state: the new version, the merged PR, and
confirmation that the package is live on NuGet.org.

## Rules

- Always update `Directory.Packages.props` and `.nuspec` together — never drift.
- Always run `CheckNugetDependenciesMatchProps.cs` after editing versions.
- Always run the editorconfig update script and build before writing issues.
- Always create a final `NNN-update-rule-reference.md` issue as the last issue in the PRD.
- Never create an issue for a rule that already has a `[RuleDoc]`.
- Never mark a rule untestable without exhausting the confounder playbook.
- When on a feature branch with an active PRD, extend that PRD and stay on the branch — never
  create a new branch in this situation.
- The feature branch created in step 3 is always named `feat/<prd-slug>` (AGENTS.md's PRD-linked
  rule), derived from `ACTIVE_PRD_PATH` — never an ad hoc name like `feat/bump-analyzers-*`. This
  is what lets step 11 hand off to `/implementation` without a branch mismatch.
- All new issues must reference the correct `ACTIVE_PRD_PATH` in their `## Parent PRD` field.
- **This skill's autonomy is deliberate, not a shortcut.** Once step 2 finds an update, steps
  10–12 run through to a published release without stopping to ask — that was explicitly
  requested so this skill could run unattended. It is not license to skip the specific gates that
  *do* exist:
  - Step 11 only proceeds into step 12 on an unambiguous, fully-successful `/implementation` run.
    Never treat a partial success (any `failed`/`skipped`/`blocked-on-human` issue) as good enough.
  - Step 12.1's clean-up-prd hand-off skips its interactive approval prompt *only* because
    `/implementation`'s own report already established completion — state that reasoning
    explicitly when invoking it; don't silently assume every future clean-up-prd call may skip it.
  - Steps 12.6, 12.8, and 12.9 each have a **stop-and-notify-loudly** condition. Never force-merge,
    never edit branch protection, never pass `-Force` to `New-ReleaseTag.ps1` to push past a check
    it raised, and never silently retry a failed release workflow — all of those turn a visible,
    fixable problem into a confusing one. Report the exact command output and stop.
- `gh pr merge` always uses `--merge` (a real merge commit), matching every prior PR in this repo's
  history — never squash or rebase merge here.
- `git branch -D` (force) is correct in step 12.7 specifically because step 12.6 already confirmed
  `state: MERGED` via the GitHub API — that's the authority, not git's local merge-tracking.
