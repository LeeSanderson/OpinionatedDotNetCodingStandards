# PRD: Make `Opinionated.DotNet.CodingStandards` production-ready

## Problem Statement

`Opinionated.DotNet.CodingStandards` is a NuGet "config" package (a development
dependency, not a code library) that bundles four Roslyn analyzer packages plus a
curated set of `.editorconfig` files and MSBuild `.props`/`.targets`. When installed,
it turns on strict compiler settings and enforces a large set of code-style, quality,
and "banned API" rules.

The core design is sound and the behaviour is well tested, but the project is not yet
fit to be published and consumed by the public:

- It cannot actually be released. CI builds and packs to an artifact directory but
  never pushes to NuGet.org, and the package version is a hardcoded literal, so every
  build would produce the same version.
- A consumer arriving from NuGet.org has almost no documentation: there is no root
  README, the packaged README is five lines, and none of the ~387 enforced rules or
  the seven configurable `Ban*` toggles are documented.
- There are latent correctness issues that survive only because CI runs on Windows:
  a filename-casing mismatch in the pipeline, and an inconsistent `DotNet`/`Dotnet`
  casing across folders, the solution, build props, and the dependency-check script.
- The test suite proves that rules *fire*, but nothing proves the package behaves well
  in the other directions: that clean code stays clean, that the documented opt-outs
  actually work, that the rules flow transitively, or that consumers can override
  severities.

## Solution

Bring the package to a state where it can be published to NuGet.org as a public
open-source package and consumed with confidence:

- Introduce tag-driven versioning and an Azure DevOps release stage so that pushing a
  `v*` git tag builds, tests, and publishes a uniquely-versioned package to NuGet.org,
  and creates a matching GitHub Release.
- Give consumers real documentation: a contributor-focused root README, a
  consumer-focused packaged README, and an auto-generated, always-accurate reference of
  every enforced rule.
- Remove the latent casing bugs and make the repository internally consistent.
- Harden the test suite so it proves the package is safe to adopt: clean code produces
  no errors, every opt-out works, the standards flow transitively, and severities are
  overridable.

CI stays on Azure DevOps and remains Windows-only; the existing `ci.yml` build/test/pack
behaviour is preserved.

## User Stories

1. As a maintainer, I want the package version to be derived from a `v*` git tag, so
   that every release has a unique, traceable version without editing a version literal.
2. As a maintainer, I want untagged and local builds to still produce a valid package
   version, so that ordinary `dotnet build` (which packs, because
   `GeneratePackageOnBuild` is on) does not fail for lack of a version.
3. As a maintainer, I want untagged builds to carry an obvious pre-release version, so
   that an artifact built outside a release tag is clearly not releasable.
4. As a maintainer, I want pushing a `v*` tag to build, test, and then publish to
   NuGet.org, so that releasing is a single deliberate action tied to a commit.
5. As a maintainer, I want the release to publish only after build and tests pass, so
   that a broken package can never reach the public feed.
6. As a maintainer, I want a GitHub Release created automatically on a `v*` tag with the
   changelog notes, so that consumers have a human-readable record of what changed.
7. As a maintainer, I want the existing per-build "outdated packages" gate to remain, so
   that contributions are kept current.
8. As a maintainer, I want a separate scheduled pipeline that reports outdated packages,
   so that I learn about staleness during quiet periods without waiting for the next
   commit.
9. As a maintainer, I want the pipeline's reference to the dependency-check script to
   match the real filename, so that the step is correct regardless of filesystem
   case-sensitivity.
10. As a maintainer, I want a single consistent `DotNet` casing across folders, the
    solution, build props, and scripts, so that there is no latent case-sensitivity bug
    and the repository reads consistently.
11. As a maintainer, I want the unused coverage dependency removed, so that the project
    has no dead dependencies misrepresenting what is measured.
12. As a maintainer, I want a CI step that fails if the generated rule reference is out
    of date, so that documentation cannot silently drift from the editorconfigs.
13. As a contributor, I want a root README that explains what the package is and how to
    build, test, and release it, so that I can contribute without reverse-engineering
    the repo.
14. As a consumer browsing NuGet.org, I want a packaged README that explains how to
    install and configure the package, so that I can adopt it quickly.
15. As a consumer, I want every configurable `Ban*` toggle documented with how to opt
    out, so that I can disable rules that do not fit my project.
16. As a consumer, I want an accurate reference of every enforced rule with its
    severity and help link, so that I understand exactly what the package will flag.
17. As a consumer, I want the style options and naming conventions described in prose,
    so that I understand the choices that are not expressed as rule IDs.
18. As a consumer, I want NuGet search tags on the package, so that I can discover it
    when searching for analyzers, code style, or editorconfig packages.
19. As a consumer, I want release notes linked from the package metadata, so that I can
    see what changed between versions.
20. As a consumer, I want assurance that fully compliant code produces zero diagnostics,
    so that adopting the package will not break my build with false positives.
21. As a consumer, I want each `Ban*` opt-out proven to work, so that I can trust the
    documented configuration.
22. As a consumer, I want the standards to apply when the package is referenced
    transitively, so that I get consistent enforcement through my dependency graph.
23. As a consumer, I want to be able to override or downgrade an individual rule's
    severity, so that the standards are a layerable starting point rather than a locked
    ruleset.
24. As a maintainer, I want the test harness's existing version override to actually
    work, so that the package can be packed under a test version cleanly.

## Implementation Decisions

**Distribution & CI home**
- The package targets public release on NuGet.org as an open-source package.
- CI/CD stays on Azure DevOps. The existing `ci.yml` build/test/pack behaviour is
  preserved, including the per-build outdated-packages gate.
- CI remains Windows-only; no cross-OS matrix is added.

**Versioning & release**
- Versioning is tag-driven. The release pipeline reads the `v*` tag and injects the
  version at pack time.
- The `.nuspec` uses a `version` replacement token instead of a hardcoded literal. This
  also makes the test harness's existing version override meaningful.
- Because the package uses a custom nuspec file, the injected version may need to flow
  through nuspec properties rather than the plain MSBuild version property; the exact
  mechanism is to be confirmed during implementation.
- A default pre-release version (`0.0.0-dev`) is set in the shared build props so that
  untagged and local builds — which pack on every build — produce a valid, obviously
  non-releasable version.
- A new release stage triggers on `v*` tags, runs the existing build and test steps,
  and on success pushes to NuGet.org and creates a GitHub Release populated from the
  changelog.

**Outdated-packages checking**
- The existing per-build outdated gate remains unchanged.
- A new scheduled pipeline runs the outdated check on a recurring schedule and reports
  staleness, decoupled from the build/release path.

**Casing & cleanup**
- The repository standardizes on the `DotNet` casing everywhere: directory names, the
  solution file, the shared build props/targets imports, and the dependency-check
  script's path. This matches the already-correct published package id and the test
  namespaces.
- The pipeline's reference to the dependency-check script is corrected to match the
  real filename.
- The unused coverage collector dependency is removed from the test project, since the
  package emits no production assembly to cover.

**Documentation**
- A root README is written for contributors (what the package is, build/test, repo
  layout, release process).
- The packaged README (rendered on NuGet.org) is written for consumers (install, the
  seven `Ban*` toggles and how to opt out, and a link to the generated rule reference).
- A generated rule-reference document lists enforced rules only (those with a real
  severity — warning, error, or suggestion), as a table of rule id, description,
  severity, and help link. It is generated from the analyzer editorconfigs, which share
  a uniform machine-readable format.
- The top-level code-style editorconfig (style options and naming conventions, which
  carry no rule-id/severity rows) is documented as a separate prose section rather than
  in the generated table.
- A new CI step — modeled on the existing dependency-sync check — regenerates the rule
  reference and fails if it is out of date.
- A changelog is maintained in the Keep a Changelog format. The package metadata's
  release-notes field links to it, and the release stage uses it to populate the GitHub
  Release.
- NuGet search tags are added to the package metadata.
- No CONTRIBUTING file is added in this PRD.

**Generator module (new, testable in isolation)**
- A rule-reference generator reads the analyzer editorconfig files and emits the
  enforced-rule reference document. Its input is the set of editorconfig files and its
  output is the deterministic reference content. The same logic backs both generation
  and the CI freshness check (generate-and-compare).

**Setup prerequisites (provided by the maintainer, outside the code changes)**
- A NuGet.org API key stored as an Azure secret/service connection.
- A GitHub service connection in Azure DevOps for creating GitHub Releases.
- The Azure Pipelines GitHub app wired up so PR checks report on GitHub pull requests.

## Testing Decisions

**What makes a good test here**
- Tests drive the package the way a consumer experiences it: pack the real package,
  build a throwaway project that references it, and assert on the resulting build
  diagnostics (the SARIF output). This is the existing pattern and it tests external
  behaviour, not implementation detail.

**Modules to be tested**
- Existing behaviour is retained: tests that assert specific rules fire.
- New "happy-path" test: fully compliant code produces zero diagnostics, guarding
  against the package becoming accidentally over-aggressive (a false positive would
  break every consumer's build).
- New opt-out tests: for each of the seven `Ban*` properties, setting it false stops the
  corresponding banned-API diagnostic. This backs the README's documented opt-outs.
- New transitive-consumption test: the standards are still enforced when the package is
  referenced transitively rather than directly, validating the multi-folder MSBuild
  wiring (build / buildTransitive / buildMultiTargeting).
- New severity-override test: a consumer can downgrade or disable an individual rule's
  severity (extending the existing warnings-as-errors-disabled test), confirming the
  configuration is layerable rather than locked.
- The rule-reference generator's logic is exercised by the CI freshness check
  (generate and compare against the committed reference).

**Prior art**
- The existing test base, project-builder helper, and package fixture (which packs the
  package once for all tests) are the model for the new tests. New tests reuse the same
  project-builder approach and assert on the same SARIF-based output helpers.

**Notes on scope of testing**
- This package has no production assembly, so code-coverage measurement is not
  meaningful; the coverage dependency is removed rather than wired up.

## Out of Scope

- Migrating CI to GitHub Actions, or running a cross-OS (Linux/macOS) build matrix.
- Relocating or weakening the existing per-build outdated-packages gate.
- Code-coverage collection or thresholds.
- A CONTRIBUTING guide.
- Changing which analyzers are bundled, or adding/removing/retuning individual rules or
  severities (this PRD documents and tests the existing ruleset; it does not redesign
  it).
- Automating the version bump itself (the release is initiated by deliberately creating
  a `v*` tag).
- Any change to the package's runtime/consumer behaviour beyond what is needed to make
  the existing behaviour releasable, documented, and tested.

## Further Notes

- The published package id is already `Opinionated.DotNet.CodingStandards` (correct
  casing), so the casing standardization is internal cleanup; it does not change the
  public id. NuGet locks display casing on first publish, so it is worth being
  consistent before the first release.
- The enforced ruleset is larger than it first appears: roughly 387 rules are actively
  enforced and roughly 386 are explicitly disabled (`none`) across the analyzer
  editorconfigs. The generated reference deliberately shows only the enforced set; the
  disabled rules remain discoverable in the editorconfig files themselves.
- Because `GeneratePackageOnBuild` is enabled on the source project, every build packs
  the package — this is why a valid fallback version is required even for local builds.
- Suggested implementation order: casing standardization first (it is the riskiest
  churn and later work builds on the renamed paths), then versioning/release, then
  documentation and the generator, then the test additions.
