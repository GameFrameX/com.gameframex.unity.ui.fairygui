//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using FairyGUI;
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
        /// <summary>
        /// 回收界面实例对象。
        /// </summary>
        /// <param name="uiForm"></param>
        /// <param name="isDispose">是否销毁释放</param>
        protected override void RecycleUIForm(IUIForm uiForm, bool isDispose = false)
        {
            uiForm.OnRecycle();
            var formHandle = uiForm.Handle as GameObject;
            if (formHandle)
            {
                var displayObjectInfo = formHandle.GetComponent<DisplayObjectInfo>();
                if (displayObjectInfo)
                {
                    if (displayObjectInfo.displayObject.gOwner is GComponent component)
                    {
                        if (isDispose)
                        {
                            component.Dispose();
                        }
                        else
                        {
                            m_InstancePool.Unspawn(component);
                        }
                    }
                }
            }
        }
    }
}