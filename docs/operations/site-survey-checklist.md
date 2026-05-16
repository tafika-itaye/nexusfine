# Pre-Deployment Site Survey Checklist

**Purpose:** Every new police station receives a physical site visit before the
Station Server is dispatched. This checklist standardises what the visiting
engineer assesses, what the station commander confirms, and what TechNexus
commits to deliver. Completed sheets are filed at HQ and accompany the PO.

| Field | Value |
|-------|-------|
| Station code | `STN-___-___` |
| Station name | |
| Survey date | |
| Surveyor (TechNexus) | |
| Station-side witness (MPS) | |
| Address | |
| GPS (lat / lng) | |

---

## 1. Power

- [ ] Mains grid available? (yes / intermittent / no)
- [ ] Hours / day of reliable mains: ____
- [ ] Existing UPS at the station? (yes / no — manufacturer / kVA)
- [ ] Generator on site? (yes / no — fuel type, run-time)
- [ ] Solar capacity already present? (yes / no — kWp, battery kWh)
- [ ] Earth + surge protection at the proposed server location? (yes / no)
- [ ] Power outlets within 2 m of proposed server rack? (count: ___)

**Recommendation block:**
- UPS sized to 30-min runtime: ☐ already adequate · ☐ supply APC Smart-UPS 1500VA
- Solar add-on required (Section E2 quotation): ☐ yes · ☐ no
- Notes: ___________________________________________________________________

---

## 2. Connectivity

- [ ] Fibre or wired ISP available? (provider, speed)
- [ ] 4G/LTE coverage measured at the proposed install point (RSSI dBm: ___)
- [ ] Best Malawian operator at this location: ☐ Airtel · ☐ TNM · ☐ Both equal
- [ ] Dual-SIM hot-swap recommended? ☐ yes · ☐ no
- [ ] Cellular signal at officers' patrol posts (separate rows per post)

**Recommendation block:**
- ISP primary: ☐ install Airtel fibre · ☐ install TNM fibre · ☐ no fibre available
- 4G failover SIM: ☐ Airtel · ☐ TNM · ☐ Both (recommended)
- Coverage gaps at patrol posts: ____

---

## 3. Physical security

- [ ] Lockable cabinet / safe for server NUC? (yes / no)
- [ ] CCTV coverage of server location? (yes / no — DVR retention)
- [ ] Access-control to server room / cabinet? (key + log / smartcard / none)
- [ ] Fire-detection in server area? (yes / no)
- [ ] Suppression / extinguisher within 5 m? (CO₂ / dry powder / water — flag if water)

**Recommendation block:**
- Lockable cabinet supplied by TechNexus: ☐ yes · ☐ already present
- Notes: ___________________________________________________________________

---

## 4. Environmental

- [ ] Ambient temperature in summer (estimated max °C): ____
- [ ] Ventilation / aircon at the server location? (yes / no)
- [ ] Dust exposure (open window, dirt-floor traffic, etc.): ☐ low · ☐ medium · ☐ high
- [ ] Roof leak risk overhead? ☐ none · ☐ minor · ☐ replace mounting

**Recommendation block:**
- Fanless NUC selected to mitigate dust: ☐ yes (default)
- Vented dust-filtered enclosure required: ☐ yes · ☐ no

---

## 5. Officer cohort

- [ ] Officers currently assigned to station (active): ____
- [ ] Officers anticipated in scope for pilot tablets: ____
- [ ] Existing patrol posts (Code · Name · Active Y/N):
  - 1.
  - 2.
  - 3.
  - 4.
- [ ] Shift structure (number of shifts / day, length): ____
- [ ] Officer language preference predominantly: ☐ English · ☐ Chichewa · ☐ both

---

## 6. Counter-payment readiness

- [ ] Existing cash-payment desk? (yes / no)
- [ ] Counter clerk(s) assigned (count): ____
- [ ] Existing receipting book serial range to retire: ____
- [ ] Bank account for daily-tallies (or Treasury sweep destination): ____

---

## 7. Training logistics

- [ ] Room available for 2-day officer training? (yes / no, capacity)
- [ ] Projector + whiteboard available? (yes / no — TechNexus supplies if no)
- [ ] Approximate cost / officer for backfill during training: ____

---

## 8. Identified risks (open)

1. _______________________________________________________________________
2. _______________________________________________________________________
3. _______________________________________________________________________

---

## 9. Decisions & commitments

- TechNexus will supply: _______________________________________________________
- MPS will provide: ____________________________________________________________
- Dependencies blocking PO: ____________________________________________________
- Target deployment date: _____________________________________________________

---

## 10. Sign-off

| | Name | Signature | Date |
|---|---|---|---|
| Surveyor (TechNexus) | | | |
| Station Commander | | | |
| MPS ICT Liaison | | | |

**Next step:** scan and upload the completed sheet to `docs/sites/{stationCode}.pdf`
within 24 hours of survey. Open a TechNexus deploy ticket against the station code.
