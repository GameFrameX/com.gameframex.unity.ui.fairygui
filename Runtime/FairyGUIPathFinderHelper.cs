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
    public static class GObjectExtensions
    {
        /// <summary>
        /// 获取UI路径
        /// </summary>
        /// <param name="self">GObject对象</param>
        /// <returns>UI路径</returns>
        public static string GetUIPath(this GObject self)
        {
            return FairyGUIPathFinderHelper.GetUIPath(self);
        }
    }

    /// <summary>
    /// FGUI 路径帮助类
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public static class FairyGUIPathFinderHelper
    {
        /// <summary>
        /// 根据UI对象获取路径
        /// </summary>
        /// <param name="o">UI对象</param>
        /// <returns>UI所在路径</returns>
        public static string GetUIPath(GObject o)
        {
            var ls = new List<string>();
            SearchParent(o, ls);
            ls.Reverse();
            return string.Join("/", ls);
        }

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
        /// 根据路径获取FUI对象
        /// </summary>
        /// <param name="path">UI路径</param>
        /// <returns>UI对象</returns>
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
        /// 路径是否包含该对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="gObject">对象</param>
        /// <returns></returns>
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