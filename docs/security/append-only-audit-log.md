# Design — Append-Only Audit Log

**Status:** Design v1.0 (not yet implemented in code)
**Owner:** Engineering · reviewed by CISO function
**Target wave:** Wave 2 — Module 7 hardening
**Decision basis:** D-009 (audit log strategy), S7 in `docs/department-recommendations.md`

This document records the design TechNexus will implement to make the
NexusFine audit log defensible in court and in front of the Auditor General.
The current `AuditLog` table is operationally adequate; the design below
raises it to evidence-grade.

---

## 1. Problem statement

Today the audit log is a regular EF Core entity in the same `AppDbContext`
as the operational tables. That has three weaknesses for forensic purposes:

1. **Same DB → same blast radius.** A SQL-injection or admin-compromise that
   can change an operational row could also tamper with the audit row that
   should prove the change.
2. **Mutable schema.** EF migrations could in principle update existing
   audit rows. No tamper-detection.
3. **No chain.** Each row is independent; a deletion or rewrite of an
   entire audit row would leave no trace.

For the demo this is acceptable. For the pilot and beyond — especially
once the Auditor General will be looking at these records — we need an
audit log that is **append-only**, **separately hosted**, and **chain-hashed**
so any retrospective edit is detectable.

---

## 2. Goals & non-goals

### Goals
- Each audit record can be cryptographically proven to have been written
  in the order claimed, with no edits since.
- The log lives in a database account that **cannot perform UPDATE or
  DELETE** on the audit table (DB-role enforcement, not just application
  code).
- An external auditor with read-only access can verify the chain in their
  own tooling (the hash schema is open).
- Replay of a payload from the chain reproduces the original event byte-for-byte.

### Non-goals
- Public-blockchain anchoring (overkill for pilot; revisit at national scale).
- Realtime streaming to an external SIEM (handled separately as part of
  the SOC 2 readiness work, S3).
- End-to-end encryption between the application and the audit DB. TLS in
  transit + at-rest encryption at the storage layer is sufficient.

---

## 3. High-level architecture

```
   [HQ API]  ──insert──►  [Audit Log DB]
                                │
                                │  read-only role
                                ▼
                  [Auditor read-replica / SIEM]
```

- The Audit Log DB is a **separate** SQL Server database (e.g.
  `NexusFineAudit`) on its own credentials. The HQ API connects to it
  with a dedicated low-privileged login.
- The application uses a **single stored procedure** to append. That sproc
  is the only object the writing role can execute. The role has no `TABLE`-
  level grants, no DDL, no SELECT on prior rows.
- Rows in `AuditEntry` carry a **link hash** to the previous row, so the
  log forms a chain. A retrospective edit breaks the chain from that row
  forward.

---

## 4. Schema

```sql
CREATE TABLE AuditEntry
(
    Id              BIGINT IDENTITY PRIMARY KEY,
    Timestamp       DATETIMEOFFSET   NOT NULL,
    Sequence        BIGINT           NOT NULL,          -- monotonic
    EntityType      VARCHAR(40)      NOT NULL,
    EntityId        BIGINT           NULL,
    Action          VARCHAR(40)      NOT NULL,
    UserId          VARCHAR(100)     NULL,
    IpAddress       VARCHAR(45)      NULL,
    UserAgent       VARCHAR(255)     NULL,
    Payload         NVARCHAR(MAX)    NULL,              -- JSON
    PrevHash        BINARY(32)       NULL,              -- SHA-256
    RowHash         BINARY(32)       NOT NULL,          -- SHA-256
    StationCode     VARCHAR(20)      NULL,
    SourceTier      VARCHAR(8)       NOT NULL,          -- HQ / Station / Device
    SchemaVersion   INT              NOT NULL DEFAULT 1
);

CREATE UNIQUE INDEX IX_AuditEntry_Sequence ON AuditEntry(Sequence);
CREATE INDEX IX_AuditEntry_EntityType_EntityId ON AuditEntry(EntityType, EntityId);
CREATE INDEX IX_AuditEntry_Timestamp ON AuditEntry(Timestamp);
```

`RowHash = SHA256( Sequence || Timestamp.ToUnixMilliseconds() || EntityType || EntityId || Action || UserId || Payload || PrevHash )`

The first row's `PrevHash` is 32 zero bytes.

### Stored procedure (the only write path)

```sql
CREATE PROCEDURE sp_AuditAppend
    @EntityType   VARCHAR(40),
    @EntityId     BIGINT       = NULL,
    @Action       VARCHAR(40),
    @UserId       VARCHAR(100) = NULL,
    @IpAddress    VARCHAR(45)  = NULL,
    @UserAgent    VARCHAR(255) = NULL,
    @Payload      NVARCHAR(MAX) = NULL,
    @StationCode  VARCHAR(20)  = NULL,
    @SourceTier   VARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Seq BIGINT, @PrevHash BINARY(32), @Ts DATETIMEOFFSET = SYSDATETIMEOFFSET();

    SELECT TOP 1 @Seq = Sequence + 1, @PrevHash = RowHash
    FROM AuditEntry ORDER BY Sequence DESC;

    IF (@Seq IS NULL) BEGIN SET @Seq = 1; SET @PrevHash = 0x0000...; END

    -- Compute row hash on-server so the application cannot precompute & forge
    DECLARE @Hash BINARY(32) = HASHBYTES('SHA2_256',
        CONCAT(@Seq, '|', DATEDIFF_BIG(MS,'1970-01-01',@Ts),'|',
               @EntityType,'|',ISNULL(@EntityId,0),'|',@Action,'|',
               ISNULL(@UserId,''),'|',ISNULL(@Payload,''),'|',
               CAST(@PrevHash AS VARCHAR(64))));

    INSERT AuditEntry (Timestamp, Sequence, EntityType, EntityId, Action,
                       UserId, IpAddress, UserAgent, Payload,
                       PrevHash, RowHash, StationCode, SourceTier)
    VALUES (@Ts, @Seq, @EntityType, @EntityId, @Action,
            @UserId, @IpAddress, @UserAgent, @Payload,
            @PrevHash, @Hash, @StationCode, @SourceTier);
END
```

### Role grants

```sql
CREATE ROLE audit_writer;
GRANT EXECUTE ON sp_AuditAppend TO audit_writer;
-- NO grants on TABLE AuditEntry to this role.

CREATE ROLE audit_reader;
GRANT SELECT ON AuditEntry TO audit_reader;
```

The HQ API's audit-write SQL login is in `audit_writer` only. The auditor's
SQL login (or read-replica) is in `audit_reader` only.

---

## 5. Verification routine

Auditors verify by walking the chain:

```sql
DECLARE @prev BINARY(32) = 0x0000...; -- 32 zero bytes
DECLARE @id BIGINT = 0, @bad BIGINT = NULL;
WHILE 1=1
BEGIN
    SELECT TOP 1 Id, Sequence, PrevHash, RowHash, ...
    FROM AuditEntry WHERE Id > @id ORDER BY Id;
    IF @@ROWCOUNT = 0 BREAK;

    IF PrevHash <> @prev BEGIN SET @bad = Id; BREAK; END
    -- recompute expected RowHash, compare; flag mismatch.

    SET @prev = RowHash;
END
SELECT @bad AS firstBrokenRowId;
```

The same routine ships as a CLI tool (`tools/audit-verify`) for off-server
verification.

---

## 6. Migration path

### Phase 1 — Add the new DB alongside the existing one
- Provision `NexusFineAudit` database.
- Run schema + sproc + roles.
- Add a second connection string `AuditDb` to API config.
- Add an `IAuditAppender` interface in `NexusFine.Infrastructure`.
- Two implementations: `LegacyAuditAppender` (writes to current
  `AppDbContext.AuditLogs`) and `ChainedAuditAppender` (calls
  `sp_AuditAppend`).
- Toggle by config flag `Audit:UseChainedLog`.

### Phase 2 — Run both in parallel for a fortnight
- `AuditLogMiddleware` calls both appenders.
- Compare row counts daily.

### Phase 3 — Cut over
- Switch the feature flag.
- Keep the legacy table read-only for 90 days; then archive.

### Phase 4 — Auditor on-boarding
- Provision Auditor General's office with `audit_reader` credentials on a
  read-replica.
- Train them on the verification routine.

---

## 7. Operational considerations

| Concern | Treatment |
|---------|-----------|
| Performance | `sp_AuditAppend` is single-row, single-index; benchmarked at >2,000 ops/sec on Azure SQL S2. NexusFine load is <50 ops/sec at national rollout. |
| Failure mode | If the audit DB is unreachable, the API **must fail the originating mutation** (no silent skips). A circuit-breaker pattern controls cascading failure. |
| Backfill / late writes | Late writes (from a station that comes back online after days offline) get a current timestamp + the original "occurred-at" stored in payload. |
| Schema evolution | `SchemaVersion` column on the row; verifier handles version bumps. |
| Retention | Audit is **never** purged at HQ. The 90-day station window applies only to local cached audit; HQ keeps forever. |

---

## 8. Open questions for the Auditor General

These should be discussed before Phase 1 starts:

1. Is SHA-256 acceptable, or does the office prefer a NIST FIPS 180-4 set?
2. Are there specific events the office wants tagged for accelerated review?
3. Does the office want the verifier tool to be a CLI, a web UI, or a SQL script?
4. What is the office's preferred export format for archive (SQL backup, CSV, JSON, signed bundle)?
5. Does the office want sample monthly reports auto-generated, or only on demand?

---

## 9. References

- `docs/DECISIONS.md` — D-009 (audit middleware strategy)
- `docs/security/threat-model.md` — STRIDE row for audit-log tampering
- `docs/compliance/dpia-malawi.md` — §5.7 (audit-log immutability)
- ISO 27001:2022 control 8.15 (Logging)
