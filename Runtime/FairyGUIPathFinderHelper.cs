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
using System.Collections.Generic;
using FairyGUI;
using GameFrameX.Runtime;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    /// <summary>
    /// GObject 扩展方法类。
    /// </summary>
    /// <remarks>
    /// Extension methods for GObject.
    /// </remarks>
    public static class GObjectExtensions
    {
        /// <summary>
        /// 获取 UI 对象的路径。
        /// </summary>
        /// <remarks>
        /// Gets the path of the UI object.
        /// </remarks>
        /// <param name="self">GObject 对象实例 / The GObject instance</param>
        /// <returns>UI 对象的路径字符串 / The path string of the UI object</returns>
        [UnityEngine.Scripting.Preserve]
        public static string GetUIPath(this GObject self)
        {
            return FairyGUIPathFinderHelper.GetUIPath(self);
        }
    }

    /// <summary>
    /// FairyGUI 路径查找辅助类，提供 UI 对象路径的查找和解析功能。
    /// </summary>
    /// <remarks>
    /// FairyGUI path finder helper that provides UI object path lookup and parsing functionality.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public static class FairyGUIPathFinderHelper
    {
        /// <summary>
        /// 根据 UI 对象获取其路径。
        /// </summary>
        /// <remarks>
        /// Gets the path of the UI object.
        /// </remarks>
        /// <param name="o">UI 对象 / The UI object</param>
        /// <returns>UI 对象的路径 / The path of the UI object</returns>
        [UnityEngine.Scripting.Preserve]
        public static string GetUIPath(GObject o)
        {
            var ls = new List<string>();
            SearchParent(o, ls);
            ls.Reverse();
            return string.Join("/", ls);
        }

        /// <summary>
        /// 递归搜索父级对象并收集名称。
        /// </summary>
        /// <remarks>
        /// Recursively searches parent objects and collects names.
        /// </remarks>
        /// <param name="o">当前 GObject 对象 / The current GObject</param>
        /// <param name="st">名称收集列表 / The name collection list</param>
        [UnityEngine.Scripting.Preserve]
        private static void SearchParent(GObject o, List<string> st)
        {
            if (o.parent != null)
            {
                st.Add(o.name);
                SearchParent(o.parent, st);
            }
            else
            {
                st.Add(o.name);
            }
        }

        /// <summary>
        /// 根据路径获取 FUI 对象。
        /// </summary>
        /// <remarks>
        /// Gets the FUI object by path.
        /// </remarks>
        /// <param name="path">UI 路径 / The UI path</param>
        /// <returns>找到的 UI 对象；如果未找到则返回 <c>null</c> / The found UI object; <c>null</c> if not found</returns>
        [UnityEngine.Scripting.Preserve]
        public static GObject GetUIFromPath(string path)
        {
            //GRoot / UISynthesisScene / ContentBox / ListSelect / 1990197248 / icon

            string[] arr = path.Split('/');

            var q = new Queue<string>();
            foreach (string v in arr)
            {
                if (v == "GRoot")
                {
                    continue;
                }

                q.Enqueue(v);
            }

            try
            {
                GObject child = SearchChild(GRoot.inst, q);
                return child;
            }
            catch (Exception exception)
            {
                Log.Error("error uiPath : can not found ui by this path :" + path + ", error : " + exception);
            }

            return null;
        }

        /// <summary>
        /// 递归搜索子对象。
        /// </summary>
        /// <remarks>
        /// Recursively searches for a child object.
        /// </remarks>
        /// <param name="o">父级组件 / The parent component</param>
        /// <param name="q">路径队列 / The path queue</param>
        /// <returns>找到的子对象 / The found child object</returns>
        /// <exception cref="Exception">路径无效时抛出异常 / Thrown when the path is invalid</exception>
        [UnityEngine.Scripting.Preserve]
        private static GObject SearchChild(GComponent o, Queue<string> q)
        {
            //防错
            if (q.Count <= 0)
            {
                return o;
            }

            string path = q.Dequeue();
            GObject child = null;
            if (path[0] == '$')
            {
                child = o.GetChild(path);
                if (child == null)
                {
                    string at = path.Substring(1);
                    int index = int.Parse(at);

                    if (index < 0 || index >= o.numChildren)
                    {
                        throw new Exception("eror path");
                    }

                    child = o.GetChildAt(index);
                }
            }
            else
            {
                child = o.GetChild(path);
            }

            if (child == null)
            {
                throw new Exception("error path");
            }

            if (q.Count <= 0)
            {
                // 说明没有下级了
                return child;
            }

            if (child is GComponent)
            {
                return SearchChild(child as GComponent, q);
            }

            throw new Exception("error path");
        }

        /// <summary>
        /// 检查路径是否包含指定对象。
        /// </summary>
        /// <remarks>
        /// Checks whether the path includes the specified object.
        /// </remarks>
        /// <param name="path">路径字符串 / The path string</param>
        /// <param name="gObject">要检查的对象 / The object to check</param>
        /// <returns>如果路径包含该对象则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if the path includes the object; otherwise <c>false</c></returns>
        [UnityEngine.Scripting.Preserve]
        public static bool SearchPathInclude(string path, GObject gObject)
        {
            if ("all".ToLower() == path)
            {
                return false;
            }

            var q = new List<string>();

            foreach (string v in path.Split('/'))
            {
                if (v == "GRoot")
                {
                    continue;
                }

                q.Add(v);
            }

            GObject current = gObject;
            var list = new List<GObject> { current, };
            while (current.parent != null && current.parent.name != "GRoot")
            {
                current = current.parent;
                list.Add(current);
            }

            // 反转链表
            list.Reverse();

            if (list.Count < q.Count)
            {
                // 路径长度小于,肯定是不对的
                return false;
            }

            for (int i = 0; i < q.Count; i++)
            {
                if (list[i].name == q[i])
                {
                    continue;
                }

                if (q[i][0] == '$')
                {
                    string at = q[i].Substring(1);
                    int index = int.Parse(at);
                    if (list[i].parent.GetChildIndex(list[i]) == index)
                    {
                        continue;
                    }

                    {
                        return false;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
