---
name: write-a-prd
description: Generate a PRD from the client brief and write it as a local markdown file in issues/. Use when the user wants to turn a client request into a structured PRD.
---

This skill will be invoked when the user wants to create a PRD. You may skip steps if you don't consider them necessary.

## Project context

This is a .NET solution. PRDs describe changes that typically cut across:

- Domain/core logic — types, services, and parsers (often in a `*.Core` or domain project).
- Application/entry layer — command handlers, API endpoints, or hosted services that orchestrate the domain.
- Tests — paired test projects (xUnit) that exercise the behavior.

The PRD lives at `issues/prd.md`. Vertical-slice issues that break it down live alongside as `issues/NNN-*.md`.

## Process

1. Ask the user for a long, detailed description of the problem they want to solve and any potential ideas for solutions.

2. Explore the repo to verify their assertions and understand the current state of the codebase — typically: which projects exist in the solution, the public entry points (command handlers, endpoints, public service methods), the shape of the inputs/outputs they produce, and which existing types the change would touch.

3. Interview the user relentlessly about every aspect of this plan until you reach a shared understanding. Walk down each branch of the design tree, resolving dependencies between decisions one-by-one. Useful branches in a .NET codebase: which entry point (new or extended)? which public contract (DTO/record/output schema) changes? which existing services or types are consumed? how is it wired into dependency injection / the composition root?

4. Sketch out the major modules you will need to build or modify to complete the implementation. Actively look for opportunities to extract deep modules that can be tested in isolation.

A deep module (as opposed to a shallow module) is one which encapsulates a lot of functionality in a simple, testable interface which rarely changes. Good shapes to aim for: a single public method that hides substantial logic (e.g. a `Parse(string) → Result` parser, an `IClient` abstraction over an external dependency, a `RunAsync(TOptions)` command handler).

Check with the user that these modules match their expectations. Check with the user which modules they want tests written for, and prioritize testing the behavior that matters most.

5. Once you have a complete understanding of the problem and solution, use the template below to write the PRD. The PRD should be written as a local markdown file at `issues/prd.md`. Create the `issues/` directory if it doesn't exist. Do NOT submit a GitHub issue or call any external service.

<prd-template>

## Problem Statement

The problem that the user is facing, from the user's perspective.

## Solution

The solution to the problem, from the user's perspective.

## User Stories

A LONG, numbered list of user stories. Each user story should be in the format of:

1. As an <actor>, I want a <feature>, so that <benefit>

<user-story-example>
1. As an API consumer, I want each response to include a correlation id, so that I can trace a request across logs
</user-story-example>

This list of user stories should be extremely extensive and cover all aspects of the feature.

## Implementation Decisions

A list of implementation decisions that were made. This can include:

- The modules that will be built/modified (e.g. a new command handler, a new domain service, an added field on an output record)
- The interfaces of those modules that will be modified
- Technical clarifications from the developer
- Architectural decisions (e.g. ports & adapters around a new external source)
- Contract/schema changes (DTOs, records, serialized output formats — note any backward-compatibility constraints on persisted or published data)
- API contracts (e.g. the shape of a JSON response or a generated file)
- Specific interactions (how the new step is wired into dependency injection / the composition root / the CLI)

Do NOT include specific file paths or code snippets. They may end up being outdated very quickly.

## Testing Decisions

A list of testing decisions that were made. Include:

- A description of what makes a good test (only test external behavior, not implementation details — drive code through its public entry point, e.g. a handler's `RunAsync`, and assert on the observable output)
- Which modules will be tested
- Prior art for the tests (point at existing test classes in the solution that model the right approach — handler tests driven through the public entry point, parser/service tests fed with real fixture inputs)

## Out of Scope

A description of the things that are out of scope for this PRD.

## Further Notes

Any further notes about the feature.

</prd-template>
