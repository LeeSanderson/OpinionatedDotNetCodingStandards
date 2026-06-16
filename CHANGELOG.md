# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial package release with Roslyn analyzers, editorconfig rules, and MSBuild props/targets.
- Seven configurable `Ban*` opt-out toggles: `BanNonUtcDateApis`, `BanInvariantCultureStringComparisonApis`, `BanEnumTryParseWithoutIgnoreCaseApis`, `BanRoundWithoutMidpointRoundingApis`, `BanUseOfCultureInfoConstructorApis`, `BanUseOfTupleInFavourOfValueTupleApis`, `BanUseOfNewtonsoftJsonApis`.
- Bundled analyzers: Meziantou.Analyzer, Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp (LGPL-3.0), StyleCop.Analyzers.

### Changed

- Updated `Meziantou.Analyzer` from 2.0.286 to 3.0.104: adds 20 new rules (MA0183–MA0202) covering format string placeholders, `InlineArray`, `GeneratedRegex` partial properties, null-forgiving operator, HasFlag, MidpointRounding, pattern merging, XML inheritdoc correctness, and conditional compilation branch detection.
- Updated `Microsoft.CodeAnalysis.NetAnalyzers` from 10.0.102 to 10.0.301: adds CA2266 (file-based program shebang).
- Updated `Microsoft.NET.Test.Sdk` from 18.0.1 to 18.6.0 in test project.
- Updated `Microsoft.CodeAnalysis.CSharp` from 4.14.0 to 5.3.0 in tooling project.
