<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX UI FairyGUI

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#quick-start) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

GameFrameX UI FairyGUI 是一个 Unity UI 适配器，将 [FairyGUI](https://www.fairygui.com/) 框架封装到 GameFrameX 模块化游戏框架中。它提供了完整的 UI 生命周期管理（打开/关闭/回收/动画），并通过 YooAsset 实现异步资源加载。

### 主要特性

- **完整 UI 生命周期** — 通过统一 API 实现界面的打开、关闭、回收和动画
- **异步资源加载** — 基于 YooAsset 的 FairyGUI 包和资源异步加载
- **对象池复用** — UI 界面实例被池化复用，最小化 GC 分配
- **单例去重** — 自动防止重复创建同一界面
- **加载队列** — 合并对同一界面的并发打开请求
- **属性驱动配置** — 通过 C# 特性控制 UI 分组、显示/隐藏动画
- **MVVM 绑定支持** — 通过 `BindablePropertyExtension` 自动清理绑定属性事件
- **双模式资源加载** — 同时支持 `Resources.Load` 和 YooAsset AssetBundle 加载
- **IL2CPP 安全** — Preserve 特性防止 IL2CPP 构建时代码裁剪

## 架构概览

```
┌─────────────────────────────────────────────┐
│              你的 UI 面板 (FUI)               │
├─────────────────────────────────────────────┤
│                  UIManager                   │
│      (打开 / 关闭 / 回收 / 加载队列)          │
├──────────┬──────────┬───────────────────────┤
│ FormHelper│UIGroup   │ PackageComponent      │
│ (创建/    │Helper    │ (添加包 /             │
│  释放)    │(深度)    │  移除包)              │
├──────────┴──────────┴───────────────────────┤
│         FairyGUI Runtime + YooAsset          │
└─────────────────────────────────────────────┘
```

| 组件 | 描述 |
|------|------|
| `UIManager` | 核心 UI 管理器，处理打开/关闭/回收生命周期 |
| `FUI` | 所有 FairyGUI 面板的基类 — 继承此类创建界面 |
| `FairyGUIPackageComponent` | MonoBehaviour，管理 FairyGUI 包的加载/卸载 |
| `FairyGUIFormHelper` | 处理界面的实例化、创建和释放 |
| `FairyGUIUIGroupHelper` | 管理 UI 分组深度和层级 |
| `FairyGUILoadAsyncResourceHelper` | 将 FairyGUI 资源请求桥接到 YooAsset |
| `FairyGUIPathFinderHelper` | 基于路径的 GObject 查找工具 |

## 快速开始

### 安装

编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：

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

`scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

Then add the package to `dependencies`:

```json
{
  "dependencies": {
    "com.gameframex.unity.ui.fairygui": "3.3.2"
  }
}
```


## 使用示例

### 显示/隐藏动画

使用特性配置动画：

```csharp
[OptionUIShowAnimation(typeof(FadeInAnimation))]
[OptionUIHideAnimation(typeof(FadeOutAnimation))]
public class AnimatedPanel : FUI { }
```

### 基于路径的对象查找

```csharp
// 获取 GObject 的层级路径
string path = gObject.GetUIPath();

// 通过路径解析 GObject
GObject obj = FairyGUIPathFinderHelper.GetUIFromPath("GRoot/Group/MyButton");
```

### MVVM 绑定自动清理

```csharp
// GObject 销毁时自动注销事件
myProperty.ClearWithGObjectDestroyed(gObject);
```

## 平台支持

| 平台 | 支持 |
|------|------|
| Android | ✅ |
| iOS | ✅ |
| Windows | ✅ |
| macOS | ✅ |
| WebGL | ✅ |

最低 Unity 版本：**2019.4**

## 文档与资源

- [GameFrameX 文档](https://gameframex.doc.alianblank.com)
- [FairyGUI 文档](https://www.fairygui.com/docs)
- [更新日志](CHANGELOG.md)
- [开源协议](LICENSE.md)

## 社区与支持

- **QQ群**：[612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**：[提交 Bug](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **作者**：Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)。


## 依赖

| 包 | 说明 |
|----|------|
| `com.gameframex.unity` | 1.1.1 |
| `com.gameframex.unity.asset` | 1.0.6 |
| `com.gameframex.unity.event` | 1.0.0 |
| `com.gameframex.unity.ui` | 1.0.0 |
## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
