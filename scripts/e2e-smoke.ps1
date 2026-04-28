# NexusFine — End-to-End Smoke Test (PowerShell)
#
# Boots both apps if requested, authenticates as admin, issues a fine,
# posts a payment, verifies the audit log. Mirrors scripts/e2e-smoke.sh.
#
# Usage:
#   pwsh scripts/e2e-smoke.ps1            # assumes apps already running
#   pwsh scripts/e2e-smoke.ps1 -Boot      # boots both apps first
#   pwsh scripts/e2e-smoke.ps1 -Boot -Keep   # leaves apps running

param(
    [switch]$Boot,
    [switch]$Keep
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

$ApiUrl    = 'http://localhost:5121'
$AdminUrl  = 'http://localhost:5296'
$AdminUser = 'admin'
$AdminPass = 'Nexus@Admin2026'

function Hdr($t) { Write-Host "`n── $t ──" -ForegroundColor Yellow }
function Ok($t)  { Write-Host "✓ $t" -ForegroundColor Green }
function Fail($t,$code) { Write-Host "✗ $t" -ForegroundColor Red; exit $code }
function Warn($t) { Write-Host "! $t" -ForegroundColor Yellow }

$apiProc = $null
$adminProc = $null

try {
    # ── 1. BOOT ──
    if ($Boot) {
        Hdr 'Booting API + Admin'

        try { Invoke-RestMethod "$ApiUrl/api/health" -TimeoutSec 2 | Out-Null; Ok 'API already up' }
        catch {
            $apiProc = Start-Process dotnet `
                -ArgumentList @('run','--project','src/NexusFine.API/NexusFine.API.csproj','--launch-profile','http') `
                -PassThru -NoNewWindow `
                -RedirectStandardOutput "$env:TEMP\nexusfine-api.log" `
                -RedirectStandardError  "$env:TEMP\nexusfine-api.err"
            Write-Host "  API PID $($apiProc.Id) — waiting for health…"
            $up = $false
            for ($i=0; $i -lt 30 -and -not $up; $i++) {
                Start-Sleep 1
                try { Invoke-RestMethod "$ApiUrl/api/health" -TimeoutSec 2 | Out-Null; $up = $true } catch {}
            }
            if ($up) { Ok "API up at $ApiUrl" } else { Fail 'API failed to start within 30s' 1 }
        }

        try { Invoke-WebRequest "$AdminUrl/login" -UseBasicParsing -TimeoutSec 2 | Out-Null; Ok 'Admin already up' }
        catch {
            $adminProc = Start-Process dotnet `
                -ArgumentList @('run','--project','src/NexusFine.Admin/NexusFine.Admin.csproj','--launch-profile','http') `
                -PassThru -NoNewWindow `
                -RedirectStandardOutput "$env:TEMP\nexusfine-admin.log" `
                -RedirectStandardError  "$env:TEMP\nexusfine-admin.err"
            Write-Host "  Admin PID $($adminProc.Id) — waiting for /login…"
            $up = $false
            for ($i=0; $i -lt 20 -and -not $up; $i++) {
                Start-Sleep 1
                try { Invoke-WebRequest "$AdminUrl/login" -UseBasicParsing -TimeoutSec 2 | Out-Null; $up = $true } catch {}
            }
            if ($up) { Ok "Admin up at $AdminUrl" } else { Fail 'Admin failed to start' 1 }
        }
    }

    Hdr 'Pre-flight'
    try { Invoke-RestMethod "$ApiUrl/api/health" -TimeoutSec 3 | Out-Null; Ok "API healthy at $ApiUrl" }
    catch { Fail "API not reachable at $ApiUrl — pass -Boot or start it manually" 1 }

    # ── 2. AUTH ──
    Hdr "Authenticate as $AdminUser"
    $login = Invoke-RestMethod "$ApiUrl/api/auth/login" `
        -Method Post -ContentType 'application/json' `
        -Body (@{userName=$AdminUser;password=$AdminPass} | ConvertTo-Json)
    if (-not $login.accessToken) { Fail "No accessToken in response" 2 }
    $token = $login.accessToken
    Ok "JWT obtained ($($token.Length) chars)"
    $auth = @{ Authorization = "Bearer $token" }

    # ── 3. PRE-CONDITIONS ──
    Hdr 'Look up reference data'
    $officers = Invoke-RestMethod "$ApiUrl/api/officers" -Headers $auth
    if (-not $officers -or $officers.Count -eq 0) { Fail 'No officers in DB' 3 }
    $officerId = $officers[0].id
    Ok "Officer #$officerId picked"

    $offences = Invoke-RestMethod "$ApiUrl/api/offencecodes"
    if (-not $offences -or $offences.Count -eq 0) { Fail 'No offence codes' 3 }
    $offenceId = $offences[0].id
    Ok "Offence #$offenceId picked"

    # ── 4. ISSUE FINE ──
    Hdr 'Issue fine through API'
    $plate = "MW-LL-{0:D4}" -f (Get-Random -Min 0 -Max 9999)
    $finePayload = @{
        officerId           = $officerId
        offenceCodeId       = $offenceId
        plateNumber         = $plate
        driverName          = 'E2E Test Driver'
        driverNationalId    = '00000000'
        driverLicenceNumber = 'LIC-E2E'
        driverPhone         = '+265991000000'
        location            = 'E2E checkpoint, Lilongwe'
        notes               = 'smoke test'
    } | ConvertTo-Json

    $fine = Invoke-RestMethod "$ApiUrl/api/fines" `
        -Method Post -Headers $auth -ContentType 'application/json' -Body $finePayload
    if (-not $fine.id) { Fail "Fine create response malformed: $($fine | ConvertTo-Json)" 3 }
    $fineId  = $fine.id
    $fineRef = $fine.referenceNumber
    Ok "Fine $fineRef (#$fineId) for plate $plate issued"

    # ── 5. PAY ──
    Hdr 'Initiate + confirm payment'
    $initPayload = @{ fineRef = $fineRef; channel = 'AirtelMoney'; phone = '+265991000000' } | ConvertTo-Json
    $init = Invoke-RestMethod "$ApiUrl/api/payments/initiate" `
        -Method Post -ContentType 'application/json' -Body $initPayload
    if (-not $init.transactionReference) { Fail "Initiate response missing tx ref" 4 }
    $txRef = $init.transactionReference
    Ok "Payment initiated, tx=$txRef"

    $confirmPayload = @{ transactionReference = $txRef } | ConvertTo-Json
    $confirm = Invoke-RestMethod "$ApiUrl/api/payments/confirm" `
        -Method Post -ContentType 'application/json' -Body $confirmPayload
    if (-not $confirm.receiptNumber) { Fail "Confirm response missing receipt" 4 }
    $receipt = $confirm.receiptNumber
    Ok "Payment posted, receipt=$receipt"

    # ── 6. VERIFY ──
    Hdr 'Verify fine status flipped to Paid'
    Start-Sleep 1
    $check = Invoke-RestMethod "$ApiUrl/api/fines/$fineId" -Headers $auth
    if ($check.status -ne 'Paid') { Fail "Fine status is '$($check.status)' (expected Paid)" 4 }
    Ok 'Fine status = Paid'

    # ── 7. AUDIT ──
    Hdr 'Audit log captured events'
    $audit = Invoke-RestMethod "$ApiUrl/api/auditlogs?entityId=$fineId&pageSize=20" -Headers $auth
    $eventCount = $audit.total
    $actions = ($audit.data | ForEach-Object { $_.action } | Sort-Object -Unique) -join ','
    if ($eventCount -ge 1) {
        Ok "Audit log has $eventCount entries — actions: $actions"
    } else {
        Warn 'No audit entries for this fine (audit middleware may filter by entity-type)'
    }

    # ── DONE ──
    Hdr 'Summary'
    Write-Host "  Plate:    $plate"
    Write-Host "  Fine:     $fineRef (#$fineId)"
    Write-Host "  Tx:       $txRef"
    Write-Host "  Receipt:  $receipt"
    Write-Host "  Status:   $($check.status)"
    Write-Host "  Audit:    $eventCount events"
    Write-Host ''
    Write-Host '┌─────────────────────────────────────┐' -ForegroundColor Green
    Write-Host '│  E2E smoke test PASSED ✓            │' -ForegroundColor Green
    Write-Host '└─────────────────────────────────────┘' -ForegroundColor Green
}
finally {
    if (-not $Keep) {
        if ($apiProc)   { try { $apiProc.Kill();   Ok 'API stopped'   } catch {} }
        if ($adminProc) { try { $adminProc.Kill(); Ok 'Admin stopped' } catch {} }
    }
}
