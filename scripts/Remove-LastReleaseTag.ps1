#Requires -Version 7
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$tags = git tag -l 'v*'
if (-not $tags) {
    Write-Error "No v* tags found. Nothing to delete."
    exit 1
}

$highest = $tags |
    Where-Object { $_ -match '^v(\d+)\.(\d+)\.(\d+)$' } |
    Sort-Object {
        $parts = $_ -replace '^v', '' -split '\.'
        [int64]('{0:D9}{1:D9}{2:D9}' -f [int]$parts[0], [int]$parts[1], [int]$parts[2])
    } -Descending |
    Select-Object -First 1

if (-not $highest) {
    Write-Error "No tags matching vMAJOR.MINOR.PATCH found. Nothing to delete."
    exit 1
}

git tag -d $highest
git push origin --delete $highest

Write-Host "Deleted tag $highest locally and from remote."
