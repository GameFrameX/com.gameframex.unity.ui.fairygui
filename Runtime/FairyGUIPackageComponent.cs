﻿using System;
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
        private readonly Dictionary<string, UIPackageData> m_UIPackages = new Dictionary<string, UIPackageData>(32);

        /// <summary>
        /// 表示UI包的数据结构。
        /// </summary>
        [Serializable]
        public sealed class UIPackageData
        {
            /// <summary>
            /// 描述文件路径。
            /// </summary>
            public string DescFilePath { get; }

            /// <summary>
            /// 是否加载资源。
            /// </summary>
            public bool IsLoadAsset { get; }

            /// <summary>
            /// UI包实例。
            /// </summary>
            public UIPackage Package { get; private set; }

            /// <summary>
            /// 获取UI包的名称。
            /// </summary>
            public string Name
            {
                get { return Package.name; }
            }

            /// <summary>
            /// 设置UI包实例。
            /// </summary>
            /// <param name="package">UI包实例。</param>
            public void SetPackage(UIPackage package)
            {
                Package = package;
            }

            public UIPackageData(string descFilePath, bool isLoadAsset)
            {
                DescFilePath = descFilePath;
                IsLoadAsset = isLoadAsset;
            }
        }

        /// <summary>
        /// 异步添加UI包。
        /// </summary>
        /// <param name="descFilePath">描述文件路径。</param>
        /// <param name="isLoadAsset">是否加载资源，默认为true。</param>
        /// <returns>返回一个表示UI包的UniTask。</returns>
        public UniTask<UIPackage> AddPackageAsync(string descFilePath, bool isLoadAsset = true)
        {
            if (!m_UIPackages.TryGetValue(descFilePath, out var packageData))
            {
                var tcs = new UniTaskCompletionSource<UIPackage>();
                packageData = new UIPackageData(descFilePath, isLoadAsset);
                m_UIPackages[descFilePath] = packageData;

                void Complete(UIPackage uiPackage)
                {
                    packageData.SetPackage(uiPackage);
                    if (isLoadAsset)
                    {
                        packageData.Package.LoadAllAssets();
                    }

                    tcs.TrySetResult(packageData.Package);
                }

                UIPackage.AddPackageAsync(descFilePath, Complete);
                return tcs.Task;
            }

            return UniTask.FromResult(packageData.Package);
        }

        /// <summary>
        /// 同步添加UI包。
        /// </summary>
        /// <param name="descFilePath">描述文件路径。</param>
        /// <param name="isLoadAsset">是否加载资源，默认为true。</param>
        public void AddPackageSync(string descFilePath, bool isLoadAsset = true)
        {
            if (!m_UIPackages.TryGetValue(descFilePath, out var packageData))
            {
                packageData = new UIPackageData(descFilePath, isLoadAsset);

                m_UIPackages[descFilePath] = packageData;
                packageData.SetPackage(UIPackage.AddPackage(descFilePath));

                if (isLoadAsset)
                {
                    packageData.Package.LoadAllAssets();
                }
            }
        }

        /// <summary>
        /// 移除指定名称的UI包。
        /// </summary>
        /// <param name="packageName">UI包的名称。</param>
        public void RemovePackage(string packageName)
        {
            string key = null;
            UIPackageData uiPackageData = null;
            foreach (var packageData in m_UIPackages)
            {
                if (packageData.Value.Name.EqualsFast(packageName))
                {
                    key = packageData.Key;
                    uiPackageData = packageData.Value;
                    break;
                }
            }

            if (uiPackageData != null)
            {
                uiPackageData.Package.UnloadAssets();
                UIPackage.RemovePackage(packageName);
                m_UIPackages.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有UI包。
        /// </summary>
        public void RemoveAllPackages()
        {
            var packages = UIPackage.GetPackages();
            foreach (var package in packages)
            {
                package.UnloadAssets();
            }

            UIPackage.RemoveAllPackages();
            m_UIPackages.Clear();
        }

        /// <summary>
        /// 检查是否存在指定名称的UI包。
        /// </summary>
        /// <param name="uiPackageName">UI包的名称。</param>
        /// <returns>如果存在返回true，否则返回false。</returns>
        public bool Has(string uiPackageName)
        {
            return Get(uiPackageName) != null;
        }

        /// <summary>
        /// 获取指定名称的UI包。
        /// </summary>
        /// <param name="uiPackageName">UI包的名称。</param>
        /// <returns>返回UI包实例，如果不存在则返回null。</returns>
        public UIPackage Get(string uiPackageName)
        {
            foreach (var packageData in m_UIPackages)
            {
                if (packageData.Value.Name.EqualsFast(uiPackageName))
                {
                    return packageData.Value.Package;
                }
            }

            return default;
        }


        protected override void Awake()
        {
            IsAutoRegister = false;
            base.Awake();
            UIPackage.SetAsyncLoadResource(new FairyGUILoadAsyncResourceHelper());
        }
    }
}