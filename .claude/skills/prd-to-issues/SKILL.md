---
name: prd-to-issues
description: Break a PRD into independently-workable issues and write each as a local markdown file in issues/. Use when the user wants to turn a PRD into a list of concrete tasks.
---

# PRD to Issues

Break a PRD into independently-grabbable issues using vertical slices (tracer bullets), written as local markdown files under `issues/`.

## Project context

This is a .NET solution. A "vertical slice" cuts through every layer the change touches, end-to-end:

- Domain/core: new or changed types, services, or parsers.
- Application/entry layer: a new/changed command handler, API endpoint, or hosted service, plus its dependency-injection wiring.
- Public contract: any new fields on a DTO/record or change to a serialized output format.
- Tests: paired xUnit tests in the corresponding test project that exercise the new behavior through its public entry point.

A complete slice is verifiable by building and running the relevant entry point (or the focused tests for it) and seeing the new behavior produce the expected output.

## Process

### 1. Locate the PRD

Ask the user for the PRD file path (e.g. `issues/prd.md`).

If the PRD is not already in your context window, read it from the file.

### 2. Explore the codebase (optional)

If you have not already explored the codebase, do so to understand the current state of the code — especially the existing public entry points, the contracts/outputs they produce, and which existing types and services the change would consume.

### 3. Draft vertical slices

Break the PRD into **tracer bullet** issues. Each issue is a thin vertical slice that cuts through ALL integration layers end-to-end, NOT a horizontal slice of one layer.

Slices may be 'HITL' or 'AFK'. HITL slices require human interaction, such as an architectural decision or a design review. AFK slices can be implemented and merged without human interaction. Prefer AFK over HITL where possible.

<vertical-slice-rules>
- Each slice delivers a narrow but COMPLETE path through every relevant layer (domain type + service + entry-point wiring + tests)
- A completed slice is demoable on its own — running the affected entry point or its tests produces a visibly different, verifiable result
- Prefer many thin slices over few thick ones (e.g. "add the new field to the output record" before "compute the new field from upstream data")
</vertical-slice-rules>

### 4. Quiz the user

Present the proposed breakdown as a numbered list. For each slice, show:

- **Title**: short descriptive name
- **Type**: HITL / AFK
- **Blocked by**: which other slices (if any) must complete first
- **User stories covered**: which user stories from the PRD this addresses

Ask the user:

- Does the granularity feel right? (too coarse / too fine)
- Are the dependency relationships correct? (e.g. does the consumer change actually need the contract/schema change to land first?)
- Should any slices be merged or split further?
- Are the correct slices marked as HITL and AFK?

Iterate until the user approves the breakdown.

### 5. Create the issue files

For each approved slice, write a markdown file in `issues/` using the naming pattern `issues/NNN-short-title.md` (e.g. `issues/004-add-correlation-id-to-response.md`).

Number issues starting from the next available number (check what files already exist in `issues/`).

Create files in dependency order (blockers first) so you can reference real filenames in the "Blocked by" field.

Do NOT use `gh issue create` or any GitHub CLI commands. Do NOT reference GitHub issue numbers. Use local filenames for all cross-references.

<issue-template>
## Parent PRD

`issues/prd.md` (or whichever PRD file was used)

## What to build

A concise description of this vertical slice. Describe the end-to-end behavior, not layer-by-layer implementation. Reference specific sections of the parent PRD rather than duplicating content. Name the entry point / service affected.

## Acceptance criteria

- [ ] Criterion 1 (e.g. invoking `<entry point>` produces output with field X)
- [ ] Criterion 2 (e.g. the matching test project has a new `Should` test asserting on the observable output)
- [ ] Criterion 3 (e.g. existing behavior/contract continues to work without regression)

## Blocked by

- Blocked by `issues/NNN-title.md` (if any)

Or "None - can start immediately" if no blockers.

## User stories addressed

Reference by number from the parent PRD:

- User story 3
- User story 7

</issue-template>

Do NOT close or modify the parent PRD file.
