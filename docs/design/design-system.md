# NexusFine — Design System

**Status:** v1.0 · **Issued:** 2026-05-17
**Owner:** UI/UX function · **Source of truth:** `src/NexusFine.Admin/wwwroot/app.css`
**Cadence:** updated whenever a new token, component, or pattern lands.

This is the standard. Anyone designing or building a new NexusFine surface
should read this before opening a Figma file or a `.razor` file. Country
variants (ZM / MZ / TZ) inherit this system; differences are documented
as overrides per variant, not as forks.

---

## 1. Brand foundations

NexusFine is a State system. Its visual identity has to read as
**authoritative, calm, and uncluttered** — the opposite of a consumer
fintech.

**Three motifs anchor everything:**

1. **Malawi flag tricolour strip** — black / red (#CE1126) / green (#339E35) — appears as a thin band on the dashboard hero, login, status pages, and printed receipts. **It is never decorative.** It signals state authority.
2. **Gold accent** — `--gold` / `--gold-bright` — used sparingly for the institutional ring around the coat of arms, for primary CTA buttons, and for the "Active" / "Paid" interior of pills. Never as a background fill outside these uses.
3. **Navy backdrop** — `--surface-0` to `--surface-3` — the admin UI runs on dark navy. The citizen portal allows lighter surfaces but the navy is the brand colour.

The **single emblem** in NexusFine surfaces is the **Coat of Arms of Malawi**, displayed inside a gold ring on a white circle. We use the SVG (`img/gov/malawi-coat-of-arms.svg`) so it scales cleanly. The MPS crest serves as a sidebar / inline brand mark; never both crest + coat-of-arms on the same surface.

---

## 2. Tokens (single source of truth)

All tokens are CSS custom properties on `:root` in `app.css`. **Never
hard-code a hex value in a component** — read the token. New tokens
land here first, then in code.

### 2.1 Colour

| Token | Value | Use |
|-------|-------|-----|
| `--navy` | `#1A2744` | mid-tone navy (panels, secondary surfaces) |
| `--navy-dark` | `#111C33` | deep navy (hero band gradient end) |
| `--navy-mid` | `#22335A` | between navy and surface-3 |
| `--navy-light` | `#2E4478` | inline accents (KPI accent bars, "blue" pills) |
| `--surface-0` | `#0E1729` | app background |
| `--surface-1` | `#131F3A` | panel / card background |
| `--surface-2` | `#1A2744` | hover / lift |
| `--surface-3` | `#22335A` | active / pressed |
| `--border-dark` | `#2A3D6B` | strong dividers |
| `--border-mid` | `#1E2E52` | subtle dividers |
| `--red` | `#C0392B` | error fill |
| `--red-bright` | `#E74C3C` | error text |
| `--red-dim` | `rgba(192,57,43,0.15)` | error background |
| `--gold` | `#C8960C` | institutional accent, ring borders |
| `--gold-bright` | `#F0B429` | active gold text |
| `--gold-dim` | `rgba(200,150,12,0.15)` | gold background tint |
| `--success-text` | `#22A060` | success text + paid pill text |
| `--success-bg` | `rgba(26,122,74,0.12)` | success background |
| `--warning-text` | `#D97706` | warning text + overdue pill text |
| `--warning-bg` | `rgba(180,83,9,0.12)` | warning background |
| `--text-on-dark` | `#E8EDF8` | body text on navy |
| `--text-muted-d` | `#7B8DB8` | secondary text on navy |
| `--white` | `#FFFFFF` | emblem fill, light-card |

**Citizen portal** (light theme) reuses the same accent tokens but
substitutes `--surface-0/1/2/3` for light values. Token names stay the
same.

### 2.2 Typography

| Token | Value |
|-------|-------|
| `--font-body` | `'Segoe UI Variable', 'Segoe UI', system-ui, -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif` |
| `--font-mono` | `'Cascadia Mono', 'Cascadia Code', Consolas, ui-monospace, 'JetBrains Mono', Menlo, 'Courier New', monospace` |

Body type is the Windows 11 default stack. Mono is Microsoft's
open-source Cascadia family, which ships with slashed-zero by default.
On platforms without those fonts, the system falls back to native
alternatives.

**Numeric rendering is enforced globally:**

```css
body { font-variant-numeric: tabular-nums; }
.mono, .kpi-value, .pill, .data-table td.mono, .rv {
  font-variant-numeric: tabular-nums slashed-zero;
  font-feature-settings: "tnum" 1, "zero" 1, "ss01" 1;
}
```

Effect: every column of numbers aligns vertically, and `0` is never
confused with `O`.

#### 2.2.1 Type scale

| Element | Size | Weight | Notes |
|---------|----:|------:|-------|
| Page title (`.page-title`) | 22 px | 800 | Top of every page |
| Section eyebrow (`.section-eyebrow`) | 11 px | 700 | letter-spacing 0.18em, uppercase, gold |
| Heading 2 (`.panel-title`) | 15 px | 700 | Sits above tables / cards |
| Body | 14 px | 400 | App default |
| Body-small | 12 px | 400 | Captions, sub-text |
| KPI value (`.kpi-value`) | 24 px | 700 | Monospace |
| Pill (`.pill`) | 10 px | 700 | letter-spacing 0.08em, uppercase, mono |
| Caption (`.caption`) | 10 px | 600 | Form labels, metadata |
| Code / receipt (`.mono`) | 11–14 px | 400–700 | Always mono |

### 2.3 Spacing

We use a **4-px grid**. Tokens not defined; use multiples of 4 directly:
`4, 8, 12, 16, 20, 24, 28, 32, 40, 48, 64`.

### 2.4 Radius

| Token | Value | Use |
|-------|-------|-----|
| `--radius` | `8 px` | buttons, inputs, pills, small surfaces |
| `--radius-lg` | `12 px` | panels, KPI cards |
| `--radius-xl` | `16 px` | hero band, modal card, status card |

### 2.5 Shadow

| Token | Value | Use |
|-------|-------|-----|
| `--shadow-sm` | `0 1px 2px rgba(0,0,0,.25)` | input focus, hover lift |
| `--shadow` | `0 4px 12px rgba(0,0,0,.35)` | KPI card, modal |
| `--shadow-lg` | `0 12px 30px rgba(0,0,0,.5)` | full-screen overlays |

### 2.6 Motion

We avoid animation. Where motion is used, it is **subtle and brief**:

- Hover transitions: 150 ms ease
- Modal fade-in: 180 ms ease-out
- Live-dot pulse: 1.5 s linear
- Typing dots: 1.2 s linear
- Spinner: 0.8 s linear

No bounces, no spring physics, no parallax. We're a government tool.

---

## 3. Components

Every component lives in `src/NexusFine.Admin/Components/Shared/` or is
defined as a CSS class in `app.css`. Each is documented here with its
canonical class names + Razor surface (if any).

### 3.1 Buttons

| Variant | Class | When |
|---------|-------|------|
| Primary action | `.btn .btn-primary` | Apply / Save in red — destructive intent only |
| Gold CTA | `.btn .btn-gold` | Primary action on a page (Register Officer, Run scripted demo) |
| Ghost | `.btn .btn-ghost` | Secondary actions (Cancel, Refresh, Edit) |
| Small | `.btn .btn-sm` | Compact toolbars |
| Large | `.btn .btn-lg` | Login form, hero CTAs |
| Block | `.btn .btn-block` | Full-width in narrow containers |
| Disabled | `[disabled]` | 55% opacity, no pointer events |

**Rule:** at most **one** primary action per surface.

### 3.2 Pills

`.pill` is the base; status colour is the modifier.

| Modifier | Background | Text |
|----------|-----------|------|
| `.pill-paid` | success-bg | success-text |
| `.pill-active` | success-bg | success-text |
| `.pill-completed` | success-bg | success-text |
| `.pill-unpaid` | red-dim | red-bright |
| `.pill-failed` | red-dim | red-bright |
| `.pill-overdue` | warning-bg | warning-text |
| `.pill-pending` | gold-dim | gold-bright |
| `.pill-disputed` | gold-dim | gold-bright |
| `.pill-onbreak` | gold-dim | gold-bright |
| `.pill-offline` | rgba(100,100,120,.15) | #8899bb |
| `.pill-cancelled` | rgba(100,100,120,.15) | #8899bb |
| `.pill-reversed` | rgba(100,100,120,.15) | #8899bb |
| `.pill-timedout` | warning-bg | warning-text |

Status colour is **semantic**, not decorative. Do not invent ad-hoc pills.

### 3.3 Panels

`.panel` — the base card surface. Header via `.panel-hdr` containing
`.panel-title` and `.panel-sub`. Used for every data table, every
group of stats, every modal-ish block on a page.

### 3.4 KPI cards

`.kpi-card .kpi-card-{gold|green|blue|red}` with a `.kpi-accent` bar
across the top, `.kpi-label`, `.kpi-value`, and an optional
`.kpi-bg-icon` watermark in the corner. Four cards per row at desktop,
collapses to two at tablet.

### 3.5 Data tables

`.data-table` — sticky header, hover row tint, `.mono` cells for refs /
plates / amounts. Empty rows are replaced by `<EmptyState>` (see 3.7),
**not** by "No data" text.

### 3.6 Modals

`.modal-shade .modal-card .modal-hdr .modal-body .modal-foot`. Form
layouts inside modals use `.form-grid` (two-column on desktop, stacked
on mobile).

### 3.7 Empty states

`<EmptyState Title="…" Hint="…" ActionLabel="…" ActionHref="…" />` —
illustrated, never bare text. Image is
`img/illustrations/empty-state-no-data.svg` by default; override via the
`Image` parameter for variants.

### 3.8 Officer avatars

Two states only (per D-014):
- **With photo** — `officer.PhotoUrl`, displayed as a 32–54 px circle
- **Without photo** — `img/officers/avatar-silhouette.svg` (uniform police silhouette)

Random cartoon avatars are **not used**. Display logic falls back to
the silhouette via the `onerror` attribute on every `<img>`.

### 3.9 Hero band (Dashboard)

`.dash-hero .dash-hero-grid .dash-hero-emblem` — only on the Dashboard.
Other pages use `.page-header` (eyebrow + title) for their lead block.

### 3.10 Brand mark (`.brand-eagle`)

The institutional gold ring with the coat of arms inside. Sizes:

| Surface | Diameter |
|---------|---------:|
| Sidebar | 52 px |
| Login mark | 72 px |
| Dashboard hero | 130 px |

All instances use `object-fit: contain` with white background and ~7px
inner padding so the artwork breathes.

### 3.11 Live dot

`.live-dot` — 8-px pulsing green dot. Used on Live Fine Feed and Live
Conversations to indicate active polling.

### 3.12 Status pages

404 and 500 use `BlankLayout` + the `.status-*` classes. Same flag
strip, same coat-of-arms ring, same support-contact footer.

### 3.13 Region band (Departments page)

`.region-band` — full-bleed photo banner per region, navy gradient
overlay, title + counts. Three banners (Northern / Central / Southern),
sorted geographically.

### 3.14 Phone mockups

Two:
- **WhatsApp** — `.phone-bezel .phone-screen .wa-*` (port 5296 `/whatsapp`)
- **USSD** — `.featurephone .featurephone-screen .ussd-*` (port 5296 `/ussd`)

Both are real chat UIs, not images. They hit `/api/chatbot/...` and
render live state.

---

## 4. Patterns

### 4.1 Page header

Every page opens with:

```razor
<div class="page-header">
    <div class="section-eyebrow">Operations</div>
    <h1 class="page-title">Dashboard</h1>
</div>
```

The eyebrow indicates the page-group (Operations · Management · Reports
· Citizen Channels · System). The title is the page name.

### 4.2 Filter toolbar

Filter chips + an inline `.form-input` search + a refresh `.btn .btn-sm
.btn-ghost`. The active filter uses `.btn-primary`. Filters always live
above the panel they affect.

### 4.3 Create / Edit modal

Opens via a `.btn .btn-gold` labelled `＋ Add X` or `✎ Edit`. Inside the
modal, form errors render as a `.alert .alert-danger` band at the top
of the modal body. Cancel + Save buttons sit in `.modal-foot`.

### 4.4 Status indicators

A status is **always** a pill (3.2). Never bare text, never coloured
circles alone. The pill colour follows §3.2 strictly.

### 4.5 Reference numbers

References (fines, receipts, transactions, badges) are **always**
rendered with `.mono`. Gold colour (`text-gold`) is reserved for fine
references; receipt numbers stay default text colour to disambiguate.

### 4.6 Amounts

Amounts are **always** mono, with `MK` prefix and comma thousands
separators. No decimals (Kwacha denomination policy).

### 4.7 Times

`dd MMM · HH:mm` on dense rows; `dd MMMM yyyy · HH:mm:ss CAT` on
detail surfaces. Times in the audit-log + receipts always include the
timezone abbreviation (`CAT`).

---

## 5. Accessibility

The pilot ships against **WCAG 2.1 AA**. Current state and gaps:

| Check | Status |
|-------|:------:|
| Colour-contrast — body text on navy | ✅ 8.5:1 |
| Colour-contrast — muted text on navy | 🟡 4.4:1 (just under AA for small text; flagged for U2) |
| Keyboard navigation through tables | ✅ |
| Focus rings on interactive elements | 🟡 partial; the focus ring on `.btn-gold` is faint |
| `<img>` alt text on every emblem | ✅ |
| ARIA labels on icon-only buttons | 🟡 inconsistent — being addressed in U2 audit |
| Page-zoom to 200% without overflow | ✅ except WhatsApp phone bezel (acceptable — non-essential surface) |
| Screen-reader-friendly status pills | 🟡 we use colour + text; OK but role="status" not declared |
| Reduced-motion preference | 🔴 not yet honoured |

The **U2 audit** (Wave 4) will close the medium-confidence items.

---

## 6. Localisation rules

The admin portal is English-only by deliberate choice — police
operators in Malawi work in English. The citizen portal toggles EN / NY.

- All citizen-facing strings live in `wwwroot/citizen/i18n.js`
- Toggle is client-side `setLang(...)` — no reload (verified in U6)
- Numbers (and decimals) are formatted EN-US even in NY mode — this
  matches MERA's currency-display convention.
- Dates: dd MMM yyyy in both languages — no MM/DD ambiguity.

When ZM / MZ / TZ variants land, each gets its own `i18n` file:
`citizen/i18n.zm.json`, `i18n.mz.json`, `i18n.tz.json` — adding Bemba,
Portuguese, and Swahili respectively. The token system itself does
not change between countries.

---

## 7. Country-flavour overrides (Wave 5+)

When a country flavour brand activates (country-flavoured branding per
TJ's directive of 2026-04-27 — "NexusFine Zambia", "NexusFine
Mozambique", etc.) the deltas live in a per-country override stylesheet
imported AFTER `app.css`:

```html
<link rel="stylesheet" href="app.css">
<link rel="stylesheet" href="overrides/{country-code}.css">
```

Override values are limited to:
- Flag-stripe colours
- Coat-of-arms emblem path
- Sidebar branding text
- Primary brand colour, if it diverges (e.g. ZM green)
- Date/number locale

Component class names, page structures, type scale and spacing **do
not** vary between countries. That uniformity is the asset.

---

## 8. Adding a new component — workflow

1. Sketch it in Figma against the existing tokens.
2. Build it as a `.razor` partial under `Components/Shared/` if it has
   state; or as a CSS class in `app.css` if it's pure presentation.
3. Add a section here.
4. Add an example usage in an existing page (we don't ship dead code).
5. PR-review checklist (one item): "Does this introduce a new token? If
   yes, did we update this document and `app.css :root`?"

---

## 9. Anti-patterns (don't do these)

- ❌ Inline hex colours in a `style="…"` attribute
- ❌ Inline pixel-value font sizes that don't appear in §2.2.1
- ❌ A new pill colour for a new status — use an existing semantic class or extend the system here first
- ❌ Animations longer than 250 ms
- ❌ More than one primary `btn-gold` per surface
- ❌ Both crest and coat-of-arms on the same page
- ❌ Random cartoon avatars (legacy — replaced per D-014)
- ❌ Emoji as a control affordance (acceptable as decoration, never as the only signifier)

---

## 10. References

- `src/NexusFine.Admin/wwwroot/app.css` — implementation
- `docs/DECISIONS.md` D-014 — officer photo design rule
- `docs/department-recommendations.md` U1 (this doc), U2 (accessibility audit, follow-up)
- WCAG 2.1 AA — accessibility baseline
- Microsoft Cascadia + Segoe UI Variable — typography references
