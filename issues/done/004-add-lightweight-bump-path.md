## Parent PRD

`issues/prd-replace-renovate-with-scheduled-check.md`

## What to build

Extend the `/update-nuget-packages` skill (`.claude/skills/update-nuget-packages/SKILL.md`) with
a second, lightweight bump path for any outdated package that is **not** one of the five analyzer
packages the skill already fully owns (`Meziantou.Analyzer`,
`Microsoft.CodeAnalysis.BannedApiAnalyzers`, `Microsoft.CodeAnalysis.NetAnalyzers`,
`SonarAnalyzer.CSharp`, `StyleCop.Analyzers`) — e.g. `xunit`, `Shouldly`, `CliWrap`,
`Microsoft.NET.Test.Sdk`. See the PRD's Implementation Decisions section, "`/update-nuget-packages`
gains a second, lightweight bump path".

The lightweight path: update the single version reference for that package (wherever it's
declared — `Directory.Packages.props` or a project file), run a full build and test pass, and
commit directly on an appropriately-named feature branch per `AGENTS.md`'s existing branch
discipline (never commit to `main`) — with no PRD, no per-rule issues, no editorconfig
regeneration, and no changelog entry, since this path never touches anything that affects the
published package's enforced rules. It does not hand off to `/implementation` — there is no
PRD/issue queue behind a single version bump.

The skill must state clearly, in its own output, which path it took (the full analyzer pipeline
vs. this new lightweight path), and its guidance must say explicitly that any bump touching one
of the five owned analyzer packages always uses the existing full pipeline, never this shortcut.

## Acceptance criteria

- [ ] `SKILL.md` documents a lightweight path that triggers when the outdated package is outside
      the five owned analyzer packages
- [ ] The lightweight path's steps are: update the version reference, run a full build and test
      pass, and commit on a properly-named feature branch — no PRD, no per-rule issues, no
      editorconfig regeneration, no changelog entry
- [ ] The skill's existing "Rules" section explicitly states that analyzer-package bumps always
      use the full existing pipeline, never the lightweight shortcut
- [ ] The skill's final report states which path was taken
- [ ] Manually verified via a real invocation against a genuinely outdated non-analyzer package
      (per the PRD's Testing Decisions): a single commit lands, no PRD/issues are created, and
      the build/tests are green
- [ ] Manually verified that invoking the skill against one of the five analyzer packages still
      takes the existing full pipeline, unaffected by this change

## Blocked by

None - can start immediately

## User stories addressed

- User story 16
- User story 17
- User story 18
- User story 19
- User story 20
