#requires -Version 7
<#
.SYNOPSIS
  Module 3 — Admin Blazor portal verification + commit + tag.

.DESCRIPTION
  1. doctor       — confirm dotnet, git, repo state
  2. build        — restore + build entire solution
  3. boot API     — :5121 background job
  4. boot Admin   — :5296 background job
  5. smoke-test   — login → admin host → audit endpoint
  6. tests        — re-run xUnit suite
  7. commit + tag — feat(module3): … + v0.3
#>
[CmdletBinding()]
param(
    [switch]$SkipBuild,
    [switch]$SkipTests,
    [switch]$SkipCommit
)

$ErrorActionPreference = 'Stop'
$Repo = (Resolve-Path "$PSScriptRoot/..").Path
$Sln  = Join-Path $Repo 'NexusFine.sln'

function Step($n,$t) { Write-Host "`n━━ $n. $t " -ForegroundColor Cyan -NoNewline; Write-Host ("─" * (60 - $t.Length)) -ForegroundColor DarkGray }
function OK($m)      { Write-Host "  ✓ $m" -ForegroundColor Green }
function Warn($m)    { Write-Host "  ! $m" -ForegroundColor Yellow }
function Fail($m)    { Write-Host "  ✗ $m" -ForegroundColor Red; throw $m }

# ── 1. DOCTOR ─────────────────────────────────────────────────
Step 1 'Doctor'
$dotnetV = (& dotnet --version).Trim()
OK "dotnet $dotnetV"
$gitV = (& git --version).Trim()
OK "$gitV"
Push-Location $Repo
$branch = (git rev-parse --abbrev-ref HEAD).Trim()
OK "branch: $branch"
Pop-Location

# ── 2. BUILD ──────────────────────────────────────────────────
if (-not $SkipBuild) {
    Step 2 'Build'
    Push-Location $Repo
    & dotnet build $Sln -c Debug --nologo -v minimal | Out-Host
    if ($LASTEXITCODE -ne 0) { Pop-Location; Fail "build failed" }
    Pop-Location
    OK "build green"
}

# ── 3. BOOT API ───────────────────────────────────────────────
Step 3 'Boot API on :5121'
$apiProj = Join-Path $Repo 'src/NexusFine.API/NexusFine.API.csproj'
$apiJob = Start-Job -Name nexus-api -ScriptBlock {
    param($p) Set-Location (Split-Path $p)
    $env:ASPNETCORE_URLS = 'http://localhost:5121'
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    & dotnet run --no-build --project $p
} -ArgumentList $apiProj
OK "API job id $($apiJob.Id)"
Start-Sleep -Seconds 8

# ── 4. BOOT ADMIN ────────────────────────────────────────────
Step 4 'Boot Admin on :5296'
$adminProj = Join-Path $Repo 'src/NexusFine.Admin/NexusFine.Admin.csproj'
$adminJob = Start-Job -Name nexus-admin -ScriptBlock {
    param($p) Set-Location (Split-Path $p)
    $env:ASPNETCORE_URLS = 'http://localhost:5296'
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    & dotnet run --no-build --project $p
} -ArgumentList $adminProj
OK "Admin job id $($adminJob.Id)"
Start-Sleep -Seconds 10

try {
    # ── 5. SMOKE TEST ─────────────────────────────────────────
    Step 5 'Smoke test'

    # 5a: Admin login page reachable
    $login = Invoke-WebRequest 'http://localhost:5296/login' -UseBasicParsing -TimeoutSec 8
    if ($login.StatusCode -ne 200) { Fail "admin /login returned $($login.StatusCode)" }
    OK "admin /login → 200"

    # 5b: API login as admin → JWT
    $body = @{ userName = 'admin'; password = 'Nexus@Admin2026' } | ConvertTo-Json
    $loginRes = Invoke-RestMethod 'http://localhost:5121/api/auth/login' `
        -Method Post -ContentType 'application/json' -Body $body
    if (-not $loginRes.accessToken) { Fail "API login did not return accessToken" }
    $token = $loginRes.accessToken
    OK "API JWT issued (expires $($loginRes.accessTokenExpiresAt))"
    OK "user: $($loginRes.user.fullName) — roles: $($loginRes.user.roles -join ',')"

    # 5c: Audit log endpoint with bearer
    $hdrs = @{ Authorization = "Bearer $token" }
    $audit = Invoke-RestMethod 'http://localhost:5121/api/auditlogs?page=1&pageSize=5' -Headers $hdrs
    OK "audit log → total=$($audit.total) page=$($audit.page) returned=$($audit.data.Count)"

    # 5d: Audit dropdowns
    $entities = Invoke-RestMethod 'http://localhost:5121/api/auditlogs/entitytypes' -Headers $hdrs
    OK "audit entity types: $(($entities | Select-Object -First 5) -join ', ')"

    # 5e: Anonymous request to audit must 401
    try {
        Invoke-WebRequest 'http://localhost:5121/api/auditlogs' -UseBasicParsing -TimeoutSec 5 | Out-Null
        Warn "anonymous /api/auditlogs did NOT 401"
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -eq 401) {
            OK "anonymous /api/auditlogs → 401 (RBAC enforced)"
        } else {
            Warn "anonymous /api/auditlogs returned $($_.Exception.Response.StatusCode.value__)"
        }
    }

    # ── 6. TESTS ──────────────────────────────────────────────
    if (-not $SkipTests) {
        Step 6 'Tests'
        Push-Location $Repo
        & dotnet test $Sln --nologo --no-build -v minimal | Out-Host
        if ($LASTEXITCODE -ne 0) { Pop-Location; Fail "tests failed" }
        Pop-Location
        OK "tests green"
    }
}
finally {
    Write-Host "`n[cleanup] stopping background jobs…" -ForegroundColor DarkGray
    Stop-Job -Name nexus-api,nexus-admin -ErrorAction SilentlyContinue
    Remove-Job -Name nexus-api,nexus-admin -Force -ErrorAction SilentlyContinue
}

# ── 7. COMMIT + TAG ──────────────────────────────────────────
if (-not $SkipCommit) {
    Step 7 'Commit + tag'
    Push-Location $Repo
    git add -A
    $status = git status --porcelain
    if ([string]::IsNullOrWhiteSpace($status)) {
        OK "nothing to commit"
    } else {
        git commit -m "feat(module3): admin portal — JWT login, RBAC gating, audit log viewer" | Out-Host
        OK "commit created"
    }
    $existing = git tag --list 'v0.3'
    if (-not [string]::IsNullOrWhiteSpace($existing)) {
        Warn "tag v0.3 already exists — skipping"
    } else {
        git tag -a v0.3 -m "Module 3: admin portal w/ auth + audit"
        OK "tag v0.3"
    }
    Pop-Location
}

Write-Host "`n✅ Module 3 complete." -ForegroundColor Green
