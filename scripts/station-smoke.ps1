# NexusFine — Station Server smoke test
#
# Boots the Station Server (port 5301), pings /api/health, fetches the
# station identity card, triggers a pull from HQ (requires HQ API on 5121),
# and pairs a fake device. Mirrors scripts/e2e-smoke.ps1 in style.
#
# Usage:
#   pwsh scripts/station-smoke.ps1            # assumes station already running
#   pwsh scripts/station-smoke.ps1 -Boot      # boots the station first
#   pwsh scripts/station-smoke.ps1 -Boot -Keep  # leaves it running

param(
    [switch]$Boot,
    [switch]$Keep
)

$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)

$StationUrl = 'http://localhost:5301'
$HqUrl      = 'http://localhost:5121'

function Hdr($t) { Write-Host "`n── $t ──" -ForegroundColor Yellow }
function Ok($t)  { Write-Host "✓ $t" -ForegroundColor Green }
function Fail($t,$code) { Write-Host "✗ $t" -ForegroundColor Red; exit $code }
function Warn($t) { Write-Host "! $t" -ForegroundColor Yellow }

$stationProc = $null
try {
    if ($Boot) {
        Hdr 'Booting Station Server'
        try { Invoke-RestMethod "$StationUrl/api/health" -TimeoutSec 2 | Out-Null; Ok 'Station already up' }
        catch {
            $stationProc = Start-Process dotnet `
                -ArgumentList @('run','--project','src/NexusFine.Station/NexusFine.Station.csproj','--launch-profile','http') `
                -PassThru -NoNewWindow `
                -RedirectStandardOutput "$env:TEMP\nexusfine-station.log" `
                -RedirectStandardError  "$env:TEMP\nexusfine-station.err"
            Write-Host "  Station PID $($stationProc.Id) — waiting for health…"
            $up = $false
            for ($i=0; $i -lt 40 -and -not $up; $i++) {
                Start-Sleep 1
                try { Invoke-RestMethod "$StationUrl/api/health" -TimeoutSec 2 | Out-Null; $up = $true } catch {}
            }
            if ($up) { Ok "Station up at $StationUrl" } else { Fail 'Station failed to start within 40s' 1 }
        }
    }

    # ── 1. Health ──
    Hdr 'Pre-flight'
    try {
        $h = Invoke-RestMethod "$StationUrl/api/health" -TimeoutSec 3
        Ok "Station healthy — $($h.stationCode) ($($h.stationName))"
    } catch { Fail "Station not reachable at $StationUrl — pass -Boot or start it manually" 1 }

    # ── 2. Identity card ──
    Hdr 'Station identity'
    $info = Invoke-RestMethod "$StationUrl/api/station/info"
    Ok "Code:     $($info.stationCode)"
    Ok "Name:     $($info.stationName)"
    Ok "HQ:       $($info.hqEndpoint)"
    Ok "Cached:   offence=$($info.cached.offenceCodes), officers=$($info.cached.officers), patrol=$($info.cached.patrolPosts), devices=$($info.cached.devices), outboundQ=$($info.cached.outboundQ)"

    # ── 3. Pull from HQ (will work only if HQ is up) ──
    Hdr 'Sync — pull reference data from HQ'
    try {
        $pull = Invoke-RestMethod "$StationUrl/api/sync/pull" -Method Post
        if ($pull.offenceCodes.success) {
            Ok "Pulled $($pull.offenceCodes.records) offence codes in $($pull.offenceCodes.millis)ms"
        } else {
            Warn "Pull failed — $($pull.offenceCodes.error)  (is the HQ API up at $HqUrl?)"
        }
    } catch {
        Warn "Pull HTTP error — $($_.Exception.Message)"
    }

    # ── 4. Pair a device ──
    Hdr 'Pair a test device'
    $serial = "DEV-TEST-$(Get-Random -Maximum 9999)"
    try {
        $pair = Invoke-RestMethod "$StationUrl/api/devices/pair" -Method Post `
            -ContentType 'application/json' `
            -Body (@{ serial = $serial } | ConvertTo-Json)
        Ok "Paired $serial — one-time token: $($pair.token.Substring(0,16))…"
    } catch {
        Fail "Device pair failed: $($_.Exception.Message)" 3
    }

    # ── 5. Heartbeat ──
    Hdr 'Heartbeat'
    $hb = Invoke-RestMethod "$StationUrl/api/devices/$serial/heartbeat" -Method Post
    Ok "Heartbeat acknowledged at $($hb.lastHeartbeatAt)"

    # ── 6. Operational push (ingest queue) ──
    Hdr 'Operational ingest'
    $body = @{
        records = @(
            @{ clientUuid = [Guid]::NewGuid().ToString();  entityType = 'Fine';     jsonPayload = '{"plate":"MW-LL-9999","amount":50000}' },
            @{ clientUuid = [Guid]::NewGuid().ToString();  entityType = 'Payment';  jsonPayload = '{"ref":"NXF-2026-0001","channel":"Cash"}' }
        )
    } | ConvertTo-Json -Depth 6
    $ing = Invoke-RestMethod "$StationUrl/api/ingest" -Method Post -ContentType 'application/json' -Body $body
    Ok "Ingested: accepted=$($ing.accepted), duplicate=$($ing.duplicate), queue depth=$($ing.queued)"

    # ── 7. Sync event log ──
    Hdr 'Recent sync events'
    $events = Invoke-RestMethod "$StationUrl/api/sync/events?limit=5"
    if ($events.Count -gt 0) {
        $events | ForEach-Object {
            Ok "[$($_.at)] $($_.entityType) $($_.direction) — $($_.outcome) ($($_.recordCount) records)"
        }
    } else {
        Warn "No sync events yet (expected if HQ pull was skipped)"
    }

    Hdr 'Summary'
    Write-Host '┌─────────────────────────────────────┐' -ForegroundColor Green
    Write-Host '│  Station smoke test PASSED ✓        │' -ForegroundColor Green
    Write-Host '└─────────────────────────────────────┘' -ForegroundColor Green
}
finally {
    if (-not $Keep) {
        if ($stationProc) { try { $stationProc.Kill(); Ok 'Station stopped' } catch {} }
    }
}
