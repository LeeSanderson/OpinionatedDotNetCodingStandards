# PRD: Auto-update analyzer editorconfigs on analyzer version bumps

## Problem Statement

The package bundles four Roslyn analyzer NuGet packages and ships a curated
`.editorconfig` per analyzer under
`packages/.../pkgsrc/config/analyzers/Analyzer.*.editorconfig`. Each file enumerates
every rule the analyzer defines (as a comment block plus a `dotnet_diagnostic.<id>.severity`
line) and carries a deliberately curated severity for each rule.

When an analyzer package is upgraded, new rule IDs appear in the analyzer assembly that
are **not** represented in the editorconfig. Today there is no mechanism to notice them:
the new rules silently default to their analyzer-defined behavior, never get a curated
severity, never get a test, and never appear in the generated rule reference. Maintaining
the editorconfigs by hand against every analyzer release is tedious and error-prone, and
nothing forces a maintainer to make a conscious decision about each newly-introduced rule.

## Solution

Introduce a deterministic regeneration tool that, for the four NuGet-backed analyzer
editorconfigs, reads the rule metadata from the **currently restored** analyzer
assemblies and rewrites each file:

- existing rules keep their curated severity but get refreshed comment metadata,
- brand-new rules are added defaulting to `warning`,
- rules that have disappeared from the analyzer are reported but left in place.

Because a new rule lands at `warning`, it becomes an "active" rule, which makes the
existing coverage test (`RuleDocCoverageShould.AllActiveRulesAreCovered`) fail until a
maintainer either writes a test with a `[RuleDoc]`, deliberately downgrades the rule, or
records it as untestable. The forcing function is the failing test.

Version detection and bumping stay outside the tool: a Renovate configuration bumps the
analyzer versions (in both `Directory.Packages.props` and the `.nuspec`), and a CI
workflow re-runs the tool on the bump PR and commits the regenerated files back, so the
test failure surfaces automatically on the PR.

## User Stories

1. As a package maintainer, I want new analyzer rules to be discovered automatically when
   I upgrade an analyzer package, so that I never silently miss a rule.
2. As a package maintainer, I want each newly-discovered rule to default to `warning`, so
   that the test suite forces me to make a deliberate decision about it.
3. As a package maintainer, I want my existing curated severities to be preserved exactly
   when the editorconfigs are regenerated, so that regeneration never undoes my decisions.
4. As a package maintainer, I want the comment metadata (title, help link, default
   severity, enabled-by-default) for existing rules refreshed from the new assembly, so
   that the files and the generated rule reference stay accurate as analyzers evolve.
5. As a package maintainer, I want rules that have been removed or renamed in a new
   analyzer version to be reported rather than silently deleted, so that I can migrate
   any associated test/`[RuleDoc]` deliberately.
6. As a package maintainer, I want regeneration to be idempotent, so that an "empty diff"
   reliably means "nothing new to triage."
7. As a package maintainer, I want the tool to target exactly the analyzer version pinned
   in the repo, so that the output matches what consumers will actually get.
8. As a package maintainer, I want the StyleCop rules sourced correctly even though the
   referenced `StyleCop.Analyzers` package is a metapackage whose real analyzer DLL lives
   in a transitively-resolved package, so that StyleCop is handled like the others.
9. As a package maintainer, I want the SDK-bundled IDE/CodeStyle editorconfig left out of
   the automation, so that the tool isn't tied to the SDK version and doesn't flood the
   suite with untestable IDE rules.
10. As a package maintainer, I want the regeneration logic to live in a properly testable
    library rather than the packaging project, so that it can be unit-tested and reused.
11. As a package maintainer, I want the existing rule-reference generator to share that
    library, so that both generators live together and the scripts depend on one place.
12. As a developer, I want a thin command-line entry point to run the regeneration
    locally, consistent with the existing `scripts/*.cs` file-based apps.
13. As a developer, I want a `--check` mode that fails without writing, so that I (or CI)
    can detect drift without mutating files.
14. As a maintainer using Renovate, I want analyzer version bumps to update both
    `Directory.Packages.props` and the `.nuspec` atomically, so that the
    nuspec/props consistency check stays green on the bump PR.
15. As a maintainer, I want CI to regenerate the editorconfigs (and the rule reference) on
    a version-bump PR and commit the result back, so that the new rules and the resulting
    test failure appear on the PR without manual steps.
16. As a maintainer, I want CI to skip the commit-back when nothing changed, so that the
    workflow doesn't loop or create noise.
17. As a maintainer, I want a console/CI summary of which rules were added and which are
    stale, so that I can see at a glance what a bump introduced.
18. As a maintainer, I want a one-time normalization of the existing files to the tool's
    canonical format reviewed for safety (no severity values changed), so that subsequent
    runs are clean no-ops.
19. As a maintainer, I want newly-forced `warning` rules that cannot fire in the
    single-project build harness to be resolvable via the existing untestable-rule
    process, so that the forcing function integrates with current conventions.
20. As a maintainer, I want regeneration to read the curated severity, header, and stale
    entries from the existing file, so that a full rewrite preserves everything that
    isn't derivable from the assembly.

## Implementation Decisions

### New tooling library

- A new class library project is added under `src/`, targeting `net10.0` and marked not
  packable. It references `Microsoft.CodeAnalysis.CSharp` so it can load analyzer
  assemblies and read their diagnostic descriptors.
- The existing rule-reference core — the rule-reference generator plus its supporting
  types (the `[RuleDoc]` attribute, the rule-doc entry record, and the reconciliation
  result) — moves from the test project into this library. The test project references
  the library and continues to apply `[RuleDoc]` to its tests (an added `using`); the
  generator reflects over the passed-in test assembly exactly as today.
- The new project is registered in the solution file.

### Metadata extraction (deep module)

- An **analyzer resolver** determines which analyzer DLLs to load by asking MSBuild for
  the fully-resolved `Analyzer` items of a dogfooded project (e.g. via
  `dotnet build … -getItem:Analyzer`, restoring first). Each resolved DLL is attributed to
  a package by the NuGet-cache path segment `/<package-id>/<version>/`. Only DLLs whose
  package id is in an explicit allow-map are kept; everything else (SDK CodeStyle, test
  framework analyzers, etc.) is discarded. This delegates transitive-metapackage
  resolution, Roslyn-version-folder selection, and language-neutral/C# pairing to
  NuGet/MSBuild.
- An explicit map associates each in-scope package id with its editorconfig file:
  the Meziantou analyzer, the banned-API analyzers, the .NET analyzers, and the
  **transitive StyleCop "Unstable" package** (where StyleCop's analyzer DLL actually
  resides) → the four `Analyzer.*.editorconfig` files.
- An **analyzer descriptor extractor** loads the resolved DLLs through Roslyn's
  analyzer-file-reference mechanism, instantiates the diagnostic analyzers, reads their
  supported diagnostics, and deduplicates by rule id, yielding for each rule: id, title,
  help-link, default severity, and enabled-by-default flag.

### Editorconfig merge/emit (deep module)

- A single **editorconfig generator** takes the existing file text and the extracted rule
  set and returns the rewritten file text plus a report of added and stale rule ids. It:
  - preserves the file header verbatim and harvests the curated
    `dotnet_diagnostic.<id>.severity` values and any stale entries from the existing file;
  - re-emits the whole file with rules sorted by id;
  - for an existing rule: rewrites the three comment lines (id+title, help link,
    enabled+default-severity) from the assembly, and keeps the curated severity line
    unchanged; omits the help-link comment line when the help-link is empty;
  - for a new rule (present in the assembly, absent from the file): emits it with
    `severity = warning` regardless of enabled-by-default;
  - for a stale rule (present in the file, absent from the assembly): carries the entry
    through unchanged and includes it in the report;
  - maps default severity to the editorconfig comment vocabulary
    (`Hidden→silent`, `Info→suggestion`, `Warning→warning`, `Error→error`).
- The generator is deterministic and idempotent: regenerating already-canonical content
  produces identical output.

### Scope

- Only the four NuGet-backed analyzer editorconfigs are regenerated. The SDK-bundled
  CodeStyle/IDE editorconfig is explicitly left to manual maintenance.

### Command-line entry points

- A new thin file-based script under `scripts/` runs the regeneration against the
  in-scope files. It supports a default write mode and a `--check` mode that writes
  nothing and exits non-zero if regeneration would change any file. It prints a summary of
  added and stale rules.
- The existing rule-reference script is re-pointed at the new tooling library.

### Automation around the tool

- A Renovate configuration is added. The native NuGet manager handles
  `Directory.Packages.props`; a custom (regex) manager additionally bumps the matching
  `<dependency id="…" version="…"/>` entries in the `.nuspec`, so a single bump PR keeps
  props and nuspec consistent and the existing nuspec/props check passes.
- A CI workflow (the repository has none today, so this is introduced) triggers on PRs
  that touch the package-version files. It restores, runs the editorconfig regeneration in
  write mode and the rule-reference regeneration, commits any changes back to the PR branch
  using a bot identity with a skip-when-unchanged guard, then builds and runs the tests.
  The resulting test failure on uncovered new rules is the intended signal.

### One-time normalization

- On first adoption the generator rewrites the existing files into its canonical format.
  The resulting diff is reviewed to confirm no curated severity values changed; afterwards
  runs are no-ops.

## Testing Decisions

- A good test here drives a module through its public entry point and asserts on the
  observable output, not on internals. The editorconfig generator is a pure
  text-in/text-out function, which is the highest-value thing to test: feed it synthetic
  existing-file text plus a synthetic descriptor set and assert on the rewritten text and
  the added/stale report. No assembly loading or network is needed for these tests.
- The editorconfig generator will be tested for: preserving curated severities, adding new
  rules at `warning` (including enabled-by-default and disabled-by-default), refreshing
  comment metadata for existing rules, omitting the help-link line when empty, the
  default-severity → comment-vocabulary mapping, carrying through and reporting stale
  entries, header preservation, sorting, and idempotency (regenerating canonical content
  yields no change).
- The analyzer resolver and descriptor extractor (which touch MSBuild/the filesystem and
  real assemblies) are exercised primarily through the end-to-end script run and the
  one-time normalization review rather than fine-grained unit tests; their correctness is
  pinned by the idempotency expectation against the currently-pinned versions.
- Prior art in the solution to model the tests on: the existing rule-reference generator
  tests, which feed parsed rule data through the generator and assert on output, and the
  reconciliation/coverage test, which asserts on a computed result object. The new generator
  tests follow the same shape (pure function, synthetic inputs, assert on output object).
- The existing coverage test (`RuleDocCoverageShould`) and rule-reference generator test
  continue to run unchanged against the relocated library and act as the integration-level
  guard that a `warning`-severity new rule forces coverage to fail.

## Out of Scope

- Detecting or deciding which analyzer version is "new" — that is Renovate's job. The tool
  only regenerates against the restored/pinned versions.
- The SDK-bundled CodeStyle/IDE analyzer editorconfig
  (`Analyzer.Microsoft.CodeAnalysis.CSharp.CodeStyle.editorconfig`).
- Adding entirely new analyzer packages (the file→package allow-map and per-file header
  for a brand-new package are a manual, one-time setup).
- Automatically writing tests or `[RuleDoc]` entries for newly-discovered rules — the tool
  deliberately leaves that to a human (forced by the failing test).
- Changing curated severities of existing rules, or auto-deleting stale rules.
- Choosing/lowering severities based on the analyzer's enabled-by-default flag — new rules
  are always `warning`.

## Further Notes

- **Host-Roslyn coupling**: to load an analyzer built against a given Roslyn version, the
  tooling library's `Microsoft.CodeAnalysis.CSharp` reference must be at least that
  version. Keeping that reference reasonably current (also via Renovate) mitigates load
  failures as analyzers target newer Roslyn.
- **Forcing function meets existing conventions**: a new `warning` rule that cannot fire in
  the single-project build harness is resolved using the established untestable-rule
  process (a class-level `[RuleDoc]` with a reason in `UntestableRules`).
- **Worktree caveat**: the scripts locate the repo root by searching for a `.git`
  *directory*, so they should be run from a normal checkout, not a git worktree (consistent
  with the existing scripts).
- **Suggested build order**: (1) create the tooling library and move the rule-reference
  core; (2) add the descriptor extractor + analyzer resolver; (3) add the editorconfig
  generator with unit tests; (4) add the regeneration script and the one-time
  normalization commit; (5) re-point the rule-reference script; (6) add CI; (7) add
  Renovate.
