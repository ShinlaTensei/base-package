#region Header
// Date: 12/01/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System;
using System.IO;
using Base.Module;
using Base.Services;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
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
        private AudioSystemEditorConfig EditorConfig
        {
            get
            {
                if (m_editorConfig == null)
                {
                    m_editorConfig = new AudioSystemEditorConfig();
                }

                return m_editorConfig;
            }
            set => m_editorConfig = value;
        }

        private string GetAssetPath() => $"{OutputPath}/{nameof(AudioDataContainer)}.asset";

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
            m_window         = GetWindow<AudioManagerEditorWindow>("Audio Asset Manager");
            m_window.minSize = new Vector2(1000f, 500f);
        }

        protected override void OnOutputPathChanged(string value)
        {
            EditorConfig.OutputPath = value;
            
            LoadConfigObject();
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
            }
        }
        
        [HorizontalGroup("OutputPath/Box", 0.1f, MarginLeft = 25f), VerticalGroup("OutputPath/Box/Vertical")]
        [Button(ButtonSizes.Small, Name = "Create"), GUIColor("green")]
        protected override void CreateDataContainer()
        {
            AudioDataContainer = AssetDatabaseUtility.LoadOrCreateScriptableObject<AudioDataContainer>(OutputPath, nameof(AudioDataContainer));
        }
        [HorizontalGroup("OutputPath/Box", 0.1f), VerticalGroup("OutputPath/Box/Vertical")]
        [Button(ButtonSizes.Small, Name = "Delete"), GUIColor("red")]
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
    }
    
    [Serializable]
    public record AudioSystemEditorConfig
    {
        [OdinSerialize] public string OutputPath { get; set; }
    }
}