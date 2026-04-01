// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using UnityEngine;
using YooAsset;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI 异步资源加载辅助器，实现 IAsyncResource 接口以支持异步资源加载。
    /// </summary>
    /// <remarks>
    /// FairyGUI async resource loading helper that implements IAsyncResource interface to support asynchronous resource loading.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    internal sealed class FairyGUILoadAsyncResourceHelper : IAsyncResource
    {
        [UnityEngine.Scripting.Preserve]
        private readonly Dictionary<string, UIPackageData> m_UIPackages = new Dictionary<string, UIPackageData>(32);

        /// <summary>
        /// 释放指定名称的 UI 包。
        /// </summary>
        /// <remarks>
        /// Releases the UI package with the specified name.
        /// </remarks>
        /// <param name="uiPackageName">要释放的 UI 包名称 / The name of the UI package to release</param>
        [UnityEngine.Scripting.Preserve]
        public void ReleasePackage(string uiPackageName)
        {
            if (m_UIPackages.TryGetValue(uiPackageName, out var uiPackageData))
            {
                AssetComponent.UnloadAsset(uiPackageData.DescAssetPath);
                if (uiPackageData.ResourceAssetPath != null)
                {
                    AssetComponent.UnloadAsset(uiPackageData.ResourceAssetPath);
                }

                uiPackageData.Dispose();
                m_UIPackages.Remove(uiPackageName);
            }
        }

        /// <summary>
        /// 释放所有已加载的 UI 包。
        /// </summary>
        /// <remarks>
        /// Releases all loaded UI packages.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void ReleaseAllPackage()
        {
            foreach (var kv in m_UIPackages)
            {
                AssetComponent.UnloadAsset(kv.Value.DescAssetPath);
                AssetComponent.UnloadAsset(kv.Value.ResourceAssetPath);
                kv.Value.Dispose();
            }

            m_UIPackages.Clear();
        }

        [UnityEngine.Scripting.Preserve]
        private AssetComponent _assetComponent;

        [UnityEngine.Scripting.Preserve]
        private AssetComponent AssetComponent
        {
            get
            {
                if (_assetComponent == null)
                {
                    _assetComponent = GameEntry.GetComponent<AssetComponent>();
                }

                return _assetComponent;
            }
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <remarks>
        /// Asynchronously loads a resource.
        /// </remarks>
        /// <param name="assetName">资源名称 / The asset name</param>
        /// <param name="uiPackageName">UI 包名称 / The UI package name</param>
        /// <param name="extension">文件扩展名 / The file extension</param>
        /// <param name="type">资源包项目类型 / The package item type</param>
        /// <param name="action">加载完成回调，参数为是否成功、资源名称、资源对象 / The load completion callback with success flag, asset name, and asset object</param>
        [UnityEngine.Scripting.Preserve]
        public async void LoadResource(string assetName, string uiPackageName, string extension, PackageItemType type, Action<bool, string, object> action)
        {
            try
            {
                if (!m_UIPackages.TryGetValue(uiPackageName, out var uiPackageData))
                {
                    uiPackageData = new UIPackageData(uiPackageName);
                    m_UIPackages.Add(uiPackageName, uiPackageData);
                }

                if (type == PackageItemType.Misc)
            {
                // 描述文件
                AssetHandle assetHandle;
                if (uiPackageData.DescAssetHandle == null)
                {
                    assetHandle = await AssetComponent.LoadAssetAsync<UnityEngine.Object>(assetName);
                    uiPackageData.SetDescAssetHandle(assetHandle, assetName);
                }
                else
                {
                    assetHandle = uiPackageData.DescAssetHandle;
                }

                action.Invoke(assetHandle != null && assetHandle.AssetObject != null, assetName, assetHandle?.GetAssetObject<TextAsset>());
                return;
            }

            if (type == PackageItemType.Image || type == PackageItemType.Atlas) //如果FGUI导出时没有选择分离透明通道，会因为加载不到!a结尾的Asset而报错，但是不影响运行
            {
                if (assetName.IndexOf("!a", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    action.Invoke(false, assetName, null);
                    return;
                }
            }

            var allAssetsHandle = await AssetComponent.LoadAllAssetsAsync<UnityEngine.Object>(assetName);
            if (!(allAssetsHandle.IsDone && allAssetsHandle.Status == EOperationStatus.Succeed))
            {
                action.Invoke(false, assetName, null);
                return;
            }

            uiPackageData.SetResourceAllAssetsHandle(allAssetsHandle, assetName);
            if (uiPackageData.ResourceAllAssetsHandle == null)
            {
                action.Invoke(false, assetName, null);
                return;
            }

            string assetShortName = Path.GetFileNameWithoutExtension(assetName);
            foreach (var assetObject in uiPackageData.ResourceAllAssetsHandle.AllAssetObjects)
            {
                if (assetObject.name == assetShortName)
                {
                    switch (type)
                    {
                        case PackageItemType.Spine:
                        {
                            action.Invoke(true, assetName, assetObject as TextAsset);
                            break;
                        }

                        case PackageItemType.Atlas:
                        case PackageItemType.Image: //如果FGUI导出时没有选择分离透明通道，会因为加载不到!a结尾的Asset而报错，但是不影响运行
                        {
                            if (assetName.IndexOf("!a", StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                action.Invoke(false, assetName, null);
                                break;
                            }

                            action.Invoke(true, assetName, assetObject as Texture);
                            break;
                        }
                        case PackageItemType.Sound:
                        {
                            action.Invoke(true, assetName, assetObject as AudioClip);
                            break;
                        }
                        case PackageItemType.Font:
                        {
                            action.Invoke(true, assetName, assetObject as Font);
                        }
                            break;
//                 case PackageItemType.Spine:
//                 {
// #if FAIRYGUI_SPINE
//                     var assetHandle = await assetComponent.LoadAssetAsync<Spine.Unity.SkeletonDataAsset>(assetName);
//                     action.Invoke(assetHandle != null && assetHandle.AssetObject != null, assetName, assetHandle?.GetAssetObject<Spine.Unity.SkeletonDataAsset>());
// #else
//                             Log.Error("加载资源失败.暂未适配 Unknown file type: " + assetName + " extension: " + extension);
//                             action.Invoke(false, assetName, null);
// #endif
//                 }
                        // break;
                        case PackageItemType.DragoneBones:
                        {
#if FAIRYGUI_DRAGONBONES
                    var assetHandle = @await AssetComponent.LoadAssetAsync<DragonBones.DragonBonesData>(assetName);
                    action.Invoke(assetHandle != null && assetHandle.AssetObject != null, assetName, assetHandle?.GetAssetObject<DragonBones.DragonBonesData>());
#else
                            Log.Error("加载资源失败.暂未适配 Unknown file type: " + assetName + " extension: " + extension);
                            action.Invoke(false, assetName, null);
#endif
                        }
                            break;
                        default:
                        {
                            Log.Error("加载资源失败 Unknown file type: " + assetName + " extension: " + extension);
                            action.Invoke(false, assetName, null);
                            break;
                        }
                    }

                    return;
                }
            }

                Log.Error("加载资源失败 Unknown file type: " + assetName + " extension: " + extension);
                action.Invoke(false, assetName, null);
            }
            catch (Exception ex)
            {
                Log.Error($"LoadResource failed for '{assetName}': {ex}");
                action?.Invoke(false, assetName, null);
            }
        }

        /// <summary>
        /// 释放资源（当前实现为空）。
        /// </summary>
        /// <remarks>
        /// Releases a resource (current implementation is empty).
        /// </remarks>
        /// <param name="obj">要释放的资源对象 / The resource object to release</param>
        [UnityEngine.Scripting.Preserve]
        public void ReleaseResource(object obj)
        {
        }

        /// <summary>
        /// UI 包数据类，管理单个 UI 包的资源句柄信息。
        /// </summary>
        /// <remarks>
        /// UI package data class that manages resource handle information for a single UI package.
        /// </remarks>
        sealed class UIPackageData : IDisposable
        {
            /// <summary>
            /// 获取 UI 包名称。
            /// </summary>
            /// <remarks>
            /// Gets the UI package name.
            /// </remarks>
            /// <value>UI 包名称 / The UI package name</value>
            [UnityEngine.Scripting.Preserve]
            public readonly string PackageName;

            /// <summary>
            /// 设置资源包句柄及其路径。
            /// </summary>
            /// <remarks>
            /// Sets the resource all assets handle and its path.
            /// </remarks>
            /// <param name="allAssetsHandle">资源包句柄 / The all assets handle</param>
            /// <param name="assetPath">资源路径 / The asset path</param>
            [UnityEngine.Scripting.Preserve]
            public void SetResourceAllAssetsHandle(AllAssetsHandle allAssetsHandle, string assetPath)
            {
                ResourceAllAssetsHandle = allAssetsHandle;
                ResourceAssetPath = assetPath;
            }

            /// <summary>
            /// 获取资源包句柄。
            /// </summary>
            /// <remarks>
            /// Gets the resource all assets handle.
            /// </remarks>
            /// <value>资源包句柄 / The all assets handle</value>
            [UnityEngine.Scripting.Preserve]
            public AllAssetsHandle ResourceAllAssetsHandle { get; private set; }

            /// <summary>
            /// 获取资源包资源路径。
            /// </summary>
            /// <remarks>
            /// Gets the resource asset path.
            /// </remarks>
            /// <value>资源路径 / The resource asset path</value>
            [UnityEngine.Scripting.Preserve]
            public string ResourceAssetPath { get; private set; }

            /// <summary>
            /// 设置描述文件句柄及其路径。
            /// </summary>
            /// <remarks>
            /// Sets the definition asset handle and its path.
            /// </remarks>
            /// <param name="descAssetHandle">描述文件句柄 / The definition asset handle</param>
            /// <param name="descAssetPath">描述文件路径 / The definition asset path</param>
            [UnityEngine.Scripting.Preserve]
            public void SetDescAssetHandle(AssetHandle descAssetHandle, string descAssetPath)
            {
                DescAssetHandle = descAssetHandle;
                DescAssetPath = descAssetPath;
            }

            /// <summary>
            /// 获取描述文件句柄。
            /// </summary>
            /// <remarks>
            /// Gets the definition asset handle.
            /// </remarks>
            /// <value>描述文件句柄 / The definition asset handle</value>
            [UnityEngine.Scripting.Preserve]
            public AssetHandle DescAssetHandle { get; private set; }

            /// <summary>
            /// 获取描述文件资源路径。
            /// </summary>
            /// <remarks>
            /// Gets the definition asset path.
            /// </remarks>
            /// <value>描述文件路径 / The definition asset path</value>
            [UnityEngine.Scripting.Preserve]
            public string DescAssetPath { get; private set; }

            /// <summary>
            /// 初始化 <see cref="UIPackageData"/> 类的新实例。
            /// </summary>
            /// <remarks>
            /// Initializes a new instance of the <see cref="UIPackageData"/> class.
            /// </remarks>
            /// <param name="packageName">UI 包名称 / The UI package name</param>
            [UnityEngine.Scripting.Preserve]
            public UIPackageData(string packageName)
            {
                PackageName = packageName;
            }

            /// <summary>
            /// 释放资源。
            /// </summary>
            /// <remarks>
            /// Releases resources.
            /// </remarks>
            [UnityEngine.Scripting.Preserve]
            public void Dispose()
            {
                ResourceAllAssetsHandle?.Dispose();
                DescAssetHandle?.Dispose();
            }
        }
    }
}
