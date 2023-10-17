#region Header
// Class: NonDrawingGraphicsEditor.cs
// Author: GearInc
// Date: 2023/06/06
// Description: <Description>
#endregion

using Base.Helper;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Base.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(NoneDrawingGraphics), false)]
    public class NoneDrawingGraphicsEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[0]);
            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}