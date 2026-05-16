# NexusFine — 90-Day Pilot Operating Playbook

**Owner:** TechNexus COO function · **Status:** v1.0
**Pilot site:** Area 18 Police Station, Lilongwe Central
**Pilot start:** T0 (date confirmed at PO signature)
**Duration:** 90 days from T0 + 14 days hyper-care = 104 days end-to-end

This playbook defines what TechNexus and MPS each do, every week, to land
NexusFine in production at one station and learn the lessons we need to
scale to all 28 districts.

---

## 1. Pilot success criteria (defined at T0; reviewed at T+90)

| KPI | Target | Source |
|-----|--------|--------|
| Officer adoption rate | ≥ 80% of fines issued via tablet at the end of month 3 | Audit log: count of `Fine.IssuedFromDeviceId IS NOT NULL` |
| Citizen-channel uptake | ≥ 50% of fines paid through digital channels (any of WhatsApp, USSD, mobile money, card, bank) within 14 days of issuance | `Payments.Channel != Cash` ratio |
| Mean reconciliation lag | ≤ 4 hours from payment confirmation to admin visibility | `Payment.CompletedAt` → `SyncedToHqAt` |
| Station uptime | ≥ 99% (excluding scheduled maintenance) | Station heartbeat to HQ |
| P1 incidents | ≤ 2 over the pilot | Internal incident log |
| Officer training pass-rate | ≥ 95% of trained officers achieve operating competence within 2 days | Training quiz score |
| Citizen satisfaction (CSAT) | ≥ 4.0 / 5.0 | Post-payment chat survey |

A pilot is "successful" when **at least 5 of 7** criteria are met. Any single
miss triggers a documented learnings note. Two or more misses trigger a
joint MPS / TechNexus review before national rollout.

---

## 2. Roles & responsibilities

### TechNexus side
| Role | Name | Coverage |
|------|------|---------|
| Account Lead | TJ (TBC) | 8 hr / day, on call 24/7 P1 |
| Field Engineer (Lilongwe-based) | TBA | 5 days/wk on site for first 14 days, then 2 days/wk |
| Senior Developer (HQ duty) | TBA | Remote, 4 hr response SLA on P1 |
| Trainer | TBA | 2 days on site at T0, T+30, T+60 |

### MPS side
| Role | Coverage |
|------|---------|
| Station Commander, Area 18 | Single point of approval at the pilot site |
| Pilot Champion (officer rank Inspector or above) | Daily operating contact, runs the parade-room briefing |
| Records Officer | Custody of paper backup; reconciles weekly |
| MPS ICT Liaison | First call for any system question |

---

## 3. Cadence

### Daily (during weeks 1–4)
- 07:30 — TechNexus field engineer on station before shift-start
- 08:00 — Shift parade briefing (5-min review of yesterday's stats by Pilot Champion)
- 17:00 — End-of-shift sync + queue flush; field engineer logs the day's tally
- 17:30 — TechNexus internal stand-up (15 min) — what worked, what blocked

### Weekly (full pilot)
- **Monday 09:00** — Weekly review meeting (45 min) — MPS pilot champion + TechNexus Account Lead + Field Engineer. Standing agenda below.
- **Friday 16:00** — Cash-up reconciliation against IFMIS

### Monthly
- **First Tuesday** — Steering committee with Director DRTSS + IG Police representative + TechNexus principal. KPI dashboard reviewed; corrective actions agreed in writing.

---

## 4. Phase schedule

### Phase 1 — Bring-up (T0 → T+14)
- T0     · Station server provisioned, paired devices in hand, 4 officers credentialed
- T0+1   · Day-1 dry run — 5 mock fines issued, none submitted to Treasury
- T0+2   · Cut-over morning: first real shift on NexusFine
- T0+7   · End-of-week-1 retro; gap list owned
- T0+14  · Phase-1 exit: ≥ 50 real fines issued via tablet; no P1 outstanding

### Phase 2 — Operational ramp (T+14 → T+60)
- Add patrol-post coverage (Kamuzu Highway North + South)
- Field engineer steps to 2 days/wk on site
- Train 4 additional officers (total 8 on tablet)
- WhatsApp + USSD channels turned on for citizen-side payments

### Phase 3 — Stabilisation (T+60 → T+90)
- Field engineer off-site, on call only
- Officer training rolled to a permanent station-trainer (train-the-trainer)
- Weekly KPI report becomes self-service for MPS

### Phase 4 — Hyper-care (T+90 → T+104)
- 14-day stand-down period before formal acceptance
- All P3/P4 issues closed; only P1/P2 channel open
- Joint sign-off meeting on day T+104

---

## 5. Incident severity & response

| Severity | Definition | Response SLA | Resolution SLA |
|----------|-----------|--------------|----------------|
| P1 | System unusable. Officers cannot issue fines AND counter cannot accept payment | 30 min | 4 hours |
| P2 | Material loss of function (one channel down, one tier offline) | 2 hours | 24 hours |
| P3 | Operational nuisance, no revenue impact | Next business day | 72 hours |
| P4 | Cosmetic / minor | Weekly | Next release |

Escalation chain: Field Engineer → Account Lead → CTO → CEO.
MPS escalation contact maintained in `Settings → About` and printed weekly.

---

## 6. Standing weekly-review agenda

1. Last week's KPI snapshot vs target (5 min)
2. Incident report — opened, closed, outstanding (5 min)
3. Officer feedback (round-robin from Pilot Champion) (10 min)
4. Citizen complaints / commendations (5 min)
5. Risks & dependencies surfacing in the coming 2 weeks (10 min)
6. Decisions taken / actions assigned with owner + due-date (5 min)
7. AOB (5 min)

A short written summary is shared with both teams within 24 h.

---

## 7. Day-1 readiness checklist (T-1 day)

- [ ] Station server NUC powered, on UPS, on 4G failover SIM, last-sync clock within 5 min of HQ
- [ ] 4 tablets paired, charged, in rugged cases, NFC tags assigned
- [ ] Each officer has signed the device-issue handover sheet (chain-of-custody)
- [ ] Field engineer has 1 spare tablet, 1 spare battery, 1 spare SIM
- [ ] Paper backup ticket book on site as fallback (with serial-range logged)
- [ ] Pilot Champion has the Plan-B card (network down, station down, device lost)
- [ ] MPS ICT helpdesk has TechNexus on-call phone number on the watch-board
- [ ] Demo runbook reviewed (`docs/demo-runbook.md`) — Plan B section in particular
- [ ] Communications go-out plan: SMS to motorists in catchment area, announcement on station notice-board

---

## 8. Exit criteria (T+90 acceptance)

- All 7 success-criteria items measured and tabulated
- No P1 incidents in the last 30 days
- All P2 incidents have a documented permanent fix
- Spare-parts inventory at ≥ 80% of policy (5% spare devices, 10% batteries)
- Officer training pyramid in place — at least one station trainer certified
- Documented lessons-learned register, signed by both parties
- Sign-off meeting minuted; formal acceptance certificate issued

---

## 9. Hand-over to national rollout

If the pilot is accepted (≥ 5 of 7 success criteria met), the rollout sequence is:
1. Mzimba (Mzuzu) — Northern region anchor — T+105
2. Blantyre + Limbe — Southern region anchor — T+135
3. Zomba + Mangochi — T+165
4. Kasungu + Salima + Mchinji — T+195
5. Remaining districts — T+225 onward, two-per-fortnight cadence

Each subsequent station follows a compressed version of this playbook: 14
days of bring-up, 30 days of stabilisation, accepted at day 45.

---

## 10. References

- `docs/demo-runbook.md` — Minister demo flow (transferable beats for the pilot day-1)
- `docs/architecture-distributed.md` — three-tier topology and sync model
- `docs/compliance/dpia-malawi.md` — data-protection compliance
- `docs/security/threat-model.md` — security posture
- `docs/DECISIONS.md` — architectural decision log
