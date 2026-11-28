#!/usr/bin/env bash
# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
set -euo pipefail
umask 022

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_DIR="$(cd -- "$SCRIPT_DIR/.." && pwd)"
CRATE_DIR="$REPO_DIR/watercooling-daemon"
SESSION_CRATE_DIR="$REPO_DIR/session-daemon"
SYSTEM_CRATE_DIR="$REPO_DIR/system-daemon"
CLI_CRATE_DIR="$REPO_DIR/iteglow-cli"
UI_DIR="$REPO_DIR/ui-tauri"
ICON_SVG="${ICON_SVG:-$REPO_DIR/iteglow/data/mechrevo-control.svg}"
RUNTIME_BIN="${RUNTIME_BIN:-$SCRIPT_DIR/cache/runtime-x86_64}"

APPIMAGE_RUNTIME_TAG="${APPIMAGE_RUNTIME_TAG:-20251108}"
APPIMAGE_RUNTIME_URL="${APPIMAGE_RUNTIME_URL:-https://github.com/AppImage/type2-runtime/releases/download/${APPIMAGE_RUNTIME_TAG}/runtime-x86_64}"
APPIMAGE_RUNTIME_SHA256="${APPIMAGE_RUNTIME_SHA256:-2fca8b443c92510f1483a883f60061ad09b46b978b2631c807cd873a47ec260d}"

STRICT_ALL="${STRICT_ALL:-0}"
BUILD_SYSTEM_BUNDLE="${BUILD_SYSTEM_BUNDLE:-0}"

PKG_NAME="iteglow-control"
VERSION="$(cd "$REPO_DIR" && python3 .github/scripts/prepare-release.py --check)"
ASSET_VERSION="${VERSION/+/_}"
# Package managers sort prereleases before the final version using "~".
NATIVE_VERSION="$(sed -E 's/^([0-9]+\.[0-9]+\.[0-9]+)-/\1~/; s/-/./g' <<< "$VERSION")"
ARCH_DEB="amd64"
ARCH_RPM="x86_64"
DIST_DIR="${DIST_DIR:-$REPO_DIR/dist}"
CARGO_TARGET_DIR="${CARGO_TARGET_DIR:-${XDG_CACHE_HOME:-$HOME/.cache}/iteglow-watercoolingd-target}"
UI_CARGO_TARGET_DIR="${UI_CARGO_TARGET_DIR:-${XDG_CACHE_HOME:-$HOME/.cache}/mechrevo-tauri-target}"
export CARGO_TARGET_DIR

WORK_DIR="$(mktemp -d "${TMPDIR:-/tmp}/iteglow-pkg.XXXXXX")"
trap 'rm -rf "$WORK_DIR"' EXIT

if [[ ! -f "$ICON_SVG" ]]; then
  echo "error: ICON_SVG not found: $ICON_SVG" >&2
  exit 1
fi

echo "==> Building Rust release binaries ($PKG_NAME $VERSION)"
(cd "$CRATE_DIR" && cargo build --release --locked)
(cd "$SESSION_CRATE_DIR" && cargo build --release --locked)
(cd "$CLI_CRATE_DIR" && cargo build --release --locked)
if [[ "$BUILD_SYSTEM_BUNDLE" == "1" ]]; then
  echo "==> Building Rust root system daemon & helper binaries (system-daemon)"
  (cd "$SYSTEM_CRATE_DIR" && cargo build --release --locked)
fi
echo "==> Building Tauri 2 desktop GUI (mechrevo-control-gui)"
(cd "$UI_DIR" && pnpm install --frozen-lockfile && pnpm build)
(cd "$UI_DIR/src-tauri" && CARGO_TARGET_DIR="$UI_CARGO_TARGET_DIR" cargo build --release --locked)

BIN_PATH="$CARGO_TARGET_DIR/release/iteglow-watercoolingd"
SESSION_BIN_PATH="$CARGO_TARGET_DIR/release/iteglow-sessiond"
CLI_BIN_PATH="$CARGO_TARGET_DIR/release/iteglow"
UI_BIN_PATH="$UI_CARGO_TARGET_DIR/release/mechrevo-control-gui"

mkdir -p "$DIST_DIR"
artifacts=()

# ----------------------------------------------------------------- Common tree
populate_root() {
  local root="$1"
  mkdir -p \
    "$root/usr/bin" \
    "$root/usr/libexec" \
    "$root/usr/lib/systemd/user" \
    "$root/usr/lib/udev/rules.d" \
    "$root/usr/share/applications" \
    "$root/usr/share/icons/hicolor/scalable/apps" \
    "$root/usr/share/doc/$PKG_NAME"

  install -m 0755 "$BIN_PATH" "$root/usr/libexec/iteglow-watercoolingd"
  ln -sf ../libexec/iteglow-watercoolingd "$root/usr/bin/iteglow-watercoolingd"
  install -m 0755 "$SESSION_BIN_PATH" "$root/usr/libexec/iteglow-sessiond"
  ln -sf ../libexec/iteglow-sessiond "$root/usr/bin/iteglow-sessiond"
  install -m 0755 "$CLI_BIN_PATH" "$root/usr/bin/iteglow"
  install -m 0755 "$UI_BIN_PATH" "$root/usr/bin/mechrevo-control-gui"

  cat > "$root/usr/lib/systemd/user/iteglow-watercooling.service" <<'EOF'
# User-session async Rust BLE water-cooling coordinator daemon (no sudo / no arbitrary EC writes).
[Unit]
Description=iteglow BLE water-cooling daemon (Rust)
PartOf=graphical-session.target
After=graphical-session.target

[Service]
Type=simple
WorkingDirectory=/
ExecStart=/usr/libexec/iteglow-watercoolingd
Restart=on-failure
RestartSec=3
TimeoutStopSec=10
MemoryLow=16M

[Install]
WantedBy=graphical-session.target
EOF
  chmod 0644 "$root/usr/lib/systemd/user/iteglow-watercooling.service"

  cat > "$root/usr/lib/systemd/user/iteglow-session-restore.service" <<'EOF'
# Restores only the active graphical user session's last successfully applied lights (async Rust daemon).
[Unit]
Description=Restore saved lighting in the active graphical session (Rust)
PartOf=graphical-session.target
After=graphical-session.target

[Service]
Type=simple
WorkingDirectory=/
ExecStart=/usr/libexec/iteglow-sessiond --apply
Restart=on-failure
RestartSec=5
TimeoutStopSec=7
MemoryLow=8M

[Install]
WantedBy=graphical-session.target
EOF
  chmod 0644 "$root/usr/lib/systemd/user/iteglow-session-restore.service"

  cat > "$root/usr/lib/udev/rules.d/60-iteglow.rules" <<'EOF'
# Grant logged-in desktop user access to ITE 8291/8233 keyboard & lightbar hidraw nodes
KERNEL=="hidraw*", SUBSYSTEM=="hidraw", ATTRS{idVendor}=="048d", ATTRS{idProduct}=="600b", TAG+="uaccess"
KERNEL=="hidraw*", SUBSYSTEM=="hidraw", ATTRS{idVendor}=="048d", ATTRS{idProduct}=="7001", TAG+="uaccess"
EOF
  chmod 0644 "$root/usr/lib/udev/rules.d/60-iteglow.rules"

  install -m 0644 "$REPO_DIR/system/81-bt-ax211-no-autosuspend.rules" \
    "$root/usr/lib/udev/rules.d/81-bt-ax211-no-autosuspend.rules"

  cat > "$root/usr/share/applications/iteglow.desktop" <<'EOF'
[Desktop Entry]
Type=Application
Name=机械革命控制中心
Comment=调节键盘灯、机身灯带、后盖 Logo、蓝牙水冷、性能档位与后台服务
Exec=/usr/bin/mechrevo-control-gui
Icon=mechrevo-control
Terminal=false
Categories=Settings;HardwareSettings;
Keywords=MECHREVO;RGB;水冷;键盘;灯带;性能;风扇;服务;
StartupWMClass=mechrevo-control-gui
EOF
  chmod 0644 "$root/usr/share/applications/iteglow.desktop"

  install -m 0644 "$ICON_SVG" "$root/usr/share/icons/hicolor/scalable/apps/mechrevo-control.svg"
  install -m 0644 "$REPO_DIR/system/bluez-le-timeout.conf.example" \
    "$root/usr/share/doc/$PKG_NAME/bluez-le-timeout.conf.example"
  install -m 0644 "$REPO_DIR/LICENSE" "$root/usr/share/doc/$PKG_NAME/LICENSE"
  install -m 0644 "$REPO_DIR/LICENSE-ADDITIONAL_TERMS-zh-CN.md" \
    "$root/usr/share/doc/$PKG_NAME/LICENSE-ADDITIONAL_TERMS-zh-CN.md"
  install -m 0644 "$REPO_DIR/LICENSE-ADDITIONAL_TERMS-en.md" \
    "$root/usr/share/doc/$PKG_NAME/LICENSE-ADDITIONAL_TERMS-en.md"
  install -m 0644 "$REPO_DIR/iteglow/LICENSE" "$root/usr/share/doc/$PKG_NAME/LICENSE-iteglow-upstream-MIT"
}

# ------------------------------------------------------------------- 1. .deb
if command -v dpkg-deb >/dev/null 2>&1; then
  echo "==> Building .deb package"
  DEB_ROOT="$WORK_DIR/deb-root"
  mkdir -p "$DEB_ROOT/DEBIAN"
  populate_root "$DEB_ROOT"
  INSTALLED_KB="$(du -sk "$DEB_ROOT/usr" | awk '{print $1}')"

  cat > "$DEB_ROOT/DEBIAN/control" <<EOF
Package: $PKG_NAME
Version: $NATIVE_VERSION
Section: utils
Priority: optional
Architecture: $ARCH_DEB
Installed-Size: $INSTALLED_KB
Maintainer: CyberNova <295488275+CyberNova2333@users.noreply.github.com>
Homepage: https://github.com/CyberNova2333/mechrevo-linux-control
Depends: libc6, bluez, libwebkit2gtk-4.1-0, libgtk-3-0t64 | libgtk-3-0, libdbus-1-3, pkexec | policykit-1
Description: MECHREVO Linux Control Center (Zero-Python Rust + Tauri 2)
 Native Tauri 2 + Vue 3 desktop control center (mechrevo-control-gui), native
 Rust CLI (iteglow), and lightweight async Rust daemons (iteglow-watercoolingd,
 iteglow-sessiond) for MECHREVO / Tongfang laptops with ITE 8291/8233
 RGB and LCT22002/LCT21001 external Bluetooth liquid coolers.
EOF

  cat > "$DEB_ROOT/DEBIAN/postinst" <<'EOF'
#!/bin/sh
set -e
if [ "$1" = "configure" ] && command -v udevadm >/dev/null 2>&1; then
  udevadm control --reload-rules >/dev/null 2>&1 || true
  udevadm trigger --subsystem-match=hidraw >/dev/null 2>&1 || true
fi
EOF
  chmod 0755 "$DEB_ROOT/DEBIAN/postinst"

  DEB_OUT="$DIST_DIR/${PKG_NAME}_${ASSET_VERSION}_${ARCH_DEB}.deb"
  dpkg-deb --root-owner-group --build "$DEB_ROOT" "$DEB_OUT"
  artifacts+=("${DEB_OUT##*/}")
elif [[ "$STRICT_ALL" == "1" ]]; then
  echo "error: dpkg-deb not found (STRICT_ALL=1)" >&2
  exit 1
else
  echo "warning: dpkg-deb not found; skipping .deb build" >&2
fi

# ------------------------------------------------------------------- 2. .rpm
if command -v rpmbuild >/dev/null 2>&1; then
  echo "==> Building .rpm package"
  RPM_TOP="$WORK_DIR/rpmbuild"
  mkdir -p "$RPM_TOP"/{BUILD,RPMS,SOURCES,SPECS,SRPMS}
  STAGE_ROOT="$WORK_DIR/rpm-stage"
  populate_root "$STAGE_ROOT"

  cat > "$RPM_TOP/SPECS/${PKG_NAME}.spec" <<EOF
%global debug_package %{nil}
%define _build_id_links none

Name:           $PKG_NAME
Version:        $NATIVE_VERSION
Release:        1
Summary:        MECHREVO Linux Control Center and async Rust BLE water-cooling daemon
License:        AGPL-3.0-only
URL:            https://github.com/CyberNova2333/mechrevo-linux-control
Requires:       bluez, webkit2gtk4.1, gtk3, dbus-libs, polkit

%description
Native Tauri 2 + Vue 3 desktop control center (mechrevo-control-gui), native
Rust CLI (iteglow), and lightweight async Rust daemons (iteglow-watercoolingd,
iteglow-sessiond) for MECHREVO / Tongfang laptops with ITE 8291/8233
RGB and LCT22002/LCT21001 Bluetooth liquid coolers.

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}
cp -a "$STAGE_ROOT/usr" %{buildroot}/

%post
if command -v udevadm >/dev/null 2>&1; then
  udevadm control --reload-rules >/dev/null 2>&1 || true
  udevadm trigger --subsystem-match=hidraw >/dev/null 2>&1 || true
fi

%files
%defattr(-,root,root,-)
/usr/bin/iteglow
/usr/bin/mechrevo-control-gui
/usr/bin/iteglow-watercoolingd
/usr/bin/iteglow-sessiond
/usr/libexec/iteglow-watercoolingd
/usr/libexec/iteglow-sessiond
/usr/lib/systemd/user/iteglow-watercooling.service
/usr/lib/systemd/user/iteglow-session-restore.service
/usr/lib/udev/rules.d/60-iteglow.rules
/usr/lib/udev/rules.d/81-bt-ax211-no-autosuspend.rules
/usr/share/applications/iteglow.desktop
/usr/share/icons/hicolor/scalable/apps/mechrevo-control.svg
/usr/share/doc/$PKG_NAME
EOF

  rpmbuild -bb --define "_topdir $RPM_TOP" --target "$ARCH_RPM" "$RPM_TOP/SPECS/${PKG_NAME}.spec"
  RPM_BUILT="$(find "$RPM_TOP/RPMS" -name '*.rpm' | head -n 1)"
  RPM_OUT="$DIST_DIR/${PKG_NAME}-${ASSET_VERSION}-1.${ARCH_RPM}.rpm"
  cp -f "$RPM_BUILT" "$RPM_OUT"
  artifacts+=("${RPM_OUT##*/}")
elif [[ "$STRICT_ALL" == "1" ]]; then
  echo "error: rpmbuild not found (STRICT_ALL=1)" >&2
  exit 1
else
  echo "warning: rpmbuild not found; skipping .rpm build" >&2
fi

# ---------------------------------------------------------------- 3. AppImage
verify_runtime_sha256() {
  local target="$1"
  local actual
  actual="$(sha256sum "$target" | awk '{print $1}')"
  [[ "$actual" == "$APPIMAGE_RUNTIME_SHA256" ]]
}

if [[ "${FETCH_APPIMAGE_RUNTIME:-0}" == "1" ]]; then
  if [[ ! -x "$RUNTIME_BIN" ]] || ! verify_runtime_sha256 "$RUNTIME_BIN"; then
    echo "==> Fetching pinned AppImage type2-runtime ($APPIMAGE_RUNTIME_TAG)"
    mkdir -p "$(dirname "$RUNTIME_BIN")"
    RUNTIME_TMP="$WORK_DIR/runtime-x86_64.tmp"
    curl -fsSL --retry 3 -o "$RUNTIME_TMP" "$APPIMAGE_RUNTIME_URL"
    if ! verify_runtime_sha256 "$RUNTIME_TMP"; then
      echo "error: downloaded AppImage runtime SHA-256 mismatch!" >&2
      echo "  expected: $APPIMAGE_RUNTIME_SHA256" >&2
      echo "  actual:   $(sha256sum "$RUNTIME_TMP" | awk '{print $1}')" >&2
      exit 1
    fi
    chmod 0755 "$RUNTIME_TMP"
    mv -f "$RUNTIME_TMP" "$RUNTIME_BIN"
  fi
fi

if [[ -x "$RUNTIME_BIN" ]] && command -v mksquashfs >/dev/null 2>&1; then
  if ! verify_runtime_sha256 "$RUNTIME_BIN"; then
    echo "error: cached AppImage runtime ($RUNTIME_BIN) failed SHA-256 verification." >&2
    echo "  expected: $APPIMAGE_RUNTIME_SHA256" >&2
    echo "  hint: rerun with FETCH_APPIMAGE_RUNTIME=1 to fetch pinned tag $APPIMAGE_RUNTIME_TAG" >&2
    exit 1
  fi
  echo "==> Building Type-2 .AppImage package"
  APPDIR="$WORK_DIR/${PKG_NAME}.AppDir"
  populate_root "$APPDIR"

  cp "$APPDIR/usr/share/applications/iteglow.desktop" "$APPDIR/iteglow.desktop"
  sed -i 's|^Exec=.*|Exec=mechrevo-control-gui|' "$APPDIR/iteglow.desktop"
  cp "$ICON_SVG" "$APPDIR/mechrevo-control.svg"
  ln -sf mechrevo-control.svg "$APPDIR/.DirIcon"

  cat > "$APPDIR/AppRun" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
HERE="$(dirname "$(readlink -f "$0")")"
export XDG_DATA_DIRS="$HERE/usr/share:${XDG_DATA_DIRS:-/usr/local/share:/usr/share}"

case "${1:-}" in
  daemon|--daemon|--cooler-daemon)
    shift || true
    exec "$HERE/usr/libexec/iteglow-watercoolingd" "$@"
    ;;
  session|--session|--session-daemon)
    shift || true
    exec "$HERE/usr/libexec/iteglow-sessiond" "$@"
    ;;
  "")
    exec "$HERE/usr/bin/mechrevo-control-gui"
    ;;
  gui)
    shift || true
    exec "$HERE/usr/bin/mechrevo-control-gui" "$@"
    ;;
  *)
    exec "$HERE/usr/bin/iteglow" "$@"
    ;;
esac
EOF
  chmod 0755 "$APPDIR/AppRun"

  SQUASH_IMG="$WORK_DIR/${PKG_NAME}.squashfs"
  mksquashfs "$APPDIR" "$SQUASH_IMG" -root-owned -noappend -comp zstd -Xcompression-level 15 -quiet
  APPIMAGE_OUT="$DIST_DIR/${PKG_NAME}-${ASSET_VERSION}-${ARCH_RPM}.AppImage"
  cat "$RUNTIME_BIN" "$SQUASH_IMG" > "$APPIMAGE_OUT"
  chmod 0755 "$APPIMAGE_OUT"
  artifacts+=("${APPIMAGE_OUT##*/}")
elif [[ "$STRICT_ALL" == "1" ]]; then
  echo "error: AppImage runtime ($RUNTIME_BIN) or mksquashfs missing (STRICT_ALL=1)" >&2
  exit 1
else
  echo "warning: runtime-x86_64 ($RUNTIME_BIN) or mksquashfs missing; set FETCH_APPIMAGE_RUNTIME=1 to download pinned runtime ($APPIMAGE_RUNTIME_TAG)" >&2
fi

# ------------------------------------------------ 4. Optional system-daemon tarball
if [[ "$BUILD_SYSTEM_BUNDLE" == "1" ]]; then
  echo "==> Packing root system-daemon companion archive"
  SYS_BUNDLE_NAME="mechrevo-system-daemon-${ASSET_VERSION}-linux-${ARCH_RPM}"
  SYS_STAGE="$WORK_DIR/$SYS_BUNDLE_NAME"
  mkdir -p "$SYS_STAGE/libexec" "$SYS_STAGE/systemd" "$SYS_STAGE/polkit" "$SYS_STAGE/licenses"

  for bin in \
    mechrevo-turbod \
    mechrevo-profile-guardd \
    mechrevo-profile-helper \
    mechrevo-acpi-check \
    mechrevo-powerd-sleep \
    mechrevo-turbo-helper \
    mechrevo-turbo-lifecycle \
    mechrevo-nvidia-hibernate-patch
  do
    install -m 0755 "$CARGO_TARGET_DIR/release/$bin" "$SYS_STAGE/libexec/$bin"
  done

  for unit in \
    mechrevo-turbo.service \
    mechrevo-profile-guard.service \
    mechrevo-acpi-baseline.service \
    mechrevo-profile-sleep.service \
    mechrevo-turbo-sleep.service \
    mechrevo-powerd-sleep.service
  do
    install -m 0644 "$REPO_DIR/system/$unit" "$SYS_STAGE/systemd/$unit"
  done

  install -m 0644 "$REPO_DIR/system/org.cybernova.mechrevo.profile.policy" \
    "$SYS_STAGE/polkit/org.cybernova.mechrevo.profile.policy"
  install -m 0644 "$REPO_DIR/LICENSE" "$SYS_STAGE/licenses/LICENSE"
  install -m 0644 "$REPO_DIR/LICENSE-ADDITIONAL_TERMS-zh-CN.md" \
    "$SYS_STAGE/licenses/LICENSE-ADDITIONAL_TERMS-zh-CN.md"
  install -m 0644 "$REPO_DIR/LICENSE-ADDITIONAL_TERMS-en.md" \
    "$SYS_STAGE/licenses/LICENSE-ADDITIONAL_TERMS-en.md"

  tar -C "$WORK_DIR" -czf "$DIST_DIR/${SYS_BUNDLE_NAME}.tar.gz" "$SYS_BUNDLE_NAME"
  artifacts+=("${SYS_BUNDLE_NAME}.tar.gz")
fi

if (( ${#artifacts[@]} == 0 )); then
  echo "error: no distribution artifacts were built" >&2
  exit 1
fi
(cd "$DIST_DIR" && sha256sum -- "${artifacts[@]}" > SHA256SUMS)

echo "==> Artifacts in $DIST_DIR:"
ls -lh "$DIST_DIR"
