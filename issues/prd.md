# PRD: Promote Four Rules from UntestableRules to Tested Coverage

## Problem Statement

Four analyzer rules currently listed in `UntestableRules.cs` are not genuinely untestable — they were marked as such due to wrong-probe errors or incorrect assumptions about analyzer gate mechanisms. As a result, coverage gaps exist in the test suite for rules that are already configured and actively enforced by the package. Consumers of the package receive no automated assurance that these rules fire as intended.

The four affected rules are:

- **MA0187** ("Use constructor injection instead of [Inject] attribute") — marked untestable because `Microsoft.AspNetCore.Components.InjectAttribute` is a framework-provided assembly not available as a NuGet package. However, the Meziantou analyzer uses a string-based metadata name lookup (`GetBestTypeByMetadataName`) with no assembly-identity verification, so a stub attribute in the correct namespace satisfies the gate.

- **S6798** ("[JSInvokable] attribute should only be used on public methods") — same root cause as MA0187; `Microsoft.JSInterop.JSInvokableAttribute` is framework-only, but SonarAnalyzer also uses pure string-based KnownType lookup.

- **MA0191** ("Do not use the null-forgiving operator") — marked untestable because "additional opt-in configuration" was claimed to be required. In fact, the rule scope is specifically `null!` or `default!` assignments; the original empirical test used general nullable-suppression patterns (`value!`, `_value!.Length`) that the rule does not target. Standard `dotnet_diagnostic.MA0191.severity = warning` (already in the editorconfig) is sufficient.

- **S6664** ("The code block contains too many logging calls") — marked untestable because the "Sonar build-time suppression logic" was claimed to block the rule. Investigation of the Sonar source shows `LegacyIsRegisteredActionEnabled` returns `true` when `ShouldExecuteRegisteredAction is null`, which is always the case in `dotnet build`. The original test used three calls of *different* log levels; per-category thresholds (Warning=1, Error=1, Information=2, Debug=4) mean a single category must be exceeded. Two `LogWarning` calls in one block is the minimal trigger.

## Solution

For each of the four rules:
1. Write a passing build-output test that demonstrates the rule fires on a minimal violation.
2. Remove the corresponding `[RuleDoc(..., Untestable = ...)]` entry from `UntestableRules.cs`.

MA0187 and MA0191 join the existing `MeziantouAnalyzers3Should` test class. S6798 and S6664 join the existing `SonarAnalyzerRulesNewShould` test class.

## User Stories

1. As a package consumer, I want MA0187 to be verified by the test suite, so that I can trust that the rule fires when a Blazor component uses `[Inject]` property injection instead of constructor injection.
2. As a package consumer, I want S6798 to be verified by the test suite, so that I can trust that the rule fires when a `[JSInvokable]`-attributed method is not public.
3. As a package consumer, I want MA0191 to be verified by the test suite, so that I can trust that the rule fires when `null!` or `default!` is used to suppress a null-reference warning.
4. As a package consumer, I want S6664 to be verified by the test suite, so that I can trust that the rule fires when too many logging calls of the same category appear in a single code block.
5. As a maintainer, I want UntestableRules.cs to only contain rules that are structurally impossible to test, so that wrong-probe entries do not obscure genuine untestable rules.
6. As a maintainer, I want each newly promoted rule to carry a `[RuleDoc]` attribute on its test method, so that the rule documentation link and rule ID are machine-readable by the coverage tooling.
7. As a contributor, I want the stub-attribute technique to be demonstrated in working tests for MA0187 and S6798, so that future Blazor-framework or JSInterop rules can be unblocked using the same pattern.

## Implementation Decisions

### MA0187 — Meziantou "Use constructor injection instead of [Inject] attribute"

- The test calls `CreateProjectBuilderAsync` with `properties: [(Name: "AssemblyVersion", Value: "9.0.0.0")]`. The analyzer checks `InjectAttributeSymbol.ContainingAssembly.Identity.Version >= 9.0.0.0`; since the stub attribute is defined in the test project, the test project's assembly version must satisfy this guard.
- The sample file defines two stubs in the `Microsoft.AspNetCore.Components` namespace: an empty `IComponent` interface and an `InjectAttribute` class inheriting from `System.Attribute`. Both are resolved by the analyzer via `GetBestTypeByMetadataName`, which performs a pure metadata-name lookup with no assembly-identity check.
- The violation class implements `IComponent` and has a property decorated with `[Microsoft.AspNetCore.Components.Inject]`. The rule fires because the property carries the attribute and the containing type implements `IComponent`.
- No negative (non-firing) test is needed; there is no consumer-facing opt-out mechanism for MA0187.

### S6798 — Sonar "[JSInvokable] attribute should only be used on public methods"

- The test calls `CreateProjectBuilderAsync` with no special properties.
- The sample file defines a stub `JSInvokableAttribute` class in the `Microsoft.JSInterop` namespace. The analyzer resolves it via `KnownType.Microsoft_JSInterop_JSInvokable` (string-based lookup, no assembly-identity check).
- The violation is a non-public (`private` or `internal`) method carrying `[Microsoft.JSInterop.JSInvokable]`.
- No negative test is needed.

### MA0191 — Meziantou "Do not use the null-forgiving operator"

- The test calls `CreateProjectBuilderAsync` with no special properties. The editorconfig already sets `dotnet_diagnostic.MA0191.severity = warning`.
- The sample file contains `string x = null!;` or `string field = default!;` — an assignment where the right-hand side is `null!` or `default!`. This is the specific pattern the rule targets.
- Previous wrong-probe test patterns (`value!`, `_value!.Length`) do not match the rule's scope and must not be used.
- No negative test is needed.

### S6664 — Sonar "The code block contains too many logging calls"

- The test calls `CreateProjectBuilderAsync` with `packageReferences: [(Name: "Microsoft.Extensions.Logging.Abstractions", Version: "10.0.0")]`. The rule guard `cc.Compilation.ReferencesAny(SupportedLoggingLibraries)` requires a supported logging library to be referenced before the node action is registered.
- The sample file uses `Microsoft.Extensions.Logging.ILogger` and contains two `logger.LogWarning(...)` calls in the same code block. The Warning-category threshold is 1; two calls (> 1) triggers the diagnostic.
- The editorconfig already sets `dotnet_diagnostic.S6664.severity = warning`. The Sonar build context's `LegacyIsRegisteredActionEnabled` returns `true` during `dotnet build` (the `ShouldExecuteRegisteredAction` delegate is null), so standard Roslyn severity override applies.
- No negative test is needed.

### UntestableRules.cs cleanup

- Remove the `[RuleDoc("MA0187", ...)]` entry.
- Remove the `[RuleDoc("S6798", ...)]` entry.
- Remove the `[RuleDoc("MA0191", ...)]` entry.
- Remove the `[RuleDoc("S6664", ...)]` entry.
- Leave all remaining entries (S6802, CA2266, IDE rules, VB-only rules, etc.) unchanged.

## Testing Decisions

Good tests in this project assert on the *observable SARIF output* of a real `dotnet build`, not on analyzer internals. Each test:
- Creates a temporary project via `CreateProjectBuilderAsync`.
- Adds a minimal C# sample file that contains exactly one rule violation.
- Calls `BuildAndGetOutputAsync()` and asserts `buildOutput.HasError("RuleId").ShouldBeTrue()`.

The test methods are decorated with `[RuleDoc("RuleId", "Rule title", HelpLink = "...")]`, which integrates with the package's rule-coverage reporting tooling.

**Prior art to follow:**
- `SonarAnalyzerRulesNewShould.LoggingInCatchShouldPassCaughtException` — demonstrates the pattern for S6667 (same file, same `Microsoft.Extensions.Logging.Abstractions` package reference, same ILogger usage structure). S6664 should follow this shape exactly.
- `BannedApiAnalyzersShould.ProhibitExternalAccessToInternalSymbolsOutsideRestrictedNamespace` — demonstrates defining stub types inline in a sample file to satisfy an analyzer's type-lookup gate. MA0187 and S6798 follow the same technique (simpler version: no multi-project setup required, stubs go directly in `sample.cs`).
- Any test in `MeziantouAnalyzers3Should` — demonstrates the standard single-file, single-violation shape for Meziantou rules. MA0187 and MA0191 follow this shape.

All four tests are integration tests against the real build pipeline. No unit mocks of analyzer internals are needed or desired.

## Out of Scope

- **S6802** ("Using lambda expressions in loops should be avoided in Blazor markup section") — requires both an `.razor` file parsed by the Blazor source generator AND an `IsEnabledByDefault=false` Sonar gate that is tied to Blazor project setup. Remains in `UntestableRules.cs`.
- **CA2266** ("File-based program entry point should start with '#!'") — requires a file-based C# program invoked via `dotnet run foo.cs`; structurally unreproducible in a project-based test harness. Remains in `UntestableRules.cs`.
- All other existing `UntestableRules.cs` entries (IDE0001/0002/0003/0038/0084, CA1047/CA2218/CA2224/CA2226/CA2258/CA2321, EnableGenerateDocumentationFile, S3216) — these remain untestable for the reasons already documented.
- No changes to any `.editorconfig` files (all four rules are already configured correctly).
- No changes to package metadata or versioning.

## Further Notes

- The stub-attribute technique works because all affected analyzers perform pure metadata-name lookups (`GetBestTypeByMetadataName` / `GetTypeByMetadataName` / `KnownType` string matching) without verifying the assembly that defines the type. This is the same technique used implicitly in `BannedApiAnalyzersShould` for `RestrictedInternalsVisibleToAttribute`.
- MA0187's assembly version guard (`ContainingAssembly.Identity.Version >= 9.0.0.0`) is satisfied by setting the test project's `AssemblyVersion` to `9.0.0.0` via an MSBuild property. The stub attribute lives in the test project, so `ContainingAssembly` is the test project itself.
- S6664's per-category thresholds (from source): Debug=4, Information=2, Warning=1, Error=1. Two `LogWarning` calls is the minimum violation. The rule also supports Serilog, NLog, log4net, and Castle.Core in addition to `Microsoft.Extensions.Logging`.
- The `[RuleDoc]` attribute removed from `UntestableRules.cs` must be added to the new test method; otherwise the rule-coverage check will report the rule as undocumented.
