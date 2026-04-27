#!/usr/bin/env bash
# Module 3 — Admin Blazor portal verification + commit + tag.
set -euo pipefail

REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SLN="$REPO/NexusFine.sln"

C='\033[36m'; G='\033[32m'; Y='\033[33m'; R='\033[31m'; D='\033[2m'; N='\033[0m'

step() { printf "\n${C}━━ %s. %s ${D}%s${N}\n" "$1" "$2" "$(printf '%.s─' $(seq 1 $((60 - ${#2}))))"; }
ok()   { printf "  ${G}✓${N} %s\n" "$1"; }
warn() { printf "  ${Y}!${N} %s\n" "$1"; }
fail() { printf "  ${R}✗${N} %s\n" "$1" >&2; exit 1; }

SKIP_BUILD=${SKIP_BUILD:-0}
SKIP_TESTS=${SKIP_TESTS:-0}
SKIP_COMMIT=${SKIP_COMMIT:-0}

# 1 ── DOCTOR
step 1 "Doctor"
ok "dotnet $(dotnet --version)"
ok "$(git --version)"
ok "branch: $(git -C "$REPO" rev-parse --abbrev-ref HEAD)"

# 2 ── BUILD
if [[ "$SKIP_BUILD" != "1" ]]; then
    step 2 "Build"
    dotnet build "$SLN" -c Debug --nologo -v minimal
    ok "build green"
fi

# 3 ── BOOT API
step 3 "Boot API on :5121"
ASPNETCORE_URLS="http://localhost:5121" ASPNETCORE_ENVIRONMENT="Development" \
    dotnet run --no-build --project "$REPO/src/NexusFine.API/NexusFine.API.csproj" > /tmp/nexus-api.log 2>&1 &
API_PID=$!
ok "API pid $API_PID"
sleep 8

# 4 ── BOOT ADMIN
step 4 "Boot Admin on :5296"
ASPNETCORE_URLS="http://localhost:5296" ASPNETCORE_ENVIRONMENT="Development" \
    dotnet run --no-build --project "$REPO/src/NexusFine.Admin/NexusFine.Admin.csproj" > /tmp/nexus-admin.log 2>&1 &
ADMIN_PID=$!
ok "Admin pid $ADMIN_PID"
sleep 10

cleanup() {
    printf "\n${D}[cleanup] stopping background processes…${N}\n"
    kill "$API_PID" "$ADMIN_PID" 2>/dev/null || true
    wait 2>/dev/null || true
}
trap cleanup EXIT

# 5 ── SMOKE TEST
step 5 "Smoke test"

code=$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5296/login || echo 000)
[[ "$code" == "200" ]] && ok "admin /login → 200" || fail "admin /login → $code"

login_json=$(curl -s -X POST http://localhost:5121/api/auth/login \
    -H 'Content-Type: application/json' \
    -d '{"userName":"admin","password":"Nexus@Admin2026"}')
token=$(echo "$login_json" | sed -n 's/.*"accessToken":"\([^"]*\)".*/\1/p')
[[ -n "$token" ]] || fail "API login did not return accessToken"
ok "API JWT issued"

audit=$(curl -s -H "Authorization: Bearer $token" \
    'http://localhost:5121/api/auditlogs?page=1&pageSize=5')
total=$(echo "$audit" | sed -n 's/.*"total":\([0-9]*\).*/\1/p')
ok "audit log → total=$total"

types=$(curl -s -H "Authorization: Bearer $token" http://localhost:5121/api/auditlogs/entitytypes)
ok "audit entity types: $(echo "$types" | head -c 80)"

anon=$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5121/api/auditlogs)
[[ "$anon" == "401" ]] && ok "anonymous /api/auditlogs → 401 (RBAC enforced)" || warn "anonymous /api/auditlogs → $anon"

# 6 ── TESTS
if [[ "$SKIP_TESTS" != "1" ]]; then
    step 6 "Tests"
    dotnet test "$SLN" --nologo --no-build -v minimal
    ok "tests green"
fi

cleanup
trap - EXIT

# 7 ── COMMIT + TAG
if [[ "$SKIP_COMMIT" != "1" ]]; then
    step 7 "Commit + tag"
    cd "$REPO"
    git add -A
    if [[ -z "$(git status --porcelain)" ]]; then
        ok "nothing to commit"
    else
        git commit -m "feat(module3): admin portal — JWT login, RBAC gating, audit log viewer"
        ok "commit created"
    fi
    if git tag --list 'v0.3' | grep -q v0.3; then
        warn "tag v0.3 already exists — skipping"
    else
        git tag -a v0.3 -m "Module 3: admin portal w/ auth + audit"
        ok "tag v0.3"
    fi
fi

printf "\n${G}✅ Module 3 complete.${N}\n"
