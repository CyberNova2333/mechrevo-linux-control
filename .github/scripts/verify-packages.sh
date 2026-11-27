#!/usr/bin/env bash
# SPDX-License-Identifier: AGPL-3.0-only
# Run from dist/ with the verified manifest version.
set -euo pipefail

version="${1:?missing package version}"
deb="iteglow-control_${version}_amd64.deb"
rpm_package="iteglow-control-${version}-1.x86_64.rpm"
appimage="iteglow-control-${version}-x86_64.AppImage"
bundle="mechrevo-system-daemon-${version}-linux-x86_64"

sha256sum -c SHA256SUMS

dpkg-deb -I "$deb"
deb_depends="$(dpkg-deb -f "$deb" Depends)"
rpm_requires="$(rpm -qp --requires "$rpm_package")"
echo "$rpm_requires"
if grep -qi python <<< "$deb_depends" || grep -qi python <<< "$rpm_requires"; then
  echo "error: .deb or .rpm unexpectedly requires Python" >&2
  exit 1
fi

test -x "$appimage"
file "$appimage"

# tar checks every requested member and reports missing entries or a damaged archive.
tar -tzf "$bundle.tar.gz" \
  "$bundle/libexec/mechrevo-profile-helper" \
  "$bundle/libexec/mechrevo-turbod" \
  "$bundle/systemd/mechrevo-turbo.service" \
  "$bundle/polkit/org.cybernova.mechrevo.profile.policy"
