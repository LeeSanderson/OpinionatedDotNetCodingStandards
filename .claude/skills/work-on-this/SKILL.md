---
name: work-on-this
description: Implement a specific piece of work described in the skill argument, end-to-end via TDD, then commit. Use when the user wants to work on a described task rather than picking from the issue queue.
args: A description of the work to be done (required).
---

# Work On This

Implement a single, explicitly described task: understand it, implement it via TDD, run the feedback loops, and commit.

## 0. Validate input

The skill argument `{{args}}` is the description of the work to do.

If `{{args}}` is empty, blank, or not provided, stop immediately and output:

```
Error: /work-on-this requires a description of the work to do.
Usage: /work-on-this <description of work>
Example: /work-on-this Add support for CA1234 in the editorconfig
```

If `{{args}}` is provided but is too vague to act on (e.g. "fix things", "do stuff", single words with no context), stop immediately and output:

```
Error: The description "{{args}}" is too vague to act on.
Please provide a clearer description of what needs to be done.
Example: /work-on-this Add support for CA1234 in the editorconfig
```

Otherwise, proceed.

## 1. Gather context

Run this to see what shipped recently — it prevents re-doing work that just landed:

```
git log -n 5 --format="%H%n%ad%n%B---" --date=short
```

Then determine whether the described work overlaps with any open issue files in `issues/` (skip `issues/done/`). If an existing issue already tracks this exact work, note it but continue — you are implementing the described work regardless.

## 2. Understand the task

Restate in one sentence what you are going to implement based on `{{args}}`. If you cannot form a coherent one-sentence statement of intent, stop and ask the user to clarify.

## 3. Explore

Read the files the task is likely to touch before changing anything. Form a clear picture of what changes are needed and where.

## 4. Implementation

Classify the work before choosing an approach:

- **Documentation or pipeline** (README updates, CI/CD config, GitHub Actions, NuGet packaging, changelog): implement directly — no TDD loop needed.
- **Code changes** (analyzers, source generators, library logic, tests): use the `tdd` skill to drive the change red-green-refactor.

## 5. Feedback loops

Skip this step entirely for pure documentation or pipeline work.

For code changes, before committing run and fix any failures:

```powershell
dotnet format &&dotnet build && dotnet test
```

If the `dotnet format` gate fails, fix any violations and ensure `dotnet build` and `dotnet test` pass before committing.

## 6. Commit

Make one git commit. The message must include:

1. A summary of the work described in the original argument
2. Key decisions made
3. Files changed
4. Blockers or notes for follow-up work (if any)

## Rules

- ONLY WORK ON A SINGLE TASK per invocation.
- Do not pick up unrelated work while implementing.
- If the work turns out to be much larger than the description implies, implement the minimal correct slice and note what's left in the commit message.
