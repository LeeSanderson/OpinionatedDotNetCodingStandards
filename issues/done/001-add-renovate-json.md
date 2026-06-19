## Parent PRD

`issues/prd-renovate-branch-protection.md`

## What to build

Add `renovate.json` at the repository root to automate weekly analyzer package bumps. The file must:

- Enable only `nuget` and `regex` managers (prevent Renovate touching Azure Pipelines YAML or other files).
- Use the built-in `nuget` manager to handle `Directory.Packages.props` automatically.
- Add a `regexManagers` entry targeting the `.nuspec`, matching `<dependency id="..." version="..."/>` lines with `datasourceTemplate: "nuget"` and `versioningTemplate: "nuget"`.
- Group all five analyzer packages (Meziantou.Analyzer, Microsoft.CodeAnalysis.BannedApiAnalyzers, Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp, StyleCop.Analyzers) into a single PR via a `packageRules` entry with `groupName: "analyzer packages"`.
- Schedule `"before 9am on Monday"` (weekly cadence, replaces `outdated.yml`).
- Set `automerge: true` and `platformAutomerge: true` to use GitHub's native automerge, triggered only after all required status checks pass.

See the **Implementation Decisions** section of the parent PRD for the full `renovate.json` specification.

## Acceptance criteria

- [ ] `renovate.json` exists at the repository root and is valid JSON.
- [ ] Only the `nuget` and `regex` managers are enabled (no other file types touched).
- [ ] All five analyzer packages are covered — three by the built-in `nuget` manager (`Directory.Packages.props`) and all five by the `regexManagers` entry (`.nuspec`).
- [ ] The five packages are grouped under a single `packageRules` entry so Renovate opens one PR, not five.
- [ ] `schedule`, `automerge`, and `platformAutomerge` are set as specified.
- [ ] Running `dotnet build` continues to pass (no regressions from adding the config file).

## Blocked by

None — can start immediately.

## User stories addressed

- User story 1 (automated weekly bumps)
- User story 2 (atomic `Directory.Packages.props` + `.nuspec` update)
- User story 3 (all five packages in a single PR)
- User story 4 (automerge on green CI)
- User story 5 (release cycle remains tag-driven)
