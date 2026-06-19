---
name: work-on-next-issue
description: Pick the next AFK issue from issues/ and implement it end-to-end via TDD, then commit. Use when the user wants to advance the ralph queue, asks to "work on the next issue", or runs this in a /loop.
---

# Work On The Next Issue

Work on a single AFK issue: pick it, implement it via TDD, run the feedback loops, commit, and close it. 

## 1. Gather context

Run this to see what shipped recently — it prevents you re-doing work that just landed:

```
git log -n 5 --format="%H%n%ad%n%B---" --date=short
```

Then list `issues/` (skip the `issues/done/` subdirectory) and read the issue files.

You work on **AFK** issues only — never HITL.

If every AFK issue is already in `issues/done/`, output exactly:

```
<promise>NO MORE TASKS</promise>
```

and stop. (The sentinel is what the surrounding `/loop` looks for to know the queue is drained.)

## 2. Task selection

Pick **one** issue. Prioritize, highest first:

1. Critical bugfixes
2. Development infrastructure (tests, types, dev scripts) — precursors to feature work
3. Tracer bullets for new features — a tiny, end-to-end slice through every layer, then expand
4. Polish and quick wins
5. Refactors

## 3. Explore

Read the files the chosen issue touches before changing anything.

## 4. Implementation

Classify the issue before choosing an approach:

- **Documentation or pipeline** (README updates, CI/CD config, GitHub Actions, NuGet packaging, changelog): implement directly — no TDD loop needed.
- **Code changes** (analyzers, source generators, library logic, tests): use the `tdd` skill to drive the change red-green-refactor.

## 5. Feedback loops

Skip this step entirely for pure documentation or pipeline issues.

For code changes:

1. **Build first:**
   ```powershell
   dotnet build
   ```

2. **Run only the new/changed test(s) with a filter** (fast — seconds, not minutes):
   ```powershell
   dotnet test --no-build --filter "FullyQualifiedName~MyNewTestMethod"
   ```
   Fix failures before proceeding.

3. **Decide whether the full suite is needed.** Consult AGENTS.md §Test speed.
   Skip the full suite if ALL hold:
   - Only new `[Fact]` methods added to existing test classes (and/or a `UntestableRules.cs` removal)
   - No changes to shared helpers, package content, or editorconfigs
   
   Otherwise run the full suite:
   ```powershell
   dotnet test --no-build
   ```

If the solution has a `dotnet format` (or analyzer/style) gate, run it too and fix any violations before committing.

## 6. Branch check

Before committing, verify you are on the correct branch for this issue.

**Derive the expected branch name:**
1. Read the chosen issue file's `## Parent PRD` field.
   - If present: derive branch as `feat/<prd-slug>`, where `<prd-slug>` is the PRD filename with the `issues/` prefix and `.md` suffix stripped.  
     Example: `Parent PRD: issues/prd-renovate-branch-protection.md` → `feat/prd-renovate-branch-protection`
   - If absent: derive branch as `feat/NNN-issue-slug`, where `NNN-issue-slug` is the issue filename with the `issues/` prefix and `.md` suffix stripped.  
     Example: `issues/007-fix-something.md` → `feat/007-fix-something`

**Three-way logic:**
- **Currently on `main`** → create and checkout the expected branch (`git checkout -b <expected>`), then proceed.
- **Already on the expected branch** → proceed.
- **On a different feature branch** → warn the user, refuse to commit by default; ask them to switch to `<expected>` manually before re-running.

## 7. Close the issue

- If complete: move the issue file to `issues/done/`.
- If incomplete: append a note to the issue file describing what was done and what's left.


## 8. Commit

Make one git commit. The message must include:

1. Key decisions made
2. Files changed
3. Blockers or notes for the next iteration

## Rules

- ONLY WORK ON A SINGLE TASK per invocation.
- Never touch HITL issues.
