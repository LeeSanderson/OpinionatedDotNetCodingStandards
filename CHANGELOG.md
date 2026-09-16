# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [v0.0.14]

### Added

- Twelve new rules covering `System.Diagnostics.Tracing.EventSource` correctness are now enforced
  as **warnings**. ETW builds an event's manifest reflectively from the concrete `EventSource`
  subclass, and `EventSource` swallows manifest-construction failures by design, so every defect in
  this family is silent at compile time *and* at runtime — the event simply never fires, or is
  decoded against the wrong schema. They fall into three groups:
  - *Shape of the type* — `MA0226` (an `EventSource` subclass should be `sealed`), `MA0233` (an
    abstract `EventSource` must not declare event methods), `MA0231` (an event method must not be
    `static`) and `MA0232` (an event method must not be an explicit interface implementation).
    Each describes a member that ETW's reflective discovery cannot see.
  - *Event identity* — `MA0228` (the event id must be greater than zero; ETW reserves `0`),
    `MA0229` (the id is already used by another event) and `MA0230` (the name is already used).
    Note that an event's name **is its method name**, so `MA0230` fires on overloads, not on the
    `EventName` argument.
  - *What is actually written* — `MA0234` (the id passed to `WriteEvent` must match the `[Event]`
    attribute), `MA0235` (the payload must match the method's parameters — an arity check),
    `MA0236` (the payload must use the parameters' declared order), `MA0237` (a method calling
    `WriteEventWithRelatedActivityId` must declare the related activity id as its first parameter,
    named `relatedActivityId`) and `MA0238` (the parameter type is not one ETW can serialise —
    `DateTime`, `Guid`, `byte[]`, `IntPtr`, enums, `string` and the primitives are; `Uri`, `object`,
    `DateTimeOffset`, `decimal` and collections are not).
- `MA0227` (avoid using `Enumerable.Contains` on a set) is now enforced as a **warning**. It fires
  on the two shapes where a set's O(1) lookup silently degrades to a linear scan: passing a
  comparer (`set.Contains(value, StringComparer.Ordinal)`, which has no `ICollection<T>` fast path
  at all), and searching for a value whose type differs from the set's element type (an `object` in
  a `HashSet<string>`, say). `Enumerable.Contains` only delegates to the set's own lookup when the
  source implements `ICollection<T>` *for the searched value's type*. Covers `HashSet<T>`,
  `ISet<T>`, `IReadOnlySet<T>` and `IImmutableSet<T>` receivers.
- `MA0239` (use `typeof` instead of `GetType()` when the type is sealed) is now enforced as a
  **warning**. When the receiver's static type is `sealed`, `GetType()` is a virtual call that can
  only ever return that one type, so `typeof(T)` is exactly equivalent, resolved at compile time,
  and cannot throw on a null receiver. Upstream ships this rule at `suggestion`; it is enforced
  here at `warning` because it fires only on that narrow, mechanically fixable shape — unlike
  `MA0214`/`MA0215`/`MA0217`/`MA0219`, which were deliberately downgraded for firing on ubiquitous
  idiomatic code.

All fourteen rules are disabled by default upstream; this package turns each of them on.

### Changed

- Bumped Meziantou.Analyzer from 3.0.228 to 3.0.259. The fourteen rules above are the only new rule
  IDs, but upstream also widened three rules this package already enforces: `MA0206` now also
  reports on `record struct` and interface declarations, `MA0068` now reports on
  `[NotNullIfNotNull]` parameter and property placements, and `MA0179` now detects a constant on
  the left-hand side of a length comparison. `MA0209` and `MA0210` each had false positives fixed,
  so they should fire slightly less often. `MA0070` (enforced here at `suggestion`) now reports on
  every declaration that can be obsolete. Other rules changed upstream in this range — `MA0018`,
  `MA0028`, `MA0051`, `MA0089`, `MA0147`, `MA0148` and `MA0149` — are all configured to `none` in
  this package, so those changes are invisible to consumers.
- Bumped Microsoft.CodeAnalysis.NetAnalyzers from 10.0.400 to 10.0.401. No new or removed rule IDs.
- Bumped SonarAnalyzer.CSharp from 10.33.0.1635 to 10.34.0.3385. No new or removed rule IDs, but
  five rules — `S1264`, `S2692`, `S3249`, `S3885` and `S6670` — became disabled-by-default
  upstream. This package sets each of them to `warning` explicitly, so **nothing changes for
  consumers**; only the regenerated metadata comments in
  `Analyzer.SonarAnalyzer.CSharp.editorconfig` moved.

## [v0.0.13]

### Added

- `MA0221` (`TryGetValue` method should use `[MaybeNullWhen(false)]` on the value parameter) is now
  enforced as a **warning**. It fires only on the `IDictionary<TKey, TValue>.TryGetValue`
  implementation of a type, and only when the `out` parameter is nullable-annotated (`out TValue?`)
  but carries no `[MaybeNullWhen(false)]`. Without the attribute a caller in a nullable context is
  not told that the value is only meaningful when the method returned `true`, so the compiler
  cannot warn about dereferencing it on the `false` path.
- `MA0222` (`JsonSourceGenerationOptions` should set `RespectNullableAnnotations`) and `MA0223`
  (`JsonSourceGenerationOptions` should set `RespectRequiredConstructorParameters`) are now enforced
  as **warnings**. Both fire on any `JsonSerializerContext`-derived type whose
  `[JsonSourceGenerationOptions]` attribute does not explicitly set the property — including a type
  with no such attribute at all. Setting either property to `true` or `false` satisfies the rule; a
  single `[JsonSourceGenerationOptions(JsonSerializerDefaults.Strict)]` satisfies both, since
  `Strict` configures each of them.
- `MA0224` (`JsonSerializerOptions` should set `RespectNullableAnnotations`) and `MA0225`
  (`JsonSerializerOptions` should set `RespectRequiredConstructorParameters`) are now enforced as
  **warnings**. These are the runtime counterparts of the two rules above, firing on a
  `new JsonSerializerOptions(...)` expression that never sets the property — whether in the object
  initializer or by later assignment. Both `System.Text.Json` options default to `false` for
  backwards compatibility, which means deserialization silently tolerates a `null` for a
  non-nullable reference type and a missing `required` constructor parameter; the rules force that
  choice to be made explicitly. `JsonSerializerDefaults.Strict` again satisfies both, and the copy
  constructor (`new JsonSerializerOptions(other)`) is exempt.

All five rules are disabled by default upstream; this package turns each of them on at `warning`.

### Changed

- Bumped Meziantou.Analyzer from 3.0.200 to 3.0.228. The five rules above are the only new rule
  IDs; no already-enforced rule changed behaviour in this project's test suite.

### Removed

- `MA0165` (make interpolated string) no longer exists in Meziantou.Analyzer as of this version. It
  was already configured at `severity = none` in this package, so nothing changes for consumers;
  the now-inert entry is left in `Analyzer.Meziantou.Analyzer.editorconfig` alongside the other
  long-standing stale entries.

## [v0.0.12]

### Added

- `MA0220` (the configured regular expression is not valid) is now enforced as a **warning**. It
  does not analyse source code — it validates `.editorconfig` option values, reporting when one of
  the regex-valued options (`MA0003.excluded_methods_regex`, `MA0104.namespaces_regex`, and the
  legacy misspelled `MA0104.namepaces_regex`) cannot be compiled to a `Regex`. Without it a typo in
  one of those options silently changes the owning rule's behaviour. Note that it validates the
  value regardless of whether the owning rule is itself enabled.
- `MA0218` (the language attribute is empty) is now enforced as a **warning**. It fires when the
  `language`/`lang` attribute of a `<c>` or `<code>` element in an XML comment is present but has
  no value, e.g. `/// <summary>Sample <c language="">{ "value": 1 }</c>.</summary>` — an
  unambiguous typo rather than a missing optional convention.
- `MA0219` (set the language attribute in XML comment) is enforced as a **suggestion**, not a
  warning, so it surfaces as a note without failing a build. It fires on *every* `<c>`/`<code>`
  element that has no `language` attribute, so at `warning` it would break the build of
  essentially any consumer that documents its API. The attribute is only a convention — it is not
  part of the XML documentation comment specification, tools may ignore it, and upstream ships the
  rule at `hidden` severity for exactly that reason. This mirrors the existing `MA0214`/`MA0215`/
  `MA0217` decisions, and the rationale is recorded in the header of
  `Analyzer.Meziantou.Analyzer.editorconfig`. Note the rule does not fire when the element contains
  a single C# keyword — `MA0154` already covers that case with `<see langword="..." />`.

### Changed

- Bumped Meziantou.Analyzer from 3.0.177 to 3.0.200. The three rules above are the only new rule
  IDs; no already-enforced rule changed behaviour in this project's test suite.

## [v0.0.11]

### Changed 

- Set `end_of_line` to `crlf` to ensure consistent line endings across different platforms
- `S3242` reduce severity level from warning to suggestion due to false positives as outlined [here](https://github.com/SonarSource/sonar-dotnet/issues/1036)

## [v0.0.10]

### Added

- `MA0213` (simplify negated boolean expression) is now enforced as a **warning**. It fires on a
  negated `&&`/`||` expression where at least one operand is itself negated, e.g.
  `!(!first && second)`.
- `MA0214` (use `await` instead of returning the task), `MA0215` (return the task instead of
  awaiting it) and `MA0217` (use a static lambda) are enforced as **suggestions**, not warnings, so
  they surface as notes without failing a build. Two deliberate reasons:
  - `MA0214` and `MA0215` are exact inverses that fire on the *same* expression. At `warning` they
    form an unsatisfiable pair — returning a task trips `MA0214`, awaiting it trips `MA0215` — which
    would leave no way to make the build pass.
  - `MA0217` fires on every non-capturing lambda, including idiomatic LINQ, so enforcing it as a
    warning would break the build of essentially any consumer that uses LINQ.

  The rationale is also recorded in the header of `Analyzer.Meziantou.Analyzer.editorconfig`.
- `MA0216` (remove unnecessary closed modifier) is configured as a **warning**, but only actually
  enforces for consumers whose SDK ships Roslyn 5.9 or newer. Meziantou.Analyzer multi-targets
  Roslyn, and the analyzer behind `MA0216` exists only in its `roslyn5.9` folder — the compiler
  picks the folder matching its own Roslyn version, so on older SDKs no loaded analyzer defines the
  id and the editorconfig entry is simply ignored. It is configured rather than omitted so it
  starts enforcing automatically as consumers move to newer SDKs.

### Changed

- Bumped Meziantou.Analyzer from 3.0.140 to 3.0.177. **`MA0130` (GetType() should not be used on
  System.Type instances) no longer fires at all in this release** — an upstream regression, not a
  deliberate narrowing. Its analyzer reports only when the `GetType()` receiver resolves to
  something inheriting `System.Type`; 3.0.140 resolved that receiver through the implicit conversion
  to `object`, while 3.0.177 does not, and since calling `object.GetType()` on a `System.Type`
  receiver always goes through that conversion, the receiver now always resolves to `System.Object`.
  Confirmed by bisection (the same test passes on 3.0.140 and fails on 3.0.177 with an identical
  checkout and SDK) and by five distinct source shapes producing no diagnostic. The rule is
  deliberately left configured at `warning` so it resumes working automatically once upstream is
  fixed; consumers should not rely on `MA0130` coverage in the meantime.
- `MA0023` was broadened upstream and is now titled "Use RegexOptions.ExplicitCapture or named
  groups" (previously "Add RegexOptions.ExplicitCapture"), so named groups now satisfy it. This
  package configures `MA0023` as a suggestion, so the change surfaces as a note.
- Bumped Microsoft.CodeAnalysis.NetAnalyzers from 10.0.302 to 10.0.400. No new rules are enforced
  and no enforced rule changed behaviour in this project's test suite.
- Bumped SonarAnalyzer.CSharp from 10.31.0.145097 to 10.33.0.1635. No new rules are enforced and no
  enforced rule changed behaviour in this project's test suite.

## [v0.0.9]

### Changed

- Bumped Meziantou.Analyzer from 3.0.125 to 3.0.140. No new rules are enforced, but `MA0060`
  (the return value of the method should be used) is substantially broadened. It previously only
  flagged an ignored `Stream.Read`/`Stream.ReadAsync` result; it now also flags ignored return
  values from `TextReader`/`BinaryReader` reads, the non-mutating `System.String` methods
  (`Trim`, `Replace`, `Substring`, `ToUpper`, `Split`, …), the immutable collection interfaces,
  any method annotated `[Pure]`, and any `bool`-returning `TryParse*` method that has an
  `out`/`ref` parameter — plus `out` parameters marked `[DoNotIgnore]` that are discarded with
  `out _`. Expect new `MA0060` warnings on code that throws these results away; the `TryParse`
  half can be disabled on its own with
  `dotnet_diagnostic.MA0060.enable_tryparse_pattern = false`. The bump also reduces false
  positives in two other enforced rules, `MA0202` (comment-only branches) and `MA0211` (fields).
- Bumped SonarAnalyzer.CSharp from 10.30.0.144632 to 10.31.0.145097. No new rules are enforced —
  the two rules introduced in this release (`S8733`, `S8718`) ship as SonarQube server-side rules
  with no Roslyn analyzer in the NuGet package, so they cannot fire during a build. The bump does
  fix false positives in three already-enforced rules: `S1144` (no longer raised for types
  registered with `Microsoft.Extensions.DependencyInjection`), `S3267` (no longer raised on Entity
  Framework `IQueryable`s), and `S1244` (no longer raised for a NaN check written as
  `x.Equals(double.NaN)`).

## [v0.0.8]

### Fixed

- Fixed `MSB4019` build failures on case-sensitive filesystems (e.g. `ubuntu-latest` CI runners).
  Every previously published version (`0.0.1`–`0.0.7`) shipped with a casing mismatch between an
  MSBuild `<Import Project="...">` path and the actual shipped file name; NTFS/APFS resolve this
  case-insensitively, so it only ever surfaced on Linux. Consumers building on Windows or macOS are
  unaffected and require no changes. The already-published `0.0.1`–`0.0.7` versions on nuget.org are
  not retroactively fixed or re-published — upgrade to `v0.0.8` to unblock Linux builds.
- Added `scripts/CheckImportPathCasing.cs`, a new pre-flight check wired into `New-ReleaseTag.ps1`
  and CI, that verifies every `.props`/`.targets` `<Import Project="...">` resolves with exact,
  case-sensitive casing — preventing this class of bug from shipping again.

### Added

- Four new enforced rules exposed by the analyzer bump below, all at `warning` severity: `MA0212`
  (use `MemoryMarshal.GetReference` instead of indexing at 0), `S8949` (use the overload accepting
  a `CancellationToken`), `S8969` (null-forgiving operators should not be redundant), and `S8970`
  (null-forgiving operators should not be used when nullable warnings are disabled).
- A new `ubuntu-latest` CI job mirroring the existing `windows-latest` job (full
  restore/build/test/pack), guarding against future cross-platform packaging regressions.

### Changed

- Bumped Meziantou.Analyzer from 3.0.123 to 3.0.125.
- Bumped SonarAnalyzer.CSharp from 10.29.0.143774 to 10.30.0.144632.

## [v0.0.7]

### Changed

- `S1309` (track uses of in-source issue suppressions) downgraded from `warning` to
  `suggestion`. Inline overrides (`#pragma warning disable`, `[SuppressMessage]`,
  `// NOSONAR`) for a specific instance no longer fail the build; they still surface as a
  build-time note so the override stays visible.

## [v0.0.6]

### Added

- One new enforced rule from Meziantou.Analyzer 3.0.123, at `warning` severity: `MA0211`
  (use multi-line syntax for XML summary comments).

### Changed

- Bumped Meziantou.Analyzer from 3.0.121 to 3.0.123.
- Bumped Microsoft.CodeAnalysis.NetAnalyzers from 10.0.301 to 10.0.302.
- Bumped SonarAnalyzer.CSharp from 10.28.0.143324 to 10.29.0.143774. No new rules are enforced,
  but `S6444` (regular expressions should be executed with a timeout) now also flags `Regex`
  constructions that the prior analyzer version did not catch; pass a `TimeSpan` timeout
  argument to satisfy the rule.

## [v0.0.5]

### Changed

- Bumped Meziantou.Analyzer from 3.0.115 to 3.0.121.
- Bumped Microsoft.CodeAnalysis.BannedApiAnalyzers from 4.14.0 to 5.6.0.
- Bumped SonarAnalyzer.CSharp from 10.27.0.140913 to 10.28.0.143324.

### Removed

- `S4792` (configuring loggers is security-sensitive) — deprecated upstream by
  SonarAnalyzer.CSharp; the diagnostic is no longer produced by any bundled analyzer.

## [v0.0.4]

### Added

- Two new enforced rules from Meziantou.Analyzer 3.0.115, both at `warning` severity:
  `MA0209` (use `in` keyword for `in` parameter) and `MA0210` (use `in` keyword to call
  the `in` overload).

### Changed

- Bumped Meziantou.Analyzer from 3.0.114 to 3.0.115.

## [v0.0.3]

### Changed

- Bumped Meziantou.Analyzer from 3.0.109 to 3.0.114. No new rules are enforced, but
  `MA0206` (remove unnecessary braces in type declaration) now also flags empty type
  bodies such as `class Foo { }`; replace them with `class Foo;` to satisfy the rule.


## [v0.0.2]

### Added

- Two new enforced rules from Meziantou.Analyzer 3.0.109, both at `warning` severity:
  `MA0207` (`[FixedAddressValueType]` fields must be static) and `MA0208`
  (`[FixedAddressValueType]` fields must be value types).

### Changed

- Bumped Meziantou.Analyzer from 3.0.108 to 3.0.109.


## [v0.0.1]

### Added

- Initial package release with Roslyn analyzers, editorconfig rules, and MSBuild props/targets.
- Seven configurable `Ban*` opt-out toggles: `BanNonUtcDateApis`, `BanInvariantCultureStringComparisonApis`, `BanEnumTryParseWithoutIgnoreCaseApis`, `BanRoundWithoutMidpointRoundingApis`, `BanUseOfCultureInfoConstructorApis`, `BanUseOfTupleInFavourOfValueTupleApis`, `BanUseOfNewtonsoftJsonApis`.
- Bundled analyzers: Meziantou.Analyzer, Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp (LGPL-3.0), StyleCop.Analyzers.