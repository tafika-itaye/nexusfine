# NexusFine — Decision Log

A running record of architecture and scope decisions. Each entry is immutable once
shipped to `main`; a later decision supersedes an earlier one by reference, never by edit.

---

## 2026-04-24 · D-001 — Stack locked to quoted .NET stack

**Context.** Two sources of truth were in conflict: the signed DRTSS quotation
(MWK 157.5M software) names ASP.NET Core 8, SQL Server, Blazor Server, .NET MAUI,
and Microsoft Azure. A newer concept document proposed Node/Spring/PostgreSQL/
Kubernetes/Terraform.

**Decision.** The quoted stack governs. The concept document's stack section is
retired.

**Consequence.**
- Backend: ASP.NET Core 8 Web API
- Data: SQL Server (LocalDB / Express for dev, Azure SQL for prod)
- Admin: Blazor Server
- Officer app: .NET MAUI (Android target)
- Citizen web: static HTML + progressive enhancement, served by the API or Azure Static Web Apps
- Hosting: Microsoft Azure App Service

---

## 2026-04-24 · D-002 — SMS not sold as a channel; retained as a software feature (revised)

**Context.** Initial direction was "SMS removed completely." Clarified on
follow-up: the **software** still ships SMS and email notification features
(so other ministries or later phases can enable them at zero extra build cost);
we simply do not **sell an SMS bundle** in the DRTSS pilot.

**Decision.**
- Software: keep SMS + email as enableable notification channels via a Composite.
- Quotation: (A) remove "SMS & Email Notifications" line; reallocate its
  MWK 5,250,000 across the remaining Software Development items so A subtotal
  returns to MWK 157,500,000. (C) Replace "SMS bundle — 200,000 messages" with
  "WhatsApp conversation bundle — 200,000 sessions" at the same price. Rename
  "USSD + SMS setup fee" to "USSD setup fee".
- **Grand Total stays MWK 218,295,000** (reallocation preserves revenue).

**Consequence (code).**
- `SmsService.cs` removed from source; replaced by `SmsNotificationService.cs`
  (same behaviour, standard interface).
- New: `INotificationService`, `WhatsAppNotificationService`,
  `SmsNotificationService`, `EmailNotificationService`,
  `CompositeNotificationService` (fans out to every enabled channel; per-channel
  failure is isolated).
- `appsettings.json`: `WhatsApp:Enabled=true`, `Sms:Enabled=false`,
  `Email:Enabled=false` by default. Flip flags to enable, no code change.
- `PaymentsController` depends on `INotificationService` (resolves to the composite).

**Consequence (commercial).**
- `docs/quotation/DRTSS_Quotation_v2_2026-04-24.docx` reflects the above.
- New Section E (optional add-ons) introduced — see D-007.

---

## 2026-04-24 · D-003 — Telegram dropped, WhatsApp is the sole chat channel

**Context.** Concept document lists both WhatsApp and Telegram. Quotation names WhatsApp only.

**Decision.** WhatsApp Business API is the only chat channel for Phase 1.
Telegram is not built, not quoted, not demoed.

---

## 2026-04-24 · D-004 — Payments simulated for the Minister demo

**Context.** Real Airtel Money / TNM Mpamba / bank gateway credentials are not
available in time for the demo, and running real gateways on stage carries
reputational risk.

**Decision.** `AirtelMoneyService` and `MpambaService` run in **simulated mode**
when their configured credentials contain `REPLACE_` placeholders. They return
deterministic success receipts with realistic latency. A server-side flag
`ApiSettings:PaymentMode` = `Simulated|Live` overrides this.

**Consequence.** Zero external dependency during the Minister demo.
Real credentials flip a config value post-contract, no code change required.

---

## 2026-04-24 · D-005 — Seven-day Minister-demo scope

**Context.** Client gave a one-week runway to a Minister presentation.

**Decision.** Scope for the 7-day cut is:
1. Officer issues a fine on MAUI (NFC + photo + offline sync)
2. Citizen looks up + pays on the portal (simulated)
3. Admin dashboard updates live
4. WhatsApp + USSD walkthrough via a demo chat widget hitting the real backend
5. Production-grade auth, audit, and seeded data

Anything in the concept document beyond the above (Elasticsearch, Kubernetes,
Terraform, Prometheus/Grafana, ELK, multi-region) is Phase 2+, not Phase 1.

---

## 2026-04-24 · D-007 — Optional RAG FAQ Chatbot (WhatsApp) module

**Context.** Client asked for an optional add-on: a RAG chatbot over
Authority-provided documents, delivered through the existing WhatsApp channel.

**Decision.** Add Section E "Optional Add-On Modules" to the DRTSS quotation.
Priced at **MWK 8,750,000** one-time (matches the Mobile Money integration
line as a calibration anchor), with monthly LLM API consumption billed at
cost pass-through.

**Scope included in the fee.**
- Document ingestion pipeline (PDFs, Word, policies, FAQs)
- Vector store (pgvector on SQL Server's FILESTREAM, Qdrant, or Pinecone)
- Retrieval + LLM integration (Azure OpenAI / Anthropic)
- WhatsApp webhook handler with session state
- Admin UI to upload and manage documents
- Usage analytics
- Bilingual responses (English / Chichewa)

**Out of scope / pass-through.** LLM API consumption charges.

---

## 2026-04-24 · D-006 — Branching policy

**Context.** User chose to work on `main`, commit per module.

**Decision.** Each module ships as one squashed commit to `main`:
`feat(moduleN): <title>`. No long-lived feature branches. Tags cut at
`v0.N` after each module completes its verification step.

---

## 2026-04-24 · D-008 — Auth stack: custom AppUser + JWT (no ASP.NET Identity)

**Context.** Module 1 needs login, RBAC, and password verification for the
admin portal, officer app, and API. Full ASP.NET Identity adds ~12 tables and
framework coupling we do not need for the MPS/DRTSS scope.

**Decision.**
- Custom `AppUser` entity with: `UserName`, `Email`, `FullName`,
  `PasswordHash`, `PasswordSalt`, `Roles` (comma-separated), optional
  `OfficerId` link, `IsActive`, `LastLoginAt`.
- `PasswordHasher`: PBKDF2-SHA256, 210,000 iterations, 16-byte salt, 32-byte
  hash. Constant-time verification.
- `JwtTokenService`: HS256, configurable issuer/audience/secret, access token
  expiry 120 min, refresh token 14 days. Refresh token storage added in
  Module 3 when the admin portal lands.
- Roles: `Admin`, `Supervisor`, `Officer` (constants in `AppRoles`).
- Default seeded accounts for demo:
  - `admin / Nexus@Admin2026` (Admin)
  - `supervisor / Nexus@Super2026` (Supervisor + Admin)
- Controllers gate by role:
  - `DashboardController` — Supervisor, Admin
  - `FinesController` — `[Authorize]`, POST limited to Officer/Supervisor/Admin;
    lookup is `AllowAnonymous` (citizen portal)
  - `OfficersController` — `[Authorize]`
  - `PaymentsController` — `AllowAnonymous` (citizen + gateway callbacks)
  - `OffenceCodesController.GetAll` — `AllowAnonymous`

**Consequence.** No Identity dependency, small schema delta, same security
posture. If the ministry ever requires SCIM / Azure AD B2C, swap the JWT
issuer for an OIDC scheme; `[Authorize(Roles=...)]` attributes remain valid.

**Rotate before production.** `Jwt:Secret` in `appsettings.json` is a
DEV-ONLY value. Production deployments must override via user-secrets,
Azure Key Vault, or environment variable.

---

## 2026-04-24 · D-009 — Audit log strategy: request-level middleware

**Context.** Pilot requires "who did what when" for every mutating admin /
officer action.

**Decision.** `AuditLogMiddleware` writes an `AuditLog` row after every
POST/PUT/PATCH/DELETE to `/api/*`. It records:

- `EntityType` derived from the path (`/api/fines/...` → `fines`)
- `EntityId` from the path if numeric
- `Action` = HTTP method
- `NewValues` = JSON snapshot of method, path, query, response status
- `UserId` from JWT subject claim
- `IpAddress` from remote endpoint
- `Timestamp`

`GET` requests are not audited; we rely on application logs for reads.
Per-entity before/after diffs are added in Module 3 for the admin fine-edit
screens using EF Core change tracker events.

---

## 2026-04-24 · D-010 — Simulated gateway as a first-class implementation

**Context.** D-004 locked payments to simulated for the Minister demo. The
first implementation lived inside `AirtelMoneyService` / `MpambaService`
behind a `REPLACE_` placeholder check, which muddles real + fake code paths.

**Decision.** Introduce `SimulatedPaymentGateway : IPaymentGateway` as a
dedicated implementation. `PaymentGatewayFactory` now reads
`ApiSettings:PaymentMode`:
- `Simulated` → always returns `SimulatedPaymentGateway` regardless of channel
- `Live`     → routes by `PaymentChannel` to the real services

`SimulatedPaymentGateway` is deterministic, logs every call, and parses the
minimal JSON payload emitted by the citizen portal's "confirm" flow. It ships
with xUnit coverage.

**Consequence.** Flip `PaymentMode` to `Live` on the server post-contract —
no code change. Real and simulated paths can also coexist if we ever want to
A/B test with a subset of users.

---

## 2026-04-25 · D-011 — Admin portal auth: JWT held in Blazor circuit memory

**Context.** Module 3 ships a Blazor Server admin portal that must authenticate
against the existing API JWT endpoint. Two options were considered:

1. **Circuit memory only.** JWT lives in the scoped `JwtAuthStateProvider`;
   page reload kicks the user back to `/login`.
2. **ProtectedSessionStorage rehydration.** JWT also persisted in encrypted
   browser storage; reloads silently restore the principal in
   `OnAfterRenderAsync`.

**Decision.** Ship option 1 for the Minister demo. Option 2 is deferred.

**Rationale.**
- Demo flow does not exercise reloads — operators sign in once and stay.
- Eliminates an entire class of bugs (storage-vs-circuit drift, double-init,
  stale tokens surviving a server restart).
- ProtectedSessionStorage rehydration adds prerender-render race conditions
  that are awkward to debug under demo time pressure.
- Refresh-token use is also deferred for the same reason; access token TTL
  of 120 minutes is comfortably longer than any demo session.

**Implementation.**
- `JwtAuthStateProvider : AuthenticationStateProvider` — scoped per circuit,
  parses the JWT with `JwtSecurityTokenHandler`, normalises legacy `"role"`
  claims to `ClaimTypes.Role`, exposes `AccessToken` for the HTTP handler.
- `JwtBearerHandler : DelegatingHandler` — injects `Authorization: Bearer …`
  into every API call from the typed `HttpClient`.
- `Routes.razor` wraps `<AuthorizeRouteView>`; `<NotAuthorized>` redirects
  unauthenticated users to `/login` and authenticated-but-wrong-role users
  to `/access-denied`.
- All admin pages carry `@attribute [Authorize(Roles="Admin,Supervisor")]`.
- `NavMenu.razor` user pill renders from JWT claims via `<AuthorizeView>`,
  with a Sign-out link to `/logout`.

**Post-demo follow-up.** Add a `ProtectedSessionStorage` rehydration step in
`JwtAuthStateProvider.OnAfterRenderAsync` and wire the API's existing
`refreshToken` field to a `/api/auth/refresh` endpoint. Tracked under the
"Polish" module (Module 7).

---

## 2026-04-27 · D-012 — Distributed three-tier topology (HQ ↔ Station ↔ Device)

**Context.** Single-node assumption baked into Modules 0–3 was insufficient.
Real deployment needs offline-tolerant operation at rural checkpoints
(Mchinji, Karonga, Nsanje). HQ centralisation of reference data was
non-negotiable for trust, but per-transaction HQ round-trips were
operationally unworkable.

**Decision.** Three tiers, each independently operable when the tier above
is unreachable:

1. **HQ (Lilongwe DRTSS):** sole source of truth for reference data
   (Department, Station, PatrolPost, OffenceCode, Officer registry).
   Master audit log. Treasury settlement.
2. **Station Server (one per police station — NUC + UPS + 4G failover):**
   replicated reference cache, owns counter-cash payments, owns its officers'
   operational records, holds the citizen-payable index for offline lookups.
3. **Officer device (Android tablet, MAUI):** scoped reference cache, queues
   field-issued fines and payments to push at shift end + every 2h on network.

**Authority rule.** Reference data flows down (HQ writes, others read).
Operational data flows up (devices/stations write, HQ aggregates).

**Reference numbering.** Prefix-then-canonicalise. Devices mint
`DEV{deviceId}-{seq}`, stations mint `STN{stationId}-{seq}` for fines
and `MPAY-STN{stationId}-{seq}` for counter cash receipts. HQ assigns
the canonical `NXF-{year}-{seq6}` on first sync.

**Sync.** HQ↔Station every 5 min over mTLS. Station↔Device at shift
boundaries + every 2h auto. End-user UX is a green/yellow/red pill;
technical sync details live in the audit log. Alarm bar at HQ + tablet
after 2 consecutive failed-sync days. Device auto-revokes after 72h
of no heartbeat.

**Counter mobile money: NO.** Strict role separation — stations process
counter cash only; mobile-money payments must originate from the
officer device, the citizen portal, USSD, or WhatsApp.

**Officer credential reset.** Always HQ IT helpdesk. No
station-side / offline reset path.

**Data retention (recommended, pending MPS / MACRA sign-off):** station
12 months rolling, then auto-purge with a "still-synced?" guard. HQ
indefinite for audit, 7 years for financial records, 10 years for
disputed fines.

**Hardware additions.** Per station: NUC server (~MWK 1.4M), 1500VA UPS
(~600k), dual-NIC + 4G failover (~250k incl. 12 mo data), backup SSD
(~180k). Off-grid sites: 200W solar + 100Ah battery (~1.8M). National-
rollout HQ adds: hot-standby DB (Always On), reverse proxy with rate
limiting, centralised log aggregation.

**Build plan reshuffle.** Module 3 admin extends with Station / PatrolPost /
Officer-registration / OffenceCode CRUD (Module 3.5). Module 4 splits into
4a (Station Server), 4b (Sync engine), 4c (Officer MAUI device). Module 6
deploy adds station-server provisioning scripts.

**Reference doc.** Full sketch with entity diagrams, sync protocol,
alarm UX and open questions: `docs/architecture-distributed.md`.

---

## 2026-04-27 · D-013 — Module 3.5 reference-data CRUD (HQ-only)

**Context.** D-012 designated HQ as sole writer of reference data. The
admin portal previously displayed reference data read-only; for the
distributed architecture to function, HQ admins need full CRUD over
Station, PatrolPost, OffenceCode and Officer.

**Decision.** Add four CRUD surfaces in the admin portal:

- `/stations` — card grid, modal for create/edit, soft-delete (deactivate)
  refuses if officers/devices still attached.
- `/patrolposts` — table, modal for create/edit, station filter, soft-delete.
- `/offencecodes` — table, modal for create/edit, soft-delete falls back to
  IsActive=false when the code is referenced by existing fines (preserves
  referential integrity for historical records).
- `/officers` — existing list page gains a "＋ Register Officer" button
  that opens a registration modal (badge, rank, name, contact, department,
  station). New officers default to `Status=Offline, IsActive=true`.

**RBAC.** All write endpoints require `[Authorize(Roles="Admin")]`;
reads accept Admin or Supervisor. The `<AuthorizeView Roles="Admin">`
wrapper hides write buttons from supervisors so they don't see UI they
can't use.

**Soft-delete semantics.** OffenceCode delete becomes IsActive=false
when referenced by any fine; Station and PatrolPost delete is always
soft (IsActive=false) and refuses on attached officers/devices.

**Migration.** A new EF migration adds `Stations`, `PatrolPosts`, `Devices`
tables plus `Officer.StationId` and `Officer.PrimaryPatrolPostId` foreign
keys, and seeds 5 stations + 6 patrol posts spanning the 4 demo zones.

**Out of scope for D-013** (deferred to Module 4a):
- Device pairing UI (mints PairingToken, displays QR, lifecycle).
- Per-station server endpoint health pings.
- Reference snapshot version pinning UI (D-012 §9b.3 follow-up).
