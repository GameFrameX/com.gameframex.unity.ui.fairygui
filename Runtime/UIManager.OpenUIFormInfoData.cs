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
    /// <summary>
    /// 打开界面信息数据类，用于存储界面打开时的相关数据。
    /// </summary>
    /// <remarks>
    /// Open UI form info data class used to store relevant data when opening a UI form.
    /// </remarks>
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

        /// <summary>
        /// 获取界面类型。
        /// </summary>
        /// <remarks>
        /// Gets the UI form type.
        /// </remarks>
        /// <value>界面类型 / The UI form type</value>
        [UnityEngine.Scripting.Preserve]
        public Type FormType
        {
            get { return m_FormType; }
        }

        /// <summary>
        /// 获取包名称。
        /// </summary>
        /// <remarks>
        /// Gets the package name.
        /// </remarks>
        /// <value>包名称 / The package name</value>
        [UnityEngine.Scripting.Preserve]
        public string PackageName
        {
            get { return m_PackageName; }
        }

        /// <summary>
        /// 获取界面名称。
        /// </summary>
        /// <remarks>
        /// Gets the UI name.
        /// </remarks>
        /// <value>界面名称 / The UI name</value>
        [UnityEngine.Scripting.Preserve]
        public string UIName
        {
            get { return m_UIName; }
        }


        /// <summary>
        /// 获取界面序列号。
        /// </summary>
        /// <remarks>
        /// Gets the UI form serial ID.
        /// </remarks>
        /// <value>界面序列号 / The UI form serial ID</value>
        [UnityEngine.Scripting.Preserve]
        public int SerialId
        {
            get { return m_SerialId; }
        }

        /// <summary>
        /// 获取是否暂停被覆盖的界面。
        /// </summary>
        /// <remarks>
        /// Gets whether to pause covered UI forms.
        /// </remarks>
        /// <value>如果暂停被覆盖的界面则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if pausing covered UI forms; otherwise <c>false</c></value>
        [UnityEngine.Scripting.Preserve]
        public bool PauseCoveredUIForm
        {
            get { return m_PauseCoveredUIForm; }
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        /// <remarks>
        /// Gets the user-defined data.
        /// </remarks>
        /// <value>用户自定义数据 / The user-defined data</value>
        [UnityEngine.Scripting.Preserve]
        public object UserData
        {
            get { return m_UserData; }
        }

        /// <summary>
        /// 创建打开界面信息数据实例。
        /// </summary>
        /// <remarks>
        /// Creates an instance of open UI form info data.
        /// </remarks>
        /// <param name="serialId">界面序列号 / The UI form serial ID</param>
        /// <param name="packageName">包名称 / The package name</param>
        /// <param name="uiName">界面名称 / The UI name</param>
        /// <param name="uiFormType">界面类型 / The UI form type</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面 / Whether to pause covered UI forms</param>
        /// <param name="userData">用户自定义数据 / User-defined data</param>
        /// <returns>创建的打开界面信息数据实例 / The created open UI form info data instance</returns>
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

        /// <summary>
        /// 清理数据，重置所有字段为默认值。
        /// </summary>
        /// <remarks>
        /// Clears the data and resets all fields to default values.
        /// </remarks>
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
