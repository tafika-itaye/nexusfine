# Malawi — Ministry Approach Map

**Status:** v1.0 · **Owner:** TechNexus GR function
**Purpose:** Internal sales playbook for navigating the political and
administrative path from first contact to signed PO and pilot go-live.
Keep this **confidential** — do not share with the client.

> "You don't sell government technology to a ministry. You sell it to the
> right people inside the ministry, in the right order, with the right
> artefact at each step." — internal mantra

---

## 1. Stakeholder map

### Statutory / decision-making
| Role | Office | What they care about | Engagement frequency |
|------|--------|----------------------|----------------------|
| Minister of Homeland Security | Cabinet | Political optics, revenue uplift, anti-corruption story | Quarterly, formal |
| Inspector General of Police | MPS HQ | Operational control, officer discipline, data sovereignty | Monthly, semi-formal |
| Permanent Secretary, Ministry of Homeland Security | Capital Hill | Procurement legality, alignment with public-service reform | Bi-monthly, formal |
| Director, DRTSS | DRTSS HQ | Service quality, throughput, KPI dashboard | Bi-weekly, working |
| Director, Public Procurement & Disposal of Assets Authority (PPDA) | Lilongwe | Procurement compliance | At award + audit |
| Auditor General | Audit House | Audit-trail quality, segregation of duties, evidence preservation | Annually + pilot review |

### Operational / day-to-day
| Role | What they care about | Daily contact? |
|------|----------------------|----------------|
| Station Commander, pilot site | His own incidents, officer welfare, peace at the desk | Yes |
| Pilot Champion (Inspector) | Looks-good-to-IG, ease of use | Yes |
| MPS ICT Director | Won't-break-anything, MPS-owned servers, vendor lock-in | Weekly |
| MPS Legal Counsel | Data Protection Act compliance, IP clauses | At contract sign |
| MPS Records Officer | Retention discipline, file management | At sign + quarterly |
| Anti-Corruption Bureau Liaison | Audit transparency, investigative access | Briefing at pilot day-0 |

### External, supporting
| Stakeholder | Role |
|-------------|------|
| Treasury / Accountant General | Settlement reconciliation, IFMIS integration |
| Reserve Bank of Malawi (RBM) | Payment-system clearance, AML/CFT alignment |
| Malawi Communications Regulatory Authority (MACRA) | Data Protection Commission; USSD aggregator licensing |
| Airtel Malawi · TNM | Channel partnership, billing-line settlement |
| Selected NGO partners | Citizen-side communications and complaint handling |

---

## 2. Approach sequence

The order matters more than any single conversation. Out-of-sequence visits
sour later steps.

### Phase 1 — Quiet preparation (months -3 to -1)
1. Hold a confidential briefing with **MPS ICT Director** to map the technical landscape (existing systems, IFMIS interface, current paper workflow).
2. Visit **Director DRTSS** to listen to their pain points. Bring the architecture overview and one of the polished mockups — not the price.
3. Brief the **Auditor General** at officer level (a working briefing, not a courtesy call). Explain audit-log immutability and retention policy.
4. Brief **MACRA / Data Protection Commissioner** on the DPIA. Ask for written acknowledgement.

### Phase 2 — Formal proposal (month -1)
5. Submit the cover letter + quotation through the **Permanent Secretary, MHS** with copy to **IG Police** and **Director DRTSS**.
6. Hold a working session with **MPS Legal Counsel** on the DPA template + IP-assignment clauses.
7. Engage with **PPDA** at procurement-officer level to confirm the procurement route (open tender / restricted tender / direct procurement under PPDA reg 84).

### Phase 3 — Demo & decision (month 0)
8. Deliver the **Minister demo** (`docs/demo-runbook.md`). The full demo, not a sales pitch.
9. Hand the Minister an executive summary one-pager (Marketing M2) right after the demo.
10. Follow up in writing within 48 h with: a) the demo recording, b) any clarifications requested, c) a proposed pilot start date.

### Phase 4 — Pilot agreement (month +1)
11. Negotiate the PO with the **Procurement Officer, MPS** under PPDA review.
12. Sign the **DPA** with MPS Legal Counsel (template in `docs/compliance/dpa-template.md`).
13. Letter of award; PO issued; payment-schedule 30/30/30/10 locked.
14. Pilot site survey (`docs/operations/site-survey-checklist.md`) commissioned at Area 18.

### Phase 5 — Pilot & scale (months +2 to +5)
15. 90-day pilot per `docs/operations/pilot-playbook-90d.md`.
16. Monthly steering committee with Director DRTSS + IG representative + Permanent Secretary's office.
17. Acceptance certificate at T+104; press release at T+105 by mutual agreement.

---

## 3. Talking-point cards

### When meeting the Permanent Secretary
- Revenue recovery — quantify the leak, then promise audit-trail.
- MSME-certified Malawian vendor — local-procurement compliance.
- Pilot-first; no large up-front commitment.
- Data sovereignty: nothing leaves Malawi.

### When meeting the IG Police
- Officer control, not officer replacement.
- Officer discipline data — fines issued, collection rate, beat coverage — becomes visible.
- Plan B is real: demonstrate offline-tolerant operation.

### When meeting the Director DRTSS
- Service-quality lift — citizens pay in 5 minutes, not in queues.
- Operational dashboards — KPIs at HQ in real time.
- 28 districts already modelled in the system on day 1.

### When meeting MPS ICT
- No new datacentre required for pilot; Azure region with Malawi residency.
- All controllers fall back gracefully; no single point of failure.
- Source-available to MPS ICT on signature; ownership clauses friendly.

### When meeting Auditor General
- Append-only audit log per row-version hash chain.
- Court-grade retention schedule (10 years on disputed matters).
- Every officer action, who-what-when-where-why, recoverable.

### When meeting MACRA / DPC
- Full DPIA prepared (Article 31 prior-consultation).
- Cross-border-transfer position: none triggered.
- DPO contact named per ministry.

---

## 4. Likely objections + counter

| Objection | Counter |
|-----------|---------|
| "We've been burned by vendors before" | TechNexus is Malawian, MSME-registered, with no foreign-court litigation exposure. Source-available to MPS ICT. |
| "How do we know it will work in Mchinji or Karonga?" | Designed three-tier offline-tolerant from day one. Station Server is the architectural answer. |
| "What about Treasury reconciliation?" | Daily IFMIS sync supported; demonstrated in the Plan-B beat. Out-of-scope items priced separately under Section E. |
| "MPS staff aren't tech-literate enough" | Bilingual interface (EN/NY) on day 1. Train-the-trainer pyramid. 2-day officer course. |
| "What if there is political change?" | The system is owned by MPS. Source available. Hand-over to MPS ICT is contracted at pilot acceptance. |
| "Why not procure off-the-shelf from South Africa or Kenya?" | Malawi-specific data sovereignty. Local-procurement preference. Local fast-response SLA. Three African data-protection regimes already mapped. |

---

## 5. Risk-watch list

| Risk | Mitigation |
|------|-----------|
| Cabinet reshuffle changes the Minister of HS | Maintain warm relationships at PS level — PS continuity through cabinet changes is the norm. |
| Treasury IFMIS API not ready | Offer manual reconciliation as a Phase-1 fallback; defer IFMIS integration to Phase 2 under Section E. |
| Telco partner pulls back on USSD pricing | Maintain dual-operator strategy from day 1; both Airtel and TNM in initial agreements. |
| Pilot site falls behind | Two-tier escalation through Station Commander → Director DRTSS. Avoid escalation to IG until P1. |
| Adverse press during pilot | Pre-agree comms protocol with MPS PR office. No statements without joint clearance. |

---

## 6. Calendar of key government dates

| Event | Window | Why we care |
|-------|--------|------------|
| National Budget Statement | February each year | Funding decisions for the next FY |
| Mid-year budget review | August | Mid-year reallocations |
| End of financial year | 30 June | Procurement deadlines |
| Cabinet reshuffles | Unpredictable | Re-engage the new minister fast |
| Independence Day (6 July) | July | Slow period; do not schedule new pitches |
| Christmas / New Year | December–January | Slow period; do not schedule new pitches |
| Local-government elections (next: 2027) | TBC | Political risk window |

Keep this file under regular review. Update after every material meeting.
