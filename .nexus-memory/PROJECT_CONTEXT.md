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
0. Foundations: code-check, stack lock, remove SMS, decision log, fix quotation
1. API backend: entities, controllers, JWT+RBAC, audit log, simulated gateway, seed, OpenAPI
2. Citizen web portal: live, bilingual EN/NY, PDF receipt
3. Admin Blazor portal: login, KPIs, officer perf, fines, audit
4. MAUI officer app: login, NFC, photo, offence capture, offline→sync
5. WhatsApp + USSD: webhook endpoints + menu state + demo chat UI
6. Deploy scripts: PowerShell + bash, EF migrate, seed, publish
7. Polish: E2E tests, demo runbook, backup offline demo

## Separate follow-up
- Clone + rewrite ra.org.mw (Road Authority site) — after NexusFine ships.
