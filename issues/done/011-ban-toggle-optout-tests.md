# Opt-out tests for all seven `Ban*` toggles

## Parent PRD

`issues/prd.md` — see *Testing Decisions → Modules to be tested* (the new opt-out tests) and User story 21.

## What to build

For each of the seven `Ban*` properties, add a test proving that setting it `false`
stops the corresponding banned-API diagnostic (`RS0030`) from firing, while the
banned code still triggers it when the toggle is left at its default. This backs the
opt-outs documented in the packaged README
(`issues/009-packaged-readme-and-prose.md`).

The test suite currently proves only `BanNonUtcDateApis=false`
(`BannedApiAnalyzersShould.NotBanNonUtcDatesWhenPropertyDisabled`). Extend coverage to
the remaining six toggles using the same `CreateProjectBuilder(properties: ...)`
pattern and the existing banned-code samples from `BannedApiAnalyzersShould`:

- `BanInvariantCultureStringComparisonApis`
- `BanEnumTryParseWithoutIgnoreCaseApis`
- `BanRoundWithoutMidpointRoundingApis`
- `BanUseOfCultureInfoConstructorApis`
- `BanUseOfTupleInFavourOfValueTupleApis`
- `BanUseOfNewtonsoftJsonApis`

End-to-end behavior: for every toggle, the banned-API sample produces `RS0030` by
default and produces no `RS0030` when the toggle is set `false`.

## Acceptance criteria

- [x] Each of the seven `Ban*` toggles has a test asserting `RS0030` is absent when the toggle is `false`
- [x] Each test uses the corresponding banned-code sample that fires `RS0030` at the default setting
- [x] All new tests pass (7/7 opt-out tests pass)
- [x] Tests reuse the existing `PackageFixture` / `ProjectBuilder` helpers

## Blocked by

None - can start immediately.

## User stories addressed

- User story 21
