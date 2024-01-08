using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base.CustomAttribute;
using Base.Helper;
using Base.Services;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class AudioAssetManagerEditor : OdinEditorWindow
{
    [MenuItem("Base Framework/Audio Asset Manager")]
    private static void OpenWindow()
    {
        GetWindow<AudioAssetManagerEditor>().Show();
    }
    
    protected override void Initialize()
    {
        string path                   = EditorPrefs.GetString(OutputKey, string.Empty);
        m_previousPath = m_outputPath = path;
    }
    
    public override void SaveChanges()
    {
        EditorPrefs.SetString(OutputKey, m_outputPath);
        base.SaveChanges();
    }

    private void OnValidate()
    {
        if (m_mappingConfiguration != null)
        {
            PopulateConfigEditors();
            UpdateViewSelector(Mathf.Clamp(m_selectedIndex, 0, m_configKeyEditors.Count - 1));
        }
    }

#region Error Group
    [ShowInInspector, HideLabel]
    [DisplayAsString, OnInspectorGUI(Prepend = nameof(CustomErrorDrawer))]
    private string m_errorText = string.Empty;

    private void CustomErrorDrawer(string value, InspectorProperty inspectorProperty)
    {
        SirenixEditorGUI.BeginHorizontalPropertyLayout(null, out Rect labelRect);
        if (!string.IsNullOrEmpty(value))
        {
            string[] split = value.Split(":");
            
            if (split.Length == 2)
            {
                MessageType messageType = MessageType.None;
                if (split[0].Contains("Error"))
                {
                    messageType = MessageType.Error;
                }
                else if (split[0].Contains("Warning"))
                {
                    messageType = MessageType.Warning;
                }
                else if (split[0].Contains("Info"))
                {
                    messageType = MessageType.Info;
                }
                SirenixEditorGUI.MessageBox(split[1], messageType); 
            }
        }
        SirenixEditorGUI.EndHorizontalPropertyLayout();
    }

    private void ShowMessage(MessageType messageType, string message = "")
    {
        m_errorText     = $"{messageType.ToString()}:{message}";
        if (messageType is MessageType.None)
        {
            m_errorText     = string.Empty;
        }
    }
#endregion

#region Output Inspector
    [FolderPath(RequireExistingPath = true, ParentFolder = "Assets", AbsolutePath = true), BoxGroup("OutputZone", ShowLabel = false),
     HorizontalGroup("OutputZone/Split"), OnValueChanged(nameof(OnOuputDirectoryChanged), InvokeOnInitialize = true), ShowInInspector]
    private string m_outputPath;
    [ShowInInspector, ReadOnly, HorizontalGroup("OutputZone/Split", 0.3f), HideLabel]
    private AudioMappingConfiguration m_mappingConfiguration;

    private bool   m_isValidate   = false;
    private string m_previousPath = string.Empty;

    private readonly string OutputKey = "AudioManagerOutputPath";

    private void OnOuputDirectoryChanged(string path)
    {
        if (string.IsNullOrEmpty(path) || m_isValidate && m_previousPath.Equals(path))
        {
            hasUnsavedChanges = false;
            return;
        }
        m_isValidate = false;

        foreach (string filePath in Directory.EnumerateFiles(path))
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

            m_mappingConfiguration = AssetDatabase.LoadAssetAtPath<AudioMappingConfiguration>(substring);

            if (m_mappingConfiguration != null)
            {
                break;
            }
        }
        hasUnsavedChanges = !m_previousPath.Equals(path);
        m_isValidate      = m_mappingConfiguration != null;
        m_previousPath    = path;

        if (m_mappingConfiguration is null)
        {
            ShowMessage(MessageType.Error, "Configuration is NULL");
            return;
        }
        
        ShowMessage(MessageType.None);
        
        PopulateConfigEditors();
    }

    [UsedImplicitly] [Button(ButtonSizes.Small, Name = "Create"), HorizontalGroup("OutputZone/Split", 0.1f), VerticalGroup("OutputZone/Split/Buttons")]
    private void Create()
    {
        if (m_mappingConfiguration is null)
        {
            string relativePath = Path.GetRelativePath("Assets", m_outputPath);
            m_mappingConfiguration = ScriptableObject.CreateInstance<AudioMappingConfiguration>();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/{relativePath}/AudioConfiguration.asset");
            AssetDatabase.CreateAsset(m_mappingConfiguration, assetPath);
            AssetDatabase.SaveAssets();
            
            PopulateConfigEditors();
        }
    }
    [UsedImplicitly] [Button(ButtonSizes.Small, Name = "Delete"), VerticalGroup("OutputZone/Split/Buttons")]
    private void Delete()
    {
        if (m_mappingConfiguration is not null)
        {
            string assetPath = AssetDatabase.GetAssetPath(m_mappingConfiguration);
            if (!string.IsNullOrEmpty(assetPath) && assetPath.Contains(Path.GetRelativePath("Assets", m_outputPath)))
            {
                if (AssetDatabase.DeleteAsset(assetPath))
                {
                    AssetDatabase.SaveAssets();
                }

            }
        }
    }
#endregion

#region Audio Config Editor
    private int m_selectedIndex;

    [HorizontalGroup("AudioKeyGroup", 0.5f)]
    [BoxGroup("AudioKeyGroup/Box", false)]
    [VerticalGroup("AudioKeyGroup/Box/Vertical", 1f), OdinSerialize] 
    [ListDrawerSettings(DraggableItems = true, ShowFoldout = false, ShowPaging = false, 
                        HideAddButton = true, HideRemoveButton = true)]
     [ListItemSelector(nameof(SelectedItem)), CustomValueDrawer(nameof(CustomListDrawer))]
    private List<string> m_configKeyEditors = new List<string>();

    [HorizontalGroup("AudioKeyGroup", 0.5f), OdinSerialize, HideLabel]
    [BoxGroup("AudioKeyGroup/Box2", false)]
    private AudioConfigKeyEditor m_editedConfig;

    private IDictionary<string, AudioConfigKeyEditor> m_configEditors = new Dictionary<string, AudioConfigKeyEditor>();

    [Button("Add", ButtonSizes.Small, ButtonStyle.Box), GUIColor("green"), VerticalGroup("AudioKeyGroup/Box/Buttons"), 
     ResponsiveButtonGroup("AudioKeyGroup/Box/Buttons/Group")]
    private void AddToList()
    {
        m_configKeyEditors.Add("None");
    }
    
    [Button("Delete", ButtonSizes.Small, ButtonStyle.Box), GUIColor("red"), VerticalGroup("AudioKeyGroup/Box/Buttons"), 
     ResponsiveButtonGroup("AudioKeyGroup/Box/Buttons/Group")]
    private void DeleteFromList()
    {
        if (m_configKeyEditors.HasIndex(m_selectedIndex))
        {
            m_configKeyEditors.RemoveAt(m_selectedIndex);
        }
    }
    
    [Button("Duplicate", ButtonSizes.Small, ButtonStyle.Box), VerticalGroup("AudioKeyGroup/Box/Buttons"), 
     ResponsiveButtonGroup("AudioKeyGroup/Box/Buttons/Group")]
    private void Duplicate()
    {
        if (m_configKeyEditors.HasIndex(m_selectedIndex))
        {
            
        }
    }
    
    [Button(ButtonSizes.Large)]
    [HorizontalGroup("AudioKeyGroup", 0.5f), PropertySpace(15f, 15f)]
    [BoxGroup("AudioKeyGroup/Box2", false)]
    private void Save()
    {
        
    }

    private void CustomListDrawer(string value, GUIContent label, Func<GUIContent, bool> callNextDrawer, InspectorProperty inspectorProperty)
    {
        Rect rect = EditorGUILayout.BeginVertical();
        GUI.enabled = false;
        EditorGUILayout.TextField(value, EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        GUI.enabled = true;
    }

    private void SelectedItem(int index)
    {
        if (m_configKeyEditors.HasIndex(index))
        {
            m_selectedIndex = index;
            
            UpdateViewSelector(m_selectedIndex);
        }
    }

    private void PopulateConfigEditors()
    {
        List<AudioInfoConfig> configs = m_mappingConfiguration.GetConfig();
        m_configEditors.Clear();
        m_configKeyEditors.Clear();
        foreach (var config in configs)
        {
            m_configKeyEditors.Add(config.AudioKey);
            m_configEditors[config.AudioKey] = new AudioConfigKeyEditor
                                               {
                                                       audioClip = config.AudioClip,
                                                       audioKey  = config.AudioKey,
                                                       is3DSound = config.Is3dAudio,
                                                       isLoop = config.Loop,
                                                       isMute = config.Mute
                                               };
        }
    }

    private void UpdateViewSelector(int selectedIndex)
    {
        string key = m_configKeyEditors[selectedIndex];
        if (!m_configEditors.TryGetValue(key, out AudioConfigKeyEditor configKeyEditor))
        {
            
        }

        m_editedConfig = configKeyEditor;
    }
#endregion
    
}


internal struct AudioConfigKeyEditor
{
    [HorizontalGroup("ConfigKeyField", 0.3f), ShowInInspector, PropertySpace(15f, 15f)]
    public string audioKey;
    [AssetsOnly, AssetSelector(Filter = "t:audioClip", DropdownTitle = "Select Audio"), ShowInInspector, HideLabel]
    [PropertySpace(15f, 15f)]
    public AudioClip audioClip;

    [ShowInInspector, InlineProperty, PropertySpace(10f, 10f)]
    public bool is3DSound;
    [ShowInInspector, InlineProperty, PropertySpace(10f, 10f)]
    public bool isMute;
    [ShowInInspector, InlineProperty, PropertySpace(10f, 10f)]
    public bool isLoop;
    [ShowInInspector, PropertyRange(0f, 1f), PropertySpace(10f, 10f)]
    public float volume;
}
