﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using FairyGUI;
using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using GameFrameX.UI.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// 默认界面辅助器。
    /// </summary>
    public class FairyGUIFormHelper : UIFormHelperBase
    {
        private AssetComponent m_AssetComponent = null;

        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源。</param>
        /// <returns>实例化后的界面。</returns>
        public override object InstantiateUIForm(object uiFormAsset)
        {
            var openUIFormInfoData = (OpenUIFormInfoData)uiFormAsset;
            GameFrameworkGuard.NotNull(openUIFormInfoData, nameof(uiFormAsset));

            return UIPackage.CreateObject(openUIFormInfoData.PackageName, openUIFormInfoData.UIName);
        }

        /// <summary>
        /// 创建界面。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            GComponent component = uiFormInstance as GComponent;
            if (component == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            DisplayObjectInfo displayObjectInfo = ((MonoBehaviour)uiGroup.Helper).gameObject.GetComponent<DisplayObjectInfo>();
            if (displayObjectInfo == null)
            {
                Log.Error("UI group is invalid.");
                return null;
            }

            var uiGroupComponent = displayObjectInfo.displayObject.gOwner as GComponent;
            if (uiGroupComponent == null)
            {
                Log.Error("UI group is invalid.");
                return null;
            }

            uiGroupComponent.AddChild(component);

            return component.displayObject.gameObject.GetOrAddComponent<UIForm>();
        }

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            // m_AssetComponent.UnloadAsset(uiFormAsset);
            Destroy((Object)uiFormInstance);
        }

        private void Start()
        {
            m_AssetComponent = GameEntry.GetComponent<AssetComponent>();
            if (m_AssetComponent == null)
            {
                Log.Fatal("Asset component is invalid.");
                return;
            }
        }
    }
}