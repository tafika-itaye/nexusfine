# Spare-Parts Inventory Model

**Status:** v1.0 · **Issued:** 2026-05-17
**Owner:** TechNexus Operations function · **Cadence:** reviewed quarterly + after any P1
**Applies to:** every TechNexus-deployed NexusFine site (pilot Lilongwe; expands to all 28 districts)

This document is the standard for what spares we hold, where we hold them,
who can pull from stock, and how the inventory is replenished.

---

## 1. Policy summary

| Component | Stock level | Rule |
|-----------|-------------|------|
| Officer tablets (Samsung Galaxy Tab A9 + rugged case) | **5% of deployed fleet**, minimum 2 per station | Rotate FIFO; oldest spare goes out first |
| Tablet batteries (swappable) | **10% of deployed fleet**, minimum 4 per station | Each officer carries 2 swappable; spares are 3rd and 4th |
| Tablet chargers + cables | 20% of deployed fleet | Cables fail more often than chargers |
| NFC officer-ID tags | 10% of deployed fleet, minimum 5 per station | Single replenishment SKU |
| Mobile NFC card readers | 10% of deployed reader fleet | Higher than tablet rate because they get handled rougher |
| Station Server (full unit, pre-imaged) | **1 per region** (Northern / Central / Southern) | Held at TechNexus HQ Blantyre + Lilongwe |
| Station Server NVMe drive | 2 per region | Cheaper than full units; covers most failures |
| Station Server UPS battery | 1 per station + 2 per region central pool | Battery is the most common failure point |
| 4G failover dongle + spare SIMs | 1 per station + 5 per region | SIMs in both Airtel and TNM |
| Network router (admin) | 2 per region | Pre-configured matching the in-station unit |
| Supervisor laptop | 1 per region pool (not per station) | Rare failure; pooled |
| Printer (counter receipt) | 1 per station | Optional Section-E item |

> **Why 5% tablets / 10% batteries?** Tablet field-failure-rate observed in
> comparable African deployments runs ~3% per year (drop damage,
> water ingress, screen failure). 5% gives a 6-month buffer
> at typical replenishment cadence. Batteries degrade faster
> (~8% / year capacity loss), so the 10% pool absorbs the swap rate
> without procurement scramble.

---

## 2. Inventory locations

```
                       TechNexus HQ Blantyre
                       (master pool: 50% of all stock)
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
        Northern hub     Central hub      Southern hub
        (Mzuzu)          (Lilongwe)       (Blantyre HQ co-located)
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
        Pilot station    +27 other stations as rolled out
        (Area 18)
        — 5% / 10% of locally-deployed quantities held on site —
```

- Each **regional hub** is a locked cabinet at one nominated police
  station per region, accessible to TechNexus field engineers + the
  Station Commander.
- Each **station** holds the local-floor stock in the same lockable
  cabinet that hosts the Station Server.
- HQ Blantyre holds the master pool + import buffer.

---

## 3. Custodianship

| Stock layer | Custodian | Backup custodian |
|-------------|-----------|------------------|
| HQ Blantyre | TechNexus Ops Manager | CFO |
| Regional hub | TechNexus Regional Field Engineer | Hub station commander (signed off) |
| Station local | Station Commander | Pilot Champion (Inspector rank+) |

Every item moves with a signed handover sheet. No exceptions, including
"emergency" moves at 02:00 — a verbal handover is recorded the next
business day or the move is treated as a loss.

---

## 4. Replenishment triggers

| Trigger | Action | Lead time |
|---------|--------|-----------|
| Station local stock drops below 60% of policy | Regional hub ships replacement | 24 h |
| Regional hub drops below 60% of policy | HQ ships replacement | 48 h |
| HQ stock drops below 25% of policy OR drops below 2 of any pre-imaged Station Server | Reorder placed with supplier | per vendor SLA |
| Any P1 incident consumes a spare unit | Reorder placed regardless of stock level | next business day |

Reorder amounts target restoring stock to **120% of policy**, not 100%.
This absorbs the next P1 without immediately triggering another
reorder.

---

## 5. Movement events (audit trail)

Every stock event is recorded in a Google Sheet (today) — moving to a
proper Asset entity in NexusFine Phase 2. Today's columns:

| Date | Item / serial | From | To | Reason | Officer / engineer | Signed |
|------|---------------|------|-----|--------|----------------------|--------|

A movement is closed (signed by both ends) within 24 hours or it is
treated as a loss event and reported.

---

## 6. Loss / theft / damage protocol

1. Custodian reports within 1 business day to the TechNexus Ops Manager
   AND the local Station Commander.
2. A police case is opened by the Station Commander.
3. The asset's serial is **immediately revoked** in NexusFine (so a
   stolen device cannot pair with any station).
4. A replacement is dispatched from regional hub within 24 hours.
5. Loss is debited against the project P&L; if it exceeds 0.5% of the
   deployed fleet value in any quarter, the COO is notified and the
   custodianship rules are reviewed.

---

## 7. Pre-imaging discipline (Station Servers + tablets)

A "ready" spare means the unit:

- Has the latest signed NexusFine image installed (cross-checked against
  the SHA-256 release manifest)
- Has the latest mTLS station cert and its station-code config blanked
  out (paired only at deploy)
- Has been bench-tested with `scripts/station-smoke.ps1` before
  shipment
- Carries an asset tag matching the inventory record

Reimaging cadence: **every 8 weeks** for unused spares, or sooner when a
critical update lands.

---

## 8. Cost model (high level)

Tracked in `docs/commercial/cost-to-deliver-model.md` §4 under
"spare-parts amortisation". Indicative monthly amortisation, national
rollout:

| Item | Units in spare pool | Replenishment / year | MWK / month |
|------|--------------------:|---------------------:|------------:|
| Officer tablets | 30 | 25% | 87,500 |
| Batteries | 60 | 40% | 12,500 |
| NFC tags | 60 | 30% | 5,250 |
| Mobile readers | 12 | 25% | 13,125 |
| Station Server | 3 | 10% | 35,000 |
| UPS battery | 32 | 20% | 18,375 |
| 4G dongles + SIMs | 33 | 30% | 6,125 |
| Misc cables / accessories | n/a | — | 5,250 |
| **Total / month (national)** | | | **183,125** |

Round to **MWK 200,000 / month** in the commercial book for buffer.

---

## 9. Procurement preferences

1. Local supplier first (Lilongwe / Blantyre).
2. Re-direct second (regional distributor in JNB / DAR).
3. Import via consolidated freight only if (1) and (2) are not
   available within 7 days.

Approved suppliers list maintained at HQ. Updated annually.

---

## 10. References

- `docs/operations/sla-tiers.md` — incident-response SLAs that drive
  spare-pull frequency
- `docs/operations/pilot-playbook-90d.md` — Phase-1 readiness checklist
  references spare-parts stock
- `docs/commercial/cost-to-deliver-model.md` §4 — financial model
