## Parent PRD

`issues/prd.md`

## What to build

Implement the analyzer resolution + descriptor extraction modules in the tooling library. See
the PRD "Metadata extraction (deep module)" implementation decision.

End-to-end behavior, in two collaborating modules:
- **Analyzer resolver** — asks MSBuild for the fully-resolved `Analyzer` items of a dogfooded
  project (e.g. `dotnet build … -getItem:Analyzer`, restoring first), attributes each resolved
  DLL to a package by the NuGet-cache path segment `/<package-id>/<version>/`, and keeps only
  the packages in an explicit allow-map (the Meziantou analyzer, the banned-API analyzers, the
  .NET analyzers, and the transitive `stylecop.analyzers.unstable` package). Everything else
  (SDK CodeStyle, test-framework analyzers, etc.) is discarded. The allow-map also associates
  each package id with its target `Analyzer.*.editorconfig` file.
- **Descriptor extractor** — loads the resolved DLLs via Roslyn's analyzer-file-reference
  mechanism, instantiates the diagnostic analyzers, reads their supported diagnostics, and
  deduplicates by rule id, yielding for each rule: id, title, help-link, default severity, and
  enabled-by-default flag.

This delegates transitive-metapackage resolution, Roslyn-version-folder selection, and
language-neutral/C# pairing to NuGet/MSBuild rather than re-implementing them.

## Acceptance criteria

- [ ] The resolver returns analyzer DLL paths grouped by the four in-scope packages, correctly
      resolving StyleCop's DLL from the transitive `stylecop.analyzers.unstable` package and
      excluding out-of-scope analyzers (SDK CodeStyle, xunit, etc.).
- [ ] The extractor returns a deduplicated descriptor set per package; a focused test/driver
      confirms the expected id prefixes are present (`MA####`, `CA####`, `RS####`, `SA####`)
      with non-empty titles and the enabled-by-default / default-severity fields populated.
- [ ] Extraction targets the versions currently restored/pinned in `Directory.Packages.props`.
- [ ] The SDK-bundled CodeStyle/IDE analyzers are not included.

## Blocked by

- Blocked by `issues/001-extract-rule-reference-core-into-tooling-library.md`
- (Can proceed in parallel with `issues/002-editorconfig-merge-generator.md`.)

## User stories addressed

- User story 1
- User story 7
- User story 8
- User story 9
