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
        
        // -------------------- Private Method ---------------------------------

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
            Rect contentRect = new Rect(0, OUTPUT_PATH_HEADER + SPACE, window.position.width, window.position.height - OUTPUT_PATH_HEADER - SPACE);
            SirenixEditorGUI.DrawSolidRect(contentRect, PEditorStyles.BackgroundColorGrey);

            bool isShow1 = false, isShow2 = false;
            Rect headerRect = new Rect(0, contentRect.y, contentRect.width, CONTENT_HEADER);
            using (new GUILayout.AreaScope(headerRect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Tab 1", GUILayout.MinHeight(headerRect.height)))
                    {
                        isShow1 = true;
                        isShow2 = false;
                    }
                    else if (GUILayout.Button("Tab 2", GUILayout.MinHeight(headerRect.height)))
                    {
                        isShow1 = false;
                        isShow2 = true;
                    }
                }
            }

            if (isShow1)
            {
                EditorGUI.LabelField(new Rect(0, headerRect.y + 10f, contentRect.width, contentRect.height), "test");
            }

            if (isShow2)
            {
                EditorGUI.LabelField(new Rect(0, headerRect.y + 20f, contentRect.width, contentRect.height), "test test ");
            }
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