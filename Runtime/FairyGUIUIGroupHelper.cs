//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using FairyGUI;
using GameFrameX.UI.Runtime;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// FairyGUI界面组辅助器。
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public sealed class FairyGUIUIGroupHelper : UIGroupHelperBase
    {
        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public override void SetDepth(int depth)
        {
            transform.localPosition = new Vector3(0, 0, depth * 100);
        }

        public override IUIGroupHelper Handler(Transform root, string groupName, string uiGroupHelperTypeName, IUIGroupHelper customUIGroupHelper)
        {
            GComponent component = new GComponent();
            GRoot.inst.AddChild(component);
            var comName = groupName;
            component.displayObject.name = comName;
            component.gameObjectName = comName;
            component.name = comName;
            component.AddRelation(GRoot.inst, RelationType.Width);
            component.AddRelation(GRoot.inst, RelationType.Height);
            component.MakeFullScreen();
            return GameFrameX.Runtime.Helper.CreateHelper(component.displayObject.gameObject, uiGroupHelperTypeName, (UIGroupHelperBase)customUIGroupHelper, 0);
        }
    }
}