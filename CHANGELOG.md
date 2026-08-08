# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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