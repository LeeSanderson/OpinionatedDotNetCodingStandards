# Refactor Candidates

After a TDD cycle, look for:

- **Duplication** → Extract a method or helper. A shared extensions type (e.g. `FileSystemExtensions`, `StringExtensions`) is the natural home for cross-component helpers.
- **Long methods** → Break into private helpers (keep tests on the public `RunAsync` / `Parse` interface). Pulling a multi-step routine out of the public method but keeping it private is the model — the tests don't change.
- **Shallow modules** → Combine or [deepen](deep-modules.md). Watch for helpers that only exist to be called once.
- **Feature envy** → Move logic to where the data lives. A method on a handler that mostly reads from some record belongs on (or near) that record.
- **Primitive obsession** → Introduce value objects instead of passing tuples or bare primitives (e.g. a `DateRange` rather than two loose `DateOnly`s). Prefer extending the existing value-object vocabulary.
- **Existing code** that the new code reveals as problematic — note it, raise it as a follow-up issue under `issues/`, don't expand scope mid-cycle.

After each refactor step run `dotnet test` (fast). If you use snapshot tests, verified snapshots may need updating — review the `.received.*` vs `.verified.*` diff carefully and accept the new output only when it is actually what you wanted.
