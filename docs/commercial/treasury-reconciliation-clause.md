# Treasury Reconciliation — Standard Contract Clause

**Status:** v1.0 · **Issued:** 2026-05-16
**Owner:** TechNexus Commercial + Legal · **Embedded in every MSA from here on**
**Cross-reference:** `mwk-price-book.md` §7, `dpia-malawi.md` §3 (data flow to Treasury)

This file holds the standard paragraph block that goes into every Master
Services Agreement TechNexus signs for NexusFine. It governs how
NexusFine settles collected funds to the State, how disputes are handled,
and what either party owes the other when the reconciliation breaks.

The block below is **pre-approved**. Account Leads may use it without
further Legal review provided no edits are made. Edits require Legal
sign-off.

---

## Clause 14 — Treasury reconciliation

### 14.1 Settlement frequency
14.1.1   The Service Provider shall remit all funds collected on behalf
of the Authority to the Authority's designated Treasury account within
the next banking day following receipt of cleared funds, except where
otherwise required by these terms.

14.1.2   For the avoidance of doubt, "cleared funds" means funds that have
been confirmed by the underlying payment-aggregator, mobile-money
operator, bank or card processor as final and irrevocable.

14.1.3   Counter-cash payments collected at a Station are reconciled
**daily**, with funds banked by the close of the next banking day at the
designated commercial bank account held by the Authority.

### 14.2 Settlement file
14.2.1   On each banking day at 06:00 East Africa Time the Service
Provider shall produce a settlement file (the "Daily Settlement File")
in the format set out in Schedule 4 hereto, covering:

(a) every fine paid since the prior Daily Settlement File;
(b) the gross amount, channel, fee, and net amount of each payment;
(c) the cumulative settlement balance carried forward;
(d) any reversals or disputes recorded during the period;
(e) the Service Provider's electronic signature and the file's SHA-256
    digest, to be reproducible by the Authority on the same payload.

14.2.2   The Daily Settlement File shall be deposited into the
Authority's secure SFTP folder no later than 07:00 East Africa Time on
the banking day in question.

14.2.3   For the avoidance of doubt, the Service Provider shall not
release any net amount to the Authority prior to delivery of the
matching Daily Settlement File.

### 14.3 Reconciliation cadence
14.3.1   The Authority's Accountant-General's office shall reconcile the
Daily Settlement File against the bank credit within **3 banking days**
of receipt.

14.3.2   Any discrepancy shall be reported in writing to the Service
Provider within those 3 banking days. After 3 banking days have elapsed
without objection, the Daily Settlement File shall be deemed accepted.

14.3.3   A weekly summary file (the "Weekly Reconciliation Report")
shall be produced on the first banking day of each week summarising the
prior week's settlements, fees, reversals, and outstanding disputes.

### 14.4 Fees and deductions
14.4.1   Channel fees properly invoiced by Airtel Money, TNM Mpamba, the
participating banks, the card processor, or the USSD aggregator may be
deducted at source from the gross collected amount **only** where:

(a) the deduction is expressly itemised in the Daily Settlement File; and
(b) the underlying fee schedule has been agreed in writing in advance,
    is published on the Authority's transparency portal, and remains the
    most recently signed-off version.

14.4.2   No other deduction, fee, commission, or set-off shall be
permitted without the prior written approval of the Authority's
Accountant-General.

### 14.5 Disputes & reversals
14.5.1   Any dispute or reversal initiated by a citizen or by an
underlying payment provider after a Daily Settlement File has been
deposited shall be netted in the **next** Daily Settlement File,
clearly itemised, with a reference back to the original transaction.

14.5.2   Where a reversal exceeds the gross collected amount in the
next Daily Settlement File, the Service Provider shall refund the
shortfall to the Authority's account within 5 banking days.

14.5.3   The Service Provider shall maintain, at its own cost, a
floating reserve equivalent to the **rolling 7-day mean of net
collections** in a Malawian commercial bank, available exclusively to
cover such reversals.

### 14.6 Late-settlement remedies
14.6.1   If the Service Provider fails to deposit a Daily Settlement
File by 07:00 on the relevant banking day, the Authority shall be
entitled to liquidated damages of **0.05% of the gross amount due for
that day, per calendar day of delay**, up to a cap of 5% of that day's
gross amount.

14.6.2   If late delivery exceeds **5 banking days**, the Authority
shall additionally be entitled to suspend further citizen-channel
payments until the delay is cured, without liability to the Service
Provider.

14.6.3   Repeat late settlement (defined as three or more occurrences
in any rolling 30-day period) shall constitute a material breach and
permit the Authority to terminate this Agreement on **30 days' written
notice**, without further compensation to the Service Provider.

### 14.7 Audit & inspection
14.7.1   The Authority, the Auditor General, and any duly-appointed
representative shall have the right to:

(a) inspect the settlement and reconciliation records on demand,
    during the Service Provider's business hours, on **5 banking days'**
    notice;
(b) re-perform the cryptographic verification of any Daily Settlement
    File against the Service Provider's audit log;
(c) interview Service-Provider personnel involved in the settlement
    process.

14.7.2   The Service Provider shall preserve all reconciliation
records, settlement files, and underlying audit-log entries for
**seven (7) years** from the date of the underlying transaction.

14.7.3   Any cost of routine annual audit shall be borne by the
Authority; any cost of audit specifically arising from a discrepancy
or breach attributable to the Service Provider shall be borne by the
Service Provider.

### 14.8 Force majeure & exception handling
14.8.1   In the event of a force-majeure event (as defined in Clause 22)
the Service Provider's settlement obligations under this Clause are
suspended for the duration of the event and a reasonable resumption
period thereafter, not to exceed 5 banking days.

14.8.2   In the event of a payment-aggregator outage that prevents
clearance, the Service Provider shall:

(a) settle on the basis of provisional confirmation if the aggregator's
    own contractual SLA permits; and
(b) deliver an out-of-cycle reconciliation when the aggregator's
    clearance is received.

### 14.9 Schedule 4 — Daily Settlement File format
14.9.1   The Daily Settlement File shall be a UTF-8 CSV file with a
fixed header in the form set out in `docs/commercial/settlement-file-format.csv`
(to be agreed at PO signature), digitally signed using the
Service Provider's published public key.

14.9.2   Any change to the file format requires the prior written
agreement of the Authority's Accountant-General and shall take effect
no earlier than 30 days after publication.

---

## Notes for the Account Lead

- This clause does **not** require the Treasury IFMIS API to be live.
  It assumes manual SFTP daily files as the floor. IFMIS-live is a
  scope addition at Section E.
- The 0.05%/day penalty is calibrated to the build margin so it does
  not bankrupt a single missed file but does discipline us if we are
  systematically late.
- The 7-day floating reserve at TechNexus's cost is non-negotiable;
  it is what makes the State comfortable with us holding funds
  overnight. Do not offer to drop it.
- The 5-banking-day audit notice is the customary minimum in Malawian
  government MSAs; do not shorten without Legal sign-off.

---

## References

- Public Finance Management Act, 2003 (Malawi) — Treasury authority
- Money Laundering, Proceeds of Serious Crime and Terrorist Financing Act, 2006
- `docs/commercial/mwk-price-book.md` — §7 payment-milestone schedule
- `docs/security/append-only-audit-log.md` — settlement-file digest verification
- `docs/operations/sla-tiers.md` — incident severities apply to settlement failures
