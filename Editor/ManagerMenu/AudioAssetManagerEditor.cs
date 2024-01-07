using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base.CustomAttribute;
using Base.Services;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class AudioAssetManagerEditor : OdinEditorWindow
{
    [MenuItem("Base Framework/Audio Asset Manager")]
    private static void OpenWindow()
    {
        GetWindow<AudioAssetManagerEditor>().Show();
    }
    
    [FolderPath(RequireExistingPath = true, ParentFolder = "Assets"), BoxGroup(GroupName = "OutputPath"), 
     ValidateInput("ValidateOutputPath", "$m_validateOutputMessage")]
    public string OutputPath;
    private string m_validateOutputMessage;
    private bool   m_isValidate    = false;
    private string m_previousPath = string.Empty;
    [UsedImplicitly]
    private bool ValidateOutputPath(string path)
    {
        if (m_isValidate && m_previousPath.Equals(path))
        {
            return true;
        }
        
        m_isValidate = false;
        if (string.IsNullOrEmpty(path))
        {
            m_validateOutputMessage = "The provided path is invalid: Null or Empty";
            m_isValidate            = false;
            return false;
        }

        string finalPath = $"{Application.dataPath}/{path}";

        AudioMappingConfiguration asset = null;
        foreach (string filePath in Directory.EnumerateFiles(finalPath))
        {
            if (filePath.Contains(".meta")) continue;

            int indexOfAssets = filePath.IndexOf("Assets", StringComparison.Ordinal);
            if (indexOfAssets < 0)
            {
                continue;
            }

            string substring = filePath.Substring(indexOfAssets);
            if (string.IsNullOrEmpty(substring))
            {
                continue;
            }
#if UNITY_EDITOR_WIN
            substring = substring.Replace("\\", "/");
#endif

            asset = AssetDatabase.LoadAssetAtPath<AudioMappingConfiguration>(substring);

            if (asset != null)
            {
                break;
            }
        }

        m_isValidate   = asset != null;
        m_previousPath = path;

        return true;
    }
}
