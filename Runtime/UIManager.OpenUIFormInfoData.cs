﻿namespace GameFrameX.UI.Runtime
{
    internal sealed class OpenUIFormInfoData : IReference
    {
        private int m_SerialId = 0;
        private UIGroup m_UIGroup = null;
        private bool m_PauseCoveredUIForm = false;
        private object m_UserData = null;
        private string m_PackageName;
        private string m_UIName;


        public string PackageName
        {
            get { return m_PackageName; }
        }

        public string UIName
        {
            get { return m_UIName; }
        }


        public int SerialId
        {
            get { return m_SerialId; }
        }

        public UIGroup UIGroup
        {
            get { return m_UIGroup; }
        }

        public bool PauseCoveredUIForm
        {
            get { return m_PauseCoveredUIForm; }
        }

        public object UserData
        {
            get { return m_UserData; }
        }

        public static OpenUIFormInfoData Create(int serialId, string packageName, string uiName, UIGroup uiGroup, bool pauseCoveredUIForm, object userData)
        {
            OpenUIFormInfoData openUIFormInfo = ReferencePool.Acquire<OpenUIFormInfoData>();
            openUIFormInfo.m_SerialId = serialId;
            openUIFormInfo.m_UIGroup = uiGroup;
            openUIFormInfo.m_PauseCoveredUIForm = pauseCoveredUIForm;
            openUIFormInfo.m_UserData = userData;
            openUIFormInfo.m_PackageName = packageName;
            openUIFormInfo.m_UIName = uiName;
            return openUIFormInfo;
        }

        public void Clear()
        {
            m_SerialId = 0;
            m_UIGroup = null;
            m_PauseCoveredUIForm = false;
            m_UserData = null;
            m_PackageName = null;
            m_UIName = null;
        }
    }
}