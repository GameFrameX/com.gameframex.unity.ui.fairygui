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
using System.Reflection;
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
    [UnityEngine.Scripting.Preserve]
    public class FairyGUIFormHelper : UIFormHelperBase
    {
        private AssetComponent m_AssetComponent = null;
        private UIComponent m_UIComponent = null;

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
        /// <param name="uiFormType">界面逻辑类型</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
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
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        /// <param name="assetHandle">资源句柄。</param>
        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance, object assetHandle)
        {
            if (uiFormInstance is GComponent component)
            {
                component.RemoveFromParent();
                component.Remove();
                component.Dispose();
            }

            m_AssetComponent.UnloadAssetHandle(assetHandle);
        }

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