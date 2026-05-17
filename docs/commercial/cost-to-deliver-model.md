# Internal Cost-to-Deliver Model

**Status:** v1.0 (Malawi pilot baseline) · **Issued:** 2026-05-16
**Owner:** TechNexus Commercial function · **Confidential — internal only**
**Cadence:** revalidated at every quarterly cost review.

This is the **internal** companion to the public `mwk-price-book.md`. It
shows what one pilot-month + one steady-state-month actually cost
TechNexus to deliver, so deal teams can negotiate informed prices
without underwater margin.

---

## 1. Assumptions for the Malawi pilot baseline

| Assumption | Value |
|------------|------:|
| Pilot duration | 90 days build + 14 days hyper-care |
| Stations deployed at pilot | 1 (Area 18) |
| Officer devices in scope | 4 |
| Officer headcount on system | 7 |
| Citizen volume (fines / month at steady state) | 1,200 |
| % of citizens going through digital channels | 50% |
| TechNexus FTE on project during build | 6.5 (account lead, 2 devs, designer, QA, field engineer 0.5) |
| TechNexus FTE at steady state | 1.8 |
| Average loaded cost / FTE / month (MWK) | 2,800,000 |
| FX reference rate | MWK 1,750 / USD |

---

## 2. Build-phase cost (one-time, ~6 months)

| Bucket | MWK |
|--------|----:|
| Labour — 6 months × 6.5 FTE × 2,800,000 | 109,200,000 |
| Cloud development environment (Azure dev sub, 6 mo) | 1,500,000 |
| QA / pen-test (third-party) | 6,000,000 |
| Training-content production (Chichewa + English) | 1,200,000 |
| Office / overheads attributable to project | 4,500,000 |
| **Build subtotal** | **122,400,000** |
| Hardware import margin (Section B at 12% blended margin) | 3,956,400 |
| **Build phase total** | **126,356,400** |

Section-A quotation revenue at MWK 157,500,000 leaves a build-phase
**contribution margin of MWK 31,143,600 (≈ 19.8%)**.

The Section-B revenue (32,970,000) sits on a thinner ≈12% margin and
mostly funds inventory and shipping risk.

---

## 3. Pilot operating cost (per month, during the 90-day pilot)

| Item | MWK / month |
|------|------------:|
| TechNexus FTE on station 5 d/wk × 4 wk + remote support | 3,200,000 |
| Azure hosting (Malawi-resident region) | 875,000 |
| Backup / DR / monitoring | 175,000 |
| WhatsApp Business API + USSD aggregator | 437,500 |
| Connectivity SIM + airtime (pilot station) | 70,000 |
| Misc cushion (15%) | 715,425 |
| **Pilot ops subtotal / month** | **5,472,925** |

Section-C + D quotation revenue across 12 months = **27,825,000 / 12 = 2,318,750 per month**, so the pilot phase is **deliberately loss-making** during the 90 days (-3.15M / month) and only breaks even once the operation transitions to steady state.

This is *normal* for the pilot. The build-phase margin is the buffer.

---

## 4. Steady-state cost per active station per month

| Item | MWK / month |
|------|------------:|
| Shared engineering / support pool (allocated) | 1,225,000 |
| Cloud (allocated share) | 200,000 |
| Channel fees (allocated share) | 175,000 |
| Spare-parts amortisation (5% spares × 24 mo) | 87,500 |
| **Steady state / station / month** | **1,687,500** |

National rollout (all 28 districts, ~ 29 stations) puts the
steady-state run rate at **48,937,500 / month** in cost, against
expected support-and-services revenue of MWK 65,000,000 / month.
Steady-state contribution margin ≈ **25%**.

---

## 5. Per-fine economics

| | Pilot (1 station) | National (29 stations) |
|---|---:|---:|
| Volume / month | 1,200 | 34,800 |
| Operating cost / fine (MWK) | 4,560 | 1,406 |
| Average revenue / fine (MWK) (channel fees + share) | 1,930 | 1,930 |
| **Pilot net per fine** | **-2,630** | **+524** |

Per-fine economics are negative in the pilot and crossover to positive
**at month 9 of national rollout** (about 20 stations live, ~24,000
fines / month). The 90-day pilot is therefore a customer-acquisition
investment, not a profit centre.

---

## 6. Sensitivity table

How net contribution per month varies with the two biggest drivers:

| FTE cost (MWK/mo) ↓ vs. Volume (fines/mo) → | 800 | 1,200 | 2,000 |
|--:|--:|--:|--:|
| 2,400,000 | -1,975,000 | -1,475,000 | -475,000 |
| 2,800,000 | -2,375,000 | -1,875,000 | -875,000 |
| 3,200,000 | -2,775,000 | -2,275,000 | -1,275,000 |

Reading: if FTE cost stays at 2.8M and citizen-channel volume reaches
2,000 fines / month, the pilot's monthly burn drops to 875k — still
investment, but bearable.

---

## 7. FX risk

Sections A, C, and D are MWK-denominated and FX-neutral.
Section B (hardware) carries the FX exposure. If MWK depreciates by
10% during build, hardware delivery margin compresses from 12% to ~3%.
Mitigation: forward-contract 80% of hardware FX exposure within 5 days
of PO (see `mwk-price-book.md` §9 and the planned `f3` hedging policy).

---

## 8. Discounting authority — re-derived

The MWK price book §8 sets discount caps at 0/2.5/7.5/15%. Re-derived
from this cost model:

| Discount | What's left | Decision |
|---------:|------------:|----------|
| 0% | full 19.8% margin | Account Lead authority |
| 2.5% | ≈17.3% margin | Account Lead authority |
| 7.5% | ≈12.3% margin | Commercial Director sign-off (we accept project-only margin, no growth funding) |
| 15% | ≈4.8% margin | CEO sign-off — only for strategic-reference deals (e.g., the first national tender) |
| > 15% | margin negative | Board resolution required |

---

## 9. Strategic price floors (lines we will not cross)

| Item | Floor |
|------|------:|
| Section A subtotal | MWK 145,000,000 — below this we cannot fund senior engineering quality |
| Officer device (Section B B1) | MWK 295,000 / unit — below this we lose hardware margin entirely |
| Steady-state support / station / month | MWK 1,950,000 — below this we cannot meet P1 SLA |
| Revenue-share rate | 1.5% of collected fines — below this the model becomes loss-making at any volume |

Going below any floor requires explicit Board sign-off. **No exceptions
for relationship reasons.**

---

## 10. Change log

| Date | Change | Author |
|------|--------|--------|
| 2026-05-16 | v1.0 — Malawi pilot baseline | TechNexus Commercial |
