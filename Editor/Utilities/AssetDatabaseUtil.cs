#region Header
// Date: 14/01/2024
// Created by: Huynh Phong Tran
// File name: AssetDatabaseUtil.cs
#endregion

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Editor
{
    public static class AssetDatabaseUtility
    {
        /// <summary>
        /// Checks for existence of a folder within the project Assets folder.
        /// Creates it, and any parent folders containing it if not found.
        /// </summary>
        /// <param name="dir"></param>
        public static void EnsureFolderExits(ref string dir)
        {
            if (!dir.Contains("Assets"))
            {
                dir = $"Assets/{dir}";
            }

            if (new DirectoryInfo($"{Application.dataPath.Replace("Assets/", "")}/{dir}").Exists) return;

            var folderNames = dir.Split('/');

            // Always include "Assets" as first directory
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(folderNames[0]);

            // Start iteration from 2nd folder name.
            for (int i = 1; i < folderNames.Length; i++)
            {
                var fullPath = $"{Application.dataPath.Replace("Assets", "")}/{pathBuilder}/{folderNames[i]}";

                if (!new DirectoryInfo(fullPath).Exists)
                {
                    AssetDatabase.CreateFolder(pathBuilder.ToString(), folderNames[i]);
                    AssetDatabase.Refresh();
                }

                pathBuilder.Append($"/{folderNames[i]}");
            }
        }

        public static T[] LoadAssetsAtPath<T>(string directory, string assetFilter = "") where T : Object
        {
            if (!directory.Contains("Assets/"))
            {
                directory = $"Assets/{directory}";
            }
            //todo add type filter
            string[] fileGuids = AssetDatabase.FindAssets(assetFilter, new[] { directory });
            T[]      results   = LoadAssetsFromGuids<T>(fileGuids);

            return results;
        }

        public static T[] LoadAssetsFromGuids<T>(string[] fileGuids) where T : Object
        {
            T[] results = new T[fileGuids.Length];
            for (int index = 0; index < fileGuids.Length; index++)
            {
                string guid  = fileGuids[index];
                string path  = AssetDatabase.GUIDToAssetPath(guid);
                T      asset = AssetDatabase.LoadAssetAtPath<T>(path);

                results[index] = asset;
            }

            return results;
        }

        public static T LoadOrCreateScriptableObject<T>(string path, string name = "") where T : ScriptableObject
        {
            EnsureFolderExits(ref path);
            T asset = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;

            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();

            return asset;
        }

        public static T LoadAssetOfType<T>(string path, string name = "") where T : UnityEngine.Object
        {
            try
            {
                EnsureFolderExits(ref path);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    return asset;
                }

                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static void CreateAssetOfType<T>(T asset, string path) where T : Object
        {
            try
            {
                EnsureFolderExits(ref path);
                AssetDatabase.CreateAsset(asset, path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }

        public static bool DoesAssetOfTypeExist<T>()
        {
            string[] guid = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            return guid.Length > 0;
        }

        public static void DeleteAsset<T>()
        {
            string[] guid = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (guid.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid[0]);
                AssetDatabase.DeleteAsset(assetPath);
            }
        }
    }
}