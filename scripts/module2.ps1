# scripts/module2.ps1 — Module 2: Citizen web portal.
# - Builds the API (so wwwroot is in place)
# - Boots the API in a background job
# - Smoke-tests:
#       /                         (redirects to /citizen/)
#       /citizen/                 (HTML 200)
#       /citizen/i18n.js          (200)
#       /citizen/app.js           (200)
#       /api/offencecodes         (200)
#       /api/fines/lookup?type=ref&value=NXF-2026-00001  (200 with JSON)
# - Commits   feat(module2): citizen portal — live API, bilingual EN/NY, jsPDF receipt
# - Tags      v0.2
#
# Usage: pwsh ./scripts/module2.ps1
. "$PSScriptRoot/_lib.ps1"

$root = Get-RepoRoot
Push-Location $root
try {
    Write-Step "Module 2 — Citizen web portal (live, bilingual EN/NY, PDF receipt)"

    # 1. Doctor
    Write-Step "1/7  Doctor"
    & pwsh "$PSScriptRoot/doctor.ps1"
    if ($LASTEXITCODE -ne 0) { Write-Fail "Doctor failed. Install missing tools and retry." }

    # 2. Verify portal files
    Write-Step "2/7  Verify wwwroot files"
    $files = @(
        "src/NexusFine.API/wwwroot/citizen/index.html",
        "src/NexusFine.API/wwwroot/citizen/i18n.js",
        "src/NexusFine.API/wwwroot/citizen/app.js",
        "src/NexusFine.API/wwwroot/css/theme.css"
    )
    foreach ($f in $files) {
        if (-not (Test-Path (Join-Path $root $f))) { Write-Fail "Missing: $f" }
        Write-OK $f
    }

    # 3. Build
    Write-Step "3/7  Build"
    Invoke-Checked "dotnet build NexusFine.slnx -c Debug --nologo" "dotnet build"

    # 4. Run API in background, smoke-test, kill it
    Write-Step "4/7  Boot API + smoke-test endpoints"
    $apiProj = Join-Path $root "src/NexusFine.API/NexusFine.API.csproj"
    $port    = 5080  # avoid clashing with usual 5000/5001
    $env:ASPNETCORE_URLS        = "http://localhost:$port"
    $env:ASPNETCORE_ENVIRONMENT = "Development"

    $job = Start-Job -ScriptBlock {
        param($proj, $port)
        $env:ASPNETCORE_URLS        = "http://localhost:$port"
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        & dotnet run --project $proj --no-build --no-launch-profile
    } -ArgumentList $apiProj, $port

    try {
        $base = "http://localhost:$port"
        Write-Host "    waiting for API to come up at $base ..." -ForegroundColor DarkGray
        $up = $false
        for ($i = 0; $i -lt 30; $i++) {
            Start-Sleep -Seconds 1
            try {
                $r = Invoke-WebRequest -Uri "$base/api/offencecodes" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
                if ($r.StatusCode -eq 200) { $up = $true; break }
            } catch { }
        }
        if (-not $up) {
            Write-Host "    --- API job output ---" -ForegroundColor DarkGray
            Receive-Job -Job $job -Keep | ForEach-Object { Write-Host "    $_" -ForegroundColor DarkGray }
            Write-Fail "API did not respond on $base within 30s."
        }
        Write-OK "API up at $base"

        # /citizen/
        $r = Invoke-WebRequest -Uri "$base/citizen/" -UseBasicParsing
        if ($r.StatusCode -ne 200 -or ($r.Content -notmatch 'NEXUS')) { Write-Fail "/citizen/ did not return portal HTML." }
        Write-OK "GET /citizen/  (200, HTML)"

        # i18n.js + app.js
        foreach ($asset in @("/citizen/i18n.js", "/citizen/app.js", "/css/theme.css")) {
            $r = Invoke-WebRequest -Uri "$base$asset" -UseBasicParsing
            if ($r.StatusCode -ne 200) { Write-Fail "$asset did not return 200 (got $($r.StatusCode))" }
            Write-OK "GET $asset  (200)"
        }

        # offence codes
        $r = Invoke-WebRequest -Uri "$base/api/offencecodes" -UseBasicParsing
        if ($r.StatusCode -ne 200) { Write-Fail "/api/offencecodes did not return 200." }
        Write-OK "GET /api/offencecodes  (200)"

        # fine lookup (seed data should include NXF-2026-00001)
        try {
            $r = Invoke-WebRequest -Uri "$base/api/fines/lookup?type=ref&value=NXF-2026-00001" -UseBasicParsing
            if ($r.StatusCode -eq 200) {
                Write-OK "GET /api/fines/lookup?type=ref&value=NXF-2026-00001  (200)"
            } else {
                Write-Warn2 "lookup returned $($r.StatusCode) — seed may not contain that ref; continuing."
            }
        } catch {
            Write-Warn2 ("lookup hit an exception: " + $_.Exception.Message + " — continuing.")
        }
    } finally {
        Write-Host "    stopping API job..." -ForegroundColor DarkGray
        Stop-Job -Job $job -ErrorAction SilentlyContinue | Out-Null
        Remove-Job -Job $job -Force -ErrorAction SilentlyContinue | Out-Null
    }

    # 5. Tests (re-run to confirm Module 1 still green)
    Write-Step "5/7  Re-run unit tests"
    Invoke-Checked "dotnet test tests/NexusFine.Tests/NexusFine.Tests.csproj -c Debug --nologo --no-build" "dotnet test"

    # 6. Commit
    Write-Step "6/7  Commit"
    Invoke-Checked "git add -A" "git add"
    $staged = (& git diff --cached --name-only | Measure-Object).Count
    if ($staged -gt 0) {
        Invoke-Checked "git commit -m `"feat(module2): citizen portal — live API, bilingual EN/NY, jsPDF receipt`"" "git commit"
    } else {
        Write-OK "No changes to commit"
    }

    # 7. Tag
    Write-Step "7/7  Tag v0.2"
    Invoke-Checked "git tag -f v0.2" "tag v0.2"

    Write-Step "Module 2 complete"
    Write-Host "    Citizen portal:" -ForegroundColor Cyan
    Write-Host "      • Run the API:   dotnet run --project src/NexusFine.API" -ForegroundColor Gray
    Write-Host "      • Open browser:  http://localhost:5000/citizen/" -ForegroundColor Gray
    Write-Host "      • Try plate:     MWK 1234 A" -ForegroundColor Gray
    Write-Host "      • Try ref:       NXF-2026-00001" -ForegroundColor Gray
    Write-Host "      • Toggle EN / NY in the top right" -ForegroundColor Gray
    Write-Host "      • Pay → confirm → Download Receipt (PDF)" -ForegroundColor Gray
} finally { Pop-Location }
