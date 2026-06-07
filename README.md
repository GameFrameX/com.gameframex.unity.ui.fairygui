<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX UI FairyGUI

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

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

Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:

```json
{
  "scopedRegistries": [
    {
      "name": "GameFrameX",
      "url": "https://gameframex.upm.alianblank.uk",
      "scopes": [
        "com.gameframex"
      ]
    }
  ]
}
```

`scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

Then add the package to `dependencies`:

```json
{
  "dependencies": {
    "com.gameframex.unity.ui.fairygui": "3.3.2"
  }
}
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
| Android | Yes |
| iOS | Yes |
| Windows | Yes |
| macOS | Yes |
| WebGL | Yes |

Minimum Unity version: **2019.4**

## Documentation & Resources

- [GameFrameX Documentation](https://gameframex.doc.alianblank.com)
- [FairyGUI Documentation](https://www.fairygui.com/docs)
- [CHANGELOG](CHANGELOG.md)
- [LICENSE](LICENSE.md)

## Community & Support

- **QQ Group**: [612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**: [Report a bug](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **Author**: Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for release history.


## Dependencies

| Package | Description |
|---------|-------------|
| `com.gameframex.unity` | 1.1.1 |
| `com.gameframex.unity.asset` | 1.0.6 |
| `com.gameframex.unity.event` | 1.0.0 |
| `com.gameframex.unity.ui` | 1.0.0 |
## License

See [LICENSE.md](LICENSE.md) for license information.
