// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useState } from "react";
import {
  Droplets,
  Gauge,
  Keyboard,
  Layers,
  PowerOff,
  SlidersHorizontal,
  Sparkles,
  Sun,
} from "lucide-react";
import { api } from "./api/tauri";
import type { PageId, SystemInfo } from "./types/hardware";
import { LightingPage } from "./pages/LightingPage";
import { LogoPage } from "./pages/LogoPage";
import { PolicyPage } from "./pages/PolicyPage";
import { ProfilesPage } from "./pages/ProfilesPage";
import { PerformancePage } from "./pages/PerformancePage";
import { WaterCoolingPage } from "./pages/WaterCoolingPage";

interface NavMeta {
  id: PageId;
  title: string;
  subtitle: string;
  group: "灯光硬件" | "自动与联动" | "性能与散热";
  Icon: typeof Keyboard;
}

const NAV_ITEMS: NavMeta[] = [
  { id: "keyboard", title: "键盘背光", subtitle: "101 键灯效与单键矩阵编辑器", group: "灯光硬件", Icon: Keyboard },
  { id: "lightbar", title: "机身灯带", subtitle: "前缘与侧翼氛围灯带控制", group: "灯光硬件", Icon: Sun },
  { id: "logo", title: "后盖 Logo", subtitle: "屏幕后盖独立发光徽标", group: "灯光硬件", Icon: Sparkles },
  { id: "lighting-policy", title: "自动灯光", subtitle: "供电切换亮度与空闲自动熄灯", group: "自动与联动", Icon: SlidersHorizontal },
  { id: "profiles", title: "情景预设", subtitle: "性能与三区灯光组合规则联动", group: "自动与联动", Icon: Layers },
  { id: "performance", title: "性能模式", subtitle: "固件运行档位、功耗调校与风扇曲线", group: "性能与散热", Icon: Gauge },
  { id: "watercooling", title: "水冷控制", subtitle: "外置蓝牙水冷泵扇、水流保护与双区灯控", group: "性能与散热", Icon: Droplets },
];

const GROUPS: NavMeta["group"][] = ["灯光硬件", "自动与联动", "性能与散热"];

export default function App() {
  const [activePage, setActivePage] = useState<PageId>("keyboard");
  const [sysInfo, setSysInfo] = useState<SystemInfo | null>(null);
  const [globalNotice, setGlobalNotice] = useState<string>("");
  const [allOffBusy, setAllOffBusy] = useState(false);
  const [refreshNonce, setRefreshNonce] = useState(0);

  useEffect(() => {
    let alive = true;
    const load = () => {
      api
        .getSystemInfo()
        .then((info) => {
          if (alive) setSysInfo(info);
        })
        .catch(() => {});
    };
    load();
    const timer = window.setInterval(load, 8000);
    return () => {
      alive = false;
      window.clearInterval(timer);
    };
  }, []);

  const currentNav = NAV_ITEMS.find((n) => n.id === activePage) ?? NAV_ITEMS[0];

  const daemonCount = sysInfo
    ? Object.values(sysInfo.daemons).filter(Boolean).length
    : 0;

  async function handleAllOff() {
    if (allOffBusy) return;
    setAllOffBusy(true);
    setGlobalNotice("");
    try {
      const res = await api.allLightsOff();
      setRefreshNonce((n) => n + 1);
      if (res.errors.length > 0) {
        setGlobalNotice(`已关闭 ${res.turned_off.join(" / ") || "部分灯区"}；异常：${res.errors.join("；")}`);
      } else {
        setGlobalNotice("已关闭全部灯光，原有配色已保留");
      }
    } catch (err) {
      setGlobalNotice(`关闭全部灯光失败：${String(err)}`);
    } finally {
      setAllOffBusy(false);
    }
  }

  return (
    <div className="app-shell">
      <aside className="sidebar" aria-label="主导航">
        <div className="brand-lockup">
          <div className="brand-crest" aria-hidden="true">
            <svg width="22" height="16" viewBox="0 0 24 18" fill="none">
              <path
                d="M2 15L7.5 3L12 11.5L16.5 3L22 15"
                stroke="#38BDF8"
                strokeWidth="2.4"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
          <div>
            <div className="brand-title">MECHREVO</div>
            <div className="brand-sub">YAOSHI 控制中心</div>
          </div>
        </div>

        {GROUPS.map((group) => (
          <div key={group}>
            <div className="nav-group-label">{group}</div>
            {NAV_ITEMS.filter((item) => item.group === group).map((item) => {
              const Icon = item.Icon;
              const selected = activePage === item.id;
              return (
                <button
                  key={item.id}
                  type="button"
                  className={`nav-item${selected ? " active" : ""}`}
                  onClick={() => setActivePage(item.id)}
                  aria-current={selected ? "page" : undefined}
                >
                  <Icon size={16} strokeWidth={2} />
                  <span>{item.title}</span>
                </button>
              );
            })}
          </div>
        ))}

        <div className="sidebar-spacer" />

        <div className="hardware-bay">
          <div className="hardware-bay-title">{sysInfo?.board_name || "YAOSHI Series"}</div>
          <div className="hardware-bay-meta mono">
            <span>BIOS {sysInfo?.bios_version || "--"}</span>
            <span>{sysInfo?.kernel || "Linux"}</span>
          </div>
          <div className="daemon-dots" title="Rust 后台守护进程健康状态（水冷 / 会话灯光 / 狂暴调度 / EC 恢复）">
            <span className={`dot ${daemonCount === 4 ? "ok" : daemonCount > 0 ? "warn" : "muted"}`} />
            <span>
              Rust 守护 <strong className="mono">{daemonCount}/4</strong> 在线
            </span>
          </div>
        </div>
      </aside>

      <div className="main-column">
        <header className="topbar">
          <div className="topbar-heading">
            <span className="topbar-title">{currentNav.title}</span>
            <span className="topbar-sub">{currentNav.subtitle}</span>
          </div>
          <div className="topbar-actions">
            {globalNotice && <span className="pill">{globalNotice}</span>}
            <button
              type="button"
              className="btn btn-danger btn-sm"
              disabled={allOffBusy}
              onClick={handleAllOff}
              title="关闭键盘、机身灯带与后盖 Logo，保留配色设置"
            >
              <PowerOff size={13} />
              <span>{allOffBusy ? "正在关闭…" : "关闭全部灯光"}</span>
            </button>
          </div>
        </header>

        <main className="workspace-scroll">
          {activePage === "keyboard" && (
            <LightingPage key={`keyboard-${refreshNonce}`} device="keyboard" />
          )}
          {activePage === "lightbar" && (
            <LightingPage key={`lightbar-${refreshNonce}`} device="lightbar" />
          )}
          {activePage === "logo" && <LogoPage key={`logo-${refreshNonce}`} />}
          {activePage === "lighting-policy" && <PolicyPage />}
          {activePage === "profiles" && (
            <ProfilesPage onHardwareChanged={() => setRefreshNonce((n) => n + 1)} />
          )}
          {activePage === "performance" && <PerformancePage />}
          {activePage === "watercooling" && <WaterCoolingPage />}
        </main>
      </div>
    </div>
  );
}
