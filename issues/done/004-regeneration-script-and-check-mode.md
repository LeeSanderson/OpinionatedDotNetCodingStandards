## Parent PRD

`issues/prd.md`

## What to build

Add the thin command-line entry point that ties the resolver, extractor, and generator
together over the four in-scope editorconfig files. See the PRD "Command-line entry points"
implementation decision.

End-to-end behavior: a new `scripts/UpdateAnalyzerEditorConfigs.cs` file-based app (referencing
the tooling library, consistent with the existing `scripts/*.cs`) resolves the analyzer DLLs,
extracts descriptors, and runs the merge generator for each of the four files. It supports a
default **write** mode and a `--check` mode that writes nothing and exits non-zero if
regeneration would change any file. It prints a per-file summary of added and stale rules.

Scope guard for this slice: it lands the script and its mechanics but **does not commit any
regenerated editorconfig changes** — normalizing the committed files (and reviewing that diff)
is the separate HITL slice 005. Because the files are not yet normalized, `--check` is expected
to report drift here; that correct non-zero exit is the verification that the pipeline works.

## Acceptance criteria

- [ ] `scripts/UpdateAnalyzerEditorConfigs.cs` exists as a thin app referencing the tooling
      library and runs to completion against the restored assemblies.
- [ ] Default mode would write the four files; `--check` writes nothing and exits non-zero when
      regeneration would change a file, zero when it would not.
- [ ] The script prints a per-file summary of added and stale rule ids.
- [ ] No regenerated editorconfig changes are committed in this slice; existing tests stay
      green and the committed files are untouched.

## Blocked by

- Blocked by `issues/002-editorconfig-merge-generator.md`
- Blocked by `issues/003-analyzer-resolution-and-descriptor-extraction.md`

## User stories addressed

- User story 12
- User story 13
- User story 17
