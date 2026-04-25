#!/usr/bin/env bash
# scripts/module2.sh — Module 2: Citizen web portal.
# Mirrors module2.ps1.
#
# Usage: ./scripts/module2.sh
source "$(dirname "$0")/_lib.sh"

ROOT="$(repo_root)"
cd "$ROOT"

step "Module 2 — Citizen web portal (live, bilingual EN/NY, PDF receipt)"

# 1. Doctor
step "1/7  Doctor"
bash "$ROOT/scripts/doctor.sh"

# 2. Verify portal files
step "2/7  Verify wwwroot files"
for f in \
    "src/NexusFine.API/wwwroot/citizen/index.html" \
    "src/NexusFine.API/wwwroot/citizen/i18n.js" \
    "src/NexusFine.API/wwwroot/citizen/app.js" \
    "src/NexusFine.API/wwwroot/css/theme.css"
do
    [[ -f "$ROOT/$f" ]] || fail "Missing: $f"
    ok "$f"
done

# 3. Build
step "3/7  Build"
run dotnet build NexusFine.slnx -c Debug --nologo

# 4. Boot API + smoke-test
step "4/7  Boot API + smoke-test endpoints"
PORT=5080
BASE="http://localhost:$PORT"
LOG="$(mktemp)"

ASPNETCORE_URLS="$BASE" \
ASPNETCORE_ENVIRONMENT=Development \
    dotnet run --project src/NexusFine.API/NexusFine.API.csproj --no-build --no-launch-profile \
    >"$LOG" 2>&1 &
API_PID=$!

cleanup() {
    if kill -0 "$API_PID" 2>/dev/null; then
        kill "$API_PID" 2>/dev/null || true
        wait "$API_PID" 2>/dev/null || true
    fi
}
trap cleanup EXIT

printf '    waiting for API at %s ...\n' "$BASE"
UP=0
for _ in $(seq 1 30); do
    sleep 1
    if curl -fsS "$BASE/api/offencecodes" >/dev/null 2>&1; then
        UP=1; break
    fi
done
[[ "$UP" -eq 1 ]] || { sed 's/^/    /' "$LOG"; fail "API did not respond on $BASE within 30s."; }
ok "API up at $BASE"

curl -fsS "$BASE/citizen/"          | grep -q 'NEXUS' && ok "GET /citizen/  (200, HTML)"  || fail "/citizen/ HTML missing"
curl -fsS "$BASE/citizen/i18n.js"   >/dev/null         && ok "GET /citizen/i18n.js (200)"  || fail "/citizen/i18n.js"
curl -fsS "$BASE/citizen/app.js"    >/dev/null         && ok "GET /citizen/app.js (200)"   || fail "/citizen/app.js"
curl -fsS "$BASE/css/theme.css"     >/dev/null         && ok "GET /css/theme.css (200)"    || fail "/css/theme.css"
curl -fsS "$BASE/api/offencecodes"  >/dev/null         && ok "GET /api/offencecodes (200)" || fail "/api/offencecodes"

if curl -fsS "$BASE/api/fines/lookup?type=ref&value=NXF-2026-00001" >/dev/null 2>&1; then
    ok "GET /api/fines/lookup?type=ref&value=NXF-2026-00001 (200)"
else
    warn2 "lookup endpoint did not return 200 — seed may not contain NXF-2026-00001; continuing."
fi

cleanup
trap - EXIT

# 5. Tests
step "5/7  Re-run unit tests"
run dotnet test tests/NexusFine.Tests/NexusFine.Tests.csproj -c Debug --nologo --no-build

# 6. Commit
step "6/7  Commit"
run git add -A
if [[ -n "$(git diff --cached --name-only)" ]]; then
    run git commit -m "feat(module2): citizen portal — live API, bilingual EN/NY, jsPDF receipt"
else
    ok "No changes to commit"
fi

# 7. Tag
step "7/7  Tag v0.2"
run git tag -f v0.2

step "Module 2 complete"
cat <<EOF
    Citizen portal:
      • Run the API:   dotnet run --project src/NexusFine.API
      • Open browser:  http://localhost:5000/citizen/
      • Try plate:     MWK 1234 A
      • Try ref:       NXF-2026-00001
      • Toggle EN / NY in the top right
      • Pay → confirm → Download Receipt (PDF)
EOF
