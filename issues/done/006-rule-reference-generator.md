# Rule-reference generator and generated reference document

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Documentation* and *Generator module (new, testable in isolation)*, and User story 16.

## What to build

A generator module that reads the analyzer editorconfig files under
`packages/Opinionated.DotNet.CodingStandards/pkgsrc/config/analyzers/*.editorconfig`
and emits a reference document listing **only the enforced rules** — those with a
real severity (`warning`, `error`, or `suggestion`), excluding rules set to `none`.
The editorconfigs share a uniform machine-readable format: each rule has a comment
block with its id, description, and help link, followed by a
`dotnet_diagnostic.<ID>.severity = <severity>` line.

The output is a deterministic reference document presenting a table of: rule id,
description, severity, and help link. The generator's input is the set of
editorconfig files and its output is the reference content; the same logic will back
both generation and the CI freshness check (see
`issues/007-rule-reference-freshness-check.md`). Commit the generated reference
document to the repository.

The disabled (`none`) rules are deliberately excluded — they remain discoverable in
the editorconfig files themselves.

End-to-end behavior: running the generator produces (or refreshes) the committed
reference document, and the document accurately lists every enforced rule with its
severity and help link.

## Acceptance criteria

- [ ] A generator (runnable in isolation, e.g. a `scripts/` C# file like the dependency-check script) reads the analyzer editorconfigs and writes the reference document
- [ ] The generated document contains a table with columns: rule id, description, severity, help link
- [ ] Only enforced rules (severity warning/error/suggestion) appear; rules set to `none` are excluded
- [ ] Output is deterministic (re-running with unchanged inputs produces byte-identical content)
- [ ] The generated reference document is committed to the repository

## Blocked by

- Blocked by `issues/001-standardize-dotnet-casing.md` (reads config under the renamed `packages/` path)

## User stories addressed

- User story 16
