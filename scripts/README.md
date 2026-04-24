# NexusFine scripts

All scripts come in two flavours:

- `.ps1` — run in PowerShell (`pwsh ./scripts/NAME.ps1`)
- `.sh`  — run in Git Bash (`./scripts/NAME.sh`, make executable first: `chmod +x scripts/*.sh`)

## Order

| # | Script | What it does |
|---|--------|--------------|
| — | `doctor.*` | Verify toolchain: .NET 8, MAUI workload, SQL Server / LocalDB, Node 20, Git, EF CLI |
| 0 | `module0.*` | Foundations: line-ending normalization, SmsService removal, build, commit, tag v0.0 |
| 1 | `module1.*` | API backend: seed DB, migrate, run tests, tag v0.1 (produced in Module 1) |
| 2 | `module2.*` | Citizen portal: produce /citizen build, tag v0.2 |
| 3 | `module3.*` | Admin Blazor build, tag v0.3 |
| 4 | `module4.*` | MAUI build + APK, tag v0.4 |
| 5 | `module5.*` | WhatsApp/USSD run + demo, tag v0.5 |
| 6 | `module6.*` | Azure publish, tag v0.6 |
| 7 | `module7.*` | E2E tests + demo runbook, tag v1.0 |

## Quick start (fresh machine)

```powershell
pwsh ./scripts/doctor.ps1
pwsh ./scripts/module0.ps1
```

or in Git Bash:

```bash
chmod +x ./scripts/*.sh
./scripts/doctor.sh
./scripts/module0.sh
```
