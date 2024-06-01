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
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

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
        private const float MIN_VOLUME = 0;
        private const float MAX_VOLUME = 1;
        
        private Rect TabContentRect { get; set; }
        private Rect TabHeaderRect { get; set; }
        private AudioDataContainer AudioContainer => DataContainer as AudioDataContainer;

        private int m_crrTabIndex = 0;
        private string m_crrSelectedAudioType = string.Empty;
        private AudioAssetData m_crrSelectedAudioAsset = null;
        private GUIStyle m_dropAreaStyle;
        private AudioClip m_selectedAudioClip;
        
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

        private void DrawAudioTypeSelect(Rect rect, ref string crrValue, bool hasCommonValue = false)
        {
            List<string> audioTypes = new List<string>();
            if (hasCommonValue) audioTypes.AddIfNotContains(ALL);
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
            
            DrawAudioTypeSelect(audioTypeSelectRect, ref m_crrSelectedAudioType, true);
            
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
            SirenixEditorGUI.ToolbarSearchField(string.Empty, false);
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

            GUI.enabled = m_crrSelectedAudioAsset != null;
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Minus))
            {
                AudioContainer.WorkingCopy.Remove(m_crrSelectedAudioAsset);
                m_crrSelectedAudioAsset = null;
            }
            GUI.enabled = true;
            
            SirenixEditorGUI.EndToolbarBoxHeader();
            GUILayout.EndArea();
            // Draw audio asset data detail
            DrawAssetDataDetail();
        }
        
        private void DrawAssetDataDetail()
        {
            if (m_crrSelectedAudioAsset == null) return;
            
            Rect detailAreaRect = new Rect(270f, 30f, TabContentRect.width - 270f, TabContentRect.height - 30f);
            GUILayout.BeginArea(detailAreaRect);
            
            // Draw separator
            EditorGUI.DrawRect(detailAreaRect.SetWidth(2f).SetX(0).SetY(-10f), PEditorStyles.SeparatorColorBlack);
            
            Rect detailHeaderRect = new Rect(10f, 5f, detailAreaRect.width - 20f, SPACE);
            EditorGUI.DrawRect(detailHeaderRect, PEditorStyles.DefaultCollectionHeaderColor);
            // Name
            EditorGUI.LabelField(detailHeaderRect.AddX(15f).AddY(15f).SetWidth(50f).SetHeight(25f), "Name");
            Rect nameRect = detailHeaderRect.AddX(70f).AddY(15f).SetWidth(90f).SetHeight(25f);
            
            string newName = SirenixEditorFields.TextField(nameRect, m_crrSelectedAudioAsset.ObjectName);
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
            
            DrawAssetList();

            GUILayout.EndArea();
        }

        private void DrawAssetList()
        {
            Rect dropArea = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 4f, EditorStyles.helpBox,
                GUILayout.MaxWidth(TabContentRect.width - 300f)).AddY(70f).AddX(15f);
            DrawClipDropArea(dropArea);
            
            DrawAudioRef();
        }
        
        /// <summary>
        /// Draws the area to drag and drop clips to.
        /// </summary>
        /// <param name="dropArea">The area to draw the drop box in.</param>
        private void DrawClipDropArea(Rect dropArea)
        {
            if (m_dropAreaStyle == null)
            {
                m_dropAreaStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            GUI.Box(dropArea, "Drag any clip(s) here to add them to this AudioAsset", m_dropAreaStyle);
            GUI.backgroundColor = col;

            HandleClipUserInput(dropArea);
        }

        private void DrawAudioRef()
        {
            if (string.IsNullOrEmpty(m_crrSelectedAudioAsset.AssetGuid))
            {
                return;
            }
            
            if (m_selectedAudioClip == null)
            {
                m_selectedAudioClip = AssetDatabaseUtility.LoadAssetFromGuid<AudioClip>(m_crrSelectedAudioAsset.AssetGuid);
            }
            
            Object clip = SirenixEditorFields.UnityObjectField(new Rect(15f, 175f, 250f, 25f), GUIContent.none, m_selectedAudioClip,
                    m_selectedAudioClip.GetType(), false);

            if (!AddressableUtility.IsAddressable(clip))
            {
                return;
            }
            
            if (!m_selectedAudioClip.Equals(clip) && clip != null)
            {
                m_selectedAudioClip = (AudioClip) clip;
                m_crrSelectedAudioAsset.ClipAssetKey = AddressableUtility.GetAddressFromObject(clip);
                m_crrSelectedAudioAsset.AssetGuid = AssetDatabaseUtility.GetAssetGuid(clip);
            }
            else
            {
                string addressableKey = AddressableUtility.GetAddressFromObject(m_selectedAudioClip);
                m_crrSelectedAudioAsset.ClipAssetKey = addressableKey;
            }
            
            EditorGUI.LabelField(new Rect(325f, 175f, 50f, 25f), "Volume");
            float crrVolume = EditorGUI.Slider(new Rect(400f, 175f, 250f, 25f), m_crrSelectedAudioAsset.Volume, MIN_VOLUME, MAX_VOLUME);
            if (Mathf.Abs(crrVolume - m_crrSelectedAudioAsset.Volume) != 0)
            {
                m_crrSelectedAudioAsset.Volume = crrVolume;
            }
        }

        /// <summary>
        /// Handles the drag and drop user input within the specified rectangle.
        /// </summary>
        /// <param name="dropArea">The screen area to use as drop area.</param>
        private void HandleClipUserInput(Rect dropArea)
        {
            Event evt = Event.current;
            if (evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform
                || !dropArea.Contains(evt.mousePosition))
            {
                return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type != EventType.DragPerform)
            {
                return;
            }

            DragAndDrop.AcceptDrag();
            foreach (UnityEngine.Object droppedObject in DragAndDrop.objectReferences)
            {
                HandleDroppedObject(droppedObject);
            }

            evt.Use();
        }

        private void HandleDroppedObject(Object droppedObject)
        {
            AudioClip clip = droppedObject as AudioClip;

            if (clip == null || !AddressableUtility.IsAddressable(clip))
            {
                return;
            }

            string address = AddressableUtility.GetAddressFromObject(droppedObject);
            string guild = AssetDatabaseUtility.GetAssetGuid(clip);

            m_crrSelectedAudioAsset.AssetGuid = guild;
            m_crrSelectedAudioAsset.ClipAssetKey = address;
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
            // Draw save/discard button
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(700f, OUTPUT_PATH_HEADER + 20f, 125f, 25f), "Save"))
            {
                AudioContainer.WorkingCopy[m_crrSelectedAudioAsset.Index - 1] = m_crrSelectedAudioAsset;
                AudioContainer.Save();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(850f, OUTPUT_PATH_HEADER + 20f, 125f, 25f), "Discard"))
            {
                AudioContainer.Revert();
                m_crrSelectedAudioAsset = null;
                m_selectedAudioClip = null;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = originalColor;
            
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
        
        protected override string LoadOutputPath()
        {
            return EditorPrefs.GetString(EditorConstant.EDITOR_OUTPUT_PATH_KEY, string.Empty);
        }
    }
}