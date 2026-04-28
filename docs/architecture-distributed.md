# NexusFine — Distributed Architecture Sketch

**Status:** Draft v2 (2026-04-27) — TJ decisions on Q1–Q7 applied
**Decision needed by:** before Module 4 (officer app) build starts
**Supersedes:** the implicit single-node assumption in earlier modules

## TJ decisions log (2026-04-27)

| # | Question | Decision |
|---|----------|----------|
| 1 | Receipt numbering | **Prefix-then-canonicalise.** Devices/stations mint local refs; HQ assigns canonical `NXF-…` on sync. |
| 2 | Multi-officer device | **Shared with login switch.** Each tablet supports multiple officers across shifts; queued records are tagged with the officer who issued them. |
| 3 | Counter lookup when HQ down | **Yes — replicate citizen-payable fines to every station.** See §11 for scope. |
| 4 | Counter-side mobile money | **No.** Strict role separation: stations process counter-cash only. Mobile money strictly officer-device or citizen-portal/USSD/WhatsApp. |
| 5 | Data retention | **Recommended policy applied** (see §12). Station: 12-month rolling hot+warm cache. HQ: 7 yr financial / 10 yr disputed / indefinite audit. To be confirmed with MPS Records & MACRA. |
| 6 | Sync alarms | **Two-layer.** End-user: simple "Synced ✅ / Failed ⚠" pill. Audit: IT/supervisors see full sync events. Alarm bar appears on tablet AND admin portal after **N consecutive failed sync days** (default N=2). |
| 7 | Lost-device auto-revoke | **72 hours** with no heartbeat → device auto-revokes; supervisor must re-pair. |

---

## 1 · Topology

Three tiers. Each tier owns its own data and runs without the others when the network is down.

```
                ┌─────────────────────────────┐
                │   HQ — DRTSS / MPS Lilongwe │
                │   (single source of truth)  │
                │                             │
                │   • Reference data master   │
                │   • Audit lake              │
                │   • Treasury settlement     │
                │   • Citizen portal + API    │
                └──────────────┬──────────────┘
                               │  HTTPS · 4G/fibre
                               │  Sync every 5 min
                               │  (or on change push)
                ┌──────────────┴──────────────┐
                │      STATION SERVER         │
                │   (one per police station)  │
                │                             │
                │   • Local cached reference  │
                │   • Local fines + payments  │
                │   • Counter cash payments   │
                │   • Officer pairing         │
                └──────────────┬──────────────┘
                               │  Wi-Fi / Bluetooth / USB
                               │  Sync at shift start/end
                               │  + every 2h on connect
                ┌──────────────┴──────────────┐
                │      OFFICER DEVICE         │
                │   (Android tablet, MAUI)    │
                │                             │
                │   • Scoped reference        │
                │   • Fines issued today      │
                │   • Payment receipts        │
                │   • NFC, camera, GPS        │
                └─────────────────────────────┘
```

---

## 2 · Authority model — who can write what

| Entity | HQ writes | Station writes | Device writes |
|--------|-----------|----------------|---------------|
| Department | ✅ master | read-only | read-only |
| Station | ✅ master | own metadata only | read-only |
| PatrolPost / StationedArea | ✅ master | read-only | read-only |
| OffenceCode + tariff | ✅ master | read-only | read-only |
| Officer (registration, badge, role) | ✅ master | read-only | read-only |
| Officer Status (Active/OnBreak/Offline) | observed | ✅ writes | ✅ writes |
| Device pairing | ✅ approves | ✅ initiates | — |
| Fine | observed (replica) | ✅ counter-issued | ✅ field-issued |
| Payment | observed | ✅ counter | ✅ field |
| Audit log | ✅ aggregated | ✅ writes own | ✅ writes own |

**Rule of thumb:** reference data flows down, operational data flows up. Conflicts at the same level (rare) resolve last-write-wins by `UpdatedAt` UTC.

---

## 3 · New / changed entities

### New

```csharp
public class Station                 // belongs to a Department
{
    public int Id { get; set; }
    public string Code { get; set; } = "";          // STN-LIL-001
    public string Name { get; set; } = "";          // "Area 18 Police Station"
    public int DepartmentId { get; set; }
    public string Zone { get; set; } = "";          // denormalised from dept for filter speed
    public string PhysicalAddress { get; set; } = "";
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string? StationServerEndpoint { get; set; }   // e.g. https://stn-lil-001.local:5121
    public string? StationServerPublicKey { get; set; }  // mTLS pinning for HQ ↔ station
    public bool IsActive { get; set; } = true;

    public ICollection<PatrolPost> PatrolPosts { get; set; } = new List<PatrolPost>();
    public ICollection<Officer>    Officers    { get; set; } = new List<Officer>();
}

public class PatrolPost              // a checkpoint or beat under a Station
{
    public int Id { get; set; }
    public string Code { get; set; } = "";          // PP-LIL-018-A
    public string Name { get; set; } = "";          // "Kamuzu Highway North"
    public int StationId { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Device                  // a tablet paired to a station
{
    public int Id { get; set; }
    public string Serial { get; set; } = "";              // tablet serial
    public string Imei { get; set; } = "";
    public int StationId { get; set; }                    // home station
    public int? AssignedOfficerId { get; set; }           // current bearer
    public string PairingToken { get; set; } = "";        // hashed
    public DateTime PairedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string AppVersion { get; set; } = "";
    public DateTime? LastSeenAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
}

public class SyncCursor              // per-node, per-entity high-water mark
{
    public int Id { get; set; }
    public string NodeId { get; set; } = "";              // "STN-LIL-001" or "DEV-7821"
    public string EntityType { get; set; } = "";         // "OffenceCode", "Officer", "Fine"
    public string Direction { get; set; } = "";          // "DOWN" or "UP"
    public DateTime LastSyncedAt { get; set; }
    public string? LastVersionTag { get; set; }          // ETag / row-version
}
```

### Modified

```csharp
public class Officer
{
    // existing fields …
    public int? StationId { get; set; }              // home station
    public int? PrimaryPatrolPostId { get; set; }    // optional default beat
}

public class Fine
{
    // existing fields …
    public int? StationId { get; set; }              // station the fine was issued at
    public int? PatrolPostId { get; set; }           // post / checkpoint
    public int? IssuedFromDeviceId { get; set; }
    public DateTime CreatedAtLocalUtc { get; set; }  // device clock at issuance
    public DateTime? SyncedToStationAt { get; set; }
    public DateTime? SyncedToHqAt { get; set; }
    public string LocalReferenceNumber { get; set; } = ""; // device-scoped, e.g. DEV7821-000142
    // ReferenceNumber stays as the canonical one (assigned at HQ on sync, or pre-allocated range)
}

public class Payment
{
    // existing fields …
    public int? StationId { get; set; }
    public int? IssuedFromDeviceId { get; set; }
    public DateTime? SyncedToStationAt { get; set; }
    public DateTime? SyncedToHqAt { get; set; }
    public string LocalReceiptNumber { get; set; } = "";
}
```

---

## 4 · Reference numbering — collision-free across nodes

**Problem:** offline-issued fines need unique references with no central allocator.

**Solution:** each tier gets a prefix. Final canonical form is generated at HQ on sync.

| Tier | Format | Example |
|------|--------|---------|
| Device (offline) | `DEV{deviceId}-{seq}` | `DEV7821-000142` |
| Station (counter) | `STN{stationId}-{seq}` | `STN18-000007` |
| HQ (canonical) | `NXF-{year}-{seq6}` | `NXF-2026-088123` |

When a fine syncs up the chain, HQ assigns the canonical `NXF-…` and the device/station replaces its local prefix the next time it sees the record. Both numbers stay searchable forever (the local prefix is kept on the row, indexed).

Pre-allocated ranges (alternative): HQ hands every station a block of 10,000 NXF numbers and the station hands devices sub-blocks of 500. Cleaner audit but harder to operate. Hold on this until we have field-pilot feedback.

---

## 5 · Sync flow

### 5.1 — HQ → Station (down, reference data)

- **Trigger:** every 5 min over HTTPS, or HQ pushes on change via webhook
- **Mechanism:** signed JSON snapshot keyed by `SyncCursor.LastVersionTag`
- **Payload (scoped to station):**
  - The station's own Department + sister stations (read-only)
  - All PatrolPosts under this station
  - The station's officer roster
  - Full active OffenceCode catalogue (small, < 500 rows — ship complete)
  - Any RBAC role definitions
- **Validation:** mTLS + HMAC; station rejects payloads it can't verify
- **Audit:** sync event written locally and at HQ

### 5.2 — Station → Device (down, scoped reference)

- **Trigger:** start-of-shift handshake + every 2h on network detect
- **Scope filter:** officer's primary station + assigned PatrolPosts; only their own user record + RBAC role, not the full roster
- **Mechanism:** local Wi-Fi / 4G / Bluetooth sideload as fallback
- **Payload size budget:** ≤ 2 MB per sync (offence codes + scoped roster)

### 5.3 — Device → Station (up, operational)

- **Trigger:** end-of-shift handover + every 2h auto-sync when on station Wi-Fi
- **Mechanism:** device queues fines/payments locally (SQLite) and pushes the queue
- **Idempotency:** every record carries a client-generated UUID; replays are no-ops
- **Conflict policy:** server overwrites only if `UpdatedAt` is newer; otherwise queues a manual-review event for the supervisor

### 5.4 — Station → HQ (up, operational + audit)

- **Trigger:** every 5 min + immediate push for `PAYMENT_POSTED` (Treasury wants it fast)
- **Mechanism:** identical to 5.3 but at the next tier
- **Backpressure:** if HQ is down, station queues up to 7 days; if queue >7d, paging alarm

### 5.5 — Manual procedures (your shift bookends)

**Start of shift (officer at station, 5 min):**
1. Officer logs into device → device requests scoped sync from station server
2. Station validates officer is on duty roster for today, station, this device
3. Device receives reference snapshot + fresh JWT (12h expiry)
4. Officer confirms beat / patrol post; status flips to `Active`
5. Audit event: `SHIFT_STARTED`

**End of shift:**
1. Officer taps "End shift" → device pushes all queued fines, payments, status updates
2. Station server acknowledges per-record; device clears its queue
3. Officer's status flips to `Offline`
4. Device prints a shift summary (count of fines issued, MK collected, dispatch incidents)
5. Audit event: `SHIFT_ENDED`

**Auto background sync (every 2h while device has network):**
- Same as 5.3 but no UI prompt; runs as a foreground service so Android doesn't kill it
- If sync fails 3 times in a row → device shows a yellow bar; supervisor sees the stale device on the admin map

---

## 6 · Identity & pairing

- **Officer** authenticates with username + password → JWT (existing)
- **Device** is paired *once* at the station: supervisor logs into the station-server admin UI, scans the tablet's QR, mints a `PairingToken` (one-time, 10 min TTL)
- **Device-to-station auth** uses mTLS + the pairing token-derived keypair; revocable from station admin
- **Station-to-HQ** uses mTLS pinned to the cert published in `Station.StationServerPublicKey`
- **Multi-station officers** (rotation): supervisor at the new station pairs the device fresh; the old pairing is revoked. One device, one active pairing at a time.

---

## 7 · Hardware additions (to the procurement list)

### Per station (one Station Server)
- **Box:** Intel NUC 13 Pro or fanless Lenovo ThinkCentre M70q (8-core / 32 GB RAM / 1 TB NVMe). MWK ~ 1.4M each
- **UPS:** APC Smart-UPS 1500VA, 30 min runtime (rural power is unreliable). MWK ~ 600k
- **Network:** dual NIC — LAN to station + 4G/LTE failover dongle (Airtel + TNM SIMs hot-swap). MWK ~ 250k incl. 12-month data
- **Backup:** external 2 TB SSD for nightly local snapshot. MWK ~ 180k
- **Optional:** 200W solar + 100Ah battery for Mchinji / Karonga / Nsanje sites where grid is < 12h/day. MWK ~ 1.8M

### Per officer device (already in scope — Module 4)
- Android 13+ tablet, NFC, 8MP rear camera, GPS, 64 GB+ storage
- 2× swappable batteries
- Rugged case + screen protector
- Pre-paid data SIM (small monthly bundle, ~500 MB)

### HQ
- Existing build (single API server) is fine for pilot; for national rollout add:
  - Hot standby DB (SQL Server Always On)
  - Reverse proxy with rate limiting (Caddy / Nginx)
  - Centralised log aggregation (Seq / Loki)

---

## 8 · Demo-day implications

For the Minister demo (7 days), we won't have hardware at every station. Plan:

1. Single laptop runs **HQ + one station + one officer device** as three separate processes / ports → narrate the topology while showing it work
2. The runbook adds a "kill HQ network" step → officer issues a fine on device, payments still post at station, all caught up when HQ comes back. Powerful narrative.
3. Pilot rollout (90 days post-demo) — start with **one** real station server (Area 18) + 4 officer devices. Validate the sync engine. Then expand.

---

## 11 · Counter lookup when HQ is offline (Q3 — applied)

To support citizen counter payments when HQ is unreachable, every station holds a **rolling cache of citizen-payable fine references**.

- **Hot scope (always held):** every active fine issued *by this station's officers* + every fine where the citizen's licensed address is in this station's jurisdiction
- **Warm scope (lookup index only):** a slim index of all national active fines (`reference, plate, amount, status, issuingStation`) so a citizen who got a fine in Lilongwe can pay at a Blantyre counter. Index size ~ 80 bytes/row × maybe 200k active fines = ~16 MB. Trivial to replicate.
- **Cold scope (HQ only):** full evidence — photos, dispute correspondence, payment history beyond 12 months — never replicated to stations.

**Privacy implication flagged:** the lookup index contains plate + name. We will gate this lookup behind the cashier's authenticated session at the counter; never exposed to the public network.

---

## 12 · Data retention policy (Q5 — recommended; pending MPS/MACRA confirmation)

**Sources consulted (best of available May-2025 knowledge):**
- Malawi Data Protection Act 2024 — personal data minimum-purpose retention
- AML/CFT Act 2017 — financial transaction records ≥ 7 years
- Penal Code, Evidence Act — court evidence retention rules
- ISO 27001 / PCI-DSS analogue — audit log retention 7+ years
- MACRA records-retention schedule (telecom-licensed providers, public)

**Recommended policy:**

| Data class | Station server | HQ |
|------------|----------------|-----|
| Active operational data (open fines, pending payments) | Hot, indefinite while open | Hot, indefinite while open |
| Closed fines + completed payments | 12 months rolling, then auto-purge | **7 years** (financial record) |
| Citizen-lookup index | 12 months rolling | Indefinite |
| Disputed / appealed fines | Until dispute resolved + 90 days at station | **10 years** post-resolution |
| Officer audit log (shift events, status changes) | 90 days at station | **Indefinite** at HQ |
| System security audit (logins, RBAC changes) | 90 days at station | **7 years** at HQ |
| Photo / NFC / GPS evidence | 12 months at station | Indefinite at HQ until fine closed + 7 years |
| Backup snapshots | Nightly local, 30-day rolling | Daily off-site, 7-year archive |

**Auto-purge job:** runs nightly at each station, hard-deletes rows older than the policy's station limit. Always preceded by a "still synced?" check against HQ — never delete data that hasn't been confirmed received.

**Action item for TJ:** before the pilot, get sign-off on this schedule from (a) MPS Records Officer, (b) MACRA Data Protection Commissioner, (c) Anti-Corruption Bureau if any objection on shorter station-side retention. Update this section with the agreed numbers.

---

## 13 · Sync alarm UX (Q6 — applied)

**Officer device (status pill, top-right of tablet UI):**
- 🟢 **Synced** — last successful station handshake < 2h ago
- 🟡 **Pending** — fines/payments queued; no network yet
- 🔴 **Sync failed** — N consecutive sync attempts failed (default N=2 days). Banner: "Contact your supervisor."

**Station server (admin sub-banner):**
- Counter clerk sees the same green/yellow/red pill for the station-to-HQ link
- Supervisor view shows per-device pills + last-seen-at timestamps

**Admin portal (HQ ops view):**
- New `/sync-health` page shows a table of every station + every device with last sync, queue depth, alarm state
- Audit log records every sync event with full payload size, transport, latency, outcome
- Email/SMS alarm to station IT contact when station has 2+ days no-sync

**End-user wording is intentionally non-technical.** "Failed" never elaborates on the cause to the field officer; that detail lives in the audit log for IT.

---

## 14 · Device pairing & officer login switch (Q2 + Q7 — applied)

**Pairing (one-time, by station supervisor):**
1. Supervisor logs into station server admin
2. Picks "Pair new device" → station prints a one-time QR with `PairingToken`
3. Tablet scans QR → derives keypair, registers with station, receives station mTLS cert
4. Tablet is now the property of the station (not of any one officer)

**Login switch (every shift change, on the tablet):**
1. Outgoing officer taps "End shift" → device pushes their queued records, clears their session
2. Incoming officer taps "Sign in", enters credentials → device validates against station roster
3. New officer's scoped reference data loads (their assigned PatrolPosts only)
4. Audit events: `SHIFT_ENDED` for outgoing, `SHIFT_STARTED` for incoming

**Lost / stolen device (Q7 = 72h):**
- Device sends a heartbeat every 15 min (lightweight, fits in any spare bandwidth)
- After **72h** of no heartbeat, station server auto-revokes the device's mTLS cert; HQ is notified
- Any queued records on a revoked device cannot be pushed; supervisor must physically retrieve the device, re-pair, and re-sync from the local SQLite (forensically, if needed)
- During the 72h grace period, the device works normally — this is so a tablet that's just had a flat battery for two days isn't bricked

---

## 9 · Open questions — RESOLVED

All seven Q1–Q7 questions answered above (see TJ decisions log + §11–§14). Remaining clarifications below.

### 9b · Follow-on clarifications — RESOLVED (2026-04-27)

| # | Question | Decision |
|---|----------|----------|
| 8 | Counter cash receipt format | **Distinct.** Counter cash receipts are `MPAY-STN{stationId}-{seq}` (e.g. `MPAY-STN18-000007`) so they are immediately distinguishable from a fine reference `STN18-000007`. |
| 9 | Officer credential reset | **Always HQ IT helpdesk.** No offline / station-side password reset path. Reduces attack surface and gives one audit trail. Helpdesk SLA: 15 min during business hours, 4 h out-of-hours. |
| 10 | Bad OffenceCode push rollback | **Versioned reference snapshots; supervisor pin.** Every push from HQ creates an immutable snapshot version. Stations keep the last 3. A station supervisor can pin a previous version (with reason logged); HQ ops sees the pin and decides whether to fix-and-re-push or accept the deviation. |

---

## 10 · Build plan (proposed)

If this sketch lands, the modules shift:

- **Module 3 (admin) — extend:** add HQ-side CRUD pages for Station, PatrolPost, Officer registration, OffenceCode editor. Already covered by the HQ-as-master rule.
- **Module 4 (was: officer MAUI):** rename to **"Officer device + Station server + sync engine"**. Build station server first as a slimmed-down ASP.NET Core service that mirrors the HQ data model + sync endpoints. Then the MAUI tablet talks to the station server.
- **New Module 4.5: Sync engine** — the actual replication code, both directions, with idempotency, retries, conflict resolution, audit.
- **Module 5 (WhatsApp/USSD):** unchanged.
- **Module 6 (deploy):** add station-server deployment scripts (PowerShell + Bash) for the NUCs.

---

*End of draft. Comment on the open questions or push back on any assumption — we revise before any code touches main.*
