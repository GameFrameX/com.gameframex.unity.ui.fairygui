//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using GameFrameX.UI.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// 界面管理器。
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    internal sealed partial class UIManager : BaseUIManager
    {
        private FairyGUIPackageComponent FairyGuiPackage { get; set; }
        private int m_Serial;

        /// <summary>
        /// 初始化界面管理器的新实例。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public UIManager()
        {
            // m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, LoadAssetUpdateCallback, LoadAssetDependencyAssetCallback);
            m_AssetManager = null;
            m_InstancePool = null;
            m_UIFormHelper = null;
            m_Serial = 0;
            m_RecycleTime = 0;
            if (m_RecycleInterval < 10)
            {
                m_RecycleInterval = 10;
            }

            m_IsShutdown = false;
            m_OpenUIFormSuccessEventHandler = null;
            m_OpenUIFormFailureEventHandler = null;
            // m_OpenUIFormUpdateEventHandler = null;
            // m_OpenUIFormDependencyAssetEventHandler = null;
        }


        /*/// <summary>
        /// 获取或设置界面实例对象池的优先级。
        /// </summary>
        public int InstancePriority
        {
            get { return m_InstancePool.Priority; }
            set { m_InstancePool.Priority = value; }
        }*/


        /*
        /// <summary>
        /// 打开界面更新事件。
        /// </summary>
        public event EventHandler<OpenUIFormUpdateEventArgs> OpenUIFormUpdate
        {
            add { m_OpenUIFormUpdateEventHandler += value; }
            remove { m_OpenUIFormUpdateEventHandler -= value; }
        }

        /// <summary>
        /// 打开界面时加载依赖资源事件。
        /// </summary>
        public event EventHandler<OpenUIFormDependencyAssetEventArgs> OpenUIFormDependencyAsset
        {
            add { m_OpenUIFormDependencyAssetEventHandler += value; }
            remove { m_OpenUIFormDependencyAssetEventHandler -= value; }
        }*/

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="assetManager">资源管理器。</param>
        public override void SetResourceManager(IAssetManager assetManager)
        {
            base.SetResourceManager(assetManager);
            FairyGuiPackage = GameEntry.GetComponent<FairyGUIPackageComponent>();
            GameFrameworkGuard.NotNull(FairyGuiPackage, nameof(FairyGuiPackage));
        }
    }
}