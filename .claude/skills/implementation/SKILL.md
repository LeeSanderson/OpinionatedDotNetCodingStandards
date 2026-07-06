---
name: implementation
description: Fully implement a PRD's issues end-to-end — a parallel read-only investigation pass, then strictly serial TDD implementation on the PRD's shared feature branch, HITL issues left for a human. Use after write-a-prd + prd-to-issues, when the user wants to "implement the PRD", run the whole issue queue, or run /implementation.
---

# Implementation

Drive an entire PRD's backlog to completion. It reads the issue DAG, investigates every runnable
AFK issue up front (in parallel — investigation never touches a file), then implements them **one
at a time, never concurrently**, directly in the primary checkout on the branch AGENTS.md's naming
rules assign to them. It stops cleanly at the HITL boundary with a precise hand-off.

It pairs with the `issue-implementer` agent (`.claude/agents/issue-implementer.md`), which does the
per-issue implementation work. The orchestration runs via the **Workflow** tool — invoking it from
this skill is sanctioned.

> **Before running:** confirm `issue-implementer` appears in your available agent list. Agent files
> are loaded at session start, so if it was just created, start a fresh session first — otherwise
> `agentType: 'issue-implementer'` won't resolve.

## Concurrency model — read this before touching the workflow

**Issues in this repo cannot be implemented in parallel. Ever.** Two independent facts force this,
and the workflow below is built specifically to make violating it hard:

1. **No git worktrees.** This repo's test harness resolves the repo root by walking up to a real
   `.git` **directory** (`tests/Opinionated.DotNet.CodingStandards.Tests/Helpers/PathHelpers.cs`).
   A git worktree's `.git` is a *file*, not a directory, so `dotnet test` cannot find the package
   sources or run `RuleDocCoverageShould` inside one. So there is no isolated per-issue checkout to
   hand to a parallel agent — every issue is implemented in the **one and only** primary checkout.
2. **One branch, one working tree, shared by every issue.** Every issue under a PRD commits to the
   *same* branch (`feat/<prd-slug>`, per AGENTS.md). Two agents editing, testing, and committing in
   that single working tree at the same time would silently clobber each other's file edits, race
   on the git index, or interleave commits — with no error, just corrupted or lost work. This is
   true even for issues with no declared dependency on each other, because the collision isn't
   logical (the DAG), it's physical (one directory).

There's a third, subtler hazard even for *read-only* work: running `dotnet build`/`test`/`pack`/
`restore` concurrently in this same directory can corrupt shared incremental build state (`bin/`,
`obj/`) even when no tracked source file is touched — two build processes can race on writing the
same MSBuild cache files. So "read-only" here means **no file writes AND no dotnet commands**, not
just "doesn't touch git."

Given that, the workflow has exactly one place where anything runs concurrently — the
**Investigate/Skeptic** pass — and it is deliberately constrained two ways at once:

- **Tool-enforced:** those calls use `agentType: 'Explore'`, which structurally has no `Edit` or
  `Write` tool. A prompt telling an agent "don't write files" can be ignored by a confused or
  over-eager model; a tool it doesn't have cannot be called by mistake. (Explore still has `Bash`,
  so it *could* run `dotnet build` — that part is prompt-enforced only, see the rules below.)
- **Verified, not trusted:** after Investigate/Skeptic finish and before a single line of Apply
  work happens, **you** (the orchestrator, with real `Bash` access — not a subagent's self-report)
  run `git status --porcelain` yourself and hard-abort the whole run if it isn't empty. This is the
  one non-negotiable gate: it catches a misbehaving read-only agent regardless of what it claims it
  did.

Everything after that gate — the **Apply** phase — is a plain, sequential `for` loop over issues:
implement → verify → repair → re-verify → commit, fully awaited, before the next issue starts. It
is not `parallel()`, not `pipeline()`, and must never become either — see the Rules section.

## Invocation

- `/implementation` — target every issue in `issues/` not already in `issues/done/`.
- `/implementation issues/prd.md` — target only issues whose **Parent PRD** matches that file.
- `--yes` — skip the plan confirmation (for unattended / `/loop` runs).

## 1. Build the plan (main loop — you have file access)

1. **Gather context.** Run `git log -n 5 --format="%H%n%ad%n%B---" --date=short` so you don't redo
   work that just landed.
2. **Collect issues.** List `issues/*.md`, excluding `issues/done/**`, `issues/prd.md`, and
   `issues/todo.md`. If a PRD path was given, keep only issues whose `Parent PRD` line matches it.
   `issues/done/` is the **completed set** — this is how re-runs resume.
3. **Parse each issue:**
   - `id` — the `NNN` prefix of the filename.
   - `title` — the first `#` heading.
   - `deps` — every `issues/NNN-….md` referenced under **Blocked by** ("None …" ⇒ no deps).
   - `type` — `HITL` if the file has a `**Type:** HITL` (or `Type: HITL`) marker, else `AFK`. Most
     existing issues have no marker; treat those as **AFK** and note the assumption in the plan.
   - `branch` — the branch this issue's commit belongs on, using AGENTS.md's naming rules:
     - `Parent PRD` present → `feat/<prd-slug>` (the PRD filename, `issues/` prefix and `.md`
       suffix stripped).
     - `Parent PRD` absent → `feat/<NNN>-<issue-slug>` (the issue's own filename, stripped the
       same way).
4. **Validate.** Abort with a clear message if:
   - the primary working tree is **dirty** (`git status --porcelain` is non-empty);
   - the current branch (`git branch --show-current`) is neither `main` nor one of the `branch`
     values computed above — this is AGENTS.md's "unexpected branch" case: warn and refuse rather
     than mixing unrelated work onto a branch a human may be using;
   - a `Blocked by` reference points to a file that doesn't exist and isn't in `done/`;
   - the dependency graph has a **cycle** (print the cycle).
5. **Resolve dependencies already satisfied.** A dep that is in `issues/done/` is satisfied — drop
   it from that issue's `deps`.
6. **Compute the frontier.**
   - Mark every `HITL` issue as **blocked-on-human**.
   - Propagate: any issue with a transitive dependency that is HITL or blocked-on-human is itself
     **blocked-on-human** (record which HITL ancestor blocks it).
   - The **runnable set** = the remaining AFK issues (not in `done/`, no HITL ancestor).
7. **Group into branch batches.** Group the runnable set by `branch`. Topologically sort issues
   within each batch. Order the batches themselves in the order their first runnable issue was
   discovered, except: if a batch contains an issue blocked by an issue in a later batch, move the
   blocking batch earlier (cross-PRD dependencies should be rare, but check for them). This
   grouping is bookkeeping only — see the concurrency model above, batches are executed one after
   another, not concurrently.
8. **Edge case — nothing to run.** If the runnable set is empty, skip straight to the report in
   step 4: list what's done and the exact HITL action(s) the human must take, then stop. Do not
   launch any Workflow.

## 2. Show the plan and confirm

Print, grouped by branch batch:

```
PLAN for <scope>  (<N> issues: <d> done, <r> to run, <s> blocked-on-human)
  BRANCH feat/prd-add-logging-rules  (3 issues)          <- run strictly one issue at a time
    017 test-ca1727              run
    018 test-ca1848      -> 017  run
    019 test-ca2253      -> 017  run
  BRANCH feat/031-fix-something  (1 issue)
    031 fix-something            run
  SKIP (needs human): 040 design-review (HITL); 041 -> 040
  ALREADY DONE: <ids in issues/done/>
  (issues with no Type marker assumed AFK: <ids>)
```

The grouping into branches and the `->` dependency arrows are for the human's readability only —
every issue listed here executes sequentially, never concurrently with any other (see the
concurrency model above).

Wait for the user's go-ahead. Skip this prompt only if `--yes` was passed.

## 3. Run the workflow — investigate, then a hard gate, then apply

This is **two separate Workflow calls with a manual verification step in between**, not one call.
The gate in the middle must be run by you directly (real `Bash`), not delegated to any agent —
that's what makes it trustworthy.

### 3a. Investigate + Skeptic (Workflow call #1 — the only concurrent step)

Call the **Workflow** tool with the script below and `args = { allIssues, repoRoot }`, where
`allIssues` is every runnable issue flattened from all batches (`{ id, file, title, branch }`) and
`repoRoot` is the absolute repo path.

```javascript
export const meta = {
  name: 'implement-prd-investigate',
  description: 'Read-only, parallel-safe investigation of every runnable issue, plus an adversarial second opinion on any untestable verdict',
  phases: [
    { title: 'Investigate' },
    { title: 'Skeptic' },
  ],
}

const { allIssues, repoRoot } = args

const PLAN = {
  type: 'object',
  required: ['issueId', 'approach', 'testable'],
  additionalProperties: false,
  properties: {
    issueId: { type: 'string' },
    approach: { type: 'string' },
    targetFiles: { type: 'array', items: { type: 'string' } },
    testable: { type: 'boolean' },
    blockedReason: { type: 'string' },
  },
}
const SKEPTIC = {
  type: 'object',
  required: ['overturned'],
  additionalProperties: false,
  properties: {
    overturned: { type: 'boolean' },
    revisedApproach: { type: 'string' },
    notes: { type: 'string' },
  },
}

const investigatePrompt = i =>
  `Read-only investigation of issue ${i.id} (${i.title}): ${i.file}, in the checkout at ${repoRoot}.\n` +
  `You are one of several investigations running concurrently against this SAME checkout — every ` +
  `other issue's investigation is reading it at the same time. Do NOT create, edit, or delete any ` +
  `file, and do NOT run 'dotnet build'/'test'/'pack'/'restore' (concurrent builds in one directory ` +
  `can corrupt shared incremental state even without touching a tracked file). Only read.\n` +
  `Read the issue, the AGENTS.md sections it points to (e.g. the confounder playbook for rule-test ` +
  `issues), and the source files it's likely to touch. Return a short implementation plan: the ` +
  `concrete approach, the files you expect to touch, and whether the acceptance criteria are ` +
  `achievable as written (testable=true) or not (testable=false, with blockedReason citing the ` +
  `precise structural reason — e.g. an EnforceOnBuild.Never tag, an mscorlib gate, a Features-` +
  `assembly gate — never just "couldn't find a way").`

const skepticPrompt = p =>
  `Read-only. Same rules as investigation: no file writes, no dotnet commands — you are running ` +
  `concurrently with other read-only checks against the same checkout.\n` +
  `Adversarially challenge this investigation's untestable verdict for issue ${p.issueId}. ` +
  `Reported blocker: ${p.blockedReason}\n` +
  `Before agreeing, exhaust AGENTS.md's confounder playbook yourself: a real PackageReference for ` +
  `external-type gates, stub types for type-existence gates, LangVersion=12 for span-overload ` +
  `routing, ignore_internalsvisibleto for friend-assembly rules. Return overturned=true with a ` +
  `revisedApproach if you find a way to make it testable; else overturned=false.`

// The ONLY concurrent phase in this whole skill. Safe only because:
//   (a) agentType 'Explore' has no Edit/Write tool at all (tool-enforced), and
//   (b) the orchestrator verifies the tree is still clean before Apply ever starts (see step 3b).
const plans = (await parallel(allIssues.map(i => () =>
  agent(investigatePrompt(i), {
    label: `investigate:${i.id}`, phase: 'Investigate', agentType: 'Explore', schema: PLAN,
  })
))).map((p, idx) => p || { issueId: allIssues[idx].id, approach: '', testable: true })

const planById = Object.fromEntries(plans.map(p => [p.issueId, p]))
const blocked = plans.filter(p => p.testable === false)
if (blocked.length) {
  const verdicts = await parallel(blocked.map(p => () =>
    agent(skepticPrompt(p), {
      label: `skeptic:${p.issueId}`, phase: 'Skeptic', agentType: 'Explore', schema: SKEPTIC,
    })
  ))
  verdicts.forEach((v, idx) => {
    if (v && v.overturned) {
      const p = blocked[idx]
      planById[p.issueId] = { ...p, testable: true, approach: v.revisedApproach || p.approach }
      log(`Skeptic overturned untestable verdict for ${p.issueId}`)
    }
  })
}

return Object.values(planById)
```

Keep the returned array (`plans`) — it feeds both the gate below and the Apply call.

### 3b. Hard gate — verify the tree yourself before Apply touches anything

Run this with your own `Bash` tool. Do not skip it, do not ask an agent to do it for you, and do
not fold it into the Workflow script (the script has no real filesystem access — only a check *you*
run is trustworthy):

```powershell
git -C <repoRoot> status --porcelain
```

- **Empty output** → proceed to 3c.
- **Any output** → STOP. A read-only investigation agent wrote to the working tree, violating its
  contract. Do not run Apply against a tree in an unknown state. Report exactly which paths
  changed, and let the user decide whether to inspect/discard them (`git status`/`git diff`) before
  a re-run. This is the scenario the whole gate exists to catch — treat it as a real incident, not
  a warning to shrug off.

### 3c. Apply (Workflow call #2 — strictly serial, never touch this)

Call the **Workflow** tool with the script below and `args = { batches, plans, repoRoot }`, where
`batches` are the branch batches from step 1.7 (each `{ branch, issues: [{ id, file, title, deps }] }`)
and `plans` is the array returned from 3a.

```javascript
export const meta = {
  name: 'implement-prd-apply',
  description: 'Implement every runnable issue one at a time, in the primary checkout, on its PRD branch',
  phases: [
    { title: 'Apply' },
    { title: 'Verify' },
  ],
}

const { batches, plans, repoRoot } = args

const RESULT = {
  type: 'object',
  required: ['status', 'issueId', 'summary'],
  additionalProperties: false,
  properties: {
    status: { type: 'string', enum: ['success', 'failed'] },
    issueId: { type: 'string' },
    branch: { type: 'string' },
    summary: { type: 'string' },
    filesChanged: { type: 'array', items: { type: 'string' } },
    failureReason: { type: 'string' },
  },
}
const VERDICT = {
  type: 'object',
  required: ['allMet'],
  additionalProperties: false,
  properties: {
    allMet: { type: 'boolean' },
    unmet: { type: 'array', items: { type: 'string' } },
    notes: { type: 'string' },
  },
}

const planById = Object.fromEntries((plans || []).map(p => [p.issueId, p]))

const applyPrompt = (i, plan) =>
  `MODE = implement\nISSUE_FILE = ${i.file}\nISSUE_ID = ${i.id}\nREPO_ROOT = ${repoRoot}\n` +
  `EXPECTED_BRANCH = ${i.branch}\n\n` +
  `Investigation plan (a head start, not gospel — verify it against the real code): ` +
  `${JSON.stringify(plan)}\n\n` +
  `Implement issue ${i.id} (${i.title}) end-to-end per your agent procedure: get onto the right ` +
  `branch, implement the change (TDD for code, direct edits for docs/pipeline), run the AGENTS.md ` +
  `gate for what you touched, move the issue into issues/done/, commit on the branch. Do NOT push, ` +
  `do NOT open a PR. No other agent is touching this checkout right now or will until you return.`

const repairPrompt = (i, v) =>
  `MODE = repair\nISSUE_FILE = ${i.file}\nISSUE_ID = ${i.id}\nREPO_ROOT = ${repoRoot}\n` +
  `EXPECTED_BRANCH = ${i.branch}\nREPAIR_NOTES:\n${(v.unmet || []).map(u => '- ' + u).join('\n')}\n` +
  `${v.notes ? 'Verifier notes: ' + v.notes : ''}\n\n` +
  `Your commit(s) for issue ${i.id} are already on branch ${i.branch} (find them via git log — the ` +
  `one moving ${i.file} into issues/done/). Close the unmet criteria above with additional commits ` +
  `on the same branch, re-run the gate, and return the structured result.`

const verifyPrompt = (i, impl) =>
  `Read-only acceptance check — do NOT modify any file. Issue ${i.id} was implemented on branch ` +
  `${i.branch} in ${repoRoot}. You are running strictly after the implementer finished and before ` +
  `the next issue starts, so the tree reflects exactly this issue's work.\n` +
  `Issue file (its 'Acceptance criteria' checklist is the contract): ${i.file}\n` +
  `Implementer summary: ${impl.summary}\n\n` +
  `Find this issue's commit(s) via 'git -C ${repoRoot} log --oneline -10' (the one moving ${i.file} ` +
  `to issues/done/) and inspect its diff. Go through EVERY acceptance criterion and adversarially ` +
  `hunt for one not genuinely met. Return allMet=false with the specific failing criteria in ` +
  `'unmet' if any fails; else allMet=true.`

// STRICTLY SEQUENTIAL. Do not wrap this in parallel() or pipeline(), and do not restructure it so
// that one issue's implement/verify/repair overlaps another's — there is exactly one working tree,
// checked out to exactly one branch, for the whole repo. Two issues "in flight" at once means two
// agents editing/committing the same files at the same time, corrupting or losing work.
const outcome = {}
for (const batch of batches) {
  const failedInBatch = new Set()
  for (const issue of batch.issues) {
    const blockedByFailure = (issue.deps || []).some(d => failedInBatch.has(d) || outcome[d]?.status === 'failed')
    if (blockedByFailure) {
      outcome[issue.id] = { status: 'skipped', reason: 'a dependency failed' }
      log(`SKIP ${issue.id}: a dependency failed`)
      continue
    }

    const plan = planById[issue.id] || { issueId: issue.id, approach: '', testable: true }
    const full = { ...issue, branch: batch.branch }

    const impl = await agent(applyPrompt(full, plan), {
      label: `apply:${issue.id}`, phase: 'Apply', agentType: 'issue-implementer', schema: RESULT,
    })
    if (!impl || impl.status !== 'success') {
      outcome[issue.id] = { status: 'failed', reason: impl?.failureReason || 'implementer did not finish' }
      failedInBatch.add(issue.id)
      log(`FAIL ${issue.id}: ${outcome[issue.id].reason}`)
      continue
    }

    let verdict = await agent(verifyPrompt(full, impl), {
      label: `verify:${issue.id}`, phase: 'Verify', agentType: 'Explore', schema: VERDICT,
    })
    if (verdict && verdict.allMet === false) {
      log(`REPAIR ${issue.id}: ${(verdict.unmet || []).length} unmet criteria`)
      const repaired = await agent(repairPrompt(full, verdict), {
        label: `repair:${issue.id}`, phase: 'Apply', agentType: 'issue-implementer', schema: RESULT,
      })
      if (!repaired || repaired.status !== 'success') {
        outcome[issue.id] = { status: 'failed', reason: 'repair pass did not finish' }
        failedInBatch.add(issue.id)
        log(`FAIL ${issue.id}: repair pass did not finish`)
        continue
      }
      verdict = await agent(verifyPrompt(full, repaired), {
        label: `reverify:${issue.id}`, phase: 'Verify', agentType: 'Explore', schema: VERDICT,
      })
    }
    if (!verdict || verdict.allMet !== true) {
      outcome[issue.id] = { status: 'failed', reason: 'acceptance criteria unmet after repair: ' + ((verdict?.unmet || []).join('; ') || 'unknown') }
      failedInBatch.add(issue.id)
      log(`FAIL ${issue.id}: ${outcome[issue.id].reason}`)
      continue
    }

    outcome[issue.id] = { status: 'done', branch: batch.branch }
    log(`DONE ${issue.id} committed on ${batch.branch}`)
  }
}

return outcome
```

### How this behaves (so you can read the result)

- **3a** runs every runnable issue's read-only research concurrently. Nothing is written; the
  `agentType: 'Explore'` restriction makes that a tool-level guarantee, not just a prompt request.
- **3b** is the single point where you, not a model, confirm the tree really is untouched. This is
  what makes the whole design trustworthy instead of merely well-intentioned.
- **3c** is one plain sequential loop: implement → verify (read-only adversarial AC check) → one
  repair pass if needed → re-verify → next issue. At no point are two issues "in flight" at once.
- A failed issue is isolated: not committed, not marked done, and its dependents (tracked via
  `deps`) resolve to `skipped`. Independent issues — including those in a later batch — still run,
  just never at the same time as anything else.
- The returned `outcome` map tells you each issue's fate: `done`, `failed` (with `reason`), or
  `skipped` (dependency failed).

## 4. Finalise

1. **Final sweep per touched branch.** For each batch with at least one `done` issue, on that
   branch run the complete gate as the last safety net:
   ```powershell
   git checkout <branch>
   dotnet build
   dotnet test
   ```
   Report the result per branch; if it's red, say so loudly — something committed green
   individually but interacts badly with a sibling issue on the same branch.
2. **Report.** Print, leading with any human action:
   ```
   === /implementation report ===
   NEXT HUMAN ACTION: <HITL issue(s) to do> | <"push <branch> and open a PR for review"> | "none"
   DONE (committed, not pushed):
     feat/prd-add-logging-rules: 017, 018, 019
   FAILED / SKIPPED: <id — reason> ...
   BLOCKED ON HUMAN: <HITL ids + their blocked dependents>
   final sweep: feat/prd-add-logging-rules: GREEN | RED (<detail>)
   ```
3. **Never push or open a PR.** Per AGENTS.md's git boundaries, commit/push only when the user
   explicitly asks. Suggest the `gh pr create` command as the human's next step; do not run it.
4. **Suggest clean-up.** If — and only if — every targeted issue is now in `issues/done/` with
   nothing failed/skipped/blocked, suggest running `/clean-up-prd`. Never run it automatically; it
   deletes the PRD and issues and needs explicit approval.
5. **Resuming.** After the user handles HITL work or opens/merges a PR, re-running `/implementation`
   picks up automatically: `issues/done/` is the state, and the branch-vs-`main` check in step 1.4
   accepts either starting point.

## Rules

- Never implement HITL issues, and never implement an issue with a HITL ancestor. The run ends at
  that boundary with a hand-off.
- Never use `git worktree` — see the concurrency model above.
- **Never run two issues at once, and never let anyone "optimize" this into parallelism.** There is
  one working tree and one branch checked out at a time for the whole repo. Concretely:
  - Do not rewrite the Apply loop (3c) as `parallel()` or `pipeline()`, even for issues in different
    batches/branches or with no declared dependency on each other — the collision is physical
    (one directory), not logical (the DAG).
  - Do not skip or relocate the `git status --porcelain` gate in 3b, and do not let a subagent
    perform it on your behalf — only a check run by the actual orchestrator's `Bash` tool counts.
  - Keep `agentType: 'Explore'` on every Investigate/Skeptic/Verify call; don't swap in an agent
    type that has `Edit`/`Write`.
  - Investigate/Skeptic/Verify prompts must keep forbidding `dotnet build`/`test`/`pack`/`restore` —
    concurrent builds can corrupt shared `bin`/`obj` state even without touching a tracked file.
- The orchestrator (this skill) never edits source itself — it parses, plans, runs the two
  Workflows with a manual gate in between, runs the final sweep, and reports. All code changes
  happen via `issue-implementer`, one issue at a time, in the primary checkout.
- Never push or open a PR — that's the user's call.
- Exactly two Workflow runs per invocation (investigate, then apply) — never combine them into one,
  and never add a third.
