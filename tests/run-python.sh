#!/usr/bin/env bash
# SPDX-License-Identifier: AGPL-3.0-only
set -euo pipefail

repo_dir="$(cd -- "$(dirname -- "$0")/.." && pwd)"
test_dir="$(mktemp -d)"
trap 'rm -rf -- "$test_dir"' EXIT
mkdir -m 700 "$test_dir"/{home,state,config,cache,runtime,data}
cd "$repo_dir"
env -u DBUS_SESSION_BUS_ADDRESS -u WAYLAND_DISPLAY \
  HOME="$test_dir/home" XDG_STATE_HOME="$test_dir/state" XDG_CONFIG_HOME="$test_dir/config" \
  XDG_CACHE_HOME="$test_dir/cache" XDG_DATA_HOME="$test_dir/data" XDG_RUNTIME_DIR="$test_dir/runtime" \
  PYTHONPATH="$repo_dir/iteglow/src" GDK_BACKEND=x11 GSK_RENDERER=cairo \
  xvfb-run -a /usr/bin/python3 -B -m unittest discover -s tests "$@"
