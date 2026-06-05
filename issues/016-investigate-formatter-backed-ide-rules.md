## Parent PRD

`issues/prd.md`

## Type

HITL — requires manual investigation of Roslyn internals, GitHub issue research,
and possibly SDK version testing across .NET 9 / .NET 10 / .NET 10.x patch releases.

## Background

Three IDE rules are currently marked `Untestable` in `UntestableRules.cs` because they
emit `IDE0055` ("Fix formatting") as their build-mode diagnostic ID rather than their
own rule ID. This was confirmed by systematic control/violation probe testing (commit
`64acd3f`, 2026-06-04):

| Rule | What it does | Expected in SARIF | Actual in SARIF |
|------|-------------|-------------------|-----------------|
| `IDE0260` | `obj as T != null` → `obj is T` (pattern matching) | `note:IDE0260` | `error:IDE0055` |
| `IDE0070` | Manual `GetHashCode` combining → `HashCode.Combine(...)` | `note:IDE0070` | `error:IDE0055` |
| `IDE0079` | Unnecessary `[SuppressMessage]` / `#pragma warning disable` | `note:IDE0079` | `error:IDE0055` |

## CA1066 — distinct but related: diagnostic simply absent from SARIF

CA1066 ("Implement IEquatable when overriding Object.Equals") is a separate failure
mode added during issue 010 (commit `63c9751`, 2026-06-04). Unlike the IDE rules above,
CA1066 does **not** re-route through IDE0055 — the formatter fires correctly when the
violation code is formatted correctly. The rule simply never appears in SARIF at all.

| Rule | What it does | Expected in SARIF | Actual in SARIF |
|------|-------------|-------------------|-----------------|
| `CA1066` | Class overrides `Equals(object)` without `IEquatable<T>` | `error:CA1066` | *(absent)* |

**Probes used (all formatted correctly with blank lines between types):**

```csharp
// violation: override Equals without IEquatable<Box> → no CA1066, no IDE0055
public class Box
{
    public override bool Equals(object? obj) => obj is Box;
    public override int GetHashCode() => 0;
}

// control: override Equals with IEquatable<Box> → no CA1066, no IDE0055 (correct)
public class Box : System.IEquatable<Box>
{
    public override bool Equals(object? obj) => obj is Box;
    public bool Equals(Box? other) => other != null;
    public override int GetHashCode() => 0;
}
```

Both violation and control produce only `error:SA1649` (file-name mismatch, harmless)
and zero CA1066. The rule is configured as `dotnet_diagnostic.CA1066.severity = warning`
in the package editorconfig with `TreatWarningsAsErrors=true`.

Note: CA1065 (also `# Enabled: False, Severity: warning` in the same editorconfig)
fires correctly as `error:CA1065`, so the `Enabled: False` comment alone does not
explain the absence.

**What to investigate for CA1066:**

1. Check whether CA1066 is marked with `EnforceOnBuild = Never` or equivalent metadata
   in the NetAnalyzers 10.0.x source (opposite of the IDE rule investigation above).
2. Confirm whether CA1066 was deprecated or folded into another rule in a recent
   NetAnalyzers release (e.g. merged into CA1067 or silently disabled).
3. Test on NetAnalyzers 9.x to see if the rule fired in earlier versions.

## What the probes showed

**Methodology:** paired violation/control tests differing only in whether the
triggering pattern is present.

### IDE0260

```csharp
// violation → IDE0055 at (2,1), IDE0260 absent
public static bool IsString(object? obj) => obj as string != null;

// control → no IDE0055, no IDE0260
public static bool IsString(object? obj) => obj is string;
```

### IDE0079

```csharp
// violation: suppress CA1822 on a method already declared static → IDE0055 at (3,1)
[SuppressMessage("Performance", "CA1822")]
public static int Main() { return 0; }

// control: suppress CA1822 on a method that would trigger it (no instance access) → no IDE0055
[SuppressMessage("Performance", "CA1822")]
public int GetZero() { return 0; }
```

### IDE0070

Two-file project (Program.cs + Point.cs):

```csharp
// violation in Point.cs → IDE0055 at (2,1) in BOTH Point.cs AND Program.cs
public override int GetHashCode() { return _x ^ _y; }

// control → neither file gets IDE0055
public override int GetHashCode() { return System.HashCode.Combine(_x, _y); }
```

**Notable:** IDE0070 fires IDE0055 in **every file** in the compilation, not just the
file containing the violation. Program.cs, which has no GetHashCode at all, still
receives `Program.cs(2,1): error IDE0055`.

## Observed characteristics of the IDE0055 diagnostic

- **Position:** always `(N, 1)` where line N is the first line of the first type
  declaration in the file (i.e. `public static class Program` or `public class Point`).
  Never at the line of the actual violating expression.
- **Scope:** for IDE0070, fires in all files in the compilation simultaneously.
- **Exclusivity:** IDE0055 is the only diagnostic in the SARIF. The target rule ID is
  completely absent — it is not suppressed, not demoted; it simply does not appear.
- **Build outcome:** IDE0055 is configured as `warning` severity and the project has
  `TreatWarningsAsErrors=true`, so it causes build failure. `HasNote("IDE0260")` etc.
  return false because the SARIF contains only `error:IDE0055`.

## Current working hypothesis

These three rules use Roslyn's formatter pipeline as their build-mode enforcement
mechanism. Rather than emitting a diagnostic under their own ID, they schedule a
"format document" operation whose outcome is reported as IDE0055. This would explain:

- Why the diagnostic position is at the type-declaration level (the formatter operates
  on the whole document, not a specific AST node)
- Why IDE0070 triggers IDE0055 in files that contain no violation (the formatter is
  dispatched across the whole compilation)
- Why the target ID never appears — the formatter doesn't know which style rule
  triggered it

This may be intentional (the rules are designed to route through the formatter in
build mode) or a bug in Roslyn's build-time analyzer for .NET 10.

## What to investigate

1. **Roslyn source**: Look at the `DocumentHighlightsService`, `FormattingCodeFixProvider`,
   and the `AbstractAddOrRemoveUnnecessarySuppressionDiagnosticAnalyzer` (IDE0079),
   `UsePatternMatchingCodeFixProvider` (IDE0260), `UseSystemHashCodeDiagnosticAnalyzer`
   (IDE0070). Confirm whether these analyzers emit their diagnostics through the formatter
   in the `EnforceOnBuild` path.

2. **Roslyn GitHub**: Search for issues relating to IDE0260 / IDE0070 / IDE0079 appearing
   as IDE0055 in build output. Known Roslyn issue tracker:
   https://github.com/dotnet/roslyn/issues

3. **SDK version matrix**: Test whether the behaviour is specific to .NET 10 / Roslyn
   4.x.y. Try reproducing on:
   - .NET 9 SDK (Roslyn 4.8.x)
   - .NET 10 SDK as used here (Roslyn 4.12.x or similar)
   - Latest .NET 10 preview if newer than the current toolchain

4. **`EnforceOnBuild` metadata**: Check the rule descriptors in
   `Microsoft.CodeAnalysis.CSharp.Features` to see if these three rules have
   `CustomTags.NotConfigurable` or a custom `EnforceOnBuildValues` that differs from
   rules like IDE0270 (which does emit its own ID in build SARIF).

5. **Workaround**: Investigate whether adding an explicit
   `dotnet_diagnostic.IDE0260.severity = suggestion` (or `warning`) override to the
   editorconfig forces the analyzer to emit the rule's own ID rather than routing
   through IDE0055.

## New untestable rules from issue 011 (added 2026-06-05)

Five CA rules were marked `Untestable` during the issue-011 backfill pass. They fall into
three distinct failure modes, described below.

### CA1419 — diagnostic absent from SARIF (same mode as CA1066)

CA1419 ("Provide a parameterless constructor … for concrete types derived from SafeHandle")
does not appear in SARIF when a concrete public `SafeHandle` subclass has no parameterless
constructor. The rule is configured as `dotnet_diagnostic.CA1419.severity = warning` with
`TreatWarningsAsErrors=true`. Observed in the build harness for NetAnalyzers 10.0.x.

**Probe used:**
```csharp
public class MyHandle : SafeHandle
{
    public MyHandle(IntPtr handle) : base(IntPtr.Zero, true) { SetHandle(handle); }
    public override bool IsInvalid => handle == IntPtr.Zero;
    protected override bool ReleaseHandle() => true;
}
```
SARIF contains only `error:IDE0055` and `error:SA1649` — no `CA1419`.

**What to investigate:**
1. Check whether `CA1419` is flagged `EnforceOnBuild = Never` in the NetAnalyzers source
   (same check recommended for CA1066 above).
2. Confirm whether the rule requires P/Invoke context (e.g. the SafeHandle is passed to a
   `[DllImport]` method) rather than firing on the type declaration alone.
3. Test whether adding a `[DllImport]` call that uses `MyHandle` as a parameter type causes
   CA1419 to fire.

---

### CA1420 / CA1421 — require `[assembly: DisableRuntimeMarshalling]`

CA1420 ("Property, type, or attribute requires runtime marshalling") and CA1421 ("This
method uses runtime marshalling even when `DisableRuntimeMarshallingAttribute` is applied")
only fire when the assembly has `[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]`
applied. Adding this attribute to the test project has assembly-wide impact — it changes
the marshalling semantics of every P/Invoke in the compilation, making isolated testing
impractical with the current single-file harness.

**What to investigate:**
1. Confirm whether the test harness's `CreateProjectBuilder` can be extended to support
   a second (separate) assembly-attributes file, keeping `DisableRuntimeMarshalling`
   isolated to the violation project without contaminating the test runner itself.
2. Identify the minimal violation pattern for each rule so a probe test can be written
   once the harness issue is resolved.
3. Check the NetAnalyzers source for CA1420/CA1421 to understand exactly which P/Invoke
   parameter types trigger each rule (e.g. `string` param vs `[MarshalAs]` attribute vs
   value-type with `[StructLayout]`).

---

### CA1516 — requires hardware SIMD intrinsics

CA1516 ("Use cross-platform intrinsics") fires when platform-specific hardware intrinsics
(`System.Runtime.Intrinsics.X86.*`, `System.Runtime.Intrinsics.Arm.*`) are used where
cross-platform `Vector128`/`Vector256` alternatives exist. No typical application code
triggers this rule, so no natural violation exists in the single-project harness.

**What to investigate:**
1. Confirm the minimal violation pattern: e.g. does `Sse2.Add(...)` without an
   `Sse2.IsSupported` guard (or with an alternative `Vector128` path available) fire CA1516?
2. Determine whether the test project's target framework and CPU architecture affect
   whether the rule fires at build time. CA1516 may only fire for projects that explicitly
   opt into unsafe / intrinsics code via `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`.
3. If a minimal probe compiles and fires the rule, promote CA1516 to tested.

---

### CA1727 — requires `Microsoft.Extensions.Logging`

CA1727 ("Use PascalCase for named placeholders") fires on non-PascalCase placeholder names
in structured logging message templates — specifically `ILogger.Log*` extension method
calls from `Microsoft.Extensions.Logging`. The package is not available in the simple
`net8.0` console-app projects created by the test harness.

**What to investigate:**
1. Determine whether the test harness's `CreateProjectBuilder` supports adding NuGet
   package references to the generated project. If so, add
   `Microsoft.Extensions.Logging.Abstractions` and write a minimal ILogger test.
2. Check whether CA1727 also fires for `[LoggerMessage]`-attributed partial methods
   (which are declared without calling `ILogger` at the call site) — this pattern might
   require only the `Abstractions` package rather than the full `Logging` stack.
3. As a fallback, confirm whether the `Opinionated.DotNet.CodingStandards` NuGet package
   itself transitively exposes `Microsoft.Extensions.Logging.Abstractions`, which would
   make `ILogger` available in test projects without an extra package reference.

---

## New untestable rules from issue 012 (added 2026-06-05)

Eight CA rules were marked `Untestable` during the issue-012 backfill pass (CA18xx
NetAnalyzers performance rules). They fall into two distinct failure modes.

### CA1802 / CA1842 / CA1843 / CA1853 / CA1870 — diagnostic absent from SARIF

These five rules are configured as `dotnet_diagnostic.<ID>.severity = warning` with
`TreatWarningsAsErrors=true` but never appear in SARIF for any tested violation pattern.

| Rule | What it does | Expected in SARIF | Actual in SARIF |
|------|-------------|-------------------|-----------------|
| `CA1802` | `public static readonly` field initialized with compile-time constant → should be `const` | `error:CA1802` | *(absent)* |
| `CA1842` | `Task.WhenAll(singleTask)` → redundant, use task directly | `error:CA1842` | *(absent)* |
| `CA1843` | `Task.WaitAll(singleTask)` → redundant, use task directly | `error:CA1843` | *(absent)* |
| `CA1853` | `dict.ContainsKey(k)` followed by `dict[k]` / `dict.TryGetValue` → use `TryGetValue` directly | `error:CA1853` | *(absent)* |
| `CA1870` | `span.IndexOfAny(SearchValues.Create(...))` inline → cache `SearchValues` as static field | `error:CA1870` | *(absent)* |

**Probes used (CA1802):**
```csharp
// violation: public static readonly field with constant initializer
public class Config
{
    public static readonly string DefaultName = "app";
    public static readonly int MaxRetries = 3;
}
```
SARIF: absent (other diagnostics such as IDE0005/IDE0055 fire but not CA1802).

**Probes used (CA1842/CA1843):**
```csharp
// CA1842 violation: Task.WhenAll with a single task
var t = Task.FromResult(42);
await Task.WhenAll(t);

// CA1843 violation: Task.WaitAll with a single task
var t = Task.FromResult(0);
Task.WaitAll(t);
```
Multiple patterns tried (generic `Task<T>`, non-generic `Task`, `Task.CompletedTask`,
`Task.Delay(0)`). CA1842/CA1843 never appear in SARIF.

**Probes used (CA1853):**
```csharp
// violation: ContainsKey + TryGetValue
if (!dict.ContainsKey("key"))
    dict.TryGetValue("key", out _);

// also tried: ContainsKey + dict[key], ContainsKey + TryAdd
```
CA1853 absent; CA1864 (ContainsKey + Add → TryAdd) fires correctly.

**Probes used (CA1870):**
```csharp
// violation: SearchValues.Create inline in IndexOfAny
var text = "hello world".AsSpan();
text.IndexOfAny(System.Buffers.SearchValues.Create("aeiou"));

// also tried: ContainsAny, fully-qualified type, various using styles
```
CA1870 absent across all patterns; IDE0055 and IDE0007 fire but CA1870 never appears.

**What to investigate for all five rules:**
1. Check whether each rule is flagged `EnforceOnBuild = Never` or `CustomTags.NotConfigurable`
   in the NetAnalyzers 10.0.x source — same investigation recommended for CA1066 and CA1419.
2. Check for any `.editorconfig` or MSBuild property that silently overrides the configured
   severity to `none` in the build harness context.
3. Test on NetAnalyzers 9.x to determine if these rules fired in earlier versions.

---

### CA1845 / CA1867 — subsumed by another rule in .NET 10

These two rules fire correctly in isolation, but a different (more general) rule fires
first for the canonical violation pattern, preventing the specific rule from appearing.

| Rule | Expected | Subsumes | Why |
|------|----------|----------|-----|
| `CA1845` | `string.Concat(s.Substring(0, n), "!")` → use span-based Concat | `IDE0057` fires instead | The Substring call fires "Substring can be simplified to range indexer" before CA1845 sees the concat |
| `CA1867` | `s.EndsWith("x", StringComparison.Ordinal)` → use char overload | `CA1865` fires instead | In .NET 10 CA1865 fires for both StartsWith and EndsWith with Ordinal; CA1867 is only supposed to cover EndsWith without StringComparison, but CA1866 fires for that case |

**CA1845 probe:**
```csharp
// canonical violation — fires IDE0057, not CA1845
string.Concat(s.Substring(0, 5), "!");

// suppressing IDE0057 does not reveal CA1845 — still absent
```

**CA1867 probe:**
```csharp
// EndsWith without StringComparison → CA1866 fires (not CA1867)
s.EndsWith("x");

// EndsWith with StringComparison.Ordinal → CA1865 fires (not CA1867)
s.EndsWith("x", StringComparison.Ordinal);
```
In .NET 10, CA1866 covers both `StartsWith(string)` and `EndsWith(string)` without
StringComparison; CA1865 covers both with `StringComparison.Ordinal`. CA1867 never fires
independently for any tested EndsWith pattern.

**What to investigate:**
1. **CA1845**: Determine whether suppressing IDE0057 (e.g. via `#pragma warning disable IDE0057`)
   causes CA1845 to appear. If so, the rule can be tested with an explicit suppression in the
   violation file.
2. **CA1867**: Check the NetAnalyzers 10.0.x source to confirm whether CA1867 was intentionally
   subsumed by CA1866/CA1865, or whether a separate trigger pattern exists that fires CA1867
   independently (e.g. `Contains(string)` with a single-char argument).

---

## Acceptance criteria

- The root cause is identified and linked to a Roslyn source location or GitHub issue.
- If the behaviour is a bug: file or link a Roslyn issue; update the `Untestable` reason
  with the issue URL.
- If the behaviour is intentional and permanent: confirm the `Untestable` entries are
  correctly worded and close this issue.
- If a workaround is found (e.g. editorconfig override that causes the rule to emit its
  own ID): implement it and promote the rules to tested.
