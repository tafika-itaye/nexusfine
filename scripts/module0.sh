#!/usr/bin/env bash
# scripts/module0.sh — Module 0: Foundations.
# See scripts/module0.ps1 for the full description.
source "$(dirname "$0")/_lib.sh"

root="$(repo_root)"
cd "$root"

step "Module 0 — Foundations"

step "1/7  Doctor"
bash "$(dirname "$0")/doctor.sh" || fail "Doctor failed. Install missing tools and retry."

step "2/7  Clear stale .git/index.lock (if any)"
if [[ -f .git/index.lock ]]; then
    age="$(( $(date +%s) - $(stat -c %Y .git/index.lock 2>/dev/null || stat -f %m .git/index.lock) ))"
    warn2 "Found .git/index.lock (${age}s old)."
    warn2 "Close Visual Studio, VS Code git UI, or any other git process using this repo."
    read -r -p "    Press Enter after closing them to delete the lock and continue... " _
    rm -f .git/index.lock
    ok "Lock removed."
else
    ok "No stale lock."
fi

step "3/7  Normalize line endings"
run git config core.autocrlf true || true
if [[ ! -f .gitattributes ]]; then
  cat > .gitattributes <<'EOF'
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
EOF
  ok ".gitattributes written"
fi
run git add --renormalize . || true

step "4/7  Remove SmsService.cs (replaced by SmsNotificationService.cs)"
if git ls-files --error-unmatch src/NexusFine.Infrastructure/Services/SmsService.cs >/dev/null 2>&1; then
    run git rm -f src/NexusFine.Infrastructure/Services/SmsService.cs
elif [[ -f src/NexusFine.Infrastructure/Services/SmsService.cs ]]; then
    rm -f src/NexusFine.Infrastructure/Services/SmsService.cs
    ok "Deleted untracked file"
else
    ok "Already removed"
fi

step "5/7  Restore + build"
run dotnet restore NexusFine.slnx
run dotnet build NexusFine.slnx -c Debug --no-restore -nologo

step "6/7  Build passes"

step "7/7  Commit + tag"
run git add -A
if ! git diff --cached --quiet; then
    run git commit -m "feat(module0): foundations — decision log, composite notifications, scripts"
    run git tag -f v0.0
    ok "Module 0 committed and tagged v0.0"
else
    ok "No changes to commit (already up to date)"
fi
