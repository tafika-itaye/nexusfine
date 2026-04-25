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
