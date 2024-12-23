using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class AudioEventBase
    {
        [SerializeField] private string m_eventName;
        [SerializeField, ValueDropdown(nameof(GetDropdownType))] private string m_audioType;
        [SerializeField, ValueDropdown(nameof(GetAudioKeyDropdown))] private string m_audioKey;

        public string EventName => m_eventName;

        public string AudioType => m_audioType;

        public string AudioKey => m_audioKey;


        private string GetSaveStringInEditor(string key)
        {
            #if UNITY_EDITOR
            return EditorPrefs.GetString(key, string.Empty);
            #endif

            return string.Empty;
        }

        private AudioDataContainer LoadAssetAtPath(string path)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<AudioDataContainer>(path);
#endif
            return null;
        }
        
#if ODIN_INSPECTOR

        private AudioDataContainer m_audioDataContainer;

        private AudioDataContainer GetAudioDataContainer()
        {
            if (m_audioDataContainer == null)
            {
                string path = GetSaveStringInEditor("OutputPath");
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                string fullPath = $"Assets{Path.DirectorySeparatorChar}{path}{Path.DirectorySeparatorChar}{nameof(AudioDataContainer)}.asset";
                m_audioDataContainer = LoadAssetAtPath(fullPath);
            }

            return m_audioDataContainer;
        }

        private IEnumerable GetDropdownType()
        {
            if (GetAudioDataContainer() == null) return new List<string>();

            return GetAudioDataContainer().AudioTypes;
        }

        private IEnumerable GetAudioKeyDropdown()
        {
            List<string> audioKeyDropdown = new List<string>();
            foreach (var audioAssetData in GetAudioDataContainer().DataCollection)
            {
                if (string.IsNullOrEmpty(m_audioType) || audioAssetData.Type.Equals(m_audioType))
                {
                    audioKeyDropdown.Add(audioAssetData.ObjectName);
                }
            }

            return audioKeyDropdown;
        }
        
#endif
    }

    public class OneShotAudioEvent : AudioEventBase
    {
        
    }
}