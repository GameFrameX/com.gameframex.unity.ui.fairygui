using FairyGUI;
using GameFrameX.UI.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public class FUI : UIFormLogic
    {
        /// <summary>
        /// UI 对象
        /// </summary>
        public GObject GObject { get; private set; }

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

                GObject.visible = value;
            }
        }


        /// <summary>
        /// 设置当前UI对象为全屏
        /// </summary>
        protected override void MakeFullScreen()
        {
            GObject?.asCom?.MakeFullScreen();
        }

        public FUI(GObject gObject, object userData = null, bool isRoot = false)
        {
            GObject = gObject;
            InitView();
        }

        public void SetGObject(GObject gObject)
        {
            GObject = gObject;
        }
    }
}