#!/usr/bin/env bash
# SPDX-License-Identifier: AGPL-3.0-only
# Run from dist/ with the expected release version.
set -euo pipefail

version="${1:?missing package version}"
deb="iteglow-control_${version}_amd64.deb"
rpm_package="iteglow-control-${version}-1.x86_64.rpm"
appimage="iteglow-control-${version}-x86_64.AppImage"
bundle="mechrevo-system-daemon-${version}-linux-x86_64"

sha256sum -c SHA256SUMS

native_version="$(sed -E 's/^([0-9]+\.[0-9]+\.[0-9]+)-/\1~/; s/-/./g' <<< "$version")"
deb_version="$(dpkg-deb -f "$deb" Version)"
rpm_version="$(rpm -qp --queryformat '%{VERSION}' "$rpm_package")"
if [[ "$deb_version" != "$native_version" || "$rpm_version" != "$native_version" ]]; then
  echo "error: expected $native_version in package metadata (deb=$deb_version, rpm=$rpm_version)" >&2
  exit 1
fi

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
