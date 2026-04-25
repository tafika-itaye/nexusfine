/* NexusFine — Citizen Portal i18n
 * EN: English (working language)
 * NY: Chichewa (Chinyanja) — national language of Malawi
 *
 * Every UI string the citizen sees lives here.
 * To translate a new node, mark the HTML element with data-i18n="key"
 * (or data-i18n-placeholder="key" for input placeholders).
 */
window.NF_I18N = {
  en: {
    // ── top bar / nav ─────────────────────────────────────────
    "gov.republic":      "REPUBLIC OF MALAWI",
    "gov.mps":           "Malawi Police Service — Road Safety & Traffic Department",
    "gov.portal":        "Official Portal",
    "gov.site":          "police.gov.mw",
    "gov.emergency":     "Emergency: 997",
    "notice.title":      "System Notice:",
    "notice.body":       "NexusFine is the only official portal for paying traffic fines. Beware of fraudulent sites. Always verify the URL.",
    "nav.lookup":        "Fine Lookup",
    "nav.how":           "How It Works",
    "nav.channels":      "Pay Options",
    "nav.faq":           "Help / FAQ",

    // ── hero ──────────────────────────────────────────────────
    "hero.eyebrow":      "Official MPS Digital Service",
    "hero.h1.line1":     "Traffic Fines.",
    "hero.h1.line2":     "Paid Online.",
    "hero.h1.line3":     "No Queues.",
    "hero.desc":         "Look up any traffic fine by plate number, reference, or driver ID. Pay instantly via mobile money, bank, or card — available 24 hours a day.",
    "hero.stat1.num":    "24/7",
    "hero.stat1.lbl":    "Payment Access",
    "hero.stat2.num":    "6",
    "hero.stat2.lbl":    "Pay Channels",
    "hero.stat3.num":    "EN / NY",
    "hero.stat3.lbl":    "Bilingual Service",
    "hero.stat4.num":    "5 Min",
    "hero.stat4.lbl":    "Avg. Pay Time",

    // ── lookup card ───────────────────────────────────────────
    "lookup.title":      "🔍 Find Your Fine",
    "lookup.sub":        "Enter your vehicle plate, fine reference, or driver ID",
    "lookup.tab.plate":  "By Plate",
    "lookup.tab.ref":    "By Reference",
    "lookup.tab.id":     "By Driver ID",
    "lookup.lbl.plate":  "Vehicle Registration Plate",
    "lookup.ph.plate":   "e.g. MWK 1234 A",
    "lookup.lbl.ref":    "Fine Reference Number",
    "lookup.ph.ref":     "e.g. NXF-2026-00001",
    "lookup.lbl.id":     "National ID / Driver's Licence No.",
    "lookup.ph.id":      "e.g. 9911-22-330044-55",
    "lookup.btn":        "Search Fines",
    "lookup.searching":  "Searching…",
    "lookup.error.empty":"Please enter a search value.",
    "lookup.error.none": "No fine found matching the provided details.",
    "lookup.error.net":  "Could not reach NexusFine. Please check your connection.",

    // ── result panel ──────────────────────────────────────────
    "result.driver":     "Driver",
    "result.id":         "National ID",
    "result.licence":    "Licence No.",
    "result.offence":    "Offence",
    "result.location":   "Location",
    "result.issued":     "Issued",
    "result.due":        "Due Date",
    "result.officer":    "Issued By",
    "result.amount.due": "Amount Due",
    "result.amount.paid":"Amount Paid",
    "result.due.note":   "Pay before due date to avoid 10% penalty",
    "result.overdue":    "OVERDUE — penalty applied",
    "result.timeline.issued":   "Issued",
    "result.timeline.notified": "Notified",
    "result.timeline.payment":  "Payment",
    "result.timeline.closed":   "Closed",
    "result.pay.heading": "Choose payment method",
    "result.pay.airtel":  "Airtel Money",
    "result.pay.mpamba":  "TNM Mpamba",
    "result.pay.bank":    "Bank Transfer",
    "result.pay.card":    "Debit / Credit Card",
    "result.pay.ussd":    "USSD *777#",
    "result.pay.whatsapp":"WhatsApp Pay",
    "result.appeal":      "Dispute this fine",
    "result.download":    "Download Receipt (PDF)",
    "result.print":       "Print Receipt",

    // ── how it works ──────────────────────────────────────────
    "how.eyebrow":       "Simple Process",
    "how.heading":       "How NexusFine Works",
    "how.s1.title":      "Look Up",
    "how.s1.body":       "Enter your plate, fine reference, or driver ID to find any active fine in seconds.",
    "how.s2.title":      "Review",
    "how.s2.body":       "Confirm the offence, amount, and due date. See the issuing officer and full evidence.",
    "how.s3.title":      "Pay",
    "how.s3.body":       "Choose mobile money, bank transfer, card, or USSD. Payment confirms instantly.",
    "how.s4.title":      "Receipt",
    "how.s4.body":       "Get your receipt as a PDF. Your fine status updates immediately on government records.",

    // ── channels ──────────────────────────────────────────────
    "ch.eyebrow":        "Pay Anywhere",
    "ch.heading":        "Available Payment Channels",
    "ch.airtel.title":   "Airtel Money",
    "ch.airtel.body":    "Pay via Airtel Money STK push. Confirm with your Airtel PIN.",
    "ch.airtel.tag":     "INSTANT",
    "ch.mpamba.title":   "TNM Mpamba",
    "ch.mpamba.body":    "Pay directly from your TNM Mpamba wallet. Confirm with your Mpamba PIN.",
    "ch.mpamba.tag":     "INSTANT",
    "ch.bank.title":     "Bank Transfer",
    "ch.bank.body":      "Transfer to the MPS Treasury account at any commercial bank. Reference: your fine number.",
    "ch.bank.tag":       "1–2 BUSINESS DAYS",
    "ch.card.title":     "Debit / Credit Card",
    "ch.card.body":      "Pay with Visa or Mastercard. 3D Secure verified.",
    "ch.card.tag":       "INSTANT",
    "ch.ussd.title":     "USSD *777#",
    "ch.ussd.body":      "No internet? Dial *777# from any phone, choose 'Pay Fine', enter your reference.",
    "ch.ussd.tag":       "NO INTERNET",
    "ch.whatsapp.title": "WhatsApp",
    "ch.whatsapp.body":  "Message +265 999 NEXUS on WhatsApp to look up and pay any fine.",
    "ch.whatsapp.tag":   "CHAT-BASED",

    // ── FAQ ───────────────────────────────────────────────────
    "faq.eyebrow":       "Common Questions",
    "faq.heading":       "Frequently Asked",
    "faq.q1":            "Is NexusFine the only legitimate fines portal?",
    "faq.a1":            "Yes. NexusFine is the only digital portal authorised by the Malawi Police Service to collect traffic fines. Always verify the URL is nexusfine.mw or police.gov.mw before entering any payment details.",
    "faq.q2":            "What if I disagree with my fine?",
    "faq.a2":            "Click 'Dispute this fine' on the fine details panel. You can submit your reason and supporting evidence. A senior officer will review your appeal within 5–10 working days.",
    "faq.q3":            "Will I get a paper receipt?",
    "faq.a3":            "Your digital PDF receipt is the official record and is legally valid. Keep it in your phone — you can show it at any roadblock or download it again here.",
    "faq.q4":            "What happens if I don't pay?",
    "faq.a4":            "After 30 days, a 10% penalty is added. After 60 days, the fine is referred to court and your driver's licence may be suspended.",

    // ── footer ────────────────────────────────────────────────
    "footer.about":      "NexusFine is the official digital fines management platform of the Malawi Police Service Road Safety & Traffic Department, built and operated by TechNexus MW.",
    "footer.col.service":"Service",
    "footer.col.help":   "Help",
    "footer.copyright":  "© 2026 Malawi Police Service. All rights reserved.",
    "footer.poweredby":  "Powered by TechNexus MW",

    // ── pay modal ─────────────────────────────────────────────
    "pay.title":         "Confirm Payment",
    "pay.sub.airtel":    "Enter the Airtel Money number to charge.",
    "pay.sub.mpamba":    "Enter the TNM Mpamba number to charge.",
    "pay.sub.bank":      "We will generate a payment reference. Use it on your bank transfer.",
    "pay.sub.card":      "You will be redirected to the secure card processor.",
    "pay.sub.ussd":      "Dial *777# and select 'Pay Fine'.",
    "pay.sub.whatsapp":  "Open WhatsApp and message +265 999 NEXUS.",
    "pay.amount.lbl":    "AMOUNT TO PAY",
    "pay.phone.lbl":     "MOBILE MONEY NUMBER",
    "pay.phone.ph":      "991234567",
    "pay.btn.confirm":   "Confirm Payment",
    "pay.btn.processing":"Processing…",
    "pay.success.title": "Payment Successful",
    "pay.success.body":  "Your fine has been settled. Receipt no.",
    "pay.success.btn":   "Download Receipt (PDF)",
    "pay.error":         "Payment could not be completed. Please try again."
  },

  ny: {
    // ── top bar / nav ─────────────────────────────────────────
    "gov.republic":      "DZIKO LA MALAŴI",
    "gov.mps":           "Apolisi a Malaŵi — Dipatimenti ya Misewu",
    "gov.portal":        "Tsamba Lovomerezeka",
    "gov.site":          "police.gov.mw",
    "gov.emergency":     "Munthawi Yoopsa: 997",
    "notice.title":      "Chenjezo:",
    "notice.body":       "NexusFine ndi tsamba lokhalo lovomerezeka kulipira chindapusa cha pamsewu. Penyetsetsani URL musanalipire.",
    "nav.lookup":        "Pezani Chindapusa",
    "nav.how":           "Mmene Zikuyendera",
    "nav.channels":      "Njira Zolipirira",
    "nav.faq":           "Thandizo / Mafunso",

    // ── hero ──────────────────────────────────────────────────
    "hero.eyebrow":      "Ntchito Yovomerezeka ya Apolisi",
    "hero.h1.line1":     "Chindapusa cha Pamsewu.",
    "hero.h1.line2":     "Lipirani pa Intaneti.",
    "hero.h1.line3":     "Popanda Mizere.",
    "hero.desc":         "Pezani chindapusa chilichonse pogwiritsa ntchito nambala ya galimoto, nambala yochindapusa, kapena chizindikiritso cha dalaivala. Lipirani msanga ndi mobile money, banki, kapena khadi — masana ndi usiku.",
    "hero.stat1.num":    "24/7",
    "hero.stat1.lbl":    "Lipirani Nthawi Iliyonse",
    "hero.stat2.num":    "6",
    "hero.stat2.lbl":    "Njira Zolipirira",
    "hero.stat3.num":    "EN / NY",
    "hero.stat3.lbl":    "Zilankhulo Ziwiri",
    "hero.stat4.num":    "Mphindi 5",
    "hero.stat4.lbl":    "Nthawi Yolipirira",

    // ── lookup card ───────────────────────────────────────────
    "lookup.title":      "🔍 Pezani Chindapusa Chanu",
    "lookup.sub":        "Lembani nambala ya galimoto, nambala yochindapusa, kapena chizindikiritso cha dalaivala",
    "lookup.tab.plate":  "Ndi Nambala",
    "lookup.tab.ref":    "Ndi Refa",
    "lookup.tab.id":     "Ndi Chizindikiritso",
    "lookup.lbl.plate":  "Nambala ya Galimoto",
    "lookup.ph.plate":   "mwachitsanzo MWK 1234 A",
    "lookup.lbl.ref":    "Nambala ya Chindapusa",
    "lookup.ph.ref":     "mwachitsanzo NXF-2026-00001",
    "lookup.lbl.id":     "ID ya Dziko / Layisensi",
    "lookup.ph.id":      "mwachitsanzo 9911-22-330044-55",
    "lookup.btn":        "Sakani",
    "lookup.searching":  "Tikufufuza…",
    "lookup.error.empty":"Lembani china chake kuti tifufuze.",
    "lookup.error.none": "Palibe chindapusa chomwe tachipeza.",
    "lookup.error.net":  "Sitinakwanitse kufikira NexusFine. Yang'anani intaneti yanu.",

    // ── result panel ──────────────────────────────────────────
    "result.driver":     "Dalaivala",
    "result.id":         "ID ya Dziko",
    "result.licence":    "Layisensi",
    "result.offence":    "Cholakwika",
    "result.location":   "Pamene Chinachitikira",
    "result.issued":     "Tsiku Lopereka",
    "result.due":        "Tsiku Lomalizira",
    "result.officer":    "Wopereka Chindapusa",
    "result.amount.due": "Ndalama Yoyenera Kulipira",
    "result.amount.paid":"Ndalama Yolipidwa",
    "result.due.note":   "Lipirani musanafike tsiku lomalizira kuti musawonjezere 10%",
    "result.overdue":    "TSIKU LADUTSA — Chindapusa chawonjezeka",
    "result.timeline.issued":   "Lapatsidwa",
    "result.timeline.notified": "Lauzidwa",
    "result.timeline.payment":  "Kulipira",
    "result.timeline.closed":   "Latha",
    "result.pay.heading": "Sankhani njira yolipirira",
    "result.pay.airtel":  "Airtel Money",
    "result.pay.mpamba":  "TNM Mpamba",
    "result.pay.bank":    "Banki",
    "result.pay.card":    "Khadi (Visa / Mastercard)",
    "result.pay.ussd":    "USSD *777#",
    "result.pay.whatsapp":"WhatsApp",
    "result.appeal":      "Tsutsani chindapusachi",
    "result.download":    "Tsitsani Risiti (PDF)",
    "result.print":       "Sindikizani Risiti",

    // ── how it works ──────────────────────────────────────────
    "how.eyebrow":       "Njira Yosavuta",
    "how.heading":       "Mmene NexusFine Ikuyendera",
    "how.s1.title":      "Fufuzani",
    "how.s1.body":       "Lembani nambala ya galimoto, nambala ya chindapusa, kapena ID ya dalaivala kuti mupeze chindapusa msanga.",
    "how.s2.title":      "Onani",
    "how.s2.body":       "Tsimikizirani cholakwika, ndalama, ndi tsiku lomalizira. Onani wapolisi amene anapereka.",
    "how.s3.title":      "Lipirani",
    "how.s3.body":       "Sankhani mobile money, banki, khadi, kapena USSD. Kulipira kumathandizidwa msanga.",
    "how.s4.title":      "Risiti",
    "how.s4.body":       "Landilani risiti yanu ngati PDF. Chindapusa chimasinthidwa msanga m'mabuku a boma.",

    // ── channels ──────────────────────────────────────────────
    "ch.eyebrow":        "Lipirani Kulikonse",
    "ch.heading":        "Njira Zomwe Mungalipirire Nazo",
    "ch.airtel.title":   "Airtel Money",
    "ch.airtel.body":    "Lipirani ndi Airtel Money STK. Tsimikizirani ndi PIN ya Airtel.",
    "ch.airtel.tag":     "MSANGAMSANGA",
    "ch.mpamba.title":   "TNM Mpamba",
    "ch.mpamba.body":    "Lipirani kuchokera ku Mpamba yanu. Tsimikizirani ndi PIN ya Mpamba.",
    "ch.mpamba.tag":     "MSANGAMSANGA",
    "ch.bank.title":     "Banki",
    "ch.bank.body":      "Tumizani ndalama ku akaunti ya MPS Treasury pa banki iliyonse. Refa: nambala ya chindapusa chanu.",
    "ch.bank.tag":       "MASIKU 1–2",
    "ch.card.title":     "Khadi",
    "ch.card.body":      "Lipirani ndi Visa kapena Mastercard. 3D Secure.",
    "ch.card.tag":       "MSANGAMSANGA",
    "ch.ussd.title":     "USSD *777#",
    "ch.ussd.body":      "Mulibe intaneti? Dialani *777#, sankhani 'Lipira Chindapusa', lembani nambala.",
    "ch.ussd.tag":       "POPANDA INTANETI",
    "ch.whatsapp.title": "WhatsApp",
    "ch.whatsapp.body":  "Tumizani uthenga ku +265 999 NEXUS pa WhatsApp.",
    "ch.whatsapp.tag":   "MWAUTHENGA",

    // ── FAQ ───────────────────────────────────────────────────
    "faq.eyebrow":       "Mafunso Ofunsidwa",
    "faq.heading":       "Mafunso Ndi Mayankho",
    "faq.q1":            "Kodi NexusFine ndi tsamba lokhalo lovomerezeka?",
    "faq.a1":            "Inde. NexusFine ndi tsamba lokhalo lovomerezeka ndi Apolisi a Malaŵi kulipira chindapusa cha pamsewu. Penyetsetsani kuti URL ndi nexusfine.mw kapena police.gov.mw musanalipire.",
    "faq.q2":            "Bwanji ngati sindikugwirizana ndi chindapusa changa?",
    "faq.a2":            "Sindani 'Tsutsani chindapusachi'. Mungafotokoze chifukwa chanu ndi umboni. Wapolisi wamkulu adzayang'ana mkati mwa masiku 5–10.",
    "faq.q3":            "Kodi ndilandila risiti pa pepala?",
    "faq.a3":            "Risiti yanu ya PDF ndi yovomerezeka. Sungani pa foni — mungaitulutse pa roadblock iliyonse.",
    "faq.q4":            "Bwanji ngati sindilipira?",
    "faq.a4":            "Akadutsa masiku 30, 10% imawonjezedwa. Akadutsa masiku 60, mlandu umaperekedwa ku khoti ndipo layisensi yanu ingayimitsidwe.",

    // ── footer ────────────────────────────────────────────────
    "footer.about":      "NexusFine ndi tsamba lovomerezeka la Apolisi a Malaŵi, Dipatimenti ya Misewu, lopangidwa ndi kuyendetsedwa ndi TechNexus MW.",
    "footer.col.service":"Ntchito",
    "footer.col.help":   "Thandizo",
    "footer.copyright":  "© 2026 Apolisi a Malaŵi. Ufulu wonse usungidwa.",
    "footer.poweredby":  "Yopangidwa ndi TechNexus MW",

    // ── pay modal ─────────────────────────────────────────────
    "pay.title":         "Tsimikizirani Kulipira",
    "pay.sub.airtel":    "Lembani nambala ya Airtel Money.",
    "pay.sub.mpamba":    "Lembani nambala ya TNM Mpamba.",
    "pay.sub.bank":      "Tipangira refa. Igwiritseni ntchito polipira ku banki.",
    "pay.sub.card":      "Mudzatumizidwa ku tsamba lotetezeka la khadi.",
    "pay.sub.ussd":      "Dialani *777# ndipo sankhani 'Lipira Chindapusa'.",
    "pay.sub.whatsapp":  "Tsegulani WhatsApp ndi kutumiza ku +265 999 NEXUS.",
    "pay.amount.lbl":    "NDALAMA YOLIPIRA",
    "pay.phone.lbl":     "NAMBALA YA MOBILE MONEY",
    "pay.phone.ph":      "991234567",
    "pay.btn.confirm":   "Tsimikizirani",
    "pay.btn.processing":"Tikukonza…",
    "pay.success.title": "Mwalipira Bwino",
    "pay.success.body":  "Chindapusa chanu chathandizidwa. Nambala ya risiti",
    "pay.success.btn":   "Tsitsani Risiti (PDF)",
    "pay.error":         "Sitinakwanitse kulipira. Yesaninso."
  }
};

window.NF_LANG = "en";

window.t = function (key) {
  const dict = window.NF_I18N[window.NF_LANG] || window.NF_I18N.en;
  return dict[key] ?? window.NF_I18N.en[key] ?? key;
};

window.applyI18n = function () {
  document.querySelectorAll("[data-i18n]").forEach(el => {
    el.textContent = window.t(el.getAttribute("data-i18n"));
  });
  document.querySelectorAll("[data-i18n-placeholder]").forEach(el => {
    el.placeholder = window.t(el.getAttribute("data-i18n-placeholder"));
  });
  document.documentElement.lang = window.NF_LANG === "ny" ? "ny" : "en";
};

window.setLang = function (lang, btn) {
  window.NF_LANG = (lang === "ny") ? "ny" : "en";
  document.querySelectorAll(".lang-toggle").forEach(b => b.classList.remove("active-lang"));
  if (btn) btn.classList.add("active-lang");
  window.applyI18n();
  // Re-render any open fine in the new language
  if (window.NF_STATE && window.NF_STATE.currentFine) {
    window.renderFine(window.NF_STATE.currentFine);
  }
};
