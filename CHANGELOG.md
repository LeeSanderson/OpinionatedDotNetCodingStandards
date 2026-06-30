# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]


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