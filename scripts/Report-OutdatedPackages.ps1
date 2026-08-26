#Requires -Version 7
<#
.SYNOPSIS
    Parses a `dotnet outdated` JSON report and creates, comments on, or closes a single,
    persistent GitHub tracking issue listing every outdated package.

.DESCRIPTION
    Used by .github/workflows/dependency-check.yml.

    `dotnet outdated` only writes the JSON report file when at least one dependency is
    outdated; a missing report means nothing is outdated. Because "no file" and "the tool
    broke" look identical from here, pass -ToolOutputPath so a missing report can be
    corroborated against the tool's own "No outdated dependencies were detected" line. A
    missing report with no corroboration is a hard failure -- silently reporting "all clear"
    and closing the issue is the worst outcome this script can produce.

    NOTIFICATIONS. A GitHub issue *body edit* raises no timeline event and therefore no
    notification, so overwriting the body every week is invisible to the maintainer. This
    script instead fingerprints the outdated set and embeds it in the body as an HTML
    comment, then:
      - set unchanged  -> does nothing at all (no edit, no comment, no noise)
      - set changed    -> rewrites the body AND posts a comment with the exact delta
      - all now current-> comments and closes
    The issue is also assigned (-Assignee) on creation, which notifies regardless of the
    maintainer's repository watch level.

    The tracking issue is found by label (-Label), falling back to an exact title match so
    an issue created before labelling existed is still adopted rather than duplicated. Only
    open issues are considered: once closed, the next genuine change creates a fresh issue,
    which is itself a notifiable event.

    Requires the `gh` CLI to be authenticated (e.g. via the GH_TOKEN environment variable)
    and to be run from within the target repository.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $ReportPath,

    # Captured stdout of the `dotnet outdated` run. Optional, but without it a missing
    # report file cannot be distinguished from a crashed tool.
    [string] $ToolOutputPath,

    # The .nuspec is the single source of truth for which analyzer packages this package
    # actually ships -- see Get-ShippedAnalyzerPackage below.
    [string] $NuspecPath = 'packages/Opinionated.DotNet.CodingStandards/Opinionated.DotNet.CodingStandards.nuspec',

    [string] $Title = 'Outdated NuGet packages detected',

    [string] $Label = 'dependencies',

    # GitHub login to assign the issue to. Assignment is a notifiable event, unlike a body
    # edit, so it is what actually reaches the maintainer's inbox.
    [string] $Assignee
)

$ErrorActionPreference = 'Stop'

# Printed by `dotnet outdated` when every dependency is current. Used to corroborate a
# missing report file.
$upToDateSentinel = 'No outdated dependencies were detected'

# Machine-readable state parked in the issue body. Invisible in rendered Markdown, and it
# is what lets a later run compute an exact delta instead of guessing from the tables.
$stateMarkerPattern = '<!--\s*dependency-check-state:\s*(?<json>\{.*?\})\s*-->'

function Stop-WithError {
    param([string] $Message)

    Write-Host "::error::$Message"
    exit 1
}

# The five analyzer packages are declared in exactly one place that is already kept in sync
# with Directory.Packages.props (by CheckNugetDependenciesMatchProps.cs): the .nuspec's
# <dependencies>. Deriving the list here means a sixth analyzer cannot silently show up in
# the "Other packages" table.
function Get-ShippedAnalyzerPackage {
    param([string] $Path)

    if (-not (Test-Path -Path $Path)) {
        Stop-WithError "Cannot derive the analyzer package list: '$Path' not found."
    }

    $nuspec = [xml](Get-Content -Path $Path -Raw)
    $ids = @($nuspec.package.metadata.dependencies.dependency | ForEach-Object { $_.id })

    if ($ids.Count -eq 0) {
        Stop-WithError "No <dependency> elements found in '$Path'; refusing to classify packages against an empty list."
    }

    return $ids
}

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

# Identity of the outdated *set* -- name plus both versions, so partial progress (one of
# three packages bumped) counts as a change and earns a notification.
function Get-OutdatedFingerprint {
    param([array] $Packages)

    $canonical = ($Packages | ForEach-Object { "$($_.Name)=$($_.Current)->$($_.Latest)" }) -join ';'
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($canonical)
    $digest = [System.Security.Cryptography.SHA256]::HashData($bytes)

    return [System.Convert]::ToHexString($digest).Substring(0, 12).ToLowerInvariant()
}

# Recovers the previous run's outdated set from the issue body. Returns $null when the body
# carries no marker (an issue created before this mechanism existed, or hand-written), which
# callers treat as "changed" so the next run re-establishes the marker.
function Read-TrackingState {
    param([string] $Body)

    if ([string]::IsNullOrWhiteSpace($Body)) {
        return $null
    }

    $match = [regex]::Match($Body, $stateMarkerPattern, 'Singleline')
    if (-not $match.Success) {
        return $null
    }

    try {
        return $match.Groups['json'].Value | ConvertFrom-Json
    }
    catch {
        # A corrupt marker is not worth failing the run over -- treat it as absent and let
        # this run rewrite it.
        Write-Host "Ignoring unparsable state marker on the tracking issue: $($_.Exception.Message)"
        return $null
    }
}

# Label first (stable across title edits), exact title second (adopts the pre-labelling
# issue instead of duplicating it). Only open issues count: a closed issue is deliberately
# left closed so the next change creates -- and notifies about -- a fresh one.
function Find-TrackingIssue {
    param(
        [string] $Title,
        [string] $Label
    )

    $issues = gh issue list --state open --json number,title,body,labels,assignees --limit 200 | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $byLabel = $issues | Where-Object { $_.labels.name -contains $Label } | Select-Object -First 1
    if ($byLabel) {
        return $byLabel
    }

    return $issues | Where-Object { $_.title -eq $Title } | Select-Object -First 1
}

# `gh issue create --label` and `--add-label` both fail on a label that does not exist yet;
# --force makes this idempotent (creates it, or updates its colour/description in place).
function Initialize-Label {
    param([string] $Label)

    gh label create $Label --color 0366d6 --description 'Outdated NuGet packages tracked by the dependency-check workflow' --force
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

# One-time repairs for an issue that predates this mechanism: it was found by the title
# fallback, so give it the label (making the primary lookup work from now on) and an
# assignee (so it can actually notify). Both guards stop this recurring.
function Repair-TrackingIssue {
    param(
        $Issue,
        [string] $Label,
        [string] $Assignee
    )

    if ($Issue.labels.name -notcontains $Label) {
        Initialize-Label -Label $Label
        gh issue edit $Issue.number --add-label $Label
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        Write-Host "Labelled tracking issue #$($Issue.number) with '$Label'."
    }

    if (-not [string]::IsNullOrWhiteSpace($Assignee) -and @($Issue.assignees).Count -eq 0) {
        gh issue edit $Issue.number --add-assignee $Assignee
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        Write-Host "Assigned tracking issue #$($Issue.number) to $Assignee."
    }
}

function Write-StepSummary {
    param([string] $Markdown)

    if ([string]::IsNullOrWhiteSpace($env:GITHUB_STEP_SUMMARY)) {
        return
    }

    Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value $Markdown
}

# Guards against the failure mode where the report exists but no longer looks like a
# `dotnet outdated` report -- a schema change would otherwise flatten to zero outdated
# packages and close the issue with a false "all clear". Note that a *structurally valid*
# report containing no version differences is legitimate (the tool reports a non-"None"
# UpgradeSeverity for some pre-release packages even when the versions match), so only the
# structure is asserted here, never the count.
function Assert-RecognisableReport {
    param(
        $Report,
        [string] $Path
    )

    if ($null -eq $Report.Projects) {
        Stop-WithError "'$Path' has no 'Projects' property -- the dotnet-outdated report schema may have changed. Refusing to infer that nothing is outdated."
    }

    $projects = @($Report.Projects)
    if ($projects.Count -eq 0) {
        Stop-WithError "'$Path' contains zero projects -- the dotnet-outdated report schema may have changed. Refusing to infer that nothing is outdated."
    }

    foreach ($project in $projects) {
        foreach ($targetFramework in $project.TargetFrameworks) {
            foreach ($dependency in $targetFramework.Dependencies) {
                if ($dependency.PSObject.Properties['ResolvedVersion'] -and
                    $dependency.PSObject.Properties['LatestVersion']) {
                    return
                }
            }
        }
    }

    Stop-WithError "'$Path' contains no dependency with both 'ResolvedVersion' and 'LatestVersion' -- the dotnet-outdated report schema may have changed. Refusing to infer that nothing is outdated."
}

# Flatten every project/target-framework's dependency list into one de-duplicated set keyed
# by package name. A package is only "outdated" when its resolved version genuinely differs
# from the latest available version -- ignore the tool's UpgradeSeverity field, which can
# report a non-"None" severity for pre-release packages even when the two versions match.
$outdated = @{}
if (Test-Path -Path $ReportPath) {
    $report = Get-Content -Path $ReportPath -Raw | ConvertFrom-Json
    Assert-RecognisableReport -Report $report -Path $ReportPath

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
elseif ($ToolOutputPath) {
    # No report file. That is the documented "everything is current" signal, but only if the
    # tool actually said so -- otherwise it crashed, wrote nowhere, and we must not close the
    # tracking issue on the strength of a missing file.
    if (-not (Test-Path -Path $ToolOutputPath)) {
        Stop-WithError "Neither the report '$ReportPath' nor the captured tool output '$ToolOutputPath' exists; cannot tell whether packages are current."
    }

    $toolOutput = Get-Content -Path $ToolOutputPath -Raw
    if ($toolOutput -notmatch [regex]::Escape($upToDateSentinel)) {
        Stop-WithError "'$ReportPath' was not written and the tool never printed '$upToDateSentinel'. Treating this as a failed check rather than as 'nothing outdated'. Captured output:`n$toolOutput"
    }
}
else {
    Write-Host "No report at '$ReportPath' and no -ToolOutputPath supplied; assuming nothing is outdated (pass -ToolOutputPath to verify this)."
}

$outdatedPackages = @($outdated.Values | Sort-Object -Property Name)

if ($outdatedPackages.Count -eq 0) {
    Write-Host 'No outdated packages detected.'
    Write-StepSummary "## Dependency check`n`nAll packages are up to date."

    $existingIssue = Find-TrackingIssue -Title $Title -Label $Label
    if (-not $existingIssue) {
        Write-Host 'No open tracking issue to close; nothing to do.'
        exit 0
    }

    # Name what was fixed when the previous run left a state marker behind -- more useful in
    # the notification than a bare "closing this".
    $previousState = Read-TrackingState -Body $existingIssue.body
    $resolvedNames = @($previousState.packages | ForEach-Object { $_.n })
    $closingComment = if ($resolvedNames.Count -gt 0) {
        "All packages are now up to date (resolved: $($resolvedNames -join ', ')). Closing this tracking issue automatically."
    }
    else {
        'All packages are now up to date. Closing this tracking issue automatically.'
    }

    gh issue close $existingIssue.number --comment $closingComment
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host "Closed tracking issue #$($existingIssue.number)."
    exit 0
}

$analyzerPackages = Get-ShippedAnalyzerPackage -Path $NuspecPath
$analyzerRows = @($outdatedPackages | Where-Object { $analyzerPackages -contains $_.Name })
$otherRows = @($outdatedPackages | Where-Object { $analyzerPackages -notcontains $_.Name })

$fingerprint = Get-OutdatedFingerprint -Packages $outdatedPackages
$state = [PSCustomObject]@{
    fingerprint = $fingerprint
    packages    = @($outdatedPackages | ForEach-Object { [PSCustomObject]@{ n = $_.Name; c = $_.Current; l = $_.Latest } })
}
$stateJson = $state | ConvertTo-Json -Compress -Depth 5

$runUrl = "$env:GITHUB_SERVER_URL/$env:GITHUB_REPOSITORY/actions/runs/$env:GITHUB_RUN_ID"
$checkedAt = [DateTime]::UtcNow.ToString('yyyy-MM-dd HH:mm')

$body = @"
This issue is automatically maintained by the ``dependency-check`` workflow. Do not edit it
by hand -- the workflow rewrites this body whenever the set of outdated packages changes,
and comments with the delta so the change actually reaches your inbox.

## Analyzer packages (use ``/update-nuget-packages``)

These already have a full bump pipeline (editorconfig regeneration, rule coverage,
changelog, release) -- run the ``/update-nuget-packages`` skill.

$(Format-PackageTable $analyzerRows)

## Other packages

These can be bumped via the lightweight path: update the version, build, test, commit.

$(Format-PackageTable $otherRows)

_Last changed $checkedAt UTC by [this workflow run]($runUrl). This issue closes itself once
every package above is current._

<!-- dependency-check-state: $stateJson -->
"@

Write-StepSummary @"
## Dependency check

$($outdatedPackages.Count) outdated package(s), fingerprint ``$fingerprint``.

### Analyzer packages

$(Format-PackageTable $analyzerRows)

### Other packages

$(Format-PackageTable $otherRows)
"@

$bodyPath = [System.IO.Path]::GetTempFileName()
Set-Content -Path $bodyPath -Value $body -NoNewline

try {
    $existingIssue = Find-TrackingIssue -Title $Title -Label $Label

    if (-not $existingIssue) {
        Initialize-Label -Label $Label

        $createArgs = @('issue', 'create', '--title', $Title, '--label', $Label, '--body-file', $bodyPath)
        if (-not [string]::IsNullOrWhiteSpace($Assignee)) {
            $createArgs += @('--assignee', $Assignee)
        }

        gh @createArgs
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        Write-Host "Created new tracking issue for fingerprint $fingerprint."
        exit 0
    }

    Repair-TrackingIssue -Issue $existingIssue -Label $Label -Assignee $Assignee

    $previousState = Read-TrackingState -Body $existingIssue.body
    if ($previousState -and $previousState.fingerprint -eq $fingerprint) {
        # Deliberately does nothing: rewriting the body with an identical set is invisible to
        # GitHub's notification system anyway, and it churns the issue's updated-at for no
        # reason. The issue is already open and already says exactly this.
        Write-Host "Tracking issue #$($existingIssue.number) already reports fingerprint $fingerprint; leaving it untouched."
        exit 0
    }

    gh issue edit $existingIssue.number --body-file $bodyPath
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    # Build the delta against the previous marker. A missing marker means we cannot know what
    # changed, so report the whole current set instead of inventing a delta.
    $commentLines = @()
    if ($previousState) {
        $previousByName = @{}
        foreach ($package in $previousState.packages) {
            $previousByName[$package.n] = $package
        }

        $added = @($outdatedPackages | Where-Object { -not $previousByName.ContainsKey($_.Name) })
        $moved = @($outdatedPackages | Where-Object {
                $previousByName.ContainsKey($_.Name) -and
                ($previousByName[$_.Name].c -ne $_.Current -or $previousByName[$_.Name].l -ne $_.Latest)
            })
        $currentNames = @($outdatedPackages | ForEach-Object { $_.Name })
        $resolved = @($previousState.packages | Where-Object { $currentNames -notcontains $_.n })

        if ($added.Count -gt 0) {
            $commentLines += '**Newly outdated**'
            $commentLines += @($added | ForEach-Object { "- ``$($_.Name)`` $($_.Current) → $($_.Latest)" })
            $commentLines += ''
        }
        if ($moved.Count -gt 0) {
            $commentLines += '**Changed**'
            $commentLines += @($moved | ForEach-Object {
                    $was = $previousByName[$_.Name]
                    "- ``$($_.Name)`` was $($was.c) → $($was.l), now $($_.Current) → $($_.Latest)"
                })
            $commentLines += ''
        }
        if ($resolved.Count -gt 0) {
            $commentLines += '**No longer outdated**'
            $commentLines += @($resolved | ForEach-Object { "- ``$($_.n)`` (was $($_.c) → $($_.l))" })
            $commentLines += ''
        }
    }
    else {
        $commentLines += '**Currently outdated**'
        $commentLines += @($outdatedPackages | ForEach-Object { "- ``$($_.Name)`` $($_.Current) → $($_.Latest)" })
        $commentLines += ''
    }

    $commentLines += "See the issue body for the full tables. [Workflow run]($runUrl)."

    $commentPath = [System.IO.Path]::GetTempFileName()
    try {
        Set-Content -Path $commentPath -Value ($commentLines -join "`n") -NoNewline
        gh issue comment $existingIssue.number --body-file $commentPath
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    finally {
        Remove-Item -Path $commentPath -ErrorAction SilentlyContinue
    }

    Write-Host "Updated tracking issue #$($existingIssue.number) and commented the delta (fingerprint $fingerprint)."
}
finally {
    Remove-Item -Path $bodyPath -ErrorAction SilentlyContinue
}
