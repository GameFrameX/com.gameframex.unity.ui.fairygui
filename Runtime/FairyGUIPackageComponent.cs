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
using System.Threading.Tasks;
using FairyGUI;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI 包管理组件，负责管理所有 UI 包的加载、缓存和卸载。
    /// </summary>
    /// <remarks>
    /// FairyGUI package management component responsible for managing the loading, caching, and unloading of all UI packages.
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("GameFrameX/FairyGUIPackage")]
    [UnityEngine.Scripting.Preserve]
    public sealed class FairyGUIPackageComponent : GameFrameworkComponent
    {
        [UnityEngine.Scripting.Preserve]
        private readonly Dictionary<string, UIPackageData> m_UILoadedPackages = new Dictionary<string, UIPackageData>(32);

        [UnityEngine.Scripting.Preserve]
        private readonly Dictionary<string, TaskCompletionSource<UIPackage>> m_UIPackageLoading = new Dictionary<string, TaskCompletionSource<UIPackage>>(32);
        [UnityEngine.Scripting.Preserve]
        FairyGUILoadAsyncResourceHelper resourceHelper;

        /// <summary>
        /// 表示 UI 包的数据结构。
        /// </summary>
        /// <remarks>
        /// Data structure representing a UI package.
        /// </remarks>
        [Serializable]
        public sealed class UIPackageData
        {
            /// <summary>
            /// 获取描述文件路径。
            /// </summary>
            /// <remarks>
            /// Gets the description file path.
            /// </remarks>
            /// <value>描述文件路径 / The description file path</value>
            [UnityEngine.Scripting.Preserve]
            public string DescFilePath { get; }

            /// <summary>
            /// 获取是否加载资源。
            /// </summary>
            /// <remarks>
            /// Gets whether to load assets.
            /// </remarks>
            /// <value>如果加载资源则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if loading assets; otherwise <c>false</c></value>
            [UnityEngine.Scripting.Preserve]
            public bool IsLoadAsset { get; }

            /// <summary>
            /// 获取 UI 包实例。
            /// </summary>
            /// <remarks>
            /// Gets the UI package instance.
            /// </remarks>
            /// <value>UI 包实例 / The UI package instance</value>
            [UnityEngine.Scripting.Preserve]
            public UIPackage Package { get; private set; }

            /// <summary>
            /// 获取 UI 包的名称。
            /// </summary>
            /// <remarks>
            /// Gets the name of the UI package.
            /// </remarks>
            /// <value>UI 包名称 / The UI package name</value>
            [UnityEngine.Scripting.Preserve]
            public string Name { get; private set; }

            /// <summary>
            /// 设置 UI 包实例。
            /// </summary>
            /// <remarks>
            /// Sets the UI package instance.
            /// </remarks>
            /// <param name="package">UI 包实例 / The UI package instance</param>
            [UnityEngine.Scripting.Preserve]
            public void SetPackage(UIPackage package)
            {
                Package = package;
            }

            /// <summary>
            /// 初始化 <see cref="UIPackageData"/> 类的新实例。
            /// </summary>
            /// <remarks>
            /// Initializes a new instance of the <see cref="UIPackageData"/> class.
            /// </remarks>
            /// <param name="descFilePath">描述文件路径 / The description file path</param>
            /// <param name="name">UI 包名称 / The UI package name</param>
            /// <param name="isLoadAsset">是否加载资源 / Whether to load assets</param>
            [UnityEngine.Scripting.Preserve]
            public UIPackageData(string descFilePath, string name, bool isLoadAsset)
            {
                DescFilePath = descFilePath;
                IsLoadAsset = isLoadAsset;
                Name = name;
            }
        }

        /// <summary>
        /// 异步添加 UI 包。
        /// </summary>
        /// <remarks>
        /// Asynchronously adds a UI package.
        /// </remarks>
        /// <param name="descFilePath">描述文件路径 / The description file path</param>
        /// <param name="isLoadAsset">是否加载资源，默认为 <c>true</c> / Whether to load assets, defaults to <c>true</c></param>
        /// <returns>表示 UI 包的异步任务 / A task representing the UI package</returns>
        [UnityEngine.Scripting.Preserve]
        public Task<UIPackage> AddPackageAsync(string descFilePath, bool isLoadAsset = true)
        {
            if (m_UIPackageLoading.TryGetValue(descFilePath, out var tcsLoading))
            {
                return tcsLoading.Task;
            }

            if (!m_UILoadedPackages.TryGetValue(descFilePath, out var packageData))
            {
                var tcs = new TaskCompletionSource<UIPackage>();

                void Complete(UIPackage uiPackage)
                {
                    m_UIPackageLoading.Remove(descFilePath);
                    packageData = new UIPackageData(descFilePath, uiPackage.name, isLoadAsset);
                    packageData.SetPackage(uiPackage);
                    if (isLoadAsset)
                    {
                        packageData.Package.LoadAllAssets();
                    }

                    m_UILoadedPackages[descFilePath] = packageData;
                    tcs.TrySetResult(packageData.Package);
                }

                m_UIPackageLoading[descFilePath] = tcs;
                UIPackage.AddPackageAsync(descFilePath, Complete);
                return tcs.Task;
            }

            return Task.FromResult(packageData.Package);
        }

        /// <summary>
        /// 同步添加 UI 包。
        /// </summary>
        /// <remarks>
        /// Synchronously adds a UI package.
        /// </remarks>
        /// <param name="descFilePath">描述文件路径 / The description file path</param>
        /// <param name="isLoadAsset">是否加载资源，默认为 <c>true</c> / Whether to load assets, defaults to <c>true</c></param>
        [UnityEngine.Scripting.Preserve]
        public void AddPackageSync(string descFilePath, bool isLoadAsset = true)
        {
            if (!m_UILoadedPackages.TryGetValue(descFilePath, out var packageData))
            {
                var package = UIPackage.AddPackage(descFilePath);
                packageData = new UIPackageData(descFilePath, package.name, isLoadAsset);
                packageData.SetPackage(package);
                m_UILoadedPackages[descFilePath] = packageData;
                if (isLoadAsset)
                {
                    packageData.Package.LoadAllAssets();
                }
            }
        }

        /// <summary>
        /// 移除指定名称的 UI 包。
        /// </summary>
        /// <remarks>
        /// Removes the UI package with the specified name.
        /// </remarks>
        /// <param name="packageName">UI 包的名称 / The name of the UI package</param>
        [UnityEngine.Scripting.Preserve]
        public void RemovePackage(string packageName)
        {
            string key = null;
            UIPackageData uiPackageData = null;
            foreach (var packageData in m_UILoadedPackages)
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
                resourceHelper.ReleasePackage(packageName);
                m_UILoadedPackages.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有 UI 包。
        /// </summary>
        /// <remarks>
        /// Removes all UI packages.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void RemoveAllPackages()
        {
            var packages = UIPackage.GetPackages();
            foreach (var package in packages)
            {
                package.UnloadAssets();
            }

            resourceHelper.ReleaseAllPackage();
            UIPackage.RemoveAllPackages();
            m_UILoadedPackages.Clear();
        }

        /// <summary>
        /// 检查是否存在指定名称的 UI 包。
        /// </summary>
        /// <remarks>
        /// Checks whether a UI package with the specified name exists.
        /// </remarks>
        /// <param name="uiPackageName">UI 包的名称 / The name of the UI package</param>
        /// <returns>如果存在则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if exists; otherwise <c>false</c></returns>
        [UnityEngine.Scripting.Preserve]
        public bool Has(string uiPackageName)
        {
            return Get(uiPackageName) != null;
        }

        /// <summary>
        /// 获取指定名称的 UI 包。
        /// </summary>
        /// <remarks>
        /// Gets the UI package with the specified name.
        /// </remarks>
        /// <param name="uiPackageName">UI 包的名称 / The name of the UI package</param>
        /// <returns>UI 包实例；如果不存在则返回 <c>null</c> / The UI package instance; <c>null</c> if not found</returns>
        [UnityEngine.Scripting.Preserve]
        public UIPackage Get(string uiPackageName)
        {
            foreach (var packageData in m_UILoadedPackages)
            {
                if (packageData.Value.Name.EqualsFast(uiPackageName))
                {
                    return packageData.Value.Package;
                }
            }

            return default;
        }


        /// <summary>
        /// 组件唤醒时初始化异步资源加载辅助器。
        /// </summary>
        /// <remarks>
        /// Initializes the async resource loading helper when the component awakens.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        protected override void Awake()
        {
            IsAutoRegister = false;
            base.Awake();
            resourceHelper = new FairyGUILoadAsyncResourceHelper();
            UIPackage.SetAsyncLoadResource(resourceHelper);
        }
    }
}
