// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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
    [UnityEngine.Scripting.Preserve]
    internal sealed class FairyGUILoadAsyncResourceHelper : IAsyncResource
    {
        private readonly Dictionary<string, UIPackageData> m_UIPackages = new Dictionary<string, UIPackageData>(32);

        /// <summary>
        /// 释放UI包
        /// </summary>
        /// <param name="uiPackageName"></param>
        public void ReleasePackage(string uiPackageName)
        {
            if (m_UIPackages.TryGetValue(uiPackageName, out var uiPackageData))
            {
                AssetComponent.UnloadAsset(uiPackageData.DefiledAssetPath);
                if (uiPackageData.ResourceAssetPath != null)
                {
                    AssetComponent.UnloadAsset(uiPackageData.ResourceAssetPath);
                }

                uiPackageData.Dispose();
                m_UIPackages.Remove(uiPackageName);
            }
        }

        /// <summary>
        /// 释放所有UI包
        /// </summary>
        public void ReleaseAllPackage()
        {
            foreach (var kv in m_UIPackages)
            {
                AssetComponent.UnloadAsset(kv.Value.DefiledAssetPath);
                AssetComponent.UnloadAsset(kv.Value.ResourceAssetPath);
                kv.Value.Dispose();
            }

            m_UIPackages.Clear();
        }

        private AssetComponent _assetComponent;

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
        /// 加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="uiPackageName">UI包名称</param>
        /// <param name="extension">扩展名</param>
        /// <param name="type">资源类型</param>
        /// <param name="action"></param>
        public async void LoadResource(string assetName, string uiPackageName, string extension, PackageItemType type, Action<bool, string, object> action)
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
                if (uiPackageData.DefiledAssetHandle == null)
                {
                    assetHandle = await AssetComponent.LoadAssetAsync(assetName);
                    uiPackageData.SetDefiledAssetHandle(assetHandle, assetName);
                }
                else
                {
                    assetHandle = uiPackageData.DefiledAssetHandle;
                }

                action.Invoke(assetHandle != null && assetHandle.AssetObject != null, assetName, assetHandle?.GetAssetObject<TextAsset>());
                return;
            }

            if (type == PackageItemType.Image || type == PackageItemType.Atlas) //如果FGUI导出时没有选择分离通明通道，会因为加载不到!a结尾的Asset而报错，但是不影响运行
            {
                if (assetName.IndexOf("!a", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    action.Invoke(false, assetName, null);
                    return;
                }
            }

            var allAssetsHandle = await AssetComponent.LoadAllAssetsAsync(assetName);
            if (!allAssetsHandle.IsSucceed())
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
                        case PackageItemType.Image: //如果FGUI导出时没有选择分离通明通道，会因为加载不到!a结尾的Asset而报错，但是不影响运行
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

        public void ReleaseResource(object obj)
        {
        }

        sealed class UIPackageData : IDisposable
        {
            /// <summary>
            /// 包名
            /// </summary>
            public readonly string PackageName;

            public void SetResourceAllAssetsHandle(AllAssetsHandle allAssetsHandle, string assetPath)
            {
                ResourceAllAssetsHandle = allAssetsHandle;
                ResourceAssetPath = assetPath;
            }

            /// <summary>
            /// 资源包
            /// </summary>
            public AllAssetsHandle ResourceAllAssetsHandle { get; private set; }

            /// <summary>
            /// 资源包资源路径
            /// </summary>
            public string ResourceAssetPath { get; private set; }

            public void SetDefiledAssetHandle(AssetHandle defiledAssetHandle, string defiledAssetPath)
            {
                DefiledAssetHandle = defiledAssetHandle;
                DefiledAssetPath = defiledAssetPath;
            }

            /// <summary>
            /// 描述文件包
            /// </summary>
            public AssetHandle DefiledAssetHandle { get; private set; }

            /// <summary>
            /// 描述文件包资源路径
            /// </summary>
            public string DefiledAssetPath { get; private set; }

            public UIPackageData(string packageName)
            {
                PackageName = packageName;
            }

            public void Dispose()
            {
                ResourceAllAssetsHandle?.Dispose();
                DefiledAssetHandle?.Dispose();
            }
        }
    }
}