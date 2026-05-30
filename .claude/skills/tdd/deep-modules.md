# Deep Modules

From "A Philosophy of Software Design":

**Deep module** = small interface + lots of implementation

```
┌─────────────────────┐
│   Small Interface   │  ← Few methods, simple params
├─────────────────────┤
│                     │
│                     │
│  Deep Implementation│  ← Complex logic hidden
│                     │
│                     │
└─────────────────────┘
```

**Shallow module** = large interface + little implementation (avoid)

```
┌─────────────────────────────────┐
│       Large Interface           │  ← Many methods, complex params
├─────────────────────────────────┤
│  Thin Implementation            │  ← Just passes through
└─────────────────────────────────┘
```

## Examples to imitate (.NET shapes)

**Deep modules to imitate:**

- A parser with a single entry point — `Parse(string input) → Task<TResult>` — that hides all the selectors, sub-parsing, and normalization behind one method.
- An external-dependency client interface (e.g. `IClient`) whose few methods hide URL construction, transport, retry handling, and deserialization.
- A command handler — `RunAsync(TOptions options)` — whose one public method hides input validation, branching, existing-state detection, and output serialization.

**Watch for shallow drift:**

- Per-field getters on a parser (`GetName(input)`, `GetDate(input)`, `GetItems(input)`) — the interface becomes as wide as the data model; merge into a single `Parse`.
- Helper classes that exist only to be called by one caller and only forward to a single library call — fold them into the caller or deepen them with the surrounding logic.

When designing or reviewing an interface, ask:

- Can I reduce the number of methods?
- Can I simplify the parameters? (Prefer an options record over six positional args.)
- Can I hide more complexity inside? (Could the caller stop knowing about the internal helpers entirely?)
