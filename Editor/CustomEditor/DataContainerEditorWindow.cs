#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: DataContainerEditorWindow.cs
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Base.Core;
using Base.Helper;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Editor
{
    public abstract class DataContainerEditorWindow<T> : OdinEditorWindow where T : class, IDataObject
    {
        /// <summary>
        /// Static reference to the window itself to avoid multiple memory allocations when opening and closing the window.
        /// </summary>
        protected static DataContainerEditorWindow<T> window;
        
        private const float HEADER_HEIGHT       = 70f;
        private const float FOLDER_BUTTON_SIZE  = 25f;
        private const float HEADER_BUTTON_WIDTH = 60f;
        private const float CORE_BUTTON_WIDTH   = 80f;
        private const float CORE_BUTTON_HEIGHT  = 30f;

        private GUITabGroup      m_tabGroup;
        private DataContainer<T> m_dataContainer;
        private T                m_dataObject;

        protected string           OutputPath    { get; private set; }
        protected DataContainer<T> DataContainer
        {
            get => m_dataContainer;
            private set => m_dataContainer = value;
        }

        protected T DataObject
        {
            get => m_dataObject;
            set => m_dataObject = value;
        }
        
        protected ReorderableList DataReorderableList { get; set; }

        protected GUITabGroup TabGroup => m_tabGroup;

        private void DrawOutputPath()
        {  
            SirenixEditorGUI.DrawSolidRect(new Rect(0, 0, position.width, HEADER_HEIGHT), PEditorStyles.BackgroundColorGrey);
            SirenixEditorGUI.DrawSolidRect(new Rect(0, HEADER_HEIGHT, position.width, 1), PEditorStyles.SeparatorColorBlack);
            
            // Draw the output path
            GUILayout.BeginArea(new Rect(0f, 0, position.width, HEADER_HEIGHT));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            Rect outputPathRect = new Rect(5f, 15f, position.width - 25f, 20f);
            EditorGUI.BeginChangeCheck();
            string path = SirenixEditorFields.FolderPathField(outputPathRect, new GUIContent("Output Path"), OutputPath, 
                                                              "Assets", false, true);
            if (EditorGUI.EndChangeCheck())
            {
                OutputPath = path;

                if (!string.IsNullOrEmpty(OutputPath) && LoadDataContainer(OutputPath, out DataContainer<T> dataContainer))
                {
                    DataContainer = dataContainer;
                    SaveOutputPathToText(OutputPath);
                    InitAfterLoadReference();
                }
                GUIHelper.ExitGUI(false);
            }
            EditorGUILayout.EndHorizontal();
            
            // Draw the asset reference
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            Rect unityObjectFieldRect = new Rect(outputPathRect.x, outputPathRect.y + 25f, 100f, 20f);
            EditorGUI.LabelField(unityObjectFieldRect, new GUIContent("Data Container"));
            GUI.enabled = false;
            Rect   unityObjectRefRect = new Rect(unityObjectFieldRect.x + unityObjectFieldRect.width + 53f, unityObjectFieldRect.y, position.width-400f, 20f);
            Object objectField        = SirenixEditorFields.UnityObjectField(unityObjectRefRect, DataContainer, typeof(ScriptableObject), false);
            GUI.enabled = true;
            if (EditorGUI.EndChangeCheck())
            {
                if (objectField != null) DataContainer = (DataContainer<T>)objectField;
                GUIHelper.ExitGUI(false);
            }

            Rect headerCreateBtnRect = new Rect(unityObjectRefRect.x + unityObjectRefRect.width + 10f, unityObjectRefRect.y, HEADER_BUTTON_WIDTH, 20f);
            GUI.enabled = DataContainer == null && !string.IsNullOrEmpty(OutputPath);
            if (GUI.Button(headerCreateBtnRect, "Create"))
            {
                DataContainer = CreateDataContainer(OutputPath);
                InitAfterLoadReference();
            }
            Rect headerDeleteBtnRect = new Rect(headerCreateBtnRect.x + headerCreateBtnRect.width + 10f, headerCreateBtnRect.y,
                                                HEADER_BUTTON_WIDTH, 20f);
            GUI.enabled = DataContainer != null;
            if (GUI.Button(headerDeleteBtnRect, "Delete"))
            {
                if (AssetDatabaseUtility.DeleteAsset(ref m_dataContainer))
                {
                    
                }
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawCoreButtons()
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);
            GUI.backgroundColor = PEditorStyles.BackgroundGreenDarkColor;
            GUI.contentColor    = SirenixGUIStyles.GreenValidColor;
            Rect saveButtonRect = new Rect(position.width - 230f, HEADER_HEIGHT + 40f, CORE_BUTTON_WIDTH, CORE_BUTTON_HEIGHT);
            if (GUI.Button(saveButtonRect, "Save"))
            {
                DataContainer.Save();
            }
            GUI.backgroundColor = PEditorStyles.BackgroundRedColor;
            GUI.contentColor    = SirenixGUIStyles.RedErrorColor;
            Rect discardBtnRect = new Rect(saveButtonRect.x + CORE_BUTTON_WIDTH + 10f, saveButtonRect.y, CORE_BUTTON_WIDTH, CORE_BUTTON_HEIGHT);
            if (GUI.Button(discardBtnRect, "Discard"))
            {
                DataContainer.Revert();
            }
            GUI.backgroundColor = Color.white;
            GUI.contentColor    = Color.white;
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }

        protected abstract void DrawTabs();

        protected override void Initialize()
        {
            m_tabGroup              = SirenixEditorGUI.CreateAnimatedTabGroup("DataContainer");
            m_tabGroup.TabLayouting = TabLayouting.MultiRow;
            m_tabGroup.ExpandHeight = true;

            if (string.IsNullOrEmpty(OutputPath))
            {
                OutputPath = LoadOutputPathFromText();
            }

            if (!string.IsNullOrEmpty(OutputPath) && LoadDataContainer(OutputPath, out DataContainer<T> dataContainer) && DataContainer == null)
            {
                DataContainer = dataContainer;
            }
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            DrawOutputPath();
            DrawCoreButtons();
            DrawTabs();
        }

        protected abstract DataContainer<T> CreateDataContainer(string path);
        protected abstract bool LoadDataContainer(string path, out DataContainer<T> dataContainer);

        protected abstract void SaveOutputPathToText(string content);
        protected abstract string LoadOutputPathFromText();
        
        protected virtual void InitAfterLoadReference() {}
    }
}