# Field-Support SLAs

**Status:** v1.0 · **Issued:** 2026-05-16
**Applies to:** the Malawi pilot, then carried forward into every subsequent rollout contract.
**Binding:** these tiers are pasted verbatim into every MSA TechNexus signs.

---

## Severity definitions

| Severity | Definition | Examples |
|----------|------------|---------|
| **P1 — Critical** | The system is unusable; revenue collection is materially affected; or a security incident is suspected. | HQ API down; SQL Server unreachable; station server bricked; a credential leak; data-protection breach. |
| **P2 — High** | A meaningful subsystem is down, but workarounds exist. | One citizen channel down (WhatsApp OR USSD); one station offline; mobile money channel rejecting transactions for 30+ min. |
| **P3 — Medium** | Function works, but with degraded performance or a known workaround. | A specific report fails to render; a misspelled UI label; auto-refresh broken. |
| **P4 — Low** | Cosmetic, documentation, or minor improvement. | Typo; icon mis-aligned; non-blocking UI ask. |

---

## Response & resolution targets

| Tier | Response time (acknowledge) | Resolution / workaround target | Status updates |
|------|---------------------------|------------------------------|----------------|
| P1 | **30 minutes**, 24/7 | **4 hours** | Hourly until resolved |
| P2 | **2 hours**, business hours | **24 hours** | Twice daily |
| P3 | **Next business day** | **72 hours** | Daily |
| P4 | **5 business days** | **Next minor release** | Weekly |

**Business hours:** 06:00 to 22:00 East Africa Time (CAT), Monday to Saturday.
**Out-of-hours:** P1 only is served 24/7. P2 acknowledged next morning.
**Public holidays:** Malawian public holidays observed for P3/P4. P1/P2 unchanged.

---

## Escalation matrix

If a target above is missed by more than 50%, escalate within the hour to the next tier.

| Tier | Hour 0 contact | Hour 1 escalation | Hour 4 escalation |
|------|----------------|------------------|-------------------|
| P1 | TechNexus on-call engineer (`+265 9XX XXX XXX`) | Account Lead | TechNexus CTO + MPS ICT Liaison |
| P2 | Account Lead | CTO | Director DRTSS + TechNexus MD |
| P3 | Helpdesk ticket queue | Field Engineer | Account Lead |
| P4 | Backlog | (no escalation) | — |

---

## Service credits (commercial)

For each missed P1 resolution target after T+30 days from pilot start:
- **0–25% over target:** 5% credit against the next monthly support charge.
- **25–100% over target:** 10% credit.
- **>100% over target:** 25% credit AND a written corrective-action plan submitted by TechNexus within 5 business days.

Credits cap at 50% of the monthly support charge.

---

## Channels for raising incidents

| Channel | Use for | Hours |
|---------|---------|-------|
| `+265 9XX XXX XXX` (on-call mobile) | P1 only | 24 / 7 |
| `support@technexusmw.com` | P2, P3, P4 | Acknowledged within SLA |
| WhatsApp Business `+265 9XX XXX XXX` | All severities, citizen support | 06:00–22:00 |
| In-portal **Help → Report an issue** button (planned) | All severities | 24 / 7 |

Every incident receives a ticket number on acknowledgement. Status is
visible to MPS through a shared Trello / Linear board (TBD at PO sign).

---

## Exclusions

These SLAs do **not** cover:
- Outages caused by Malawi national power-grid disruption beyond the 30-minute UPS window (unless TechNexus has been contracted for solar back-up under Section E of the quotation).
- Outages caused by telco partners (Airtel, TNM) outside TechNexus's control. TechNexus will assist with diagnosis and escalation to the affected operator.
- Outages during scheduled maintenance windows announced 48 h in advance.
- Force-majeure events.

---

## Quarterly SLA review

A formal SLA-performance report is delivered to MPS the first business day
of each quarter. It contains:
1. Incident count by severity
2. Mean response, mean resolution
3. % within target
4. Credit accrual (if any)
5. Trend vs previous quarter
6. Top 3 recurring failure modes + remediation plan

The report is the basis for the quarterly steering committee.
