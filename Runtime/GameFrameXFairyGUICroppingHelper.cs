using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameFrameX.UI.FairyGUI.Runtime
{
    [Preserve]
    public class GameFrameXFairyGUICroppingHelper : MonoBehaviour
    {
        private Type[] m_Types;

        [Preserve]
        private void Start()
        {
            m_Types = new Type[]
            {
                // typeof(GObjectHelper),
                typeof(FUI),
                // typeof(FairyGUIComponent),
                typeof(FairyGUILoadAsyncResourceHelper),
                typeof(FairyGUIPackageComponent),
                typeof(FairyGUIPathFinderHelper),
            };
        }
    }
}