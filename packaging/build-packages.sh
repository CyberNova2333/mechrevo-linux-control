#!/usr/bin/env bash
# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
set -euo pipefail
umask 022

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
REPO_DIR="$(cd -- "$SCRIPT_DIR/.." && pwd)"
CRATE_DIR="$REPO_DIR/watercooling-daemon"
ITEGLOW_SRC="${ITEGLOW_SRC:-$REPO_DIR/iteglow/src/iteglow}"
ICON_SVG="${ICON_SVG:-$REPO_DIR/iteglow/data/mechrevo-control.svg}"
RUNTIME_BIN="${RUNTIME_BIN:-$SCRIPT_DIR/cache/runtime-x86_64}"

PKG_NAME="iteglow-control"
VERSION="$(sed -n 's/^version = "\([^"]*\)"/\1/p' "$CRATE_DIR/Cargo.toml" | head -n 1)"
ARCH_DEB="amd64"
ARCH_RPM="x86_64"
DIST_DIR="${DIST_DIR:-$REPO_DIR/dist}"
CARGO_TARGET_DIR="${CARGO_TARGET_DIR:-${XDG_CACHE_HOME:-$HOME/.cache}/iteglow-watercoolingd-target}"
export CARGO_TARGET_DIR

WORK_DIR="$(mktemp -d "${TMPDIR:-/tmp}/iteglow-pkg.XXXXXX")"
trap 'rm -rf "$WORK_DIR"' EXIT

if [[ ! -d "$ITEGLOW_SRC" ]]; then
  echo "error: ITEGLOW_SRC not found: $ITEGLOW_SRC" >&2
  exit 1
fi
if [[ ! -f "$ICON_SVG" ]]; then
  echo "error: ICON_SVG not found: $ICON_SVG" >&2
  exit 1
fi

echo "==> Building Rust release binary ($PKG_NAME $VERSION)"
(cd "$CRATE_DIR" && cargo build --release)
BIN_PATH="$CARGO_TARGET_DIR/release/iteglow-watercoolingd"

mkdir -p "$DIST_DIR"

# ----------------------------------------------------------------- Common tree
populate_root() {
  local root="$1"
  mkdir -p \
    "$root/usr/bin" \
    "$root/usr/libexec" \
    "$root/usr/lib/$PKG_NAME" \
    "$root/usr/lib/systemd/user" \
    "$root/usr/lib/udev/rules.d" \
    "$root/usr/share/applications" \
    "$root/usr/share/icons/hicolor/scalable/apps" \
    "$root/usr/share/doc/$PKG_NAME"

  install -m 0755 "$BIN_PATH" "$root/usr/libexec/iteglow-watercoolingd"
  ln -sf ../libexec/iteglow-watercoolingd "$root/usr/bin/iteglow-watercoolingd"

  cp -r "$ITEGLOW_SRC" "$root/usr/lib/$PKG_NAME/iteglow"
  find "$root/usr/lib/$PKG_NAME/iteglow" -type d -name '__pycache__' -exec rm -rf {} +
  find "$root/usr/lib/$PKG_NAME/iteglow" -type f -name '*.pyc' -delete
  find "$root/usr/lib/$PKG_NAME/iteglow" -type d -exec chmod 0755 {} +
  find "$root/usr/lib/$PKG_NAME/iteglow" -type f -exec chmod 0644 {} +

  cat > "$root/usr/bin/iteglow" <<EOF
#!/usr/bin/env bash
export PYTHONPATH="/usr/lib/$PKG_NAME\${PYTHONPATH:+:\$PYTHONPATH}"
exec /usr/bin/python3 -m iteglow.cli "\$@"
EOF
  chmod 0755 "$root/usr/bin/iteglow"

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

  cat > "$root/usr/lib/systemd/user/iteglow-session-restore.service" <<EOF
# Restores only the active graphical user session's last successfully applied lights.
[Unit]
Description=Restore saved lighting in the active graphical session
PartOf=graphical-session.target
After=graphical-session.target

[Service]
Type=simple
WorkingDirectory=/
Environment=PYTHONPATH=/usr/lib/$PKG_NAME
ExecStart=/usr/bin/python3 -m iteglow.lighting_session --apply
Restart=on-failure
RestartSec=5
TimeoutStopSec=7

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
Comment=调节键盘灯、机身灯带、蓝牙水冷与性能档位
Exec=/usr/bin/iteglow gui
Icon=mechrevo-control
Terminal=false
Categories=Settings;HardwareSettings;
Keywords=MECHREVO;RGB;水冷;键盘;灯带;性能;风扇;
StartupWMClass=mechrevo-control
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
Version: $VERSION
Section: utils
Priority: optional
Architecture: $ARCH_DEB
Installed-Size: $INSTALLED_KB
Maintainer: CyberNova <295488275+CyberNova2333@users.noreply.github.com>
Homepage: https://github.com/CyberNova2333/mechrevo-linux-control
Depends: libc6, python3 (>= 3.10), bluez
Recommends: python3-gi, gir1.2-gtk-4.0
Description: MECHREVO Linux Control Center and async Rust BLE water-cooling daemon
 Desktop control center (CLI + GTK4 GUI) and lightweight async Rust
 BLE water-cooling coordinator (iteglow-watercoolingd) for MECHREVO /
 Tongfang laptops with ITE 8291/8233 RGB and LCT22002/LCT21001 external
 Bluetooth liquid coolers.
EOF

  cat > "$DEB_ROOT/DEBIAN/postinst" <<'EOF'
#!/bin/sh
set -e
if [ "$1" = "configure" ] && command -v udevadm >/dev/null 2>&1; then
  udevadm control --reload-rules >/dev/null 2>&1 || true
fi
EOF
  chmod 0755 "$DEB_ROOT/DEBIAN/postinst"

  DEB_OUT="$DIST_DIR/${PKG_NAME}_${VERSION}_${ARCH_DEB}.deb"
  dpkg-deb --root-owner-group --build "$DEB_ROOT" "$DEB_OUT"
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
Version:        $VERSION
Release:        1
Summary:        MECHREVO Linux Control Center and async Rust BLE water-cooling daemon
License:        AGPL-3.0-only
URL:            https://github.com/CyberNova2333/mechrevo-linux-control
Requires:       python3 >= 3.10, bluez
Recommends:     python3-gobject, gtk4

%description
Desktop control center (CLI + GTK4 GUI) and lightweight async Rust BLE
water-cooling daemon (iteglow-watercoolingd) for MECHREVO / Tongfang laptops
with ITE 8291/8233 RGB and LCT22002/LCT21001 Bluetooth liquid coolers.

%install
rm -rf %{buildroot}
mkdir -p %{buildroot}
cp -a "$STAGE_ROOT/usr" %{buildroot}/

%post
if command -v udevadm >/dev/null 2>&1; then
  udevadm control --reload-rules >/dev/null 2>&1 || true
fi

%files
%defattr(-,root,root,-)
/usr/bin/iteglow
/usr/bin/iteglow-watercoolingd
/usr/libexec/iteglow-watercoolingd
/usr/lib/$PKG_NAME
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
  RPM_OUT="$DIST_DIR/${PKG_NAME}-${VERSION}-1.${ARCH_RPM}.rpm"
  cp -f "$RPM_BUILT" "$RPM_OUT"
else
  echo "warning: rpmbuild not found; skipping .rpm build" >&2
fi

# ---------------------------------------------------------------- 3. AppImage
if [[ ! -x "$RUNTIME_BIN" ]] && [[ "${FETCH_APPIMAGE_RUNTIME:-0}" == "1" ]]; then
  mkdir -p "$(dirname "$RUNTIME_BIN")"
  curl -fsSL --retry 3 -o "$RUNTIME_BIN" \
    "https://github.com/AppImage/type2-runtime/releases/download/continuous/runtime-x86_64"
  chmod +x "$RUNTIME_BIN"
fi

if [[ -x "$RUNTIME_BIN" ]] && command -v mksquashfs >/dev/null 2>&1; then
  echo "==> Building Type-2 .AppImage package"
  APPDIR="$WORK_DIR/${PKG_NAME}.AppDir"
  populate_root "$APPDIR"

  cp "$APPDIR/usr/share/applications/iteglow.desktop" "$APPDIR/iteglow.desktop"
  sed -i 's|^Exec=.*|Exec=iteglow gui|' "$APPDIR/iteglow.desktop"
  cp "$ICON_SVG" "$APPDIR/mechrevo-control.svg"
  ln -sf mechrevo-control.svg "$APPDIR/.DirIcon"

  cat > "$APPDIR/AppRun" <<EOF
#!/usr/bin/env bash
set -euo pipefail
HERE="\$(dirname "\$(readlink -f "\$0")")"
export PYTHONPATH="\$HERE/usr/lib/$PKG_NAME\${PYTHONPATH:+:\$PYTHONPATH}"
export XDG_DATA_DIRS="\$HERE/usr/share:\${XDG_DATA_DIRS:-/usr/local/share:/usr/share}"

case "\${1:-}" in
  daemon|--daemon|--cooler-daemon)
    shift || true
    exec "\$HERE/usr/libexec/iteglow-watercoolingd" "\$@"
    ;;
  "")
    exec /usr/bin/python3 -m iteglow.cli gui
    ;;
  *)
    exec /usr/bin/python3 -m iteglow.cli "\$@"
    ;;
esac
EOF
  chmod 0755 "$APPDIR/AppRun"

  SQUASH_IMG="$WORK_DIR/${PKG_NAME}.squashfs"
  mksquashfs "$APPDIR" "$SQUASH_IMG" -root-owned -noappend -comp zstd -Xcompression-level 15 -quiet
  APPIMAGE_OUT="$DIST_DIR/${PKG_NAME}-${VERSION}-${ARCH_RPM}.AppImage"
  cat "$RUNTIME_BIN" "$SQUASH_IMG" > "$APPIMAGE_OUT"
  chmod 0755 "$APPIMAGE_OUT"
else
  echo "warning: runtime-x86_64 ($RUNTIME_BIN) or mksquashfs missing; set FETCH_APPIMAGE_RUNTIME=1 to download runtime" >&2
fi

(
  cd "$DIST_DIR"
  rm -f SHA256SUMS
  for art in "${PKG_NAME}_${VERSION}_${ARCH_DEB}.deb" "${PKG_NAME}-${VERSION}-1.${ARCH_RPM}.rpm" "${PKG_NAME}-${VERSION}-${ARCH_RPM}.AppImage"; do
    [[ -f "$art" ]] && sha256sum "$art" >> SHA256SUMS
  done
)

echo "==> Artifacts in $DIST_DIR:"
ls -lh "$DIST_DIR"
