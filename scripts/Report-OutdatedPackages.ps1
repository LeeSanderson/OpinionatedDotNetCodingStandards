#Requires -Version 7
<#
.SYNOPSIS
    Parses a `dotnet outdated` JSON report and creates or updates a single, persistent
    GitHub tracking issue listing every outdated package.

.DESCRIPTION
    Used by .github/workflows/dependency-check.yml. `dotnet outdated` only writes the JSON
    report file when at least one dependency is outdated; a missing report means nothing is
    outdated. When nothing is outdated and the tracking issue is currently open, this script
    closes it with a short explanatory comment; when the tracking issue is already closed (or
    was never created), this is a no-op -- it never reopens or recreates anything.

    The issue is looked up by an exact, fixed title so repeated runs update (or close) the same
    issue in place instead of creating duplicates. Requires the `gh` CLI to be authenticated
    (e.g. via the GH_TOKEN environment variable) and to be run from within the target
    repository.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $ReportPath,

    [string] $Title = 'Outdated NuGet packages detected'
)

$ErrorActionPreference = 'Stop'

# The five analyzer packages this repository already owns a full bump pipeline for
# (see the /update-nuget-packages skill). Everything else uses the lightweight bump path.
$analyzerPackages = @(
    'Meziantou.Analyzer',
    'Microsoft.CodeAnalysis.BannedApiAnalyzers',
    'Microsoft.CodeAnalysis.NetAnalyzers',
    'SonarAnalyzer.CSharp',
    'StyleCop.Analyzers'
)

function Format-PackageTable {
    param([array] $Rows)

    if ($Rows.Count -eq 0) {
        return '_None._'
    }

    $lines = @('| Package | Current | Latest |', '| --- | --- | --- |')
    foreach ($row in $Rows) {
        $lines += "| $($row.Name) | $($row.Current) | $($row.Latest) |"
    }

    return ($lines -join "`n")
}

# Looks up the single, persistent tracking issue by its exact, fixed title. Shared by both the
# "packages found" (create/update) and "nothing outdated" (close) branches below so there is
# only one place that knows how the issue is identified.
function Find-OpenTrackingIssue {
    param([string] $Title)

    return gh issue list --state open --json number,title --limit 100 |
        ConvertFrom-Json |
        Where-Object { $_.title -eq $Title } |
        Select-Object -First 1
}

# Flatten every project/target-framework's dependency list into one de-duplicated set keyed
# by package name. A package is only "outdated" when its resolved version genuinely differs
# from the latest available version -- ignore the tool's UpgradeSeverity field, which can
# report a non-"None" severity for pre-release packages even when the two versions match.
$outdated = @{}
if (Test-Path -Path $ReportPath) {
    $report = Get-Content -Path $ReportPath -Raw | ConvertFrom-Json
    foreach ($project in $report.Projects) {
        foreach ($targetFramework in $project.TargetFrameworks) {
            foreach ($dependency in $targetFramework.Dependencies) {
                if ($dependency.ResolvedVersion -ne $dependency.LatestVersion) {
                    $outdated[$dependency.Name] = [PSCustomObject]@{
                        Name    = $dependency.Name
                        Current = $dependency.ResolvedVersion
                        Latest  = $dependency.LatestVersion
                    }
                }
            }
        }
    }
}

$outdatedPackages = @($outdated.Values | Sort-Object -Property Name)

if ($outdatedPackages.Count -eq 0) {
    Write-Host 'No outdated packages detected.'

    $existingIssue = Find-OpenTrackingIssue -Title $Title
    if (-not $existingIssue) {
        Write-Host 'No open tracking issue to close; nothing to do.'
        exit 0
    }

    gh issue close $existingIssue.number `
        --comment 'All packages are now up to date. Closing this tracking issue automatically.'
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host "Closed tracking issue #$($existingIssue.number)."
    exit 0
}

$analyzerRows = @($outdatedPackages | Where-Object { $analyzerPackages -contains $_.Name })
$otherRows = @($outdatedPackages | Where-Object { $analyzerPackages -notcontains $_.Name })

$runUrl = "$env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY/actions/runs/$env:GITHUB_RUN_ID"
$checkedAt = [DateTime]::UtcNow.ToString('yyyy-MM-dd HH:mm')

$body = @"
This issue is automatically maintained by the ``dependency-check`` workflow. Do not edit it
by hand -- the workflow overwrites this body on every run while packages remain outdated.

## Analyzer packages (use ``/update-nuget-packages``)

These already have a full bump pipeline (editorconfig regeneration, rule coverage,
changelog, release) -- run the ``/update-nuget-packages`` skill.

$(Format-PackageTable $analyzerRows)

## Other packages

These can be bumped via the lightweight path: update the version, build, test, commit.

$(Format-PackageTable $otherRows)

_Last checked $checkedAt UTC by [this workflow run]($runUrl)._
"@

$bodyPath = [System.IO.Path]::GetTempFileName()
Set-Content -Path $bodyPath -Value $body -NoNewline

try {
    $existingIssue = Find-OpenTrackingIssue -Title $Title

    if ($existingIssue) {
        gh issue edit $existingIssue.number --body-file $bodyPath
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        Write-Host "Updated existing tracking issue #$($existingIssue.number)."
    }
    else {
        gh issue create --title $Title --body-file $bodyPath
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        Write-Host 'Created new tracking issue.'
    }
}
finally {
    Remove-Item -Path $bodyPath -ErrorAction SilentlyContinue
}
