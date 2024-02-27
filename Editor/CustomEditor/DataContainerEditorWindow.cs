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

        protected string           OutputPath    { get; private set; }
        protected DataContainer<T> DataContainer
        {
            get => m_dataContainer;
            private set => m_dataContainer = value;
        }

        protected GUITabGroup TabGroup => m_tabGroup;

        private void DrawOutputPath()
        {  
            SirenixEditorGUI.DrawSolidRect(new Rect(0, 0, position.width, HEADER_HEIGHT), PEditorStyles.BackgroundColorGrey);
            SirenixEditorGUI.DrawSolidRect(new Rect(0, HEADER_HEIGHT, position.width, 1), PEditorStyles.SeparatorColorBlack);
            
            // Draw the output path
            GUILayout.BeginArea(new Rect(0f, 0, position.width, HEADER_HEIGHT));
            SirenixEditorGUI.BeginVerticalPropertyLayout(null);
            SirenixEditorGUI.BeginHorizontalPropertyLayout(null);
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
                }
                GUIHelper.ExitGUI(false);
            }
            SirenixEditorGUI.EndHorizontalPropertyLayout();
            
            // Draw the asset reference
            SirenixEditorGUI.BeginHorizontalPropertyLayout(null);
            EditorGUI.BeginChangeCheck();
            Rect unityObjectFieldRect = new Rect(outputPathRect.x, outputPathRect.y + 25f, 100f, 20f);
            EditorGUI.LabelField(unityObjectFieldRect, new GUIContent("Data Container"));
            GUI.enabled = false;
            Rect   unityObjectRefRect = new Rect(unityObjectFieldRect.x + unityObjectFieldRect.width + 53f, unityObjectFieldRect.y, 650f, 20f);
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
            
            SirenixEditorGUI.EndHorizontalPropertyLayout();
            
            SirenixEditorGUI.EndVerticalPropertyLayout();
            GUILayout.EndArea();
        }

        private void DrawCoreButtons()
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);
            GUI.backgroundColor = PEditorStyles.BackgroundGreenDarkColor;
            GUI.contentColor    = SirenixGUIStyles.GreenValidColor;
            Rect saveButtonRect = new Rect(position.width - 230f, 80f, CORE_BUTTON_WIDTH, CORE_BUTTON_HEIGHT);
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

        private void DrawTabs()
        {
            GUITabPage dataTab    = m_tabGroup.RegisterTab("Data");
            GUITabPage settingTab = m_tabGroup.RegisterTab("Setting");
            m_tabGroup.BeginGroup(style: SirenixGUIStyles.BoxContainer);
            {
                if (dataTab.BeginPage())
                {
                    
                }
                dataTab.EndPage();
                if (settingTab.BeginPage())
                {
                     
                }
                settingTab.EndPage();
            }
            m_tabGroup.EndGroup();
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_tabGroup              = SirenixEditorGUI.CreateAnimatedTabGroup("DataContainer");
            m_tabGroup.TabLayouting = TabLayouting.MultiRow;
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
    }
}