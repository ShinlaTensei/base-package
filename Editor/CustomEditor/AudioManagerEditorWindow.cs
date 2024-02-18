#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using UnityEditor;
using UnityEngine;

namespace Base.Editor
{
    public class AudioManagerEditorWindow : DataContainerEditorWindow<AudioAssetData>
    {
        [MenuItem("PFramework/Audio Manager")]
        private static void OpenWindow()
        {
            window         = GetWindow<AudioManagerEditorWindow>("Audio Manager");
            window.minSize = new Vector2(500f, 500f);
        }
    }
}