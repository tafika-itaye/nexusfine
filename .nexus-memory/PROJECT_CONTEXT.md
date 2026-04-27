# NexusFine — Project Memory (saved 2026-04-24)

## Owner / Client
- Vendor: **TechNexus MW** (MSME-certified Small Enterprise, Blantyre)
- Client: **Malawi Police Service / DRTSS** (Traffic & Road Safety Services)
- Demo target: **Minister of Malawi government** — THIS WEEK (≤7 days)
- Delivery format: **modules via Git Bash / PowerShell scripts** on Windows
- Status: NOT a demo. Full working product to actively bill client with.

## Contracted Stack (per DRTSS Quotation 2026-04-04, MWK 218,295,000)
- Backend: **ASP.NET Core 8 API + SQL Server** (EBPP Core)
- Officer app: **.NET MAUI Android**, NFC, offline, photo capture
- Admin: **Blazor Server**
- Citizen web: responsive portal
- Channels: USSD, WhatsApp Business API, SMS, Email
- Integrations: NRB (national ID), Airtel Money, TNM Mpamba, bank/card gateway
- Hosting: **Microsoft Azure**

## Demo Surface (user-confirmed 2026-04-24)
- Citizen portal (web) ✓
- Admin portal (web) ✓
- API backend (live) ✓
- Mobile app ✓ (was to be skipped, now IN)
- Payments: **simulated** (fake but realistic) for demo — real gateways post-contract

## Reference sites
- https://www.police.gov.mw/ — visual/branding reference
- https://www.ra.org.mw/ — SEPARATE TASK: clone and rewrite entirely, pitch alongside app

## Conflicts to resolve
1. Concept doc suggests Node/Postgres/Elasticsearch/K8s/Terraform — contradicts signed-looking quotation (.NET/SQL/Azure). Keep quoted stack.
2. User earlier said "SMS removed completely" but quotation line item 7 is "SMS & Email Notifications" (MWK 5,250,000). Need ruling.
3. Telegram in concept doc, NOT in quotation. Scope decision needed.

## Current repo state
- Path: C:\Users\HP1\Documents\GitHub\nexusfine
- Solution: NexusFine.slnx → Admin, API, Core, Infrastructure, Mobile
- Static HTML demos: /citizen/index.html, /admin/index.html, /css/theme.css
- Payment services exist: AirtelMoneyService, MpambaService, SmsService, PaymentGatewayFactory
- EF migration: 20260409041318_InitialCreate
- Mobile: MAUI project, had issues opening on phone — deferred

## Key Principles
- Keep answers short, bullet points (user preference)
- Production standards, no placeholders

## DECISIONS LOCKED 2026-04-24
- Stack: **.NET 8 + SQL Server + Blazor Server + .NET MAUI + Azure** (per quotation).
- **SMS & Email: kept as software features**, NOT sold as bundles. Code: Composite notifier (WhatsApp default-on; SMS/Email off by default, flip via config). Quotation: A7 removed and reallocated across remaining A items; C4 restored as "WhatsApp conversation bundle — 200,000 sessions" (same price); C3 renamed "USSD setup fee". **Grand total unchanged: MWK 218,295,000**. New Section E with OPTIONAL RAG FAQ Chatbot (WhatsApp) at MWK 8,750,000.
- **Telegram: OUT.** Build a single **WhatsApp integration** instead.
- **Demo must-haves (bulletproof on stage):**
  1. Officer issues fine in field (MAUI + NFC/photo) — end-to-end, live.
  2. Citizen looks up + pays fine on portal — live, simulated payment.
  3. WhatsApp + USSD walkthrough — simulated chat/menu against live backend.
- Admin KPIs: in scope, not demo-critical.
- Payments for demo: **simulated** (deterministic fake gateway). Real Airtel/Mpamba wired post-contract.

## Module Plan (7 days)
0. ✅ Foundations: code-check, stack lock, remove SMS, decision log, fix quotation (v0.0)
1. ✅ API backend: entities, controllers, JWT+RBAC, audit log, simulated gateway, seed, OpenAPI, xUnit tests (v0.1)
2. ✅ Citizen web portal: live API, bilingual EN/NY, jsPDF receipt (v0.2)
3. ✅ Admin Blazor portal: JWT login, RBAC gating, audit log viewer (v0.3)
4. MAUI officer app: login, NFC, photo, offence capture, offline→sync
5. WhatsApp + USSD: webhook endpoints + menu state + demo chat UI
6. Deploy scripts: PowerShell + bash, EF migrate, seed, publish
7. Polish: E2E tests, demo runbook, backup offline demo

## Module 1 — delivered (2026-04-24)
- Custom `AppUser` + `PasswordHasher` (PBKDF2-SHA256, 210k iter) + `JwtTokenService` (HS256, 120min access, 14d refresh)
- `AuthController`: /login (anon), /register (Admin only), /me
- `SimulatedPaymentGateway` as first-class `IPaymentGateway`; factory routes by `ApiSettings:PaymentMode`
- `AuditLogMiddleware` records every mutating /api request
- Default seeded accounts: `admin / Nexus@Admin2026`, `supervisor / Nexus@Super2026`
- Demo seed: 4 officers (one per zone) + 8 sample fines spanning Paid / Unpaid / Overdue
- Swagger with Bearer scheme on /swagger
- xUnit test project `tests/NexusFine.Tests` — PasswordHasher, JwtTokenService, SimulatedPaymentGateway, Fine lifecycle
- Packages added (Infrastructure): `Microsoft.IdentityModel.Tokens 8.2.1`, `System.IdentityModel.Tokens.Jwt 8.2.1`, `Microsoft.Extensions.Options.ConfigurationExtensions 10.0.5`
- Packages added (API): `Swashbuckle.AspNetCore 7.2.0`
- EF migration: `Module1AddAppUsers` (scripted, run by module1.ps1/.sh)
- Scripts: `scripts/module1.ps1` and `scripts/module1.sh`; doctor now checks .NET 10

## Module 2 — delivered (2026-04-25)
- API now serves static SPA assets (`UseDefaultFiles` + `UseStaticFiles`); root `/` → `/citizen/`
- Citizen portal moved to `src/NexusFine.API/wwwroot/citizen/` (single-origin: no CORS)
- `index.html` — full layout (gov topbar, notice, hero, lookup card with 3 tabs, how-it-works, channels, FAQ, footer, pay modal); every UI string tagged `data-i18n`
- `i18n.js` — complete EN + Chichewa (NY) dictionary; `setLang()` re-applies + re-renders open fine in target language
- `app.js` — calls live API: `lookup → initiate → confirm` (simulated gateway path); generates branded PDF receipt via jsPDF (CDN); also supports re-downloading receipt for already-paid fines
- Channels mapped to backend enum: AirtelMoney / TnmMpamba / BankTransfer / Card / Ussd / WhatsApp
- Scripts: `scripts/module2.ps1` and `scripts/module2.sh` — verify files → build → boot API on :5080 → smoke-test `/citizen/`, assets, `/api/offencecodes`, `/api/fines/lookup` → re-run tests → commit + tag v0.2
- Verified live: lookup `NXF-2026-00001` returns "Demo Driver 1, Unroadworthy Vehicle, Toyota Hilux, MK 60,000" → simulated pay flow flips status to PAID; NY language correctly translates timeline + amount labels
- Auth gating:
  - Dashboard — Supervisor, Admin
  - Fines — [Authorize]; lookup AllowAnonymous; POST Officer/Supervisor/Admin
  - Officers — [Authorize]
  - Payments — AllowAnonymous (citizen + gateway callbacks)
  - OffenceCodes.GetAll — AllowAnonymous

## Module 3 — delivered (2026-04-25)
- Auth design (D-011): JWT held in **circuit memory only**; reload = back to login. Acceptable for demo. Post-demo TODO: ProtectedSessionStorage rehydration + `/api/auth/refresh` endpoint.
- New API endpoint: `AuditLogsController` — `GET /api/auditlogs?entityType=&action=&entityId=&from=&to=&page=&pageSize=`, plus `/entitytypes` and `/actions` distinct lists. `[Authorize(Roles="Admin,Supervisor")]`.
- Admin services:
  - `JwtAuthStateProvider : AuthenticationStateProvider` — scoped per circuit; parses JWT with `JwtSecurityTokenHandler`; normalises `"role"` → `ClaimTypes.Role`; expiry-aware.
  - `JwtBearerHandler : DelegatingHandler` — injects `Authorization: Bearer …` into every API call.
- Admin pages:
  - `Login.razor` — EditForm → POST `/api/auth/login` → `Auth.SignInAsync(token, exp)` → `/`. Hint shows admin / supervisor demo creds.
  - `Logout.razor` — clears the principal.
  - `AccessDenied.razor` — wrong-role landing.
  - `AuditLog.razor` — paginated table + filters (entityType, action, entityId, date range) + JSON pretty-print viewer for OldValues / NewValues. Toggleable per row.
- Routing: `Routes.razor` wraps `<AuthorizeRouteView>`; `<NotAuthorized>` redirects anonymous → `/login`, authenticated wrong-role → `/access-denied`.
- All admin pages carry `@attribute [Authorize(Roles="Admin,Supervisor")]` (Home, Fines, Officers, Reconciliation, AuditLog).
- `NavMenu.razor` user pill replaced with `<AuthorizeView>` rendering `fullName` + top role from JWT claims, with Sign-out link.
- Packages: `Microsoft.AspNetCore.Components.Authorization 10.0.5`, `System.IdentityModel.Tokens.Jwt 8.2.1`.
- Scripts: `scripts/module3.ps1` and `scripts/module3.sh` — boot API + Admin → smoke-test login → audit endpoint → verify anonymous 401 → re-run tests → commit + tag v0.3.

## Separate follow-up
- Clone + rewrite ra.org.mw (Road Authority site) — after NexusFine ships.
