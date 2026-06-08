## Parent PRD

`issues/prd.md`

## What to build

Implement the editorconfig merge generator as a pure, deeply-testable module in the tooling
library, with unit tests. See the PRD "Editorconfig merge/emit (deep module)" implementation
decision and the "Testing Decisions" section.

End-to-end behavior: a single public function takes the existing editorconfig file text plus
an extracted rule descriptor set and returns the rewritten file text plus a report of
added and stale rule ids. It is a pure text-in/text-out function — no assembly loading, no
filesystem, no network — so it can be exercised entirely with synthetic inputs.

Merge semantics to implement (per the PRD):
- preserve the file header verbatim and harvest curated
  `dotnet_diagnostic.<id>.severity` values + stale entries from the existing text;
- re-emit the whole file sorted by id;
- existing rule → refresh the three comment lines from the descriptor, keep the curated
  severity line; omit the help-link comment line when the help-link is empty;
- new rule → `severity = warning` regardless of enabled-by-default;
- stale rule (in file, not in descriptor set) → carry through unchanged and include in the
  report;
- map default severity to comment vocabulary (`Hidden→silent`, `Info→suggestion`,
  `Warning→warning`, `Error→error`);
- deterministic and idempotent (canonical input → identical output).

## Acceptance criteria

- [ ] A public generator function returns `(rewritten text, added ids, stale ids)` from
      `(existing text, descriptor set)`.
- [ ] Unit tests in the test project assert observable output for: curated-severity
      preservation, new-rule→`warning` (both enabled- and disabled-by-default), comment
      refresh on title/help-link/default-severity change, help-link line omitted when empty,
      the severity-word mapping, stale carry-through + report, header preservation, sorting,
      and idempotency (regenerating canonical content yields no change and an empty report).
- [ ] Tests are driven through the public function only (no assertions on internals), modeled
      on the existing rule-reference/reconciliation tests.

## Blocked by

- Blocked by `issues/001-extract-rule-reference-core-into-tooling-library.md`

## User stories addressed

- User story 2
- User story 3
- User story 4
- User story 5
- User story 6
- User story 20
