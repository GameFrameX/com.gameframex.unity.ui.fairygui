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
using GameFrameX.UI.Runtime;
using YooAsset;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// 界面管理器 - 打开功能部分类。
    /// </summary>
    /// <remarks>
    /// UI Manager - Open functionality partial class.
    /// </remarks>
    internal sealed partial class UIManager
    {
        [UnityEngine.Scripting.Preserve]
        private readonly List<UIFormLoadingObject> m_LoadingUIForms = new List<UIFormLoadingObject>(64);

        /// <summary>
        /// 异步打开界面的内部实现。
        /// </summary>
        /// <remarks>
        /// Internal implementation for opening a UI form asynchronously.
        /// </remarks>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormType">界面类型 / The UI form type</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面 / Whether to pause covered UI forms</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <param name="isFullScreen">是否全屏显示 / Whether to display in full screen</param>
        /// <returns>表示界面实例的异步任务 / A task representing the UI form instance</returns>
        [UnityEngine.Scripting.Preserve]
        protected override async Task<IUIForm> InnerOpenUIFormAsync(string uiFormAssetPath, Type uiFormType, bool pauseCoveredUIForm, object userData, bool isFullScreen = false)
        {
            var uiFormAssetName = uiFormType.Name;

            if (UseSingletonOpenMode(uiFormType))
            {
                // Singleton behavior: if already opened, return the existing instance.
                var openedUIForm = GetUIForm(uiFormAssetName);
                if (openedUIForm != null)
                {
                    RefocusUIForm(openedUIForm, userData);
                    return openedUIForm;
                }

                foreach (var value in m_LoadingUIForms)
                {
                    if (value.UIFormAssetPath == uiFormAssetPath && value.UIFormAssetName == uiFormAssetName && value.UIFormType == uiFormType)
                    {
                        return await value.Task;
                    }
                }
            }

            var uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);

            if (uiFormInstanceObject != null)
            {
                // 如果对象池存在
                return InternalOpenUIForm(-1, uiFormAssetPath, uiFormAssetName, uiFormType, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData, isFullScreen);
            }

            var uiForm = InnerLoadUIFormAsync(uiFormAssetPath, uiFormType, pauseCoveredUIForm, userData, isFullScreen);
            UIFormLoadingObject uiFormLoadingObject = UIFormLoadingObject.Create(uiFormAssetPath, uiFormAssetName, uiFormType, uiForm);
            m_LoadingUIForms.Add(uiFormLoadingObject);
            try
            {
                return await uiForm;
            }
            finally
            {
                if (m_LoadingUIForms.Remove(uiFormLoadingObject))
                {
                    ReferencePool.Release(uiFormLoadingObject);
                }
            }
        }

        /// <summary>
        /// 异步加载界面的内部实现。
        /// </summary>
        /// <remarks>
        /// Internal implementation for loading a UI form asynchronously.
        /// </remarks>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormType">界面类型 / The UI form type</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面 / Whether to pause covered UI forms</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <param name="isFullScreen">是否全屏显示 / Whether to display in full screen</param>
        /// <returns>表示界面实例的异步任务 / A task representing the UI form instance</returns>
        [UnityEngine.Scripting.Preserve]
        private async Task<IUIForm> InnerLoadUIFormAsync(string uiFormAssetPath, Type uiFormType, bool pauseCoveredUIForm, object userData, bool isFullScreen = false)
        {
            var uiFormAssetName = uiFormType.Name;
            int serialId = ++m_Serial;
            m_UIFormsBeingLoaded.Add(serialId, uiFormAssetName);
            string assetPath = PathHelper.Combine(uiFormAssetPath, uiFormAssetName);

            var lastIndexOfStart = uiFormAssetPath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            // 如果路径中没有 /，则整个路径作为包名（适用于 Resources 目录下的简单路径）
            var packageName = lastIndexOfStart >= 0 ? uiFormAssetPath.Substring(lastIndexOfStart + 1) : uiFormAssetPath;
            // 检查UI包是否已经加载过
            var hasUIPackage = FairyGuiPackage.Has(packageName);

            var openUIFormInfoData = OpenUIFormInfoData.Create(serialId, packageName, uiFormAssetName, uiFormType, pauseCoveredUIForm, userData);
            var openUIFormInfo = OpenUIFormInfo.Create(serialId, assetPath, uiFormAssetName, uiFormType, pauseCoveredUIForm, userData, isFullScreen);
            if (assetPath.IndexOf(Utility.Asset.Path.BundlesDirectoryName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                // 从Resources 中加载
                if (!hasUIPackage)
                {
                    FairyGuiPackage.AddPackageSync(assetPath);
                }

                return LoadAssetSuccessCallback(uiFormAssetPath, uiFormAssetName, openUIFormInfoData, 0, openUIFormInfo);
            }

            // 检查UI包是否已经加载过
            if (hasUIPackage)
            {
                // 如果UI 包存在则创建界面
                return LoadAssetSuccessCallback(uiFormAssetPath, uiFormAssetName, openUIFormInfoData, 1, openUIFormInfo);
            }

            if (packageName == uiFormAssetName)
            {
                // 如果UI资源名字和包名一致则直接加载
                await FairyGuiPackage.AddPackageAsync(assetPath);
            }
            else
            {
                // 不一致则重新拼接路径
                string newPackagePath = PathHelper.Combine(uiFormAssetPath, packageName);
                await FairyGuiPackage.AddPackageAsync(newPackagePath);
            }

            string newAssetPackagePath = assetPath;
            if (packageName != uiFormAssetName)
            {
                newAssetPackagePath = PathHelper.Combine(uiFormAssetPath, packageName);
            }

            newAssetPackagePath += "_fui";
            // 从包中加载
            var assetHandle = await m_AssetManager.LoadAssetAsync<UnityEngine.Object>(newAssetPackagePath);

            if (assetHandle.IsDone && assetHandle.Status == EOperationStatus.Succeed)
            {
                // 加载成功
                openUIFormInfo.SetAssetHandle(assetHandle);
                return LoadAssetSuccessCallback(assetPath, uiFormAssetName, openUIFormInfoData, assetHandle.Progress, openUIFormInfo);
            }

            // UI包不存在
            return LoadAssetFailureCallback(assetPath, uiFormAssetName, assetHandle.LastError, openUIFormInfo);
        }

        /// <summary>
        /// 内部打开界面的实现。
        /// </summary>
        /// <remarks>
        /// Internal implementation for opening a UI form.
        /// </remarks>
        /// <param name="serialId">界面序列号 / The UI form serial ID</param>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormAssetName">界面资源名称 / The UI form asset name</param>
        /// <param name="uiFormType">界面类型 / The UI form type</param>
        /// <param name="uiFormInstance">界面实例对象 / The UI form instance object</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面 / Whether to pause covered UI forms</param>
        /// <param name="isNewInstance">是否为新实例 / Whether this is a new instance</param>
        /// <param name="duration">加载持续时间 / The loading duration</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <param name="isFullScreen">是否全屏显示 / Whether to display in full screen</param>
        /// <returns>打开的界面实例 / The opened UI form instance</returns>
        [UnityEngine.Scripting.Preserve]
        private IUIForm InternalOpenUIForm(int serialId, string uiFormAssetPath, string uiFormAssetName, Type uiFormType, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData, bool isFullScreen)
        {
            try
            {
                IUIForm uiForm = m_UIFormHelper.CreateUIForm(uiFormInstance, uiFormType, userData);
                if (uiForm == null)
                {
                    throw new GameFrameworkException("Can not create UI form in UI form helper.");
                }

                var uiGroup = uiForm.UIGroup;
                if (serialId < 0)
                {
                    // 处理已加载的界面，界面复用
                    if (m_UIFormsToReleaseOnLoad.Contains(uiForm.SerialId))
                    {
                        m_UIFormsToReleaseOnLoad.Remove(uiForm.SerialId);
                    }
                }

                uiForm.Init(serialId, uiFormAssetPath, uiFormAssetName, uiGroup, OnInitAction, pauseCoveredUIForm, isNewInstance, userData, RecycleInterval, isFullScreen);

                void OnInitAction(IUIForm obj)
                {
                    if (obj is FUI fui)
                    {
                        fui.SetGObject(uiFormInstance as GObject);
                    }
                }

                if (!uiGroup.InternalHasInstanceUIForm(uiFormAssetName, uiForm))
                {
                    uiGroup.AddUIForm(uiForm);
                }

                uiForm.OnOpen(userData);
                uiForm.BindEvent();
                uiForm.LoadData();
                uiForm.UpdateLocalization();
                if (uiForm.EnableShowAnimation)
                {
                    uiForm.Show(m_UIFormShowHandler, null);
                }

                uiGroup.Refresh();

                if (m_OpenUIFormSuccessEventHandler != null)
                {
                    OpenUIFormSuccessEventArgs openUIFormSuccessEventArgs = OpenUIFormSuccessEventArgs.Create(uiForm, duration, userData);
                    m_OpenUIFormSuccessEventHandler(this, openUIFormSuccessEventArgs);
                    // ReferencePool.Release(openUIFormSuccessEventArgs);
                }

                return uiForm;
            }
            catch (Exception exception)
            {
                if (m_OpenUIFormFailureEventHandler != null)
                {
                    OpenUIFormFailureEventArgs openUIFormFailureEventArgs = OpenUIFormFailureEventArgs.Create(serialId, uiFormAssetName, pauseCoveredUIForm, exception.ToString(), userData);
                    m_OpenUIFormFailureEventHandler(this, openUIFormFailureEventArgs);
                }

                throw;
            }
        }

        /// <summary>
        /// 资源加载成功回调。
        /// </summary>
        /// <remarks>
        /// Callback for successful asset loading.
        /// </remarks>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormAssetName">界面资源名称 / The UI form asset name</param>
        /// <param name="uiFormAsset">界面资源对象 / The UI form asset object</param>
        /// <param name="duration">加载持续时间 / The loading duration</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <returns>打开的界面实例 / The opened UI form instance</returns>
        /// <exception cref="GameFrameworkException">当界面信息无效时抛出 / Thrown when UI form info is invalid</exception>
        [UnityEngine.Scripting.Preserve]
        private IUIForm LoadAssetSuccessCallback(string uiFormAssetPath, string uiFormAssetName, object uiFormAsset, float duration, object userData)
        {
            var openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            var openUIFormInfoData = (OpenUIFormInfoData)uiFormAsset;
            if (openUIFormInfoData == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                var form = GetUIForm(openUIFormInfo.SerialId);
                m_UIFormHelper.ReleaseUIForm(uiFormAsset, null, openUIFormInfo.AssetHandle, uiFormAssetPath, openUIFormInfo.AssetName);
                // This branch discards the loaded result immediately.
                // Create a temporary instance so helper can release concrete UI resources.
                var tempUIFormInstance = m_UIFormHelper.InstantiateUIForm(uiFormAsset);
                m_UIFormHelper.ReleaseUIForm(uiFormAsset, tempUIFormInstance, openUIFormInfo.AssetHandle, uiFormAssetPath, openUIFormInfo.AssetName);
                ReferencePool.Release(openUIFormInfo);
                ReferencePool.Release(openUIFormInfoData);
                return form;
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            var uiFormInstanceObject = UIFormInstanceObject.Create(uiFormAssetPath, uiFormAssetName, uiFormAsset, m_UIFormHelper.InstantiateUIForm(uiFormAsset), m_UIFormHelper, openUIFormInfo.AssetHandle);
            m_InstancePool.Register(uiFormInstanceObject, true);

            var uiForm = InternalOpenUIForm(openUIFormInfo.SerialId, uiFormAssetPath, uiFormAssetName, openUIFormInfo.FormType, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration, openUIFormInfo.UserData, openUIFormInfo.IsFullScreen);
            ReferencePool.Release(openUIFormInfo);
            ReferencePool.Release(openUIFormInfoData);
            return uiForm;
        }

        /// <summary>
        /// 资源加载失败回调。
        /// </summary>
        /// <remarks>
        /// Callback for failed asset loading.
        /// </remarks>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormAssetName">界面资源名称 / The UI form asset name</param>
        /// <param name="errorMessage">错误消息 / The error message</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <returns>已存在的界面实例（如果有）/ The existing UI form instance if any</returns>
        /// <exception cref="GameFrameworkException">当界面信息无效且无失败处理器时抛出 / Thrown when UI form info is invalid and no failure handler exists</exception>
        [UnityEngine.Scripting.Preserve]
        private IUIForm LoadAssetFailureCallback(string uiFormAssetPath, string uiFormAssetName, string errorMessage, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            try
            {
                if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
                {
                    m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                    var uiForm = GetUIForm(openUIFormInfo.SerialId);
                    return uiForm;
                }

                m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
                string appendErrorMessage = Utility.Text.Format("Load UI form failure, asset name '{0}', error message '{1}'.", uiFormAssetName, errorMessage);
                if (m_OpenUIFormFailureEventHandler != null)
                {
                    OpenUIFormFailureEventArgs openUIFormFailureEventArgs = OpenUIFormFailureEventArgs.Create(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.PauseCoveredUIForm, appendErrorMessage, openUIFormInfo.UserData);
                    m_OpenUIFormFailureEventHandler(this, openUIFormFailureEventArgs);
                    var uiForm = GetUIForm(openUIFormInfo.SerialId);
                    return uiForm;
                }

                throw new GameFrameworkException(appendErrorMessage);
            }
            finally
            {
                ReferencePool.Release(openUIFormInfo);
            }
        }
    }
}
