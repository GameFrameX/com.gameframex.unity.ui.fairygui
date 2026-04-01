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

using FairyGUI;
using GameFrameX.UI.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI 界面组辅助器，提供界面组的创建和深度管理功能。
    /// </summary>
    /// <remarks>
    /// FairyGUI UI group helper that provides UI group creation and depth management functionality.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class FairyGUIUIGroupHelper : UIGroupHelperBase
    {
        /// <summary>
        /// 获取界面组深度。
        /// </summary>
        /// <remarks>
        /// Gets the depth of the UI group.
        /// </remarks>
        /// <value>界面组深度 / The depth of the UI group</value>
        [UnityEngine.Scripting.Preserve]
        public override int Depth { get; protected set; }

        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <remarks>
        /// Sets the depth of the UI group.
        /// </remarks>
        /// <param name="depth">界面组深度 / The depth of the UI group</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetDepth(int depth)
        {
            Depth = depth;
            transform.localPosition = new Vector3(0, 0, depth * 100);
        }

        /// <summary>
        /// 创建界面组。
        /// </summary>
        /// <remarks>
        /// Creates a UI group.
        /// </remarks>
        /// <param name="root">根节点 / The root transform</param>
        /// <param name="groupName">界面组名称 / The name of the UI group</param>
        /// <param name="uiGroupHelperTypeName">界面组辅助器类型名 / The type name of the UI group helper</param>
        /// <param name="customUIGroupHelper">自定义的界面组辅助器 / Custom UI group helper</param>
        /// <param name="depth">界面组深度 / The depth of the UI group</param>
        /// <returns>创建的界面组辅助器实例 / The created UI group helper instance</returns>
        [UnityEngine.Scripting.Preserve]
        public override IUIGroupHelper Handler(Transform root, string groupName, string uiGroupHelperTypeName, IUIGroupHelper customUIGroupHelper, int depth = 0)
        {
            SetDepth(depth);
            GComponent component = new GComponent();
            GRoot.inst.AddChild(component);
            var comName = groupName;
            component.displayObject.name = comName;
            component.gameObjectName = comName;
            component.name = comName;
            component.opaque = false;
            component.AddRelation(GRoot.inst, RelationType.Width);
            component.AddRelation(GRoot.inst, RelationType.Height);
            component.MakeFullScreen();
            return GameFrameX.Runtime.Helper.CreateHelper(component.displayObject.gameObject, uiGroupHelperTypeName, (UIGroupHelperBase)customUIGroupHelper, 0);
        }
    }
}
