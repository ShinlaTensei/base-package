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
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
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
            window.minSize = new Vector2(1000f, 800f);
        }
        // Private field
        private bool         m_isAddTypes              = false;
        private string       m_audioTypesAddTemplate   = string.Empty;
        private List<string> m_audioTypeDeletion       = new List<string>();
        private int          m_selectedAudioTypeIndex  = 0;
        private int          m_selectedAudioAssetIndex = 0;
        private Vector2      m_currentScrollPosition   = Vector2.zero;

        private AudioDataContainer AudioDataContainer => DataContainer as AudioDataContainer;
        
        private ReorderableList AudioTypeReorderableList { get; set; }
        
        private AudioSettingData AudioSettingData { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            ResetDefault();
            InitAfterLoadReference();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (AudioDataContainer != null)
            {
                AudioDataContainer.WorkingCopy.Clear();
            }
            
            AudioEditorUtility.StopAllClips();
        }

        private void ResetDefault()
        {
            m_selectedAudioTypeIndex  = 0;
            m_selectedAudioAssetIndex = 0;
            AudioSettingData          = null;
        }

        protected override void InitAfterLoadReference()
        {
            if (AudioDataContainer != null)
            {
                AudioTypeReorderableList = new ReorderableList(AudioDataContainer.AudioTypes, typeof(string), false, false,
                                                               false, false)
                                           {
                                                   drawElementCallback = DrawAudioTypeCallback,
                                                   multiSelect         = false,
                                                   onSelectCallback    = OnSelectAudioTypeElementCallback,
                                                   index = m_selectedAudioTypeIndex
                                           };
                
                if (m_selectedAudioTypeIndex >= 0 && m_selectedAudioTypeIndex < AudioDataContainer.AudioTypes.Count)
                {
                    AudioTypeReorderableList.Select(m_selectedAudioTypeIndex);
                    
                    OnSelectAudioTypeElementCallback(AudioTypeReorderableList);
                }
                
                if (DataObject != null && m_selectedAudioAssetIndex >= 0 && m_selectedAudioAssetIndex < DataObject.ClipAssetKeys.Count)
                {
                    DataReorderableList.Select(m_selectedAudioAssetIndex);

                    OnSelectAudioAssetKeyCallback(DataReorderableList);
                }
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
                    Rect areRect = DrawAudioTypeSetting();
                    Rect dataObjectRect = DrawDataObject(areRect);
                    DrawAudioSettingObject(dataObjectRect);
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
        
        /// <summary>
        /// Draw Audio Type
        /// </summary>
        /// <returns>The Rect of AudioType GUI</returns>
        private Rect DrawAudioTypeSetting()
        {
            if (AudioDataContainer == null) return new Rect();

            Rect areRect = new Rect(5f, 5f, TabGroup.InnerRect.width / 4f, TabGroup.InnerRect.height);
            
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
            // Remove an audio type
            // Should remove the data container as well
            int count = m_audioTypeDeletion.Count;
            for (int i = 0; i < count; i++)
            {
                string deleteType = m_audioTypeDeletion[i];
                if (AudioDataContainer.AudioTypes.Contains(deleteType))
                {
                    int index = AudioDataContainer.AudioTypes.IndexOf(deleteType);
                    AudioDataContainer.AudioTypes.Remove(deleteType);
                    // Check and remove from data container and working copy collection
                    if (index >= 0 && index < AudioDataContainer.WorkingCopy.Count)
                    {
                        AudioDataContainer.WorkingCopy.RemoveAt(index);
                    }
                }
                if (DataObject != null && DataObject.ObjectName.Equals(deleteType))
                {
                    DataObject = null;
                }
            }
            m_audioTypeDeletion.Clear();

            return areRect;
        }
        
        private Rect DrawDataObject(in Rect previousRect)
        {
            if (DataObject == null)
                return previousRect;

            Rect dataObjectRect = previousRect.AddX(previousRect.width + 10f).SetWidth(TabGroup.InnerRect.width - previousRect.width - 20f);
            dataObjectRect = dataObjectRect.SetWidth(dataObjectRect.width / 1.75f);
            GUILayout.BeginArea(dataObjectRect);
            SirenixEditorGUI.BeginBox(GUILayout.MinHeight(dataObjectRect.height - 250f));
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5f);
            AudioAssetData audioAssetData = AudioDataContainer.WorkingCopy[m_selectedAudioTypeIndex];
            EditorGUILayout.LabelField("AssetID", audioAssetData.Guid, SirenixGUIStyles.BoldLabel);
            GUILayout.Space(10f);
            if (DataReorderableList != null)
            {
                DataReorderableList.list = audioAssetData.ClipAssetKeys;
                m_currentScrollPosition  = EditorGUILayout.BeginScrollView(m_currentScrollPosition);
                DataReorderableList.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
            SirenixEditorGUI.EndBox();
            GUILayout.EndArea();
            return dataObjectRect;
        }

        private void DrawAudioSettingObject(in Rect previousRect)
        {
            if (AudioSettingData is null) return;
            
            Rect rect = previousRect.AddX(previousRect.width + 10f).SetWidth(TabGroup.InnerRect.width - previousRect.width - 60f);
            rect = rect.SetWidth(rect.width / 1.75f);
            GUILayout.BeginArea(rect);
            SirenixEditorGUI.BeginBox();
            EditorGUI.BeginChangeCheck();
            float volume      = EditorGUILayout.Slider("Volume", AudioSettingData.Volume, 0, 1f);
            bool playOneShot = EditorGUILayout.Toggle("Oneshot", AudioSettingData.PlayOneShot);
            if (EditorGUI.EndChangeCheck())
            {
                AudioSettingData.Volume      = volume;
                AudioSettingData.PlayOneShot = playOneShot;

                AudioAssetData audioAssetData = AudioDataContainer.WorkingCopy.Find(item => item.Guid.Equals(DataObject.Guid));
                audioAssetData.SettingDataMap[audioAssetData.ClipAssetKeys[m_selectedAudioAssetIndex]] = AudioSettingData;
            }
            if (SirenixEditorGUI.IconButton(EditorIcons.Play))
            {
                string   clipId = DataObject.ClipAssetKeys[m_selectedAudioAssetIndex];
                string[] guids  = AssetDatabase.FindAssets(clipId);
                string   guid   = guids.Length > 0 ? guids[0] : string.Empty;
                if (!string.IsNullOrEmpty(guid))
                {
                    AudioClip clip = AssetDatabaseUtility.LoadAssetFromGuid<AudioClip>(guid);
                    AudioEditorUtility.PlayAudio(clip);
                }
            }
            SirenixEditorGUI.EndBox();
            GUILayout.EndArea();
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
            m_selectedAudioTypeIndex = list.selectedIndices[0];

            if (AudioDataContainer.TryGetData(AudioDataContainer.AudioTypes[m_selectedAudioTypeIndex], out AudioAssetData assetData))
            {
                DataObject = assetData;
            }
            else
            {
                DataObject = new AudioAssetData(AudioDataContainer.WorkingCopy.Count, AudioDataContainer.AudioTypes[m_selectedAudioTypeIndex],
                                                GUID.Generate().ToString());
            }
            AudioDataContainer.WorkingCopy.AddIfNotContainsT(DataObject);
            DataObject = AudioDataContainer.WorkingCopy[m_selectedAudioTypeIndex];

            DataReorderableList = new ReorderableList(DataObject.ClipAssetKeys, typeof(string), true, true, true, true)
                                  {
                                          onAddCallback      = OnAddAudioAssetKeyCallback,
                                          onRemoveCallback   = OnRemoveAudioAssetKeyCallback,
                                          drawHeaderCallback = DrawAudioAssetHeaderCallback,
                                          onSelectCallback   = OnSelectAudioAssetKeyCallback,
                                  };
        }

        private void DrawAudioAssetHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect.SetWidth(100f).SetHeight(20f), nameof(DataObject.ClipAssetKeys));
        }

        private void OnAddAudioAssetKeyCallback(ReorderableList list)
        {
            AddressableAssetSettings settings       = AddressableAssetSettingsDefaultObject.Settings;
            string[]                 audioClipId    = AssetDatabase.FindAssets("t:audioClip");
            GenericMenu              genericMenu    = new GenericMenu();
            AudioAssetData           audioAssetData = AudioDataContainer.WorkingCopy[m_selectedAudioTypeIndex];
            for (int i = 0; i < audioClipId.Length; ++i)
            {
                AddressableAssetEntry entry = settings.FindAssetEntry(audioClipId[i]);
                if (entry != null && !audioAssetData.ClipAssetKeys.Contains(entry.address))
                {
                    genericMenu.AddItem(new GUIContent(entry.address), false, () =>
                                                                              {
                                                                                  audioAssetData.ClipAssetKeys.Add(entry.address);
                                                                              });
                }
            }
            
            genericMenu.ShowAsContext();
        }
        
        private void OnRemoveAudioAssetKeyCallback(ReorderableList list)
        {
            AudioAssetData audioAssetData = AudioDataContainer.WorkingCopy[m_selectedAudioTypeIndex];
            audioAssetData.ClipAssetKeys.RemoveAt(m_selectedAudioAssetIndex);
        }

        private void OnSelectAudioAssetKeyCallback(ReorderableList list)
        {
            if (list.selectedIndices.Count <= 0)
                return;
            
            m_selectedAudioAssetIndex = list.selectedIndices[0];

            AudioSettingData = DataObject.CreateOrGetSettingData(DataObject.ClipAssetKeys[m_selectedAudioAssetIndex]);
        }
    }
}