using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Base.Services
{
    [Serializable]
    public struct AudioInfoConfig : ISearchFilterable
    {
        [FoldoutGroup("$FriendlyName")]
        [PropertyOrder(1)]
        [HideLabel, SerializeField]
        private AudioGenre m_audioGenre;
        
        [FoldoutGroup("$FriendlyName")]
        [ReadOnly]
        [PropertyOrder(0)]
        [DisplayAsString, HideLabel, SerializeField]
        private string m_audioId;
        
        [FoldoutGroup("$FriendlyName")]
        [HideLabel, PropertyOrder(2), SerializeField]
        private string m_audioKey;

        [FoldoutGroup("$FriendlyName"), SerializeField]
        [HorizontalGroup("$FriendlyName/Initialize Settings", Order = 4), LabelText("3D Audio")]
        private bool m_is3dAudio;
        
        [HorizontalGroup("$FriendlyName/Initialize Settings", Order = 4), SerializeField]
        private bool m_loop;
        
        [HorizontalGroup("$FriendlyName/Initialize Settings", Order = 4), SerializeField]
        private bool m_mute;
        
        [FoldoutGroup("$FriendlyName"), SerializeField]
        [HideLabel, PropertyOrder(3), AssetSelector(Filter = "t:audioClip", FlattenTreeView = true, DropdownTitle = "Select Audio")]
        private AudioClip m_audioClip;

        public string FriendlyName => $"{m_audioGenre}:{m_audioKey}";
        
        public AudioGenre AudioGenre => m_audioGenre;
        public string     AudioId    => m_audioId;
        public string     AudioKey   => m_audioKey;
        public bool       Is3dAudio  => m_is3dAudio;
        public bool       Loop       => m_loop;
        public bool       Mute       => m_mute;
        public AudioClip  AudioClip  => m_audioClip;

        public AudioInfoConfig(AudioGenre genre)
        {
            m_audioGenre = genre;
            m_audioId = Guid.NewGuid().ToString();
            m_audioKey = string.Empty;
            m_is3dAudio = false;
            m_loop = false;
            m_mute = false;
            m_audioClip = null;
        }
        public bool IsMatch(string searchString)
        {
            return m_audioKey.Contains(searchString);
        }
    }

    [CreateAssetMenu(menuName = "BasePackage/AudioGlobalConfig", order = 0)]
    public class AudioMappingConfiguration : SerializedScriptableObject
    {
        [ValueDropdown(nameof(GetPossibleAudioConfigValue), DrawDropdownForListElements = false)]
        [ListDrawerSettings(DraggableItems = true, ShowPaging = true, ShowFoldout = false)]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [SerializeField] private List<AudioInfoConfig> m_audioInfoConfigs;

        private IEnumerable GetPossibleAudioConfigValue()
        {
            List<ValueDropdownItem> dropdownItems = new List<ValueDropdownItem>()
            {
                new ValueDropdownItem("OneShot", new AudioInfoConfig(AudioGenre.OneShot)),
                new ValueDropdownItem("Music", new AudioInfoConfig(AudioGenre.Music)),
                new ValueDropdownItem("Sound", new AudioInfoConfig(AudioGenre.Sound))
            };

            return dropdownItems;
        }

        public List<AudioInfoConfig> GetAudioConfig(AudioGenre genre)
        {
            return m_audioInfoConfigs.Where(item => item.AudioGenre == genre).ToList();
        }

        public List<AudioInfoConfig> GetConfig() => m_audioInfoConfigs;
    }
}