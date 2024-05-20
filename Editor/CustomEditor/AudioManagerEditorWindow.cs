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
        
        private Rect TabContentRect { get; set; }
        private Rect TabHeaderRect { get; set; }

        private int m_crrTabIndex = 0;
        
        // -------------------- Private Method ---------------------------------

        private void ShowTabContent(in int tabIndex)
        {
            GUILayout.BeginArea(TabContentRect);
            switch (tabIndex)
            {
                case 0:
                    EditorGUI.LabelField(new Rect(0, 0, TabContentRect.width, 25f), "test");
                    break;
                case 1:
                    EditorGUI.LabelField(new Rect(0, 0, TabContentRect.width, 25f), "test test ");
                    break;
            }
            GUILayout.EndArea();
        }

        // ------------------- Inherited method -------------------------------------

        protected override void Initialize()
        {
            base.Initialize();

            window = this;
            
            TabContentRect = new (0, OUTPUT_PATH_HEADER + SPACE + CONTENT_HEADER, window.minSize.x ,
                window.position.height - OUTPUT_PATH_HEADER - SPACE - CONTENT_HEADER);
            
            TabHeaderRect =  new Rect(0, OUTPUT_PATH_HEADER + SPACE, window.minSize.x, CONTENT_HEADER);
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void DrawTabs()
        {
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