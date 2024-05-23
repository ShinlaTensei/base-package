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
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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
            window.minSize = new Vector2(1000f, 200f);
            window.maxSize = new Vector2(1000f, 1000f);
        }
        
        // ------------------- Private Member ------------------------------------

        private const float SPACE = 60f;
        private const float CONTENT_HEADER = 40f;
        private const int AMOUNT_TO_SCROLLABLE = 15;
        private const string ALL = "All";
        
        private Rect TabContentRect { get; set; }
        private Rect TabHeaderRect { get; set; }
        private AudioDataContainer AudioContainer => DataContainer as AudioDataContainer;

        private int m_crrTabIndex = 0;
        private string m_crrSelectedAudioType = string.Empty;
        private AudioAssetData m_crrSelectedAudioAsset = null;

        private ReorderableList m_audioNameList;
        
        // -------------------- Private Method ---------------------------------

        private void ShowTabContent(in int tabIndex)
        {
            GUILayout.BeginArea(TabContentRect);
            switch (tabIndex)
            {
                case 0:
                    DrawAudioAsset();
                    break;
                case 1:
                    EditorGUI.LabelField(new Rect(0, 0, TabContentRect.width, 25f), "test test ");
                    break;
            }
            GUILayout.EndArea();
        }

        private void DrawAudioAsset()
        {
            if (AudioContainer == null) return;
            
            List<string> audioTypes = new List<string>();
            audioTypes.AddIfNotContains(ALL);
            foreach (var audioType in AudioContainer.AudioTypes)
            {
                audioTypes.AddIfNotContains(audioType);
            }
            
            if (string.IsNullOrEmpty(m_crrSelectedAudioType))
            {
                m_crrSelectedAudioType = audioTypes[0];
            }

            Rect audioTypeSelectRect = new Rect(10f, 10f, 125f, 25f);
            string newAudioType = SirenixEditorFields.Dropdown(audioTypeSelectRect, GUIContent.none, m_crrSelectedAudioType, audioTypes);
            if (!m_crrSelectedAudioType.Equals(newAudioType))
            {
                m_crrSelectedAudioType = newAudioType;
            }

            List<string> audioNames = new List<string>();
            foreach (var audioAsset in AudioContainer.WorkingCopy)
            {
                audioNames.AddIfNotContains(audioAsset.ObjectName);
            }

            ReorderableList audioNameList = new ReorderableList(audioNames, typeof(string), true, false, true, true)
            {
                multiSelect = false,
                draggable = true,
                onAddCallback = OnAddAudioAssetCallback,
                onSelectCallback = OnSelectAudioAssetCallback,
                drawElementBackgroundCallback = OnDrawElementBackgroundCallback,
            };

            if (audioNames.Count >= AMOUNT_TO_SCROLLABLE)
            {
                
            }
            else
            {
                audioNameList.DoList(audioTypeSelectRect.AddY(50f).SetWidth(250f));
            }
            
        }

        private void OnAddAudioAssetCallback(ReorderableList list)
        {
            AudioAssetData assetData = new AudioAssetData()
            {
                Type = string.Empty,
                Guid = GUID.Generate().ToString(),
                Index = list.count,
                ObjectName = string.Empty
            };
            AudioContainer.WorkingCopy.AddIfNotContainsT(assetData);
        }

        private void OnSelectAudioAssetCallback(ReorderableList list)
        {
            int crrSelectedIndex = list.selectedIndices[0];

            m_crrSelectedAudioAsset = AudioContainer.WorkingCopy[crrSelectedIndex];
        }

        private void OnDrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            bool isSelected = m_crrSelectedAudioAsset != null && m_crrSelectedAudioAsset.Index == index;
            
            GUIStyle HeaderBackground = new GUIStyle("RL Header");

            Color[] pix = new Color[Mathf.FloorToInt(rect.width * rect.height)];
          
            for (int i = 0; i < pix.Length; i++)
                pix[i] = isSelected ? Color.blue : index % 2 == 0 ? Color.cyan : Color.yellow;
          
            Texture2D result = new Texture2D((int)rect.width, (int)5);
            result.SetPixels(pix);
            result.Apply();
          
            HeaderBackground.normal.background = result;
            EditorGUI.LabelField(rect, "", HeaderBackground);
        }

        // ------------------- Inherited method -------------------------------------

        protected override void Initialize()
        {
            base.Initialize();

            window = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void DrawTabs()
        {
            TabContentRect = new (0, OUTPUT_PATH_HEADER + SPACE + CONTENT_HEADER, window.minSize.x ,
                window.position.height - OUTPUT_PATH_HEADER - SPACE - CONTENT_HEADER);
            
            TabHeaderRect =  new Rect(0, OUTPUT_PATH_HEADER + SPACE, window.minSize.x, CONTENT_HEADER);
            
            SirenixEditorGUI.DrawSolidRect(TabContentRect, PEditorStyles.BackgroundColorGrey);
            
            
            using (new GUILayout.AreaScope(TabHeaderRect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Tab 1", GUILayout.MinHeight(TabHeaderRect.height)))
                    {
                        m_crrTabIndex = 0;
                    }
                    else if (GUILayout.Button("Tab 2", GUILayout.MinHeight(TabHeaderRect.height)))
                    {
                        m_crrTabIndex = 1;
                    }
                }
            }
            
            ShowTabContent(m_crrTabIndex);
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

        protected override void SaveOutputPath(string content)
        {
            string configPath = Path.Combine(PathUtility.GetProjectPath(), EditorConstant.EDITOR_CONFIG_PATH);
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            FileUtilities.SaveTextFile(configPath + $"/{nameof(AudioDataContainer)}.txt", content);
        }
        protected override string LoadOutputPath()
        {
            return EditorPrefs.GetString(nameof(OutputPath), string.Empty);
        }
    }
}