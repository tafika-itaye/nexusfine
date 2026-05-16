# MWK Price Book — NexusFine

**Status:** v1.0 · **Issued:** 2026-05-16
**Owner:** TechNexus Commercial function
**Review cadence:** quarterly (re-issued with FX adjustment if MWK/USD moves >5%)
**Currency:** Malawian Kwacha (MWK) unless otherwise noted

This is the **internal pricing reference**. Every quotation TechNexus issues
must be derivable from this book + signed-off variances. Where the book and
a quotation differ, the quotation governs (until next book issue).

---

## 1. Foreign-exchange reference

**Indicative rate:** MWK 1,750 / USD
**Source:** Reserve Bank of Malawi mid-rate, taken on 2026-05-15.
**Trigger to re-issue this book:** when the RBM mid-rate moves by more than
5% from the reference rate. As at issue date 2026-05-16 the band is:

| Lower | Reference | Upper |
|------:|----------:|------:|
| MWK 1,663 | MWK 1,750 | MWK 1,838 |

Quotations issued at the indicative rate carry a clause permitting
re-quote (or hold) when the prevailing rate exits the band.

---

## 2. Software licences & build (Section A)

| Code | Item | Unit | Price (MWK) | Notes |
|------|------|-----|------------:|-------|
| A1 | EBPP core platform (one-time build) | one-off | 33,250,000 | per pilot tenant |
| A2 | Officer MAUI app build | one-off | 21,000,000 | |
| A3 | Admin + Supervisor dashboard | one-off | 17,500,000 | includes CRUD across Department/Station/PatrolPost/Officer/OffenceCode |
| A4 | Citizen web portal | one-off | 12,250,000 | bilingual EN/NY |
| A5 | USSD service integration | one-off | 10,500,000 | shared state machine with WhatsApp |
| A6 | WhatsApp Business API integration | one-off | 7,000,000 | |
| A7 | NRB API integration | one-off | 7,000,000 | National Registration Bureau |
| A8 | Mobile-money integration (Airtel + TNM) | one-off | 8,750,000 | per country pair; add 4,375,000 per extra provider |
| A9 | Bank & card gateway integration | one-off | 7,000,000 | |
| A10 | QA, testing & security audit | one-off | 10,500,000 | |
| A11 | Deployment & go-live support | one-off | 5,250,000 | 2 weeks hyper-care included |
| A12 | Technical documentation | one-off | 6,125,000 | |
| A13 | Project management | retained | 11,375,000 | for full pilot lifecycle |
| **Section A subtotal** | | | **157,500,000** | |

**Variance authority:** Section A line items may flex by ±5% without
prior approval. Above ±5% requires CTO sign-off.

---

## 3. Hardware supply (Section B)

| Code | Item | Unit | Price (MWK) | Notes |
|------|------|-----|------------:|-------|
| B1 | NFC-enabled Android tablet (Samsung Galaxy Tab A9) | each | 315,000 | bulk-of-50 unit price |
| B2 | Rugged case + screen protector | each | 43,750 | |
| B3 | RFID/NFC officer ID tag | each | 8,750 | |
| B4 | Mobile NFC card reader | each | 262,500 | |
| B5 | Supervisor admin laptop | each | 1,312,500 | spec: i5 / 16 GB / 512 GB SSD / 14" |
| B6 | Network router for command centre | each | 490,000 | |
| B7 | UPS unit for HQ server | each | 612,500 | APC Smart-UPS 1500VA |

**Variance authority:** Section B is FX-sensitive. Quotations specify the
reference rate; final invoice may be re-rated against actual import rate.

---

## 4. Infrastructure & annual services (Section C)

| Code | Item | Basis | Price (MWK) | Notes |
|------|------|-------|------------:|-------|
| C1 | Microsoft Azure hosting | per 12 months | 10,500,000 | Malawi-resident region preferred |
| C2 | Domain, SSL, DNS management | per 12 months | 350,000 | per country tenant |
| C3 | USSD aggregator setup fee | one-off | 875,000 | per country |
| C4 | WhatsApp conversation bundle (200,000 sessions) | per 12 months | 2,625,000 | rolls to year-2 at same price unless WhatsApp price-list changes |
| C5 | WhatsApp Business API annual licence | per 12 months | 2,625,000 | |
| C6 | Backup, DR & monitoring | per 12 months | 2,100,000 | |

**Year-2 onward:** Section C renews automatically at the prior year's price
**+ MERA-published inflation rate, capped at 10% per annum**.

---

## 5. Training & change management (Section D)

| Code | Item | Unit | Price (MWK) |
|------|------|-----|------------:|
| D1 | Officer training — 2 days on-site | per cohort of 50 | 3,500,000 |
| D2 | Supervisor & admin training — 1 day | per cohort of 10 | 1,750,000 |
| D3 | Train-the-trainer programme | one-off | 2,625,000 |
| D4 | Printed quick-reference guides (EN + NY) | per 100 booklets | 875,000 |

---

## 6. Optional add-ons (Section E)

| Code | Item | Unit | Price (MWK) | Notes |
|------|------|-----|------------:|-------|
| E1 | Station Server build | per station | 2,430,000 | NUC + 1500VA UPS + 4G failover + backup SSD |
| E2 | Off-grid solar pack | per station | 1,800,000 | 200W panel + 100Ah battery + controller |
| E3 | RAG-FAQ WhatsApp chatbot module | one-off | 8,750,000 | per D-007 of 24 Apr 2026 |

---

## 7. Standard payment terms

- 30% — on signature of Purchase Order
- 30% — on demonstration & user-acceptance testing
- 30% — on go-live
- 10% — on completion of 90-day pilot hyper-care

Variance authority: payment-schedule changes require Commercial Director sign-off.

---

## 8. Discount authority matrix

| Discount % off section subtotal | Required approval |
|-------------------------------:|--------------------|
| 0 – 2.5% | Account Lead |
| 2.5 – 7.5% | Commercial Director |
| 7.5 – 15% | CEO |
| > 15% | Board-of-directors resolution |

Total deal-level discount caps at 15% irrespective of section.

---

## 9. Re-quote scenarios

The following events require a re-quote within 5 business days of detection:

1. RBM mid-rate exits the band defined in §1.
2. Section A scope additions (e.g., new gateway, new ministry adoption).
3. Hardware cost moves more than ±10% at supplier level.
4. WhatsApp / USSD partner price-list changes published.
5. Tax-treatment change applicable to the deal (VAT band, withholding tax).

---

## 10. Alternative pricing model — revenue share (preview)

For ministries wishing to defer capex, TechNexus offers an opt-in
revenue-share model:

- **Years 1–3:** 2.0% of collected fine revenue paid monthly to TechNexus.
- **Year 4 onward:** rate drops to 0.75% (maintenance + uplift only).
- **Caps:** monthly minimum of MWK 7,500,000 (covers operating cost);
  monthly maximum of MWK 35,000,000 (caps Treasury exposure).
- **Hardware** still procured up-front under Section B; financed through
  a 24-month interest-bearing instalment if requested.

The revenue-share model is priced to be revenue-neutral to TechNexus over
five years compared with the conventional one-off + annual model.
Full model in `docs/commercial/revenue-share-model.md` *(to ship)*.
