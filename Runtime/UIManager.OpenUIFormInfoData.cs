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
using GameFrameX.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    internal sealed class OpenUIFormInfoData : IReference
    {
        [UnityEngine.Scripting.Preserve]
        private int m_SerialId = 0;
        [UnityEngine.Scripting.Preserve]
        private bool m_PauseCoveredUIForm = false;
        [UnityEngine.Scripting.Preserve]
        private object m_UserData = null;
        [UnityEngine.Scripting.Preserve]
        private string m_PackageName;
        [UnityEngine.Scripting.Preserve]
        private string m_UIName;
        [UnityEngine.Scripting.Preserve]
        private Type m_FormType;

        [UnityEngine.Scripting.Preserve]
        public Type FormType
        {
            get { return m_FormType; }
        }

        [UnityEngine.Scripting.Preserve]
        public string PackageName
        {
            get { return m_PackageName; }
        }

        [UnityEngine.Scripting.Preserve]
        public string UIName
        {
            get { return m_UIName; }
        }


        [UnityEngine.Scripting.Preserve]
        public int SerialId
        {
            get { return m_SerialId; }
        }

        [UnityEngine.Scripting.Preserve]
        public bool PauseCoveredUIForm
        {
            get { return m_PauseCoveredUIForm; }
        }

        [UnityEngine.Scripting.Preserve]
        public object UserData
        {
            get { return m_UserData; }
        }

        [UnityEngine.Scripting.Preserve]
        public static OpenUIFormInfoData Create(int serialId, string packageName, string uiName, Type uiFormType, bool pauseCoveredUIForm, object userData)
        {
            OpenUIFormInfoData openUIFormInfo = ReferencePool.Acquire<OpenUIFormInfoData>();
            openUIFormInfo.m_SerialId = serialId;
            openUIFormInfo.m_PauseCoveredUIForm = pauseCoveredUIForm;
            openUIFormInfo.m_UserData = userData;
            openUIFormInfo.m_PackageName = packageName;
            openUIFormInfo.m_UIName = uiName;
            openUIFormInfo.m_FormType = uiFormType;
            return openUIFormInfo;
        }

        [UnityEngine.Scripting.Preserve]
        public void Clear()
        {
            m_SerialId = 0;
            m_PauseCoveredUIForm = false;
            m_UserData = null;
            m_PackageName = null;
            m_UIName = null;
            m_FormType = null;
        }
    }
}
