<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX UI FairyGUI

[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui?label=version&color=green)](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE.md)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使**

[📖 文檔](https://gameframex.doc.alianblank.com) • [🚀 快速開始](#快速開始) • [💬 QQ群: 612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)

---

🌐 **語言**: [English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

---

</div>

## 項目簡介

GameFrameX UI FairyGUI 是一個 Unity UI 適配器，將 [FairyGUI](https://www.fairygui.com/) 框架封裝到 GameFrameX 模組化遊戲框架中。它提供了完整的 UI 生命週期管理（開啟/關閉/回收/動畫），並透過 YooAsset 實現非同步資源載入。

### 主要特性

- **完整 UI 生命週期** — 透過統一 API 實現介面的開啟、關閉、回收和動畫
- **非同步資源載入** — 基於 YooAsset 的 FairyGUI 包和資源非同步載入
- **物件池復用** — UI 介面實例被池化復用，最小化 GC 分配
- **單例去重** — 自動防止重複建立同一介面
- **載入佇列** — 合併對同一介面的並行開啟請求
- **屬性驅動配置** — 透過 C# 特性控制 UI 分組、顯示/隱藏動畫
- **MVVM 繫結支援** — 透過 `BindablePropertyExtension` 自動清理繫結屬性事件
- **雙模式資源載入** — 同時支援 `Resources.Load` 和 YooAsset AssetBundle 載入
- **IL2CPP 安全** — Preserve 特性防止 IL2CPP 建置時程式碼裁剪

## 架構概覽

```
┌─────────────────────────────────────────────┐
│              你的 UI 面板 (FUI)               │
├─────────────────────────────────────────────┤
│                  UIManager                   │
│      (開啟 / 關閉 / 回收 / 載入佇列)          │
├──────────┬──────────┬───────────────────────┤
│ FormHelper│UIGroup   │ PackageComponent      │
│ (建立/    │Helper    │ (新增包 /             │
│  釋放)    │(深度)    │  移除包)              │
├──────────┴──────────┴───────────────────────┤
│         FairyGUI Runtime + YooAsset          │
└─────────────────────────────────────────────┘
```

| 元件 | 描述 |
|------|------|
| `UIManager` | 核心 UI 管理器，處理開啟/關閉/回收生命週期 |
| `FUI` | 所有 FairyGUI 面板的基底類別 — 繼承此類別建立介面 |
| `FairyGUIPackageComponent` | MonoBehaviour，管理 FairyGUI 包的載入/卸載 |
| `FairyGUIFormHelper` | 處理介面的實例化、建立和釋放 |
| `FairyGUIUIGroupHelper` | 管理 UI 分組深度和層級 |
| `FairyGUILoadAsyncResourceHelper` | 將 FairyGUI 資源請求橋接到 YooAsset |
| `FairyGUIPathFinderHelper` | 基於路徑的 GObject 查找工具 |

## 快速開始

### 安裝方式

選擇以下任一方式安裝：

**方式一：manifest.json**

在專案的 `Packages/manifest.json` 的 `dependencies` 節點下新增：

```json
{
  "com.gameframex.unity.ui.fairygui": "https://github.com/AlianBlank/com.gameframex.unity.ui.fairygui.git"
}
```

**方式二：Unity Package Manager（Git URL）**

開啟 Unity Package Manager → 透過 Git URL 新增：

```
https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git
```

**方式三：手動安裝**

下載倉庫並放置到 Unity 專案的 `Packages/` 目錄下，Unity 會自動識別。

### 依賴項

| 套件名稱 | 版本 | 說明 |
|----------|------|------|
| `com.gameframex.unity` | ≥ 1.1.1 | 核心框架執行時 |
| `com.gameframex.unity.ui` | ≥ 1.0.0 | 基礎 UI 抽象層 |
| `com.gameframex.unity.asset` | ≥ 1.0.6 | 資源載入系統 |
| `com.gameframex.unity.event` | ≥ 1.0.0 | 事件系統 |
| FairyGUI Runtime | — | FairyGUI 函式庫 |
| YooAsset | — | 資源管理 |
| UniTask | — | 非同步等待支援 |

### 基本使用

1. **新增 `FairyGUIPackageComponent`** 到場景中（透過 `GameFrameX → FairyGUIPackage` 選單）

2. **繼承 `FUI` 建立 UI 面板**：

```csharp
using GameFrameX.UI.FairyGUI.Runtime;

[OptionUIGroup("Default")]
public class MyPanel : FUI
{
    protected override void OnInit()
    {
        // 初始化 UI 元素
    }

    protected override void OnOpen(object userData)
    {
        // 處理開啟邏輯
    }

    protected override void OnClose()
    {
        // 處理關閉邏輯
    }
}
```

3. **透過框架 UI 元件開啟面板**：

```csharp
// 非同步開啟 UI 面板
await GameEntry.GetComponent<UIComponent>().OpenUIFormAsync("MyPackage", "MyPanel");
```

## 使用範例

### 顯示/隱藏動畫

使用特性配置動畫：

```csharp
[OptionUIShowAnimation(typeof(FadeInAnimation))]
[OptionUIHideAnimation(typeof(FadeOutAnimation))]
public class AnimatedPanel : FUI { }
```

### 基於路徑的物件查找

```csharp
// 取得 GObject 的層級路徑
string path = gObject.GetUIPath();

// 透過路徑解析 GObject
GObject obj = FairyGUIPathFinderHelper.GetUIFromPath("GRoot/Group/MyButton");
```

### MVVM 繫結自動清理

```csharp
// GObject 銷毀時自動註銷事件
myProperty.ClearWithGObjectDestroyed(gObject);
```

## 平台支援

| 平台 | 支援 |
|------|------|
| Android | ✅ |
| iOS | ✅ |
| Windows | ✅ |
| macOS | ✅ |
| WebGL | ✅ |

最低 Unity 版本：**2019.4**

## 文檔與資源

- [GameFrameX 文檔](https://gameframex.doc.alianblank.com)
- [FairyGUI 文檔](https://www.fairygui.com/docs)
- [更新日誌](CHANGELOG.md)
- [開源協議](LICENSE.md)（MIT + Apache 2.0）

## 社區與支援

- **QQ群**：[612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**：[提交 Bug](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **作者**：Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## 更新日誌

詳見 [CHANGELOG.md](CHANGELOG.md)。

## 開源協議

本專案基於 [MIT 協議](https://opensource.org/licenses/MIT) 和 [Apache 2.0 協議](http://www.apache.org/licenses/LICENSE-2.0) 雙重許可。詳見 [LICENSE.md](LICENSE.md)。
