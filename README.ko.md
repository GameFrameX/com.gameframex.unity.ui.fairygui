<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX UI FairyGUI

[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.ui.fairygui?label=version&color=green)](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE.md)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현**

[📖 문서](https://gameframex.doc.alianblank.com) • [🚀 빠른 시작](#빠른-시작) • [💬 QQ 그룹: 612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)

---

🌐 **언어**: [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

---

</div>

## 프로젝트 개요

GameFrameX UI FairyGUI는 [FairyGUI](https://www.fairygui.com/) 프레임워크를 GameFrameX 모듈형 게임 프레임워크에 통합하는 Unity UI 어댑터입니다. YooAsset 기반 비동기 에셋 로딩으로 완전한 UI 라이프사이클 관리(열기/닫기/재활용/애니메이션)를 제공합니다.

### 주요 기능

- **완전한 UI 라이프사이클** — 통합 API로 UI 폼의 열기, 닫기, 재활용, 애니메이션 처리
- **비동기 에셋 로딩** — YooAsset 기반의 FairyGUI 패키지 및 리소스 비동기 로딩
- **오브젝트 풀 재사용** — UI 폼 인스턴스를 풀링하여 재사용, GC 할당 최소화
- **싱글톤 중복 제거** — 동일한 폼의 중복 생성 자동 방지
- **로딩 큐** — 동일한 폼에 대한 동시 열기 요청 통합
- **속성 기반 설정** — C# 어트리뷰트로 UI 그룹, 표시/숨기기 애니메이션 제어
- **MVVM 바인딩 지원** — `BindablePropertyExtension`으로 바인딩 속성 이벤트 자동 정리
- **듀얼 에셋 로딩** — `Resources.Load` 및 YooAsset AssetBundle 로딩 모두 지원
- **IL2CPP 안전** — Preserve 어트리뷰트로 IL2CPP 빌드 시 코드 스트리핑 방지

## 아키텍처

```
┌─────────────────────────────────────────────┐
│              당신의 UI 패널 (FUI)             │
├─────────────────────────────────────────────┤
│                  UIManager                   │
│     (열기 / 닫기 / 재활용 / 로딩 큐)          │
├──────────┬──────────┬───────────────────────┤
│ FormHelper│UIGroup   │ PackageComponent      │
│ (생성/    │Helper    │ (패키지 추가 /         │
│  해제)    │(깊이)    │  패키지 제거)          │
├──────────┴──────────┴───────────────────────┤
│         FairyGUI Runtime + YooAsset          │
└─────────────────────────────────────────────┘
```

| 컴포넌트 | 설명 |
|---------|------|
| `UIManager` | 열기/닫기/재활용 라이프사이클을 관리하는 UI 매니저 |
| `FUI` | 모든 FairyGUI 패널의 기본 클래스 — 이 클래스를 상속하여 UI 생성 |
| `FairyGUIPackageComponent` | MonoBehaviour. FairyGUI 패키지 로딩/언로딩 관리 |
| `FairyGUIFormHelper` | 폼의 인스턴스화, 생성, 해제 처리 |
| `FairyGUIUIGroupHelper` | UI 그룹 깊이 및 레이어 관리 |
| `FairyGUILoadAsyncResourceHelper` | FairyGUI 리소스 요청을 YooAsset에 연결 |
| `FairyGUIPathFinderHelper` | 경로 기반 GObject 검색 유틸리티 |

## 빠른 시작

### 설치

다음 방법 중 하나를 선택하세요:

**방법 1: manifest.json**

프로젝트의 `Packages/manifest.json`의 `dependencies`에 추가:

```json
{
  "com.gameframex.unity.ui.fairygui": "https://github.com/AlianBlank/com.gameframex.unity.ui.fairygui.git"
}
```

**방법 2: Unity Package Manager (Git URL)**

Unity Package Manager를 열고 Git URL에서 추가:

```
https://github.com/gameframex/com.gameframex.unity.ui.fairygui.git
```

**방법 3: 수동 설치**

리포지토리를 다운로드하여 Unity 프로젝트의 `Packages/` 디렉토리에 배치합니다. Unity가 자동으로 인식합니다.

### 의존성

| 패키지 | 버전 | 설명 |
|--------|------|------|
| `com.gameframex.unity` | ≥ 1.1.1 | 코어 프레임워크 런타임 |
| `com.gameframex.unity.ui` | ≥ 1.0.0 | 기본 UI 추상화 레이어 |
| `com.gameframex.unity.asset` | ≥ 1.0.6 | 에셋 로딩 시스템 |
| `com.gameframex.unity.event` | ≥ 1.0.0 | 이벤트 시스템 |
| FairyGUI Runtime | — | FairyGUI 라이브러리 |
| YooAsset | — | 에셋 관리 |
| UniTask | — | 비동기/await 지원 |

### 기본 사용법

1. **`FairyGUIPackageComponent`를 씬에 추가** (`GameFrameX → FairyGUIPackage` 메뉴에서)

2. **`FUI`를 상속하여 UI 패널 생성**:

```csharp
using GameFrameX.UI.FairyGUI.Runtime;

[OptionUIGroup("Default")]
public class MyPanel : FUI
{
    protected override void OnInit()
    {
        // UI 요소 초기화
    }

    protected override void OnOpen(object userData)
    {
        // 열기 로직 처리
    }

    protected override void OnClose()
    {
        // 닫기 로직 처리
    }
}
```

3. **프레임워크 UI 컴포넌트로 패널 열기**:

```csharp
// UI 패널 비동기 열기
await GameEntry.GetComponent<UIComponent>().OpenUIFormAsync("MyPackage", "MyPanel");
```

## 사용 예시

### 표시/숨기기 애니메이션

어트리뷰트로 애니메이션 설정:

```csharp
[OptionUIShowAnimation(typeof(FadeInAnimation))]
[OptionUIHideAnimation(typeof(FadeOutAnimation))]
public class AnimatedPanel : FUI { }
```

### 경로 기반 객체 검색

```csharp
// GObject의 계층 경로 가져오기
string path = gObject.GetUIPath();

// 경로에서 GObject 해석
GObject obj = FairyGUIPathFinderHelper.GetUIFromPath("GRoot/Group/MyButton");
```

### MVVM 바인딩 자동 정리

```csharp
// GObject 파괴 시 자동으로 이벤트 등록 해제
myProperty.ClearWithGObjectDestroyed(gObject);
```

## 플랫폼 지원

| 플랫폼 | 지원 |
|--------|------|
| Android | ✅ |
| iOS | ✅ |
| Windows | ✅ |
| macOS | ✅ |
| WebGL | ✅ |

최소 Unity 버전: **2019.4**

## 문서 및 자료

- [GameFrameX 문서](https://gameframex.doc.alianblank.com)
- [FairyGUI 문서](https://www.fairygui.com/docs)
- [변경 로그](CHANGELOG.md)
- [라이선스](LICENSE.md) (MIT + Apache 2.0)

## 커뮤니티 및 지원

- **QQ 그룹**: [612311526](https://jq.qq.com/?_wv=1027&k=5HXWqCg)
- **GitHub Issues**: [버그 보고](https://github.com/gameframex/com.gameframex.unity.ui.fairygui/issues)
- **작성자**: Blank ([alianblank@outlook.com](mailto:alianblank@outlook.com))

## 변경 로그

[CHANGELOG.md](CHANGELOG.md)를 참조하세요.

## 라이선스

본 프로젝트는 [MIT 라이선스](https://opensource.org/licenses/MIT) 및 [Apache 2.0 라이선스](http://www.apache.org/licenses/LICENSE-2.0) 듀얼 라이선스입니다. 자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.
