<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX UI FairyGUI

[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui?label=version&color=green)](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE.md)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援**

[📖 ドキュメント](https://gameframex.doc.alianblank.com) • [🚀 クイックスタート](#クイックスタート) • [💬 QQグループ: 612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)

---

🌐 **言語**: [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

---

</div>

## プロジェクト概要

GameFrameX UI FairyGUI は、[FairyGUI](https://www.fairygui.com/) フレームワークを GameFrameX モジュラーゲームフレームワークに統合する Unity UI アダプターです。YooAsset による非同期アセットローディングで、完全な UI ライフサイクル管理（オープン/クローズ/リサイクル/アニメーション）を提供します。

### 主な機能

- **完全な UI ライフサイクル** — 統一 API による UI フォームのオープン、クローズ、リサイクル、アニメーション
- **非同期アセットローディング** — YooAsset ベースの FairyGUI パッケージとリソースの非同期ローディング
- **オブジェクトプール再利用** — UI フォームインスタンスをプールして再利用し、GC アロケーションを最小化
- **シングルトン重複排除** — 同一フォームの重複作成を自動防止
- **ローディングキュー** — 同一フォームへの同時オープンリクエストを統合
- **属性駆動設定** — C# 属性で UI グループ、表示/非表示アニメーションを制御
- **MVVM バインディングサポート** — `BindablePropertyExtension` によるバインディングプロパティイベントの自動クリーンアップ
- **デュアルアセットローディング** — `Resources.Load` と YooAsset AssetBundle ローディングの両方をサポート
- **IL2CPP 対応** — Preserve 属性により IL2CPP ビルド時のコードストリッピングを防止

## アーキテクチャ

```
┌─────────────────────────────────────────────┐
│              あなたの UI パネル (FUI)         │
├─────────────────────────────────────────────┤
│                  UIManager                   │
│     (オープン / クローズ / リサイクル /        │
│      ローディングキュー)                       │
├──────────┬──────────┬───────────────────────┤
│ FormHelper│UIGroup   │ PackageComponent      │
│ (作成/    │Helper    │ (パッケージ追加 /      │
│  解放)    │(深度)    │  パッケージ削除)       │
├──────────┴──────────┴───────────────────────┤
│         FairyGUI Runtime + YooAsset          │
└─────────────────────────────────────────────┘
```

| コンポーネント | 説明 |
|---------------|------|
| `UIManager` | オープン/クローズ/リサイクルライフサイクルを管理する UI マネージャー |
| `FUI` | すべての FairyGUI パネルの基底クラス — これを継承して UI を作成 |
| `FairyGUIPackageComponent` | MonoBehaviour。FairyGUI パッケージの読み込み/アンロードを管理 |
| `FairyGUIFormHelper` | フォームのインスタンス化、作成、解放を処理 |
| `FairyGUIUIGroupHelper` | UI グループの深度とレイヤー管理 |
| `FairyGUILoadAsyncResourceHelper` | FairyGUI のリソースリクエストを YooAsset に橋渡し |
| `FairyGUIPathFinderHelper` | パスベースの GObject 検索ユーティリティ |

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

**方法 1: manifest.json**

プロジェクトの `Packages/manifest.json` の `dependencies` に追加：

```json
{
  "com.gameframex.unity.ui.fairygui": "https://github.com/AlianBlank/com.gameframex.unity.ui.fairygui.git"
}
```

**方法 2: Unity Package Manager（Git URL）**

Unity Package Manager を開き、Git URL から追加：

```
https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git
```

**方法 3: 手動インストール**

リポジトリをダウンロードして Unity プロジェクトの `Packages/` ディレクトリに配置します。Unity が自動的に認識します。

### 依存関係

| パッケージ | バージョン | 説明 |
|-----------|-----------|------|
| `com.gameframex.unity` | ≥ 1.1.1 | コアフレームワークランタイム |
| `com.gameframex.unity.ui` | ≥ 1.0.0 | 基本 UI 抽象レイヤー |
| `com.gameframex.unity.asset` | ≥ 1.0.6 | アセットローディングシステム |
| `com.gameframex.unity.event` | ≥ 1.0.0 | イベントシステム |
| FairyGUI Runtime | — | FairyGUI ライブラリ |
| YooAsset | — | アセット管理 |
| UniTask | — | 非同期/await サポート |

### 基本的な使い方

1. **`FairyGUIPackageComponent` をシーンに追加**（`GameFrameX → FairyGUIPackage` メニューから）

2. **`FUI` を継承して UI パネルを作成**：

```csharp
using GameFrameX.UI.FairyGUI.Runtime;

[OptionUIGroup("Default")]
public class MyPanel : FUI
{
    protected override void OnInit()
    {
        // UI 要素の初期化
    }

    protected override void OnOpen(object userData)
    {
        // オープン時の処理
    }

    protected override void OnClose()
    {
        // クローズ時の処理
    }
}
```

3. **フレームワークの UI コンポーネントからパネルを開く**：

```csharp
// UI パネルを非同期でオープン
await GameEntry.GetComponent<UIComponent>().OpenUIFormAsync("MyPackage", "MyPanel");
```

## 使用例

### 表示/非表示アニメーション

属性でアニメーションを設定：

```csharp
[OptionUIShowAnimation(typeof(FadeInAnimation))]
[OptionUIHideAnimation(typeof(FadeOutAnimation))]
public class AnimatedPanel : FUI { }
```

### パスベースのオブジェクト検索

```csharp
// GObject の階層パスを取得
string path = gObject.GetUIPath();

// パスから GObject を解決
GObject obj = FairyGUIPathFinderHelper.GetUIFromPath("GRoot/Group/MyButton");
```

### MVVM バインディング自動クリーンアップ

```csharp
// GObject 破棄時に自動的にイベントを登録解除
myProperty.ClearWithGObjectDestroyed(gObject);
```

## プラットフォーム対応

| プラットフォーム | 対応 |
|----------------|------|
| Android | ✅ |
| iOS | ✅ |
| Windows | ✅ |
| macOS | ✅ |
| WebGL | ✅ |

Unity 最低バージョン：**2019.4**

## ドキュメントとリソース

- [GameFrameX ドキュメント](https://gameframex.doc.alianblank.com)
- [FairyGUI ドキュメント](https://www.fairygui.com/docs)
- [変更履歴](CHANGELOG.md)
- [ライセンス](LICENSE.md)（MIT + Apache 2.0）

## コミュニティとサポート

- **QQグループ**：[612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**：[バグ報告](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **作者**：Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## 変更履歴

[CHANGELOG.md](CHANGELOG.md) をご覧ください。

## ライセンス

本プロジェクトは [MIT ライセンス](https://opensource.org/licenses/MIT) と [Apache 2.0 ライセンス](http://www.apache.org/licenses/LICENSE-2.0) のデュアルライセンスです。詳細は [LICENSE.md](LICENSE.md) をご覧ください。
