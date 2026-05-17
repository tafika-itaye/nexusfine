# Court-Evidence Retention Policy

**Status:** v1.0 · **Issued:** 2026-05-17 · **Authority:** TechNexus + MPS Records Officer (signature pending)
**Owner:** TechNexus Compliance function + MPS Records Officer (joint)
**Cadence:** annual review, plus on any change in primary legislation.

NexusFine generates and holds records that may later become evidence in
civil disputes, criminal prosecutions, or audits by the Auditor General,
the Anti-Corruption Bureau, or MACRA's Data Protection Commissioner.
This policy defines, per record class, **how long we hold what**, **in
what state**, and **what we destroy when**.

It complements `docs/compliance/dpia-malawi.md` §6 by adding the
prosecutorial and audit-evidence framing.

---

## 1. Statutory anchors

| Source | What it requires |
|--------|------------------|
| Road Traffic Act (Cap. 69:01) + GN 38 / 2026 | Records of fine issuance, payment, and disposition. Time limits on appeal. |
| Data Protection Act, 2024 | Personal data retained only as long as necessary for the purpose. Subject rights (access, rectification, erasure with carve-outs). |
| AML/CFT Act, 2006 | Financial transaction records ≥ 7 years. |
| Penal Code + Criminal Procedure & Evidence Code | Evidence retained until final disposal of any related criminal matter + a prescribed grace period. |
| Public Audit Act | Audit-trail records preserved for the Auditor General. |
| Public Finance Management Act, 2003 | Government-revenue records preservation. |
| ISO 27001 control 8.15 (Logging) | Minimum logging retention disciplines. |
| Limitation Act (civil) | Civil claims time-bars (6 years for contract; 12 for some statute-based). |

The policy below is calibrated to the **most-protective** of the
applicable sources for each record class.

---

## 2. Record-class retention table

| Class | Hot store (station) | Warm store (HQ) | Cold store (HQ archive) | Trigger to start counting |
|-------|--------------------:|----------------:|------------------------:|---------------------------|
| **Fines** — closed (paid or cancelled) | 12 months rolling, auto-purge | 7 years online | 10 years total | Close date |
| **Fines** — disputed / appealed | until resolution + 90 days | 10 years post-resolution | indefinite cold | Appeal resolution date |
| **Fines** — overdue (unpaid past due date) | indefinite while open | indefinite while open | n/a | n/a |
| **Payments** — completed | 12 months rolling | 7 years online | 10 years cold | Completion date |
| **Payments** — reversed | 24 months hot | 10 years online | indefinite cold | Reversal date |
| **Citizen-lookup index** | 12 months rolling | indefinite | n/a | Last update |
| **Officer audit log** (shift events, status changes) | 90 days | indefinite | n/a | Event date |
| **Security audit log** (login, RBAC, permission denied) | 90 days | 7 years online | indefinite cold | Event date |
| **Mutation audit log** (POST / PUT / PATCH / DELETE) | 90 days | indefinite (append-only, hash-chained, see S7) | n/a | Event date |
| **Photo / NFC / GPS evidence** attached to a fine | 12 months | indefinite until fine closed | + 7 years after close | Fine close date |
| **Officer face photo** (D-014) | not held locally | duration of service + 7 years | n/a | Officer separation date |
| **Officer training records** | 12 months | duration of service + 7 years | indefinite cold | Training completion date |
| **Chain-of-custody sheets** (paper + scans) | 90 days (paper at station) | 10 years (scan at HQ) | indefinite cold | Event date |
| **Sync events** (HQ ↔ Station, Station ↔ Device) | 90 days | 12 months online | 7 years cold | Event date |
| **Settlement files** (daily Treasury reconciliation) | n/a | 7 years online | indefinite cold | Settlement date |
| **System backups** | n/a | 30-day rolling local + 7-year off-site | indefinite air-gap | Backup date |
| **Personal-data subject-rights requests** | n/a | 6 years | n/a | Closure date |
| **Whistle-blower case files** | n/a | 10 years | indefinite cold | Case closure |
| **DPIA + DECISIONS log + threat model** | n/a | indefinite | n/a | Indefinite (versioned) |

Hot / warm / cold definitions:
- **Hot:** queryable in real time by the operating UI.
- **Warm:** queryable by ops + audit consoles with normal SLAs.
- **Cold:** queryable on request only, typically within 5 banking days,
  may live on encrypted off-site tape / cloud-glacier.

---

## 3. Auto-purge discipline

Three rules govern any automated deletion:

1. **Sync-confirmed first.** Hot-store rows are only purged after the
   warm store has confirmed receipt (sync-event row exists with status
   "OK"). No exceptions.
2. **Hold flags.** Any row with a `legalHold = true` flag is excluded
   from auto-purge regardless of age. Hold flags are set only by:
   - the legal team (litigation or anticipated litigation)
   - the Anti-Corruption Bureau liaison
   - the Auditor General's office
   - the Data Protection Commissioner
   Hold flags are recorded in the audit log with the requesting office.
3. **Subject-rights override.** Erasure requests under §20 of the DPA
   2024 take priority **except** where the record relates to:
   - an active criminal investigation
   - an ongoing dispute
   - a financial record within its statutory minimum retention window
   In those cases, the subject is notified that the request is refused
   under DPA §20(c) with reasons.

---

## 4. Cold-store discipline

Cold archives ship monthly. Each archive is:

- A signed, encrypted ZIP / 7z bundle.
- Hash-anchored to the production audit-log chain at the time of export.
- Encrypted with an AES-256 key held in escrow by **two** parties
  (TechNexus + MPS Records Officer). Both keys are required to decrypt.
- Stored at **two** physically separated locations (HQ vault + MPS
  archive room).
- Indexed by record class + period + chain anchor.

The archive index is **never** purged — it is the only evidence that
particular records ever existed.

---

## 5. Legal-hold workflow

```
   1. Request received (court order, ACB letter, DPC notice, subpoena)
   2. Compliance Officer logs it as a HOLD-EVENT ticket
   3. The relevant entity ID(s) are flagged legalHold=true
   4. Auto-purge skips the flagged rows nightly
   5. Hold remains until written release by the originating authority
   6. Release: legalHold=false; row resumes normal retention schedule
   7. Audit-log records the start and end of every hold
```

If the request asks for record **production** (delivery to court):

- Records are exported in their original form (no normalisation).
- The export carries a SHA-256 digest signed by the Compliance Officer.
- A summary letter accompanies the export listing every entity,
  schema version, and applicable retention class.
- Delivery is by sealed courier or government-secure channel — never
  email.

---

## 6. Destruction discipline

When records reach end-of-retention and no hold is in force:

| Media | Method | Witness |
|-------|--------|---------|
| Database row | DELETE with transaction id captured | Compliance + ops |
| File (photo, scanned doc) | Secure overwrite (DoD 5220.22-M 3-pass) then delete | Compliance + ops |
| Paper (chain-of-custody, training register) | Cross-cut shredding to 1mm or finer | Compliance + Station Commander |
| Backup tape / off-site bundle | Crypto-erase (revoke escrowed key) + physical destruction at end-of-cycle | Compliance + MPS Records Officer |

A **Destruction Certificate** is issued for each batch, signed by both
TechNexus and MPS, recording the date, classes of record, count,
hash anchors, and the witnesses. The certificate itself is retained
indefinitely.

---

## 7. Annual review

Each January the Compliance Officer:

1. Re-confirms the statutory anchors are current.
2. Re-confirms the retention table against current jurisprudence.
3. Reviews the previous year's hold events.
4. Reviews any subject-rights requests refused; quality-checks the
   refusal reasons.
5. Issues a one-page status note to the Board + MPS Records Officer.

---

## 8. Open clarifications

These four items remain to be ratified with the Office of the
Solicitor General before pilot go-live:

1. **Photo evidence on a withdrawn fine** — keep until the *original
   ticket's* retention period expires, or destroy at withdrawal?
2. **Audit-log purging at station tier (90 d)** — does the AG's office
   want it retained at 180 d to ease investigations of stale matters?
3. **Subject-rights workflow** — agreed channel: `dataprotection@mps.gov.mw`
   or a dedicated TechNexus address? Recommendation in DPIA is the
   former.
4. **Legal-hold notification window** — proposed 5 business days
   between receipt of request and confirmation back to the originating
   authority. To be confirmed.

---

## 9. References

- `docs/compliance/dpia-malawi.md` — DPA-2024 alignment
- `docs/security/append-only-audit-log.md` — chain-hashed audit
  preservation
- `docs/architecture-distributed.md` §12 — retention figures
- `docs/operations/chain-of-custody-handover.md` — paper-record
  retention
- `docs/commercial/treasury-reconciliation-clause.md` 14.7.2 — 7-year
  reconciliation-record requirement
