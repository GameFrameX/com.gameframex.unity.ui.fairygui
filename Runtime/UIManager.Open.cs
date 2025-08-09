//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FairyGUI;
using GameFrameX.Asset.Runtime;
using GameFrameX.ObjectPool;
using GameFrameX.Runtime;
using GameFrameX.UI.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// 界面管理器。
    /// </summary>
    internal sealed partial class UIManager
    {
        protected override async Task<IUIForm> InnerOpenUIFormAsync(string uiFormAssetPath, Type uiFormType, bool pauseCoveredUIForm, object userData, bool isFullScreen = false, bool isMultiple = false)
        {
            var uiFormAssetName = uiFormType.Name;
            UIFormInstanceObject uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);

            if (uiFormInstanceObject != null && isMultiple == false)
            {
                // 如果对象池存在
                return InternalOpenUIForm(-1, uiFormAssetName, uiFormType, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData, isFullScreen);
            }

            int serialId = ++m_Serial;
            m_UIFormsBeingLoaded.Add(serialId, uiFormAssetName);
            string assetPath = PathHelper.Combine(uiFormAssetPath, uiFormAssetName);

            var lastIndexOfStart = uiFormAssetPath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            var packageName = uiFormAssetPath.Substring(lastIndexOfStart + 1);
            // 检查UI包是否已经加载过
            var hasUIPackage = FairyGuiPackage.Has(packageName);

            OpenUIFormInfoData openUIFormInfoData = OpenUIFormInfoData.Create(serialId, packageName, uiFormAssetName, uiFormType, pauseCoveredUIForm, userData);
            OpenUIFormInfo openUIFormInfo = OpenUIFormInfo.Create(serialId, uiFormType, pauseCoveredUIForm, userData, isFullScreen);
            if (assetPath.IndexOf(Utility.Asset.Path.BundlesDirectoryName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                // 从Resources 中加载
                if (!hasUIPackage)
                {
                    FairyGuiPackage.AddPackageSync(assetPath);
                }

                return LoadAssetSuccessCallback(uiFormAssetName, openUIFormInfoData, 0, openUIFormInfo);
            }

            // 检查UI包是否已经加载过
            if (hasUIPackage)
            {
                // 如果UI 包存在则创建界面
                return LoadAssetSuccessCallback(uiFormAssetName, openUIFormInfoData, 1, openUIFormInfo);
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

            if (assetHandle.IsSucceed)
            {
                // 加载成功
                return LoadAssetSuccessCallback(uiFormAssetName, openUIFormInfoData, assetHandle.Progress, openUIFormInfo);
            }

            // UI包不存在
            return LoadAssetFailureCallback(uiFormAssetName, assetHandle.LastError, openUIFormInfo);
        }

        private IUIForm InternalOpenUIForm(int serialId, string uiFormAssetName, Type uiFormType, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData, bool isFullScreen)
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
                    if (m_UIFormsToReleaseOnLoad.Contains(uiForm.SerialId))
                    {
                        m_UIFormsToReleaseOnLoad.Remove(uiForm.SerialId);
                    }
                }

                uiForm.Init(serialId, uiFormAssetName, uiGroup, OnInitAction, pauseCoveredUIForm, isNewInstance, userData, isFullScreen);

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
                    return GetUIForm(openUIFormFailureEventArgs.SerialId);
                }

                throw;
            }
        }

        private IUIForm LoadAssetSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
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
                ReferencePool.Release(openUIFormInfo);
                m_UIFormHelper.ReleaseUIForm(uiFormAsset, null);
                return GetUIForm(openUIFormInfo.SerialId);
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            UIFormInstanceObject uiFormInstanceObject = UIFormInstanceObject.Create(uiFormAssetName, uiFormAsset, m_UIFormHelper.InstantiateUIForm(uiFormAsset), m_UIFormHelper);
            m_InstancePool.Register(uiFormInstanceObject, true);

            var uiForm = InternalOpenUIForm(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.FormType, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration, openUIFormInfo.UserData, openUIFormInfo.IsFullScreen);
            ReferencePool.Release(openUIFormInfo);
            ReferencePool.Release(openUIFormInfoData);
            return uiForm;
        }

        private IUIForm LoadAssetFailureCallback(string uiFormAssetName, string errorMessage, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new GameFrameworkException("Open UI form info is invalid.");
            }

            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                return GetUIForm(openUIFormInfo.SerialId);
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            string appendErrorMessage = Utility.Text.Format("Load UI form failure, asset name '{0}', error message '{2}'.", uiFormAssetName, errorMessage);
            if (m_OpenUIFormFailureEventHandler != null)
            {
                OpenUIFormFailureEventArgs openUIFormFailureEventArgs = OpenUIFormFailureEventArgs.Create(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.PauseCoveredUIForm, appendErrorMessage, openUIFormInfo.UserData);
                m_OpenUIFormFailureEventHandler(this, openUIFormFailureEventArgs);
                return GetUIForm(openUIFormInfo.SerialId);
            }

            throw new GameFrameworkException(appendErrorMessage);
        }
    }
}