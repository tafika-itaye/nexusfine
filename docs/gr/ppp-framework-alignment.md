# Public-Private Partnership (PPP) — Framework Alignment

**Status:** v1.0 · **Issued:** 2026-05-17
**Owner:** TechNexus GR + Legal · **Confidential — internal**
**Authority:** Public-Private Partnership Act, 2018 (Malawi); PPP Commission

This note maps NexusFine onto Malawi's PPP framework. Even where the
NexusFine pilot procures via PPDA (open or restricted tender), the
**national-rollout phase will likely be more efficient as a PPP** —
specifically a service-delivery PPP for digital-revenue collection.

This document keeps the option warm so that, at the right political
moment, we can pivot from PPDA-procurement to PPP-arrangement without
restarting the engagement.

---

## 1. Why PPP, and why later

PPDA procurement is the right vehicle for the **pilot**:
- Single agency (MPS), single output (digital fines).
- Predictable scope, predictable cost, finite duration.
- TechNexus delivers; the State owns operational risk.

PPP becomes more attractive at **national rollout** because:
- The scope crosses multiple agencies (MPS, DRTSS, Treasury, Roads, MERA).
- The duration is long (5–15 years).
- The revenue is observable (fines collected) and material.
- Operational risk benefits from being shared with a private partner
  whose interests are aligned to collection success.

A typical NexusFine PPP would be a **service-delivery contract** with
revenue-share alignment — what HM Treasury calls "operations & maintenance
with payment by results".

---

## 2. Malawi PPP framework — what it requires

The PPP Act 2018 + the PPP Commission's standard procedures impose a
documented gate sequence:

| Gate | Document required | Owner | Approver |
|------|------------------|-------|----------|
| G0 — Project identification | Project Identification Note (PIN) | Contracting Authority | PPP Commission |
| G1 — Pre-feasibility | Pre-feasibility study + value-for-money screen | Contracting Authority + advisor | PPP Commission Board |
| G2 — Feasibility | Full feasibility study + risk allocation + affordability + market-sounding | Contracting Authority + transaction advisor | Cabinet Committee on the Economy |
| G3 — Procurement of PP | Standard PPP tender or single-source justification | PPP Commission + Contracting Authority | PPP Commission Board |
| G4 — Negotiation | Term sheet + DPA + project agreement | Contracting Authority | Cabinet |
| G5 — Financial close | Sign-off + commencement plan | All parties | Cabinet |

The Pilot, by virtue of its size (MWK ~218M, < $125k under the
threshold) and its short duration (90 days + 14 days hyper-care),
does not trigger PPP-Act review. **National rollout would.**

---

## 3. Where NexusFine sits in the PPP taxonomy

| PPP class | Characteristic | Fit? |
|-----------|----------------|------|
| Concession (BOT, BOOT) | Long-lived asset transferred at end | ❌ no physical asset |
| Affermage / lease | Operator runs existing public service for a fee | Partial fit |
| Management contract | Operator manages on a flat fee | Possible Y1 vehicle |
| **Service-delivery contract** (the right slot) | Operator provides a discrete public service against measurable outputs | ✅ best fit |
| Joint venture | Both parties share equity | ❌ unnecessary for software |
| Outsourcing with revenue share | Operator paid out of a slice of the revenue it enables | ✅ best fit (variant) |

The recommendation, when the time comes, is a **service-delivery PPP
with revenue-share economics** — combining today's `revenue-share-model.md`
with the PPP-Act protections that long-duration arrangements need.

---

## 4. Value-for-Money (VfM) framing

The PPP Commission requires a VfM case. For NexusFine, that case writes
itself:

| Lens | NexusFine vs paper | Notes |
|------|-------------------|-------|
| Revenue uplift | Estimated **~30% recovery rate** improvement | Comparator: 2024 paper-process recovery rate per MPS data |
| Operating cost | Lower per-fine cost at scale | Per `cost-to-deliver-model.md` §5 |
| Audit cost | Lower — append-only audit log replaces manual file review | Per `append-only-audit-log.md` |
| Court-evidence cost | Lower — digital evidence ready vs paper retrieval | Per `evidence-retention-policy.md` |
| Risk transfer | Citizen-channel risk + collection risk shift to operator | Revenue-share model alignment |
| Citizen experience | Materially better | 5-minute pay vs queue at Treasury counter |

A formal VfM analysis would be commissioned at G1.

---

## 5. Risk allocation (PPP-Act §15)

A standard PPP risk matrix; we have a recommended allocation already.

| Risk | Allocation | Rationale |
|------|-----------|-----------|
| Demand (fine volume) | **Shared** | Reflects both road-traffic policy AND operator performance |
| Build (software + hardware) | **Operator** | We control quality |
| Performance (uptime, SLA) | **Operator** | We control execution |
| Regulatory change (tariff, policy) | **Authority** | Beyond our control |
| Telco / payment-aggregator outage | **Shared** | Neither party controls aggregators alone |
| Force majeure | **Shared** | Standard PPP clause |
| Currency (FX on hardware imports) | **Operator** | Conventional |
| Cyber-security breach (system) | **Operator** | We are the data processor |
| Cyber-security breach (state credentials misuse) | **Authority** | Inside its operational perimeter |
| Constitutional / sovereign default | **Authority** | Conventional |

The allocation is anchored in the project agreement at G4. The
revenue-share model is what makes the **shared-demand** allocation
financially tractable.

---

## 6. Contractual instruments needed at G4

The project agreement will reference (or include) at a minimum:

- The PPP Commission's standard form Project Agreement (2024 edition)
- A Data Processing Agreement compliant with DPA 2024 §38
- A bespoke Service-Level Agreement schedule
  (`docs/operations/sla-tiers.md` is the starting point)
- A Treasury reconciliation schedule
  (`docs/commercial/treasury-reconciliation-clause.md` Clause 14)
- A revenue-share schedule
  (`docs/commercial/revenue-share-model.md`)
- A retention schedule
  (`docs/compliance/evidence-retention-policy.md`)
- A change-management plan template
  (`docs/operations/change-management-template.md`)
- An exit / hand-back plan (drafted at G2; matures by G4)

The exit plan is critical — at end-of-PPP, the State must be able to
operate NexusFine without TechNexus. We commit to source-available
delivery (with the caveats already in our standard MSA) and a
minimum 12-month transition window.

---

## 7. PPP Commission engagement plan

Approach is patient and sequential — the Commission is a small office
and prioritises by Cabinet attention.

| Window | Action |
|--------|--------|
| **Now → Pilot acceptance** | Build the pilot under PPDA. Do not pre-empt the Commission. |
| **Pilot acceptance + 60 days** | Brief the PPP Commission's Director informally with the pilot results. |
| **+90 days** | Submit a Project Identification Note (PIN) at the Commission's invitation. |
| **+180 days** | Pre-feasibility study commissioned (TechNexus + Authority co-fund). |
| **+360 days** | Full feasibility ready; Cabinet briefing scheduled. |
| **+450 days** | PPP tender / single-source justification at G3. |
| **+540 days** | Project agreement signed (G4); financial close (G5). |

We do **not** pitch PPP at the Minister demo. We let the pilot succeed
first.

---

## 8. PPDA → PPP transition risks

| Risk | Mitigation |
|------|-----------|
| Mid-flight scope/instrument change discomforts MPS | Make the transition a successor instrument, not a replacement — pilot completes cleanly first. |
| PPP scope expands beyond MPS (DRTSS, Treasury, Roads) | Negotiate scope deliberately; we control breadth. |
| Multi-agency political coordination breaks down | Sponsor one cross-government working group (PPP Commission chair) before any agreement is signed. |
| Revenue-share economics raise misperceived "privatisation" risk | Communications strategy emphasises: State owns the data, State owns the policy; TechNexus delivers a service. |

---

## 9. The "do not" list

For protocol and reputation, NexusFine does **not** approach the PPP
Commission with:

1. A solicited unsolicited proposal before pilot completion.
2. Any suggestion that PPP avoids PPDA's competitive disciplines —
   the PPP Act has its own competitive procurement gate.
3. Any discussion of equity in TechNexus by a state body. We are an
   operator, not a JV partner.
4. Discussion of revenue-share rates publicly. The model is opt-in
   only and is shown in confidence (`revenue-share-model.md`).

---

## 10. References

- Public-Private Partnership Act, 2018 (Cap. 22:11)
- PPP Commission standard documents (2024 edition)
- World Bank — *Public-Private Partnerships Reference Guide v3.0*
- HM Treasury — *Value for Money assessment* methodology
- `docs/commercial/revenue-share-model.md`
- `docs/commercial/cost-to-deliver-model.md`
- `docs/compliance/ppda-malawi-procurement-map.md`
- `docs/gr/malawi-ministry-approach.md`
