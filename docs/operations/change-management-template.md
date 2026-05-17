# Change-Management Template (per ministry)

**Status:** v1.0 · **Issued:** 2026-05-17
**Use this template** at the start of every new ministry / agency engagement.
Copy this file to `docs/clients/{ministry-code}/change-mgmt.md`, fill in
the placeholders, get it signed by the ministry's Change Sponsor, and
keep it as the single source of truth for the rollout.

---

## 1. Engagement at a glance

| Field | Value |
|-------|-------|
| Client ministry | _e.g. Malawi Police Service / DRTSS_ |
| Programme name | NexusFine — Digital Traffic Fines |
| TechNexus account lead | _name + email_ |
| Client change sponsor | _Permanent-Secretary level or delegated_ |
| Client pilot champion | _Director / senior officer_ |
| Programme start date | _T0_ |
| Programme end date (acceptance) | _T0 + 104 days_ |
| Reference contract | _MSA reference number_ |

---

## 2. Stakeholder map (RACI)

For each activity below, mark a single **A** (accountable), one or more
**R** (responsible), and the right **C** (consulted) and **I** (informed)
parties.

| Activity | TechNexus | Sponsor | Pilot Champion | Affected officers | MPS ICT | Auditor / DPC |
|----------|:-----:|:-----:|:-----:|:-----:|:-----:|:-----:|
| Pilot scope sign-off | R | A | C | I | C | I |
| Officer training | R | I | A | C | C | I |
| Counter-clerk training | R | I | A | C | C | — |
| Citizen comms (radio, station notice-board) | C | A | R | I | — | — |
| Data-protection impact assessment | R | I | C | I | C | A |
| Cut-over weekend | A,R | I | C | C | R | — |
| Day-1 hyper-care | R | I | C | C | C | — |
| Acceptance (T+104) | C | A | R | I | C | I |

> Re-edit this table at engagement start. Don't ship the template's
> defaults to a real client without review.

---

## 3. Communications calendar (T-30 to T+104)

| When | Audience | Channel | Owner | Message |
|------|----------|---------|-------|---------|
| T-30 | Officers in scope | Parade brief + printed handout | Pilot Champion | "Why are we changing how we issue fines?" |
| T-21 | Counter clerks | 30-min huddle | Pilot Champion | Walk-through of counter cash-handling on NexusFine |
| T-14 | Citizens in pilot catchment | Radio + station notice-board | Sponsor's office + TechNexus marketing | "From [date], you can look up + pay your fine on your phone" |
| T-7 | All police-station staff | Brief + printed quick-reference | Pilot Champion | Day-of-cut-over procedures |
| T-1 | Pilot Champion | 1:1 with TechNexus Account Lead | TechNexus | Day-0 readiness — Plan B confirmed |
| T0 | All staff at station | 5-min parade brief | Pilot Champion + TechNexus engineer | Go-live |
| T+7 | All staff | Parade brief | Pilot Champion | Week-1 retro snapshot |
| T+14 | Officers + Sponsor | Email update | TechNexus | Phase-1 exit metrics |
| T+30 | Sponsor + MPS Comms | Update for IG | TechNexus + Pilot Champion | Month-1 outcomes |
| T+60 | Sponsor + IG / DRTSS | Steering committee | Both | Steer adjustments |
| T+90 | All staff + citizens | Press statement | Sponsor's PR | Pilot concluded; rollout plan |

---

## 4. Training schedule

| Cohort | Size | Duration | Trainer | Materials | Pass-criterion |
|--------|-----:|----------|---------|-----------|----------------|
| Officers (pilot) | 4 | 2 days on-site | TechNexus Trainer | Officer handbook (EN + NY); printed quick-card | 95% pass on competency quiz; live-issue a fine end-to-end |
| Supervisors / admins | 2 | 1 day on-site | TechNexus Trainer | Admin handbook; runbook | 100% pass; demo a reconciliation + audit-log query |
| Counter clerks | 1–2 | half day | Pilot Champion (after T+30 train-the-trainer) | Counter handbook; cash-balance sheet template | Demo a counter cash payment + close-of-day reconcile |
| MPS ICT (handover) | 2 | 1 day at HQ | TechNexus Senior Dev | Architecture overview + deploy guide | Walk-through of deploy + rollback |

> Train-the-trainer at T+30 transitions training authority to the
> client. After that point, TechNexus is escalation only.

---

## 5. Risk register (initial)

Pre-populated with the failure modes we've seen most often. Edit per
ministry.

| # | Risk | Likelihood | Impact | Owner | Mitigation |
|---|------|:----------:|:------:|-------|------------|
| 1 | Officer resistance — "the paper book works fine" | Med | High | Pilot Champion | Visible IG/PS endorsement; pair-issue with a TechNexus engineer for the first day |
| 2 | Power outage during cut-over weekend | High | Med | TechNexus Field Engineer | UPS pre-tested; generator on standby; paper backup ticket book |
| 3 | Connectivity loss to HQ | Med | Low (with Station Server) | TechNexus | Demonstrated graceful degradation; auto-resync when network returns |
| 4 | Misalignment with Treasury reconciliation | Low | High | Account Lead | Clause 14 of MSA covers; weekly recon walkthrough at T+7 |
| 5 | Adverse press during pilot | Low | Med | Sponsor PR + TechNexus | Pre-agreed comms protocol; no statements without joint clearance |
| 6 | Officer demands additional offence codes | Med | Low | Pilot Champion | Offence-code editor in admin portal; 24-h SLA for any code addition |
| 7 | Citizens distrust mobile-money channel | Med | Low | Sponsor PR | Counter-cash always available; receipt PDF demonstrably verifiable |

Risks are re-scored at the weekly review. New risks added in writing.

---

## 6. Decision log (template)

| Date | Decision | Made by | Rationale | Reference |
|------|----------|---------|-----------|-----------|
| T-X | _e.g. defer card-payment activation to T+30_ | Account Lead + Sponsor | _summary_ | Meeting note |

Every material decision (anything that changes scope, schedule or
budget) is added here within 48 hours and visible to both parties.

---

## 7. Success metrics (mirror of pilot playbook §1)

Edit the targets per engagement. The shape stays the same.

| KPI | Target | Source | Owner |
|-----|--------|--------|-------|
| Officer adoption rate | ≥ 80% by end of month 3 | Audit log | Pilot Champion |
| Citizen digital-channel uptake | ≥ 50% | Payment channel mix | Sponsor PR + TechNexus |
| Mean reconciliation lag | ≤ 4 hours | Sync events | TechNexus |
| Station uptime | ≥ 99% | Heartbeat history | TechNexus |
| P1 incidents (cumulative) | ≤ 2 | Incident log | TechNexus |
| Officer training pass-rate | ≥ 95% | Quiz score | Trainer |
| Citizen satisfaction (CSAT) | ≥ 4.0 / 5.0 | Post-payment survey | TechNexus |

---

## 8. Acceptance sign-off

At T+104, the following are reviewed and signed by both parties:

- Final KPI tally vs target (table above)
- Incident report — opened / closed / outstanding
- Open risks transferred to operating BAU
- Documentation hand-over completed (architecture, runbook, training)
- Spare-parts inventory ≥ 80% of policy on all stations
- Train-the-trainer pyramid established

A formal **Acceptance Certificate** is issued by the Sponsor's office.
Outstanding items, if any, are listed with owners and target dates.

---

## 9. Hand-over to BAU

Final week (T+98 → T+104) covers:

- Final supervisor + admin briefing
- Documentation index distributed to the ministry's archive
- Escalation contacts updated on the public station notice-board
- Quarterly KPI review schedule agreed
- Steady-state SLA in force (see `sla-tiers.md`)

---

## 10. Lessons learned (transferable)

A 1-page **lessons-learned note** is written by the TechNexus account
lead within 14 days of acceptance. The note is folded into the
*next* ministry's change-management plan as a section called
"Lessons from {previous-ministry}".

After three completed engagements, this template itself is reviewed and
re-issued.
