#region Header
// Date: 12/01/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System;
using System.Collections;
using Base.Helper;
using Base.Services;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        
        [HorizontalGroup("CoreButton", 150f,PaddingLeft = 0.65f, Gap = 10f), Button("Save", ButtonSizes.Medium), 
         GUIColor("green"), PropertySpace(5f, 5f)]
        private void SaveEditor()
        {
            AudioDataContainer.Save();
        }
        
        [HorizontalGroup("CoreButton", 150f), Button("Discard", ButtonSizes.Medium), GUIColor("red"), PropertySpace(5f, 5f)]
        private void DiscardEditor()
        {
            AudioDataContainer.Revert();
        }

        [TabGroup("Tab", "Data", Order = 2), OdinSerialize, HorizontalGroup("Tab/Data/Horizontal", .3f)]
        [ValueDropdown(nameof(GetAudioType)), OnValueChanged(nameof(OnAudioTypeChanged))]
        private string m_audioType;
        
        [TabGroup("Tab", "Data", Order = 2)]
        [SerializeField, ShowIf("@m_audioType != null"), InlineProperty, HideLabel]
        private AudioAssetData m_audioAssetData;

        private IEnumerable GetAudioType()
        {
            ValueDropdownList<string> result = new ValueDropdownList<string>();
            for (int i = 0; i < AudioDataContainer.AudioTypes.Count; ++i)
            {
                result.Add(AudioDataContainer.AudioTypes[i]);
            }

            return result;
        }

        private void OnAudioTypeChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            AudioDataContainer.TryGetData(value, out m_audioAssetData);

            m_audioAssetData ??= new AudioAssetData(AudioDataContainer.WorkingCopy.Count, value, GUID.Generate().ToString());
            
            AudioDataContainer.WorkingCopy.AddIfNotContainsT(m_audioAssetData);
        }


        [TabGroup("Tab", "Settings")]
        [OdinSerialize]
        private string m_test;
    }
    
    [Serializable]
    public record AudioSystemEditorConfig : IEditorConfig
    {
        [OdinSerialize] public string OutputPath { get; set; }
    }
}