# Audit-OSS-Licences.ps1
#
# Walks every csproj in the solution, lists all NuGet packages (direct +
# transitive), queries the NuGet API for the licence expression and project
# URL, and produces a CSV of every (project × package) pair.
#
# Usage:
#   pwsh .\scripts\audit-oss-licences.ps1
#   pwsh .\scripts\audit-oss-licences.ps1 -OutFile docs/compliance/oss-licence-report.csv
#
# Policy classes (per docs/compliance/oss-licence-audit.md):
#   APPROVED: MIT, Apache-2.0, BSD-2-Clause, BSD-3-Clause, ISC, MS-PL, MS-RL
#   ADVISORY: MPL-2.0, EPL-2.0, LGPL-*
#   BLOCKED : GPL-*, AGPL-*
#   UNKNOWN : everything else / no licence on NuGet

param(
    [string]$OutFile = 'docs/compliance/oss-licence-report.csv'
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

$approved = @('MIT','Apache-2.0','BSD-2-Clause','BSD-3-Clause','ISC','MS-PL','MS-RL')
$advisory = @('MPL-2.0','EPL-2.0','LGPL-2.1-only','LGPL-2.1-or-later','LGPL-3.0-only','LGPL-3.0-or-later')
$blocked  = @('GPL-2.0-only','GPL-2.0-or-later','GPL-3.0-only','GPL-3.0-or-later','AGPL-3.0-only','AGPL-3.0-or-later')

function Get-Class($expr) {
    if (-not $expr) { return 'UNKNOWN' }
    foreach ($lic in $expr -split '\s+OR\s+|\s+AND\s+|\s+WITH\s+|\(|\)') {
        $l = $lic.Trim()
        if ($blocked  -contains $l) { return 'BLOCKED' }
    }
    foreach ($lic in $expr -split '\s+OR\s+|\s+AND\s+|\s+WITH\s+|\(|\)') {
        $l = $lic.Trim()
        if ($approved -contains $l) { return 'APPROVED' }
    }
    foreach ($lic in $expr -split '\s+OR\s+|\s+AND\s+|\s+WITH\s+|\(|\)') {
        $l = $lic.Trim()
        if ($advisory -contains $l) { return 'ADVISORY' }
    }
    return 'UNKNOWN'
}

Write-Host "Walking csproj files…" -ForegroundColor Yellow
$projects = Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' }

$rows = @()
$cache = @{}
$count = 0

foreach ($proj in $projects) {
    Write-Host "  $($proj.Name)…" -ForegroundColor Cyan
    $out = dotnet list $proj.FullName package --include-transitive 2>&1 | Out-String

    foreach ($line in $out -split "`n") {
        # Direct entries:    "   > Microsoft.AspNetCore.X    10.0.5    10.0.5"
        # Transitive:        "      > System.Y               4.7.0"
        if ($line -match '>\s+(?<pkg>[\w\.\-]+)\s+(?<ver>[\d\.\-\w]+)') {
            $pkg = $Matches.pkg
            $ver = $Matches.ver
            $key = "$pkg|$ver"

            if (-not $cache.ContainsKey($key)) {
                # Use the v3 flat-container nuspec endpoint — returns XML directly,
                # works for every public NuGet package, no auth needed.
                # https://api.nuget.org/v3-flatcontainer/{id-lower}/{ver}/{id-lower}.nuspec
                $lowerId = $pkg.ToLower()
                $url = "https://api.nuget.org/v3-flatcontainer/$lowerId/$ver/$lowerId.nuspec"
                $licence = ''
                $projUrl = ''
                try {
                    $xmlText = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop |
                                Select-Object -ExpandProperty Content
                    [xml]$xml = $xmlText
                    $meta = $xml.package.metadata

                    # Modern packages use <license type="expression">MIT</license>
                    if ($meta.license) {
                        if ($meta.license -is [string]) {
                            $licence = $meta.license
                        } elseif ($meta.license.'#text') {
                            $licence = $meta.license.'#text'
                        } elseif ($meta.license.InnerText) {
                            $licence = $meta.license.InnerText
                        }
                    }
                    # Legacy packages use <licenseUrl>https://…/LICENSE.txt</licenseUrl>
                    if (-not $licence -and $meta.licenseUrl) {
                        $licence = "URL: $($meta.licenseUrl)"
                    }
                    $projUrl = $meta.projectUrl
                } catch {
                    # 404 or transient — leave unknown
                }
                $cache[$key] = [pscustomobject]@{ Licence = $licence; ProjectUrl = $projUrl }
            }

            $info  = $cache[$key]
            $class = Get-Class $info.Licence
            $rows += [pscustomobject]@{
                Project    = $proj.Name
                Package    = $pkg
                Version    = $ver
                Licence    = $info.Licence
                ProjectUrl = $info.ProjectUrl
                Status     = $class
            }
            $count++
        }
    }
}

$rows | Sort-Object Project, Package | Export-Csv -Path $OutFile -NoTypeInformation
Write-Host "`nReport: $OutFile  ($count pairs)" -ForegroundColor Green

# Summary
$summary = $rows | Group-Object Status | Select-Object Name, Count
Write-Host "`nLicence-class summary:" -ForegroundColor Yellow
$summary | Format-Table -AutoSize

$blockedRows = $rows | Where-Object { $_.Status -eq 'BLOCKED' } | Sort-Object Package -Unique
if ($blockedRows) {
    Write-Host "BLOCKED packages found:" -ForegroundColor Red
    $blockedRows | Format-Table Package, Version, Licence -AutoSize
    exit 2
}
$unknownRows = $rows | Where-Object { $_.Status -eq 'UNKNOWN' } | Sort-Object Package -Unique
if ($unknownRows) {
    Write-Host "UNKNOWN licence (resolve manually):" -ForegroundColor Yellow
    $unknownRows | Format-Table Package, Version, Licence -AutoSize
}
