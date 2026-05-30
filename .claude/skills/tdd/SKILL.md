---
name: tdd
description: Test-driven development with red-green-refactor loop. Use when user wants to build features or fix bugs using TDD, mentions "red-green-refactor", wants integration tests, or asks for test-first development.
---

# Test-Driven Development

## Project context

This is a .NET solution tested with xUnit. A typical, well-structured test surface uses:

- **xUnit** as the test framework (`[Fact]` / `[Theory]`), with an assertion library (e.g. Shouldly or FluentAssertions).
- **A mocking library** (e.g. NSubstitute or Moq) used only at system boundaries.
- **Snapshot testing** (e.g. Verify) for asserting on rich output, with `.verified.*` files committed alongside the tests.
- **HTTP-layer fakes** (e.g. `RichardSzalay.MockHttp`) when a component takes an `HttpClient`.

Tests are conventionally named `{Class}Should.{Behavior}`. Reusable test helpers/builders wire up the standard system-boundary fakes (file system, clock, external clients) so handlers can be driven end-to-end without real I/O.

## Philosophy

**Core principle**: Tests should verify behavior through public interfaces, not implementation details. Code can change entirely; tests shouldn't.

**Good tests** are integration-style: they exercise real code paths through public APIs. A command-handler test is the model — it calls the public entry point (e.g. `RunAsync`) and asserts on the observable result (the file/content/response produced, captured via a faked `IFileSystem` or an in-memory sink). It describes _what_ the system does, not _how_.

**Bad tests** are coupled to implementation. They mock internal collaborators (private parsers, helpers), assert on `Received(n)` call counts for things that aren't true boundaries, or independently invoke an internal component to verify a handler.

See [tests.md](tests.md) for good vs bad examples and [mocking.md](mocking.md) for where the system boundaries actually are (`IFileSystem`, a clock abstraction, `HttpClient`, an external-service client interface).

## Anti-Pattern: Horizontal Slices

**DO NOT write all tests first, then all implementation.** This is "horizontal slicing" — treating RED as "write all tests" and GREEN as "write all code."

This produces **crap tests**:

- Tests written in bulk test _imagined_ behavior, not _actual_ behavior
- You end up testing the _shape_ of things (data structures, method signatures) rather than user-facing behavior
- Tests become insensitive to real changes — they pass when behavior breaks, fail when behavior is fine
- You outrun your headlights, committing to test structure before understanding the implementation

**Correct approach**: Vertical slices via tracer bullets. One test → one implementation → repeat. Each test responds to what you learned from the previous cycle. Because you just wrote the code, you know exactly what behavior matters and how to verify it.

```
WRONG (horizontal):
  RED:   test1, test2, test3, test4, test5
  GREEN: impl1, impl2, impl3, impl4, impl5

RIGHT (vertical):
  RED→GREEN: test1→impl1
  RED→GREEN: test2→impl2
  RED→GREEN: test3→impl3
  ...
```

## Workflow

### 1. Planning

Before writing any code:

- [ ] Confirm with user what interface changes are needed (e.g. a new command handler, a new service, or a new parser)
- [ ] Confirm with user which behaviors to test (prioritize)
- [ ] Identify opportunities for [deep modules](deep-modules.md) (small interface, deep implementation)
- [ ] Design interfaces for [testability](interface-design.md)
- [ ] List the behaviors to test (not implementation steps)
- [ ] Get user approval on the plan

Ask: "What should the public interface look like? Which behaviors are most important to test?"

**You can't test everything.** Confirm with the user exactly which behaviors matter most. Focus testing effort on critical paths and complex logic, not every possible edge case.

### 2. Tracer Bullet

Write ONE test that confirms ONE thing about the system:

```
RED:   Write test for first behavior → test fails
GREEN: Write minimal code to pass → test passes
```

For a new command handler, the tracer bullet is usually the happy-path "does the work and produces the expected output" test, verified by driving the public entry point and asserting on the result (e.g. via a snapshot of the produced content).

### 3. Incremental Loop

For each remaining behavior:

```
RED:   Write next test → fails
GREEN: Minimal code to pass → passes
```

Rules:

- One test at a time
- Only enough code to pass current test
- Don't anticipate future tests
- Keep tests focused on observable behavior

### 4. Refactor

After all tests pass, look for [refactor candidates](refactoring.md):

- [ ] Extract duplication (often into a shared extensions/helpers type)
- [ ] Deepen modules (move complexity behind simple interfaces)
- [ ] Apply SOLID principles where natural
- [ ] Consider what new code reveals about existing code
- [ ] Run `dotnet test` after each refactor step

**Never refactor while RED.** Get to GREEN first.

## Checklist Per Cycle

```
[ ] Test describes behavior, not implementation
[ ] Test uses public interface only (e.g. RunAsync, Parse)
[ ] Test would survive internal refactor
[ ] Code is minimal for this test
[ ] No speculative features added
```
