using System;
using Base.Core;
using Base.Helper;
using Base.Module;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Base.Editor
{
    public abstract class DataManagerEditorWindow<T> : OdinEditorWindow where T : class, IDataObject
    {
        protected abstract string OutputPath       { get; set; }
        protected abstract string         EditorConfigName { get; }

        protected abstract IEditorConfig EditorConfig { get; set; }

        protected override void Initialize()
        {
            if (!string.IsNullOrEmpty(EditorConfigName))
            {
                DeSerializeConfigData(LoadEditorConfig());
            }
        }

        protected override void OnDisable()
        {
            SerializeConfigData(JsonConvert.SerializeObject(EditorConfig));
        }

        protected abstract void OnOutputPathChanged(string value);
        protected abstract void DeSerializeConfigData(string stringData);
        protected abstract void CreateDataContainer();
        protected abstract void DeleteDataContainer();
        private void SerializeConfigData(string data)
        {
            string pathToConfig = EditorConstant.EDITOR_CONFIG_PATH;
            AssetDatabaseUtility.EnsureFolderExits(ref pathToConfig);
            pathToConfig = PathUtility.Combine(Application.dataPath, EditorConstant.EDITOR_CONFIG_PATH, EditorConfigName);
            FileUtilities.SaveTextFile(pathToConfig, data);
        }

        private string LoadEditorConfig()
        {
            string pathToConfig = PathUtility.Combine(Application.dataPath, EditorConstant.EDITOR_CONFIG_PATH, EditorConfigName);
            if (!FileUtilities.HasFile(pathToConfig))
            {
                return string.Empty;
            }
            string textResource = FileUtilities.LoadTextFile(pathToConfig);
            return textResource;
        }
    }

    public interface IEditorConfig
    {
        public string OutputPath { get; set; }
    }
}