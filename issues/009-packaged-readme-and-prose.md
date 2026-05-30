# Packaged README (consumer-focused) with `Ban*` toggles and style/naming prose

## Parent PRD

`issues/prd.md` — see *Implementation Decisions → Documentation* and User stories 14, 15, and 17.

## What to build

Rewrite the packaged README
(`packages/Opinionated.DotNet.CodingStandards/pkgsrc/README.md`, rendered on
NuGet.org) for consumers so they can adopt the package quickly. The current packaged
README is five lines. Cover:

- How to install the package (it is a development dependency).
- The seven configurable `Ban*` toggles and how to opt out of each
  (`BanNonUtcDateApis`, `BanInvariantCultureStringComparisonApis`,
  `BanEnumTryParseWithoutIgnoreCaseApis`, `BanRoundWithoutMidpointRoundingApis`,
  `BanUseOfCultureInfoConstructorApis`, `BanUseOfTupleInFavourOfValueTupleApis`,
  `BanUseOfNewtonsoftJsonApis`), with the property to set `false` to disable each
  (User story 15).
- A link to the generated rule reference (User story 16's output, from
  `issues/006-rule-reference-generator.md`).
- A prose section describing the style options and naming conventions from the
  top-level code-style editorconfig — the choices that carry no rule-id/severity rows
  and therefore are not in the generated table (User story 17).

End-to-end behavior: a consumer browsing NuGet.org sees install instructions, a
documented set of opt-outs, a link to the full rule reference, and prose for the
style/naming choices.

## Acceptance criteria

- [ ] The packaged README explains how to install the package
- [ ] All seven `Ban*` toggles are documented with the exact property name and how to opt out
- [ ] The packaged README links to the generated rule reference
- [ ] A prose section documents the style options and naming conventions not expressed as rule IDs
- [ ] The README renders correctly (valid markdown) and is the file referenced by the nuspec `readme` element

## Blocked by

- Blocked by `issues/006-rule-reference-generator.md` (links to the generated reference)

## User stories addressed

- User story 14
- User story 15
- User story 17
