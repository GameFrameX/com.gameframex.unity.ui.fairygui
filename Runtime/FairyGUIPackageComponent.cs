using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FairyGUI;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// 管理所有UI 包
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/FairyGUIPackage")]
    [UnityEngine.Scripting.Preserve]
    public sealed class FairyGUIPackageComponent : GameFrameworkComponent
    {
        private readonly Dictionary<string, UIPackage> m_UIPackages = new Dictionary<string, UIPackage>(32);
        private readonly Dictionary<string, string> m_UIAssetPathPackages = new Dictionary<string, string>(32);

        /// <summary>
        /// 异步添加UI 包
        /// </summary>
        /// <param name="descFilePath">UI包路径</param>
        /// <param name="isLoadAsset">是否在加载描述文件之后, 加载资源</param>
        /// <returns></returns>
        public UniTask<UIPackage> AddPackageAsync(string descFilePath, bool isLoadAsset = true)
        {
            if (!m_UIPackages.TryGetValue(descFilePath, out var package))
            {
                var tcs = new UniTaskCompletionSource<UIPackage>();

                void Complete(UIPackage uiPackage)
                {
                    package = uiPackage;
                    if (isLoadAsset)
                    {
                        package.LoadAllAssets();
                    }

                    m_UIAssetPathPackages.Add(package.name, descFilePath);
                    m_UIPackages.Add(descFilePath, package);
                    tcs.TrySetResult(package);
                }

                UIPackage.AddPackageAsync(descFilePath, Complete);
                return tcs.Task;
            }

            return UniTask.FromResult(package);
        }

        /// <summary>
        /// 同步添加UI 包
        /// </summary>
        /// <param name="descFilePath">包路径</param>
        /// <param name="complete">加载完成回调</param>
        /// <param name="isLoadAsset">是否在加载描述文件之后, 加载资源</param>
        public void AddPackageSync(string descFilePath, Action<UIPackage> complete, bool isLoadAsset = true)
        {
            if (!m_UIPackages.TryGetValue(descFilePath, out var package))
            {
                void Complete(UIPackage uiPackage)
                {
                    package = uiPackage;
                    if (isLoadAsset)
                    {
                        package.LoadAllAssets();
                    }

                    m_UIAssetPathPackages.Add(package.name, descFilePath);
                    m_UIPackages.Add(descFilePath, package);
                    complete?.Invoke(package);
                }

                UIPackage.AddPackageAsync(descFilePath, Complete);
            }
        }

        /// <summary>
        /// 同步添加UI 包
        /// </summary>
        /// <param name="descFilePath">UI包路径</param>
        /// <param name="isLoadAsset">是否在加载描述文件之后, 加载资源</param>
        public void AddPackageSync(string descFilePath, bool isLoadAsset = true)
        {
            if (!m_UIPackages.TryGetValue(descFilePath, out var package))
            {
                package = UIPackage.AddPackage(descFilePath);
                if (isLoadAsset)
                {
                    package.LoadAllAssets();
                }

                m_UIAssetPathPackages.Add(package.name, descFilePath);
                m_UIPackages.Add(descFilePath, package);
            }
        }

        /// <summary>
        /// 移除UI 包
        /// </summary>
        /// <param name="packageName">UI包路径</param>
        public void RemovePackage(string packageName)
        {
            if (m_UIAssetPathPackages.TryGetValue(packageName, out var value))
            {
                UIPackage.RemovePackage(packageName);
                m_UIAssetPathPackages.Remove(packageName);
                m_UIPackages.Remove(value);
            }
        }

        /// <summary>
        /// 移除所有UI 包
        /// </summary>
        public void RemoveAllPackages()
        {
            UIPackage.RemoveAllPackages();
            m_UIPackages.Clear();
            m_UIAssetPathPackages.Clear();
        }


        /// <summary>
        /// 是否包含UI 包
        /// </summary>
        /// <param name="uiPackageName">UI包名称</param>
        /// <returns></returns>
        public bool Has(string uiPackageName)
        {
            return Get(uiPackageName) != null;
        }

        /// <summary>
        /// 获取UI 包
        /// </summary>
        /// <param name="uiPackageName">UI包名称</param>
        /// <returns></returns>
        public UIPackage Get(string uiPackageName)
        {
            if (m_UIAssetPathPackages.TryGetValue(uiPackageName, out var value) && m_UIPackages.TryGetValue(value, out var package))
            {
                return package;
            }

            return null;
        }

        protected override void Awake()
        {
            IsAutoRegister = false;
            base.Awake();
            UIPackage.SetAsyncLoadResource(new FairyGUILoadAsyncResourceHelper());
        }
    }
}