#region Header
// Date: 24/01/2024
// Created by: Huynh Phong Tran
// File name: AudioEventDrawer.cs
#endregion

using Base.Helper;
using Base.Services;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Base.Editor
{
    [DrawerPriority(DrawerPriorityLevel.ValuePriority)]
    public sealed class AudioEventDrawer : OdinValueDrawer<AudioEvent>
    {
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIStyle style = new GUIStyle
                             {
                                     fontStyle = FontStyle.Bold,
                                     fontSize  = 15,
                                     alignment = TextAnchor.MiddleLeft,
                                     normal = new GUIStyleState()
                                              {
                                                      textColor = Color.white
                                              }
                             };
            string titleName = ValueEntry.SmartValue.Name.ToTitleCase();
            SirenixEditorGUI.BeginBox();
            EditorGUILayout.LabelField(titleName, style);
            SirenixEditorGUI.EndBox();
        }
    }
}