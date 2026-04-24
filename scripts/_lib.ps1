# NexusFine shared PowerShell helpers.
# Dot-source from module scripts: . "$PSScriptRoot/_lib.ps1"

$ErrorActionPreference = 'Stop'

function Write-Step($msg)  { Write-Host "`n==> $msg" -ForegroundColor Cyan }
function Write-OK($msg)    { Write-Host "    OK  $msg" -ForegroundColor Green }
function Write-Warn2($msg) { Write-Host "    !!  $msg" -ForegroundColor Yellow }
function Write-Fail($msg)  { Write-Host "    XX  $msg" -ForegroundColor Red; throw $msg }

function Get-RepoRoot {
    $root = Split-Path -Parent $PSScriptRoot
    if (-not (Test-Path (Join-Path $root 'NexusFine.slnx'))) {
        Write-Fail "NexusFine.slnx not found at $root. Run scripts from the repo."
    }
    return $root
}

function Test-Command($name) {
    $null -ne (Get-Command $name -ErrorAction SilentlyContinue)
}

function Invoke-Checked([string]$cmd, [string]$label) {
    Write-Host "    > $cmd" -ForegroundColor DarkGray
    & cmd /c $cmd
    if ($LASTEXITCODE -ne 0) { Write-Fail "$label failed (exit $LASTEXITCODE)" }
}
