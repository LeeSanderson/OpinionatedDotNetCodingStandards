#Requires -Version 7
[CmdletBinding()]
param(
    [switch] $Major,
    [switch] $Minor,
    [switch] $Patch,
    [switch] $Force
)

$ErrorActionPreference = 'Stop'

$selected = @($Major, $Minor, $Patch) | Where-Object { $_ }
if ($selected.Count -eq 0) {
    Write-Error "Specify exactly one of -Major, -Minor, or -Patch."
    exit 1
}
if ($selected.Count -gt 1) {
    Write-Error "-Major, -Minor, and -Patch are mutually exclusive. Specify exactly one."
    exit 1
}

$branch = git rev-parse --abbrev-ref HEAD
if ($branch -ne 'main') {
    Write-Error "Must be on 'main' to create a release tag. Current branch: $branch"
    exit 1
}

Write-Host "Pulling latest changes..."
git pull
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Checking NuGet dependency sync..."
dotnet ./scripts/CheckNugetDependenciesMatchProps.cs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Checking rule reference..."
dotnet ./scripts/GenerateRuleReference.cs --check
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$tags = git tag -l 'v*'
$highest = $tags |
    Where-Object { $_ -match '^v(\d+)\.(\d+)\.(\d+)$' } |
    Sort-Object {
        $parts = $_ -replace '^v', '' -split '\.'
        [int64]('{0:D9}{1:D9}{2:D9}' -f [int]$parts[0], [int]$parts[1], [int]$parts[2])
    } -Descending |
    Select-Object -First 1

if (-not $highest) {
    $highest = 'v0.0.0'
}

$parts = $highest -replace '^v', '' -split '\.'
$major = [int]$parts[0]
$minor = [int]$parts[1]
$patch = [int]$parts[2]

if ($Major) {
    $major++; $minor = 0; $patch = 0
} elseif ($Minor) {
    $minor++; $patch = 0
} else {
    $patch++
}

$newTag = "v$major.$minor.$patch"
Write-Host "New tag: $newTag (previous: $highest)"

if (-not $Force) {
    $changelog = Get-Content -Path 'CHANGELOG.md' -Raw -ErrorAction SilentlyContinue
    if (-not $changelog) {
        Write-Error "CHANGELOG.md not found. Use -Force to skip the changelog check."
        exit 1
    }
    $version = "$major.$minor.$patch"
    if ($changelog -notmatch "(?im)^##\s*\[$version\]") {
        Write-Error "CHANGELOG.md has no heading matching '## [$version]'. Add a changelog entry or use -Force to skip."
        exit 1
    }
}

Write-Host "Running full test suite..."
dotnet test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git tag $newTag
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git push origin $newTag
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Released $newTag"
