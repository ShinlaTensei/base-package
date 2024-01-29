#region Header
// Date: 11/01/2024
// Created by: Huynh Phong Tran
// File name: AudioAssetBase.cs
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Base.Core;
using Base.Pattern;
using Base.CustomAttribute;
using Base.Helper;
using Cysharp.Threading.Tasks;
using UnityEditor;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif
using UnityEngine;

namespace Base.Core
{
    [Serializable]
    public class AudioAssetData : ReferenceData, ISearchFilterable
    {
        /// <summary>
        /// Backing field for <see cref="Clips"/> used for serialization.
        /// </summary>
        [SerializeField, ValueDropdown("@this.ClipNameValueDropdown", ExcludeExistingValuesInList = true, DisableGUIInAppendedDrawer = true, 
        DrawDropdownForListElements = false), 
        ListItemSelector(nameof(OnClipSelectItem))]
        [Searchable, CustomValueDrawer(nameof(CustomClipListItemGUI))]
        private List<string> m_clips = new List<string>();
        
        /// <summary>
        /// Backing field of <see cref="CachedClip"/>
        /// </summary>
        private IDictionary<string, AudioClip> m_cachedClip;

#if UNITY_EDITOR
        [NonSerialized]
        private IEnumerable ClipNameValueDropdown = new ValueDropdownList<string>() { new ValueDropdownItem<string>("Default", "Default") };
        [NonSerialized]
        private Action<string> m_onClipSelectAction;

        private string CustomClipListItemGUI(string value, GUIContent label)
        {
            EditorGUILayout.LabelField(value);
            return value;
        }

        public void Subscribe(Action<string> listener)
        {
            m_onClipSelectAction += listener;
        }

        public void Remove()
        {
            m_onClipSelectAction = null;
        }

        public void SetClipNameValueDropdown(IEnumerable newValue)
        {
            ClipNameValueDropdown = newValue;
        }

        private void OnClipSelectItem(int index)
        {
            index = Mathf.Clamp(index, 0, m_clips.Count - 1);
            m_onClipSelectAction?.Invoke(m_clips[index]);
        }
#endif


        /// <summary>
        /// The audio clips addressable name associated with the asset.
        /// </summary>
        public List<string> Clips
        {
            get => m_clips;
            set => m_clips = value;
        }
        /// <summary>
        /// Cached the audio clip asset when loaded first time
        /// </summary>
        private IDictionary<string, AudioClip> CachedClip => m_cachedClip ?? new Dictionary<string, AudioClip>();
        /// <summary>
        /// Empty constructor with default member value
        /// </summary>
        public AudioAssetData() {}
        
        /// <summary>
        /// Constructor setting the members to the provided values.
        /// </summary>
        /// <param name="index">The data's unique index.</param>
        /// <param name="objectName">The data's name.</param>
        /// <param name="guid">The referenced object's guid.</param>
        public AudioAssetData(int index, string objectName, string guid) : base(index, objectName, guid)
        {
            
        }

        public AudioAssetData(AudioAssetData original) : base(original)
        {
            CopyData(original);
        }

        public async UniTask<AudioClip> Evaluate(AddressableManager addressableService, string clipId)
        {
            if (!Clips.Contains(clipId))
            {
                throw new NullReferenceException($"NullReference: There is no asset with ID {clipId} in data of type {Type}");
            }
            if (!CachedClip.TryGetValue(clipId, out AudioClip clip))
            {
                clip = await addressableService.LoadAsset<AudioClip>(clipId, 3);

                if (clip != null)
                {
                    CachedClip[clipId] = clip;
                }
            }

            return clip;
        }

        public bool IsMatch(string searchString)
        {
            return ObjectName.Contains(searchString);
        }

        public override void CopyData(StringData data)
        {
            base.CopyData(data);

            if (data is AudioAssetData audioAssetData)
            {
                m_clips = new List<string>();
                m_clips.AddRange(audioAssetData.Clips);
            }
        }
    }
    
    #if ODIN_INSPECTOR
    public record AudioSetting : IBaseSetting
    {
        /// <summary>
        /// The index of the <see cref="UnityEngine.Audio.AudioMixerGroup"/> to be assigned to the <see cref="AudioSource"/> playing the clips.
        /// </summary>
        [OdinSerialize] public int MixerGroup { get; set; } = -1;
        /// <summary>
        /// The base volume to play the asset's <see cref="AudioSource"/> with.
        /// </summary>
        [OdinSerialize, PropertyRange(0, 1f)] public float Volume { get; set; } = 1f;
        /// <summary>
        /// Predefine if the asset is looping
        /// </summary>
        [OdinSerialize] public bool Loop { get; set; } = false;
        /// <summary>
        /// Predefine if the asset is play in 3D environment
        /// </summary>
        [OdinSerialize] public bool Is3DAudio { get; set; } = false;
    }
    #endif
}