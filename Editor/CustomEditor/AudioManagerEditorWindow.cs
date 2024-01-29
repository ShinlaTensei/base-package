#region Header
// Date: 12/01/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Base.CustomAttribute;
using Base.Helper;
using Base.Services;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Base.Editor
{
    public class AudioManagerEditorWindow : DataManagerEditorWindow<AudioAssetData>
    {
        protected override string OutputPath
        {
            get => m_outputPath;
            set => m_outputPath = value;
        }

        protected override string EditorConfigName { get; } = "AudioSystemEditorConfig.txt";

        private AudioSystemEditorConfig m_editorConfig;
        protected override IEditorConfig EditorConfig
        {
            get
            {
                if (m_editorConfig == null)
                {
                    m_editorConfig = new AudioSystemEditorConfig();
                }

                return m_editorConfig;
            }
            set => m_editorConfig = (AudioSystemEditorConfig)value;
        }

        private AudioDataContainer AudioDataContainer
        {
            get => m_audioDataContainer;
            set => m_audioDataContainer = value;
        }

        private IList<string> AudioClipID { get; } = new List<string>();

        private List<string> AudioClipEntries { get; set; } = new List<string>();

        [BoxGroup("OutputPath", false), HorizontalGroup("OutputPath/Box")]
        [FolderPath(ParentFolder = "Assets", RequireExistingPath = true, UseBackslashes = true), OdinSerialize]
        [OnValueChanged(nameof(OnOutputPathChanged))]
        private string m_outputPath;

        [OdinSerialize, HorizontalGroup("OutputPath/Box", 0.3f), ReadOnly, HideLabel]
        private AudioDataContainer m_audioDataContainer;

        [MenuItem("BaseFramework/Audio Asset Manager %&a", false, 300)]
        public static void ShowWindow()
        {
            GetWindow<AudioManagerEditorWindow>("Audio Asset Manager");
        }

        protected override void OnOutputPathChanged(string value)
        {
            EditorConfig.OutputPath = value;
            LoadConfigObject();
        }

        protected override void Initialize()
        {
            base.Initialize();

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AudioClipID.AddRange(AssetDatabase.FindAssets("t:audioClip"));
            for (int i = 0; i < AudioClipID.Count; ++i)
            {
                AddressableAssetEntry entry = settings.FindAssetEntry(AudioClipID[i]);
                if (entry != null)
                {
                    AudioClipEntries.Add(entry.address);
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            AudioDataContainer.Revert();
        }

        protected override void DeSerializeConfigData(string stringData)
        {
            if (string.IsNullOrEmpty(stringData))
            {
                return;
            }
            EditorConfig = JsonConvert.DeserializeObject<AudioSystemEditorConfig>(stringData);

            if (EditorConfig != null)
            {
                OutputPath = EditorConfig.OutputPath;
                LoadConfigObject();
            }
        }

        [HorizontalGroup("OutputPath/Box", 0.1f, MarginLeft = 25f), VerticalGroup("OutputPath/Box/Vertical")]
        [Button(ButtonSizes.Small, Name = "Create")]
        protected override void CreateDataContainer()
        {
            AudioDataContainer = AssetDatabaseUtility.LoadOrCreateScriptableObject<AudioDataContainer>(OutputPath, nameof(AudioDataContainer));
        }
        [HorizontalGroup("OutputPath/Box", 0.1f), VerticalGroup("OutputPath/Box/Vertical")]
        [Button(ButtonSizes.Small, Name = "Delete")]
        protected override void DeleteDataContainer()
        {
            AssetDatabaseUtility.DeleteAsset<AudioDataContainer>();
        }

        private void LoadConfigObject()
        {
            AudioDataContainer[] result = AssetDatabaseUtility.LoadAssetsAtPath<AudioDataContainer>(OutputPath, $"t:{nameof(AudioDataContainer)}");
            if (result.Length > 0)
            {
                AudioDataContainer = result[0];
            }
        }

        [HorizontalGroup("CoreButton", 150f, PaddingLeft = 0.65f, Gap = 10f), Button("Save", ButtonSizes.Medium),
         GUIColor("green"), PropertySpace(5f, 5f)]
        private void SaveEditor()
        {
            if (m_audioAssetData != null)
            {
                if (!AudioDataContainer.WorkingCopy.ContainsT(m_audioAssetData))
                {
                    AudioDataContainer.WorkingCopy.Add(m_audioAssetData);
                }
                else
                {
                    AudioAssetData value = AudioDataContainer.WorkingCopy.Get(m_audioAssetData);
                    if (value != null)
                    {
                        value.CopyData(m_audioAssetData);
                    }
                }
            }
            AudioDataContainer.Save();
        }

        [HorizontalGroup("CoreButton", 150f), Button("Discard", ButtonSizes.Medium), GUIColor("red"), PropertySpace(5f, 5f)]
        private void DiscardEditor()
        {
            AudioDataContainer.Revert();

            AudioDataContainer.TryGetData(m_audioType, out AudioAssetData audioAssetData);

            if (audioAssetData != null)
            {
                m_audioAssetData.CopyData(audioAssetData);
            }
        }

        [TabGroup("Tab", "Data", Order = 2), OdinSerialize, HorizontalGroup("Tab/Data/Horizontal", .3f)]
        [ValueDropdown(nameof(GetAudioType)), OnValueChanged(nameof(OnAudioTypeChanged))]
        private string m_audioType;

        [TabGroup("Tab", "Data", Order = 2), HorizontalGroup("Tab/Data/Horizontal2", .5f), OnInspectorGUI(Append = nameof(OnAudioAssetDataGUI))]
        [OdinSerialize, ShowIf("@m_audioType != null"), InlineProperty, HideLabel, PropertySpace(10f)]
        private AudioAssetData m_audioAssetData = null;

        [TabGroup("Tab", "Data", Order = 2), HorizontalGroup("Tab/Data/Horizontal2", .45f, MarginLeft = 25f)]
        [OdinSerialize, HideLabel, ShowIf("@this.m_previewAudio != null"), OnInspectorGUI(Append = nameof(AppendPreviewAudioGUI))]
        private AudioClip m_previewAudio;

        private void AppendPreviewAudioGUI(InspectorProperty property)
        {
            SirenixEditorGUI.BeginHorizontalPropertyLayout(null);
            if (SirenixEditorGUI.IconButton(EditorIcons.Play, 50, 50))
            {
                if (AudioEditorUtility.IsPreviewClipPlaying())
                {
                    AudioEditorUtility.StopAllClips();
                }
                AudioEditorUtility.PlayAudio(m_previewAudio);
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Stop, 50, 50))
            {
                AudioEditorUtility.StopAllClips();
            }

            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }

        private IEnumerable GetAudioType()
        {
            ValueDropdownList<string> result = new ValueDropdownList<string>();
            for (int i = 0; i < AudioDataContainer.AudioTypes.Count; ++i)
            {
                result.Add(AudioDataContainer.AudioTypes[i]);
            }

            return result;
        }

        private void OnAudioAssetDataGUI(InspectorProperty property)
        {
            SirenixEditorGUI.DrawVerticalLineSeperator(property.LastDrawnValueRect.width + 20, property.LastDrawnValueRect.y, property
                                                                                                                             .LastDrawnValueRect.height, 1f);
        }

        private void OnAudioTypeChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            m_audioAssetData = null;
            AudioAssetData audioAssetData = AudioDataContainer.WorkingCopy.Find(item => item.ObjectName.Equals(value));

            if (audioAssetData != null)
            {
                m_audioAssetData = AudioDataContainer.GetDataCopy(audioAssetData);
            }
            m_audioAssetData?.Remove();
            m_audioAssetData ??= new AudioAssetData(AudioDataContainer.WorkingCopy.Count, value, GUID.Generate().ToString());
            m_audioAssetData.Subscribe(OnSelectClipHandler);
            m_audioAssetData.SetClipNameValueDropdown(AudioClipEntries);

            AudioDataContainer.WorkingCopy.AddIfNotContainsT(m_audioAssetData);
        }

        private void OnSelectClipHandler(string clipName)
        {
            AddressableAssetSettings setting = AddressableAssetSettingsDefaultObject.Settings;
            foreach (var clipId in AudioClipID)
            {
                AddressableAssetEntry entry = setting.FindAssetEntry(clipId);
                if (entry != null && entry.address.Equals(clipName))
                {
                    AudioClip clip = AssetDatabaseUtility.LoadAssetFromGuid<AudioClip>(clipId);
                    if (clip != null)
                    {
                        m_previewAudio = clip;
                        break;
                    }
                }
            }
        }


        [TabGroup("Tab", "Settings"), Title("Audio Type")]
        [OdinSerialize, HideLabel, HorizontalGroup("Tab/Settings/Horizontal", .3f)]
        private List<string> AudioTypeSettings
        {
            get => AudioDataContainer.AudioTypes;
            set => AudioDataContainer.AudioTypes = value;
        }

        [TabGroup("Tab", "Settings"), Title("Audio Key Settings")]
        [OdinSerialize, HideLabel, HorizontalGroup("Tab/Settings/Horizontal", .3f, PaddingLeft = 10f),
         ListItemSelector(nameof(SelectAudioKey))]
        [ListDrawerSettings(HideAddButton = true, CustomRemoveElementFunction = nameof(CustomRemoveElementKeySettings), 
                            OnTitleBarGUI = nameof(DrawCustomTitleBarGUI))]
        [OnInspectorGUI(nameof(CustomKeySettingGUI)), CustomValueDrawer(nameof(CustomListItemGUI))]
        private List<string> AudioKeySettings
        {
            get => AudioDataContainer.AudioKeySettings;
            set => AudioDataContainer.AudioKeySettings = value;
        }

        [TabGroup("Tab", "Settings"), Title("Audio Setting")]
        [OdinSerialize, HideLabel, HorizontalGroup("Tab/Settings/Horizontal", .3f, PaddingLeft = 10f), ShowIf("@this.AudioSetting != null")]
        [OnValueChanged(nameof(OnAudioSettingUpdate)), InlineProperty]
        private AudioSetting AudioSetting { get; set; }

        private int  m_selectedIndex;
        private bool m_isAddClicked = false;

        private void CustomRemoveElementKeySettings(string removeElement, InspectorProperty property, List<string> list)
        {
            list.Remove(removeElement);
            if (AudioDataContainer.AudioSettingMap.Remove(removeElement))
            {
                AudioSetting = null;
            }
        }

        private void DrawCustomTitleBarGUI()
        {
            GUIContent addContent = new GUIContent("Add");
            if (SirenixEditorGUI.ToolbarButton(addContent))
            {
                m_isAddClicked = !m_isAddClicked;
            }
        }
        
        private string CustomListItemGUI(string item, GUIContent label)
        {
            EditorGUILayout.LabelField(item);
            return item;
        }

        private string value = string.Empty;
        private void CustomKeySettingGUI(InspectorProperty property)
        {
            if (m_isAddClicked)
            {
                GUIContent addContent = new GUIContent("Add");
                SirenixEditorGUI.BeginHorizontalPropertyLayout(null);
                SirenixEditorGUI.BeginBox(null);
                value = SirenixEditorFields.TextField("Key Name", value);
                if (GUILayout.Button(addContent))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        AudioKeySettings.AddIfNotContains(value);
                        //SelectAudioKey(AudioKeySettings.Count - 1);
                        m_isAddClicked = false;
                    }
                }
                SirenixEditorGUI.EndBox();
                SirenixEditorGUI.EndHorizontalPropertyLayout();
            }
        }

        private void SelectAudioKey(int index)
        {
            AudioSetting    = null;
            index           = Mathf.Clamp(index, 0, AudioKeySettings.Count - 1);
            m_selectedIndex = index;
            AudioSetting    = AudioDataContainer.GetSetting<AudioSetting>(AudioKeySettings[index]);
        }

        private void OnAudioSettingUpdate()
        {
            if (AudioSetting != null)
            {
                AudioDataContainer.AddSetting(AudioKeySettings[m_selectedIndex], AudioSetting);
            }
        }
    }
    
    [Serializable]
    public record AudioSystemEditorConfig : IEditorConfig
    {
        [OdinSerialize] public string OutputPath { get; set; }
    }
}