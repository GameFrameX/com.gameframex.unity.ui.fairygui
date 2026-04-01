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
using FairyGUI;
using GameFrameX.UI.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI 界面基类，提供 FairyGUI 特定的界面功能实现。
    /// </summary>
    /// <remarks>
    /// Base class for FairyGUI forms, providing FairyGUI-specific form functionality implementation.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public class FUI : UIForm
    {
        /// <summary>
        /// 获取关联的 FairyGUI GObject 对象。
        /// </summary>
        /// <remarks>
        /// Gets the associated FairyGUI GObject instance.
        /// </remarks>
        /// <value>FairyGUI GObject 对象 / The FairyGUI GObject instance</value>
        [UnityEngine.Scripting.Preserve]
        public GObject GObject { get; private set; }

        /// <summary>
        /// 获取或设置界面可见性改变事件的回调。
        /// </summary>
        /// <remarks>
        /// Gets or sets the callback for visibility changed event.
        /// </remarks>
        /// <value>可见性改变回调，参数为当前可见状态 / Visibility changed callback with current visibility state as parameter</value>
        [UnityEngine.Scripting.Preserve]
        public Action<bool> OnVisibleChanged { get; set; }

        /// <summary>
        /// 显示界面。
        /// </summary>
        /// <remarks>
        /// Shows the UI form.
        /// </remarks>
        /// <param name="handler">界面显示处理器接口 / The UI form show handler interface</param>
        /// <param name="complete">显示完成后的回调函数 / Callback function invoked after showing is complete</param>
        [UnityEngine.Scripting.Preserve]
        public override void Show(IUIFormShowHandler handler, Action complete)
        {
            if (handler != null)
            {
                handler.Handler(GObject, EnableShowAnimation, ShowAnimationName, complete);
            }
            else
            {
                complete?.Invoke();
            }
        }

        /// <summary>
        /// 隐藏界面。
        /// </summary>
        /// <remarks>
        /// Hides the UI form.
        /// </remarks>
        /// <param name="handler">界面隐藏处理器接口 / The UI form hide handler interface</param>
        /// <param name="complete">隐藏完成后的回调函数 / Callback function invoked after hiding is complete</param>
        [UnityEngine.Scripting.Preserve]
        public override void Hide(IUIFormHideHandler handler, Action complete)
        {
            if (handler != null)
            {
                handler.Handler(GObject, EnableHideAnimation, HideAnimationName, complete);
            }
            else
            {
                complete?.Invoke();
            }
        }

        /// <summary>
        /// 内部设置界面的显示状态，不发出事件。
        /// </summary>
        /// <remarks>
        /// Internally sets the visibility state of the UI form without triggering events.
        /// </remarks>
        /// <param name="value">要设置的可见性状态 / The visibility state to set</param>
        [UnityEngine.Scripting.Preserve]
        protected override void InternalSetVisible(bool value)
        {
            if (GObject.visible == value)
            {
                return;
            }

            GObject.visible = value;
            OnVisibleChanged?.Invoke(value);
            EventSubscriber?.Fire(UIFormVisibleChangedEventArgs.EventId, UIFormVisibleChangedEventArgs.Create(this, value));
        }

        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        /// <remarks>
        /// Gets or sets whether the UI form is visible.
        /// </remarks>
        /// <value>如果界面可见则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if the form is visible; otherwise <c>false</c></value>
        [UnityEngine.Scripting.Preserve]
        public override bool Visible
        {
            get
            {
                if (GObject == null)
                {
                    return false;
                }

                return GObject.visible;
            }
            protected set
            {
                if (GObject == null)
                {
                    return;
                }

                InternalSetVisible(value);
            }
        }


        /// <summary>
        /// 将当前 UI 对象设置为全屏显示。
        /// </summary>
        /// <remarks>
        /// Sets the current UI object to display in full screen mode.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        protected override void MakeFullScreen()
        {
            GObject?.asCom?.MakeFullScreen();
        }

        /// <summary>
        /// 初始化 <see cref="FUI"/> 类的新实例。
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the <see cref="FUI"/> class.
        /// </remarks>
        /// <param name="gObject">关联的 FairyGUI GObject 对象 / The associated FairyGUI GObject instance</param>
        /// <param name="isRoot">是否为根界面 / Whether this is a root form</param>
        [UnityEngine.Scripting.Preserve]
        public FUI(GObject gObject, bool isRoot = false)
        {
            GObject = gObject;
            InitView();
        }

        /// <summary>
        /// 设置关联的 GObject 对象。
        /// </summary>
        /// <remarks>
        /// Sets the associated GObject instance.
        /// </remarks>
        /// <param name="gObject">要设置的 FairyGUI GObject 对象 / The FairyGUI GObject instance to set</param>
        [UnityEngine.Scripting.Preserve]
        public void SetGObject(GObject gObject)
        {
            GObject = gObject;
        }
    }
}
