#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System.Collections.Generic;
using System.IO;
using Base.Core;
using Base.Helper;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
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
        
        private bool         m_isAddTypes            = false;
        private string       m_audioTypesAddTemplate = string.Empty;
        private List<string> m_audioTypeDeletion = new List<string>();

        private const float DEFAULT_BUTTON_HEIGHT = 20f;

        private AudioDataContainer AudioDataContainer => DataContainer as AudioDataContainer;
        
        private ReorderableList AudioTypeReorderableList { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            if (AudioDataContainer != null)
            {
                AudioTypeReorderableList = new ReorderableList(AudioDataContainer.AudioTypes, typeof(string), true, false,
                    false, false)
                {
                    drawElementCallback = DrawAudioTypeCallback,
                    multiSelect = false,
                    onSelectCallback = OnSelectAudioTypeElementCallback 
                };
            }
        }

        protected override void DrawTabs()
        {
            GUITabPage audioPage = TabGroup.RegisterTab("Audio List");

            TabGroup.BeginGroup();
            if (audioPage.BeginPage())
            {
                if (AudioDataContainer != null)
                {
                    DrawAudioTypeSetting();
                }
            }
            audioPage.EndPage();
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
            using (new EditorGUILayout.VerticalScope())
            {
                if (AudioTypeReorderableList != null && AudioDataContainer.AudioTypes.Count > 0)
                {
                    AudioTypeReorderableList.DoLayoutList();
                }
            }
            SirenixEditorGUI.EndBox();

            GUILayout.EndArea();
            int count = m_audioTypeDeletion.Count;
            for (int i = 0; i < count; i++)
            {
                string deleteType = m_audioTypeDeletion[i];
                if (AudioDataContainer.AudioTypes.Contains(deleteType))
                {
                    AudioDataContainer.AudioTypes.Remove(deleteType);
                }
            }
            m_audioTypeDeletion.Clear();
        }

        private void DrawAudioTypeCallback(Rect rect, int index, bool isActive, bool isFocus)
        {
            if (AudioDataContainer == null || index < 0 || index >= AudioDataContainer.AudioTypes.Count)
            {
                return;
            }
            
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;

            Rect label = new Rect(rect)
            {
                width = rect.width - 15.0f
            };

            string audioType = AudioDataContainer.AudioTypes[index];
            EditorGUI.LabelField(label, audioType);
            if (SirenixEditorGUI.IconButton(rect.AlignRight(25f), EditorIcons.X))
            {
                m_audioTypeDeletion.AddIfNotContains(audioType);
            }
        }

        private void OnSelectAudioTypeElementCallback(ReorderableList list)
        {
            int selectedIndex = list.selectedIndices[0];

            if (AudioDataContainer.TryGetData(AudioDataContainer.AudioTypes[selectedIndex], out AudioAssetData assetData))
            {
                
            }
            else
            {
                //SirenixEditorGUI.Message
            }
        }
    }
}