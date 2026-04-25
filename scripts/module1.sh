#!/usr/bin/env bash
# scripts/module1.sh — Module 1: API backend.
# Usage:  ./scripts/module1.sh
source "$(dirname "$0")/_lib.sh"

root="$(repo_root)"
cd "$root"

step "Module 1 — API backend (auth, RBAC, audit, simulated gateway, seed, OpenAPI, tests)"

# 1. Doctor
step "1/8  Doctor"
"$root/scripts/doctor.sh" || fail "Doctor failed. Install missing tools and retry."

# 2. Stale git lock
step "2/8  Clear stale .git/index.lock (if any)"
if [[ -f "$root/.git/index.lock" ]]; then
    warn2 "Found .git/index.lock. Close Visual Studio / VS Code Git UI, then press Enter to continue..."
    read -r
    rm -f "$root/.git/index.lock"
    ok "Lock removed."
else
    ok "No stale lock."
fi

# 3. Restore
step "3/8  Restore"
run dotnet restore NexusFine.slnx

# 4. Build
step "4/8  Build"
run dotnet build NexusFine.slnx -c Debug --no-restore -nologo

# 5. EF migration — add AppUsers
step "5/8  EF migration (add AppUsers)"
api_proj="src/NexusFine.API/NexusFine.API.csproj"
inf_proj="src/NexusFine.Infrastructure/NexusFine.Infrastructure.csproj"

if ! find . -iname '*Module1AddAppUsers*' -print -quit | grep -q .; then
    run dotnet ef migrations add Module1AddAppUsers --project "$inf_proj" --startup-project "$api_proj"
else
    ok "Module1AddAppUsers migration already present"
fi

step "6/8  Apply migration to local DB"
run dotnet ef database update --project "$inf_proj" --startup-project "$api_proj"

# 7. Tests
step "7/8  Run tests"
run dotnet test tests/NexusFine.Tests/NexusFine.Tests.csproj -c Debug --nologo

# 8. Commit + tag
step "8/8  Commit + tag"
run git add -A
staged=$(git diff --cached --name-only | wc -l | tr -d ' ')
if [[ "$staged" -gt 0 ]]; then
    run git commit -m "feat(module1): API backend — auth, RBAC, audit, simulated gateway, seed, OpenAPI, tests"
    run git tag -f v0.1
    ok "Module 1 committed and tagged v0.1"
else
    ok "No changes to commit (already up to date)"
fi

step "Module 1 complete"
echo "    API surface:"
echo "      • POST /api/auth/login        (admin / Nexus@Admin2026)"
echo "      • POST /api/auth/login        (supervisor / Nexus@Super2026)"
echo "      • GET  /api/offencecodes      (public)"
echo "      • GET  /api/fines/lookup      (public citizen lookup)"
echo "      • POST /api/fines             (Officer/Supervisor/Admin)"
echo "      • GET  /api/dashboard/*       (Supervisor/Admin)"
echo "      • POST /api/payments/initiate (public — simulated)"
echo "      • swagger: /swagger"
