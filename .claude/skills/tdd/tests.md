# Good and Bad Tests

A well-structured .NET test suite uses **xUnit** with an assertion library, a mocking library used only at boundaries, and (optionally) snapshot testing for rich output. Tests are conventionally named `{Class}Should.{Behavior}` (e.g. `ImportCommandHandlerShould.SkipDownloadWhenOutputAlreadyExists`).

## Good Tests

**Integration-style**: Drive a command handler through its public `RunAsync` entry point with mocked system boundaries (`IFileSystem`, a clock, an external client), then assert on the observable side-effect — the content captured by the faked file system.

```csharp
// GOOD: drives the handler through its public entry point
[Fact]
public async Task ProducesOutputWhenNoneExists()
{
    var result = await ExecuteHandler();

    result.Should().Be(ExitCodes.Success);
    await Verify(_fakeFileSystem.GetContent(OutputPath));
}
```

Characteristics:

- Calls the real handler's `RunAsync` — no internal types mocked
- Mocks only the system boundaries (clock, file system, external client)
- Asserts on the content actually written (e.g. via a `Verify` snapshot), not on which collaborators were called
- Would survive a refactor that splits or merges private helpers

For pure parsers/calculators, feed real fixture input and assert on the returned result. A parser is a deep module — drive it through `Parse`, not its private helpers.

```csharp
// GOOD: exercises the real parser, asserts on the parsed result
[Fact]
public async Task ParsesAllItems()
{
    var result = await new ItemParser().Parse(FixtureData.SamplePayload);

    result.Items.Should().HaveCount(8);
}
```

## Bad Tests

**Implementation-detail tests**: Coupled to internal collaborators or the way the handler is wired together.

```csharp
// BAD: asserts on how many times an internal collaborator was called
[Fact]
public async Task Import_CallsClientForEachPage()
{
    await ExecuteHandler();

    await _fakeClient.Received(2)
        .ListItemUrls(Arg.Any<DateOnly>(), Arg.Any<DateOnly>());
}
```

Red flags:

- Asserts on call counts/arguments of an internal collaborator (`Received(2)`, `DidNotReceive()` on something that *isn't* a true system boundary)
- Test name describes HOW (calls the client twice) not WHAT (imports the missing items)
- Test breaks if the handler switches from per-page iteration to a single-range call, even though the produced output is identical

```csharp
// BAD: bypasses the handler to verify behavior
[Fact]
public async Task Import_ParsesItemsCorrectly()
{
    var parser = new ItemParser();
    var result = await parser.Parse(FixtureData.SamplePayload);
    result.Items.Should().HaveCount(8);
}
```

This is fine **as a parser test**, but not as an "Import" test — verify the handler's behavior through the output it produces, not by independently invoking the parser.
