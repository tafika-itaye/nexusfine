# NexusFine — Build Progress

**Last updated:** 2026-05-16
**Current tag:** `v0.6` (Module 4a Station Server stub + Module 5 polish)
**Demo readiness:** End-to-end demo-ready. Plan B (HQ-down → station autonomy) demonstrable.

---

## Status legend

- 🟢 **Done** — shipped, smoke-tested, committed
- 🟡 **In progress** — code in place, needs polish / test / review
- 🔴 **Pending** — not started
- ⚪ **Deferred** — explicitly out of scope until post-pilot

---

## Modules

### 🟢 Module 0 — Foundations
**Tagged:** v0.0 · **Decisions:** D-001 (stack lock), D-002 (SMS), D-003 (Telegram out), D-004 (sim payments), D-005 (7-day demo)

- 🟢 Stack locked to ASP.NET Core / SQL Server / Blazor Server / MAUI / Azure
- 🟢 Quotation cleaned: SMS line reallocated; WhatsApp bundle substituted
- 🟢 Telegram dropped from sold channels (WhatsApp sole chat channel)
- 🟢 Simulated payment gateway agreed for Minister demo
- 🟢 Scripts: `module0.ps1` + `module0.sh` (code-check, stack lock, drop SMS)
- 🟢 Doctor / boot scripts for new operators

### 🟢 Module 1 — API backend
**Tagged:** v0.1 · **Decisions:** D-008 (auth), D-009 (audit), D-010 (sim gateway)

- 🟢 ASP.NET Core 10 Web API
- 🟢 EF Core 10 + SQL Server (LocalDB)
- 🟢 Entities: `Fine`, `Payment`, `Officer`, `OffenceCode`, `Department`, `Appeal`, `AuditLog`, `AppUser`
- 🟢 Custom AppUser auth (no ASP.NET Identity) + JWT
- 🟢 Roles: `Admin`, `Supervisor`, `Officer` enforced via `[Authorize(Roles=…)]`
- 🟢 Request-level `AuditLogMiddleware` — captures POST/PUT/PATCH/DELETE
- 🟢 Audit-id extraction from `Location` header on 201 Created
- 🟢 Payment gateway abstraction with `AirtelMoney`, `TnmMpamba` simulator implementations
- 🟢 Demo seed data (initial 10 offence codes, 4 departments)
- 🟢 OpenAPI/Swagger with Bearer scheme
- 🟢 `/api/health` endpoint
- 🟢 `JsonStringEnumConverter` globally (enum-as-string)
- 🟢 Collision-safe `GenerateReference` / `GenerateReceipt` (max-of-prefix + Guid fallback)

### 🟢 Module 2 — Citizen portal
**Tagged:** v0.2

- 🟢 Bilingual EN/Chichewa via `data-i18n` driven swap
- 🟢 Hero photo + navy gradient + topographic pattern overlay
- 🟢 Lookup by plate / reference / driver licence
- 🟢 Fine detail view with status pill
- 🟢 Payment channels grid (Airtel, TNM, Bank, Card, USSD, WhatsApp) with real brand marks
- 🟢 PDF receipt generation client-side (jsPDF)
- 🟢 Authority strip footer (MPS · DRTSS · Coat of Arms)
- 🟢 Mobile-responsive
- ⚪ Production deploy to public domain — deferred to Module 6

### 🟢 Module 3 — Admin Blazor portal (core)
**Tagged:** v0.3 · **Decisions:** D-011 (JWT in circuit memory)

- 🟢 Blazor Server `net10.0`, `<Routes @rendermode="InteractiveServer" />`
- 🟢 `JwtAuthStateProvider` — circuit-scoped, parses JWT, normalises role claims
- 🟢 `NoOpAuthHandler` — permissive HTTP-layer auth (real check in `AuthorizeRouteView`)
- 🟢 `JwtBearerHandler` — attaches Bearer token to outgoing API calls
- 🟢 Bypass of `IHttpClientFactory` to fix the cross-scope token-leak bug
- 🟢 Login / Logout / AccessDenied / BlankLayout pages
- 🟢 Dashboard with KPI cards (Fines today, Revenue MTD, Collection rate, Officers on duty)
- 🟢 Live Fine Feed table
- 🟢 Officer Performance — Today (ranked, with avatars)
- 🟢 Fines page (paginated, filterable)
- 🟢 Payments page (channel/status/date filters)
- 🟢 Reconciliation page (zone breakdown, period totals)
- 🟢 Analytics page (issued-vs-collected chart, top offences, zone table)
- 🟢 Audit Log viewer (filters, JSON old/new diff)
- 🟢 Alerts feed (operational / security / officer-roster filters)
- 🟢 Settings (user profile from JWT claims, session expiry, build info)

### 🟢 Module 3.5 — HQ reference-data CRUD
**Tagged:** v0.5 · **Decisions:** D-013

- 🟢 New entities: `Station`, `PatrolPost`, `Device` (+ `DeviceStatus` enum)
- 🟢 `Officer` gains `StationId` + `PrimaryPatrolPostId` FKs (DeleteBehavior.SetNull)
- 🟢 `Department` gains `Region` field ("Northern" / "Central" / "Southern")
- 🟢 EF migration `Module35_DistrictsAndRegion` + previous `Module35_StationsPatrolPostsDevices`
- 🟢 28 districts seeded across the 3 regions
- 🟢 29 stations seeded (one per district + 1 extra in Lilongwe)
- 🟢 6 patrol posts seeded (Lilongwe / Blantyre / Mzuzu / Zomba anchors)
- 🟢 Offence codes updated to **Government Notice No. 38 of 8 May 2026** — 21 codes total
- 🟢 `StationsController` — full CRUD with soft-delete + officer/device guard
- 🟢 `PatrolPostsController` — full CRUD with soft-delete
- 🟢 `DepartmentsController` — full CRUD with rollups + soft-delete
- 🟢 `OffenceCodesController` — full CRUD with reference-aware soft-delete
- 🟢 `OfficersController` — Create + Update endpoints
- 🟢 Admin pages: `/stations`, `/patrolposts`, `/departments` (region-banded), `/offencecodes`, `/officers` with modal-based create/edit
- 🟢 Shared `Stat.razor`, `FieldRow.razor` components
- 🟢 Modal CSS (`.modal-shade`, `.modal-card`, `.form-grid`)

### 🔴 Module 4 (deferred-split) — Officer device + sync

| Sub-module | Status | Notes |
|------------|--------|-------|
| 4a — Station Server stub | 🟢 v0.6 | See dedicated section below |
| 4b — Sync engine (full) | 🔴 Pending | mTLS, batched push, conflict resolution, retry queue |
| 4c — Officer MAUI tablet | 🔴 Pending | Shared-device login switch, NFC, photo, offline queue |

#### 🟢 Module 4a — Station Server stub
**Tagged:** v0.6 · **Decisions:** D-012

- 🟢 New project `src/NexusFine.Station` — minimal API, SQLite local store
- 🟢 `StationDbContext` — narrow schema:
  - `CachedOfficer`, `CachedOffenceCode`, `CachedPatrolPost` (read replicas)
  - `PairedDevice` (hashed pairing tokens, heartbeat, revocation)
  - `QueuedRecord` (outbound queue with ClientUuid idempotency)
  - `SyncCursor` (per-entity per-direction high-water mark)
  - `SyncEvent` (audit trail of every sync attempt)
- 🟢 `HqSyncClient` — pulls offence codes from HQ, writes SyncEvent
- 🟢 Endpoints:
  - `GET /api/health` — station identity + status
  - `GET /api/station/info` — cached counts + last sync
  - `POST /api/sync/pull` — manual reference-data refresh
  - `GET /api/sync/events?limit=N` — recent sync activity
  - `POST /api/devices/pair` — mints one-time pairing token (hashed, 10-min TTL)
  - `POST /api/devices/{serial}/heartbeat` — 15-min liveness ping
  - `POST /api/ingest` — operational record queue with ClientUuid idempotency
- 🟢 Launch profile on `http://localhost:5301`
- 🟢 SQLite auto-creates `station.db` on first run
- 🟢 `scripts/station-smoke.ps1` — covers all 7 endpoints
- 🟢 Graceful degradation: works standalone when HQ unreachable (verified)
- 🟡 Swagger UI exposed at root for inspection

### 🟢 Module 5 — Citizen channels (WhatsApp + USSD)
**Tagged:** v0.5 (base) / v0.6 (polish)

- 🟢 `ChatBotController` — shared state machine for both channels, in-memory sessions
- 🟢 Full conversation flow: Welcome → Lookup → Fine list → Channel pick → Phone → Receipt
- 🟢 Real DB lookups for fines (`Fines.Include(OffenceCode)`)
- 🟢 **Chat receipts now write real `Payment` rows** (status=Completed, channel=WhatsApp/USSD)
- 🟢 **Fine status flips to Paid** with audit trail
- 🟢 Collision-safe `GenerateChatReceipt` — same MPAY-2026-NNNNN sequence as desk
- 🟢 `/api/chatbot/sessions` — JSON surface for the Live Conversations admin page
- 🟢 Activity tracking: turns, startedAt, lastActivityAt, idleSeconds, paymentPostedAt
- 🟢 `/whatsapp` admin page — full phone-bezel mockup with chat bubbles, typing indicator, quick-reply chips, scripted-demo button
- 🟢 `/ussd` admin page — feature-phone mockup with `*244#` flow, scripted-demo button
- 🟢 **`/conversations` admin page** — KPIs (Total / Active / Completed / Drop-off), channel filter, active-only filter, 5s auto-refresh, real-time idle indicator
- 🟢 Sidebar "Citizen Channels" group with WhatsApp / USSD / Live Conversations
- 🔴 Real Meta WhatsApp Business webhook wiring — deferred to post-pilot
- 🔴 Real Africa's Talking USSD aggregator wiring — deferred to post-pilot

### 🔴 Module 6 — Deploy scripts (PowerShell + Bash + Azure)

- 🔴 Azure App Service Bicep/ARM templates
- 🔴 Azure SQL provisioning script
- 🔴 Station Server image build (Windows-on-NUC or Linux-on-NUC choice)
- 🔴 SSL cert procurement (DigiCert / Let's Encrypt via Caddy)
- 🔴 Production `appsettings.Production.json` template
- 🔴 Backup / DR runbook
- 🔴 Day-0 / day-1 / day-2 operations guide

### 🟡 Module 7 — Polish & demo readiness

- 🟢 **Demo runbook** (`docs/demo-runbook.md`): 15-min Minister script, T-30 pre-flight, 5 demo beats, Plan-B recovery, Q&A prep, post-demo checklist
- 🟢 **E2E smoke test** Bash + PowerShell (`scripts/e2e-smoke.{sh,ps1}`): auth → fine → payment → status check → audit verify, exit codes 1-5 per step
- 🟢 **Station smoke test** PowerShell (`scripts/station-smoke.ps1`): health → identity → pull → pair → heartbeat → ingest → events
- 🟢 Government Notice No. 38 fines schedule alignment + `docs/road-traffic-fines-2026.md`
- 🟢 Polish pass v1 — assets, hero band, region banners, coat-of-arms gold ring
- 🟢 Polish pass v2 — coat-of-arms padding fix, sidebar accent strip, expanded avatar pool (4 portraits)
- 🟢 Updated proposal documents (`docs/proposal/`):
  - 🟢 `DRTSS_CoverLetter_2026-05-15.docx` — formal A4, MPS/DRTSS recipient, GN-38 reference, demo-ready claim
  - 🟢 `DRTSS_Quotation_2026-05-15.docx` — QTN-NXF-2026-001, MWK 218,295,000 committed, Section E optional add-ons (Station Server, solar, RAG chatbot)
- 🟡 **Backup demo video** — OBS scene list + narration script pending
- 🔴 Citizen portal hero photo licence clearance
- 🔴 Replace placeholder Alamy stock photos with TechNexus-shot imagery (post-pilot)
- 🔴 Final visual polish pass — empty-state illustrations wired in, bldg photos on Settings/About

### ⚪ Side project — Clone + rewrite ra.org.mw
Out of scope until NexusFine pilot stable.

---

## Architecture decisions log

Reference: `docs/DECISIONS.md`. Current decisions:

- D-001 — Stack locked to quoted .NET stack
- D-002 — SMS not sold as a channel; retained as a software feature
- D-003 — Telegram dropped, WhatsApp sole chat channel
- D-004 — Payments simulated for Minister demo
- D-005 — Seven-day Minister-demo scope
- D-006 — Branching policy
- D-007 — Optional RAG FAQ Chatbot (WhatsApp) as Section E add-on
- D-008 — Auth stack: custom AppUser + JWT
- D-009 — Audit log strategy: request-level middleware
- D-010 — Simulated gateway as first-class implementation
- D-011 — Admin portal auth: JWT held in Blazor circuit memory
- D-012 — Distributed three-tier topology (HQ ↔ Station ↔ Device)
- D-013 — Module 3.5 reference-data CRUD (HQ-only)

---

## Demo readiness scorecard

| Surface | Working? | Demo-able? |
|---------|----------|------------|
| Citizen portal (lookup → pay → receipt) | 🟢 | 🟢 |
| Admin dashboard (KPIs + officer ranking) | 🟢 | 🟢 |
| Admin Departments / Stations / PatrolPosts CRUD | 🟢 | 🟢 |
| Admin Officers / OffenceCodes CRUD | 🟢 | 🟢 |
| Admin Payments page (incl. chat-channel receipts) | 🟢 | 🟢 |
| Admin Reconciliation page | 🟢 | 🟢 |
| Admin Analytics with charts | 🟢 | 🟢 |
| Admin Audit Log viewer | 🟢 | 🟢 |
| Admin Alerts feed | 🟢 | 🟢 |
| WhatsApp simulator with scripted demo | 🟢 | 🟢 |
| USSD `*244#` simulator with scripted demo | 🟢 | 🟢 |
| **Live Conversations admin page** | 🟢 | 🟢 |
| Station Server (offline-tolerant tier) | 🟢 | 🟢 |
| **Plan B — kill HQ → station keeps working** | 🟢 | 🟢 |
| End-to-end smoke test (CI-quality) | 🟢 | n/a |
| Updated proposal docs (cover letter + quotation) | 🟢 | 🟢 |
| Demo runbook printed for operator | 🟢 | n/a |
| Backup demo video | 🟡 | 🟡 |
| Officer MAUI tablet app | 🔴 | 🟡 (script narrates it) |
| Treasury IFMIS integration | ⚪ | ⚪ (out of scope) |

**Grand assessment:** demo can run today. Only Module 7 backup video is a meaningful gap, and the runbook's Plan B is already demonstrable via the station-server topology.

---

## Tag history

- `v0.0` — Foundations (Module 0)
- `v0.1` — API backend (Module 1)
- `v0.2` — Citizen portal (Module 2)
- `v0.3` — Admin core (Module 3)
- `v0.5` — Module 3.5 CRUD + Module 5 base + Module 7 (runbook + smoke test)
- `v0.6` — Module 4a Station Server stub + Module 5 polish (real receipts, Live Conversations) + GN-38 alignment + proposal docs

---

## Next-up queue (in priority order)

1. 🟡 **Module 7 — Backup demo video.** OBS scene list + narration script + 1-take recording of the runbook flow. ~2 hours. Demo safety net.
2. 🔴 **Module 4b — Sync engine (initial).** Just enough to push the Station Server's outbound queue to HQ on a timer. Doesn't need mTLS for the demo. ~half day.
3. 🔴 **Module 7 — Final visual polish.** Wire `empty-state-no-data.svg` into empty tables; building photos on Settings/About; sidebar eagle watermark if you find one. ~1 hour.
4. 🔴 **Module 6 — Deploy scripts.** Only when there's a real Azure subscription + Malawi-resident region confirmation. ~half day.
5. 🔴 **Module 4c — Officer MAUI tablet.** Multi-day. Post-demo.
6. ⚪ **ra.org.mw rewrite.** Separate engagement, after pilot stabilises.

---

## Known gaps / debt

| # | Item | Severity | Notes |
|---|------|---------|-------|
| 1 | `/api/chatbot/sessions` is `[AllowAnonymous]` | Medium | Demo expedience. Add `[Authorize(Roles="Admin,Supervisor")]` before pilot. |
| 2 | Session memory is per-process | Low | Acceptable for single-tenant pilot. Move to Redis at scale. |
| 3 | Chat-side payment skips the gateway simulator | Low | Reasonable for demo. Real production flow routes through `PaymentsController.Initiate`. |
| 4 | Station-server `/api/devices/pair` is unauth'd | Medium | Add supervisor JWT gate before pilot rollout. |
| 5 | No HTTPS / mTLS yet | High for production | Demo runs over HTTP on localhost. |
| 6 | Officer MAUI app exists as csproj only — no UI | Expected | Module 4c work. |
| 7 | Old officer avatar SVGs (male-1..3, female-1..2) are placeholder garbage | Low | Excluded from rotation; can be deleted from disk. |
| 8 | `IHttpClientFactory` bypassed in admin (D-011 trade-off) | Documented | Acceptable for single-tenant scale. |

---

## How to verify everything works (12-min cold start)

```powershell
# 1. Database up
sqllocaldb start MSSQLLocalDB

# 2. Boot all three tiers (3 terminals)
dotnet run --project src/NexusFine.API/NexusFine.API.csproj         --launch-profile http
dotnet run --project src/NexusFine.Admin/NexusFine.Admin.csproj     --launch-profile http
dotnet run --project src/NexusFine.Station/NexusFine.Station.csproj --launch-profile http

# 3. Smoke-test all three (4th terminal)
pwsh .\scripts\e2e-smoke.ps1            # HQ + Admin end-to-end
pwsh .\scripts\station-smoke.ps1        # Station Server end-to-end

# 4. Browser sanity-check
#   - Admin Dashboard:        http://localhost:5296/
#   - WhatsApp simulator:     http://localhost:5296/whatsapp
#   - USSD simulator:         http://localhost:5296/ussd
#   - Live Conversations:     http://localhost:5296/conversations
#   - Citizen portal:         http://localhost:5121/citizen/
#   - Station Swagger:        http://localhost:5301/swagger
```

If all four smoke phases (e2e + station) report green and `/conversations` shows your earlier chat sessions after running the scripted WhatsApp demo, you can ship.
