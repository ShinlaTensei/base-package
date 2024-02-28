#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Base.Core;
using Base.Helper;
using Sirenix.Utilities.Editor;
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

        private GenericMenu  m_audioTypesMenu;
        private List<string> m_audioTypes            = new List<string>();
        private int          m_typeIndexSelected     = 0;
        private bool         m_isAddTypes            = false;
        private string       m_audioTypesAddTemplate = string.Empty;

        private const float DEFAULT_BUTTON_HEIGHT = 20f;

        private AudioDataContainer AudioDataContainer => DataContainer as AudioDataContainer;

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void DrawTabs()
        {
            GUITabPage audioPage = TabGroup.RegisterTab("Audio List");
            GUITabPage settingPage = TabGroup.RegisterTab("Setting");
            
            TabGroup.BeginGroup();
            if (audioPage.BeginPage())
            {
                if (AudioDataContainer != null)
                {
                    DrawAudioTypeSelection();
                }
            }
            audioPage.EndPage();

            if (settingPage.BeginPage())
            {
                DrawAudioTypeSetting();
            }
            settingPage.EndPage();
            TabGroup.EndGroup();
        }

        protected override DataContainer<AudioAssetData> CreateDataContainer(string path)
        {
            return AssetDatabaseUtility.LoadOrCreateScriptableObject<AudioDataContainer>(path, nameof(AudioDataContainer));
        }
        protected override bool LoadDataContainer(string path, out DataContainer<AudioAssetData> dataContainer)
        {
            dataContainer = null;
            AudioDataContainer[] result = AssetDatabaseUtility.LoadAssetsAtPath<AudioDataContainer>(path, $"t:{nameof(AudioDataContainer)}");
            if (result.Length > 0)
            {
                dataContainer = result[0];
            }

            return dataContainer != null;
        }

        protected override void SaveOutputPathToText(string content)
        {
            string configPath = Path.Combine(PathUtility.GetProjectPath(), EditorConstant.EDITOR_CONFIG_PATH);
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            FileUtilities.SaveTextFile(configPath + $"/{nameof(AudioDataContainer)}.txt", content);
        }

        protected override string LoadOutputPathFromText()
        {
            string configPath = Path.Combine(PathUtility.GetProjectPath(), EditorConstant.EDITOR_CONFIG_PATH);
            if (!Directory.Exists(configPath))
            {
                return string.Empty;
            }
            return FileUtilities.LoadTextFile(Path.GetFullPath(EditorConstant.EDITOR_CONFIG_PATH) + $"/{nameof(AudioDataContainer)}.txt");
        }

        private void DrawAudioTypeSelection()
        {
            m_audioTypesMenu = new GenericMenu();
            m_audioTypes.Clear();
            m_audioTypes.Add("Empty");
            m_audioTypes.AddRange(AudioDataContainer.AudioTypes);
            
            for (int i = 0; i < m_audioTypes.Count; i++)
            {
                string type = m_audioTypes[i];
                bool on = m_typeIndexSelected == i;
                m_audioTypesMenu.AddItem(new GUIContent(type), on, () => SelectAudioType(type));
            }
            
            Rect audioTypeDropdownRect = new Rect(5f, 5f, 150f, DEFAULT_BUTTON_HEIGHT);
            if (EditorGUI.DropdownButton(audioTypeDropdownRect, new GUIContent(m_audioTypes[m_typeIndexSelected]), FocusType.Passive))
            {
                m_audioTypesMenu.DropDown(audioTypeDropdownRect);
            }
        }

        private void SelectAudioType(string type)
        {
            int index = m_audioTypes.IndexOf(type);
            if (index >= 0)
            {
                m_typeIndexSelected = index;
            }
        }

        private void DrawAudioTypeSetting()
        {
            if (AudioDataContainer == null) return;

            Rect areRect = new Rect(5f, 5f, TabGroup.InnerRect.width / 3f, TabGroup.InnerRect.height);
            
            GUILayout.BeginArea(areRect);
            // Start header
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginToolbarBoxHeader(EditorConstant.TOOLBAR_BOX_HEADER_HEIGHT);
            EditorGUI.LabelField(new Rect(areRect.x + 10f, 0, 100f, 
                                          EditorConstant.TOOLBAR_BOX_HEADER_HEIGHT), "Audio Types");

            GUIStyle italicStyle = new GUIStyle(GUI.skin.label)
                                   {
                                           fontStyle = FontStyle.Italic,
                                           fontSize = 10
                                   };
            EditorGUI.LabelField(new Rect(areRect.width - 120f, 0, 100f, 
                                          EditorConstant.TOOLBAR_BOX_HEADER_HEIGHT),$"{AudioDataContainer.AudioTypes.Count} items", italicStyle);

            GUILayout.Space(areRect.width - 75f);
            
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                m_isAddTypes = !m_isAddTypes;
            }
            SirenixEditorGUI.EndToolbarBoxHeader();
            if (SirenixEditorGUI.BeginFadeGroup("Audio Types Add", m_isAddTypes))
            {
                SirenixEditorGUI.BeginBox();
                EditorGUI.indentLevel++;
                m_audioTypesAddTemplate = EditorGUILayout.TextField("Audio Types", m_audioTypesAddTemplate);
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Add"))
                {
                    AudioDataContainer.AudioTypes.AddIfNotContains(m_audioTypesAddTemplate);
                    m_audioTypesAddTemplate = string.Empty;
                    m_isAddTypes            = false;
                }
                SirenixEditorGUI.EndBox();
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
            // End header

            GUILayout.EndArea();
        }
        
        private void DrawAddStringGUI()
        {
            
        }

        private void DrawListItem()
        {
            
        }
    }
}