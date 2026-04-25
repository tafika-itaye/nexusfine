# scripts/module1.ps1 — Module 1: API backend.
# - Runs doctor
# - Clears stale .git/index.lock if present (with warning)
# - Restore + build (must include new tests project)
# - Refreshes the EF migration (adds AppUser + AppUsers DbSet)
# - Updates the dev database
# - Runs unit tests (xUnit, InMemory DB)
# - Commits `feat(module1): API backend — auth, audit, simulated gateway, seed, OpenAPI, tests`
# - Tags v0.1
#
# Usage: pwsh ./scripts/module1.ps1
. "$PSScriptRoot/_lib.ps1"

$root = Get-RepoRoot
Push-Location $root
try {
    Write-Step "Module 1 — API backend (auth, RBAC, audit, simulated gateway, seed, OpenAPI, tests)"

    # 1. Doctor
    Write-Step "1/8  Doctor"
    & pwsh "$PSScriptRoot/doctor.ps1"
    if ($LASTEXITCODE -ne 0) { Write-Fail "Doctor failed. Install missing tools and retry." }

    # 2. Stale git lock
    Write-Step "2/8  Clear stale .git/index.lock (if any)"
    $lock = Join-Path $root '.git/index.lock'
    if (Test-Path $lock) {
        $age = (Get-Date) - (Get-Item $lock).LastWriteTime
        Write-Warn2 ("Found .git/index.lock (" + [math]::Round($age.TotalSeconds) + "s old).")
        Write-Warn2 "Close Visual Studio / VS Code Git UI, then press Enter to continue..."
        [void][System.Console]::ReadLine()
        Remove-Item -Force $lock
        Write-OK "Lock removed."
    } else { Write-OK "No stale lock." }

    # 3. Restore
    Write-Step "3/8  Restore"
    Invoke-Checked "dotnet restore NexusFine.slnx" "dotnet restore"

    # 4. Build
    Write-Step "4/8  Build"
    Invoke-Checked "dotnet build NexusFine.slnx -c Debug --no-restore -nologo" "dotnet build"

    # 5. EF migration — refresh so AppUser gets added
    Write-Step "5/8  EF migration (add AppUsers)"
    $apiProj = "src/NexusFine.API/NexusFine.API.csproj"
    $infProj = "src/NexusFine.Infrastructure/NexusFine.Infrastructure.csproj"

    # check whether a "Module1AddAppUsers" migration already exists
    $existing = Get-ChildItem -Recurse -Filter '*Module1AddAppUsers*' -ErrorAction SilentlyContinue
    if (-not $existing) {
        Invoke-Checked "dotnet ef migrations add Module1AddAppUsers --project `"$infProj`" --startup-project `"$apiProj`"" "ef migrations add"
    } else {
        Write-OK "Module1AddAppUsers migration already present"
    }

    Write-Step "6/8  Apply migration to local DB"
    Invoke-Checked "dotnet ef database update --project `"$infProj`" --startup-project `"$apiProj`"" "ef database update"

    # 7. Tests
    Write-Step "7/8  Run tests"
    Invoke-Checked "dotnet test tests/NexusFine.Tests/NexusFine.Tests.csproj -c Debug --nologo" "dotnet test"

    # 8. Commit + tag
    Write-Step "8/8  Commit + tag"
    Invoke-Checked "git add -A" "git add"
    $staged = (& git diff --cached --name-only | Measure-Object).Count
    if ($staged -gt 0) {
        Invoke-Checked "git commit -m `"feat(module1): API backend — auth, RBAC, audit, simulated gateway, seed, OpenAPI, tests`"" "git commit"
        Invoke-Checked "git tag -f v0.1" "tag v0.1"
        Write-OK "Module 1 committed and tagged v0.1"
    } else {
        Write-OK "No changes to commit (already up to date)"
    }

    Write-Step "Module 1 complete"
    Write-Host "    API surface:" -ForegroundColor Cyan
    Write-Host "      • POST /api/auth/login        (admin / Nexus@Admin2026)" -ForegroundColor Gray
    Write-Host "      • POST /api/auth/login        (supervisor / Nexus@Super2026)" -ForegroundColor Gray
    Write-Host "      • GET  /api/offencecodes      (public)" -ForegroundColor Gray
    Write-Host "      • GET  /api/fines/lookup      (public citizen lookup)" -ForegroundColor Gray
    Write-Host "      • POST /api/fines             (Officer/Supervisor/Admin)" -ForegroundColor Gray
    Write-Host "      • GET  /api/dashboard/*       (Supervisor/Admin)" -ForegroundColor Gray
    Write-Host "      • POST /api/payments/initiate (public — simulated)" -ForegroundColor Gray
    Write-Host "      • swagger: /swagger" -ForegroundColor Gray
} finally { Pop-Location }
