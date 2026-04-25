/* NexusFine — Citizen Portal application logic.
 *
 * Talks to the live NexusFine API:
 *   GET  /api/fines/lookup?type={plate|ref|id}&value=...
 *   POST /api/payments/initiate   { fineRef, channel, phone? }
 *   POST /api/payments/confirm    { transactionReference, gatewayPayload? }
 *
 * Channels match the PaymentChannel enum on the backend:
 *   AirtelMoney = 0, TnmMpamba = 1, BankTransfer = 2, Card = 3, Ussd = 4, WhatsApp = 5, Cash = 6
 */
window.NF_STATE = {
  searchType:   "plate",
  currentFine:  null,
  currentTxn:   null,   // { reference, amount, channel } after initiate
  currentReceipt: null  // { receiptNumber, amount, channel, completedAt } after confirm
};

const API = (() => {
  // Same-origin: served by the API itself out of wwwroot
  const base = "";
  return {
    async lookup(type, value) {
      const r = await fetch(`${base}/api/fines/lookup?type=${encodeURIComponent(type)}&value=${encodeURIComponent(value)}`);
      if (r.status === 404) return { notFound: true };
      if (!r.ok) throw new Error(`Lookup failed (${r.status})`);
      return await r.json();
    },
    async initiate(fineRef, channel, phone) {
      const r = await fetch(`${base}/api/payments/initiate`, {
        method:  "POST",
        headers: { "Content-Type": "application/json" },
        body:    JSON.stringify({ fineRef, channel, phone })
      });
      const j = await r.json().catch(() => ({}));
      if (!r.ok) throw new Error(j.message || "Could not initiate payment");
      return j;
    },
    async confirm(transactionReference, gatewayPayload) {
      const r = await fetch(`${base}/api/payments/confirm`, {
        method:  "POST",
        headers: { "Content-Type": "application/json" },
        body:    JSON.stringify({ transactionReference, gatewayPayload })
      });
      const j = await r.json().catch(() => ({}));
      if (!r.ok) throw new Error(j.message || "Could not confirm payment");
      return j;
    }
  };
})();

// ── HELPERS ───────────────────────────────────────────────────
const $ = (sel) => document.querySelector(sel);
const fmtMK = (n) => "MK " + (n ?? 0).toLocaleString("en-MW");
const fmtDate = (iso) => {
  if (!iso) return "—";
  try {
    const d = new Date(iso);
    return d.toLocaleString(window.NF_LANG === "ny" ? "en-MW" : "en-MW",
      { year: "numeric", month: "short", day: "2-digit", hour: "2-digit", minute: "2-digit" });
  } catch { return iso; }
};
const fmtDateOnly = (iso) => {
  if (!iso) return "—";
  try { return new Date(iso).toLocaleDateString("en-MW", { year: "numeric", month: "short", day: "2-digit" }); }
  catch { return iso; }
};

// Channel string → backend enum name
const CHANNEL = {
  airtel:   "AirtelMoney",
  mpamba:   "TnmMpamba",
  bank:     "BankTransfer",
  card:     "Card",
  ussd:     "Ussd",
  whatsapp: "WhatsApp"
};

// ── SEARCH UI ────────────────────────────────────────────────
window.switchTab = function (which, btn) {
  window.NF_STATE.searchType = which;
  document.querySelectorAll(".tab-btn").forEach(b => b.classList.remove("active"));
  if (btn) btn.classList.add("active");
  ["plate", "ref", "id"].forEach(k => {
    const el = document.getElementById("tab-" + k);
    if (el) el.style.display = (k === which) ? "block" : "none";
  });
};

window.lookupFine = async function () {
  const type  = window.NF_STATE.searchType;
  const inp   = document.getElementById(type + "-input");
  const value = (inp?.value || "").trim();
  const btn   = document.getElementById("btn-search");
  const panel = document.getElementById("result-panel");

  if (!value) {
    showAlert(window.t("lookup.error.empty"), "danger");
    return;
  }

  btn.disabled    = true;
  btn.dataset.original = btn.dataset.original || btn.querySelector("span")?.textContent;
  btn.querySelector("span").textContent = window.t("lookup.searching");

  try {
    const fine = await API.lookup(type, value);
    if (fine.notFound) {
      showAlert(window.t("lookup.error.none"), "danger");
      panel.style.display = "none";
      return;
    }
    window.NF_STATE.currentFine = fine;
    window.renderFine(fine);
  } catch (err) {
    console.error(err);
    showAlert(window.t("lookup.error.net"), "danger");
  } finally {
    btn.disabled = false;
    btn.querySelector("span").textContent = window.t("lookup.btn");
  }
};

// ── RESULT RENDER ────────────────────────────────────────────
window.renderFine = function (fine) {
  const panel = document.getElementById("result-panel");
  panel.style.display = "block";

  const totalDue   = (fine.amount ?? 0) + (fine.penaltyAmount ?? 0);
  const isPaid     = fine.status === "Paid" || fine.status === 1; // EF returns enum name as string
  const isOverdue  = !isPaid && fine.dueDate && (new Date(fine.dueDate) < new Date());
  const offence    = fine.offenceCode?.name ?? "—";
  const officer    = fine.officer ? `${fine.officer.rank ?? ""} ${fine.officer.fullName ?? ""}`.trim() : "—";
  const lastPayment = (fine.payments || []).find(p => p.status === "Completed" || p.status === 1);

  panel.innerHTML = `
    <div class="fine-result">
      <div class="fine-result-header">
        <div>
          <div class="fine-ref-mono">${fine.referenceNumber ?? ""}</div>
          <div class="fine-plate-text">${fine.plateNumber ?? ""}${fine.vehicleMake ? " · " + fine.vehicleMake + " " + (fine.vehicleModel ?? "") : ""}</div>
        </div>
        <span class="pill ${isPaid ? "pill-paid" : "pill-unpaid"}">${isPaid ? "PAID" : "UNPAID"}</span>
      </div>
      <div class="fine-result-body">
        <div class="detail-grid">
          <div class="detail-item"><div class="k">${window.t("result.driver")}</div><div class="v">${fine.driverName ?? "—"}</div></div>
          <div class="detail-item"><div class="k">${window.t("result.id")}</div><div class="v mono">${fine.driverNationalId ?? "—"}</div></div>
          <div class="detail-item full"><div class="k">${window.t("result.offence")}</div><div class="v">${offence}</div></div>
          <div class="detail-item"><div class="k">${window.t("result.location")}</div><div class="v">${fine.location ?? "—"}</div></div>
          <div class="detail-item"><div class="k">${window.t("result.issued")}</div><div class="v">${fmtDateOnly(fine.issuedAt)}</div></div>
          <div class="detail-item"><div class="k">${window.t("result.due")}</div><div class="v">${fmtDateOnly(fine.dueDate)}</div></div>
          <div class="detail-item"><div class="k">${window.t("result.officer")}</div><div class="v">${officer}</div></div>
        </div>

        <div class="fine-timeline">
          <div class="timeline-step done"><div class="timeline-dot">✓</div><div class="timeline-label">${window.t("result.timeline.issued")}</div></div>
          <div class="timeline-step done"><div class="timeline-dot">✓</div><div class="timeline-label">${window.t("result.timeline.notified")}</div></div>
          <div class="timeline-step ${isPaid ? "done" : "active"}"><div class="timeline-dot">${isPaid ? "✓" : "$"}</div><div class="timeline-label">${window.t("result.timeline.payment")}</div></div>
          <div class="timeline-step ${isPaid ? "done" : ""}"><div class="timeline-dot">${isPaid ? "✓" : "·"}</div><div class="timeline-label">${window.t("result.timeline.closed")}</div></div>
        </div>

        <div class="fine-amount-box">
          <div>
            <div class="lbl">${isPaid ? window.t("result.amount.paid") : window.t("result.amount.due")}</div>
            <div class="amount ${isPaid ? "paid" : "unpaid"}">${fmtMK(totalDue)}</div>
          </div>
          <div class="due-note">${isOverdue ? window.t("result.overdue") : (isPaid ? "" : window.t("result.due.note"))}</div>
        </div>

        ${ isPaid
          ? `<button class="btn-search" onclick="downloadPaidReceipt()">📄 ${window.t("result.download")}</button>`
          : `<div style="font-size:11px;color:var(--text-muted-l);text-transform:uppercase;letter-spacing:0.08em;margin-bottom:8px;">${window.t("result.pay.heading")}</div>
             <div class="pay-grid">
               <button class="pay-option" onclick="openPay('airtel')">  <span class="ico">📱</span> ${window.t("result.pay.airtel")}</button>
               <button class="pay-option" onclick="openPay('mpamba')">  <span class="ico">📲</span> ${window.t("result.pay.mpamba")}</button>
               <button class="pay-option" onclick="openPay('bank')">    <span class="ico">🏦</span> ${window.t("result.pay.bank")}</button>
               <button class="pay-option" onclick="openPay('card')">    <span class="ico">💳</span> ${window.t("result.pay.card")}</button>
               <button class="pay-option" onclick="openPay('ussd')">    <span class="ico">📞</span> ${window.t("result.pay.ussd")}</button>
               <button class="pay-option" onclick="openPay('whatsapp')"><span class="ico">💬</span> ${window.t("result.pay.whatsapp")}</button>
             </div>`
        }
      </div>
    </div>
  `;

  // Smooth scroll to result
  panel.scrollIntoView({ behavior: "smooth", block: "center" });
};

function showAlert(msg, kind) {
  const panel = document.getElementById("result-panel");
  panel.style.display = "block";
  panel.innerHTML = `<div class="alert alert-${kind}" style="background:#fef2f2;color:#b91c1c;border-color:#dc2626;">⚠ ${msg}</div>`;
}

// ── PAY MODAL ────────────────────────────────────────────────
window.openPay = function (channelKey) {
  const fine    = window.NF_STATE.currentFine;
  if (!fine) return;
  const total   = (fine.amount ?? 0) + (fine.penaltyAmount ?? 0);
  const overlay = document.getElementById("pay-modal");
  const channel = CHANNEL[channelKey];

  document.getElementById("pay-amount").textContent = fmtMK(total);
  document.getElementById("pay-fine-ref").textContent = fine.referenceNumber;
  document.getElementById("pay-sub").textContent = window.t("pay.sub." + channelKey);
  document.getElementById("pay-modal-title").textContent =
    window.t("pay.title") + " — " + window.t("result.pay." + channelKey);

  // Phone field only for mobile money
  const phoneRow = document.getElementById("phone-row");
  phoneRow.style.display = (channelKey === "airtel" || channelKey === "mpamba") ? "block" : "none";
  document.getElementById("phone-input").value = fine.driverPhone?.replace(/^\+?265/, "") || "";

  // Reset state
  document.getElementById("pay-form-state").style.display   = "block";
  document.getElementById("pay-success-state").style.display = "none";
  document.getElementById("pay-error").textContent = "";

  const btn = document.getElementById("pay-confirm-btn");
  btn.disabled = false;
  btn.textContent = window.t("pay.btn.confirm");
  btn.onclick = () => doPay(channel, channelKey);

  overlay.classList.add("open");
};

window.closePay = function () {
  document.getElementById("pay-modal").classList.remove("open");
};

async function doPay(channel, channelKey) {
  const fine  = window.NF_STATE.currentFine;
  const btn   = document.getElementById("pay-confirm-btn");
  const errEl = document.getElementById("pay-error");
  const phone = document.getElementById("phone-input").value.trim();

  errEl.textContent = "";
  btn.disabled = true;
  btn.textContent = window.t("pay.btn.processing");

  try {
    const phoneFull = phone ? ("+265" + phone.replace(/^0+/, "")) : null;
    const init = await API.initiate(fine.referenceNumber, channel, phoneFull);
    // Simulated gateway: confirm immediately for the demo flow.
    const conf = await API.confirm(init.transactionReference, JSON.stringify({ source: "citizen-portal" }));

    window.NF_STATE.currentReceipt = {
      receiptNumber: conf.receiptNumber,
      amount:        conf.amount,
      channel:       conf.channel,
      completedAt:   conf.completedAt,
      fineRef:       conf.fineReference,
      driverName:    fine.driverName,
      offence:       fine.offenceCode?.name,
      location:      fine.location,
      plate:         fine.plateNumber
    };

    document.getElementById("pay-form-state").style.display    = "none";
    document.getElementById("pay-success-state").style.display = "block";
    document.getElementById("success-receipt").textContent     = conf.receiptNumber;
    document.getElementById("success-body").textContent        = window.t("pay.success.body");
    document.getElementById("success-title").textContent       = window.t("pay.success.title");
    document.getElementById("success-download").textContent    = window.t("pay.success.btn");

    // Mark the in-memory fine as paid + re-render so the underlying card flips to PAID
    fine.status = "Paid";
    fine.payments = [...(fine.payments || []), {
      receiptNumber: conf.receiptNumber,
      amount:        conf.amount,
      channel:       conf.channel,
      status:        "Completed",
      completedAt:   conf.completedAt
    }];
    window.renderFine(fine);
  } catch (err) {
    console.error(err);
    errEl.textContent = err.message || window.t("pay.error");
    btn.disabled = false;
    btn.textContent = window.t("pay.btn.confirm");
  }
}

// ── PDF RECEIPT (jsPDF) ──────────────────────────────────────
window.downloadReceipt = function () {
  const r = window.NF_STATE.currentReceipt;
  if (!r) return;
  generatePdfReceipt(r);
};

window.downloadPaidReceipt = function () {
  // For an already-paid fine pulled from the API
  const fine = window.NF_STATE.currentFine;
  if (!fine) return;
  const completed = (fine.payments || []).find(p => p.status === "Completed" || p.status === 1)
                   || (fine.payments || [])[0];
  if (!completed) return;
  generatePdfReceipt({
    receiptNumber: completed.receiptNumber,
    amount:        completed.amount,
    channel:       completed.channel,
    completedAt:   completed.completedAt,
    fineRef:       fine.referenceNumber,
    driverName:    fine.driverName,
    offence:       fine.offenceCode?.name,
    location:      fine.location,
    plate:         fine.plateNumber
  });
};

function generatePdfReceipt(r) {
  if (!window.jspdf) {
    alert("PDF library is loading, please try again in a moment.");
    return;
  }
  const { jsPDF } = window.jspdf;
  const doc = new jsPDF({ unit: "pt", format: "a4" });

  // Brand colour bar
  doc.setFillColor(26, 39, 68); // navy
  doc.rect(0, 0, 595, 70, "F");
  doc.setFillColor(200, 150, 12); // gold
  doc.rect(0, 70, 595, 4, "F");

  // Header text
  doc.setTextColor(255, 255, 255);
  doc.setFont("helvetica", "bold");
  doc.setFontSize(20);
  doc.text("NEXUSFINE", 40, 38);
  doc.setFontSize(10);
  doc.setFont("helvetica", "normal");
  doc.text("MALAWI POLICE SERVICE — TRAFFIC FINE RECEIPT", 40, 58);

  // Body
  doc.setTextColor(20, 20, 30);
  doc.setFontSize(11);

  let y = 110;
  const labelCol = 40, valCol = 220;
  const row = (label, value) => {
    doc.setFont("helvetica", "bold");
    doc.text(label, labelCol, y);
    doc.setFont("helvetica", "normal");
    doc.text(String(value ?? "—"), valCol, y);
    y += 22;
  };

  doc.setFont("helvetica", "bold");
  doc.setFontSize(13);
  doc.text("Official Payment Receipt", labelCol, y);
  y += 28;

  doc.setFontSize(11);
  row("Receipt No.",      r.receiptNumber);
  row("Fine Reference",   r.fineRef);
  row("Plate Number",     r.plate);
  row("Driver",           r.driverName);
  row("Offence",          r.offence);
  row("Location",         r.location);
  row("Channel",          r.channel);
  row("Paid On",          fmtDate(r.completedAt));
  y += 8;

  // Amount block
  doc.setFillColor(244, 246, 251);
  doc.rect(40, y, 515, 60, "F");
  doc.setFontSize(10);
  doc.setTextColor(90, 106, 138);
  doc.text("AMOUNT PAID", 56, y + 22);
  doc.setFontSize(22);
  doc.setFont("helvetica", "bold");
  doc.setTextColor(34, 160, 96);
  doc.text(fmtMK(r.amount), 56, y + 48);
  y += 90;

  // Status pill
  doc.setFillColor(34, 160, 96);
  doc.roundedRect(40, y, 70, 22, 4, 4, "F");
  doc.setTextColor(255, 255, 255);
  doc.setFontSize(10);
  doc.setFont("helvetica", "bold");
  doc.text("PAID", 60, y + 15);

  // Disclaimer
  y += 60;
  doc.setTextColor(90, 106, 138);
  doc.setFontSize(9);
  doc.setFont("helvetica", "normal");
  doc.text("This is a digitally generated receipt and is legally valid as proof of payment.", 40, y);
  doc.text("Verify authenticity at https://nexusfine.mw/verify  ·  Receipt: " + r.receiptNumber, 40, y + 14);

  // Footer bar
  doc.setFillColor(17, 28, 51);
  doc.rect(0, 800, 595, 42, "F");
  doc.setTextColor(180, 192, 220);
  doc.setFontSize(9);
  doc.text("Malawi Police Service · Road Safety & Traffic Department", 40, 822);
  doc.text("Powered by TechNexus MW", 420, 822);

  doc.save(`NexusFine-${r.receiptNumber}.pdf`);
}

// ── FAQ TOGGLE ───────────────────────────────────────────────
window.toggleFaq = function (btn) {
  btn.classList.toggle("open");
  btn.nextElementSibling.classList.toggle("open");
};

// ── ON LOAD ──────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", () => {
  window.applyI18n();
  // Keep nav link highlight on scroll? Not critical for demo.
});
