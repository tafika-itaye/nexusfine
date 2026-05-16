# Department recommendations — execution tracker

**Approved by TJ:** 2026-05-16
**Scope:** Malawi-first delivery. Regional items (ZM / MZ / TZ) flagged ⚪ and parked.
**Rollout sequence:** Sequential MW → ZM → MZ → TZ. One tenant per country (data sovereignty).

---

## Status legend
- 🟢 Done · 🟡 In progress · 🔴 Pending · ⚪ Deferred to post-pilot regional rollout · ⛔ Requires external action (TechNexus must do offline)

## Execution waves
- **Wave 1** — code changes that visibly improve the demo (this session)
- **Wave 2** — repo-committed doc artefacts (policies, playbooks, templates)
- **Wave 3** — outbound-ready collateral (case study, white paper, landing page)
- **Wave 4** — TechNexus offline actions (lawyer engagement, certs, training)

---

## 🎯 Marketing

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| M1 | Reposition to "Africa-modular" | — | ⚪ deferred |
| M2 | One-page case study PDF (Malawi pilot) | 3 | 🔴 |
| M3 | Airtel Africa master MOU | — | ⚪ deferred |
| M4 | White paper "Closing the Revenue Leak" | 3 | 🔴 |
| M5 | GR playbook PS → Director → Minister (Malawi) | 2 | 🔴 |
| M6 | Regional press kit EN/PT/SW | — | ⚪ deferred |
| M7 | Speaker slots — Malawi events only | 4 | ⛔ user |
| M8 | French/Portuguese/Swahili decks | — | ⚪ deferred |
| M9 | LinkedIn newsletter (founders) | 4 | ⛔ user |
| M10 | `nexusfine.mw` landing page | 3 | 🔴 |

## 🎨 UI/UX + Design + Media

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| U1 | Design system documentation (tokens + components) | 2 | 🔴 |
| U2 | WCAG AA accessibility audit + fix | 4 | ⛔ partial automation |
| U3 | Mobile-first redesign of citizen portal | 1/2 | 🔴 |
| U4 | Wire empty-state illustrations everywhere | **1** | 🟡 |
| U5 | Polish officer-device fine flow | — | post Module 4c |
| U6 | Bilingual toggle without page reload | **1** | 🟡 verify (citizen already client-side swap) |
| U7 | Print-quality receipt template (Malawi format) | 2 | 🔴 |
| U8 | Localised iconography (Malawi) | 3 | 🔴 |
| U9 | Custom 404 / 500 / Maintenance pages | **1** | 🟡 |
| U10 | 60-sec animated explainer video | 3 | ⛔ user / agency |

## 🔐 Security

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| S1 | STRIDE threat model committed to repo | 2 | 🔴 |
| S2 | mTLS HQ ↔ Station ↔ Device | 4 | ⛔ requires CA + ops |
| S3 | SOC 2 Type I readiness checklist | 2 | 🔴 |
| S4 | 3rd-party pen test booking | 4 | ⛔ user |
| S5 | SQLCipher encrypted-at-rest for Station | 1 | 🔴 |
| S6 | Secrets vault (Azure Key Vault for Malawi) | 4 | ⛔ user/Azure |
| S7 | Append-only audit log on separate DB | 2 | 🔴 design doc first |
| S8 | Malawi DPIA per Data Protection Act 2024 | 2 | 🔴 |
| S9 | 2FA at supervisor + admin | 4 | 🔴 |
| S10 | Rate limiting + WAF on public endpoints | 1 | 🔴 in-app rate-limit |

## 🛠 Operations

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| O1 | Finalise 90-day pilot operating playbook | 2 | 🔴 |
| O2 | Field-support SLA tiers (P1/P2/P3) | 2 | 🔴 |
| O3 | Pre-deployment site survey checklist | 2 | 🔴 |
| O4 | Spare-parts inventory model (5% / 10%) | 2 | 🔴 |
| O5 | Train-the-trainer pyramid | 4 | ⛔ user |
| O6 | Regional support hub model (Malawi only) | 4 | ⛔ user |
| O7 | Change-management template per ministry | 2 | 🔴 |
| O8 | Internal KPI dashboard (TechNexus ops) | 3 | 🔴 |
| O9 | Chain-of-custody RFID + handover sheet | 2 | 🔴 |
| O10 | Quarterly tabletop exercise (4 scenarios) | 2 | 🔴 |

## ⚖️ Compliance & Legal

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| C1 | Malawi legal opinion on data processing | 4 | ⛔ user/counsel |
| C2 | Standard DPA template | 2 | 🔴 |
| C3 | PPDA Malawi public-procurement compliance map | 2 | 🔴 |
| C4 | Anti-bribery / FCPA training | 4 | ⛔ user |
| C5 | Open-source licence audit (deps in repo) | 2 | 🔴 |
| C6 | IP assignment paperwork | 4 | ⛔ user/legal |
| C7 | Conflict-of-interest register | 2 | 🔴 |
| C8 | Court-evidence retention policy | 2 | 🔴 |
| C9 | Trademark NexusFine (Malawi) | 4 | ⛔ user/IP attorney |
| C10 | Whistle-blower mechanism | 2 | 🔴 |

## 💰 Commercial & Finance

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| F1 | MWK price book (signed off, FX-indexed quarterly) | 2 | 🔴 |
| F2 | 30/30/30/10 payment milestones in every quotation | — | 🟢 done (in QTN-NXF-2026-001) |
| F3 | FX hedging policy for hardware imports | 4 | ⛔ user/bank |
| F4 | Tax treatment opinion (Malawi) | 4 | ⛔ user/accountant |
| F5 | Revenue-share model option (2% × 3 yrs) | 2 | 🔴 |
| F6 | Country P&L view (Malawi pilot) | 3 | 🔴 |
| F7 | Working-capital line of credit | 4 | ⛔ user/bank |
| F8 | Vendor consolidation MOUs | 4 | ⛔ user |
| F9 | Internal cost-to-deliver model | 2 | 🔴 |
| F10 | Treasury reconciliation contract clause | 2 | 🔴 |

## 🤝 Government Relations & Partnerships

| # | Item | Wave | Status |
|---|------|:----:|:------:|
| G1 | Malawi ministry approach map | 2 | 🔴 |
| G2 | MERA / RTSA-equivalent MOU (Malawi) | 4 | ⛔ user |
| G3 | Telco MOU — Airtel MW + TNM | 4 | ⛔ user |
| G4 | Letter of recommendation post-pilot | — | gated on pilot |
| G5 | Advisory board ex-PS recruitment | 4 | ⛔ user |
| G6 | Quarterly govt-stakeholder newsletter | 3 | 🔴 |
| G7 | Reference customer list | — | gated on pilot |
| G8 | COMESA / SADC working group | — | ⚪ deferred |
| G9 | PPP framework alignment | 2 | 🔴 |
| G10 | Diplomatic representation check-ins | — | ⚪ deferred |

---

## Totals

| | Wave 1 | Wave 2 | Wave 3 | Wave 4 | Deferred | Done | Total |
|---|---:|---:|---:|---:|---:|---:|---:|
| Items | 5 | 27 | 8 | 19 | 10 | 1 | 70 |

**Wave 1 items being delivered in the current session:**
- U4 — wire empty-state illustrations into data tables
- U6 — verify bilingual toggle no-reload (citizen portal)
- U9 — custom 404 / 500 / Maintenance Razor pages
- S5 — SQLCipher encrypted-at-rest for Station DB
- S10 — in-app rate limiting on public endpoints

Wave 2 (next session) starts with the highest-leverage docs:
threat model, DPIA, pilot playbook, ministry approach map, price book.
