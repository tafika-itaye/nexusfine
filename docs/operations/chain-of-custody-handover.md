# Chain-of-Custody Handover Sheet

**Status:** v1.0 · **Issued:** 2026-05-17
**Owner:** TechNexus Operations function
**Use:** every officer device, Station Server, NFC reader, and accessory
is signed in / out using this sheet. Each scanned handover is filed
under `docs/handovers/{YYYY}/{station}-{date}.pdf`.

This is the operational discipline that makes a stolen tablet a crime
report rather than an audit-finding.

---

## Why this matters

Every physical asset that touches a citizen-facing fine is potential
evidence. A defendant may, months after issuance, ask:

- "Who held the tablet that issued my ticket that day?"
- "Has the device ever been re-imaged? When and by whom?"
- "Did anyone other than the officer have access to it?"

Without a chain-of-custody record, we cannot answer those questions.
With it, the audit-log of in-system events is anchored to the physical
custody record outside the system — and a competent prosecutor can
defend a fine in court.

---

## 1. Asset types covered

| Class | Examples | Mandatory? |
|-------|----------|:----------:|
| Officer device | Samsung Galaxy Tab A9 + rugged case + battery pack | ✅ |
| Station Server | Intel NUC + UPS + 4G dongle | ✅ |
| Authentication accessory | NFC officer-ID tag, supervisor key card | ✅ |
| Mobile reader | NFC card reader | ✅ |
| Sensitive consumable | Paper ticket-book (Plan-B fallback) | ✅ |
| Connectivity SIM | Airtel / TNM 4G SIM | ✅ (each by SIM number) |
| Non-sensitive consumable | Charging cable, screen protector | ❌ (inventory only) |

---

## 2. The handover sheet (per event)

```
TECHNEXUS MW × MALAWI POLICE SERVICE — DEVICE HANDOVER
══════════════════════════════════════════════════════════════════

Sheet ID:           HOC-_______________   (sequential, station-prefixed)
Station:            _______________________________________________
Date / time:        ______ / ______ / 2026   ____ : ____  CAT
Reason (tick one):  ☐ Issue to officer (shift)
                    ☐ Issue to officer (extended assignment)
                    ☐ Return from officer (shift end)
                    ☐ Reassignment between officers
                    ☐ Send to TechNexus (repair / re-image)
                    ☐ Receive from TechNexus (return / replacement)
                    ☐ Audit / forensic hold
                    ☐ Decommission

ASSET
──────────────────────────────────────────────────────────────────
Type:         ____________________  Asset tag:  ___________________
Serial:       ____________________  IMEI:       ___________________
Last status:  ____________________  Image SHA:  ___________________

CONDITION ASSESSMENT (handover-OUT)
──────────────────────────────────────────────────────────────────
Screen intact?           ☐ Y  ☐ N  notes: _____________________
Body intact?             ☐ Y  ☐ N  notes: _____________________
Charger / cables (count) ___ / ___
Battery health (%):      ___
SIM(s) installed:        ___  Carrier(s): _____________________
NFC pairing token:       ☐ active  ☐ revoked  ☐ pending
Photo of device taken?   ☐ Y  (attach print or filename)

FROM (custodian releasing)
──────────────────────────────────────────────────────────────────
Name:        _______________________________  Badge: ____________
Role:        _______________________________
Signature:   _______________________________  Time: ____________

TO (custodian receiving)
──────────────────────────────────────────────────────────────────
Name:        _______________________________  Badge: ____________
Role:        _______________________________
Signature:   _______________________________  Time: ____________

WITNESS
──────────────────────────────────────────────────────────────────
Name:        _______________________________  Badge: ____________
Signature:   _______________________________

ACKNOWLEDGEMENT
──────────────────────────────────────────────────────────────────
I confirm I have personally inspected the asset above. I am
authorised to receive (or release) it under TechNexus Policy
OPS-COC-V1. Any prior or new damage, loss, or disagreement is
recorded in the notes.

NOTES (open):
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
```

A4 portrait, one event per sheet. Two-part (carbon) recommended:
station retains one copy, TechNexus collects the other weekly.

---

## 3. Process flow — daily shift

```
   06:30 — Officer arrives, presents badge
        ▼
   Station Commander opens cabinet, retrieves device
        ▼
   Both inspect, sign sheet (HOC-IN)
        ▼
   Officer takes device on patrol
        ▼
   18:00 — Officer returns, presents device
        ▼
   Both inspect, sign sheet (HOC-OUT)
        ▼
   Device locked in cabinet
        ▼
   Field engineer pulls signed pair into the weekly batch
```

Time-cost: ~ 90 seconds per handover. Two events per officer per day.
Cabinet design includes a labelled rack and the sheet pad on a chain.

---

## 4. Process flow — extended events

| Event | Additional step |
|-------|-----------------|
| Reassignment between officers | Mid-day; both officers + Pilot Champion as witness; revoke + re-pair the device session |
| Repair to TechNexus | Field Engineer signs as receiving custodian; asset travels in a tamper-evident pouch |
| Re-imaging | New `Image SHA` recorded before reissue; old SHA noted for the audit trail |
| Audit / forensic hold | TechNexus and Police case officer co-sign; device is sealed in tamper-evident bag with case number |
| Decommission | Device wiped + Station Server cert revoked; certificate of destruction issued |

---

## 5. Reconciliation cadence

| Cadence | Action |
|---------|--------|
| Daily | Station Commander tallies open sheets — any device unaccounted-for is escalated to TechNexus on-call by 19:00 |
| Weekly | Field engineer collects signed sheets, scans, files them; HQ reconciles against the asset register |
| Monthly | TechNexus Ops Manager reviews any anomalies; reports to MPS ICT in writing |
| Quarterly | Full physical audit at one station (rotation); findings appended to the quarterly SLA report |

---

## 6. Loss / theft

Per `spare-parts-inventory.md` §6:
- Report within 1 business day to TechNexus Ops + Station Commander.
- Open a police case.
- Revoke device + cert immediately in NexusFine.
- Replacement from regional hub within 24 h.
- Loss debited against the project P&L; >0.5% of fleet value in a
  quarter triggers a COO review.

The signed handover sheet associated with the most-recent custody is
**evidence in the police case**. It is preserved unchanged (do not
amend after a loss) and a copy is provided to the case officer.

---

## 7. Digital equivalent (Phase 2)

Within 6 months of pilot acceptance, handover sheets move into the
admin portal as a first-class entity (`AssetHandover`). The paper
form survives as a Plan-B fallback during connectivity outages, but
the digital path becomes primary. The Phase-2 design is:

- Asset tag scanned (QR or RFID).
- Both parties tap their officer NFC tags on the device that's being
  handed over.
- A condition checklist is completed on screen.
- Server records the event with a timestamp + IP and emits an audit-log entry.
- A receipt is printed (optional) for the station's records.

Designed in `docs/templates/asset-handover-digital-v2.md` *(to ship in Module 4c)*.

---

## 8. References

- `docs/operations/spare-parts-inventory.md` §6 — loss protocol
- `docs/operations/pilot-playbook-90d.md` §7 — Day-1 readiness uses this sheet
- `docs/security/threat-model.md` 2.2 — Officer device threats
- `docs/security/append-only-audit-log.md` — system-side records that this paper anchors
