param(
    [string]$Owner = "mahmoudadel42",
    [string]$RepoName = "payroll-system",
    [string]$Visibility = "private",
    [string]$ProjectTitle = "Payroll System"
)

$ErrorActionPreference = "Continue"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:Path += ";C:\Program Files\GitHub CLI"
$Repo = "$Owner/$RepoName"
$Root = Split-Path $PSScriptRoot -Parent
$Utf8 = New-Object System.Text.UTF8Encoding($false)
$Tmp = Join-Path $env:TEMP "payroll-gh"
New-Item -ItemType Directory -Force -Path $Tmp | Out-Null

function Invoke-GhApi([string]$Method, [string]$Path, $Payload) {
    $ghArgs = @("api", "-X", $Method, $Path, "-H", "Accept: application/vnd.github+json")
    if ($null -ne $Payload) {
        $file = Join-Path $Tmp ([guid]::NewGuid().ToString() + ".json")
        [IO.File]::WriteAllText($file, ($Payload | ConvertTo-Json -Depth 10), $Utf8)
        $ghArgs += @("--input", $file)
    }
    $out = & gh @ghArgs 2>$null
    if ($LASTEXITCODE -ne 0) { throw "gh api $Method $Path failed: $out" }
    if ($out) { return ($out -join "`n") | ConvertFrom-Json }
}

# 1. Repository + initial push
& gh repo view $Repo *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Creating repository $Repo ($Visibility)"
    & gh repo create $Repo "--$Visibility" --description "Payroll management system (.NET 10 + Angular, Arabic/English)"
    if ($LASTEXITCODE -ne 0) { throw "repo create failed" }
}

Push-Location $Root
if (-not (Test-Path ".git")) {
    git init -b main | Out-Null
    git add .
    git commit -m "docs: requirements analysis, architecture and implementation plan" | Out-Null
    git remote add origin "https://github.com/$Repo.git"
    & gh auth setup-git
    git push -u origin main
    git push origin main:develop
}
Pop-Location

# 2. Labels
$labels = @(
    @{ name = "type:epic"; color = "5319E7"; description = "Epic grouping several tasks" },
    @{ name = "type:task"; color = "1D76DB"; description = "Implementation task (one PR)" },
    @{ name = "area:backend"; color = "0E8A16"; description = ".NET API" },
    @{ name = "area:frontend"; color = "D93F0B"; description = "Angular app" },
    @{ name = "area:devops"; color = "6F42C1"; description = "CI/CD, Docker" },
    @{ name = "area:qa"; color = "FBCA04"; description = "Testing" },
    @{ name = "area:docs"; color = "C5DEF5"; description = "Documentation" },
    @{ name = "module:core"; color = "BFDADC"; description = "Shared / infrastructure" },
    @{ name = "module:auth"; color = "BFDADC"; description = "Authentication & users" },
    @{ name = "module:employees"; color = "BFDADC"; description = "Employee management" },
    @{ name = "module:salaries"; color = "BFDADC"; description = "Salary management" },
    @{ name = "module:departments"; color = "BFDADC"; description = "Department management" },
    @{ name = "module:incentives"; color = "BFDADC"; description = "Incentive management" },
    @{ name = "module:attendance"; color = "BFDADC"; description = "Attendance management" },
    @{ name = "module:payroll"; color = "BFDADC"; description = "Payroll calculation" },
    @{ name = "module:reports"; color = "BFDADC"; description = "Reports & dashboard" },
    @{ name = "priority:high"; color = "B60205"; description = "" },
    @{ name = "priority:medium"; color = "E99695"; description = "" },
    @{ name = "priority:low"; color = "F9D0C4"; description = "" },
    @{ name = "i18n"; color = "006B75"; description = "Arabic / English localization" }
)
foreach ($l in $labels) {
    & gh label create $l.name --repo $Repo --color $l.color --description $l.description --force | Out-Null
}
Write-Host "Labels ready"

# 3. Milestones
$milestoneDefs = [ordered]@{
    "M0 - Project Setup"             = "Repo, solution skeletons, DB, localization, CI, Docker, authentication"
    "M1 - Master Data"               = "Departments, grade salaries, employees"
    "M2 - Incentives & Attendance"   = "Department & service incentives, absence rules and monthly absence entry"
    "M3 - Payroll Engine"            = "Payroll calculation, monthly runs, payslips, approval"
    "M4 - Reports & Dashboard"       = "Four reports (PDF/Excel, AR/EN) and dashboard"
    "M5 - QA & Release"              = "Integration/E2E tests, hardening, docs, release"
}
$existingMs = @(Invoke-GhApi GET "repos/$Repo/milestones?state=all&per_page=100" $null)
$milestones = @{}
foreach ($title in $milestoneDefs.Keys) {
    $m = $existingMs | Where-Object { $_.title -eq $title } | Select-Object -First 1
    if (-not $m) { $m = Invoke-GhApi POST "repos/$Repo/milestones" @{ title = $title; description = $milestoneDefs[$title] } }
    $milestones[$title] = $m.number
}
Write-Host "Milestones ready"

# 4. Issues
$existingIssues = @()
$page = 1
do {
    $batch = @(Invoke-GhApi GET "repos/$Repo/issues?state=all&per_page=100&page=$page" $null)
    $existingIssues += $batch
    $page++
} while ($batch.Count -eq 100)

function Get-OrCreateIssue($title, $body, $labelList, $milestoneNumber) {
    $found = $existingIssues | Where-Object { $_.title -eq $title -and -not $_.pull_request } | Select-Object -First 1
    if ($found) { Write-Host "  exists #$($found.number) $title"; return $found }
    $issue = Invoke-GhApi POST "repos/$Repo/issues" @{ title = $title; body = $body; labels = $labelList; milestone = $milestoneNumber }
    Write-Host "  created #$($issue.number) $title"
    return $issue
}

$allIssues = @()
$blockPattern = '(?s)<!-- (EPIC|TASK) -->\s*(.*?)(?=<!-- (?:EPIC|TASK) -->|\z)'
foreach ($file in Get-ChildItem (Join-Path $Root "docs\issues") -Filter *.md | Sort-Object Name) {
    Write-Host "Processing $($file.Name)"
    $content = [IO.File]::ReadAllText($file.FullName, $Utf8)
    $epic = $null; $epicMilestone = $null; $epicBody = $null; $children = @()
    foreach ($m in [regex]::Matches($content, $blockPattern)) {
        $kind = $m.Groups[1].Value
        $parts = [regex]::Split($m.Groups[2].Value, '\r?\n---\r?\n', 2)
        $meta = @{}
        foreach ($line in ($parts[0] -split '\r?\n')) {
            if ($line -match '^(\w+):\s*(.*)$') { $meta[$Matches[1]] = $Matches[2].Trim() }
        }
        $body = $parts[1].Trim()
        $labelList = @($meta.labels -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ })
        $msTitle = if ($meta.milestone) { $meta.milestone } else { $epicMilestone }
        if ($kind -eq "EPIC") {
            $epicMilestone = $msTitle
            $epicBody = $body
            $epic = Get-OrCreateIssue $meta.title $body $labelList $milestones[$msTitle]
            $allIssues += $epic
        } else {
            $body = "**Epic:** #$($epic.number)`n`n$body"
            $task = Get-OrCreateIssue $meta.title $body $labelList $milestones[$msTitle]
            $children += $task
            $allIssues += $task
            try { Invoke-GhApi POST "repos/$Repo/issues/$($epic.number)/sub_issues" @{ sub_issue_id = [int64]$task.id } | Out-Null } catch { }
        }
    }
    $checklist = ($children | ForEach-Object { "- [ ] #$($_.number)" }) -join "`n"
    Invoke-GhApi PATCH "repos/$Repo/issues/$($epic.number)" @{ body = "$epicBody`n`n## Tasks`n$checklist" } | Out-Null
}

# 5. Project board
$projects = (& gh project list --owner $Owner --format json --limit 100 | Out-String | ConvertFrom-Json).projects
$project = $projects | Where-Object { $_.title -eq $ProjectTitle } | Select-Object -First 1
if (-not $project) {
    $project = & gh project create --owner $Owner --title $ProjectTitle --format json | Out-String | ConvertFrom-Json
}
& gh project link $project.number --owner $Owner --repo $Repo 2>$null | Out-Null
foreach ($i in $allIssues) {
    & gh project item-add $project.number --owner $Owner --url $i.html_url | Out-Null
}

Write-Host ""
Write-Host "Repository: https://github.com/$Repo"
Write-Host "Issues:     https://github.com/$Repo/issues ($($allIssues.Count))"
Write-Host "Project:    $($project.url)"
