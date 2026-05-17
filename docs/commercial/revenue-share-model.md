# Revenue-Share Pricing Model

**Status:** v1.0 · **Issued:** 2026-05-17
**Owner:** TechNexus Commercial function · **Authority:** Board of Directors
**Companion to:** `mwk-price-book.md` §10, `cost-to-deliver-model.md`

This is the **alternative** commercial model TechNexus offers ministries
that prefer to avoid the up-front capex profile of the conventional
pricing book. It is **opt-in only** — never volunteered without
ministry interest first.

---

## 1. Why we offer it

The conventional book requires the client to fund roughly MWK 218M up
front (split 30/30/30/10 across the build). For some ministries — the
ones currently running on a constrained budget vote — that capex
profile is the deal-breaker, not the merits.

Revenue-share offers the State a **near-zero up-front commitment** for
the **operating** services in exchange for a small percentage of fines
actually collected. Hardware remains procured separately (still up-front
or on instalment) because hardware does not "earn" revenue.

The model only works because NexusFine **generates** revenue. It would
not apply to a different government IT project that is pure cost
recovery (e.g. an identity registry).

---

## 2. The structure

### 2.1 Pricing
| Period | TechNexus share of **collected fines** | Notes |
|--------|-----------------------------------------:|-------|
| Year 1 — pilot + first national year | **2.0%** | Higher to recover build cost |
| Year 2 | **2.0%** | Same |
| Year 3 | **2.0%** | Same |
| Year 4 onward | **0.75%** | Steady-state maintenance + uplift only |
| Year 10 | Renegotiate | Default rollover at year-9 rates |

### 2.2 Floors and caps
| | MWK / month |
|--|------------:|
| Floor (TechNexus minimum) | **7,500,000** |
| Cap (ministry maximum) | **35,000,000** |

The floor protects TechNexus when fine volumes are low (e.g. during a
rainy-season operational slow-down). The cap protects the State when
volumes are unexpectedly high (e.g. after an enforcement campaign).

### 2.3 Settlement frequency
- **Monthly,** netted against the Daily Settlement File
  (`treasury-reconciliation-clause.md` Clause 14).
- Share computed on the previous-month closed-fine **collected** total.
- Disputed fines that resolve later are netted in the following month.

### 2.4 Hardware
- Procured at standard Section-B prices.
- May be financed by TechNexus's preferred bank over 24 months at the
  prevailing commercial rate. Optional.

### 2.5 Annual services & training
- Sections C & D **discount to 50%** of the price-book values for
  revenue-share customers, since the rev-share takes the place of those
  margins. Training cohorts beyond the first remain at standard cost.

---

## 3. Worked example — Malawi national rollout

Assumptions (pessimistic, "P50"):

| Year | Stations live | Avg fines / station / month | Avg MWK / fine | Monthly collected |
|------|--------------:|----------------------------:|---------------:|------------------:|
| Y1 | 5 → 12 | 1,200 | 45,000 | ramp from 270M → 648M |
| Y2 | 12 → 22 | 1,400 | 47,500 (CPI) | 800M → 1.46B |
| Y3 | 22 → 29 | 1,500 | 50,000 | 1.65B → 2.18B |
| Y4 | 29 (steady) | 1,600 | 52,500 | 2.44B |

TechNexus monthly receipts (capped at 35M):

| Year | At 2% | After cap |
|------|------:|----------:|
| Y1 avg | ~ 9.2M | **9.2M** |
| Y2 avg | ~ 22.6M | **22.6M** |
| Y3 avg | ~ 38.3M | **35.0M** *(cap binding)* |
| Y4 (steady, 0.75%) | ~ 18.3M | **18.3M** |

10-year cash-flow comparison vs the conventional book:

| Model | Total TechNexus take, 10 years | Total ministry outlay, 10 years |
|-------|------------------------------:|--------------------------------:|
| Conventional book (price-book + annual services) | **MWK 1.45B** | **MWK 1.45B** |
| Revenue share | **MWK 1.41B** | **MWK 1.41B** |

By design, **revenue-neutral over 10 years** to TechNexus (within ±5%
under nominal assumptions). The State trades long-term cost equivalence
for the cash-flow profile that suits its budget.

---

## 4. Risk and where it lives

| Risk | Conventional book | Revenue share |
|------|------------------|---------------|
| Volume below forecast | Ministry over-paid relative to actual use | TechNexus under-recovers; **floor** mitigates |
| Volume above forecast | Ministry under-paid; we gain | TechNexus over-recovers; **cap** mitigates |
| Build-cost over-run | Ministry exposed via change-orders | TechNexus exposed; **fixed-floor** prevents bleeding |
| Counter-party default | Ministry must pay invoices | TechNexus has automatic netting at source |
| FX swing (hardware) | Variation clause | Same — hardware is separate |

Net effect: revenue-share **transfers** volume risk from the ministry
to TechNexus, in return for **transferring** budget-cycle risk from
TechNexus to the ministry. This trade is appropriate when:

- the State has tight short-term budget but high political will, OR
- the State wants to align supplier incentives to actual citizen-facing performance.

---

## 5. Eligibility (when we will offer it)

We require **all of**:

1. Five-year minimum commitment (we cannot recover the build cost in
   anything shorter).
2. Single-source for the contract period (no parallel vendor running
   the same service alongside).
3. Treasury reconciliation at Clause-14 daily cadence — non-negotiable
   under this model since collection visibility *is* the revenue
   visibility.
4. Performance security of MWK 25M from TechNexus (substituting the
   conventional 10% performance security on a capex contract).
5. Audit access for PPDA + AG at the same Clause 14 cadence.

We **decline** revenue share when:

- The ministry's collected-fine forecasts cannot be benchmarked (no
  historical paper data, no realistic comparator).
- The pilot pipeline does not include at least one anchor station with
  a clear catchment.
- The Treasury reconciliation is anything less than daily.

---

## 6. Commercial guard-rails

| Guard-rail | Why |
|-----------|-----|
| Quarterly true-up | Catches reconciliation drift before it compounds |
| Hardware separately financed | Hardware does not "earn" — bundling distorts incentives |
| Annual MERA-pegged CPI adjustment | Maintains real-value of the floor over a decade |
| Termination on 12-month notice | Protects both parties from policy change |
| Cap re-negotiation triggers | Volume above 130% of forecast for 6 months → both parties can re-open the cap |

---

## 7. Internal approval matrix

Per `cost-to-deliver-model.md` §8:

| Decision | Approver |
|----------|----------|
| Offer revenue-share at all | Commercial Director |
| Floor below MWK 7.5M / month | CEO |
| Cap above MWK 35M / month | CEO |
| Year-1 rate below 2% | CEO + Board (revenue at risk) |
| Year-4+ rate above 1.0% | Commercial Director |
| Commitment under 5 years | CEO (with documented exception) |

No exceptions are granted on the **floor** without the Board's
explicit minute. The floor is what keeps the model from becoming
loss-making.

---

## 8. How to introduce it in a meeting

Suggested flow when the ministry's procurement officer raises budget
constraints:

1. Confirm the underlying issue is *cash flow*, not *price*.
2. Frame revenue-share as **optional**, alongside the conventional book.
3. Walk through §3 — show the 10-year revenue-neutrality.
4. Note the alignment-of-incentives angle (we get paid when they
   collect; they want us collecting).
5. Bring §5 to the conversation — these are not pre-conditions we are
   inventing, they are the discipline that makes the model work for both
   sides.
6. Offer the choice in writing. Let them go away and discuss internally.

---

## 9. References

- `docs/commercial/mwk-price-book.md` §10 — preview of this model
- `docs/commercial/cost-to-deliver-model.md` §§ 6–8 — cost basis
- `docs/commercial/treasury-reconciliation-clause.md` Clause 14
- `docs/compliance/ppda-malawi-procurement-map.md` §1 — PPDA-method
  implications
