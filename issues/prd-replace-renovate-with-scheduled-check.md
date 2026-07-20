# PRD: Replace Renovate with a Scheduled Outdated-Package Check

## Problem Statement

`renovate.json` was added directly to `main` on 2026-06-19 to automate weekly analyzer-package
bumps. Since then there have been zero Renovate pull requests and zero Renovate-created issues
(no Dependency Dashboard, no configuration-error issue) across roughly five scheduled Mondays —
strong evidence the Renovate GitHub App has lost repository access, or was never fully
authorized beyond its onboarding PR (#3, "Configure Renovate - autoclosed", which was closed
without being merged).

Even a working Renovate PR would fail CI under the repository's current architecture. The
Azure Pipelines → GitHub Actions migration deleted `analyzer-bump.yml`, which used to
regenerate analyzer editorconfigs and the rule-reference documentation and commit the result
back to the PR branch. The current `ci.yml` is deliberately read-only — it checks that
generated files are fresh and fails the PR on drift, but never regenerates or commits anything
itself. Renovate has no mechanism to produce fresh generated files, so its PRs could never pass
CI and automerge could never fire, even with a fully reconnected app.

A related, already-confirmed symptom: `ci.yml`'s "Check for outdated packages" step
(`dotnet outdated --fail-on-updates`) gates the *whole* solution, not just the five analyzer
packages. It recently blocked a PR unrelated to any analyzer bump because
`Microsoft.NET.Test.Sdk` had gone stale, forcing a manual just-in-time version bump just to
unblock CI. Nothing currently auto-bumps non-analyzer packages, so this can recur at any time.

The repository already has a skill, `/update-nuget-packages`, that does the full job for the
five analyzer packages end-to-end — bump both version files, regenerate editorconfigs and rule
docs, author new-rule tests, update the changelog, open a PR, automerge, tag, and publish to
NuGet.org — far more thoroughly than Renovate ever attempted. The actual gap is not "apply the
bump," it is "notice there's a bump to apply," and "handle the non-analyzer packages the skill
currently treats as out of scope."

## Solution

Retire Renovate entirely and replace its one useful function — noticing that a package is
outdated — with a small, deterministic, easily-inspectable mechanism built entirely out of tools
already proven in this repository:

1. A new scheduled GitHub Actions workflow runs `dotnet outdated` (the same tool `ci.yml`
   already uses) across the whole solution every Monday, matching the old Renovate cadence.
2. When it finds outdated packages, it opens or updates a single persistent GitHub issue listing
   them; when nothing is outdated, it closes that issue. No new secrets, no third-party
   notification service — GitHub's own issue notifications (which email watchers, including the
   repository owner, by default) are the entire notification channel.
3. The workflow never bumps a version, opens a dependency PR, or invokes any skill itself — it
   only detects and reports. A human decides when to act on the notification.
4. `/update-nuget-packages` gains a second, lightweight path for the packages it currently
   excludes (test-only dependencies such as `xunit`, `Shouldly`, `CliWrap`,
   `Microsoft.NET.Test.Sdk`): a simple version edit, build, test, and commit — without the
   PRD/issue/rule-coverage machinery that exists for analyzer rule changes.
5. `renovate.json` is deleted. Uninstalling the Renovate GitHub App itself is a manual,
   maintainer-owned follow-up outside this PRD's scope (it's a GitHub account setting, not repo
   content).

## User Stories

1. As a maintainer, I want a scheduled weekly check for outdated NuGet packages, so that I don't
   have to remember to run `dotnet outdated` manually or depend on a third-party bot that can
   silently stop working.
2. As a maintainer, I want the check to cover every package in the solution, analyzer and
   non-analyzer alike, so that a stale test-only dependency can never again silently block an
   unrelated PR the way `Microsoft.NET.Test.Sdk` just did.
3. As a maintainer, I want the check to reuse the exact `dotnet-outdated` tool already used in
   `ci.yml`, so that the scheduled check's notion of "outdated" always agrees with the PR gate's.
4. As a maintainer, I want the check to run on a schedule independent of pull-request activity,
   so that outdated packages are surfaced even during weeks with no open PRs.
5. As a maintainer, I want to trigger the check manually from the Actions tab, so that I can
   verify it works — or get an immediate answer — without waiting for the next Monday.
6. As a maintainer, I want the scheduled workflow to declare only the repository permissions it
   actually needs (read code, write issues), so that it can never push code or touch anything
   else even if compromised or misconfigured.
7. As a maintainer, I want outdated packages reported in a single persistent GitHub issue, so
   that I have one place to check rather than a flood of duplicate notifications.
8. As a maintainer, I want that issue updated in place on every run rather than duplicated, so
   that repeated detections of the same stale package don't create noise in the issue tracker.
9. As a maintainer, I want the issue automatically closed once every package is current again,
   so that I never have to remember to clean it up myself.
10. As a maintainer, I want the issue body to list each outdated package with its current and
    latest available version, so that I know exactly what needs bumping without re-running the
    check myself.
11. As a maintainer, I want the issue to distinguish the five analyzer packages (actionable via
    the existing `/update-nuget-packages` pipeline) from everything else (actionable via its new
    lightweight path), so that I immediately know which remediation applies to each entry.
12. As a maintainer, I want to rely on GitHub's own issue notifications rather than a new
    email/SMTP integration, so that no new secrets or third-party services need to be created or
    maintained.
13. As a maintainer, I want `renovate.json` removed from the repository, so that the repo
    doesn't carry dead configuration that misleads a future reader (human or agent) into
    believing Renovate is active.
14. As a maintainer, I want a clear, documented reminder to uninstall the Renovate GitHub App
    from the repository's integration settings, so that an abandoned integration doesn't retain
    repository access indefinitely.
15. As a future contributor or agent, I want no residual documentation implying Renovate
    automerges dependency bumps, so that nobody wastes time debugging a mechanism that no longer
    exists.
16. As a maintainer, I want a lightweight bump path for simple, non-analyzer packages, so that
    fixing a stale test-only dependency doesn't require going through the full PRD/issues/
    rule-coverage pipeline meant for analyzer rule changes.
17. As a maintainer, I want that lightweight path to just edit the version, build, and run the
    test suite before committing, so that trivial dependency bumps stay trivial.
18. As a maintainer, I want the lightweight path to still refuse to commit on a failing build or
    test run, so that a quick bump can never regress the solution.
19. As a maintainer invoking `/update-nuget-packages`, I want it to state clearly which path it
    took (the full analyzer pipeline or the simple bump), so that I understand what happened
    without reading the whole transcript.
20. As a maintainer, I want the lightweight path to follow the same branch-naming and
    commit-discipline rules as the rest of the skill ecosystem, so that it needs no special-case
    handling in `AGENTS.md`.
21. As a maintainer, I want the existing `dotnet outdated --fail-on-updates` gate in `ci.yml`
    left untouched, so that PRs still cannot merge with a stale dependency — the new workflow
    only adds earlier, proactive warning, it doesn't replace that gate.
22. As a maintainer, I want this entire change delivered through the repository's normal
    PRD → issues → `/implementation` pipeline on its own feature branch, so that it lands with
    the same review discipline as every other change here.
23. As a maintainer, I want to manually verify (via a dry run of the scheduled workflow) that the
    tracking issue is correctly created, updated, and closed before I rely on it day to day, so
    that I trust the mechanism actually works — unlike Renovate, which failed silently for over
    a month before anyone noticed.

## Implementation Decisions

- **New scheduled workflow: `.github/workflows/dependency-check.yml`.**
  - Triggers: `schedule` (weekly, targeting "before 9am UTC on Monday" — the same cadence the
    old `renovate.json` used) plus `workflow_dispatch`, so the check can be run on demand for
    verification.
  - Permissions: `contents: read` and `issues: write` only — no ability to push code or modify
    anything beyond issues.
  - Steps: checkout, set up the .NET SDK from `global.json`, restore, install the
    `dotnet-outdated-tool` global tool (same install pattern already used in `ci.yml`), then run
    `dotnet outdated` across `Opinionated.DotNet.CodingStandards.slnx` — **without**
    `--fail-on-updates`, since this workflow's job should succeed and report, not fail — using
    the tool's structured (JSON) output so the outdated packages (name, current version, latest
    version) can be parsed programmatically rather than scraped from console text.
  - Issue management: look for an open issue with one exact, fixed title (e.g. "Outdated NuGet
    packages detected"), optionally labeled for discoverability. If outdated packages are found:
    create that issue if none is open, or update its body in place if one already is — never
    open a second one. If none are found and the issue is currently open, close it with a short
    explanatory comment. The issue body separates the five analyzer packages this repository
    already owns a bump pipeline for from every other outdated package.

- **`renovate.json` deleted outright.** No replacement configuration file is added; the
  Renovate-specific `nuget`/`regex` manager and package-grouping configuration are no longer
  needed anywhere in the repository.

- **`/update-nuget-packages` gains a second, lightweight bump path** for any outdated package
  that is not one of the five analyzer packages it already fully owns:
  - Update the single version reference for that package (wherever it is declared — either
    `Directory.Packages.props` or a project file), run a full build and test pass, and commit
    directly on an appropriately-named feature branch — no PRD, no per-rule issues, no
    editorconfig regeneration, and no changelog entry, since this path never touches anything
    that affects the published package's enforced rules.
  - This path still follows the existing branch-discipline rules in `AGENTS.md` (never commit to
    `main`), but does not hand off to `/implementation`, since there is no PRD/issue queue behind
    it — it is a single, immediate, self-contained commit.
  - The skill's guidance is extended to state explicitly that the lightweight path is only for
    packages that don't add analyzer rules or otherwise change the enforced rule set; anything
    touching one of the five owned analyzer packages always goes through the existing full
    pipeline, never the shortcut.

- **No change to `ci.yml`'s existing `dotnet outdated --fail-on-updates` gate.** It continues to
  fail PRs on any outdated package exactly as today. The new scheduled workflow is purely
  additive — proactive, informational warning ahead of that gate ever being hit by an unrelated
  PR.

- **Manual follow-up, outside repository content:** once this PRD's PR is merged, the maintainer
  uninstalls/deauthorizes the Renovate GitHub App via `github.com/settings/installations`. This
  is a GitHub account action, not something an issue in this PRD implements.

## Testing Decisions

This change is CI/CD configuration and skill-prompt (Markdown) content, not library code, so it
is not covered by the xUnit integration suite — the same precedent set by the
`prd-migrate-to-github-actions` and `prd-renovate-branch-protection` PRDs. Validation is by
observing real behavior:

- **Scheduled workflow, packages outdated.** Trigger `dependency-check.yml` manually via
  `workflow_dispatch` while at least one package is genuinely outdated; confirm it opens (or
  updates) the tracking issue with an accurate, correctly-categorized list.
- **Scheduled workflow, re-run with the same stale package.** Trigger it again without changing
  anything; confirm the existing issue is updated in place rather than a duplicate being
  created.
- **Scheduled workflow, resolved.** After bumping the outdated package(s), trigger it again and
  confirm the tracking issue is closed automatically.
- **Permission sufficiency.** Confirm the job succeeds end-to-end with only `contents: read` and
  `issues: write` — no broader permission is required.
- **`/update-nuget-packages` lightweight path.** Manually invoke the skill against a real
  outdated non-analyzer package (re-check with `dotnet outdated` first, since
  `Microsoft.NET.Test.Sdk` was already bumped) and confirm it takes the lightweight path: a
  single commit, no PRD or issues created, build and tests green.
- **`/update-nuget-packages` full pipeline, unaffected.** Confirm invoking the skill against one
  of the five analyzer packages still takes the existing full pipeline unchanged.
- **No regression to the existing suite.** The xUnit integration suite (including
  `RuleDocCoverageShould`) remains green, since no analyzer or editorconfig behavior changes.

A good test here asserts on observed, external behavior — the issue's actual state, the commit
that lands, the job's pass/fail — never on internal workflow step wiring.

## Out of Scope

- Reconnecting or otherwise fixing the Renovate GitHub App itself.
- Any autonomous or unattended triggering of package bumps from the scheduled workflow — it only
  detects and reports; a human always decides when to act.
- Email, Slack, or any other third-party notification channel — GitHub's native issue
  notifications are the sole channel.
- Changing the existing `dotnet outdated --fail-on-updates` gate in `ci.yml`.
- Uninstalling the Renovate GitHub App itself (a manual GitHub UI action for the maintainer,
  documented here but not automated by any issue).
- A `CHANGELOG.md` entry for this work — it changes repository tooling only, not the published
  package's enforced rules or behavior.
- Handling packages `dotnet outdated` cannot resolve, or version-tracking strategies beyond what
  the tool already supports (e.g. major-version pinning policies).

## Further Notes

- Precedent: `prd-migrate-to-github-actions` established the "check, never fix" read-only CI
  pattern (`contents: read`, no commit-back) that this new workflow follows for its own
  least-privilege permissions.
- This PRD supersedes the automerge intent of the original `prd-renovate-branch-protection` PRD;
  branch protection and feature-branch discipline from that PRD are unaffected and remain in
  place.
- GitHub Actions `schedule` triggers can run somewhat later than the specified time under
  platform load — acceptable here since this is a non-blocking, informational check, not a gate.
- Branch for this work: `feat/prd-replace-renovate-with-scheduled-check`.
