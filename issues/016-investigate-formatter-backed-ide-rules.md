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

## Acceptance criteria

- The root cause is identified and linked to a Roslyn source location or GitHub issue.
- If the behaviour is a bug: file or link a Roslyn issue; update the `Untestable` reason
  with the issue URL.
- If the behaviour is intentional and permanent: confirm the `Untestable` entries are
  correctly worded and close this issue.
- If a workaround is found (e.g. editorconfig override that causes the rule to emit its
  own ID): implement it and promote the rules to tested.
