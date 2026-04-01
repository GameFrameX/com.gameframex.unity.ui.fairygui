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
using System.Collections.Generic;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// GObject 辅助类，提供 FUI 对象的缓存和管理功能。
    /// </summary>
    /// <remarks>
    /// GObject helper that provides FUI object caching and management functionality.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public static class GObjectHelper
    {
        [UnityEngine.Scripting.Preserve]
        private static readonly Dictionary<GObject, FUI> s_GObjectToFuiMap = new Dictionary<GObject, FUI>();

        /// <summary>
        /// 清理所有缓存的 FUI 对象。
        /// </summary>
        /// <remarks>
        /// Clears all cached FUI objects.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public static void ClearAll()
        {
            s_GObjectToFuiMap.Clear();
        }

        /// <summary>
        /// 从组件池中获取关联的 FUI 对象。
        /// </summary>
        /// <remarks>
        /// Gets the associated FUI object from the component pool.
        /// </remarks>
        /// <typeparam name="T">FUI 派生类型 / The derived type of FUI</typeparam>
        /// <param name="self">GObject 对象实例 / The GObject instance</param>
        /// <returns>关联的 FUI 对象；如果不存在则返回默认值 / The associated FUI object; default value if not found</returns>
        [UnityEngine.Scripting.Preserve]
        public static T Get<T>(this GObject self) where T : FUI
        {
            if (self != null && s_GObjectToFuiMap.TryGetValue(self, out var pair))
            {
                return pair as T;
            }

            return default(T);
        }

        /// <summary>
        /// 将 FUI 对象添加到组件池。
        /// </summary>
        /// <remarks>
        /// Adds an FUI object to the component pool.
        /// </remarks>
        /// <param name="self">GObject 对象实例 / The GObject instance</param>
        /// <param name="fui">要添加的 FUI 对象 / The FUI object to add</param>
        [UnityEngine.Scripting.Preserve]
        public static void Add(this GObject self, FUI fui)
        {
            if (self != null && fui != null)
            {
                s_GObjectToFuiMap[self] = fui;
            }
        }

        /// <summary>
        /// 从组件池中删除 FUI 对象。
        /// </summary>
        /// <remarks>
        /// Removes an FUI object from the component pool.
        /// </remarks>
        /// <param name="self">GObject 对象实例 / The GObject instance</param>
        /// <returns>被删除的 FUI 对象；如果不存在则返回默认值 / The removed FUI object; default value if not found</returns>
        [UnityEngine.Scripting.Preserve]
        public static FUI Remove(this GObject self)
        {
            if (self != null && s_GObjectToFuiMap.TryGetValue(self, out var value))
            {
                s_GObjectToFuiMap.Remove(self);
                return value;
            }

            return default;
        }
    }
}
