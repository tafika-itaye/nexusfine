# NexusFine — Threat Model (STRIDE)

**Status:** Draft v1 · **Owner:** TechNexus CISO function · **Reviewed:** 2026-05-16
**Next review:** quarterly, and after any architecture change
**Scope:** Malawi pilot. Same model applies to ZM/MZ/TZ tenants by substitution.

This document classifies threats against NexusFine using STRIDE (Spoofing, Tampering,
Repudiation, Information disclosure, Denial of service, Elevation of privilege) and
records the controls in place, the residual risk, and the planned hardening path.

---

## 1. System decomposition

NexusFine has six trust boundaries:

```
  [Citizen]  ──HTTPS─►  [Citizen Portal (HQ)]  ─►  [HQ API]  ─►  [HQ DB]
                            │                       ▲
                            │                       │
  [Officer]  ─paired────►  [Tablet]  ──mTLS────►  [Station Server]  ──mTLS──►  [HQ API]
                                                       │
                                                       └──►  [Station DB (SQLite)]

  [Supervisor]  ──HTTPS─►  [Admin Portal]  ──Bearer JWT──►  [HQ API]
```

**Data classes:**
- **PII** — driver name, national ID, licence number, phone, plate
- **Financial** — payment amounts, receipts, channel references
- **Operational** — fines issued, status, photos, GPS, NFC tag IDs
- **Audit** — every state change, write-only, immutable

---

## 2. STRIDE analysis (top assets)

### 2.1 Citizen-portal fine lookup

| STRIDE | Threat | Control | Residual |
|---|---|---|---|
| S | Citizen impersonates another driver to view their fines | Public lookup intentionally returns only reference, plate, amount, status — no PII. Detail page gated by a one-time code sent to phone on record. | Low |
| T | Attacker tampers with lookup response in-flight | TLS 1.2+ enforced on public endpoint. HSTS preload. | Low |
| R | Disputed lookups | Lookup events captured in audit log with IP + user agent. | Low |
| I | Mass scrape of plates / refs | Rate limiter `public` policy: 120 req/min/IP. WAF rules at deploy. | Medium |
| D | DDoS against the public site | Cloud WAF (Azure Front Door) + auto-scaling App Service. | Medium |
| E | Lookup endpoint escalates to admin | `[AllowAnonymous]` only on `lookup`; all admin endpoints require `[Authorize(Roles=...)]`. | Low |

### 2.2 Officer fine issuance (tablet → Station → HQ)

| STRIDE | Threat | Control | Residual |
|---|---|---|---|
| S | Stolen device impersonates an officer | Shared-device login switch; 72-hour heartbeat → auto-revoke. Device cert pinned at station. | Medium |
| T | Tampering with offline-queued fines | Each record carries a client-UUID idempotency key; HQ rejects on mismatch with cryptographic hash on push. | Medium |
| R | Officer denies issuing a fine | Every issuance records badge, GPS, timestamp, photo, NFC scan — all in audit log. | Low |
| I | Citizen PII captured on a tablet exposed to others | Tablet PIN; screen auto-locks after 60s; SQLCipher on the local cache (planned S5). | Medium |
| D | Tablet flood pushes fake fines to station | Per-device throttle at station (planned). | Medium |
| E | Officer obtains supervisor token | JWT roles enforced server-side; supervisor 2FA (planned S9). | Low |

### 2.3 Payment posting

| STRIDE | Threat | Control | Residual |
|---|---|---|---|
| S | Forged gateway callback marks a fine as paid | Callback verified against gateway's HMAC signature. Simulator clearly labelled for demo. | Low |
| T | In-flight tampering with payment amount | TLS to gateways. Server-side recompute of amount from fine.id + offence tariff at confirm time. | Low |
| R | Citizen denies authorising payment | All initiates logged with phone + IP + channel; gateway TX reference stored verbatim. | Low |
| I | Receipt leaks PII | Receipt template carries only plate, reference, amount, channel — no national ID or address. | Low |
| D | Spurious "pay" floods | Rate limiter on `/api/payments/initiate` (treat as `public`); idempotency via fine-ref + channel + window. | Low |
| E | Payment endpoint accepts unauth state change | Initiate is intentionally `[AllowAnonymous]` (citizen path) but never sets `Fine.Status` — only `Confirm` flow does that, and the simulator mirrors the prod HMAC check. | Low |

### 2.4 Admin portal (supervisor / admin)

| STRIDE | Threat | Control | Residual |
|---|---|---|---|
| S | Credential stuffing on `/api/auth/login` | Rate limiter `sensitive`: 10 req/min/IP. 2FA at supervisor + admin (planned S9). Account-lock-out after 10 failures (planned). | Medium |
| T | XSS / CSRF | Blazor Server is server-rendered; `app.UseAntiforgery()` set. No `innerHTML` usage. | Low |
| R | Admin denies running a destructive action | All POST/PUT/PATCH/DELETE captured by `AuditLogMiddleware` with userId, IP, before/after JSON. | Low |
| I | JWT theft | JWT held only in Blazor circuit memory (D-011). Page reload kicks user back to `/login`. | Medium |
| D | Login flood | `sensitive` rate-limit window. | Low |
| E | Supervisor accesses Admin-only CRUD | All write endpoints carry `[Authorize(Roles="Admin")]`; UI hides write buttons via `<AuthorizeView Roles="Admin">`. | Low |

### 2.5 Station Server ↔ HQ sync

| STRIDE | Threat | Control | Residual |
|---|---|---|---|
| S | Rogue station server posts fake fines to HQ | mTLS with per-station cert pinned in `Station.StationServerPublicKey` (planned S2). | High in current build |
| T | Replay of operational push | ClientUuid idempotency key on every record; HQ dedups. | Low |
| R | Station disputes records it sent | Sync events recorded both ends with payload digest. | Low |
| I | Reference data leakage on station theft | SQLCipher on the station DB (planned S5). Disk-level encryption (BitLocker / LUKS) on NUC. | Medium |
| D | HQ unreachable | Designed-for graceful degradation: stations operate fully offline; outbound queue retries up to 7 days. | Low |
| E | Station server escalates to HQ admin | Station's cert grants only the `station` role at HQ; no admin scope. | Low |

---

## 3. Top residual risks (sorted by demo-day exposure)

| # | Risk | Owner | Target close date |
|---|------|-------|------------------|
| 1 | mTLS not yet enforced HQ ↔ Station | CISO | Before pilot go-live |
| 2 | Station SQLite is plain-text | CISO | Wave 2 (S5) |
| 3 | No 2FA at supervisor/admin level | CISO | Wave 4 (S9) |
| 4 | No WAF in front of public endpoints | CISO + Ops | Pilot deploy phase |
| 5 | NoOpAuthHandler is permissive (admin HTTP layer) | Engineering | Replace once we have a real cookie + session strategy |

---

## 4. Controls inventory (what's already in place)

- ASP.NET Core JWT auth (HMAC-SHA256, configurable secret, 120-min TTL)
- `AuditLogMiddleware` records every mutation
- `JsonStringEnumConverter` globally — prevents enum-as-int payload tampering
- Rate-limiter middleware (`public` / `sensitive` policies)
- Collision-safe receipt and reference generation (no `Count() + 1` race window)
- HTTPS redirection middleware
- Antiforgery middleware (Blazor)
- Role-based authorisation on every write endpoint
- One-time, hashed pairing tokens for tablet enrolment
- Heartbeat-based device revocation (72-h policy)

---

## 5. Change log

| Date | Change |
|------|--------|
| 2026-05-16 | Initial draft |
