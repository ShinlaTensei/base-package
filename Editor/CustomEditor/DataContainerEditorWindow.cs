#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: DataContainerEditorWindow.cs
#endregion

using System;
using Base.Core;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Base.Editor
{
    public abstract class DataContainerEditorWindow<T> : OdinEditorWindow where T : class, IDataObject
    {
        /// <summary>
        /// Static reference to the window itself to avoid multiple memory allocations when opening and closing the window.
        /// </summary>
        protected static DataContainerEditorWindow<T> window;
        
        private const float HEADER_HEIGHT      = 70f;
        private const float FOLDER_BUTTON_SIZE = 25f;

        protected string           outputPath = string.Empty;
        protected DataContainer<T> DataContainer { get; set; }

        private void DrawHeader()
        {
            SirenixEditorGUI.DrawSolidRect(new Rect(0, 0, position.width, HEADER_HEIGHT), PEditorStyles.BackgroundColorGrey);
            SirenixEditorGUI.DrawSolidRect(new Rect(0, HEADER_HEIGHT, position.width, 1), PEditorStyles.SeparatorColorBlack);
            
            // Draw the output path
            GUILayout.BeginArea(new Rect(0f, 0, position.width, HEADER_HEIGHT));
            SirenixEditorGUI.BeginVerticalPropertyLayout(null);
            SirenixEditorGUI.BeginHorizontalPropertyLayout(null);
            Rect outputPathRect = new Rect(5f, 15f, position.width - 25f, 20f);
            EditorGUI.BeginChangeCheck();
            string path = SirenixEditorFields.FolderPathField(outputPathRect, new GUIContent("Output Path"), outputPath, 
                                                              "Assets", true, true);
            if (EditorGUI.EndChangeCheck())
            {
                outputPath = path;
                GUIHelper.ExitGUI(false);
            }
            SirenixEditorGUI.EndHorizontalPropertyLayout();


            SirenixEditorGUI.EndVerticalPropertyLayout();
            GUILayout.EndArea();
        }

        private void DrawFolderMenu()
        {
            
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            DrawHeader();
        }
    }
}