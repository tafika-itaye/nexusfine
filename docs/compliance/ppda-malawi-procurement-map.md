# PPDA Malawi — Public-Procurement Compliance Map

**Status:** v1.0 · **Issued:** 2026-05-17
**Authority:** Public Procurement and Disposal of Public Assets Act, 2017 (PPDA Act)
**Regulator:** Public Procurement and Disposal of Public Assets Authority (PPDA)
**Owner:** TechNexus Commercial + Legal · **Cross-reference:** `gr/malawi-ministry-approach.md`

The PPDA Act and its 2020 Regulations govern every transaction TechNexus
enters into with the Malawian State. This map shows which clauses apply
to the NexusFine pilot, what evidence each clause requires from us, and
where to file that evidence so the procurement officer can find it
without a phone call.

> If a clause is **not** captured here, treat the deal as out of compliance
> until Legal confirms otherwise.

---

## 1. Threshold map (FY 2026/27)

PPDA sets monetary thresholds that determine which procurement method is
available. Indicative values as published by PPDA on 2026-03; reconfirm
quarterly with the procurement officer.

| Method | Threshold (MWK) | Decision authority | Notes |
|--------|----------------:|--------------------|-------|
| Open National Bidding (ONB) | > 250,000,000 | PPDA Internal Procurement & Disposal Committee | Default for NexusFine national rollout |
| Restricted Tender | 50M – 250M | Procuring Entity (MPS) IPDC | Pilot range, requires PPDA approval |
| Request for Quotations (RFQ) | 5M – 50M | Procuring Entity IPDC | Pilot Section-E add-ons may fall here |
| Direct Procurement | < 5M, OR exceptional circumstances (PPDA Reg 84) | PPDA prior written approval | Rare; only justifiable for security or single-source |
| Framework Agreement | n/a | Government-wide | Possible national-rollout vehicle |

**NexusFine pilot positioning.** Grand total MWK 218,295,000 places the
pilot at the top end of the Restricted Tender band. Recommendation:
go to **PPDA for ONB designation** to remove ambiguity and pre-empt
challenges.

---

## 2. Pre-bid compliance (what we hold ready)

Before any tender is submitted, TechNexus must hold the following
documents current and on file. Procurement officers can reject a bid
for missing any one of them.

| Document | Issued by | Renewal | Filed under |
|----------|-----------|---------|-------------|
| Certificate of Incorporation | Registrar General | one-off | `docs/compliance/corporate/incorporation.pdf` |
| MSME certificate (Small Enterprise) | Ministry of Trade | annual | `docs/compliance/corporate/msme.pdf` |
| Tax Clearance Certificate (TCC) | Malawi Revenue Authority | annual | `docs/compliance/corporate/tcc-2026.pdf` |
| TPIN registration | MRA | one-off | `docs/compliance/corporate/tpin.pdf` |
| MSE Bank Account & RBM-clearance letter | Reserve Bank of Malawi | one-off | `docs/compliance/corporate/rbm-clearance.pdf` |
| Letter of good standing (PPDA register) | PPDA | annual | `docs/compliance/corporate/ppda-good-standing-2026.pdf` |
| Statutory pension (NPF) compliance letter | NPF | annual | `docs/compliance/corporate/npf-2026.pdf` |
| Workers' Compensation compliance | Ministry of Labour | annual | `docs/compliance/corporate/workers-comp-2026.pdf` |
| Anti-bribery declaration | TechNexus (self) | annual | `docs/compliance/whistleblower-mechanism.md` references |
| Audited financial statements (3 yrs) | TechNexus auditor | annual | `docs/compliance/corporate/afs-2024.pdf` etc. |
| Insurance — Professional Indemnity | Insurer | annual | `docs/compliance/corporate/pi-2026.pdf` |
| Statement of Capacity (refs + similar projects) | TechNexus | per bid | `docs/compliance/corporate/capacity-2026.pdf` |
| Conflict-of-Interest declarations (per §27) | All directors + project leads | per bid | `docs/compliance/conflict-of-interest-register.md` |

A **bid pack template** lives under `docs/compliance/bid-pack/` and is
the single zip TechNexus issues per RFP.

---

## 3. Mandatory clauses to look for in any RFP (what we must comply with)

The procurement officer is duty-bound to include these. If they are
missing, ask why before submitting:

| RFP clause | Origin | What it asks for |
|------------|--------|------------------|
| Conflict of interest declaration | PPDA Act §27 | Signed declaration that no member of the bidding team has a conflict |
| Anti-bribery undertaking | Corrupt Practices Act §32 + ISO 37001 | Signed commitment |
| Conditions for cancellation | PPDA Reg 56 | Re-state the procuring entity's right to cancel |
| Local-content preference | PPDA Reg 76 + MSME Policy | MSME margin of preference up to 10% |
| Bid security | PPDA Reg 60 | Usually 2–5% of bid value, refundable, bank-guaranteed |
| Performance security | PPDA Reg 87 | 10% of contract value, retained until acceptance |
| Liquidated damages | PPDA Reg 88 | Cap typically 10% of contract value |
| Dispute resolution | PPDA Reg 99 | Tiered: internal → PPDA Review Panel → courts |
| Data-protection obligations | DPA 2024 §38 | Sub-processor & cross-border-transfer terms |

If any of these is **missing** from the RFP, lodge a request for
clarification before bid submission — silence is later read as
acceptance.

---

## 4. Bid-submission workflow (single-tender pilot)

```
   T-30  · Notice of intent published on PPDA portal
   T-20  · Pre-bid clarification window (we attend, ask questions)
   T-10  · Bid pack assembled, internal sign-off (CEO + Legal)
   T-3   · Wet-ink signatures on the bid forms
   T-1   · Bid security secured at bank (NBM / NBS preferred)
   T0    · Bid hand-delivered to MPS Procurement Office (with copy
           to PPDA) before the published closing time
   T+0   · Bid opening (public; TechNexus may attend)
   T+14  · Bid evaluation (PPDA + MPS IPDC)
   T+21  · Notification of award (or non-award) in writing
   T+28  · Standstill period closes; contract negotiation begins
   T+35  · Contract signed; bid security returned
   T+40  · Performance security lodged; PO issued; pilot begins
```

Each gate has a documented owner inside TechNexus and a corresponding
checkpoint in the change-management plan (`change-management-template.md` §3).

---

## 5. Margin of preference (MSME)

PPDA Regulations and the 2019 MSME Policy permit a **margin of
preference** of up to **10%** of bid value in favour of an MSME-
certified supplier in restricted or open tenders.

To claim the preference, the bid pack must include:

- Current MSME certificate (Small Enterprise band)
- Cover letter from TechNexus's directors confirming local-content
  composition: % Malawian directors, % Malawian staff, % Malawian
  hardware sourcing, % Malawian intellectual-property
- A signed undertaking that the preference will not be used to mask
  sub-contracting to a non-Malawian primary

We **always** claim the preference. It is one of the strongest reasons
TechNexus wins competitive tenders.

---

## 6. Conflict-of-interest discipline at bid time

Per PPDA §27 + our internal register (`conflict-of-interest-register.md`):

- Every named project lead in the bid signs a fresh declaration **dated
  within 14 days of bid submission**.
- The Account Lead does an **independent search** against the
  procurement officer's name (PPDA registry, public LinkedIn) to
  identify any prior engagement.
- If any potential conflict surfaces, it is declared in the bid cover
  letter even where TechNexus would otherwise be entitled to remain
  silent. This protects against "discovery surprise" after award.

---

## 7. Standstill + objection period

Per PPDA Reg 73 + 96, after notification of award there is a **7-day
standstill period** during which an aggrieved bidder may lodge an
objection. If we are awarded:

- No contract negotiation in writing until the standstill expires.
- Verbal courtesy contact with MPS is acceptable.
- Bid security stays lodged until contract signature.

If we are **not** awarded and have grounds, the objection process is:

| Step | Window | Body |
|------|--------|------|
| Written objection to procuring entity | 7 days from non-award letter | MPS IPDC |
| Escalation to PPDA Review Panel | 7 days from procuring-entity response | PPDA |
| Judicial review | 30 days from PPDA Panel decision | High Court |

We document every step and treat them as ordinary legal proceedings.

---

## 8. Post-award documentary discipline

After contract signature, TechNexus retains for **10 years** at HQ
(`evidence-retention-policy.md` §2):

- The signed bid pack
- The award notification
- The contract + every annex + every variation
- Bid security receipts
- Performance security receipts and releases
- Every settlement file lodged under Treasury (Clause 14)
- Every SLA report
- Every conflict-of-interest declaration
- Every audit-log export covering the contract period

These are the documents the Auditor General will ask for.

---

## 9. Things that get a bid disqualified (in plain language)

- Bid submitted after the closing time, even by 1 minute.
- Bid security wrong amount, expired, or wrong issuer.
- Tax Clearance Certificate expired.
- Missing PPDA letter of good standing.
- An undeclared conflict of interest discovered later.
- Inconsistent figures across bid forms (the procuring entity will use
  the **least favourable** figure if not clarified).
- Misrepresentation of capacity (reference contacts that don't pick up,
  references that say they don't know us).
- Sub-contracting to an entity that itself failed to lodge eligibility.

Pre-bid review by Legal is therefore **mandatory** for every
NexusFine-related submission.

---

## 10. References

- Public Procurement and Disposal of Public Assets Act, 2017 (Cap. 37:03)
- Public Procurement and Disposal of Assets Regulations, 2020
- MSME Policy (Ministry of Trade, 2019, updated 2024)
- PPDA Standard Bidding Documents (Goods, Services, Consultancy)
- `docs/compliance/conflict-of-interest-register.md`
- `docs/compliance/whistleblower-mechanism.md`
- `docs/commercial/mwk-price-book.md` §7 (payment-milestone structure)
- `docs/commercial/treasury-reconciliation-clause.md` (Clause 14 — embedded in every MSA)
