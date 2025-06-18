// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using FairyGUI;
using FairyGUIEditor;
using GameFrameX.Runtime;
using GameFrameX.UI.FairyGUI.Runtime;
using UnityEditor;
using UnityEngine;

namespace GameFrameX.UI.FairyGUI.Editor
{
    [CustomEditor(typeof(DisplayObjectInfo))]
    public class FairyGUIDisplayObjectEditor : DisplayObjectEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var displayObjectInfo = (target as DisplayObjectInfo);
            DisplayObject obj = displayObjectInfo?.displayObject;
            if (obj == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Operations", GUILayout.Width(200));
                if (GUILayout.Button("Copy Path", GUILayout.ExpandWidth(true)))
                {
                    string path = FairyGUIPathFinderHelper.GetUIPath(displayObjectInfo.displayObject.gOwner);
                    Log.Debug("Copy Path: {0}", path);
                    EditorGUIUtility.systemCopyBuffer = path;
                }

                EditorGUILayout.Separator();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}