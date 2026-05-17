# Conflict-of-Interest Register

**Status:** v1.0 template · **Issued:** 2026-05-16
**Owner:** TechNexus Compliance function
**Cadence:** updated within 5 business days of any qualifying event; full re-declaration each quarter.

This is the operational template. The signed quarterly declarations are
filed under `docs/compliance/declarations/{quarter}/{name}.pdf` and are
**not** in this repository — they live in TechNexus's HR vault.

---

## 1. Why we keep this register

NexusFine sells into Malawian government. The minute we start invoicing
the State, every senior person at TechNexus and every consultant on the
project becomes potentially conflicted by:

- ownership of, or directorship in, a competing vendor
- a family member in a position to influence the procurement
- a paid advisory role to an MPS / DRTSS official
- prior consulting work for an MPS, MDF, ACB or AG office
- any ongoing case (civil or criminal) involving the State
- any beneficial interest in a payment-aggregator, telco, bank, or
  hardware supplier named in our supply chain
- gifts, hospitality, or honoraria received from any of the above above a
  cumulative annual value of MWK 50,000 per source

This register exists to declare those interests proactively, to assess
each for materiality, and to record the mitigation. **Failure to declare
is grounds for dismissal AND constitutes an offence under §32 of the
Corrupt Practices Act, 1995 (Malawi).**

---

## 2. Who must declare

| Role | Frequency | Authority to approve mitigation |
|------|-----------|--------------------------------|
| Executive directors (CEO, CTO, CFO) | Quarterly + ad-hoc | Board (recused) |
| Senior managers (Account Lead, Head of Delivery) | Quarterly + ad-hoc | CEO |
| Engineers / Designers on the NexusFine project | Quarterly | CTO |
| Sub-contractors / consultants | At engagement + quarterly | CEO |
| Board members and Advisors | Annually + ad-hoc | Board (recused) |

---

## 3. Declaration template

Each person submits a one-page declaration containing:

```
TECHNEXUS MW — CONFLICT-OF-INTEREST DECLARATION

Name:                _____________________________________________
Role:                _____________________________________________
Period:              Q__ 2026  (Jan–Mar / Apr–Jun / Jul–Sep / Oct–Dec)

Direct interests
────────────────
☐ I have no direct or indirect financial interest in any competing
  traffic-fines / e-government vendor, payment aggregator, telco, or
  hardware supplier listed in our active supply chain.

If you cannot tick the box above, describe in full:
_________________________________________________________________
_________________________________________________________________

Family / household interests
────────────────────────────
☐ No member of my immediate family (spouse, child, sibling, parent) is
  employed by, or holds a beneficial interest in, any of the above; nor
  is any member of my household in a procurement, approval, or oversight
  role at MPS, DRTSS, Treasury, MACRA, or the Office of the Auditor
  General.

If you cannot tick the box above, describe in full:
_________________________________________________________________
_________________________________________________________________

Outside engagements
───────────────────
☐ I hold no paid consulting, board, or advisory role with any of the
  parties named above in the current quarter.

If you cannot tick the box above, describe in full:
_________________________________________________________________
_________________________________________________________________

Gifts and hospitality (this quarter)
────────────────────────────────────
List every gift, meal, or hospitality with cumulative value above
MWK 10,000 from any party connected to NexusFine, regardless of who
hosted:

Date          | Source                  | Nature        | MWK value
─────────────────────────────────────────────────────────────────────
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________

Legal proceedings
─────────────────
☐ I am not party to any civil or criminal case involving the State, MPS,
  DRTSS, Treasury, or any TechNexus customer.

If you cannot tick the box above, describe in full:
_________________________________________________________________

Declaration
───────────
I confirm the statements above are true and complete to the best of my
knowledge. I will notify the Compliance Officer within 5 business days
of any change.

Signed:  _______________________________   Date:  ____________________
Witness: _______________________________   Date:  ____________________
```

---

## 4. Materiality test (used by the approver)

A declared interest is **material** if a reasonable observer would
question the integrity of TechNexus's decision-making on NexusFine.
The approver classifies each declaration as:

| Class | Definition | Action |
|-------|-----------|--------|
| **Cleared** | No actual or perceived conflict | File the declaration. |
| **Recuse** | Interest exists but can be managed by recusing the declarant from specific decisions | Document recusal scope; require sign-off by an alternate decision-maker. |
| **Restructure** | The interest is sufficient that recusal alone is not enough; the engagement must be restructured | Examples: divest the interest; remove the declarant from the project; decline the engagement entirely. |
| **Decline** | Cannot be mitigated; project terminated for that person | Escalate to Board. |

A decision is recorded against every declaration. The Compliance Officer
maintains the master register and reports to the Board quarterly.

---

## 5. Register summary table (kept current)

| Quarter | Declarations submitted | Cleared | Recuse | Restructure | Decline |
|--------:|-----------------------:|--------:|-------:|------------:|--------:|
| 2026-Q2 | (TBD) | | | | |
| 2026-Q3 | | | | | |

---

## 6. Specific watch-list for NexusFine

The following relationships require **standing declarations** every quarter
without exception:

- Direct relatives of any MPS / DRTSS officer above the rank of Inspector
- Direct relatives of any Permanent Secretary or Deputy in MHS, MoT, MoF, or Justice
- Anyone with a beneficial interest in Airtel Malawi, TNM, Standard Bank,
  NBM, FDH Bank, or NBS Bank (named in the active supply chain)
- Anyone with a beneficial interest in Intel, Samsung, APC, or any other
  named hardware supplier
- Anyone who has previously sat on a PPDA tender-evaluation committee
  that adjudicated a TechNexus bid

---

## 7. Sanctions for non-declaration

In ascending order:
1. Verbal warning + retroactive declaration.
2. Written warning + removal from the project for the quarter.
3. Termination of engagement.
4. Referral to the Corrupt Practices Act, 1995 §32 (failure-to-disclose offence).

Sanctions 3 and 4 require Board approval.

---

## 8. References

- Corrupt Practices Act, 1995 (Malawi), §32 (failure to declare)
- Public Procurement and Disposal of Public Assets Act, 2017, §27 (conflicts in procurement)
- ISO 37001:2016 (Anti-bribery management systems) §5.2
- `docs/compliance/dpia-malawi.md` — companion DPA-compliance artefact
- `docs/department-recommendations.md` C7
