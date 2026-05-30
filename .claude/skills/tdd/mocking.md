# When to Mock

Mock at **system boundaries** only. In a .NET codebase those boundaries are typically:

- **External services / the network** — any remote API or site. Hide it behind an interface (e.g. `IClient`), with the transport (`HttpClient`) one level down. Tests stub the interface; they don't reach the network.
- **The file system** — hide it behind `IFileSystem` from `System.IO.Abstractions`. All file/stream I/O goes through it, so tests can capture written content in memory.
- **The clock** — hide it behind a clock abstraction (e.g. `IClock`). Production uses a real clock; tests substitute a fixed one.
- **Other ambient state** — environment variables, randomness, machine identity — wrap and inject.

**Don't mock:**

- Internal parsers, mappers, calculators — they're (ideally) pure functions over their inputs; exercise them with real fixture data.
- Domain types, value objects, and other internal collaborators — they're not boundaries.
- Logging — pass a test logger that forwards to the test output sink instead of mocking `ILogger<T>`.

## Designing for Mockability

### 1. Constructor inject the boundary, never `new` it inside

```csharp
// GOOD: every boundary is injected
public class ImportCommandHandler(
    IFileSystem fileSystem,
    IClient client,
    IClock clock,
    ILogger<ImportCommandHandler> logger)
    : CommandHandlerBase<ImportCommandHandler, ImportOptions>(fileSystem, logger)
{
    protected override async Task InternalRunAsync(ImportOptions options) { ... }
}

// BAD: a handler that owns its dependencies — impossible to test without hitting the network
public class ImportCommandHandler
{
    private readonly HttpClient _http = new();
    private readonly IFileSystem _fs = new FileSystem();
    private DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
```

### 2. Keep the boundary interface narrow and per-operation (SDK-style)

Give the boundary one method per logical operation, so each can be stubbed independently:

```csharp
// GOOD: each operation is independently mockable
public interface IClient
{
    IAsyncEnumerable<string> ListItemUrls(DateOnly start, DateOnly end);
    Task<TItem> DownloadItem(string url);
}

// BAD: one generic fetcher forces tests to branch on arguments inside the mock setup
public interface IClient
{
    Task<object> Fetch(string kind, DateOnly date, string? url = null);
}
```

The narrow-per-operation style lets a reusable fake compose stubs cleanly:

```csharp
_fakeClient = await FakeClient
    .New()
    .WithItemUrls()
    .WithDownloadedItems();
```

### 3. Stub returns, don't verify calls

Most mocking libraries make both `.Returns(...)` and `.Received(n)` easy. Prefer `.Returns(...)` for boundary inputs and assert on the observable output (e.g. the content captured by a faked `IFileSystem`):

```csharp
// GOOD: arrange boundary behavior, assert on what was produced
_fakeFileSystem.File.Exists(ExistingOutputPath).Returns(true);
await SeedExistingOutput(ExistingOutputPath);

var result = await ExecuteHandler();

result.Should().Be(ExitCodes.Success);
await _fakeFileSystem.File
    .DidNotReceive().WriteAllTextAsync(ExistingOutputPath, Arg.Any<string>());
```

The `DidNotReceive` here is acceptable because *not writing the file* is the user-observable behavior we care about ("skip work when output already exists"). Contrast with asserting `.Received()` on an internal client call to verify internal sequencing — that's coupling to implementation.

### 4. For HTTP-layer tests, fake the message handler

When a component takes an `HttpClient`, inject one built from a fake message handler (e.g. `RichardSzalay.MockHttp`'s `MockHttpMessageHandler`) to verify request shaping, retry, and error handling without mocking `HttpClient` itself.
