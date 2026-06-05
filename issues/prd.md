# PRD: Test-Sourced Rule Documentation & Coverage

## Problem Statement

The package enforces a large set of analyzer rules (387 build-enforced rules across five
analyzers, plus 6 editor-tier rules in `Opinionated.editorconfig` — 393 active in total). Two
problems follow from how this is currently managed:

1. **Documentation is sourced from unreliable free-text.** `docs/rule-reference.md` is generated
   by scraping hand-maintained comments out of the `*.editorconfig` files (rule description, help
   link). These comments are free-format, easy to get wrong, and easy to let drift from reality.
   There is no typed, inspectable source of truth for a rule's description.

2. **There is no guarantee that enforced rules are actually tested.** Tests assert behavior with a
   bare string literal (`buildOutput.HasError("CA1000").ShouldBeTrue()`), so nothing links a test
   to "the rule it covers." Today ~76 distinct rule IDs are referenced across 95 tests, while 393
   rules are active. ~317 enforced rules have no test at all, and nobody can tell which.

The maintainer wants the documentation to be derived from something reliable and machine-readable,
and wants an enforced guarantee that every active rule is backed by a test (or is explicitly,
reasoned-ly exempted).

## Solution

Make **tests the source of truth** for rule documentation, via a typed, reflectable
`[RuleDoc]` attribute applied to the test that demonstrates each rule:

- Each active rule is bound to exactly one canonical `[RuleDoc]` attribute that carries the
  genuinely free-text fields — `Description`, optional `HelpLink`, and (for exempt rules) an
  `Untestable` reason. The attribute is placed on the positive test method that proves the rule
  fires, or — for rules that cannot be triggered by the build-based test harness — on a dedicated
  `UntestableRules` class with a written reason.
- The **active set, severity, and group** are derived from the authoritative, machine-readable
  editorconfig **directives** (`dotnet_diagnostic.<id>.severity = …`) and the file each directive
  lives in — not from comments.
- A **coverage unit test** reconciles the two: every active rule must have exactly one `[RuleDoc]`
  (real test or reasoned exemption), and no `[RuleDoc]` may name an inactive rule. It fails CI on
  gaps and orphans.
- A **doc-generation script** references the test project, reflects over the same compiled test
  assembly the coverage test uses (guaranteeing they observe identical metadata), and regenerates
  `docs/rule-reference.md` from `[RuleDoc]` descriptions/help + editorconfig severity/group.
- **Freshness** is guarded not by a test but by regenerating in CI and failing on a dirty working
  tree (`git diff --exit-code`).

The end state is 100% coverage. Because ~300 tests must be backfilled, the work ships incrementally
behind a shrinking `KnownUncovered` allowlist so CI stays green throughout, and the generated doc
stays complete during the transition via a temporary editorconfig-comment fallback.

## User Stories

1. As a maintainer, I want each rule's description to live in a typed C# attribute, so that the
   documentation source is structured and reflectable rather than scraped from free-text comments.
2. As a maintainer, I want every build-enforced rule to be backed by at least one test, so that I
   know the rules I ship actually behave as documented.
3. As a maintainer, I want CI to fail if an active rule has no `[RuleDoc]`, so that coverage gaps
   cannot be merged silently.
4. As a maintainer, I want CI to fail if a `[RuleDoc]` names a rule that is no longer active, so
   that stale/orphaned documentation attributes are caught.
5. As a maintainer, I want each rule documented exactly once, so that two tests can never assert
   conflicting descriptions for the same rule.
6. As a maintainer, I want to add a `[RuleDoc]` to a test by naming the rule it demonstrates, so
   that the link between test and rule is explicit and discoverable.
7. As a maintainer, I want a rule that genuinely cannot be triggered by the build harness to be
   documented with a written reason instead of a test, so that coverage can honestly reach 100%
   without fabricating tests.
8. As a maintainer, I want severity, active-status, and analyzer group to be read from the
   editorconfig directives, so that they reflect what actually configures the build and never drift
   from a hand-typed attribute value.
9. As a maintainer, I want the doc generator and the coverage test to read `[RuleDoc]` data through
   the same compiled assembly, so that the documentation and the coverage gate can never disagree.
10. As a maintainer, I want to regenerate `docs/rule-reference.md` with a single script command, so
    that updating the doc after a rule change is a one-step local action.
11. As a maintainer, I want CI to fail when the committed `docs/rule-reference.md` is out of date,
    so that a stale, lying doc cannot be shipped.
12. As a maintainer, I want the regenerated doc to keep the existing table shape (Rule ID,
    Description, Severity, Help) grouped by analyzer, so that the published reference looks the same
    to consumers.
13. As a maintainer, I want the 6 editor-tier rules from `Opinionated.editorconfig` to appear in a
    clearly-labeled separate section, so that enforced build rules are not conflated with IDE-tier
    rules whose build behavior varies.
14. As a maintainer, I want the editor-tier section to use an honest, neutral label, so that a rule
    that *is* build-enforced (e.g. IDE0049) is not mislabeled "not build-enforced".
15. As a maintainer, I want the backfill of ~300 missing tests to be tracked by a shrinking
    allowlist, so that remaining work is measurable and CI stays green while it proceeds.
16. As a maintainer, I want the `KnownUncovered` allowlist to only shrink, so that coverage can
    never silently regress during the backfill.
17. As a maintainer, I want the public `docs/rule-reference.md` to stay complete throughout the
    backfill, so that consumers never see a temporarily shrunken reference.
18. As a maintainer, I want the editorconfig-comment fallback removed once the allowlist is empty,
    so that the unreliable comment-scraping is fully retired at the end.
19. As a maintainer, I want the existing 76 tested rules tagged with `[RuleDoc]` as part of the
    foundation, so that the migration starts from the coverage we already have.
20. As a maintainer, I want the duplicated generation logic in the two existing scripts collapsed
    into one shared generator, so that there is a single code path for producing the doc.
21. As a maintainer, I want `CheckRuleReferenceFreshness.cs` and its dedicated CI step deleted, so
    that the pipeline is simplified to regenerate-and-diff.
22. As a contributor, I want a clear convention for which test carries the `[RuleDoc]` for a
    multi-test rule (e.g. RS0030), so that I know exactly one canonical test to tag.
23. As a contributor, I want negative and toggle tests to remain untagged, so that "the rule is
    enforced" is only ever claimed by a test that proves the rule fires.
24. As a contributor, I want to add a new rule and immediately be told by a failing test that it
    needs documentation/coverage, so that new rules cannot ship undocumented.
25. As a consumer of the package, I want the rule reference to accurately describe every enforced
    rule, so that I can trust the documentation.
26. As a maintainer, I want each test file to stay under 1000 lines, so that files remain
    navigable and pull request diffs are reviewable.

## Implementation Decisions

### Source of truth and field ownership
- **Tests are the source of truth for rule documentation.** A typed `[RuleDoc]` attribute, applied
  to tests, owns the free-text fields. Editorconfig comments are treated as unreliable and are
  retired as a source (kept only as a temporary transitional fallback — see Migration).
- The `[RuleDoc]` attribute owns only: `RuleId` (required), `Description` (required),
  `HelpLink` (optional), `Untestable` (optional reason). Severity and analyzer **group are NOT on
  the attribute** — they are derived.
- **Active set, severity, and group are derived from the editorconfig directives.** A rule is
  "active" if a `dotnet_diagnostic.<id>.severity = warning|error|suggestion` directive exists.
  `none`/`silent` rules are neither documented nor required to be tested. Severity is read from the
  directive value. Group is the source file the directive lives in.

### The `[RuleDoc]` attribute
- `AttributeUsage` targets **Method and Class**, with `AllowMultiple = true`.
- Constructor takes `ruleId` and `description`; `HelpLink` and `Untestable` are init-only optional
  properties.
- **Exactly one canonical `[RuleDoc]` per RuleId** across the whole test assembly. The coverage
  test enforces uniqueness; this prevents two tests asserting conflicting descriptions.
- **Method-level** `[RuleDoc]` marks the canonical positive test demonstrating the rule fires;
  its `Untestable` must be null. A method may carry more than one `[RuleDoc]` if it canonically
  demonstrates more than one distinct rule (uniqueness is per-RuleId, not per-method).
- For rules with several positive tests (e.g. RS0030's banned-API families), exactly **one** is
  tagged canonical; the rest remain ordinary untagged tests.
- **Negative tests** (assert a rule does not fire) and **toggle tests** (assert a `Ban*` property
  disables a rule) remain **untagged** — coverage means "a test proves enforcement."

### Untestable rules
- Rules that cannot be triggered by the build-based harness get a class-level `[RuleDoc]` on a
  dedicated `UntestableRules` class, each with a non-empty `Untestable` reason. These satisfy
  coverage without a test.

### Active set scope and grouping (393 rules)
- **387 build-enforced rules** come from `config/analyzers/Analyzer.<Name>.editorconfig`, grouped
  by analyzer name (the existing behavior/sections are preserved).
- **6 editor-tier rules** come from `config/Opinionated.editorconfig`
  (`IDE0001, IDE0002, IDE0003, IDE0038, IDE0049, IDE0084`) and render in their **own section**.
  The section uses a neutral, honest header (e.g. "IDE / editor rules") with a note that build
  enforcement varies per rule — because at least one (IDE0049) is build-enforced while others
  (e.g. IDE0003) are explicitly build-disabled.
- Grouping is by **source file**: `analyzers/Analyzer.<X>.editorconfig` → group "<X>";
  `Opinionated.editorconfig` → the editor-tier section. No directives are relocated between files
  (relocation would risk changing the deliberate editorconfig layering / build behavior).
- During implementation, **each of the 6 editor-tier rules is probed** with the existing build
  harness to determine whether it fires at build: those that fire get a real positive test +
  method-level `[RuleDoc]`; those that don't get a testless `UntestableRules` entry reasoned
  "IDE-only; not emitted by build analysis."

### Modules
- **`RuleDocAttribute`** (new, in the test project) — the typed, reflectable documentation marker
  described above. A deliberately tiny, stable contract.
- **`RuleReferenceGenerator`** (new public class, in the test project) — the single deep module
  that holds all logic: parse the editorconfig directives (active set, severity, group, and the
  temporary comment fallback), reflect over the test assembly for `[RuleDoc]` attributes, reconcile
  the two, and render the markdown. Exposes a small surface: produce the document text, and expose
  the active-set / documented-set reconciliation so the coverage test and the script both call it.
- **Doc-generation script** (`scripts/GenerateRuleReference.cs`, rewritten) — a thin entry point
  that adds a `#:project` reference to the test project, calls `RuleReferenceGenerator`, and writes
  `docs/rule-reference.md`. Reflects over the **same compiled assembly** the coverage test uses.
- **Coverage unit test** (new xUnit test in the test project) — calls `RuleReferenceGenerator`'s
  reconciliation to assert: every active rule has exactly one `[RuleDoc]` (or is on the shrinking
  `KnownUncovered` allowlist), method/class invariants hold (method ⇒ no `Untestable`; class ⇒
  reason required), and no `[RuleDoc]` names an inactive rule (orphan check).
- **`UntestableRules`** (new class in the test project) — carries class-level `[RuleDoc]` entries
  for exempt rules.
- **`KnownUncovered` allowlist** (new, in the test project) — seeded with every currently-untested
  active rule; shrinks as backfill proceeds; deleted when empty.

### File organisation convention

When a test file exceeds 1000 lines it is split into logically-grouped files placed in a folder
named after the original file (without `.cs`). Each split file and its class use the
fully-qualified name `<OriginalClass><Group>Should` (e.g. `CodeAnalysisRulesDesignShould`).
Namespace mirrors the folder structure. Tests within each file are sorted by rule ID.

### Removed / changed
- `scripts/CheckRuleReferenceFreshness.cs` is **deleted**; its job becomes "regenerate + diff" in
  CI. The duplicated generation logic between the two old scripts is collapsed into
  `RuleReferenceGenerator`.
- The CI "Check rule reference is up to date" step is replaced by: run
  `scripts/GenerateRuleReference.cs`, then `git diff --exit-code docs/`. This step moves to run
  **after** the test assembly is built (reflection requires the built assembly).

### Mechanism notes
- Documentation output keeps the **existing table shape** (Rule ID | Description | Severity | Help)
  grouped by analyzer, plus the new editor-tier section. **No code examples** are included — this
  keeps the mechanism **reflection-only** (no Roslyn / source parsing).
- The doc generator and coverage test must read `[RuleDoc]` from the **same compiled assembly** so
  they can never diverge.

## Testing Decisions

- **What makes a good test here:** drive a rule through the real public entry point — a built
  sample project — and assert on observable build output (`buildOutput.HasError(...)` /
  `HasWarning(...)`), exactly as the existing suite does. Do not assert on implementation details.
  A positive test compiles code that violates the rule and asserts the diagnostic is produced.
- **Prior art to model:** `CodeAnalysisRulesShould`, `CodingStandardsShould`, and
  `BannedApiAnalyzersShould` (each builds a sample project via `CreateProjectBuilder` /
  `AddFile` / `BuildAndGetOutput` and asserts on the diagnostics). New backfill tests follow this
  exact shape. `HappyPathShould` models the zero-diagnostics baseline.
- **Modules to be tested:**
  - The **coverage reconciliation** is itself a test (the coverage unit test) — it is the primary
    new behavior and must assert: missing-coverage detection, orphan detection, uniqueness,
    method/class `Untestable` invariants, and allowlist handling.
  - Every active build-enforced rule gets a **positive behavioral test** (backfilled), tagged with
    its canonical `[RuleDoc]`.
  - The **editor-tier rules that fire at build** get positive behavioral tests; those that don't
    get reasoned `UntestableRules` entries (no test).
- **Freshness is intentionally NOT a test.** It is enforced by regenerating in CI and failing on a
  dirty tree. Generation logic lives once in `RuleReferenceGenerator`; the script and the coverage
  test both call it, so there is no duplicated generation code to test twice.
- Existing negative and toggle tests are retained as-is (they remain valuable) but are not tagged
  and do not count toward coverage.

## Out of Scope

- **Worked code examples in the documentation.** The doc remains a table; the triggering snippet is
  not emitted. (This kept the mechanism reflection-only; examples could be a later enhancement.)
- **Roslyn / source parsing** of test bodies. All `[RuleDoc]` discovery is via reflection over the
  compiled test assembly.
- **Relocating or reorganizing editorconfig directives** between files. The deliberate layering of
  `Opinionated.editorconfig` vs the `analyzers/` files is preserved; grouping adapts to the files
  as they are.
- **Moving severity/active-status into attributes.** These remain owned by the editorconfig
  directives.
- **Documenting `none`/`silent` rules.** Only `warning`/`error`/`suggestion` rules are in scope.
- **Changing which rules are enforced or their severities.** This PRD is about documentation and
  test coverage, not about the rule set itself.
- **Verifying that a tagged test's assertion genuinely targets its `RuleId`.** Coverage trusts the
  attribute plus the normal test run; it does not parse test bodies to confirm the assertion.

## Further Notes

- **Magnitude:** 393 active rules; ~76 distinct rule IDs tested today across 95 test methods; ~317
  rules to backfill. NetAnalyzers alone is 253 rules and must be sub-batched into right-sized
  issues.
- **Transition completeness decision (adopted recommendation, open to veto on review):** during
  backfill the generator **prefers the `[RuleDoc]` description and falls back to the editorconfig
  comment** for not-yet-tagged rules, so `docs/rule-reference.md` stays complete the whole time.
  The fallback (the last of the unreliable comment parsing) is **removed when the allowlist
  empties**. The considered alternative — omit untagged rules and let the doc grow from 76 → 393 —
  was rejected to avoid a temporarily incomplete public doc.
- **Sequencing:** a single **foundation** issue lands the attribute, `RuleReferenceGenerator`, the
  rewritten script, the coverage test (seeded with the full allowlist), the CI rewire, the deletion
  of `CheckRuleReferenceFreshness.cs`, the tagging of the existing 76 canonical tests, and the
  `UntestableRules` scaffold (including probing/classifying the 6 editor-tier rules). Then
  **batched backfill** issues by analyzer group (Meziantou 56, CodeStyle 74, NetAnalyzers 253
  sub-batched, BannedApiAnalyzers 3, StyleCop 1), each writing positive tests, tagging canonical
  `[RuleDoc]`s, and shrinking the allowlist.
- This PRD is intended to be broken into vertical-slice issues via the `prd-to-issues` workflow and
  ground out via `work-on-next-issue`.
- **File-size cleanup (issues 017–019):** once the backfill issues land, three test files exceed
  1000 lines. Issues 017–019 apply the file-organisation convention to split them:
  `CodeAnalysisRulesShould.cs` → 7 files under `CodeAnalysisRules/`;
  `CodingStandardsShould.cs` → 2 files under `CodingStandards/`;
  `MeziantouAnalyzersShould.cs` → 2 files under `MeziantouAnalyzers/`.
