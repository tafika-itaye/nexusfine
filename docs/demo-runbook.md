# NexusFine — Minister Demo Runbook

**Audience:** Minister of Homeland Security · IG Police · Director DRTSS · TechNexus team
**Duration:** 15 min demo + 10 min Q&A
**Operator:** TJ (TechNexus)
**Last rehearsed:** _____ / _____ / 2026

---

## 0 · Pre-flight (T-30 min)

Run on the demo laptop, in this order. Stop if any check fails — fall back to Plan B (recorded video, see §6).

```bash
# 1. Pull latest
cd ~/Documents/GitHub/nexusfine
git status                                # must be clean
git log --oneline -5                      # confirm v0.3 / polish tags present

# 2. Database health
sqllocaldb info MSSQLLocalDB               # State: Running
# if not: sqllocaldb start MSSQLLocalDB

# 3. Boot the API
dotnet run --project src/NexusFine.API/NexusFine.API.csproj --launch-profile http &
sleep 8
curl -s http://localhost:5121/api/health   # expect: {"status":"ok",...}

# 4. Boot the Admin portal
dotnet run --project src/NexusFine.Admin/NexusFine.Admin.csproj --launch-profile http &
sleep 6
curl -s -o /dev/null -w "%{http_code}\n" http://localhost:5296/login   # expect 200

# 5. Open browsers (3 windows, side-by-side)
#    Window A — Citizen portal:   http://localhost:5121/citizen/
#    Window B — Admin portal:     http://localhost:5296/login
#    Window C — Audit log tab:    (will navigate after login)
```

**Visual checks before walking on stage:**

- [ ] Citizen hero photo loaded (officer at checkpoint, navy gradient, gold band)
- [ ] Authority strip visible (MPS · DRTSS · Coat of Arms)
- [ ] Channel logos (Airtel, TNM, banks, Visa/MC, USSD, WhatsApp) all rendering — none broken
- [ ] Admin login: MPS crest, gold ring, map watermark backdrop
- [ ] Phone hotspot ON (in case venue Wi-Fi dies)
- [ ] Laptop on AC, screen brightness max, notifications muted
- [ ] Backup video on USB stick, in pocket

---

## 1 · Opening (90 sec)

> "Honourable Minister — today our roads collect about MK 800 million in fines. About MK 280 million of that never reaches Treasury. NexusFine is the digital traffic-fines spine that closes that gap. What you'll see in the next 15 minutes is the system, live, end-to-end — citizen, officer, supervisor, auditor."

**Show:** Citizen portal home page (Window A). Let the hero land. Don't speak over it.

---

## 2 · Citizen lookup (2 min)

**Story beat:** "A driver has just been pulled over in Lilongwe. Officer hands him a ticket. He scans the QR code — it lands here."

1. Click **EN / NY** toggle once → switch to Chichewa, then back to English. ("Bilingual from day one — same database.")
2. In the lookup box, paste reference number: **`NF-2026-000142`**
3. Show the fine detail: plate, offence, amount, due date, status
4. Pause on the **payment options grid** — point at the real channel logos:
   - "Airtel Money, TNM Mpamba — 78% of Malawians have a mobile-money wallet"
   - "Standard Bank, NBM, FDH, NBS — bank transfer with auto-reference"
   - "Visa / Mastercard — for business fleets"
   - "USSD `*247#` — for feature phones, no internet needed"
   - "WhatsApp — pay through chat, with receipt back as PDF"

**Do not click pay yet.** Set up the supervisor view first.

---

## 3 · Supervisor / admin view (3 min)

**Switch to Window B.** Login as `admin` / `Nexus@Admin2026`.

> "This is what the duty supervisor at MPS HQ sees right now."

1. **Dashboard** — read out the four KPIs left-to-right:
   - Fines Issued Today
   - Revenue Month-to-date (in MK)
   - Collection Rate (%)
   - Officers On Duty / Total
2. Scroll to **Live Fine Feed** — "Every issuance lands here within seconds."
3. Scroll to **Officer Performance — Today** — "Ranked by fines issued, with collection rate. Each badge has a face — that's deterministic, anonymised for now, real headshots post-pilot."

**Switch to `/officers`** in the sidebar.

4. Click the **Lilongwe** zone filter. "Drill down by zone — I can see who's deployed, where they last synced, what device they're on."

---

## 4 · Live payment (the big moment, 4 min)

**Switch back to Window A** — citizen portal still on the fine detail page.

1. Click **Airtel Money**.
2. Type a phone number (any 09… mobile). Click **Pay**.
3. The simulated gateway processes — show the spinner, then the success screen.
4. Click **Download Receipt (PDF)** — let the PDF open in a new tab. Highlight: reference, amount, channel, timestamp, MPS crest.

**Switch to Window B (Admin) — refresh the dashboard.**

5. The KPI **Revenue MTD** has bumped up. Pull-quote that number.
6. The **Live Fine Feed** now shows the fine status as `Paid` (green pill).

> "From driver's phone to Treasury's ledger — under 30 seconds. No paper. No middleman."

---

## 5 · Audit & accountability (3 min)

**Switch to Window B → click `Audit Log` in the sidebar.**

> "Every action — login, fine issued, payment posted, status change — is recorded with who, when, what changed. This is for the IG, the Auditor General, and the Anti-Corruption Bureau."

1. Filter by Action = **`PAYMENT_POSTED`** → show the entry from §4.
2. Click into the row → show the OldValues / NewValues JSON pretty-print.
3. Filter by Action = **`LOGIN_SUCCESS`** → show your own login from earlier.
4. Filter by EntityType = **`Officer`** → "Every officer roster change is here too."

**Closing line:**

> "Every shilling that flows through NexusFine has a fingerprint. Nothing moves in the dark."

---

## 6 · Plan B — if something breaks

| What broke | Recovery |
|------------|----------|
| API won't start | Show pre-recorded video (USB stick, file `nexusfine-demo-v1.mp4`). Continue narration over the video. |
| Admin won't load | Demo from API Swagger UI at `http://localhost:5121/swagger` — show the `POST /api/fines` and `POST /api/payments` flow with curl-like requests. |
| Payment gateway times out | The gateway is simulated — restart with `Ctrl-C` then `dotnet run` on the API project. While it boots, talk over the offline-first design (officers can issue fines without network; sync when back online). |
| Wi-Fi dies | Phone hotspot. All three apps run on `localhost` so the demo works fully offline anyway. |
| Total laptop failure | Co-presenter laptop is mirroring this exact environment. Hand mic to them. |

---

## 7 · Q&A prep — likely questions

**Q: Why not just buy off-the-shelf?**
> NexusFine is built around three Malawi-specific facts: bilingual EN/Chichewa from day one, mobile-money first (not card-first), and offline-tolerant for rural checkpoints. Off-the-shelf systems are built for the opposite assumptions.

**Q: How does it integrate with Treasury?**
> Module 6 (deploy) wires into the Treasury IFMIS API for daily settlement. Today's demo uses a simulated gateway end-point that mirrors the IFMIS contract spec.

**Q: What about data sovereignty?**
> All data is hosted on Malawi-resident infrastructure (MERA-licensed datacentre, Lilongwe). No data leaves the country. We can show the deployment diagram if helpful.

**Q: When can it go live?**
> Pilot in 90 days (one zone — Lilongwe Central). National rollout in 12 months, phased zone-by-zone. The architecture you saw today is production-grade — we are not building a prototype.

**Q: Cost?**
> Refer to the signed quotation, MWK 218,295,000 grand total. Ministry already has a copy. Happy to walk through line items in a separate session.

**Q: Security review?**
> ISO 27001 controls baked in. Penetration test scheduled for week 8 of pilot. JWT-based auth, role-based access, full audit trail. Open to MPS Cybersecurity Unit reviewing the code at any point.

---

## 8 · Post-demo

```bash
# Clean shutdown — don't leave processes running on the demo laptop
pkill -f "dotnet run"
```

- [ ] Send thank-you note to Permanent Secretary by 18:00 same day
- [ ] Push any demo-day fixes to a `demo-day-fixes` branch (don't commit straight to main)
- [ ] Capture lessons-learned in `docs/demo-retro.md` while it's fresh

---

**Confidence checklist for the operator:**

- [ ] Rehearsed the full 15 min at least 3 times end-to-end
- [ ] Memorised the four pull-quotes (KPIs, "30 seconds", "fingerprint", "data sovereignty")
- [ ] Watch on the table — silent timer, glance at minute 8 and minute 14
- [ ] Water bottle within reach
- [ ] Phone face-down, vibrate off
