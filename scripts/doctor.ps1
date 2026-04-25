# scripts/doctor.ps1 — Verify local toolchain for NexusFine.
# Run:  pwsh ./scripts/doctor.ps1
. "$PSScriptRoot/_lib.ps1"

$root = Get-RepoRoot
Write-Step "NexusFine Doctor — $root"

$missing = @()

# .NET 10 (project targets net10.0 with packages 10.0.5)
if (Test-Command dotnet) {
    $sdks  = & dotnet --list-sdks
    $has10 = $sdks | Where-Object { $_ -match '^10\.' }
    if ($has10) { Write-OK ".NET 10 SDK: $($has10 | Select-Object -First 1)" }
    else {
        Write-Warn2 ".NET installed but no 10.x SDK found. Output:`n$sdks"
        $missing += '.NET 10 SDK — https://dotnet.microsoft.com/download/dotnet/10.0 (winget install Microsoft.DotNet.SDK.10)'
    }
} else {
    $missing += '.NET SDK 10 — https://dotnet.microsoft.com/download/dotnet/10.0 (winget install Microsoft.DotNet.SDK.10)'
}

# MAUI workload
if (Test-Command dotnet) {
    $wl = & dotnet workload list 2>$null
    if ($wl -match 'maui|android') { Write-OK "MAUI workload installed" }
    else { $missing += 'MAUI workload — run: dotnet workload install maui-android' }
}

# EF CLI
if (Test-Command dotnet) {
    $tools = & dotnet tool list -g 2>$null
    if ($tools -match 'dotnet-ef') { Write-OK "dotnet-ef (global)" }
    else { $missing += 'dotnet-ef tool — run: dotnet tool install -g dotnet-ef' }
}

# SQL Server / LocalDB
$sql = $false
if (Test-Command sqllocaldb) { Write-OK "SqlLocalDB: $(sqllocaldb info)"; $sql = $true }
elseif (Test-Command sqlcmd) { Write-OK "sqlcmd present"; $sql = $true }
if (-not $sql) { $missing += 'SQL Server Express or LocalDB — winget install Microsoft.SQLServer.2022.Express  (or LocalDB)' }

# Node (for optional frontend tooling / playwright)
if (Test-Command node) {
    $v = (& node --version) -replace '^v',''
    if ([version]$v -ge [version]'20.0.0') { Write-OK "Node $v" }
    else { Write-Warn2 "Node $v (>=20 recommended)" }
} else {
    $missing += 'Node.js 20 LTS — winget install OpenJS.NodeJS.LTS'
}

# Git
if (Test-Command git) { Write-OK "git $(git --version)" } else { $missing += 'Git — winget install Git.Git' }

Write-Step 'Summary'
if ($missing.Count -eq 0) {
    Write-OK 'All required tools present. You can run scripts/module0.ps1 next.'
    exit 0
}
Write-Warn2 "Missing / recommended:"
$missing | ForEach-Object { Write-Host "      - $_" -ForegroundColor Yellow }
Write-Host ""
Write-Host "    Install what's missing, then re-run: pwsh ./scripts/doctor.ps1" -ForegroundColor Yellow
exit 1
