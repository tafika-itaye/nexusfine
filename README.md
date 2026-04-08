# NexusFine — Static HTML Demo Package
**TechNexus MW** · technexusmw.com  
Client: Malawi Police Service / DRTSS  
Project: Digital Traffic Fines Management & Payment Platform  
Version: 2.0 · State Blue Theme · April 2026

---

## File Tree

```
nexusfine/
├── css/
│   └── theme.css              ← Shared brand tokens (colors, fonts, components)
├── citizen/
│   └── index.html             ← Public-facing fine lookup & payment portal
├── admin/
│   └── index.html             ← Supervisor/admin operations dashboard
└── README.md                  ← This file
```

---

## GitHub Pages Deployment

1. Push this folder to a GitHub repository (e.g. `nexusfine-demo`)
2. Go to **Settings → Pages → Source: main branch / root**
3. Access portals at:
   - `https://<username>.github.io/nexusfine-demo/citizen/`
   - `https://<username>.github.io/nexusfine-demo/admin/`

> Both portals reference `../css/theme.css` — keep the folder structure intact.

---

## What Is a Demo vs Live System

| Feature | Demo (this package) | Live ASP.NET System |
|---------|--------------------|--------------------|
| Fine lookup | Hardcoded sample data | Real SQL Server query |
| Payment processing | Simulated | Airtel/Mpamba/bank API |
| Officer data | Static dummy data | Live officer API |
| Charts | Static dummy data | Real-time DB aggregates |
| Auth / login | None | ASP.NET Identity + RBAC |
| Language switch | Partial (EN/NY) | Full i18n with resource files |
| Receipt PDF | Browser print | Server-generated PDF |
| USSD / WhatsApp | Not present | Gateway integrations |
| Audit log | Not present | Full SQL audit trail |

---

## Backend Migration Notes

When connecting to the ASP.NET Core 8 backend, replace the following in each file:

### citizen/index.html
```javascript
// DEMO: hardcoded fine objects
const sampleFines = { unpaid: {...}, paid: {...} };

// LIVE: replace lookupFine() with:
async function lookupFine() {
  const val = document.getElementById(inputMap[activeTab]).value.trim();
  const res = await fetch(`/api/fines/lookup?type=${activeTab}&value=${encodeURIComponent(val)}`);
  const fine = await res.json();
  renderFine(fine);
}

// LIVE: replace simulatePay() with:
async function simulatePay() {
  const res = await fetch('/api/payments/initiate', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ fineRef: currentPayRef, phone: document.getElementById('pay-phone').value, channel: currentChannel })
  });
  const result = await res.json();
  // handle result.receiptNo, result.status
}
```

### admin/index.html
```javascript
// DEMO: static arrays for officers, fines, KPIs

// LIVE: replace on page load with:
async function loadDashboard() {
  const [kpis, officers, feed] = await Promise.all([
    fetch('/api/dashboard/kpis').then(r => r.json()),
    fetch('/api/officers/performance?date=today').then(r => r.json()),
    fetch('/api/fines/recent?limit=8').then(r => r.json()),
  ]);
  renderKPIs(kpis);
  buildOfficerTable(officers);
  buildFeed(feed);
}

// Chart data — replace buildLine() static datasets with:
async function buildLine(period) {
  const data = await fetch(`/api/analytics/trend?period=${period}`).then(r => r.json());
  // pass data.labels, data.issued, data.collected to Chart.js
}
```

---

## Brand Colors (MPS Identity)

| Token | Hex | Usage |
|-------|-----|-------|
| `--navy` | `#1a2744` | Primary nav, header backgrounds |
| `--navy-dark` | `#111c33` | Page background, gov topbar |
| `--gold` | `#c8960c` | Accent, borders, highlights |
| `--gold-bright` | `#f0b429` | Text on dark, active states |
| `--red` | `#c0392b` | MPS accent, danger, unpaid |
| `--red-bright` | `#e74c3c` | Hover states |

---

## Image Search Terms

Use these terms on Unsplash, Pexels, or Pixabay for real photography:

### Citizen Portal
- `"malawi road traffic police"` — officer at roadside
- `"african police officer uniform"` — law enforcement authority
- `"mobile payment africa smartphone"` — mobile money UX
- `"malawi road highway africa"` — local road context
- `"online payment government service"` — fintech/gov UX

### Admin Dashboard
- `"police control room africa"` — command centre feel
- `"african police officer tablet"` — officer with device
- `"traffic enforcement africa"` — field operations
- `"data dashboard government"` — analytics context

> All images must be royalty-free, culturally appropriate, and ideally featuring Malawian/African subjects. Avoid generic stock photos with Western settings.

---

## Contacts

**TechNexus MW**  
Plot 484 Naperi Avenue, Blantyre, Malawi  
BRN: A6SNWQY | MSME Certified Small Enterprise  
📧 technexus_mw@proton.me  
📞 +265 889 941 700  
🌐 technexusmw.com
