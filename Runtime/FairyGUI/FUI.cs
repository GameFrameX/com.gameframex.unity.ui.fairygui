using FairyGUI;
using System;
using GameFrameX.UI.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    public class FUI : UIFormLogic
    {
        /// <summary>
        /// UI 对象
        /// </summary>
        public GObject GObject { get; }

        /// <summary>
        /// 设置UI的显示状态，不发出事件
        /// </summary>
        /// <param name="value"></param>
        protected override void InternalSetVisible(bool value)
        {
            if (GObject.visible == value)
            {
                return;
            }

            GObject.visible = value;
        }

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
            set
            {
                if (GObject == null)
                {
                    return;
                }

                if (GObject.visible == value)
                {
                    return;
                }

                if (value == false)
                {
                    // OnHideBeforeAction?.Invoke(this);
                    // foreach (var child in Children)
                    // {
                    //     ((FUI)child.Value).Visible = value;
                    // }

                    // OnHide();
                    // OnHideAfterAction?.Invoke(this);
                }

                GObject.visible = value;
                if (value)
                {
                    // OnShowBeforeAction?.Invoke(this);
                    // foreach (var child in Children)
                    // {
                    //     ((FUI)child.Value).Visible = value;
                    // }
                    //
                    // OnShow();
                    // OnShowAfterAction?.Invoke(this);
                    // Refresh();
                }
            }
        }

        /// <summary>
        /// 销毁UI对象
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // if (IsDisposed)
            // {
            //     return;
            // }
            //
            // IsDisposed = true;
            // // 删除所有的孩子
            // DisposeChildren();
            //
            // // 删除自己的UI
            // if (!IsRoot)
            // {
            //     RemoveFromParent();
            // }
            //
            // // 释放UI
            // OnDispose();
            // // 删除自己的UI
            // if (!IsRoot)
            // {
            //     GObject.Dispose();
            // }
            //
            // Release(false);
            // isFromFGUIPool = false;
        }

        /// <summary>
        /// 添加UI对象到子级列表
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="index">添加到的目标UI层级索引位置</param>
        /// <exception cref="Exception"></exception>
        // protected override void AddInner(UI.Runtime.UI ui, int index = -1)
        // {
        //     var fui = (FUI)ui;
        //     Children.Add(ui.Name, ui);
        //     if (index < 0 || index > Children.Count)
        //     {
        //         GObject.asCom.AddChild(fui.GObject);
        //     }
        //     else
        //     {
        //         GObject.asCom.AddChildAt(fui.GObject, index);
        //     }
        //
        //     fui.Parent = this;
        //
        //     if (fui.IsInitVisible)
        //     {
        //         // 显示UI
        //         fui.Show(fui.UserData);
        //     }
        // }

        /// <summary>
        /// 设置当前UI对象为全屏
        /// </summary>
        protected override void MakeFullScreen()
        {
            GObject?.asCom?.MakeFullScreen();
        }

        /// <summary>
        /// 删除指定UI名称的UI对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /*public override bool Remove(string name)
        {
            if (Children.TryGetValue(name, out var ui))
            {
                Children.Remove(name);

                if (ui != null)
                {
                    ui.RemoveChildren();

                    ui.Hide();

                    if (IsComponent)
                    {
                        GObject.asCom.RemoveChild(((FUI)ui).GObject);
                    }

                    return true;
                }
            }

            return false;
        }*/
        public FUI(GObject gObject, object userData = null, bool isRoot = false)
        {
            GObject = gObject;
        }
    }
}