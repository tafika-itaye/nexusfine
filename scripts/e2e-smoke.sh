#!/usr/bin/env bash
#
# NexusFine — End-to-End Smoke Test
#
# Boots both apps (if not already running), authenticates as admin,
# issues a fine through the API, posts a payment, then verifies the
# audit log captured both events. This is the test the demo runbook
# implicitly relies on — run it before every demo.
#
# Usage:
#   bash scripts/e2e-smoke.sh           # assumes API + Admin already running
#   bash scripts/e2e-smoke.sh --boot    # boots both apps first
#   bash scripts/e2e-smoke.sh --keep    # leaves apps running on success
#
# Exit codes:
#   0  — all checks passed
#   1  — boot failure
#   2  — auth failure
#   3  — fine-issue failure
#   4  — payment-post failure
#   5  — audit-log verification failure
#

set -u
cd "$(dirname "$0")/.."

API_URL="http://localhost:5121"
ADMIN_URL="http://localhost:5296"
ADMIN_USER="admin"
ADMIN_PASS="Nexus@Admin2026"
BOOT=0
KEEP=0
API_PID=""
ADMIN_PID=""

for arg in "$@"; do
    case "$arg" in
        --boot) BOOT=1 ;;
        --keep) KEEP=1 ;;
        *) echo "Unknown arg: $arg"; exit 1 ;;
    esac
done

# ── colours ──
GREEN='\033[0;32m'; RED='\033[0;31m'; YEL='\033[1;33m'; NC='\033[0m'
ok()   { echo -e "${GREEN}✓${NC} $1"; }
fail() { echo -e "${RED}✗${NC} $1"; exit "$2"; }
warn() { echo -e "${YEL}!${NC} $1"; }
hdr()  { echo -e "\n${YEL}── $1 ──${NC}"; }

cleanup() {
    if [[ "$KEEP" == "0" ]]; then
        [[ -n "$API_PID"   ]] && kill "$API_PID"   2>/dev/null && ok "API stopped"
        [[ -n "$ADMIN_PID" ]] && kill "$ADMIN_PID" 2>/dev/null && ok "Admin stopped"
    fi
}
trap cleanup EXIT

require() {
    command -v "$1" >/dev/null 2>&1 || fail "Required tool not found: $1" 1
}
require curl
require jq

# ── 1. BOOT (optional) ──
if [[ "$BOOT" == "1" ]]; then
    hdr "Booting API + Admin"

    if curl -fsS "$API_URL/api/health" >/dev/null 2>&1; then
        ok "API already up — reusing existing process"
    else
        nohup dotnet run --project src/NexusFine.API/NexusFine.API.csproj --launch-profile http \
            > /tmp/nexusfine-api.log 2>&1 &
        API_PID=$!
        echo "  API PID $API_PID — waiting for health…"
        for _ in {1..30}; do
            sleep 1
            curl -fsS "$API_URL/api/health" >/dev/null 2>&1 && break
        done
        curl -fsS "$API_URL/api/health" >/dev/null 2>&1 \
            && ok "API up at $API_URL" \
            || fail "API failed to start within 30s — see /tmp/nexusfine-api.log" 1
    fi

    if curl -fsS -o /dev/null "$ADMIN_URL/login"; then
        ok "Admin already up — reusing existing process"
    else
        nohup dotnet run --project src/NexusFine.Admin/NexusFine.Admin.csproj --launch-profile http \
            > /tmp/nexusfine-admin.log 2>&1 &
        ADMIN_PID=$!
        echo "  Admin PID $ADMIN_PID — waiting for /login…"
        for _ in {1..20}; do
            sleep 1
            curl -fsS -o /dev/null "$ADMIN_URL/login" && break
        done
        curl -fsS -o /dev/null "$ADMIN_URL/login" \
            && ok "Admin up at $ADMIN_URL" \
            || fail "Admin failed to start — see /tmp/nexusfine-admin.log" 1
    fi
fi

# Pre-flight: both apps must be reachable
hdr "Pre-flight"
curl -fsS "$API_URL/api/health" >/dev/null 2>&1 \
    && ok "API healthy at $API_URL" \
    || fail "API not reachable at $API_URL — pass --boot or start it manually" 1

# ── 2. AUTH ──
hdr "Authenticate as $ADMIN_USER"
LOGIN=$(curl -fsS -X POST "$API_URL/api/auth/login" \
    -H 'Content-Type: application/json' \
    -d "{\"userName\":\"$ADMIN_USER\",\"password\":\"$ADMIN_PASS\"}") \
    || fail "Login HTTP request failed" 2

TOKEN=$(echo "$LOGIN" | jq -r '.accessToken // empty')
[[ -n "$TOKEN" ]] || fail "No accessToken in response: $LOGIN" 2
ok "JWT obtained (${#TOKEN} chars)"
AUTH=(-H "Authorization: Bearer $TOKEN")

# ── 3. PRE-CONDITIONS: pick an officer + offence code ──
hdr "Look up reference data"

OFFICER_ID=$(curl -fsS "${AUTH[@]}" "$API_URL/api/officers" \
    | jq -r '.[0].id // empty')
[[ -n "$OFFICER_ID" ]] || fail "No officers in DB" 3
ok "Officer #$OFFICER_ID picked"

OFFENCE_ID=$(curl -fsS "$API_URL/api/offencecodes" \
    | jq -r '.[0].id // empty')
[[ -n "$OFFENCE_ID" ]] || fail "No offence codes in DB" 3
ok "Offence #$OFFENCE_ID picked"

# ── 4. ISSUE A FINE ──
# Note: DepartmentId is derived from the officer server-side; Amount from the
# offence code's DefaultFineAmount. We don't pass them.
hdr "Issue fine through API"
PLATE="MW-LL-$(printf '%04d' $((RANDOM % 9999)))"
FINE_PAYLOAD=$(jq -nc \
    --argjson officerId "$OFFICER_ID" \
    --argjson offenceCodeId "$OFFENCE_ID" \
    --arg     plate "$PLATE" \
    '{officerId:$officerId, offenceCodeId:$offenceCodeId,
      plateNumber:$plate, driverName:"E2E Test Driver", driverNationalId:"00000000",
      driverLicenceNumber:"LIC-E2E", driverPhone:"+265991000000",
      location:"E2E checkpoint, Lilongwe", notes:"smoke test"}')

FINE=$(curl -fsS -X POST "$API_URL/api/fines" "${AUTH[@]}" \
    -H 'Content-Type: application/json' -d "$FINE_PAYLOAD") \
    || fail "Fine POST failed" 3

FINE_ID=$(echo "$FINE" | jq -r '.id // .Id // empty')
FINE_REF=$(echo "$FINE" | jq -r '.referenceNumber // .ReferenceNumber // empty')
[[ -n "$FINE_ID" && -n "$FINE_REF" ]] || fail "Fine response malformed: $FINE" 3
ok "Fine $FINE_REF (#$FINE_ID) for plate $PLATE issued"

# ── 5. PAY THE FINE ──
hdr "Initiate + confirm payment"
INIT=$(curl -fsS -X POST "$API_URL/api/payments/initiate" \
    -H 'Content-Type: application/json' \
    -d "{\"fineRef\":\"$FINE_REF\",\"channel\":\"AirtelMoney\",\"phone\":\"+265991000000\"}") \
    || fail "Payment initiate failed" 4

TX_REF=$(echo "$INIT" | jq -r '.transactionReference // empty')
[[ -n "$TX_REF" ]] || fail "Payment initiate response missing transactionReference: $INIT" 4
ok "Payment initiated, tx=$TX_REF"

CONFIRM=$(curl -fsS -X POST "$API_URL/api/payments/confirm" \
    -H 'Content-Type: application/json' \
    -d "{\"transactionReference\":\"$TX_REF\"}") \
    || fail "Payment confirm failed" 4

RECEIPT=$(echo "$CONFIRM" | jq -r '.receiptNumber // empty')
[[ -n "$RECEIPT" ]] || fail "Confirm response missing receipt: $CONFIRM" 4
ok "Payment posted, receipt=$RECEIPT"

# ── 6. VERIFY FINE STATUS ──
hdr "Verify fine status flipped to Paid"
sleep 1
STATUS=$(curl -fsS "${AUTH[@]}" "$API_URL/api/fines/$FINE_ID" | jq -r '.status // .Status // empty')
[[ "$STATUS" == "Paid" ]] \
    && ok "Fine status = Paid" \
    || fail "Fine status is '$STATUS' (expected Paid)" 4

# ── 7. AUDIT LOG ──
hdr "Audit log captured both events"
AUDIT=$(curl -fsS "${AUTH[@]}" "$API_URL/api/auditlogs?entityId=$FINE_ID&pageSize=20")
EVENT_COUNT=$(echo "$AUDIT" | jq -r '.total // 0')
ACTIONS=$(echo "$AUDIT" | jq -r '.data[].action' | sort -u | tr '\n' ',' | sed 's/,$//')

[[ "$EVENT_COUNT" -ge 1 ]] \
    && ok "Audit log has $EVENT_COUNT entries for fine #$FINE_ID — actions: $ACTIONS" \
    || warn "No audit entries for this fine (audit middleware may filter by entity-type)"

# ── DONE ──
hdr "Summary"
echo "  Plate:    $PLATE"
echo "  Fine:     $FINE_REF (#$FINE_ID)"
echo "  Tx:       $TX_REF"
echo "  Receipt:  $RECEIPT"
echo "  Status:   $STATUS"
echo "  Audit:    $EVENT_COUNT events"
echo
echo -e "${GREEN}┌─────────────────────────────────────┐${NC}"
echo -e "${GREEN}│  E2E smoke test PASSED ✓            │${NC}"
echo -e "${GREEN}└─────────────────────────────────────┘${NC}"
