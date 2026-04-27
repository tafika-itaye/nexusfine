# Polish Pass — Asset Substitutions & Notes

Snapshot of the Module 7 polish wiring (2026-04-25). Items marked **placeholder** should be revisited before contract delivery.

## Brand & logo

| Surface | What's in place now | Future replacement |
|---------|---------------------|---------------------|
| Citizen `<nav>` brand-eagle | MPS crest (`img/gov/mps-crest.png`) | Paid NexusFine wordmark + emblem (per agreement, Q3 2026) |
| Admin sidebar brand-eagle | MPS crest | Paid NexusFine wordmark |
| Admin login brand-eagle | MPS crest | Paid NexusFine wordmark |
| Citizen footer brand row | MPS crest + "NEXUSFINE" wordmark | Paid logo lockup |
| Favicon (both portals) | MPS crest (`favicon.png`) | NexusFine icon-only mark |

The "NEXUSFINE" text wordmark is rendered with the Inter font as a placeholder until the paid brand asset lands. Swap by replacing the logo files only — markup uses `mps-crest.png`, no class names tied to the wordmark.

## Authority strip

A new section above the citizen footer shows:

- MPS crest → "Authorised by Malawi Police Service"
- DRTSS logo → "Operated by DRTSS · Road Safety & Traffic"
- Malawi coat of arms → "An Initiative of the Government of Malawi"

This is the institutional credibility row for the Minister demo.

## Hero image

Citizen hero now shows `hero-officer-checkpoint.jpg` (Alamy stock) under a navy-gradient overlay (92% → 78% opacity), with a navy topographic pattern at 18% opacity on top.

If the licensing on the Alamy file is unclear for production, swap to `hero-malawi-road.jpg` (audiala.com travel blog) which is editorial-permissive, or commission a TechNexus-shot photo from a real DRTSS checkpoint in Lilongwe.

## Channel logos

Citizen pay-options grid now shows real brand marks:

- Airtel Money — `airtel-money.png`
- TNM Mpamba — `tnm-mpamba.png`
- Bank Transfer card — 2×2 mosaic of Standard Bank, NBM, FDH, NBS
- Card card — Visa + Mastercard side-by-side
- USSD — `ussd-icon.png`
- WhatsApp — `whatsapp.svg`

All bank/card/wallet logos are used under brand-trademark fair-use; that is fine for an MPS-operated portal *displaying available payment methods*. If any rights-holder objects, swap their card to a generic icon and remove the named brand from copy.

## Officer avatars

Officers list (admin) and dashboard officer ranking now show avatar SVGs — chosen deterministically by hashing the badge number, so the same officer always gets the same face. Pool: 5 (male-1..3 + female-1..2). Default fallback: `avatar-default.svg`.

When real officer headshots are introduced (post-pilot), replace `AvatarFor(badge)` with `o.PhotoUrl ?? AvatarFor(badge)`.

## Decorative

- Citizen hero overlay pattern: `pattern-bg-navy.svg` @ opacity 0.18
- Admin sidebar background: same pattern, blend-mode: overlay
- Admin login backdrop: `malawi-map-outline.svg` watermark @ 40% center + pattern @ 12%

## Items not used yet

These came in the pack but aren't wired into any surface — keeping them in the repo for the next polish pass:

- `hero-citizen-paying.jpg` — earmarked for "How it works" section background
- `hero-malawi-road.jpg` — earmarked for FAQ section divider
- `hero-officer-tablet.jpg` — earmarked for Module 4 (MAUI officer app) marketing screen
- `hero-court-justice.jpg` — earmarked for "Disputes" page (post-pilot)
- `placeholder-vehicle.jpg`, `placeholder-document.jpg` — earmarked for fine-detail mockups
- `pattern-bg-light.svg` — earmarked for citizen "How it works" section
- `icon-fine-receipt.svg`, `icon-vehicle.svg`, `icon-money-mk.svg`, `icon-shield-check.svg`, `icon-clock-warning.svg` — earmarked for status pills in the next pass

## Asset mirroring

Both portals (`NexusFine.API` on :5121 and `NexusFine.Admin` on :5296) need their own copy of `wwwroot/img/`. Module 7 ships them mirrored once. The deploy script in Module 6 (when written) must re-run the mirror as a build step.

For now, manual sync after asset changes:

```bash
cp -r src/NexusFine.API/wwwroot/img/* src/NexusFine.Admin/wwwroot/img/
```

## Outstanding (when you have time)

- Paid NexusFine logo
- Real officer headshots (post-pilot)
- License clearance on hero photos
- Optimise photos through squoosh.app (target ≤200 KB)
