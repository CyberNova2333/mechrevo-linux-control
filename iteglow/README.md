# iteglow

Userspace control for the **RGB keyboard backlight and lightbar** on
Tongfang/Uniwill barebones laptops (sold as **Captiva**, and rebadged by XMG,
Eluktronics, Monster, Mechrevo, Machenike and others). These use **ITE 8291**
(keyboard, USB `048d:600b`) and **ITE 8233/8291** (lightbar, USB `048d:7001`)
controllers.

`iteglow` talks to them over `hidraw` using the same HID reports the kernel
driver uses — so you get full control of **modes, colours, speed, direction and
brightness** for both devices, including hardware effects the kernel driver
doesn't expose (e.g. lightbar wave/breathing/marquee/neon).

> **Status:** alpha. The lightbar effect protocol for `048d:7001` was
> reverse-engineered for this project; see [`docs/protocol.md`](docs/protocol.md).

```
iteglow doctor                         # check drivers/devices/permissions
iteglow set lightbar wave --speed 4    # rainbow wave on the lightbar
iteglow set keyboard solid --color '#00ffcc'
iteglow off                            # turn everything off
iteglow gui                            # graphical control panel
```

---

## 1. Is my hardware supported?

Run these and look for the ITE devices:

```bash
lsusb | grep -i 048d
#   ID 048d:600b  ITE Device(8291)   <- keyboard backlight
#   ID 048d:7001  ITE Device(8233)   <- lightbar

# What laptop chassis is this?
sudo dmidecode -s system-manufacturer; sudo dmidecode -s system-product-name
cat /sys/devices/virtual/dmi/id/product_sku
```

If you have `048d:7001` and/or `048d:600b`, you're in scope. Other ITE ids
(`048d:6010`, `048d:7000`) are handled by the kernel driver for related models;
support in `iteglow` can be added — see [`docs/hardware.md`](docs/hardware.md).

After the driver step below, the kernel LED nodes appear here:

```bash
ls /sys/class/leds/ | grep -E 'kbd_backlight|lightbar'
#   rgb:kbd_backlight   rgb:lightbar
```

## 2. Prerequisite: tuxedo-drivers (the kernel modules)

`iteglow` needs the **`tuxedo-drivers`** package (the `ite_8291` / `ite_8291_lb`
kernel modules — they create the LED nodes and bind the devices).

Add TUXEDO's apt repo following their official guide, then install the package:

> **Set up the repo:** <https://www.tuxedocomputers.com/en/Add-TUXEDO-software-package-sources.tuxedo>

```bash
sudo apt install tuxedo-drivers
```

**Secure Boot:** these are DKMS modules — they must be signed and the key
enrolled or they won't load. On Ubuntu the install prompts you to set a MOK
password; complete enrollment at the blue *MOK Manager* screen on the next
reboot (or disable Secure Boot in firmware).

Verify:

```bash
lsmod | grep -E 'ite_8291|ite_8291_lb'
iteglow doctor
```

## 3. Install iteglow

This section describes the upstream Python distribution. For this repository’s native CLI, desktop app and systemd user services, follow the [MECHREVO Linux Control installation guide](../README.md).

### Upstream Debian / Ubuntu package

Grab the `.deb` from the [releases page](https://github.com/JeanElsner/iteglow/releases)
and install it:

```bash
sudo apt install ./iteglow_*_all.deb
```

The package sets up everything in one go: the udev rule (run without sudo,
effective immediately), the GTK4 GUI dependencies (as Recommends), the desktop
entry, the polkit policy for the GUI's pin button, and
`iteglow-restore.service` for boot/resume persistence. Skip straight to
[Usage](#5-usage).

To build the package yourself instead:

```bash
git clone https://github.com/JeanElsner/iteglow.git && cd iteglow
sudo apt install debhelper dh-python python3-all python3-hatchling \
                 pybuild-plugin-pyproject python3-pytest
dpkg-buildpackage -us -uc
sudo apt install ../iteglow_*_all.deb
```

### Other distros — pip/pipx

The core + CLI are **pure Python (no dependencies)** and install cleanly into
a venv anywhere:

```bash
git clone https://github.com/JeanElsner/iteglow.git
cd iteglow
python3 -m venv --system-site-packages .venv && . .venv/bin/activate
pip install -e .            # or: pip install -e ".[dev]" for tests
```

(`--system-site-packages` lets the GUI see the distro's PyGObject.) Or with
**pipx**: `pipx install .` — isolated and on PATH.

The GUI uses **GTK4 via PyGObject**, best provided by your distro rather
than pip:

```bash
# Fedora:  sudo dnf install python3-gobject gtk4
# Arch:    sudo pacman -S python-gobject gtk4
```

Launch with `iteglow gui` (or `iteglow-gui`, or the desktop entry in
`data/iteglow.desktop`).

## 4. Device permissions (run without sudo)

**Debian package users: already handled — skip this step.** Everyone else,
install the udev rule so your user can access the devices:

```bash
sudo bash scripts/install-udev.sh
# internal USB can't be replugged; if access still fails, reboot once
```

(Or just run `iteglow`/`iteglow gui` with `sudo` — but a root GTK app may fail
to open a window on Wayland, which is why the udev rule is preferred.)

## 5. Usage

```bash
iteglow list                       # modes per device
iteglow doctor                     # drivers + permissions self-check

# Lightbar
iteglow set lightbar static  --color '#00ffcc' --brightness 80
iteglow set lightbar wave    --speed 4
iteglow set lightbar neon    --direction reverse --palette ff0000 00ff00 0000ff

# Keyboard
iteglow set keyboard solid     --color 255,0,128
iteglow set keyboard aurora
iteglow set keyboard wave      --direction ttb

# Off
iteglow off lightbar
iteglow off                        # all detected devices
```

Modes: lightbar = `off static breathing wave marquee neon runner`;
keyboard = `off solid breathing wave reactive rainbow ripple marquee raindrop aurora spark`.

## How it works

ITE keyboard and lightbar are independent USB-HID devices. `iteglow` sends
8-byte **feature reports** (`SET_REPORT`) and, for keyboard solid colour,
interrupt **output reports** — exactly what the kernel driver does. Full
protocol notes (effect ids, byte layout, how it was reverse-engineered) are in
[`docs/protocol.md`](docs/protocol.md).

## Persistence across reboots and suspend

The controllers lose their state on suspend (and dual-boot leaves them in
whatever state Windows set). Worse, the kernel driver cannot re-enable the
`048d:7001` lightbar on resume ([tuxedo-drivers #371](https://gitlab.com/tuxedocomputers/development/packages/tuxedo-drivers/-/issues/371)).

In MECHREVO Linux Control, `iteglow set` saves the current user's lighting
settings. To restore them at graphical login and after resume, run the
following as that user after installing the native package.

```bash
systemctl --user enable --now iteglow-session-restore.service
```

For a manual restore, run `iteglow restore`. To share a snapshot system-wide,
run `sudo iteglow pin` once and use `iteglow restore --system` as the desktop
user when you want to apply that snapshot.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Protocol additions for other ITE ids
are very welcome — the byte tables in `docs/protocol.md` are the place to start.

## Acknowledgements

- **TUXEDO Computers** — the GPL [`tuxedo-drivers`](https://gitlab.com/tuxedocomputers/development/packages/tuxedo-drivers)
  kernel modules, the reference for most of the protocol.
- **pobrn** — [`ite8291r3-ctl`](https://github.com/pobrn/ite8291r3-ctl) /
  [`qc71_laptop`](https://github.com/pobrn/qc71_laptop), prior art on ITE keyboards.
- **OpenRGB** — the [Clevo keyboard/lightbar controllers](https://gitlab.com/CalcProgrammer1/OpenRGB)
  (GPL-2.0), reference for the per-key row protocol on `048d:600b`.

No GPL source is copied into this project; command byte sequences are functional
facts used to interoperate with the hardware.

## License

MIT — see [LICENSE](LICENSE).
