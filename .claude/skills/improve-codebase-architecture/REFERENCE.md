# Reference

## Dependency Categories

When assessing a candidate for deepening, classify its dependencies:

### 1. In-process

Pure computation, in-memory state, no I/O. Always deepenable — just merge the modules and test directly.

Examples: parsers, mappers, calculators, value objects, and extension helpers — anything that takes its inputs as arguments and returns a result with no I/O or static state.

### 2. Local-substitutable

Dependencies that have local test stand-ins. Deepenable if the test substitute exists. The deepened module is tested with the local stand-in running in the test suite.

The canonical .NET example is `IFileSystem` from `System.IO.Abstractions` — production uses the real `FileSystem`, tests use an in-memory stand-in (often an `IFileSystem` substitute that captures written content). A clock abstraction (e.g. `IClock`) is similar: a real clock in production, a fixed one in tests.

### 3. Remote but owned (Ports & Adapters)

Your own services across a network boundary (microservices, internal APIs). Define a port (interface) at the module boundary. The deep module owns the logic; the transport is injected. Tests use an in-memory adapter. Production uses the real HTTP/gRPC/queue adapter.

Recommendation shape: "Define a shared interface (port), implement an HTTP adapter for production and an in-memory adapter for testing, so the logic can be tested as one deep module even though it's deployed across a network boundary."

### 4. True external (Mock)

Third-party services you don't control. Mock at the boundary. The deepened module takes the external dependency as an injected port (e.g. an `IClient` interface), and tests provide a fake implementation. For components that take an `HttpClient` directly, drive the HTTP layer with a fake message handler (e.g. `RichardSzalay.MockHttp`).

## Testing Strategy

The core principle: **replace, don't layer.**

- Old unit tests on shallow modules are waste once boundary tests exist — delete them
- Write new tests at the deepened module's interface boundary (typically a command handler's `RunAsync` or a parser's `Parse`)
- Tests assert on observable outcomes through the public interface (e.g. the content captured by a faked `IFileSystem`, or the parsed result object), not internal state
- Tests should survive internal refactors — they describe behavior, not implementation

## Issue Template

<issue-template>

## Problem

Describe the architectural friction:

- Which modules are shallow and tightly coupled (cite project + file paths)
- What integration risk exists in the seams between them (e.g. a contract/schema, or a producer/consumer handshake)
- Why this makes the codebase harder to navigate and maintain

## Proposed Interface

The chosen interface design:

- Interface signature (C# `interface` or abstract class — types, methods, params)
- Usage example showing how callers use it (which command handler / service)
- What complexity it hides internally

## Dependency Strategy

Which category applies and how dependencies are handled:

- **In-process**: merged directly (e.g. fold a helper into the parser)
- **Local-substitutable**: tested with `IFileSystem`/clock stand-ins
- **Ports & adapters**: port definition, production adapter (HTTP/gRPC/queue), in-memory test adapter
- **Mock**: mock the external boundary via an injected `IClient`-style port

## Testing Strategy

- **New boundary tests to write**: describe the behaviors to verify at the interface (e.g. "verify the content written for a given input")
- **Old tests to delete**: list the shallow module tests that become redundant
- **Test environment needs**: any fixtures or verified snapshots required

## Implementation Recommendations

Durable architectural guidance that is NOT coupled to current file paths:

- What the module should own (responsibilities)
- What it should hide (implementation details)
- What it should expose (the interface contract)
- How callers should migrate to the new interface

</issue-template>
