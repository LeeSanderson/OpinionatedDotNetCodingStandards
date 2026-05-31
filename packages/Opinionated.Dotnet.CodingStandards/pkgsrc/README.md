# Opinionated.DotNet.CodingStandards

Strongly opinionated coding standards for .NET — a single development-dependency
package that wires in four Roslyn analyzer packages plus a curated editorconfig,
MSBuild props, and targets so every project in your solution enforces the same
quality bar automatically.

## Installation

```bash
dotnet add package Opinionated.DotNet.CodingStandards
```

Because it is a `developmentDependency` package it has no impact on your published
assemblies or transitive consumers.

## Rule reference

The full table of enforced rules (rule ID, description, severity, help link) is in
[`docs/rule-reference.md`](https://github.com/LeeSanderson/OpinionatedDotNetCodingStandards/blob/main/docs/rule-reference.md)
in the repository.

## Configurable ban toggles

Seven banned-API groups are enabled by default. Each can be opted out of individually
by setting the corresponding MSBuild property to `false` in your project file or in a
`Directory.Build.props`.

```xml
<PropertyGroup>
  <!-- Opt out of any toggle you don't need: -->
  <BanNonUtcDateApis>false</BanNonUtcDateApis>
  <BanInvariantCultureStringComparisonApis>false</BanInvariantCultureStringComparisonApis>
  <BanEnumTryParseWithoutIgnoreCaseApis>false</BanEnumTryParseWithoutIgnoreCaseApis>
  <BanRoundWithoutMidpointRoundingApis>false</BanRoundWithoutMidpointRoundingApis>
  <BanUseOfCultureInfoConstructorApis>false</BanUseOfCultureInfoConstructorApis>
  <BanUseOfTupleInFavourOfValueTupleApis>false</BanUseOfTupleInFavourOfValueTupleApis>
  <BanUseOfNewtonsoftJsonApis>false</BanUseOfNewtonsoftJsonApis>
</PropertyGroup>
```

| Property | What it bans | Reason |
|----------|-------------|--------|
| `BanNonUtcDateApis` | `DateTime.Now`, `DateTimeOffset.Now`, etc. | Prefer explicit UTC to avoid timezone bugs |
| `BanInvariantCultureStringComparisonApis` | `string.Compare` / `IndexOf` without `StringComparison` | Locale-sensitive comparisons are non-deterministic |
| `BanEnumTryParseWithoutIgnoreCaseApis` | `Enum.TryParse` without `ignoreCase` parameter | Silently case-sensitive parsing is a common bug |
| `BanRoundWithoutMidpointRoundingApis` | `Math.Round` without `MidpointRounding` | Default banker's rounding surprises most developers |
| `BanUseOfCultureInfoConstructorApis` | `new CultureInfo(...)` | Prefer `CultureInfo.GetCultureInfo` (cached, safer) |
| `BanUseOfTupleInFavourOfValueTupleApis` | `Tuple<T1, T2, ...>` | Use `(T1, T2)` value-tuple syntax instead |
| `BanUseOfNewtonsoftJsonApis` | `Newtonsoft.Json` types | Prefer `System.Text.Json` |

## Style and naming conventions

The following choices are configured via editorconfig and apply across the solution.
They are enforced as IDE suggestions where a Roslyn diagnostic exists; otherwise they
are expressed as editorconfig formatting rules.

### `var` usage

Use `var` everywhere the type is apparent or inferred — for built-in types, when the
type appears on the right-hand side, and everywhere else.

```csharp
var count = 0;
var items = new List<string>();
var result = GetResult();
```

### Namespace declarations

File-scoped namespaces (`namespace Foo;`) are preferred over block-scoped
(`namespace Foo { … }`).

### Braces

All control-flow blocks (`if`, `for`, `while`, etc.) must use braces, even for
single-line bodies.

### Expression-bodied members

- **Properties and accessors**: expression bodies (`=>`) preferred.
- **Methods and constructors**: block bodies preferred.

### Operator placement when wrapping

When a binary expression wraps across lines, the operator goes at the **beginning** of
the continuation line.

### Accessibility modifiers

Explicit accessibility modifiers are required on all non-interface members.

### Naming conventions

| Symbol | Convention | Example |
|--------|-----------|---------|
| Interfaces | `I` prefix + PascalCase | `IRepository` |
| Classes, structs, enums | PascalCase | `OrderService` |
| Methods, properties, events | PascalCase | `GetById`, `IsValid` |
| Private instance fields | `_camelCase` | `_repository` |
| `static readonly` fields | PascalCase | `DefaultTimeout` |
| `const` fields | PascalCase | `MaxRetries` |
| Parameters and local variables | camelCase | `orderId`, `result` |

### `using` directives

- Placed **outside** the namespace declaration.
- `System.*` directives sorted first.
- No blank lines between import groups.

### Language keywords vs BCL types

Use language keywords (`int`, `string`, `bool`) rather than BCL type names
(`Int32`, `String`, `Boolean`) for both locals and member access.

### Nullable reference types and warnings-as-errors

Nullable context is enabled and all warnings are treated as errors.
`implicit usings` and the latest C# language version are also set by default.
