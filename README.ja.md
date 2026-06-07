<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX UI FairyGUI

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui)](https://github.com/GameFrameX/com.gameframex.unity.ui.fairygui/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#quick-start) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

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

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加してください：
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
     ],
     "dependencies": {
       "com.gameframex.unity.ui.fairygui": "3.3.2"
     }
   }
   ```

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接追加：
   ```json
   {
      "com.gameframex.unity.ui.fairygui": "https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git"
   }
   ```
3. Unity の **Package Manager** で **Git URL** を使用して追加：`https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。
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
| Android | はい |
| iOS | はい |
| Windows | はい |
| macOS | はい |
| WebGL | はい |

Unity 最低バージョン：**2019.4**

## ドキュメントとリソース

- [GameFrameX ドキュメント](https://gameframex.doc.alianblank.com)
- [FairyGUI ドキュメント](https://www.fairygui.com/docs)
- [変更履歴](CHANGELOG.md)
- [ライセンス](LICENSE.md)

## コミュニティとサポート

- **QQグループ**：[612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**：[バグ報告](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **作者**：Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## 変更履歴

[CHANGELOG.md](CHANGELOG.md) をご覧ください。


## 依存関係

| パッケージ | 説明 |
|----------|------|
| `com.gameframex.unity` | 1.1.1 |
| `com.gameframex.unity.asset` | 1.0.6 |
| `com.gameframex.unity.event` | 1.0.0 |
| `com.gameframex.unity.ui` | 1.0.0 |
## ライセンス

詳しくは [LICENSE.md](LICENSE.md) をご参照ください。
