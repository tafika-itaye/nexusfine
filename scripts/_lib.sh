#!/usr/bin/env bash
# NexusFine shared bash helpers.
# Source from module scripts: source "$(dirname "$0")/_lib.sh"

set -euo pipefail

c_cyan=$'\e[36m'; c_green=$'\e[32m'; c_yellow=$'\e[33m'; c_red=$'\e[31m'; c_dim=$'\e[2m'; c_off=$'\e[0m'

step()  { printf '\n%s==> %s%s\n' "$c_cyan"  "$1" "$c_off"; }
ok()    { printf '    %sOK%s  %s\n'            "$c_green"  "$c_off" "$1"; }
warn2() { printf '    %s!!%s  %s\n'            "$c_yellow" "$c_off" "$1"; }
fail()  { printf '    %sXX%s  %s\n'            "$c_red"    "$c_off" "$1"; exit 1; }

repo_root() {
    local r
    r="$(cd "$(dirname "$0")/.." && pwd)"
    [[ -f "$r/NexusFine.slnx" ]] || fail "NexusFine.slnx not found at $r"
    echo "$r"
}

have() { command -v "$1" >/dev/null 2>&1; }

run() {
    printf '    %s> %s%s\n' "$c_dim" "$*" "$c_off"
    "$@"
}
