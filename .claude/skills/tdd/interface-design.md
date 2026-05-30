# Interface Design for Testability

Good interfaces make testing natural. The patterns to follow in a .NET codebase:

## 1. Accept dependencies, don't create them

Have components receive every boundary by constructor. This is what makes a handler testable without hitting the network or the disk: the test wires up substitutes for every boundary.

```csharp
// Testable — the test wires up substitutes for every boundary
public class ImportCommandHandler(
    IFileSystem fileSystem,
    IClient client,
    IClock clock,
    ILogger<ImportCommandHandler> logger)
    : CommandHandlerBase<ImportCommandHandler, ImportOptions>(fileSystem, logger);

// Hard to test — the handler reaches for ambient state at runtime
public class ImportCommandHandler
{
    private readonly Client _client = new(new HttpClient(), new RealClock());
    private DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
```

Reach for an abstraction over anything non-deterministic or external: the clock, the file system, the network, the environment.

## 2. Return results, don't produce side effects

Prefer methods that compute and return over methods that mutate shared state. A parser that takes input in and returns a result — no I/O, no statics — is fully deterministic and trivial to test.

```csharp
// Testable
public Task<TResult> Parse(string input);

// Hard to test
public Task Parse(string input); // writes parsed records into a static cache
```

## 3. Small surface area

Keep boundary interfaces narrow — just enough methods to cover the operations callers actually need. A clock abstraction might expose `Today`/`IsToday`; an external client might expose one method per logical operation. Each can be stubbed in a few lines.

When designing a new module:

- Fewer methods → fewer tests needed
- Fewer parameters → simpler test setup (collapse a wide parameter list into a single options record, e.g. `RunAsync(TOptions options)`)
- Hide complexity behind the method — keep the multi-step internal logic private, so tests only need to drive the public entry point
