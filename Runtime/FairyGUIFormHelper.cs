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
using System.Reflection;
using FairyGUI;
using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using GameFrameX.UI.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI 界面辅助器，提供界面实例化、创建和释放功能。
    /// </summary>
    /// <remarks>
    /// FairyGUI form helper that provides form instantiation, creation, and release functionality.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public class FairyGUIFormHelper : UIFormHelperBase
    {
        [UnityEngine.Scripting.Preserve]
        private AssetComponent m_AssetComponent = null;
        [UnityEngine.Scripting.Preserve]
        private UIComponent m_UIComponent = null;

        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <remarks>
        /// Instantiates a UI form.
        /// </remarks>
        /// <param name="uiFormAsset">要实例化的界面资源数据 / The UI form asset data to instantiate</param>
        /// <returns>实例化后的界面对象 / The instantiated UI form object</returns>
        [UnityEngine.Scripting.Preserve]
        public override object InstantiateUIForm(object uiFormAsset)
        {
            var openUIFormInfoData = (OpenUIFormInfoData)uiFormAsset;
            GameFrameworkGuard.NotNull(openUIFormInfoData, nameof(uiFormAsset));

            return UIPackage.CreateObject(openUIFormInfoData.PackageName, openUIFormInfoData.UIName);
        }

        /// <summary>
        /// 创建界面。
        /// </summary>
        /// <remarks>
        /// Creates a UI form instance.
        /// </remarks>
        /// <param name="uiFormInstance">界面实例对象 / The UI form instance object</param>
        /// <param name="uiFormType">界面逻辑类型 / The UI form logic type</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <returns>创建的界面实例 / The created UI form instance</returns>
        [UnityEngine.Scripting.Preserve]
        public override IUIForm CreateUIForm(object uiFormInstance, Type uiFormType, object userData)
        {
            GComponent component = uiFormInstance as GComponent;
            if (component == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            var componentType = component.displayObject.gameObject.GetOrAddComponent(uiFormType);
            var uiForm = componentType as IUIForm;
            if (uiForm == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            if (uiForm.IsAwake == false)
            {
                uiForm.OnAwake();
            }

            var uiGroup = uiForm.UIGroup;

            if (uiGroup == null)
            {
                var attribute = uiFormType.GetCustomAttribute(typeof(OptionUIGroupAttribute));
                if (attribute is OptionUIGroupAttribute optionUIGroup)
                {
                    uiGroup = m_UIComponent.GetUIGroup(optionUIGroup.GroupName);
                }

                uiForm.UIGroup = uiGroup;
            }

            var showAnimationAttribute = uiFormType.GetCustomAttribute(typeof(OptionUIShowAnimationAttribute));
            if (showAnimationAttribute is OptionUIShowAnimationAttribute optionShowAnimation)
            {
                uiForm.EnableShowAnimation = optionShowAnimation.Enable;
                uiForm.ShowAnimationName = optionShowAnimation.AnimationName;
            }
            else
            {
                uiForm.EnableShowAnimation = m_UIComponent.IsEnableUIShowAnimation;
            }

            var hideAnimationAttribute = uiFormType.GetCustomAttribute(typeof(OptionUIHideAnimationAttribute));
            if (hideAnimationAttribute is OptionUIHideAnimationAttribute optionHideAnimation)
            {
                uiForm.EnableHideAnimation = optionHideAnimation.Enable;
                uiForm.HideAnimationName = optionHideAnimation.AnimationName;
            }
            else
            {
                uiForm.EnableHideAnimation = m_UIComponent.IsEnableUIHideAnimation;
            }

            if (uiGroup == null)
            {
                Log.Error("UI group is invalid.");
                return null;
            }

            DisplayObjectInfo displayObjectInfo = ((MonoBehaviour)uiGroup.Helper).gameObject.GetComponent<DisplayObjectInfo>();
            if (displayObjectInfo == null)
            {
                Log.Error("Display object info is invalid.");
                return null;
            }

            var uiGroupComponent = displayObjectInfo.displayObject.gOwner as GComponent;
            if (uiGroupComponent == null)
            {
                Log.Error("UI group component is invalid.");
                return null;
            }

            uiGroupComponent.AddChild(component);
            return uiForm;
        }

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <remarks>
        /// Releases a UI form.
        /// </remarks>
        /// <param name="uiFormAsset">要释放的界面资源 / The UI form asset to release</param>
        /// <param name="uiFormInstance">要释放的界面实例 / The UI form instance to release</param>
        /// <param name="assetHandle">资源句柄 / The asset handle</param>
        /// <param name="uiFormAssetPath">界面资源路径 / The UI form asset path</param>
        /// <param name="uiFormAssetName">界面资源名称 / The UI form asset name</param>
        [UnityEngine.Scripting.Preserve]
        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance, object assetHandle, string uiFormAssetPath, string uiFormAssetName)
        {
            if (!m_UIComponent.EnableAutoReleaseUIForm)
            {
                return;
            }

            if (uiFormInstance is GComponent component)
            {
                component.RemoveFromParent();
                component.Remove();
                component.Dispose();
            }

            if (uiFormAssetPath.IndexOf(Utility.Asset.Path.BundlesDirectoryName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (assetHandle is YooAsset.AssetHandle handle)
                {
                    handle.Release();
                }

                m_AssetComponent.UnloadAsset(uiFormAssetPath);
            }
            else
            {
                // Resources 资源卸载：仅当实例非 GComponent 时才需要销毁 Unity 对象
                if (!(uiFormInstance is GComponent) && uiFormInstance is UnityEngine.Object unityObject)
                {
                    Destroy(unityObject);
                    UnityEngine.Resources.UnloadAsset(unityObject);
                }
            }
        }

        /// <summary>
        /// 组件唤醒时初始化资源组件和 UI 组件引用。
        /// </summary>
        /// <remarks>
        /// Initializes asset component and UI component references when the component awakens.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        private void Awake()
        {
            m_AssetComponent = GameEntry.GetComponent<AssetComponent>();
            if (m_AssetComponent == null)
            {
                Log.Fatal("Asset component is invalid.");
                return;
            }

            m_UIComponent = GameEntry.GetComponent<UIComponent>();
            if (m_UIComponent == null)
            {
                Log.Fatal("UI component is invalid.");
                return;
            }
        }
    }
}