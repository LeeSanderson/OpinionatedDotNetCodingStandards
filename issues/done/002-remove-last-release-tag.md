## Parent PRD

`issues/prd-release-tagging-scripts.md`

## What to build

Create `scripts/Remove-LastReleaseTag.ps1`. The script finds the highest semver `v*` tag in the local repo, deletes it both locally and from the remote, and prints a confirmation naming the deleted tag. It requires no parameters and prompts for nothing. This is the recovery path when a release pipeline run fails after a tag has been pushed.

Step order (per PRD):
1. Find all local tags matching `v*`, sort by semver descending (numeric component comparison, not lexicographic), take the highest; exit with error if no tags exist.
2. `git tag -d <tag>` to delete locally.
3. `git push origin --delete <tag>` to delete from the remote.
4. Print a confirmation message naming the deleted tag.

## Acceptance criteria

- [ ] `scripts/Remove-LastReleaseTag.ps1` exists alongside the existing scripts in `scripts/`
- [ ] Running the script deletes the highest semver `v*` tag both locally (`git tag`) and remotely (`git ls-remote --tags origin`)
- [ ] The script prints a confirmation message that names the deleted tag
- [ ] The script exits with a clear error message if no `v*` tags exist
- [ ] Semver sorting is numeric (e.g. `v1.10.0` sorts above `v1.9.0`)

## Blocked by

None — can start immediately.

## User stories addressed

- User story 15
- User story 16
