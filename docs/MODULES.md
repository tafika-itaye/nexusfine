# NexusFine — Seven-Day Module Plan

| # | Module | Owner | Ships | Tag |
|---|--------|-------|-------|-----|
| 0 | Foundations: code-check, SMS removal, decision log, scripts | arch | Day 1 | v0.0 |
| 1 | API backend: entities, JWT+RBAC, audit, simulated gateway, seed, OpenAPI | be | Day 1–2 | v0.1 |
| 2 | Citizen web portal: live, bilingual EN/NY, PDF receipt | fe | Day 2–3 | v0.2 |
| 3 | Admin Blazor portal: login, KPIs, officers, fines, audit | fe | Day 3–4 | v0.3 |
| 4 | MAUI officer app: login, NFC, photo, offence capture, offline→sync | mob | Day 4–5 | v0.4 |
| 5 | WhatsApp + USSD webhook + menu state + demo chat UI | be | Day 5–6 | v0.5 |
| 6 | Deploy scripts (PowerShell + Bash) + Azure publish | ops | Day 6 | v0.6 |
| 7 | Polish: E2E tests, Minister demo runbook, offline backup demo | qa | Day 7 | v1.0 |

## Module completion criteria

Every module ends with:
1. `scripts/moduleN.ps1` (or `.sh`) runs clean.
2. `dotnet test` green.
3. Decision log entry if any architectural choice was made.
4. Git tag `v0.N` cut.

## Out of scope (Phase 2+)

Elasticsearch, Kubernetes, Terraform, Prometheus/Grafana, ELK, multi-region,
Telegram bot, SMS channel, bank/card gateway live integration.

See `DECISIONS.md` for the full decision log.
