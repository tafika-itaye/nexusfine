# Data Protection Impact Assessment (DPIA) — Malawi

**Project:** NexusFine — Digital Traffic Fines Management & Payment Platform
**Controller:** Malawi Police Service / Directorate of Road Traffic and Safety Services
**Processor:** TechNexus MW (Pty) Ltd
**Statutory basis:** Data Protection Act, 2024 (Malawi); Road Traffic Act (Cap. 69:01)
**DPIA version:** v1.0 · 2026-05-16
**Reviewer:** [Data Protection Officer — TBD]

This DPIA has been prepared in advance of the pilot deployment to satisfy
the prior-consultation duty in §31 of the Data Protection Act, 2024 and to
inform the Malawi Data Protection Commission's review.

---

## 1. Description of the processing

### 1.1 Nature
Citizens encounter NexusFine in three ways:
1. They look up a fine using their plate, reference number, or driver's licence.
2. They pay the fine through a citizen-facing channel (web, WhatsApp, USSD, mobile money, bank, card).
3. They receive a PDF receipt by download or chat.

Police officers and supervisors:
1. Issue fines in the field via tablet (Module 4c) or at the desk (Module 3 portal).
2. Reconcile collections in the admin portal.
3. Manage reference data (departments, stations, officers, offence codes).

### 1.2 Scope (data categories)
- **Personal data:** full name, national ID, driver's licence number, phone number, plate number, address (only at issuance), photograph (only at issuance).
- **Special-category data:** none. (No biometrics beyond face photograph; no health or political data.)
- **Financial data:** payment amount, payment channel, mobile-money phone, bank/card reference (tokenised), receipt number.
- **Operational data:** GPS at issuance, NFC tag scanned, officer badge, timestamp.

### 1.3 Context
NexusFine replaces a paper-based ticketing process. The lawful basis is:
- **Article 23(a)** — necessary for compliance with a legal obligation of the controller (the Road Traffic Act).
- **Article 23(d)** — necessary for the performance of a task in the public interest.

### 1.4 Purpose
To digitise the road-traffic enforcement and collection workflow such that
the State recovers fines lawfully due, citizens have a frictionless payment
experience, and the Auditor General has an immutable record of every action.

---

## 2. Necessity & proportionality

| Question | Assessment |
|----------|-----------|
| Is the processing necessary to deliver the purpose? | Yes. The Road Traffic Act requires identification of the offender and assessment of a prescribed penalty. |
| Is each data field strictly required? | Reviewed and reduced. Full address is captured only at issuance and not displayed on public receipts. Photographs are only captured at issuance and never displayed to the public. |
| Could the purpose be achieved with less data? | The set above is the minimum to satisfy enforcement, payment reconciliation, and audit traceability. |
| Retention proportionate? | Yes — see §6. |

---

## 3. Data flows & controllers

| Stage | Who controls | Who processes | Where stored |
|-------|--------------|---------------|--------------|
| Issuance (officer device) | MPS | TechNexus (sub-processor: device OEM patches only) | Tablet SQLite (encrypted, planned), uploaded to Station Server within 2 h or shift end |
| Station Server | MPS | TechNexus | Station NUC (SQLite, encrypted-at-rest) |
| HQ Operations | MPS | TechNexus | Malawi-resident SQL Server (Azure region: Africa-resident or Lilongwe datacentre) |
| Payment posting | MPS + payment gateway | Airtel Money / TNM Mpamba / partner banks | Gateway's own systems for the channel's required retention |
| Treasury settlement | Treasury | Treasury IFMIS | Treasury systems |

No data leaves Malawi. No data is processed by a sub-processor outside Malawi
or the Common Market for Eastern and Southern Africa (COMESA) data-residency
zone. **Section 36** of the Data Protection Act 2024 is therefore not engaged.

---

## 4. Rights of data subjects

| Right | Provision in NexusFine |
|-------|------------------------|
| Be informed (§17) | A privacy notice is displayed on the citizen portal and on the officer-issued paper receipt. |
| Access (§18) | Citizens may request a copy of their record through `dataprotection@mps.gov.mw`. SLA 30 days. |
| Rectification (§19) | Same channel; MPS supervisor processes requests against the audit log. |
| Erasure (§20) | NOT applicable to active investigations or pending court matters; otherwise honoured. |
| Restrict processing (§21) | A "disputed" flag exists on every fine; once set, the citizen-portal balance display is suppressed pending review. |
| Data portability (§22) | A signed JSON export of the citizen's records is available on written request. |
| Object (§23) | Right to object is qualified — fines arise from a statutory obligation. Citizen may dispute the underlying offence through the existing appeals process. |
| Automated decision-making (§24) | NexusFine does not make solely-automated decisions of legal significance. Fines are issued by officers; the system supports but does not replace human judgement. |

---

## 5. Risks identified and mitigations

| # | Risk | Likelihood | Impact | Mitigation |
|---|------|-----------|--------|------------|
| 1 | Citizen plate scraping reveals motorist patterns | Med | Low | Public lookup returns minimal data; rate-limited; full PII gated behind one-time code sent to phone on record. |
| 2 | Officer device theft exposes 12-month operational cache | Med | High | Tablet PIN; SQLCipher; remote-revoke after 72-h heartbeat loss. |
| 3 | Misuse of supervisor access | Low | High | Role-segregated; all writes audit-logged; 2FA mandated for supervisor + admin (S9). |
| 4 | Payment-gateway breach exposes citizen phone numbers | Low | Med | Gateway operators are independently regulated by MACRA; minimum data shared (amount + reference); review their controls quarterly. |
| 5 | Disputed-fine retention conflicts with right to erasure | Low | Low | Retention policy §6 carves out active investigations + 10-year court window. |
| 6 | Cross-border data leakage during sync | Low | High | All sync endpoints terminate within Malawi-resident infrastructure; no cross-border replication. |
| 7 | Audit-log tampering | Low | High | Append-only on a separate database with row-version hashing (S7 planned). |

---

## 6. Retention schedule (cross-reference: `docs/architecture-distributed.md` §12)

| Data class | Station hot | Station warm | HQ |
|------------|-------------|--------------|-----|
| Active operational data | indefinite | indefinite | indefinite |
| Closed fines + completed payments | 12 months rolling | — | 7 years |
| Citizen-lookup index | 12 months rolling | — | indefinite |
| Disputed fines | until resolution + 90 d | — | 10 years post-resolution |
| Audit log (operational) | 90 d | — | indefinite |
| Audit log (security) | 90 d | — | 7 years |
| Photo / NFC / GPS evidence | 12 months | — | indefinite until close + 7 yrs |

Auto-purge at the station is preceded by a "still-synced?" check against HQ
to ensure no data is destroyed until confirmed received upstream.

---

## 7. Outcomes of consultation

To be completed prior to pilot go-live:
- [ ] Internal review with the Office of the Solicitor General
- [ ] MPS Records Officer sign-off on retention schedule
- [ ] MACRA Data Protection Commissioner pre-consultation (§31)
- [ ] Anti-Corruption Bureau acknowledgement (operational data accessible for investigations)
- [ ] Treasury / Accountant General's office on settlement-data flow

---

## 8. Sign-off

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Data Protection Officer (MPS) | | | |
| CISO (TechNexus MW) | | | |
| Director, DRTSS | | | |
| Approver: Inspector General of Police | | | |

---

*This DPIA will be re-issued: (a) annually as a matter of routine; (b)
whenever a new category of personal data is introduced; (c) whenever a new
sub-processor is engaged; (d) whenever the cross-border-transfer position
changes; (e) after any reported personal-data breach.*
