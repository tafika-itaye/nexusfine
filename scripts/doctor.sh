#!/usr/bin/env bash
# scripts/doctor.sh — Verify local toolchain for NexusFine (Git Bash on Windows).
# Usage:  ./scripts/doctor.sh
source "$(dirname "$0")/_lib.sh"

root="$(repo_root)"
step "NexusFine Doctor — $root"

missing=()

if have dotnet; then
    sdks="$(dotnet --list-sdks 2>/dev/null)"
    if echo "$sdks" | grep -q '^8\.'; then
        ok ".NET 8 SDK: $(echo "$sdks" | grep '^8\.' | head -1)"
    else
        warn2 ".NET installed but no 8.x SDK"
        missing+=(".NET 8 SDK — winget install Microsoft.DotNet.SDK.8")
    fi
    if dotnet workload list 2>/dev/null | grep -qiE 'maui|android'; then ok "MAUI workload installed"; else missing+=("MAUI workload — dotnet workload install maui-android"); fi
    if dotnet tool list -g 2>/dev/null | grep -q dotnet-ef; then ok "dotnet-ef (global)"; else missing+=("dotnet-ef tool — dotnet tool install -g dotnet-ef"); fi
else
    missing+=(".NET 8 SDK — winget install Microsoft.DotNet.SDK.8")
fi

if have sqllocaldb || have sqlcmd; then ok "SQL Server tooling present"; else missing+=("SQL Server Express / LocalDB — winget install Microsoft.SQLServer.2022.Express"); fi

if have node; then
    v="$(node --version | sed 's/^v//')"
    maj="${v%%.*}"
    if [[ "$maj" -ge 20 ]]; then ok "Node $v"; else warn2 "Node $v (>=20 recommended)"; fi
else
    missing+=("Node.js 20 LTS — winget install OpenJS.NodeJS.LTS")
fi

if have git; then ok "$(git --version)"; else missing+=("Git — winget install Git.Git"); fi

step "Summary"
if (( ${#missing[@]} == 0 )); then
    ok "All required tools present. You can run scripts/module0.sh next."
    exit 0
fi
warn2 "Missing / recommended:"
for m in "${missing[@]}"; do echo "      - $m"; done
echo
echo "    Install what's missing, then re-run: ./scripts/doctor.sh"
exit 1
