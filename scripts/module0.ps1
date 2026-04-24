# scripts/module0.ps1 — Module 0: Foundations.
# - Runs doctor
# - Clears stale .git/index.lock if present (with warning)
# - Normalizes line endings (resolves the 96-file CRLF/LF diff)
# - Removes SmsService.cs from git (file already emptied to tombstone in source)
# - Restores, builds
# - Commits with "feat(module0): foundations" and tags v0.0
#
# Usage: pwsh ./scripts/module0.ps1
. "$PSScriptRoot/_lib.ps1"

$root = Get-RepoRoot
Push-Location $root
try {
    Write-Step "Module 0 — Foundations"

    # Doctor
    Write-Step "1/7  Doctor"
    & pwsh "$PSScriptRoot/doctor.ps1"
    if ($LASTEXITCODE -ne 0) { Write-Fail "Doctor failed. Install missing tools and retry." }

    # Stale git lock
    Write-Step "2/7  Clear stale .git/index.lock (if any)"
    $lock = Join-Path $root '.git/index.lock'
    if (Test-Path $lock) {
        $age = (Get-Date) - (Get-Item $lock).LastWriteTime
        Write-Warn2 ("Found .git/index.lock (" + [math]::Round($age.TotalSeconds) + "s old).")
        Write-Warn2 "This usually means Visual Studio, VS Code Git UI, or another git process is open."
        Write-Warn2 "Close them now, then press Enter to delete the lock and continue..."
        [void][System.Console]::ReadLine()
        Remove-Item -Force $lock
        Write-OK "Lock removed."
    } else { Write-OK "No stale lock." }

    # Line endings
    Write-Step "3/7  Normalize line endings"
    Invoke-Checked "git config core.autocrlf true" "git config autocrlf"
    $attr = Join-Path $root '.gitattributes'
    if (-not (Test-Path $attr)) {
        @"
* text=auto eol=lf
*.cs        text diff=csharp
*.csproj    text merge=union
*.sln       text eol=crlf
*.ps1       text eol=crlf
*.sh        text eol=lf
*.razor     text
*.xaml      text
*.json      text
*.html      text
*.css       text
*.md        text
*.png binary
*.jpg binary
*.docx binary
*.pdf binary
"@ | Set-Content -NoNewline -Encoding UTF8 $attr
        Write-OK ".gitattributes written"
    }
    Invoke-Checked "git add --renormalize ." "renormalize"

    # Remove SmsService.cs tombstone
    Write-Step "4/7  Remove SmsService.cs (replaced by SmsNotificationService.cs)"
    $sms = "src/NexusFine.Infrastructure/Services/SmsService.cs"
    if (git ls-files $sms) {
        Invoke-Checked "git rm -f `"$sms`"" "git rm SmsService.cs"
    } elseif (Test-Path $sms) {
        Remove-Item -Force $sms
        Write-OK "Deleted untracked $sms"
    } else { Write-OK "Already removed" }

    # Restore + build
    Write-Step "5/7  Restore + build"
    Invoke-Checked "dotnet restore NexusFine.slnx" "dotnet restore"
    Invoke-Checked "dotnet build NexusFine.slnx -c Debug --no-restore -nologo" "dotnet build"

    Write-Step "6/7  Build passes"

    # Commit + tag
    Write-Step "7/7  Commit + tag"
    Invoke-Checked "git add -A" "git add"
    $staged = (& git diff --cached --name-only | Measure-Object).Count
    if ($staged -gt 0) {
        Invoke-Checked "git commit -m `"feat(module0): foundations — decision log, composite notifications, scripts`"" "git commit"
        Invoke-Checked "git tag -f v0.0" "tag v0.0"
        Write-OK "Module 0 committed and tagged v0.0"
    } else {
        Write-OK "No changes to commit (already up to date)"
    }
} finally { Pop-Location }
