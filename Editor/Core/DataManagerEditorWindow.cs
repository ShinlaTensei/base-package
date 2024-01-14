using System;
using Base.Core;
using Base.Helper;
using Base.Module;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Base.Editor
{
    public abstract class DataManagerEditorWindow<T> : OdinEditorWindow where T : class, IDataObject
    {
        /// <summary>
        /// Static reference to the window itself to avoid multiple memory allocations when opening and closing the window.
        /// </summary>
        protected static DataManagerEditorWindow<T> m_window;
        protected abstract string OutputPath       { get; set; }
        protected abstract string         EditorConfigName { get; } 

        protected override void Initialize()
        {
            if (!string.IsNullOrEmpty(EditorConfigName))
            {
                DeSerializeConfigData(LoadEditorConfig());
            }
        }

        protected abstract void OnOutputPathChanged(string value);
        protected abstract void DeSerializeConfigData(string stringData);
        protected abstract void CreateDataContainer();
        protected abstract void DeleteDataContainer();
        protected void SerializeConfigData(string data)
        {
            string pathToConfig = PathUtility.Combine(Application.dataPath, EditorConstant.EDITOR_CONFIG_PATH, EditorConfigName);
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
}