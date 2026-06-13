# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Initial package release with Roslyn analyzers, editorconfig rules, and MSBuild props/targets.
- Seven configurable `Ban*` opt-out toggles: `BanNonUtcDateApis`, `BanInvariantCultureStringComparisonApis`, `BanEnumTryParseWithoutIgnoreCaseApis`, `BanRoundWithoutMidpointRoundingApis`, `BanUseOfCultureInfoConstructorApis`, `BanUseOfTupleInFavourOfValueTupleApis`, `BanUseOfNewtonsoftJsonApis`.
- Bundled analyzers: Meziantou.Analyzer, Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp (LGPL-3.0), StyleCop.Analyzers.
