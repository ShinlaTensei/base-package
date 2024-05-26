#region Header
// Date: 18/02/2024
// Created by: Huynh Phong Tran
// File name: AudioManagerEditorWindow.cs
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const float NAME_LIST_HEADER_SIZE = 22f;
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

        private void DrawAudioTypeSelect(Rect rect, ref string crrValue)
        {
            List<string> audioTypes = new List<string>();
            audioTypes.AddIfNotContains(ALL);
            foreach (var audioType in AudioContainer.AudioTypes)
            {
                audioTypes.AddIfNotContains(audioType);
            }
            
            if (string.IsNullOrEmpty(crrValue))
            {
                crrValue = audioTypes[0];
            }
            
            string newAudioType = SirenixEditorFields.Dropdown(rect, GUIContent.none, crrValue, audioTypes);
            if (!crrValue.Equals(newAudioType))
            {
                crrValue = newAudioType;
            }
        }

        private void DrawAudioAsset()
        {
            if (AudioContainer == null) return;
            Rect audioTypeSelectRect = new Rect(10f, 10f, 125f, 25f);
            
            DrawAudioTypeSelect(audioTypeSelectRect, ref m_crrSelectedAudioType);
            
            List<string> audioNames = new List<string>();
            foreach (var audioAsset in AudioContainer.WorkingCopy)
            {
                if (audioAsset.Type.Equals(m_crrSelectedAudioType) || m_crrSelectedAudioType.Equals(ALL))
                {
                    audioNames.AddIfNotContains(audioAsset.ObjectName);
                }
            }

            ReorderableList audioNameList = new ReorderableList(audioNames, typeof(string), false, false, false, false)
            {
                multiSelect = false,
                onSelectCallback = OnSelectAudioAssetCallback,
                drawElementBackgroundCallback = OnDrawElementBackgroundCallback,
            };

            Rect audioNameListRect = audioTypeSelectRect.AddY(50f).SetWidth(250f);
            if (audioNames.Count >= AMOUNT_TO_SCROLLABLE)
            {
                
            }
            else
            {
                audioNameList.DoList(audioNameListRect);
            }
            
            // Draw Audio Name list
            // Draw header
            GUILayout.BeginArea(audioNameListRect.SubY(NAME_LIST_HEADER_SIZE));
            SirenixEditorGUI.BeginToolbarBoxHeader(NAME_LIST_HEADER_SIZE);
            GUILayout.Label("Audio Assets");
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                AudioAssetData assetData = new AudioAssetData()
                {
                    Type = m_crrSelectedAudioType.Equals(ALL) ? string.Empty : m_crrSelectedAudioType,
                    Guid = GUID.Generate().ToString(),
                    Index = audioNames.Count,
                    ObjectName = string.Empty
                };
                AudioContainer.WorkingCopy.AddIfNotContainsT(assetData);
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Minus))
            {
                
            }
            SirenixEditorGUI.EndToolbarBoxHeader();
            GUILayout.EndArea();
            
            // Draw audio asset data detail
            DrawAssetDataDetail();
        }

        private AudioClip m_selectClip = null;
        private void DrawAssetDataDetail()
        {
            if (m_crrSelectedAudioAsset == null) return;
            
            Rect detailAreaRect = new Rect(270f, 30f, TabContentRect.width - 270f, TabContentRect.height - 30f);
            GUILayout.BeginArea(detailAreaRect);
            
            EditorGUI.DrawRect(detailAreaRect.SetWidth(2f).SetX(0).SetY(-10f), PEditorStyles.SeparatorColorBlack);
            Rect detailHeaderRect = new Rect(10f, 5f, detailAreaRect.width - 20f, 60f);
            EditorGUI.DrawRect(detailHeaderRect, PEditorStyles.DefaultCollectionHeaderColor);
            // Name
            EditorGUI.LabelField(detailHeaderRect.AddX(15f).AddY(15f).SetWidth(50f).SetHeight(25f), "Name");
            Rect nameRect = detailHeaderRect.AddX(70f).AddY(15f).SetWidth(75f).SetHeight(25f);
            string newName = SirenixEditorFields.TextField(detailHeaderRect.AddX(70f).AddY(15f).SetWidth(75f).SetHeight(25f),
                m_crrSelectedAudioAsset.ObjectName);
            if (!m_crrSelectedAudioAsset.ObjectName.Equals(newName))
            {
                m_crrSelectedAudioAsset.ObjectName = newName;
            }
            // Type
            EditorGUI.LabelField(nameRect.AddX(125f).SetWidth(50f).SetHeight(25f), "Type");
            string audioType = m_crrSelectedAudioAsset.Type;
            Rect typeRect = nameRect.AddX(165f).SetWidth(150f).SetHeight(25f).AddY(5f);
            DrawAudioTypeSelect(nameRect.AddX(165f).SetWidth(150f).SetHeight(25f).AddY(5f), ref audioType);
            m_crrSelectedAudioAsset.Type = audioType;
            // GUID
            EditorGUI.LabelField(typeRect.AddX(typeRect.width + 50f).SetY(20f), "GUID:");
            EditorGUI.LabelField(typeRect.AddX(typeRect.width + 85f).SubY(5f).SetWidth(250f), m_crrSelectedAudioAsset.Guid);
            
            GUILayout.EndArea();
        }

        private void OnSelectAudioAssetCallback(ReorderableList list)
        {
            int crrSelectedIndex = list.selectedIndices[0];
            if (list.list is not List<string> nameList) return;
            
            string assetName = nameList[crrSelectedIndex];

            m_crrSelectedAudioAsset = AudioContainer.WorkingCopy.Find(item => item.ObjectName.Equals(assetName));
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
                pix[i] = isSelected ? PEditorStyles.SelectedDarkGreyColor : PEditorStyles.DefaultEditorDarkColor;
          
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
            m_crrSelectedAudioAsset = null;
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