# Quarterly Tabletop Exercises

**Cadence:** Q1, Q2, Q3, Q4 — one scenario per quarter.
**Format:** 90 minutes. Facilitator (rotated), scribe (fixed), participants (TechNexus on-call rotation + MPS ICT liaison + Pilot Champion).
**Output:** a one-page lessons-learned note filed under `docs/operations/lessons/{date}-{scenario}.md`, plus any corrective actions logged as engineering issues.

These four scenarios cover the failure modes most likely to arrive at
NexusFine first. Run them in rotation; introduce new scenarios as live
incidents teach us new ones.

---

## Scenario 1 — HQ outage during business hours

### Setting
Friday 14:30 CAT. The Lilongwe Azure region is unreachable for the HQ API.
Five officer devices are on shift across Area 18 station. A citizen has just
attempted to pay a fine via WhatsApp.

### Inject sequence
- T+0  · The on-call engineer's pager fires: "HQ heartbeat lost, last seen 2 min ago."
- T+2  · Pilot Champion at Area 18 calls in: "WhatsApp says payment failed; the supervisor portal is blank."
- T+5  · A second station (Mzuzu, in roll-out phase) is also affected.
- T+12 · Treasury asks where the daily reconciliation file is.

### What we expect to see
- Station servers continue accepting counter cash and queueing operational
  records. Officer devices continue issuing fines into their local queue.
- The on-call engineer triggers Azure status check + opens an Azure
  support ticket within 5 minutes.
- The Account Lead sends an "issue acknowledged" message to MPS ICT
  within 10 minutes.
- Status page on `status.nexusfine.mw` updates within 15 minutes.
- Once HQ comes back, the station outbound queues drain in order; receipts
  appear in the admin Payments page with the correct timestamps.

### Questions for the team
1. Who picks up the on-call pager if the primary engineer is at lunch and not answering?
2. Do we have the Azure support phone number printed?
3. Who decides whether to invoke the Plan-B paper backup?
4. How do we confirm the queue drain completed correctly (no lost records)?
5. What goes in the SLA monthly report?

---

## Scenario 2 — Station Server failure (single site)

### Setting
Tuesday 09:15. Power has been out at Mchinji Border for 16 hours; the NUC
came back on this morning but the SQLite file is reporting "database is
locked" and the office Wi-Fi is up but unstable.

### Inject sequence
- T+0  · A patrol officer fails to start their shift — the tablet says
  "station unreachable." The officer phones the station commander.
- T+10 · The station commander tries the supervisor admin URL — connection
  refused.
- T+25 · Three more officers report the same. A bus has just been pulled
  over for overloading; the officer has to write a paper ticket.

### What we expect to see
- Tablets fall back to paper-ticket mode (officers physically hold a paper
  book; serial range was logged at deploy time, see site-survey checklist).
- Field engineer dispatched within 90 minutes; spare NUC available at
  Lilongwe HQ ready to ship.
- Recovery procedure executed: pull the SQLite file from station to HQ,
  inspect, repair OR replace. No data loss expected because every record
  was already replicated to HQ on the morning sync.

### Questions for the team
1. Does the station commander know the on-call procedure WITHOUT looking it up?
2. How long do paper tickets need to be retained before being archived into NexusFine after recovery?
3. Do we have the SQLite recovery commands documented and tested?
4. Is the spare NUC pre-imaged or does it need a fresh deploy?

---

## Scenario 3 — Officer device theft / loss

### Setting
Sunday 18:00. An officer reports their device stolen at a roadblock outside
Mzuzu CBD. The device was paired to STN-MZU-001 yesterday. The officer had
shift-started but had not yet issued any fines today.

### Inject sequence
- T+0  · The officer calls the station commander.
- T+30 · The commander dials the on-call engineer via WhatsApp.
- T+2h · The engineer needs to revoke the device certificate AND inform
  MPS ICT for the police case.

### What we expect to see
- The on-call engineer revokes the device via `POST /api/devices/{serial}/revoke` (or its admin-UI equivalent).
- The audit log records the revocation event with who/when/IP.
- A police case is opened by the station commander — TechNexus assists by
  providing the device's last-known GPS from the heartbeat history.
- The device is automatically blocked from posting to its station server
  (mTLS cert revoked).
- 72-hour auto-revoke would also have caught it on Wednesday; manual
  revocation accelerates the timeline.

### Questions for the team
1. Who is authorised to revoke a device?
2. Is the revocation endpoint protected against social-engineering?
3. What evidence do we hand to the police?
4. Replacement device — how fast can the station get a paired spare?
5. Does revocation cascade to in-flight queued records?

---

## Scenario 4 — Suspected supervisor-credential compromise

### Setting
Wednesday 11:00. The CISO function notices an anomalous pattern: the
supervisor account `super.lilongwe` logged in from an IP address in
Lagos 90 minutes ago. The supervisor in question is currently in a
station meeting in Lilongwe.

### Inject sequence
- T+0  · CISO alert: "anomalous login geography."
- T+5  · The supervisor confirms they did not log in from anywhere this morning.
- T+10 · The on-call engineer must decide whether to force a session lock,
  rotate credentials, or both.

### What we expect to see
- The compromised account is immediately disabled.
- All active sessions for that account are invalidated (revoke the JWT
  signing key, OR force a token-version bump on the user's row).
- Audit log queried for everything that account has done in the last 24
  hours; any unauthorised changes flagged and rolled back.
- New temporary credentials issued through HQ IT helpdesk (per the
  documented credential-reset flow).
- 2FA enforcement (S9, planned) would have stopped this entirely.

### Questions for the team
1. Do we have a "kill the JWT signing key" emergency procedure?
2. How quickly can we read the audit log for one user across all entities?
3. What do we tell the MPS Cybersecurity Unit, and when?
4. Is there a public statement required (Data Protection Act §28)?
5. How do we know it wasn't just a VPN exit node? Confidence threshold?

---

## Running the exercise

1. **Pick a date** at least a week in advance. Block 90 minutes on calendars.
2. **Pick a facilitator** who hasn't run the last one.
3. **Send the scenario** to participants the morning of the exercise. Don't
   send it earlier — we want to test reactions, not preparation.
4. **Run it live**. The facilitator reads injects out loud at the times
   listed. Participants act / answer in real time. The scribe captures
   actions and gaps.
5. **Debrief immediately** — 15 minutes at the end. What worked, what
   didn't, what's missing.
6. **Write up** — one-page lessons learned under `docs/operations/lessons/`.
   Open issues for any corrective actions.

A scenario that runs perfectly to plan is suspicious. We expect at least
two gaps per exercise. Closing them is the value of doing it.
