<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX UI FairyGUI

[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui?label=version&color=green)](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE.md)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams**

[📖 Documentation](https://gameframex.doc.alianblank.com) • [🚀 Quick Start](#quick-start) • [💬 QQ Group: 612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)

---

🌐 **Language**: **English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

</div>

## Project Overview

GameFrameX UI FairyGUI is a Unity UI adapter that wraps the [FairyGUI](https://www.fairygui.com/) framework into the GameFrameX modular game framework. It provides complete UI lifecycle management (open/close/recycle/animate) with async asset loading via YooAsset.

### Key Features

- **Complete UI Lifecycle** — Open, close, recycle, and animate UI forms with a unified API
- **Async Asset Loading** — YooAsset-backed async loading for FairyGUI packages and resources
- **Object Pooling** — UI form instances are pooled for reuse, minimizing GC allocations
- **Singleton Deduplication** — Automatically prevents duplicate form creation
- **Loading Queue** — Coalesces concurrent open requests for the same form
- **Attribute-Driven Configuration** — Control UI groups, show/hide animations via C# attributes
- **MVVM Binding Support** — Automatic cleanup of bindable property events with `BindablePropertyExtension`
- **Dual Asset Loading** — Supports both `Resources.Load` and YooAsset AssetBundle loading
- **IL2CPP Safe** — Preserve attributes prevent code stripping on IL2CPP builds

## Architecture

```
┌─────────────────────────────────────────────┐
│              Your UI Panels (FUI)            │
├─────────────────────────────────────────────┤
│                  UIManager                   │
│    (Open / Close / Recycle / Loading Queue)  │
├──────────┬──────────┬───────────────────────┤
│ FormHelper│UIGroup   │ PackageComponent      │
│ (Create/  │Helper    │ (AddPackage /         │
│  Release) │(Depth)   │  RemovePackage)       │
├──────────┴──────────┴───────────────────────┤
│         FairyGUI Runtime + YooAsset          │
└─────────────────────────────────────────────┘
```

| Component | Description |
|-----------|-------------|
| `UIManager` | Central UI manager handling open/close/recycle lifecycle |
| `FUI` | Base class for all FairyGUI panels — inherit from this |
| `FairyGUIPackageComponent` | MonoBehaviour managing FairyGUI package loading/unloading |
| `FairyGUIFormHelper` | Handles form instantiation, creation, and release |
| `FairyGUIUIGroupHelper` | Manages UI group depth and layering |
| `FairyGUILoadAsyncResourceHelper` | Bridges FairyGUI resource requests to YooAsset |
| `FairyGUIPathFinderHelper` | Path-based GObject lookup utility |

## Quick Start

### Installation

Choose one of the following methods:

**Method 1: manifest.json**

Add the following to your project's `Packages/manifest.json` under `dependencies`:

```json
{
  "com.gameframex.unity.ui.fairygui": "https://github.com/AlianBlank/com.gameframex.unity.ui.fairygui.git"
}
```

**Method 2: Unity Package Manager (Git URL)**

Open Unity Package Manager → Add package from git URL:

```
https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git
```

**Method 3: Manual**

Download the repository and place it in your Unity project's `Packages/` directory. Unity will auto-detect it.

### Dependencies

| Package | Version | Description |
|---------|---------|-------------|
| `com.gameframex.unity` | ≥ 1.1.1 | Core framework runtime |
| `com.gameframex.unity.ui` | ≥ 1.0.0 | Base UI abstraction layer |
| `com.gameframex.unity.asset` | ≥ 1.0.6 | Asset loading system |
| `com.gameframex.unity.event` | ≥ 1.0.0 | Event system |
| FairyGUI Runtime | — | FairyGUI library |
| YooAsset | — | Asset management |
| UniTask | — | Async/await support |

### Basic Usage

1. **Add `FairyGUIPackageComponent`** to your scene (via `GameFrameX → FairyGUIPackage` menu)

2. **Create a UI panel** by inheriting from `FUI`:

```csharp
using GameFrameX.UI.FairyGUI.Runtime;

[OptionUIGroup("Default")]
public class MyPanel : FUI
{
    protected override void OnInit()
    {
        // Initialize UI elements
    }

    protected override void OnOpen(object userData)
    {
        // Handle open logic
    }

    protected override void OnClose()
    {
        // Handle close logic
    }
}
```

3. **Open the panel** through the framework's UI component:

```csharp
// Open a UI panel asynchronously
await GameEntry.GetComponent<UIComponent>().OpenUIFormAsync("MyPackage", "MyPanel");
```

## Usage Examples

### Show/Hide Animations

Use attributes to configure animations:

```csharp
[OptionUIShowAnimation(typeof(FadeInAnimation))]
[OptionUIHideAnimation(typeof(FadeOutAnimation))]
public class AnimatedPanel : FUI { }
```

### Path-Based Object Lookup

```csharp
// Get the hierarchical path of a GObject
string path = gObject.GetUIPath();

// Resolve a GObject from path
GObject obj = FairyGUIPathFinderHelper.GetUIFromPath("GRoot/Group/MyButton");
```

### MVVM Binding with Auto-Cleanup

```csharp
// Automatically unregisters events when the GObject is destroyed
myProperty.ClearWithGObjectDestroyed(gObject);
```

## Platform Support

| Platform | Supported |
|----------|-----------|
| Android | ✅ |
| iOS | ✅ |
| Windows | ✅ |
| macOS | ✅ |
| WebGL | ✅ |

Minimum Unity version: **2019.4**

## Documentation & Resources

- [GameFrameX Documentation](https://gameframex.doc.alianblank.com)
- [FairyGUI Documentation](https://www.fairygui.com/docs)
- [CHANGELOG](CHANGELOG.md)
- [LICENSE](LICENSE.md) (MIT + Apache 2.0)

## Community & Support

- **QQ Group**: [612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**: [Report a bug](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **Author**: Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for release history.

## License

This project is dual-licensed under the [MIT License](https://opensource.org/licenses/MIT) and [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0). See [LICENSE.md](LICENSE.md) for details.
