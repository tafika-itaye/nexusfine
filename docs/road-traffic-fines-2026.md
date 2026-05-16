# Road Traffic Fines Schedule — 2026

**Source:** Government Notice No. 38, Republic of Malawi, 8 May 2026
**Authority:** Road Traffic Act (Cap. 69:01), Road Traffic (Prescribed Offences and Penalties) Regulations, 2026
**Issued by:** J. Mhango, Minister of Transport and Public Works (signed 6 May 2026)

The schedule below is the source of truth for `OffenceCode.DefaultFineAmount` in the seed.
Any future revisions land here first, then propagate to `AppDbContext.OnModelCreating`
and trigger a new EF migration.

## Schedule (most common offences)

| # | OC code | Offence | Prescribed fine (MWK) | Notes |
|---|---------|---------|----------------------:|-------|
| 1 | OC-009 | Driving without a licence | 200,000 | |
| 2 | OC-004 | Using an unregistered vehicle | 100,000 | |
| 3 | OC-011 | Failure to display number plates / vehicle ID | 100,000 | |
| 4 | OC-010 | Using phone while driving | 50,000 | |
| 5 | OC-003 | No seatbelt | 15,000 | per passenger |
| 6 | OC-012 | Drunk driving | up to 300,000 | + licence suspension |
| 7 | OC-001 | Speeding | 20,000 – 90,000 | depending on excess speed |
| 8 | OC-013 | No insurance | 100,000 (private) / up to 800,000 (PSV) | |
| 9 | OC-014 | No Certificate of Fitness (COF) | 50,000 | |
| 10 | OC-005 | Unroadworthy vehicle | 50,000 | |
| 11 | OC-015 | Parking illegally / causing obstruction | 30,000 | |
| 12 | OC-002 | Failure to obey road signs | 50,000 | covers red-light violations |
| 13 | OC-016 | Failing to stop for traffic officers | 50,000 | |
| 14 | OC-006 | Dangerous overtaking | 50,000 | |
| 15 | OC-007 | Riding motorcycle without helmet | 30,000 | |
| 16 | OC-017 | Carrying excess passengers on motorcycle | 20,000 | per passenger |
| 17 | OC-018 | Hooting unnecessarily | 30,000 | |
| 18 | OC-019 | Driving while texting or calling | 50,000 | logged separately from OC-010 in the Notice |
| 19 | OC-020 | No warning triangles / fire extinguisher / spare tyre | 10,000 | per item |
| 20a | OC-008 | Carrying unsecured goods / load | 100,000 | |
| 20b | OC-021 | Throwing rubbish from vehicle | 50,000 | |

## Notes on mapping

- **Existing IDs 1–10 preserved.** Renames updated names and amounts in place;
  existing seeded fines that reference these IDs continue to work.
- **`Red Light Violation` (formerly OC-002)** is folded into the broader
  `Failure to Obey Road Signs` consistent with the Notice's wording.
- **Drunk driving (OC-012)** carries an additional licence-suspension consequence
  that is *not* captured in the `DefaultFineAmount` column. When implementing
  the disposition workflow, drunk-driving fines should also mark the driver's
  licence as `SuspensionPending` (entity work tracked under Module 4+).
- **`Per-X` amounts (seatbelt, motorcycle excess passenger, missing safety
  items)** store the per-unit amount as the default; the issuing officer
  multiplies in the app when writing the fine.

## Provenance

Original PDF / image of Government Notice No. 38 should be kept under
`docs/assets/road-traffic-fines-2026-notice.pdf`. Add a digitally-signed copy
when MPS Records officer countersigns the schedule for the pilot.
